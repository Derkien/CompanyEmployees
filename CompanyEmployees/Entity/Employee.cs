namespace CompanyEmployees.Entity
{
    public class Employee : AbstractEntity
    {
        private string _name;
        private string _surname;
        private int _age;
        private Department _department;

        public Employee(string name, string surname, int age, Department department)
        {
            Name = name;
            Surname = surname;
            Age = age;
            Department = department;
        }

        public Employee(int id, string name, string surname, int age, Department department)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Age = age;
            Department = department;
        }

        public int Id { get; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged("Age");
            }
        }
        public Department Department
        {
            get { return _department; }
            set
            {
                _department = value;
                OnPropertyChanged("Department");
            }
        }
    }
}
