using System.ComponentModel;
using MEATaste.Infrastructure;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.Controls.Models
{
    public class Cell : BaseNotifyPropertyChanged, ICell
    {
        public Cell()
        {
            this.SetDefaultValues();
        }

        public Cell(bool state)
            : this()
        {
            State = state;
        }

        #region Implementation ICell

        private bool state;

        [DefaultValue(false)]
        public bool State
        {
            get => state;
            set => SetProperty(ref state, value);
        }

        #endregion

        public static ICell Empty => new Cell();
    }
}
