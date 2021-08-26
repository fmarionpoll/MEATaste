using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;


namespace MEATaste.Views.ElectrodesMap
{
    public class ElectrodesMapPanelModel : INotifyPropertyChanged
    {

        private int selectedElectrodeIndex;
        public int SelectedElectrodeIndex
        {
            get => selectedElectrodeIndex;

            set
            {
                selectedElectrodeIndex = value;
                OnPropertyChanged(nameof(SelectedElectrodeIndex));
            }
        }

        private int outlinedElectrodeIndex;
        public int OutlinedElectrodeIndex
        {
            get => outlinedElectrodeIndex;

            set
            {
                outlinedElectrodeIndex = value;
                OnPropertyChanged(nameof(OutlinedElectrodeIndex));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

