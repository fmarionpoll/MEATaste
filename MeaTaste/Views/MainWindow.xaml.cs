using Microsoft.Win32;
using System.Windows;
using MeaTaste.DataMEA.Models;
using System.Collections.Generic;
using TasteMEA.DataMEA.MaxWell;
using static ApplicationState;


namespace MeaTaste
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public MainWindow(ApplicationState state, MeaFileReader meaFileReader)
        {
            this.state = state;
            this.meaFileReader = meaFileReader;
            InitializeComponent();
        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = openFileDialog.FileName;
                CurrentMeaExperiment = meaFileReader.ReadFile(fileName);
                UpdateLabelsWhenNewFileIsOpened();
            }
        }

        private void UpdateLabelsWhenNewFileIsOpened()
        {
            OpenedFileLabel.Content = "File: " + CurrentMeaExperiment.FileName;
            OpenedFileVersion.Content = "Version: " + CurrentMeaExperiment.FileVersion;
            var channelsList = CurrentMeaExperiment.Descriptors.GetChannelNumbers();
            ListViewChannels.ItemsSource = channelsList;
            InitElectrodesMap();
        }
    }
}

public class MeaFileReader
{
    public MeaExperiment ReadFile(string fileName)
    {
        if (FileReader.OpenReadMaxWellFile(fileName)
            && FileReader.IsFileReadableAsMaxWellFile())
        {
            return FileReader.GetExperimentInfos();
        }

        return null;
    }
}
