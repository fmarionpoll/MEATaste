using System;
using MEATaste.Infrastructure;

namespace MEATaste.Views.SwitchMaps
{
    class SwitchMapsPanelController
    {
        public SwitchMapsPanelModel Model { get; }
        private readonly ApplicationState state;
        private bool updatingFromState;

        public SwitchMapsPanelController(ApplicationState state, IEventSubscriber eventSubscriber)
        {
            this.state = state;
            Model = new SwitchMapsPanelModel();
            eventSubscriber.Subscribe(EventType.MeaExperimentChanged, OnExperimentChanged);
            eventSubscriber.Subscribe(EventType.PlayheadTimeChanged, OnPlayheadTimeChanged);
        }

        public void PlayheadScroll(double value)
        {
            if (updatingFromState) return;
            SetPlayheadSeconds(value);
        }

        private void OnExperimentChanged()
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null)
            {
                updatingFromState = true;
                Model.PlayheadEnabled = false;
                Model.PlayheadMaximum = 1;
                Model.PlayheadViewportSize = 0.01;
                Model.PlayheadValue = 0;
                updatingFromState = false;
                return;
            }

            var duration = DurationSeconds(experiment.DataAcquisitionSettings.nDataAcquisitionPoints,
                experiment.DataAcquisitionSettings.SamplingRate);
            updatingFromState = true;
            Model.PlayheadEnabled = duration > 0;
            Model.PlayheadMaximum = duration > 0 ? duration : 1;
            Model.PlayheadViewportSize = duration > 0 ? Math.Max(0.01, duration * 0.02) : 0.01;
            Model.PlayheadSmallChange = 0.01;
            Model.PlayheadLargeChange = 0.1;
            Model.PlayheadValue = Math.Clamp(state.PlayheadTime.Get(), 0, Model.PlayheadMaximum);
            updatingFromState = false;
        }

        private void OnPlayheadTimeChanged()
        {
            updatingFromState = true;
            Model.PlayheadValue = Math.Clamp(state.PlayheadTime.Get(), 0, Model.PlayheadMaximum);
            updatingFromState = false;
        }

        private void SetPlayheadSeconds(double seconds)
        {
            var experiment = state.MeaExperiment.Get();
            if (experiment == null)
            {
                state.PlayheadTime.Set(0);
                return;
            }

            var duration = DurationSeconds(experiment.DataAcquisitionSettings.nDataAcquisitionPoints,
                experiment.DataAcquisitionSettings.SamplingRate);
            state.PlayheadTime.Set(Math.Clamp(seconds, 0, duration));
        }

        private static double DurationSeconds(double nPoints, double samplingRate)
        {
            if (nPoints <= 0 || samplingRate <= 0) return 0;
            return (nPoints - 1) / samplingRate;
        }
    }
}
