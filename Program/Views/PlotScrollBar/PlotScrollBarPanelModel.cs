using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MEATaste.Annotations;
using MEATaste.DataMEA.Models;
using ScottPlot;


namespace MEATaste.Views.PlotScrollBar 
{
    public class PlotScrollBarPanelModel : INotifyPropertyChanged
    {
        private AxisLimits axisLimitsForDataPlot;
        private double _scrollValue;
        
        
        public AxisLimits AxisLimitsForDataPlot
        {
            get => axisLimitsForDataPlot;
            set
            {
                if (axisLimitsForDataPlot.XMin == value.XMin
                    && AxisLimitsForDataPlot.XMax == value.XMax
                    && AxisLimitsForDataPlot.YMin == value.YMin
                    && AxisLimitsForDataPlot.YMax == value.YMax
                ) return;
                axisLimitsForDataPlot = value;
                OnPropertyChanged(nameof(AxisLimitsForDataPlot));
            }
        }

        public double ScrollValue
        {
            get => _scrollValue;
            set
            {
                if (_scrollValue == value) return;
                _scrollValue = value;
                OnPropertyChanged(nameof(ScrollValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

   
}
