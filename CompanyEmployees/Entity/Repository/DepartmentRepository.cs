using System.Collections.Generic;
using System.Linq;

namespace CompanyEmployees.Entity.Repository
{
    internal class DepartmentRepository
    {
        public const string DepartmentNameA = "Department_a";
        public const string DepartmentNameB = "Department_b";
        public const string DepartmentNameC = "Department_c";
        public const string DepartmentNameD = "Department_d";

        private List<Department> DepartmentList;

        /// <summary>
        /// Imitate request to db... Return all departments from db
        /// </summary>
        /// <returns></returns>
        public List<Department> GetDepartments()
        {
            if (DepartmentList != null)
            {
                return DepartmentList;
            }

            DepartmentList = new List<Department>() { };
            DepartmentList.Add(new Department(DepartmentNameA));
            DepartmentList.Add(new Department(DepartmentNameB));
            DepartmentList.Add(new Department(DepartmentNameC));
            DepartmentList.Add(new Department(DepartmentNameD));

            return DepartmentList;
        }

        public Department GetDepartmentByName(string name)
        {
            return GetDepartments().FirstOrDefault(d => d.Name == name);
        }

        public void AddNewDepartment(Department department)
        {
            DepartmentList.Insert(0, department);
        }

        /// <summary>
        /// Singleton, while db not implemented 
        /// </summary>
        private static DepartmentRepository instance;
        public static DepartmentRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DepartmentRepository();
                }
                return instance;
            }
        }
    }
}
