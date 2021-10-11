using MEATaste.DataMEA.Models;

namespace MEATaste.Infrastructure
{
    public class ApplicationState
    {
        public StateProperty<MeaExperiment> MeaExperiment { get; }
        public StateProperty<ChannelsDictionary> DataSelected { get; }
        public StateProperty<AxesExtrema> AxesMaxMin { get; }
        public StateProperty<int> FilterProperty { get; }

        public ApplicationState(StatePropertyFactory statePropertyFactory)
        {
            MeaExperiment = statePropertyFactory.Create<MeaExperiment>(null, EventType.MeaExperimentChanged);
            DataSelected = statePropertyFactory.Create(new ChannelsDictionary(), EventType.SelectedChannelsChanged);
            AxesMaxMin = statePropertyFactory.Create<AxesExtrema>(null, EventType.AxesMaxMinChanged);
            FilterProperty = statePropertyFactory.Create(0, EventType.FilterChanged);
        }
    }
}
