using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using HDF5.NET;
using ISA_L.PInvoke;

namespace MEATaste.DataMEA.MaxWell
{
    
    public static class DeflateHelper_Intel_ISA_L
    {
        private static int _stateLength = Unsafe.SizeOf<inflate_state>();

        private static System.Threading.ThreadLocal<IntPtr> _statePtr = new ThreadLocal<IntPtr>(
            valueFactory: DeflateHelper_Intel_ISA_L.CreateState,
            trackAllValues: false);

        public static unsafe Memory<byte> FilterFunc(H5FilterFlags flags, uint[] parameters, Memory<byte> buffer)
        {
            /* We're decompressing */
            if (flags.HasFlag(H5FilterFlags.Decompress))
            {
                var state = new Span<inflate_state>(_statePtr.Value.ToPointer(), _stateLength);

                ISAL.isal_inflate_reset(_statePtr.Value);

                buffer = buffer.Slice(2); // skip ZLIB header to get only the DEFLATE stream

                var length = 0;
                var inflated = new byte[buffer.Length /* minimum size to expect */];
                var sourceBuffer = buffer.Span;
                var targetBuffer = inflated.AsSpan();

                fixed (byte* ptrIn = sourceBuffer)
                {
                    state[0].next_in = ptrIn;
                    state[0].avail_in = (uint)sourceBuffer.Length;

                    while (true)
                    {
                        fixed (byte* ptrOut = targetBuffer)
                        {
                            state[0].next_out = ptrOut;
                            state[0].avail_out = (uint)targetBuffer.Length;

                            var status = ISAL.isal_inflate(_statePtr.Value);

                            if (status != inflate_return_values.ISAL_DECOMP_OK)
                                throw new Exception($"Error encountered while decompressing: {status}.");

                            length += targetBuffer.Length - (int)state[0].avail_out;

                            if (state[0].block_state != isal_block_state.ISAL_BLOCK_FINISH && /* not done */
                                state[0].avail_out == 0 /* and work to do */)
                            {
                                // double array size
                                var tmp = inflated;
                                inflated = new byte[tmp.Length * 2];
                                tmp.CopyTo(inflated, 0);
                                targetBuffer = inflated.AsSpan(start: tmp.Length);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                return inflated
                    .AsMemory(0, length);
            }

            /* We're compressing */
            else
            {
                throw new Exception("Writing data chunks is not yet supported by HDF5.NET.");
            }
        }

        private static unsafe IntPtr CreateState()
        {
            var ptr = Marshal.AllocHGlobal(Unsafe.SizeOf<inflate_state>());
            new Span<byte>(ptr.ToPointer(), _stateLength).Fill(0);

            return ptr;
        }
    }
}
