using System.Runtime.InteropServices;

namespace MEATaste.DataMEA.MaxWell
{
    public class FileMapElectrodeProperties
    {
        [StructLayout(LayoutKind.Explicit, Size = 24)]
        internal struct DatasetMembers
        {
            [FieldOffset(0)]
            public int channel;

            [FieldOffset(4)]
            public int electrode;

            [FieldOffset(8)]
            public double x;

            [FieldOffset(16)]
            public double y;
        };

    }


}
