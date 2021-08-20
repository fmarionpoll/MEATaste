using Microsoft.Win32;
using System.Windows;
using MeaTaste.DataMEA.MaxWell;
using MeaTaste.DataMEA.Models;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using TasteMEA.DataMEA.Utilies;


namespace MeaTaste
{
    public partial class MainWindow : Window
    {

        public MeaExperiment MEAExpInfos;
        public ushort[,] allData = null;
        public ushort[] oneIntRow = null;

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
            OpenedFileLabel.Content = "File: " + MEAExpInfos.FileName;
            OpenedFileVersion.Content = "Version: " + MEAExpInfos.FileVersion;
            List<string> channelsList = MEAExpInfos.Descriptors.electrodeChannels();
            ListViewChannels.ItemsSource = channelsList;
        }

        private void UpdateChannelDisplayed(int selected)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {

                oneIntRow = FileReader.ReadAll_OneElectrodeAsInt(selected);

                var plt = wpfPlot1.Plot;
                plt.Clear();
                double[] myData = oneIntRow.Select(x => (double)x).ToArray();
                plt.AddSignal(myData, MEAExpInfos.Descriptors.SamplingRate);
                string title = $"channel: {selected} electrode: {MEAExpInfos.Descriptors.electrodes[selected].ElectrodeNumber} (position : x={MEAExpInfos.Descriptors.electrodes[selected].XCoordinates_um}, y={MEAExpInfos.Descriptors.electrodes[selected].YCoordinates_um} µm)";
                plt.Title(title);

                var plt2 = wpfPlot2.Plot;
                plt2.Clear();
                double[] derivRow = Filter.BDeriv(myData, myData.Length);
                plt2.AddSignal(derivRow, MEAExpInfos.Descriptors.SamplingRate, System.Drawing.Color.Orange);
                plt2.Title("derivRow");

                //double[] medianRow = Filter.BMedian(myData, myData.Length, 30);
                //plt2.AddSignal(medianRow, MEAExpInfos.Descriptors.SamplingRate, System.Drawing.Color.Green);
                //plt2.Title("derivRow + medianRow");

            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        
        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int selected = ListViewChannels.SelectedIndex;
            int selectedIndex = MEAExpInfos.Descriptors.electrodes[selected].ChannelNumber;
            UpdateChannelDisplayed(selectedIndex);
        }
    }

 
}

