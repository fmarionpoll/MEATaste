using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using MEATaste.Annotations;
using MEATaste.Views.ElectrodesHeatmap;
using MEATaste.Views.ElectrodesMap;

namespace MEATaste.Views.MainView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly ObservableCollection<TabItem> tabCollection;
        public ObservableCollection<TabItem> TabCollection => tabCollection;

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

        public MainViewModel()
        {
            tabCollection = new ObservableCollection<TabItem>
            {
                new() { Content = new ElectrodesMapPanelModel(), Header = "Map"},
                new() { Content= new ElectrodesHeatmapPanelModel(), Header="Heat map"}
            };
            SelectedTabIndex = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
    
}
