using System.Windows.Input;
using MEATaste.Views.Controls;

namespace MEATaste.Views.DynamicGrid
{
    internal class DesignCellViewModel : ICellViewModel
    {
        public ICell Cell { get; set; } = Controls.Models.Cell.Empty;
        public ICommand ChangeCellStateCommand { get; } = null;
    }
}
