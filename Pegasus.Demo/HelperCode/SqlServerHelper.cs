using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Pegasus.Demo
{
    public class SqlServerHelper
    {
        #region Fields and Properties

        private SqlConnection _sqlConn = null;
        internal string _connStr;
        private bool _connectionSuccessful;

        #endregion

        public SqlServerHelper(string serverName, string databaseName, string userName, string password)
        {
            _connStr = "Data Source=" + serverName + ";" +
                    "Initial Catalog=" + databaseName + ";" +
                    "User id=" + userName + ";" +
                    "Password=" + password + ";";
        }

        #region Connection Methods

        internal void OpenConnection()
        {
            _sqlConn = new SqlConnection();
            _sqlConn.ConnectionString = _connStr;
            try
            {
                _sqlConn.Open();
                _connectionSuccessful = true;
            }
            catch (Exception e)
            {
                _connectionSuccessful = false;
                Console.WriteLine("Exception in Opening SqlServer Connection: " + e.Message);
            }
        }

        internal void CloseConnection()
        {
            _sqlConn.Close();
        }

        #endregion

        #region Command Methods

        internal SqlCommand ExecuteCommand(string procName, List<SqlParameter> paramList)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = _sqlConn;
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter pl in paramList)
                {
                    cmd.Parameters.Add(pl);
                }
                return cmd;
            }
            catch (Exception e)
            {
                return cmd;
            }
        }

        public DataSet GetDataSet(string procName, List<SqlParameter> paramList)
        {
            DataSet ds = new DataSet();
            try
            {
                OpenConnection();
                SqlDataAdapter dataAdapter;
                if (_connectionSuccessful == true)
                {
                    dataAdapter = new SqlDataAdapter(ExecuteCommand(procName, paramList));
                    dataAdapter.Fill(ds);
                    CloseConnection();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        public DataSet GetDataSet(string tsql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmnd = new SqlCommand();
            cmnd.CommandText = tsql;
            try
            {
                OpenConnection();
                cmnd.Connection = _sqlConn;
                SqlDataAdapter dataAdapter;
                if (_connectionSuccessful == true)
                {
                    dataAdapter = new SqlDataAdapter(cmnd);
                    dataAdapter.Fill(ds);
                    CloseConnection();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                CloseConnection();
            }
            return ds;
        }

        #endregion
    }
}
