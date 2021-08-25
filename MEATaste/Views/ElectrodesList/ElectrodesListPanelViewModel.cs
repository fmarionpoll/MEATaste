using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;

namespace MEATaste.Views.ElectrodesList
{
    public class ElectrodesListPanelViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Electrode> electrodesTable;

        public ObservableCollection<Electrode> ElectrodesTable
        {
            get => electrodesTable;
            set
            {
                if (electrodesTable == value) return;
                electrodesTable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ElectrodesTable)));
            }
        }

        // Binding must be set to One-Way for read-only properties
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

        private Electrode selectedElectrode;
        public Electrode SelectedElectrode
        {
            get => selectedElectrode;
            set
            {
                selectedElectrode = value;
                OnPropertyChanged(nameof(SelectedElectrode));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}