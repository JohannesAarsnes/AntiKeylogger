using System.ComponentModel;

namespace AntiKeyloggerUI.Auxiliary
{
    public abstract class BindingProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged( string propertyName )
        {   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
