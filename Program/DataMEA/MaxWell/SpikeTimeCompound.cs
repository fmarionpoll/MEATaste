using System;
using System.Runtime.InteropServices;

namespace MEATaste.DataMEA.MaxWell
{
    class SpikeTimeCompound
    {
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        internal struct DatasetMembers
        {
            [FieldOffset(0)]
            public Int64 frameno;

            [FieldOffset(8)]
            public Int32 channel;

            [FieldOffset(12)]
            public float amplitude;
        };

    }
}
