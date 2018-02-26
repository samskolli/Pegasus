using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISConnectionManager
    {
        #region ctor

        #region ctor helpers

        /// <summary>
        /// Assign the connection string and name
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        private void SetNameAndConnString(string connectionString, string name)
        {
            ConnectionString = connectionString;
            Name = name;
        }

        /// <summary>
        /// Helper method to add a connection mananger tothe project
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private ConnectionManager AddProjectConnectionManager(string connectionString, string name, string creationName, Project p)
        {
            var connMgr = p.ConnectionManagerItems.Add(creationName, name + ".conmgr").ConnectionManager;
            return connMgr;
        }

        /// <summary>
        /// Grabs the connection manager if already exists otherwise creates a new Connection Manager for a Project
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="p"></param>
        internal void SetProjectConnectionManager(string connectionString, string name, string creationName, Project p)
        {
            ConnectionManager = p.ConnectionManagerItems[name + ".conmgr"] == null ? AddProjectConnectionManager(connectionString, name, creationName, p) : p.ConnectionManagerItems[name + ".conmgr"].ConnectionManager;
            SetNameAndConnString(connectionString, name);
        }

        /// <summary>
        /// Grabs the connection manager if already exists otherwise creates a new Connection Manager for a Package.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="p"></param>
        private void SetPackageConnectionManager(string connectionString, string name, string creationName, Package p)
        {
            bool exists = false;
            foreach(var conn in p.Connections)
            {
                if (conn.Name == name)
                {
                    exists = true;
                    ConnectionManager = conn;
                }
            }

            if (!(exists))
            {
                ConnectionManager = p.Connections.Add(creationName);
                SetNameAndConnString(connectionString, name);
            }
        }

        #endregion

        /// <summary>
        /// Base ctor for internal use. If possible, do not use this outside of this class.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        internal ISConnectionManager(string connectionString, string name, string creationName, Project project = null, Package package = null)
        {
            if (project != null)
            {
                SetProjectConnectionManager(connectionString, name, creationName, project);
            }
            if (package != null)
            {
                SetPackageConnectionManager(connectionString, name, creationName, package);
            }
        }

        /// <summary>
        /// Base ctor for internal use. Use this internally outside of this class.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        internal ISConnectionManager(string connectionString, string name, string creationName, ISProject project = null, ISPackage package = null) 
            : this(connectionString, name, creationName, 
                  project == null ? null : project.Project, 
                  package == null ? null : package.Package)
        {

        }

        /// <summary>
        /// ctor that accepts a DTS Object Connection Manager. Primarily for implicit conversion.
        /// </summary>
        /// <param name="connMgr"></param>
        internal ISConnectionManager(ConnectionManager connMgr)
        {
            ConnectionManager = connMgr;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// The ConnectionManager object which we want to create/manipulate.
        /// </summary>
        internal ConnectionManager ConnectionManager { get; set; }

        #endregion

        #region Object Properties

        public string ID
        {
            get { return ConnectionManager.ID; }
        }

        /// <summary>
        /// Connection String to use
        /// </summary>
        public string ConnectionString
        {
            get { return ConnectionManager.ConnectionString; }
            set { ConnectionManager.ConnectionString = value; }
        }

        /// <summary>
        /// Creation Name refers to the type of the connection. Like OLEDB, ODBC etc.
        /// </summary>
        public string CreationName
        {
            get { return ConnectionManager.CreationName; }
        }

        /// <summary>
        /// Gets or sets a Boolean that indicates whether package validation is delayed until the package runs.
        /// </summary>
        public bool DelayValidation
        {
            get { return ConnectionManager.DelayValidation; }
            set { ConnectionManager.DelayValidation = value; }
        }

        /// <summary>
        /// Gets or sets the description of the ConnectionManager object.
        /// </summary>
        public string Description
        {
            get { return ConnectionManager.Description; }
            set { ConnectionManager.Description = value; }
        }

        /// <summary>
        /// Gets a value that indicates whether the connection manager has properties set through expressions.
        /// </summary>
        public bool HasExpressions
        {
            get { return ConnectionManager.HasExpressions; }
        }

        /// <summary>
        /// Gets an enumeration that describes the type of host that the connection is being used by. This property is read-only.
        /// </summary>
        public HostType HostType
        {
            get { return DtsUtility.EnumAToEnumB<DTSObjectHostType, HostType>(ConnectionManager.HostType); }
        }

        /// <summary>
        /// Gets or sets the name of the ConnectionManager object.
        /// </summary>
        public string Name
        {
            get { return ConnectionManager.Name; }
            set { ConnectionManager.Name = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the connection manager is in offline mode.
        /// </summary>
        public bool OfflineMode
        {
            get { return ConnectionManager.OfflineMode; }
            set { ConnectionManager.OfflineMode = value; }
        }

        /// <summary>
        /// Gets a collection of property objects for the ConnectionManager. This is the method of accessing the properties of the specific connection that is being hosted by the connection manager. 
        /// </summary>
        internal DtsProperties Properties
        {
            get { return ConnectionManager.Properties; }
        }

        /// <summary>
        /// Gets or sets the level of encryption performed on sensitive data that is contained in the package.
        /// </summary>
        public ProtectionLevel ProtectionLevel
        {
            get { return DtsUtility.EnumAToEnumB<DTSProtectionLevel, ProtectionLevel>(ConnectionManager.ProtectionLevel); }
            set { ConnectionManager.ProtectionLevel = DtsUtility.EnumAToEnumB<ProtectionLevel, DTSProtectionLevel>(value); }
        }

        /// <summary>
        /// Gets or sets the qualifier for the connection manager.
        /// </summary>
        public string Qualifier
        {
            get { return ConnectionManager.Qualifier; }
            set { ConnectionManager.Qualifier = value; }
        }

        //Scope
        /// <summary>
        /// DTSConnectionManagerScope CONTAINS:
        ///     Package
        ///     Project
        /// </summary>
        public ConnectionManagerScope Scope
        {
            get { return DtsUtility.EnumAToEnumB<DTSConnectionManagerScope, ConnectionManagerScope>(ConnectionManager.Scope); }
        }

        /// <summary>
        /// Returns a Boolean that indicates if the connection supports Microsoft Distributed Transaction Coordinator (MS DTC) transactions.
        /// </summary>
        public bool SupportsDTCTransactions
        {
            get { return ConnectionManager.SupportsDTCTransactions; }
        }

        //VariableDispenser
        internal Microsoft.SqlServer.Dts.Runtime.VariableDispenser VariableDispenser
        {
            get { return ConnectionManager.VariableDispenser; }
        }

        //Variables
        internal Variables Variables
        {
            get { return ConnectionManager.Variables; }
        }

        #endregion

        #region Methods to Get-Set Connection Manager Property Values

        /// <summary>
        /// Returns the value of a property in Connection Manager Properties. Returned value is of type Object
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetConnectionManagerPropertyValue(string propertyName)
        {
            return Properties[propertyName].GetValue(ConnectionManager);
        }

        /// <summary>
        /// Set the value of a property in Connection Manager Properties.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetConnectionManagerPropertyValue(string propertyName, object value)
        {
            Properties[propertyName].SetValue(ConnectionManager, value);
        }

        #endregion

        #region Expression Methods

        /// <summary>
        /// Sets the value for an Expression
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetExpression(string property, string value)
        {
            ConnectionManager.Properties[property].SetExpression(ConnectionManager, value);
        }

        /// <summary>
        /// Gets the value of an Expression
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetExpression(string property, string value)
        {
            return ConnectionManager.Properties[property].GetExpression(ConnectionManager);
        }

        #endregion

        #region Implicit Operator

        /// <summary>
        /// Implicit operator to convert a DTSOBject Connection Manager to this assmebly's version.
        /// </summary>
        /// <param name="connMgr"></param>
        public static implicit operator ISConnectionManager(ConnectionManager connMgr)
        {
            if (connMgr == null)
                return null;

            return new ISConnectionManager(connMgr);
        }

        #endregion

    }
}