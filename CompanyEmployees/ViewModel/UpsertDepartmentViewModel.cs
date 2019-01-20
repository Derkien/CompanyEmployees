using CompanyEmployees.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompanyEmployees.ViewModel
{
    class UpsertDepartmentViewModel : INotifyPropertyChanged
    {
        public event Action<Department> SaveChanges;
        public event Action CancelEditing;

        public Department Department { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ViewModelCommand _OkButtonCommand;
        private ViewModelCommand _CancelButtonCommand;

        public UpsertDepartmentViewModel(Department department)
        {
            Department = department;
        }

        public ICommand OkButtonCommand
        {
            get
            {
                return _OkButtonCommand ?? (_OkButtonCommand = new ViewModelCommand(
                    parameter => {
                        Department.Name = parameter.ToString();
                        SaveChanges?.Invoke(Department);
                    },
                    parameter => { return true; }
                    )
                );
            }
        }

        public ICommand CancelButtonCommand
        {
            get
            {
                return _CancelButtonCommand ?? (_CancelButtonCommand = new ViewModelCommand(
                    parameter => { CancelEditing(); },
                    parameter => { return true; }
                    )
                );
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
