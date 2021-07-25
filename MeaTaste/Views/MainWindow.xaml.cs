﻿using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MeaTaste.Domain.Hdf5.Service;

namespace MeaTaste
{
    public partial class MainWindow : Window
    {
        private MainWindowModel _mainWindowModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowModel = new MainWindowModel();
            DataContext = _mainWindowModel;
        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string _fileName = openFileDialog.FileName;
                _mainWindowModel.OpenedFileLabelContent = _fileName; 
                int Idfile = FileReader.Hdf5OpenFile(_fileName);
            }
        }
    }

    public class MainWindowModel : INotifyPropertyChanged
    {
        private string _openedFileLabelContent = "";

        public string OpenedFileLabelContent
        {
            get { return _openedFileLabelContent; }
            set
            {
                _openedFileLabelContent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

