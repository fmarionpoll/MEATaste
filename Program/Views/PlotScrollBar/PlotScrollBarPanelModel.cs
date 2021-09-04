
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;



namespace MEATaste.Views.PlotScrollBar 
{
    public class PlotScrollBarPanelModel : INotifyPropertyChanged
    {
        private string xFirst;
        private string xLast;
        private double viewPortWidth;
        private double scrollableMaximum;
        private double scrollableMinimum;
        private double scrollPosition;

        public string XFirst
        {
            get => xFirst;
            set
            {
                if (xFirst == value) return;
                xFirst = value;
                OnPropertyChanged(nameof(XFirst));
            }
        }

        public string XLast
        {
            get => xLast;
            set
            {
                if (xLast == value) return;
                xLast = value;
                OnPropertyChanged(nameof(XLast));
            }
        }

        public double ViewPortWidth
        {
            get => viewPortWidth;
            set
            {
                if (viewPortWidth == value) return;
                viewPortWidth = value;
                OnPropertyChanged(nameof(ViewPortWidth));
            }
        }

        public double ScrollableMaximum
        {
            get => scrollableMaximum;
            set
            {
                if (scrollableMaximum == value) return;
                scrollableMaximum = value;
                OnPropertyChanged(nameof(ScrollableMaximum));
            }
        }

        public double ScrollableMinimum
        {
            get => scrollableMinimum;
            set
            {
                if (scrollableMinimum == value) return;
                scrollableMinimum = value;
                OnPropertyChanged(nameof(ScrollableMinimum));
            }
        }

        public double ScrollPosition
        {
            get => scrollPosition;
            set
            {
                if (scrollPosition == value) return;
                scrollPosition = value;
                OnPropertyChanged(nameof(ScrollPosition));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

   
}
