using System.Collections.Generic;
using System.Linq;

namespace MEATaste.DataMEA.Models
{
    public class ChannelsDictionary
    {

        public Dictionary<int, ushort[]> Channels { set; get; }
        
        public ChannelsDictionary()
        {
            Channels = new Dictionary<int, ushort[]>(); 
        }

        public ChannelsDictionary(Dictionary<int, ushort[]> dictionary)
        {
            Channels = dictionary;
        }

        public void TrimDictionaryToList(List<int> selectedChannels)
        {
            AddMissingChannelsToDictionary(selectedChannels);
            RemoveDictionaryKeysNotInList(selectedChannels);
        }

        public bool IsListEqualToStateSelectedItems(List<int> newSelectedChannels)
        {
            var stateSelectedChannels = Channels.Keys.ToList();
            if (stateSelectedChannels.Count == 0) return false;

            var set = new HashSet<int>(stateSelectedChannels);
            var equals = set.SetEquals(newSelectedChannels);
            return equals;
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
