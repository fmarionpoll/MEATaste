using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.SwitchGrids
{
    public class SwitchGridsPanelModel : INotifyPropertyChanged
    {
        private int nColumns;

        public int NColumns
        {
            get => nColumns;
            set
            {
                if (nColumns == value) return;
                nColumns = value;
                OnPropertyChanged(nameof(NColumns));
            }
        }

        private int nRows;

        public int NRows
        {
            get => nRows;
            set
            {
                if (nRows == value) return;
                nRows = value;
                OnPropertyChanged(nameof(NRows));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
