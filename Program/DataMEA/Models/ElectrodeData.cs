

using System.Collections.Generic;

namespace MEATaste.DataMEA.Models
{
    public class ElectrodeData
    {
        public ElectrodeProperties Electrode { get; set; }
        public List<SpikeDetected> SpikeTimes { get; set; }
        public bool Selected { get; set; }

        public ElectrodeData(ElectrodeProperties ec)
        {
            Electrode = ec;
            Selected = false;
        }

    }
}
 