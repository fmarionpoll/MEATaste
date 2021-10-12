using System;
using System.Collections.Generic;


namespace MEATaste.DataMEA.MaxWell
{
    public partial class DelegateSelection : Selection
    {
        private Func<ulong[], IEnumerable<Step>> _walker;

        public DelegateSelection(ulong totalElementCount, Func<ulong[], IEnumerable<Step>> walker)
        {
            this.TotalElementCount = totalElementCount;
            _walker = walker;
        }

        public override ulong TotalElementCount { get; }

        public override IEnumerable<Step> Walk(ulong[] limits)
        {
            return _walker(limits);
        }
    }
}
