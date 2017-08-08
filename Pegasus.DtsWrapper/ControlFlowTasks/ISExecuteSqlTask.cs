using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;
using System.Collections.Generic;
using System.Collections;

namespace Pegasus.DtsWrapper
{
    /// <summary>
    /// Stand-in for the Execute SQL Task
    /// </summary>
    public class ISExecuteSqlTask : ISTaskHost
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISExecuteSqlTask(string displayName, ISContainer immediateContainer):
            base("STOCK:SQLTask", displayName, immediateContainer)
        {
            SQLTask = TaskHost.InnerObject as ExecuteSQLTask;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal ExecuteSQLTask SQLTask { get; set; }

        #endregion

        #region Task Properties

        #region Result Set Bindings

        internal IDTSResultBindings ResultSetBindings_m
        {
            get { return SQLTask.ResultSetBindings; }
        }

        public List<ISResultSetBinding> ResultSetBindings
        {
            get
            {
                List<ISResultSetBinding> resultSetBindings = new List<ISResultSetBinding>();
                foreach (IDTSResultBinding rb in ResultSetBindings_m)
                {
                    resultSetBindings.Add(new ISResultSetBinding(this, rb.DtsVariableName, rb.ResultName));
                }
                return resultSetBindings;
            }
        }

        public ISResultSetBinding AddResultSetBinding(string dtsVariableName, object resultName)
        {
            return new ISResultSetBinding(this, dtsVariableName, resultName);
        }

        #endregion

        #region Parameter Bindings

        internal IDTSParameterBindings ParameterBindings_m
        {
            get { return SQLTask.ParameterBindings; }
        }

        public List<ISParameterBinding> ParameterBindings
        {
            get
            {
                List<ISParameterBinding> parameterBindings = new List<ISParameterBinding>();
                foreach (IDTSParameterBinding pb in ParameterBindings_m)
                {
                    parameterBindings.Add(new ISParameterBinding(this, pb.DataType, pb.DtsVariableName, 
                        DtsUtility.EnumAToEnumB<Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ParameterDirections, ParameterDirections>(pb.ParameterDirection), 
                        pb.ParameterName, pb.ParameterSize));
                }
                return parameterBindings;
            }
        }

        public ISParameterBinding AddParameterBinding(int dataType, string dtsVariableName, ParameterDirections paramDirection, object paramName, int paramSize)
        {
            return new ISParameterBinding(this, dataType, dtsVariableName, paramDirection, paramName, paramSize);
        }

        #endregion

        #region SQL Statement Source Type

        /// <summary>
        /// Gets or sets a value that indicates the type of source that contains the SQL statement that the Execute SQL task runs.
        /// </summary>
        public SqlStatementSourceType SqlStatementSourceType
        {
            get { return DtsUtility.EnumAToEnumB<Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.SqlStatementSourceType, SqlStatementSourceType>(SQLTask.SqlStatementSourceType); }
            set { SQLTask.SqlStatementSourceType = DtsUtility.EnumAToEnumB<SqlStatementSourceType, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.SqlStatementSourceType>(value); }
        }

        #endregion

        #region Result Set Type

        /// <summary>
        /// Gets or sets a value that indicates the type of result set returned by the SQL statement that the Execute SQL tasks runs.
        /// </summary>
        public ResultSetType ResultSetType
        {
            get { return DtsUtility.EnumAToEnumB<Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ResultSetType, ResultSetType>(SQLTask.ResultSetType); }
            set { SQLTask.ResultSetType = DtsUtility.EnumAToEnumB<ResultSetType, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ResultSetType>(value); }
        }

        #endregion

        #region SQL Statement Source

        /// <summary>
        /// Gets or sets the name of the source that contains the SQL statement that the Execute SQL task runs.
        /// </summary>
        public string SqlStatementSource
        {
            get { return SQLTask.SqlStatementSource; }
            set { SQLTask.SqlStatementSource = value; }
        }

        #endregion

        #region Is Stored Proc

        /// <summary>
        /// Gets or sets a Boolean that indicates whether the SQL statement specified by the Execute SQL task is a stored procedure.
        /// </summary>
        public bool IsStoredProcedure
        {
            get { return SQLTask.IsStoredProcedure; }
            set { SQLTask.IsStoredProcedure = value; }
        }

        #endregion

        #region Connection

        /// <summary>
        /// Gets or sets the name of the connection manager that connects to the relational database management system(RDBMS) where the Execute SQL task is run.
        /// </summary>
        public string Connection
        {
            get { return SQLTask.Connection; }
            set { SQLTask.Connection = value; }
        }

        #endregion

        #region By Pass Prepare

        /// <summary>
        /// Gets or sets a Boolean that indicates whether the Execute SQL task skips preparation of the statement when sending the SQL statement to the relational database management system (RDBMS).
        /// </summary>
        public bool BypassPrepare
        {
            get { return SQLTask.BypassPrepare; }
            set { SQLTask.BypassPrepare = value; }
        }

        #endregion

        #region Code Page

        /// <summary>
        /// Gets or sets the code page to use when translating variable values that are stored as Unicode wide chars to multi-bytes. Translation occurs either when storing values to or extracting values from databases.
        /// </summary>
        public uint CodePage
        {
            get { return SQLTask.CodePage; }
            set { SQLTask.CodePage = value; }
        }

        #endregion

        #region Time Out

        /// <summary>
        /// Gets or sets an integer that indicates the maximum number of seconds the task can run before timing out. 
        /// </summary>
        public uint TimeOut
        {
            get { return SQLTask.TimeOut; }
            set { SQLTask.TimeOut = value; }
        }

        #endregion

        #region TypeConversionMode

        /// <summary>
        /// Gets or sets the conversion mode used by the Execute SQL Task.
        /// </summary>
        public TypeConversionMode TypeConversionMode
        {
            get { return DtsUtility.EnumAToEnumB<Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode, TypeConversionMode>(SQLTask.TypeConversionMode); }
            set { SQLTask.TypeConversionMode = DtsUtility.EnumAToEnumB<TypeConversionMode, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode>(value); }
        }

        #endregion

        #endregion
    }

