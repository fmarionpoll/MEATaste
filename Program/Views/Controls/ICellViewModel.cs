using System.Windows.Input;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.Controls
{

    public interface ICellViewModel
    {
        ICell Cell { get; set; }
        ICommand ChangeCellStateCommand { get; }
    }
}