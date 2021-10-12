using System.Collections.Generic;


namespace MEATaste.DataMEA.MaxWell
{
    public abstract class Selection
    {
        public abstract ulong TotalElementCount { get; }

        public abstract IEnumerable<Step> Walk(ulong[] limits);
    }
}
