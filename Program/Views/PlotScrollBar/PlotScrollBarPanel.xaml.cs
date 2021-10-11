using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;


namespace MEATaste.Views.PlotScrollBar
{
    public partial class PlotScrollBarPanel : UserControl
    {
        private readonly PlotScrollBarPanelController controller;
        
        public PlotScrollBarPanel()
        {
            controller = App.ServiceProvider.GetService<PlotScrollBarPanelController>();
            DataContext = controller!.Model;
            InitializeComponent();
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null)
                {
                    binding.UpdateSource();
                    controller.UpdateXAxisLimitsFromModelValues();
                }
            }
        }

        private void TextBox_KeyEnterUpdateZoom(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null)
                {
                    binding.UpdateSource();
                    controller.UpdateZoomFromModelValues();
                }
            }
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            controller.ScrollBar_Scroll(sender, e);
        }
    }
}
