
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;



namespace MEATaste.Views.PlotScrollBar 
{
    public class PlotScrollBarPanelModel : INotifyPropertyChanged
    {
        private string xFirst;
        private string xLast;
        private double scrollViewPortSize;
        private double scrollMaximum;
        private double scrollMinimum;
        private double scrollValue;

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

        public double ScrollViewPortSize
        {
            get => scrollViewPortSize;
            set
            {
                if (scrollViewPortSize == value) return;
                scrollViewPortSize = value;
                OnPropertyChanged(nameof(ScrollViewPortSize));
            }
        }

        public double ScrollMaximum
        {
            get => scrollMaximum;
            set
            {
                if (scrollMaximum == value) return;
                scrollMaximum = value;
                OnPropertyChanged(nameof(ScrollMaximum));
            }
        }

        public double ScrollMinimum
        {
            get => scrollMinimum;
            set
            {
                if (scrollMinimum == value) return;
                scrollMinimum = value;
                OnPropertyChanged(nameof(ScrollMinimum));
            }
        }

        public double ScrollValue
        {
            get => scrollValue;
            set
            {
                if (scrollValue == value) return;
                scrollValue = value;
                OnPropertyChanged(nameof(ScrollValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

   
}
