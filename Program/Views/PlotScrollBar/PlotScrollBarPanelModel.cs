
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;



namespace MEATaste.Views.PlotScrollBar 
{
    public class PlotScrollBarPanelModel : INotifyPropertyChanged
    {
        private string xFirst;
        private string xLast;

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

   
}
