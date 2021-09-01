using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;


namespace MEATaste.Views.FileOpen
{
    public class FileOpenPanelModel : INotifyPropertyChanged
    {
        private string fileNameLabel = string.Empty;

        public string FileNameLabel
        {
            get => fileNameLabel;
            set
            {
                if (fileNameLabel == value) return;
                fileNameLabel = value;
                OnPropertyChanged(nameof(FileNameLabel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
