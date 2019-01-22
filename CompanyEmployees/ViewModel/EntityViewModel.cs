using CompanyEmployees.Entity;
using CompanyEmployees.Entity.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
        private ViewModelCommand _UpsertEmployeeCommand;
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
            return MoveToSelectedDepartment != null && SelectedEmployee != null && SelectedEmployee.Id > 0;
        }

        private void MoveEmployeeAction(object parameter)
        {
            Department TargetDepartment = parameter as Department;

            if (TargetDepartment.Name == _SelectedEmployee.Department.Name)
            {
                MessageBox.Show("Choose another department");

                return;
            }

            SelectedEmployee.Department = TargetDepartment;
            var UpdatedEmployee = EmployeeRepository.UpdateEmpoyee(SelectedEmployee);
            SelectedDepartment = TargetDepartment;
            
            SelectedEmployee = UpdatedEmployee;

            MessageBox.Show($"Employee {SelectedEmployee.Id} is moved! Switching view to {TargetDepartment.Name}...");

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
            SelectedEmployee = new Employee("", "", 18, _SelectedDepartment);
        }

        private bool CanExecuteAddEmployeeAction(object parameter)
        {
            return _SelectedDepartment != null;
        }

        public ICommand UpsertEmployeeCommand
        {
            get
            {
                return _UpsertEmployeeCommand ?? (_UpsertEmployeeCommand = new ViewModelCommand(UpsertEmployeeAction, CanExecuteUpsertEmployeeAction));
            }
        }

        private void UpsertEmployeeAction(object parameter)
        {
            var values = (object[])parameter;
            var (name, surname, age, department, isValid, message) = ValidatedValues(values);

            if (!isValid)
            {
                MessageBox.Show(message, "Data error", MessageBoxButton.OK);

                return;
            }

            SelectedEmployee.Name = name;
            SelectedEmployee.Surname = surname;
            SelectedEmployee.Age = age;
            SelectedEmployee.Department = department;

            if (SelectedEmployee.Id > 0)
            {
                SelectedEmployee = EmployeeRepository.UpdateEmpoyee(SelectedEmployee);
            }
            else
            {
                SelectedEmployee = EmployeeRepository.AddNewEmployee(SelectedEmployee);
            }

            RefreshEmployeeCollection();
        }

        private (string name, string surname, int age, Department department, bool isValid, string message) ValidatedValues(object[] values)
        {
            var message = new StringBuilder();
            var isValid = true;

            var name = values[0].ToString();
            if (name.Length == 0)
            {
                isValid = false;
                message.AppendLine("Invalid name");
            }
            var surname = values[1].ToString();
            if (surname.Length == 0)
            {
                isValid = false;
                message.AppendLine("Invalid surname");
            }

            if (!int.TryParse(values[2].ToString(), out int age) || age < 18)
            {
                isValid = false;
                message.AppendLine("Age cant be lower 18");
            }
            var department = values[3] as Department;

            return (name, surname, age, department, isValid, message.ToString());
        }

        private bool CanExecuteUpsertEmployeeAction(object parameter)
        {
            return _SelectedEmployee != null;
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
            try
            {
                EmployeeCollection = new ObservableCollection<Employee>(EmployeeRepository.GetEmployeeByDepartment(_SelectedDepartment));
            }
            catch (Exception)
            {
                EmployeeCollection = new ObservableCollection<Employee>() { };
            }

            OnPropertyChanged("EmployeeCollection");
        }

        public bool IsSelectedEmployeeEditable
        {
            get { return _SelectedEmployee != null; }
        }

        #endregion

        #region Departments operations

        public Department SelectedDepartment
        {
            get { return _SelectedDepartment; }
            set
            {
                var origValue = _SelectedDepartment;

                if (value == _SelectedDepartment)
                    return;

                _SelectedDepartment = value;

                if (SelectedEmployee != null && SelectedEmployee.Id == 0)
                {
                    var MessgeBoxResult = MessageBox.Show("You have unsaved data. Continue?", "Unsaved data", MessageBoxButton.OKCancel);
                    if (MessgeBoxResult == MessageBoxResult.Cancel)
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                _SelectedDepartment = origValue;
                                OnPropertyChanged("SelectedDepartment");
                            }),
                            DispatcherPriority.ContextIdle,
                            null
                        );

                        return;
                    }

                    SelectedEmployee = null;
                }
               
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
                SelectedDepartment = DepartmentRepository.AddNewDepartment(Department);
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
            RefreshDepartmentCollection();
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

        private void RefreshDepartmentCollection()
        {
            DepartmentCollection = new ObservableCollection<Department>(DepartmentRepository.GetDepartments());

            OnPropertyChanged("DepartmentCollection");
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
