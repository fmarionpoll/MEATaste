using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelViewModel : INotifyPropertyChanged
    {
        private Electrode[] electrodes;

        public Electrode[] Electrodes
        {
            get => electrodes;
            set
            {
                if (electrodes == value) return;
                electrodes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Electrodes)));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}