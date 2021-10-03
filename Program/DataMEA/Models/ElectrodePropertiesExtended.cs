
using System.Runtime.CompilerServices;

namespace MEATaste.DataMEA.Models
{
    public record ElectrodePropertiesExtended
    {
        public int Channel;
        public int ElectrodeNumber;
        public double XuM;
        public double YuM;
        public int nspikes;
        
        public ElectrodePropertiesExtended (ElectrodeData electrodeData)
        {
            Channel = electrodeData.Electrode.Channel;
            ElectrodeNumber = electrodeData.Electrode.ElectrodeNumber;
            XuM = electrodeData.Electrode.XuM;
            YuM = electrodeData.Electrode.YuM;
            nspikes = 0;
            if (electrodeData.SpikeTimes != null)
                nspikes = electrodeData.SpikeTimes.Count;
        }
    }

    
}
