using System.Windows.Input;

namespace MEATaste.Views.Controls
{

    public interface ICellViewModel
    {
        ICell Cell { get; set; }
        ICommand ChangeCellStateCommand { get; }
    }
}