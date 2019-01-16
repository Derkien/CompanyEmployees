using CompanyEmployees.Entity;
using CompanyEmployees.Entity.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CompanyEmployees.ViewModel
{
    public class EntityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Employee> EmployeeCollection { get; private set; }
        public ObservableCollection<Department> DepartmentCollection { get; private set; }
        public Department MoveToSelectedDepartment { get; set; }

        private EmployeeRepository EmployeeRepository { get; }
        private DepartmentRepository DepartmentRepository { get; }

        private ViewModelCommand _MoveEmployeeCommand;
        private ViewModelCommand _AddEmployeeCommand;
        private Employee _SelectedEmployee;
        private Department _SelectedDepartment;

        public EntityViewModel()
        {
            EmployeeRepository = new EmployeeRepository();
            DepartmentRepository = new DepartmentRepository();

            EmployeeCollection = new ObservableCollection<Employee>() { };
            DepartmentCollection = new ObservableCollection<Department>(DepartmentRepository.GetDepartments());
        }

        public ICommand MoveEmployeeCommand
        {
            get
            {
                return _MoveEmployeeCommand ?? (_MoveEmployeeCommand = new ViewModelCommand(MoveEmployeeAction, CanExecuteMoveEmployeeAction));
            }
        }

        private bool CanExecuteMoveEmployeeAction(object parameter)
        {
            return MoveToSelectedDepartment != null && _SelectedEmployee != null;
        }

        private void MoveEmployeeAction(object parameter)
        {
            Department TargetDepartment = parameter as Department;

            if (TargetDepartment.Name == _SelectedEmployee.Department.Name)
            {
                MessageBox.Show("Choose another department");

                return;
            }

            _SelectedEmployee.Department = TargetDepartment;

            SelectedDepartment = TargetDepartment;

            MessageBox.Show($"Employee {_SelectedEmployee.Id} is moved! Switching view to {TargetDepartment.Name}...");

            OnPropertyChanged("SelectedEmployee");
        }

        public ICommand AddEmployeeCommand
        {
            get
            {
                return _AddEmployeeCommand ?? (_AddEmployeeCommand = new ViewModelCommand(AddEmployeeAction, CanExecuteAddEmployeeAction));
            }
        }

        private void AddEmployeeAction(object parameter)
        {
            Employee Employee = new Employee("", "", 18, _SelectedDepartment);
            EmployeeCollection.Insert(0, Employee);
            EmployeeRepository.AddNewEmployee(Employee);

            SelectedEmployee = Employee;
        }

        private bool CanExecuteAddEmployeeAction(object parameter)
        {
            return _SelectedDepartment != null;
        }

        public Employee SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set
            {
                _SelectedEmployee = value;
                OnPropertyChanged("SelectedEmployee");
                OnPropertyChanged("IsSelectedEmployeeEditable");
            }
        }

        public Department SelectedDepartment
        {
            get { return _SelectedDepartment; }
            set
            {
                _SelectedDepartment = value;
                OnPropertyChanged("SelectedDepartment");

                RefreshEmployeeCollection();
            }
        }

        private void RefreshEmployeeCollection()
        {
            EmployeeCollection = new ObservableCollection<Employee>(EmployeeRepository.GetEmployeeByDepartment(_SelectedDepartment));

            OnPropertyChanged("EmployeeCollection");
        }

        public bool IsSelectedEmployeeEditable
        {
            get { return _SelectedEmployee != null; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
