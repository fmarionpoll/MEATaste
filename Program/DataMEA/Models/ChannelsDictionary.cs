using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEATaste.DataMEA.Models
{
    public class ChannelsDictionary
    {
        public ChannelsDictionary(Dictionary<int, ushort[]> channelsDictionary)
        {
            Channels = channelsDictionary;
        }

        public Dictionary<int, ushort[]> Channels { set; get; }

        public void TrimDictionaryToList(List<int> selectedChannels)
        {
            AddMissingChannelsToDictionary(selectedChannels);
            RemoveDictionaryKeysNotInList(selectedChannels);
        }
        
        private void AddMissingChannelsToDictionary(List<int> selectedChannels)
        {
            foreach (int channel in selectedChannels)
            {
                if (!Channels.ContainsKey(channel))
                    Channels.Add(channel, null);
            }
        }

        private void RemoveDictionaryKeysNotInList(List<int> selectedChannels)
        {
            foreach (var key in Channels.Keys)
            {
                if (selectedChannels.IndexOf(key) < 0)
                    Channels.Remove(key);
            }
        }
    }
}
