using Microsoft.SqlServer.Dts.Runtime;
using runTimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;


namespace Pegasus.DtsWrapper
{
    #region Database Connection Managers

    public class ISDatabaseParametersConnectionManager : ISConnectionManager
    {
        #region ctor

        /// <summary>
        /// Base ctor for createing Database Connection Managers. used internally within this assembly.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        internal ISDatabaseParametersConnectionManager(string connectionString, string name, string creationName, ISProject project, ISPackage package)
            : base(connectionString, name, creationName, project, package)
        {

        }

        #endregion

        #region Object Model object

        /// <summary>
        /// The wrapped DTS Object
        /// </summary>
        internal runTimeWrapper.IDTSConnectionManagerDatabaseParameters100 Connection
        {
            get
            {
                return ConnectionManager.InnerObject as runTimeWrapper.IDTSConnectionManagerDatabaseParameters100;
            }
            set { }
        }

        #endregion

        #region Properties

        #region InitialCatalog

        public string InitialCatalog
        {
            get { return (string)GetConnectionManagerPropertyValue("InitialCatalog"); }
            set { SetConnectionManagerPropertyValue("InitialCatalog", value); }
        }

        #endregion

        #region Password

        public string Password
        {
            //get { return (string)GetConnectionManagerPropertyValue("Password"); }
            set { SetConnectionManagerPropertyValue("Password", value); }
        }

        #endregion

        #region RetainSameConnection

        public bool RetainSameConnection
        {
            get { return (bool)GetConnectionManagerPropertyValue("RetainSameConnection"); }
            set { SetConnectionManagerPropertyValue("RetainSameConnection", value); }
        }

        #endregion

        #region UserName

        public string UserName
        {
            get { return (string)GetConnectionManagerPropertyValue("UserName"); }
            set { SetConnectionManagerPropertyValue("UserName", value); }
        }

        #endregion

        #region ServerName

        public string ServerName
        {
            get { return (string)GetConnectionManagerPropertyValue("ServerName"); }
            set { SetConnectionManagerPropertyValue("ServerName", value); }
        }

        #endregion

        #endregion
    }

    public class ISOledbConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a OleDb Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISOledbConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "OLEDB", project, package)
        {

        }

        #endregion
    }

    public class ISODBCConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a ODBC Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISODBCConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "ODBC", project, package)
        {

        }
                
        #endregion
    }

    public class ISAdoConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a ADO Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISAdoConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "ADO", project, package)
        {

        }

        #endregion
    }

    public class ISAdoNetConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a ADO.Net Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISAdoNetConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "ADO.NET", project, package)
        {

        }

        #endregion
    }

    public class ISOLAPConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a OLAP Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISOLAPConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "MSOLAP90", project, package)
        {

        }

        #endregion
    }

    public class ISSqlMobileConnectionManager : ISDatabaseParametersConnectionManager
    {
        #region ctor

        /// <summary>
        /// ctor for creating a Sql Mobile Connection Manager
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISSqlMobileConnectionManager(
            string connectionString,
            string name,
            ISProject project = null,
            ISPackage package = null) : base(connectionString, name, "SQLMOBILE", project, package)
        {

        }

        #endregion
    }

    #endregion
}
