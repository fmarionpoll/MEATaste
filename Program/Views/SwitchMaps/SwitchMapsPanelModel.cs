using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.SwitchMaps
{
    public class SwitchMapsPanelModel : INotifyPropertyChanged
    {
        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set
            {
                if (selectedTabIndex == value) return;
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        private double playheadValue;
        public double PlayheadValue
        {
            get => playheadValue;
            set
            {
                if (Math.Abs(playheadValue - value) < 1e-12) return;
                playheadValue = value;
                OnPropertyChanged(nameof(PlayheadValue));
            }
        }

        private double playheadMaximum = 1;
        public double PlayheadMaximum
        {
            get => playheadMaximum;
            set
            {
                if (Math.Abs(playheadMaximum - value) < 1e-12) return;
                playheadMaximum = value;
                OnPropertyChanged(nameof(PlayheadMaximum));
            }
        }

        private double playheadViewportSize = 0.01;
        public double PlayheadViewportSize
        {
            get => playheadViewportSize;
            set
            {
                if (Math.Abs(playheadViewportSize - value) < 1e-12) return;
                playheadViewportSize = value;
                OnPropertyChanged(nameof(PlayheadViewportSize));
            }
        }

        private double playheadSmallChange = 0.01;
        public double PlayheadSmallChange
        {
            get => playheadSmallChange;
            set
            {
                if (Math.Abs(playheadSmallChange - value) < 1e-12) return;
                playheadSmallChange = value;
                OnPropertyChanged(nameof(PlayheadSmallChange));
            }
        }

        private double playheadLargeChange = 0.1;
        public double PlayheadLargeChange
        {
            get => playheadLargeChange;
            set
            {
                if (Math.Abs(playheadLargeChange - value) < 1e-12) return;
                playheadLargeChange = value;
                OnPropertyChanged(nameof(PlayheadLargeChange));
            }
        }

        private bool playheadEnabled;
        public bool PlayheadEnabled
        {
            get => playheadEnabled;
            set
            {
                if (playheadEnabled == value) return;
                playheadEnabled = value;
                OnPropertyChanged(nameof(PlayheadEnabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
