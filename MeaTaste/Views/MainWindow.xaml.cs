using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MeaTaste.DataMEA.MaxWell;
using MeaTaste.DataMEA.Models;
using System.Diagnostics;
using ScottPlot;
using System.Linq;

namespace MeaTaste
{
    public partial class MainWindow : Window
    {

        public MeaExperiment MEAExpInfos;
        public ushort[] oneRow = null;

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
                    UpdateLabels();
                }

            }
        }

        private void UpdateLabels()
        {
            OpenedFileLabel.Visibility = Visibility.Visible;
            OpenedFileVersion.Visibility = Visibility.Visible;
            ListOfElectrodes.Visibility = Visibility.Visible;
            OpenedFileLabel.Content = "File: " + MEAExpInfos.FileName;
            OpenedFileVersion.Content = "Version: " + MEAExpInfos.FileVersion;
            ListOfElectrodes.ItemsSource = MEAExpInfos.Descriptors.electrodes;
        }
        
        private void ListOfElectrodes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selectedIndex = MEAExpInfos.Descriptors.electrodes[0].ChannelNumber;
            oneRow = FileReader.ReadAllElectrodeDataAsInt(selectedIndex);

            var plt = new Plot(600, 400);
            double[] myData = oneRow.Select(x => (double)x).ToArray();
            plt.AddSignal(myData, sampleRate: 20_000);
            plt.Title("Scott Plot of selected row");
        }
    }

 
}

