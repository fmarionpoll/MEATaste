
using System.Runtime.CompilerServices;

namespace MEATaste.DataMEA.Models
{
    public class ElectrodePropertiesExtended
    {
        public int Channel { get; set; }
        public int ElectrodeNumber { get; set; }
        public double XuM { get; set; }
        public double YuM { get; set; }
        public int Nspikes { get; set; }

        public ElectrodePropertiesExtended (ElectrodeData electrodeData)
        {
            Channel = electrodeData.Electrode.Channel;
            ElectrodeNumber = electrodeData.Electrode.ElectrodeNumber;
            XuM = electrodeData.Electrode.XuM;
            YuM = electrodeData.Electrode.YuM;
            Nspikes = 0;
            if (electrodeData.SpikeTimes != null)
                Nspikes = electrodeData.SpikeTimes.Count;
        }
    }

    
}
