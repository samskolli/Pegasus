using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.DtsWrapper
{
    /// <summary>
    /// The data types that a Variable can be.
    /// </summary>
    public enum VariableDataType
    {
        Boolean,
        Byte,
        Char,
        Datetime,
        Decimal,
        Double,
        Int16,
        Int32,
        Int64,
        Sbyte,
        Single,
        String,
        UInt32,
        UInt64,
        Object
    }

    /// <summary>
    /// Stand--in for DTSPrecedenceEvalOp
    /// </summary>
    public enum PrecedenceEvalOp
    {
        Expression = 1,
        Constraint = 2,
        ExpressionAndConstraint = 3,
        ExpressionOrConstraint = 4
    }

    /// <summary>
    /// Stand-in for DTSExecResult
    /// </summary>
    public enum ExecResult
    {
        Success = 0,
        Failure = 1,
        Completion = 2,
        Canceled = 3
    }

    /// <summary>
    /// Stand-in for DTSObjectHostType
    /// </summary>
    public enum ObjectHostType
    {
        Task = 0,
        ConnectionManager = 1,
        LogProvider = 2,
        ForEachEnumerator = 3
    }

    /// <summary>
    /// Stand-in for DTSPropertyKind
    /// </summary>
    public enum PropertyKind
    {
        Other = 0,
        VariableReadOnly = 1,
        VariableReadWrite = 2,
        Connection = 3,
        Sensitive = 4
    }

    /// <summary>
    /// Stand-in for DTSCheckpointUsage
    /// </summary>
    public enum CheckpointUsage
    {
        Never = 0,
        IfExists = 1,
        Always = 2
    }

    /// <summary>
    /// Stand-in for DTSPriorityClass
    /// </summary>
    public enum PriorityClass
    {
        Default = 0,
        AboveNormal = 1,
        Normal = 2,
        BelowNormal = 3,
        Idle = 4
    }

    /// <summary>
    /// Stand-in for DTSPackageType
    /// </summary>
    public enum PackageType
    {
        Default = 0,
        DTSWizard = 1,
        DTSDesigner = 2,
        SQLReplication = 3,
        DTSDesigner100 = 5,
        SQLDBMaint = 6
    }

    /// <summary>
    /// Stand-in for DTSProtectionLevel
    /// </summary>
    public enum ProtectionLevel
    {
        DontSaveSensitive = 0,
        EncryptSensitiveWithUserKey = 1,
        EncryptSensitiveWithPassword = 2,
        EncryptAllWithPassword = 3,
        EncryptAllWithUserKey = 4,
        ServerStorage = 5
    }

    /// <summary>
    /// Stand-in for DTSTargetServerVersion
    /// </summary>
    public enum TargetServerVersion
    {
        SQLServer2012 = 110,
        SQLServer2014 = 120,
        SQLServer2016 = 130,
        Latest = 130
    }

    /// <summary>
    /// Stand-in for DTSObjectType
    /// </summary>
    public enum ObjectType
    {
        OT_UNSPECIFIED = 0,
        OT_SOURCEADAPTER = 1,
        OT_DESTINATIONADAPTER = 2,
        OT_TRANSFORM = 4,
        OT_COMPONENT = 8,
        OT_COMPONENTVIEWER = 16,
        OT_PATH = 32,
        OT_INPUT = 64,
        OT_VIRTUALINPUT = 128,
        OT_OUTPUT = 256,
        OT_INPUTCOLUMN = 512,
        OT_OUTPUTCOLUMN = 1024,
        OT_VIRTUALINPUTCOLUMN = 2048,
        OT_PROPERTY = 4096,
        OT_RUNTIMECONNECTION = 8192,
        OT_EXTERNALMETADATACOLUMN = 16384
    }

    /// <summary>
    /// Lists the roles that a ScriptComponent can be used as
    /// </summary>
    public enum ScriptComponentType
    {
        Source = 1,
        Transform = 2,
        Destination = 3
    }

    /// <summary>
    /// Row Disposition
    /// </summary>
    public enum RowDisposition
    {
        RD_IgnoreFailure = 0,
        RD_RedirectRow = 1,
        RD_FailComponent = 2,
        RD_NotUsed = 3
    }

    /// <summary>
    /// Stand-in for DataType
    /// </summary>
    public enum SSISDataType
    {
        DT_EMPTY = 0,
        DT_NULL = 1,
        DT_I2 = 2,
        DT_I4 = 3,
        DT_R4 = 4,
        DT_R8 = 5,
        DT_CY = 6,
        DT_DATE = 7,
        DT_BOOL = 11,
        DT_DECIMAL = 14,
        DT_I1 = 16,
        DT_UI1 = 17,
        DT_UI2 = 18,
        DT_UI4 = 19,
        DT_I8 = 20,
        DT_UI8 = 21,
        DT_FILETIME = 64,
        DT_GUID = 72,
        DT_BYTES = 128,
        DT_STR = 129,
        DT_WSTR = 130,
        DT_NUMERIC = 131,
        DT_DBDATE = 133,
        DT_DBTIME = 134,
        DT_DBTIMESTAMP = 135,
        DT_DBTIME2 = 145,
        DT_DBTIMESTAMPOFFSET = 146,
        DT_IMAGE = 301,
        DT_TEXT = 302,
        DT_NTEXT = 303,
        DT_DBTIMESTAMP2 = 304,
        DT_BYREF_I2 = 16386,
        DT_BYREF_I4 = 16387,
        DT_BYREF_R4 = 16388,
        DT_BYREF_R8 = 16389,
        DT_BYREF_CY = 16390,
        DT_BYREF_DATE = 16391,
        DT_BYREF_BOOL = 16395,
        DT_BYREF_DECIMAL = 16398,
        DT_BYREF_I1 = 16400,
        DT_BYREF_UI1 = 16401,
        DT_BYREF_UI2 = 16402,
        DT_BYREF_UI4 = 16403,
        DT_BYREF_I8 = 16404,
        DT_BYREF_UI8 = 16405,
        DT_BYREF_FILETIME = 16448,
        DT_BYREF_GUID = 16456,
        DT_BYREF_NUMERIC = 16515,
        DT_BYREF_DBDATE = 16517,
        DT_BYREF_DBTIME = 16518,
        DT_BYREF_DBTIMESTAMP = 16519,
        DT_BYREF_DBTIME2 = 16520,
        DT_BYREF_DBTIMESTAMPOFFSET = 16521,
        DT_BYREF_DBTIMESTAMP2 = 16522
    }

    /// <summary>
    /// OleDb Source Access Mode
    /// </summary>
    public enum OleDbSourceAccessMode
    {
        OpenRowSet = 0,
        OpenRowSet_Variable = 1,
        SqlCommand = 2,
        SqlCommand_Variable = 3
    }

    /// <summary>
    /// OleDb Source Access Mode
    /// </summary>
    public enum AdoNetSourceAccessMode
    {
        TableOrViewName = 0,
        SqlCommand = 2
    }

    /// <summary>
    /// Stand-in for DTSInsertPlacement
    /// </summary>
    public enum InsertPlacement
    {
        IP_BEFORE = 0,
        IP_AFTER = 1
    }

    /// <summary>
    /// Cache Type possibilities for a Lookup Component
    /// </summary>
    public enum LookupCacheType
    {
        Full = 0,
        Partial = 1,
        Nocache = 2
    }

    /// <summary>
    /// Connection Type possibilities for a Lookup Component
    /// </summary>
    public enum LookupConnectionType
    {
        OLEDB = 0,
        Cache = 1
    }

    /// <summary>
    /// Possibilities to handle no matches in a lookup component
    /// </summary>
    public enum LookupNoMatchHandle
    {
        IgnoreFailure = 0,
        RedirectToErrorOutput = 1,
        FailComponent = 2,
        RedirectToNoMatchOutput = 3
    }

    /// <summary>
    /// Possibilities for how Lookup Component should behave for no matches
    /// </summary>
    public enum LookupNoMatchBehavior
    {
        TreatAsError = 0,
        SendToNoMatchOutput = 1
    }

    /// <summary>
    /// Stand-in for DTSUSageType
    /// </summary>
    public enum UsageType
    {
        UT_READONLY = 0,
        UT_READWRITE = 1,
        UT_IGNORED = 2
    }

    /// <summary>
    /// Options for Lookup Output Column Operations
    /// </summary>
    public enum LookupOutputColumnOperation
    {
        Add = 0,
        Replace = 1
    }

    /// <summary>
    /// Stand-in for Execute SQL Task's SqlStatementSourceType.
    /// </summary>
    public enum SqlStatementSourceType
    {
        DirectInput = 1,
        FileConnection = 2,
        Variable = 3
    }

    /// <summary>
    /// Stand-in for Execute SQL Task's ResultSetType.
    /// </summary>
    public enum ResultSetType
    {
        ResultSetType_None = 1,
        ResultSetType_SingleRow = 2,
        ResultSetType_Rowset = 3,
        ResultSetType_XML = 4
    }

    /// <summary>
    /// Stand-in for Execute SQL Tasks's TypeConversionMode.
    /// </summary>
    public enum TypeConversionMode
    {
        None = 1,
        Allowed = 2
    }
    /// <summary>
    /// Stand-in for the Parameter Directions used in Execute SQL Task's Parameter Binding
    /// </summary>
    public enum ParameterDirections
    {
        Input = 1,
        Output = 2,
        ReturnValue = 4
    }

    /// <summary>
    /// Options for Script Task Language
    /// </summary>
    public enum ScriptTaskLanguage
    {
        VisualBasic = 0,
        CSharp = 1
    }

    /// <summary>
    /// Stand-in for DTSFileConnectionUsageType
    /// </summary>
    public enum FileConnectionUsageType
    {
        DTSFCU_FILEEXISTS = 0,
        DTSFCU_CREATEFILE = 1,
        DTSFCU_FOLDEREXISTS = 2,
        DTSFCU_CREATEFOLDER = 3
    }

    /// <summary>
    /// Stand-in for the DTSOBjectHostType
    /// </summary>
    public enum HostType
    {
        Task = 0,
        ConnectionManager = 1,
        LogProvider = 2,
        ForEachEnumerator = 3
    }

    /// <summary>
    /// Stand-in for DTSConnectionManagerScope
    /// </summary>
    public enum ConnectionManagerScope
    {
        Package = 0,
        Project = 1
    }

    /// <summary>
    /// List of Event Handlers
    /// </summary>
    public enum SSISEventHandler
    {
        OnError,
        OnExecStatusChanged,
        OnInformation,
        OnPostExecute,
        OnPostValidate,
        OnPreExecute,
        OnPreValidate,
        OnProgress,
        OnQueryCancel,
        OnTaskFailed,
        OnVariableValueChanged,
        OnWarning

    }

    /// <summary>
    /// Options when adding/replacing in a derived column component
    /// </summary>
    public enum DerivedColumnAction
    {
        New, Replace
    }

    /// <summary>
    /// OleDb Destination Access Modes
    /// </summary>
    public enum OleDbDestinationAccessMode
    {
        OpenRowSet = 0,
        OpenRowSet_Variable = 1,
        SqlCommand = 2,
        OpenRowSet_FastLoad = 3,
        OpenRowSet_FastLoad_Variable = 4
    }
}
