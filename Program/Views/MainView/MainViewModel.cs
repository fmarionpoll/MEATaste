using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;


namespace MEATaste.Views.MainView
{
    public class MainViewModel : INotifyPropertyChanged
    {
        
        public MainViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
    
}
