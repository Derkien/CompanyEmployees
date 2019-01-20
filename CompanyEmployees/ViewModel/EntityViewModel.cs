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
        private ViewModelCommand _AddDepartmentCommand;
        private ViewModelCommand _EditDepartmentCommand;
        private Employee _SelectedEmployee;
        private Department _SelectedDepartment;

        private UpsertDepartmentWindow UpsertDepartmentWindow;

        public EntityViewModel()
        {
            EmployeeRepository = EmployeeRepository.Instance;
            DepartmentRepository = DepartmentRepository.Instance;

            EmployeeCollection = new ObservableCollection<Employee>() { };
            DepartmentCollection = new ObservableCollection<Department>(DepartmentRepository.GetDepartments());
        }

        #region Employee operations
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

        public Employee SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set
            {
                _SelectedEmployee = value;
                MoveToSelectedDepartment = _SelectedEmployee?.Department ?? null;

                OnPropertyChanged("SelectedEmployee");
                OnPropertyChanged("MoveToSelectedDepartment");
                OnPropertyChanged("IsSelectedEmployeeEditable");
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

        private bool CanExecuteAddEmployeeAction(object parameter)
        {
            return _SelectedDepartment != null;
        }

        #endregion

        #region Departments operations

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

        public ICommand AddDepartmentCommand
        {
            get
            {
                return _AddDepartmentCommand ?? (_AddDepartmentCommand = new ViewModelCommand(AddDepartmentAction, CanAddDepartmentAction));
            }
        }

        private void AddDepartmentAction(object parameter)
        {
            Department Department = new Department("");

            InitUpsertDepartmentWindow(Department);

            if (UpsertDepartmentWindow.DialogResult == true)
            {
                DepartmentCollection.Insert(0, Department);
                DepartmentRepository.AddNewDepartment(Department);
                SelectedDepartment = Department;
            }
        }

        private void InitUpsertDepartmentWindow(Department Department)
        {
            var UpsertDepartmentViewModel = new UpsertDepartmentViewModel(Department);
            UpsertDepartmentViewModel.CancelEditing += CloseChildWindow;
            UpsertDepartmentViewModel.SaveChanges += SaveChangesWindow;

            UpsertDepartmentWindow = new UpsertDepartmentWindow
            {
                DataContext = UpsertDepartmentViewModel
            };
            UpsertDepartmentWindow.ShowDialog();
        }

        private UpsertDepartmentWindow ShowUpsertDepartmentVindow(UpsertDepartmentViewModel UpserDepartmentViewModel)
        {
            var UpsertDepartmentWindow = new UpsertDepartmentWindow
            {
                DataContext = UpserDepartmentViewModel
            };
            UpsertDepartmentWindow.ShowDialog();

            return UpsertDepartmentWindow;
        }

        private bool CanAddDepartmentAction(object parameter)
        {
            return true;
        }

        public ICommand EditDepartmentCommand
        {
            get
            {
                return _EditDepartmentCommand ?? (_EditDepartmentCommand = new ViewModelCommand(EditDepartmentAction, CanEditDepartmentAction));
            }
        }

        private void EditDepartmentAction(object parameter)
        {
            InitUpsertDepartmentWindow(_SelectedDepartment);
        }

        private void SaveChangesWindow(Department department)
        {
            SelectedDepartment = department;
            UpsertDepartmentWindow.DialogResult = true;
        }

        private void CloseChildWindow()
        {
            UpsertDepartmentWindow.DialogResult = false;
        }

        private bool CanEditDepartmentAction(object parameter)
        {
            return _SelectedDepartment != null;
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
