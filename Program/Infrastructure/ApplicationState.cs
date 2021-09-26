using MEATaste.DataMEA.Models;

namespace MEATaste.Infrastructure
{
    public class ApplicationState
    {
        public StateProperty<MeaExperiment> CurrentExperiment { get; }
        public StateProperty<ElectrodeProperties> CurrentElectrode { get; }
        public StateProperty<ElectrodeDataBuffer> ElectrodeBuffer { get; }
        public StateProperty<AxesExtrema> AxesMaxMin { get; }

        public ApplicationState(StatePropertyFactory statePropertyFactory)
        {
            CurrentExperiment = statePropertyFactory.Create<MeaExperiment>(null, EventType.CurrentExperimentChanged);
            CurrentElectrode = statePropertyFactory.Create<ElectrodeProperties>(null, EventType.SelectedElectrodeChanged);
            ElectrodeBuffer = statePropertyFactory.Create<ElectrodeDataBuffer>(null, EventType.ElectrodeRecordLoaded);
            AxesMaxMin = statePropertyFactory.Create<AxesExtrema>(null, EventType.AxesMaxMinChanged);
        }
    }
}
