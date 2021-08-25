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

        //// Binding must be set to One-Way for read-only properties
        //public int SelectedElectrodesTableIndex
        //{
        //    get
        //    {
        //        if (ElectrodesTable is {Count: > 0})
        //            return ElectrodesTable.IndexOf(SelectedElectrode);
        //        else
        //            return -1;
        //    }
            
        //}

        //private Electrode selectedElectrode;
        //public Electrode SelectedElectrode
        //{
        //    get => selectedElectrode;
        //    set
        //    {
        //        selectedElectrode = value;
        //        OnPropertyChanged(nameof(SelectedElectrode));
        //        OnPropertyChanged(nameof(SelectedElectrodesTableIndex));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}