using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MEATaste.Infrastructure
{
    public class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetProperty<T>(ref T innerValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(innerValue, value))
                return;

            innerValue = value;
            OnPropertyChanged(propertyName);
        }

        #endregion

    }
}
