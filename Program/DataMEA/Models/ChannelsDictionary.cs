using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public ChannelsDictionary(List<int> keyList)
        {
            Channels = new Dictionary<int, ushort[]>();
            AddMissingChannelsToDictionary(keyList);
        }

        public void TrimDictionaryToList(List<int> selectedChannels)
        {
            Trace.WriteLine("ChannelsDictionary (start): channels selected= " + Channels.Count);
            RemoveDictionaryKeysNotInList(selectedChannels);
            AddMissingChannelsToDictionary(selectedChannels);
            Trace.WriteLine("ChannelsDictionary (end): channels selected= " + Channels.Count);
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
            foreach (var channel in selectedChannels.Where(channel => !Channels.ContainsKey(channel)))
            {
                Channels.Add(channel, null);
            }
        }

        private void RemoveDictionaryKeysNotInList(List<int> selectedChannels)
        {
            foreach (var (key, _) in Channels)
            {
                if (selectedChannels.IndexOf(key) < 0)
                    Channels.Remove(key);
            }
        }

    }
}
