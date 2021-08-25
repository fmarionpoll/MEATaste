﻿using MEATaste.DataMEA.MaxWell;
using MEATaste.Infrastructure;

namespace MEATaste.Views.MainWindow
{
    public partial class MainWindow
    {
        private readonly ApplicationState _applicationState;
        private readonly MeaFileReader _meaFileReader;

        public MainWindow(MeaFileReader meaFileReader, ApplicationState applicationState)
        {
            _meaFileReader = meaFileReader;
            _applicationState = applicationState;
            DataContext = _applicationState;
            InitializeComponent();
        }

        // TODO
        //private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    int selected = ListViewChannels.SelectedIndex;
        //    Electrode electrode = state.CurrentMeaExperiment.Descriptors.Electrodes[selected];
        //    UpdateSelectedElectrode(electrode);
        //}

    }
}


