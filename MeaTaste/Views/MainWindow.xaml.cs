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
        public int[] oneRow = null;

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
                    OpenedFileLabel.Visibility = Visibility.Visible;
                    OpenedFileVersion.Visibility = Visibility.Visible;
                    ListOfElectrodes.Visibility = Visibility.Visible;
                }

            }
        }

       

        
        private void OpenedFileLabel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (OpenedFileLabel.Visibility == Visibility.Visible)
            {
                OpenedFileLabel.Content = "File: " + MEAExpInfos.FileName;
            }
        }

        private void OpenedFileVersion_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (OpenedFileVersion.Visibility == Visibility.Visible)
            {
                OpenedFileVersion.Content = "Version: " + MEAExpInfos.FileVersion;
            }
        }

        private void ListOfElectrodes_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ListOfElectrodes.Visibility == Visibility.Visible)
            {
                ListOfElectrodes.ItemsSource = MEAExpInfos.Descriptors.electrodes;

            }
        }

        private void ListOfElectrodes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selectedIndex = MEAExpInfos.Descriptors.electrodes[0].ChannelNumber; // ListOfElectrodes.SelectedIndex;
            oneRow = FileReader.ReadAllElectrodeDataAsInt(selectedIndex);
        }
    }

 
}

