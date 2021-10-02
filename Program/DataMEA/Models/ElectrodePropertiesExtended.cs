
using System.Runtime.CompilerServices;

namespace MEATaste.DataMEA.Models
{
    public class ElectrodePropertiesExtended
    {
        private int Channel;
        private int ElectrodeNumber;
        private double XuM;
        private double YuM;
        private int nspikes;
        
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
