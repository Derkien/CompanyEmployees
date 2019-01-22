using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CompanyEmployees.Entity
{
    public abstract class AbstractEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
