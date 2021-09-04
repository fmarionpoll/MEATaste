
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;



namespace MEATaste.Views.PlotScrollBar 
{
    public class PlotScrollBarPanelModel : INotifyPropertyChanged
    {
        private string xFirst;
        private string xLast;
        private double sbViewPortSize;
        private double sbMaximum;
        private double sbMinimum;
        private double sbValue;

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

        public double SBViewPortSize
        {
            get => sbViewPortSize;
            set
            {
                if (sbViewPortSize == value) return;
                sbViewPortSize = value;
                OnPropertyChanged(nameof(SBViewPortSize));
            }
        }

        public double SBMaximum
        {
            get => sbMaximum;
            set
            {
                if (sbMaximum == value) return;
                sbMaximum = value;
                OnPropertyChanged(nameof(SBMaximum));
            }
        }

        public double SBMinimum
        {
            get => sbMinimum;
            set
            {
                if (sbMinimum == value) return;
                sbMinimum = value;
                OnPropertyChanged(nameof(SBMinimum));
            }
        }

        public double SBValue
        {
            get => sbValue;
            set
            {
                if (sbValue == value) return;
                sbValue = value;
                OnPropertyChanged(nameof(SBValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

   
}
