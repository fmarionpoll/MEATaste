using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MeaTaste.DataMEA.MaxWell;
using MeaTaste.DataMEA.Models;

namespace MeaTaste
{
    public partial class MainWindow : Window
    {

        public MeaExperiment MEAExpInfos;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string _fileName = openFileDialog.FileName;
                if (FileReader.OpenReadMaxWellFile(_fileName)
                    && FileReader.IsFileReadableAsMaxWellFile())
                {
                    MEAExpInfos = FileReader.GetExperimentInfos();

                    OpenedFileLabel.Content = "File: " + FileReader.FileName;
                    OpenedFileLabel.Visibility = Visibility.Visible;
                    OpenedFileVersion.Content = "Version: "+ FileReader.FileVersion;
                    OpenedFileVersion.Visibility = Visibility.Visible;

                    bool flag = FileReader.ReadAllElectrodeDataAsInt(MEAExpInfos, 0);
                }

            }
        }
    }

 
}

