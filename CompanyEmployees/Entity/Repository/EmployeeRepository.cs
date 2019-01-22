using CompanyEmployees.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CompanyEmployees.Entity.Repository
{
    internal class EmployeeRepository
    {
        public const string EmployeeNameAvatar = "Avatar";
        public const string EmployeeNameBabatar = "Babatar";
        public const string EmployeeNameGoga = "Goga";
        public const string EmployeeNameZaza = "Zaza";

        private EmployeeRepository(DbManager dbManager)
        {
            DbManager = dbManager;

            Adapter = new SqlDataAdapter();
        }

        /// <summary>
        /// Imitate request to db... Return all employees from db
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetEmployees(DataTable dataTable)
        {
            var EmployeeList = new List<Employee>() { };

            foreach (var item in dataTable.AsEnumerable())
            {
                EmployeeList.Add(
                    new Employee(
                        int.Parse(item["Id"].ToString()),
                        item["Name"].ToString(),
                        item["Surname"].ToString(),
                        int.Parse(item["Age"].ToString()),
                        DepartmentRepository.Instance.GetDepartmentById((int)item["Department"])
                    )
                );
            }

            return EmployeeList;
        }

        public Employee GetEmployeeByName(string name)
        {
            var DataTable = new DataTable();
            var SelectCommand = DbManager.CreateSqlCommand();
            SelectCommand.CommandText = "SELECT * FROM Employees WHERE Name = @Name";
            SelectCommand.Parameters.AddWithValue("@Name", name);
            Adapter.SelectCommand = SelectCommand;
            Adapter.Fill(DataTable);

            return GetEmployees(DataTable).First();
        }

        public Employee AddNewEmployee(Employee employee)
        {
            var InsertCommand = DbManager.CreateSqlCommand();
            InsertCommand.CommandText = "INSERT INTO Employees(Name, Surname, Age, Department) VALUES(@Name, @Surname, @Age, @Department); SET @Id = @@IDENTITY; SELECT SCOPE_IDENTITY()";
            InsertCommand.Parameters.AddWithValue("@Name", employee.Name);
            InsertCommand.Parameters.AddWithValue("@Surname", employee.Surname);
            InsertCommand.Parameters.AddWithValue("@Age", employee.Age);
            InsertCommand.Parameters.AddWithValue("@Department", employee.Department.Id);
            var param = InsertCommand.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
            param.Direction = ParameterDirection.Output;

            var EmployeeId = DbManager.ExecuteScalar(InsertCommand);

            return new Employee(EmployeeId, employee.Name, employee.Surname, employee.Age, employee.Department);
        }

        public IEnumerable<Employee> GetEmployeeByDepartment(Department department)
        {
            var DataTable = new DataTable();
            var SelectCommand = DbManager.CreateSqlCommand();
            SelectCommand.CommandText = "SELECT * FROM Employees WHERE Department = @Department";
            SelectCommand.Parameters.AddWithValue("@Department", department.Id);
            Adapter.SelectCommand = SelectCommand;
            Adapter.Fill(DataTable);

            if (DataTable.Rows.Count == 0)
            {
                throw new Exception($"Employees with department: '{department}' not found");
            }

            return GetEmployees(DataTable);
        }

        public Employee UpdateEmpoyee(Employee employee)
        {
            var UpdateCommand = DbManager.CreateSqlCommand();
            UpdateCommand.CommandText = "UPDATE Employees SET Name = @Name, Surname = @Surname, Age = @Age, Department = @Department WHERE Id = @Id";
            UpdateCommand.Parameters.AddWithValue("@Name", employee.Name);
            UpdateCommand.Parameters.AddWithValue("@Surname", employee.Surname);
            UpdateCommand.Parameters.AddWithValue("@Age", employee.Age);
            UpdateCommand.Parameters.AddWithValue("@Department", employee.Department.Id);
            var param = UpdateCommand.Parameters.AddWithValue("@Id", employee.Id);
            // param.SourceVersion = DataRowVersion.Original;

            DbManager.ExecuteScalar(UpdateCommand);

            return employee;
        }

        /// <summary>
        /// Singleton, while db not implemented 
        /// </summary>
        private static EmployeeRepository instance;
        public static EmployeeRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmployeeRepository(new DbManager("CompanyEmployees"));
                }
                return instance;
            }
        }

        public DbManager DbManager { get; }

        private SqlDataAdapter Adapter;
    }
}
