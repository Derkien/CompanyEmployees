namespace CompanyEmployees.Entity
{
    public class Employee
    {
        public Employee(string name, string surname, int age, Department department)
        {
            Name = name;
            Surname = surname;
            Age = age;
            Department = department;
        }

        public int Id { get { return GetHashCode(); } }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public Department Department { get; set; }
    }
}
