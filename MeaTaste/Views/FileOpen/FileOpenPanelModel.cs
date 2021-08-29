using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;


namespace MEATaste.Views.FileOpen
{
    public class FileOpenPanelModel : INotifyPropertyChanged
    {
        private string fileVersionLabel = string.Empty;
        private string fileNameLabel = string.Empty;

        public string FileVersionLabel
        {
            get => fileVersionLabel;
            set
            {
                if (fileVersionLabel == value) return;
                fileVersionLabel = value;
                OnPropertyChanged(nameof(FileVersionLabel));
            }
        }

        public string FileNameLabel
        {
            get => fileNameLabel;
            set
            {
                if (fileNameLabel == value) return;
                fileNameLabel = value;
                OnPropertyChanged(nameof(FileNameLabel));
                if (NewHdf5FileIsLoadedAction != null)
                    NewHdf5FileIsLoadedAction();
            }
        }

        public static event Action NewHdf5FileIsLoadedAction;
        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
