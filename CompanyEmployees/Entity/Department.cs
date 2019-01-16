namespace CompanyEmployees.Entity
{
    public class Department
    {
        public Department(string name)
        {
            Name = name;
        }

        public int Id { get { return GetHashCode(); } }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