    // <summary>
    /// Stand in for the Parameter Binding used in configuration of Execute Sql Task
    /// </summary>
    public class ISParameterBinding
    {
        #region DtsObject

        internal IDTSParameterBinding ParameterBinding { get; set; }

        #endregion

        #region Dts Object Properties

        #region DataType

        /// <summary>
        /// Gets or sets the data type of the parameter binding.
        /// </summary>
        public int DataType
        {
            get { return ParameterBinding.DataType; }
            set { ParameterBinding.DataType = value; }
        }

        #endregion

        #region DtsVariableName

        /// <summary>
        /// Gets or sets the name of the SSIS package variable.
        /// </summary>
        public string DtsVariableName
        {
            get { return ParameterBinding.DtsVariableName; }
            set { ParameterBinding.DtsVariableName = value; }
        }

        #endregion

        #region ParameterDirection

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        public ParameterDirections ParameterDirection
        {
            get { return DtsUtility.EnumAToEnumB<Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ParameterDirections, ParameterDirections>(ParameterBinding.ParameterDirection); }
            set { ParameterBinding.ParameterDirection = DtsUtility.EnumAToEnumB<ParameterDirections, Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ParameterDirections>(value); }
        }

        #endregion

        #region ParameterName

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public object ParameterName
        {
            get { return ParameterBinding.ParameterName; }
            set { ParameterBinding.ParameterName = value; }
        }

        #endregion

        #region ParameterSize

        /// <summary>
        /// Gets or sets the size of the parameter.
        /// </summary>
        public int ParameterSize
        {
            get { return ParameterBinding.ParameterSize; }
            set { ParameterBinding.ParameterSize = value; }
        }

        #endregion

        #endregion

        #region ctor

        /// <summary>
        /// ctor accepts an ISExecuteSqlTask and various Paramater Binding Properties
        /// If a ParameterBinding with the passed name exists, then that is used; otherwise a new Parameter Binding is created
        /// </summary>
        /// <param name="sqlTask"></param>
        /// <param name="dataType"></param>
        /// <param name="dtsVariableName"></param>
        /// <param name="paramDirection"></param>
        /// <param name="paramName"></param>
        /// <param name="paramSize"></param>
        public ISParameterBinding(ISExecuteSqlTask sqlTask, int dataType, string dtsVariableName, ParameterDirections paramDirection, object paramName, int paramSize)
        {
            bool paramExists = false;
            IEnumerator parameters = sqlTask.ParameterBindings_m.GetEnumerator();
            while (parameters.MoveNext())
            {
                IDTSParameterBinding param = (parameters.Current as IDTSParameterBinding);
                if (param.ParameterName.ToString() == paramName.ToString())
                {
                    paramExists = true;
                    ParameterBinding = param;
                }
            }
            if (paramExists == false)
            {
                ParameterBinding = sqlTask.ParameterBindings_m.Add();
                ParameterName = paramName;
            }
            DataType = dataType;
            DtsVariableName = dtsVariableName;
            ParameterDirection = paramDirection;
            ParameterSize = paramSize;
        }

        #endregion
    }

    /// <summary>
    /// Stand in for the Result Binding used in configuration of Execute Sql Task
    /// </summary>
    public class ISResultSetBinding
    {
        #region DtsObject

        internal IDTSResultBinding ResultSetBinding { get; set; }

        #endregion

        #region Properties

        #region DtsVariableName

        /// <summary>
        ///  Gets or sets the name of the SSIS package variable.
        /// </summary>
        public string DtsVariableName
        {
            get { return ResultSetBinding.DtsVariableName; }
            set { ResultSetBinding.DtsVariableName = value; }
        }

        #endregion

        #region ResultName

        /// <summary>
        ///  Gets or sets the name of the result.
        /// </summary>
        public object ResultName
        {
            get { return ResultSetBinding.ResultName; }
            set { ResultSetBinding.ResultName = value; }
        }

        #endregion

        #endregion

        #region ctor

        /// <summary>
        /// ctor accepts an ISExecuteSqlTask, result name and variable name
        /// If a ResultBinding with the same name exists, then that is reused otherwise a new Result Binding is created
        /// </summary>
        /// <param name="sqlTask"></param>
        /// <param name="dtsVariableName"></param>
        /// <param name="resultName"></param>
        public ISResultSetBinding(ISExecuteSqlTask sqlTask, string dtsVariableName, object resultName)
        {
            bool resultSetExists = false;
            IEnumerator resultSets = sqlTask.ResultSetBindings_m.GetEnumerator();
            while (resultSets.MoveNext())
            {
                IDTSResultBinding resultSet = (resultSets.Current as IDTSResultBinding);
                if (resultSet.ResultName.ToString() == ResultName.ToString())
                {
                    resultSetExists = true;
                    ResultSetBinding = resultSet;
                }
            }
            if (resultSetExists == false)
            {
                ResultSetBinding = sqlTask.ResultSetBindings_m.Add();
                ResultName = resultName;
            }
            DtsVariableName = dtsVariableName;
        }

        #endregion
    }
}
