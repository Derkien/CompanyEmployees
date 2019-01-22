using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CompanyEmployees.Service
{
    internal class DbManager
    {
        private readonly string ConnectionString;
        private SqlConnection Connection;

        public DbManager(string connectionName)
        {
            //TODO: just example, need validation of existance... etc...
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            Connection = new SqlConnection(ConnectionString);
        }


        public int ExecuteScalar(SqlCommand sqlCommand)
        {
            try
            {
                Connection.Open();
                return Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (SqlException e)
            {
                throw new Exception($"Error while db request: {e.Message}");
            }
            finally
            {
                Connection.Close();
            }
        }

        public SqlCommand CreateSqlCommand()
        {
            return Connection.CreateCommand();
        }
    }
}
