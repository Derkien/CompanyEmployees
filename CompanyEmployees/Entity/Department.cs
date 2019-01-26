using System.ComponentModel;

namespace CompanyEmployees.Entity
{
    public class Department : AbstractEntity
    {
        private string _name;

        public Department(string name)
        {
            Name = name;
        }

        public Department(int id, string name)
        {
            Id = id;
            Name = name;
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

        public override string ToString()
        {
            return Name;
        }
    }
}
