using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using ScottPlot.WPF;

namespace MEATaste.Views.MapVoltage
{
    public class MapVoltageModel : INotifyPropertyChanged
    {
        private WpfPlot plotControl;
        private string timeText = "0.000";
        private string scaleAmplitudeText = "1.000";
        private bool autoScale = true;
        private bool loop;
        private bool isRunning;
        private string runStopLabel = "Run";

        public WpfPlot PlotControl
        {
            get => plotControl;
            set
            {
                if (plotControl == value) return;
                plotControl = value;
                OnPropertyChanged(nameof(PlotControl));
            }
        }

        public string TimeText
        {
            get => timeText;
            set
            {
                if (timeText == value) return;
                timeText = value;
                OnPropertyChanged(nameof(TimeText));
            }
        }

        public string ScaleAmplitudeText
        {
            get => scaleAmplitudeText;
            set
            {
                if (scaleAmplitudeText == value) return;
                scaleAmplitudeText = value;
                OnPropertyChanged(nameof(ScaleAmplitudeText));
            }
        }

        public bool AutoScale
        {
            get => autoScale;
            set
            {
                if (autoScale == value) return;
                autoScale = value;
                OnPropertyChanged(nameof(AutoScale));
            }
        }

        public bool Loop
        {
            get => loop;
            set
            {
                if (loop == value) return;
                loop = value;
                OnPropertyChanged(nameof(Loop));
            }
        }

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (isRunning == value) return;
                isRunning = value;
                RunStopLabel = value ? "Stop" : "Run";
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public string RunStopLabel
        {
            get => runStopLabel;
            set
            {
                if (runStopLabel == value) return;
                runStopLabel = value;
                OnPropertyChanged(nameof(RunStopLabel));
            }
        }

        public MapVoltageModel()
        {
            PlotControl = new WpfPlot();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
