using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MEATaste.Views.CellView
{
    public partial class CellView : UserControl
    {
        public CellView()
        {
            InitializeComponent();
        }

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public static readonly DependencyProperty StartColorProperty =
            DependencyProperty.Register(
                "StartColor",
                typeof(Color),
                typeof(CellView));


        public Color FinishColor
        {
            get => (Color)GetValue(FinishColorProperty);
            set => SetValue(FinishColorProperty, value);
        }

        public static readonly DependencyProperty FinishColorProperty =
            DependencyProperty.Register(
                "FinishColor",
                typeof(Color),
                typeof(CellView));

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(
                "BorderColor",
                typeof(Color),
                typeof(CellView));

    }
}
