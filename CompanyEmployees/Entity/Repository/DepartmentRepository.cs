using CompanyEmployees.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        private DepartmentRepository(DbManager dbManager)
        {
            DbManager = dbManager;

            Adapter = new SqlDataAdapter();

            DepartmentList = new List<Department>() { };
        }

        /// <summary>
        /// Imitate request to db... Return all departments from db
        /// </summary>
        /// <returns></returns>
        public List<Department> GetDepartments()
        {
            var DataTable = new DataTable();
            var SelectCommand = DbManager.CreateSqlCommand();
            SelectCommand.CommandText = "SELECT * FROM Departments";
            Adapter.SelectCommand = SelectCommand;
            Adapter.Fill(DataTable);

            foreach (var item in DataTable.AsEnumerable())
            {
                DepartmentList.Add(
                    new Department(
                        (int)item["Id"],
                        item["Name"].ToString()
                    )
                );
            }

            return DepartmentList;
        }

        public Department GetDepartmentByName(string name)
        {
            var DataTable = new DataTable();
            var SelectCommand = DbManager.CreateSqlCommand();
            SelectCommand.CommandText = "SELECT * FROM Departments WHERE Name = @Name";
            SelectCommand.Parameters.AddWithValue("@Name", name);
            Adapter.SelectCommand = SelectCommand;
            Adapter.Fill(DataTable);

            if (DataTable.Rows.Count == 0)
            {
                throw new Exception($"Department with name: '{name}' not found");
            }

            DataRow row = DataTable.Rows[0];

            return new Department((int)row["Id"], row["Name"].ToString());
        }

        public Department GetDepartmentById(int id)
        {
            var DataTable = new DataTable();
            var SelectCommand = DbManager.CreateSqlCommand();
            SelectCommand.CommandText = "SELECT * FROM Departments WHERE Id = @Id";
            SelectCommand.Parameters.AddWithValue("@Id", id);
            Adapter.SelectCommand = SelectCommand;
            Adapter.Fill(DataTable);

            if (DataTable.Rows.Count == 0)
            {
                throw new Exception($"Department with id: '{id}' not found");
            }

            DataRow row = DataTable.Rows[0];

            return new Department((int)row["Id"], row["Name"].ToString());
        }

        public Department AddNewDepartment(Department department)
        {
            var InsertCommand = DbManager.CreateSqlCommand();
            InsertCommand.CommandText = "INSERT INTO Departments(Name) VALUES(@Name); SET @Id = @@IDENTITY; SELECT SCOPE_IDENTITY()";
            InsertCommand.Parameters.AddWithValue("@Name", department.Name);
            var param = InsertCommand.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
            param.Direction = ParameterDirection.Output;

            var DepartmentId = DbManager.ExecuteScalar(InsertCommand);

            return new Department(DepartmentId, department.Name);
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
                    instance = new DepartmentRepository(new DbManager("CompanyEmployees"));
                }
                return instance;
            }
        }

        public DbManager DbManager { get; }

        private SqlDataAdapter Adapter;
    }
}
