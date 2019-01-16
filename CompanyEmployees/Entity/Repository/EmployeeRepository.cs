using System.Collections.Generic;
using System.Linq;

namespace CompanyEmployees.Entity.Repository
{
    internal class EmployeeRepository
    {
        private DepartmentRepository DepartmentRepository = new DepartmentRepository();

        public const string EmployeeNameAvatar = "Avatar";
        public const string EmployeeNameBabatar = "Babatar";
        public const string EmployeeNameGoga = "Goga";
        public const string EmployeeNameZaza = "Zaza";

        private List<Employee> EmployeeList;

        /// <summary>
        /// Imitate request to db... Return all employees from db
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetEmployees()
        {
            if (EmployeeList != null)
            {
                return EmployeeList;
            }

            EmployeeList = new List<Employee>() { };
            EmployeeList.Add(new Employee(
                EmployeeNameAvatar,
                "Baba",
                23,
                DepartmentRepository.GetDepartmentByName(DepartmentRepository.DepartmentNameA))
                );
            EmployeeList.Add(new Employee(
                EmployeeNameBabatar,
                "Gaga",
                25,
                DepartmentRepository.GetDepartmentByName(DepartmentRepository.DepartmentNameB))
                );
            EmployeeList.Add(new Employee(
                EmployeeNameGoga,
                "Publishvili",
                31,
                DepartmentRepository.GetDepartmentByName(DepartmentRepository.DepartmentNameC))
                );
            EmployeeList.Add(new Employee(
                EmployeeNameZaza,
                "Atata",
                35,
                DepartmentRepository.GetDepartmentByName(DepartmentRepository.DepartmentNameD))
                );

            return EmployeeList;
        }

        public Employee GetEmployeeByName(string name)
        {
            return GetEmployees().FirstOrDefault(e => e.Name == name);
        }

        public void AddNewEmployee(Employee employee)
        {
            EmployeeList.Insert(0, employee);
        }

        public IEnumerable<Employee> GetEmployeeByDepartment(Department department)
        {
            var DepartmentEmployees = from e in GetEmployees() where e.Department.Name == department.Name select e;

            return DepartmentEmployees;
        }
    }
}
