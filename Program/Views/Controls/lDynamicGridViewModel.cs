using System.Collections.ObjectModel;
using System.Windows.Media;


namespace MEATaste.Views.Controls
{
    public interface IDynamicGridViewModel

    {
        ObservableCollection<ObservableCollection<ICellViewModel>> Cells { get; }
        int GridWidth { get; }
        int GridHeight { get; }
        Color StartColor { get; set; }
        Color FinishColor { get; set; }
        Color BorderColor { get; set; }
    }
}