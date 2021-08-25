﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using MEATaste.Annotations;


namespace MEATaste.Views.FileOpen
{
    public class FileOpenPanelViewModel : INotifyPropertyChanged
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileVersionLabel)));
            }
        }

        public string FileNameLabel
        {
            get => fileNameLabel;
            set
            {
                if (fileNameLabel == value) return;
                fileNameLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileNameLabel)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
