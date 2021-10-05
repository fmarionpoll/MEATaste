using System.Collections.Generic;
using MEATaste.DataMEA.Models;

namespace MEATaste.Infrastructure
{
    public class ApplicationState
    {
        public StateProperty<MeaExperiment> MeaExperiment { get; }
        public StateProperty<List<int>> ListSelectedChannels { get; set; }
        public StateProperty<AxesExtrema> AxesMaxMin { get; set; }

        public ApplicationState(StatePropertyFactory statePropertyFactory)
        {
            MeaExperiment = statePropertyFactory.Create<MeaExperiment>(null, EventType.CurrentExperimentChanged);
            ListSelectedChannels = statePropertyFactory.Create<List<int>>(null, EventType.SelectedChannelsChanged);
            AxesMaxMin = statePropertyFactory.Create<AxesExtrema>(null, EventType.AxesMaxMinChanged);
        }
    }
}
