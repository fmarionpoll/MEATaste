using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MeaTaste.DataMEA.MaxWell;
using MeaTaste.DataMEA.Models;
using System.Diagnostics;
using ScottPlot.WPF;
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
            int selected = ListOfElectrodes.SelectedIndex;
            int selectedIndex = MEAExpInfos.Descriptors.electrodes[selected].ChannelNumber;
            oneRow = FileReader.ReadAllElectrodeDataAsInt(selectedIndex);

            var plt = wpfPlot1.Plot;
            double[] myData = oneRow.Select(x => (double)x).ToArray();
            plt.Clear();
            plt.AddSignal(myData, sampleRate: 20_000);
            string title = $"channel: {selectedIndex} electrode: {MEAExpInfos.Descriptors.electrodes[selected].ElectrodeNumber}" ;
            plt.Title(title );
        }
    }

 
}

