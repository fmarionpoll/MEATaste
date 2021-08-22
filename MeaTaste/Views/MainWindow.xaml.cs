using System.Windows;
using Microsoft.Win32;
using TasteMEA.DataMEA.MaxWell;

namespace TasteMEA.Views
{
    public partial class MainWindow
    {
        private readonly ApplicationState state;
        private readonly MeaFileReader meaFileReader;

        public MainWindow(MeaFileReader meaFileReader, ApplicationState state)
        {
            this.meaFileReader = meaFileReader;
            this.state = state;
            InitializeComponent();
        }

       
    }
}


