using MEATaste.DataMEA.Models;

namespace MEATaste.Infrastructure
{
    public class ApplicationState
    {
        public StateProperty<MeaExperiment> MeaExperiment { get; }
        public StateProperty<ElectrodeProperties> CurrentElectrode { get; }
        public StateProperty<AxesExtrema> AxesMaxMin { get; }

        public ApplicationState(StatePropertyFactory statePropertyFactory)
        {
            MeaExperiment = statePropertyFactory.Create<MeaExperiment>(null, EventType.CurrentExperimentChanged);
            CurrentElectrode = statePropertyFactory.Create<ElectrodeProperties>(null, EventType.SelectedElectrodeChanged);
            AxesMaxMin = statePropertyFactory.Create<AxesExtrema>(null, EventType.AxesMaxMinChanged);
        }
    }
}
