using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;

namespace MEATaste.Views.FileOpenPanel
{
    public class FileOpenPanelViewModel : INotifyPropertyChanged
    {
        private string fileVersionLabel;
        private string fileNameLabel;

        public string FileVersionLabel
        {
            get => fileVersionLabel;
            set
            {
                if (fileVersionLabel != value)
                {
                    fileVersionLabel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileVersionLabel"));
                }
            }
        }

        public string FileNameLabel
        {
            get => fileNameLabel;
            set
            {
                if (fileNameLabel != value)
                {
                    fileNameLabel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileNameLabel"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
