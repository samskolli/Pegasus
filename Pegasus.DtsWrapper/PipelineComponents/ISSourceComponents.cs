using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISSourceComponent : ISPipelineComponent
    {
        #region ctor

        internal ISSourceComponent(IDTSComponentMetaData100 component) : base(component)
        {

        }

        public ISSourceComponent(ISDataFlowTask parentDataFlowTask, string componentMoniker, string componentname)
            : base(parentDataFlowTask, componentMoniker, componentname)
        {

        }

        #endregion

        #region Private properties

        internal bool _connectionAssgined = false;
        #endregion
    }

    public class ISOleDbSourceComponent : ISSourceComponent
    {
        #region ctor

        internal ISOleDbSourceComponent(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISOleDbSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.OleDbSource", componentname)
        {
            InitDefaults();
        }

        public ISOleDbSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname, ISConnectionManager cm, bool connectionIsOffline = false) :
            this(parentDataFlowTask, componentname)
        {
            if (!(connectionIsOffline))
            {
                if (cm != null)
                    AssignConnectionManager(cm);
                else
                    System.Console.WriteLine("WARN::: No Connection Manager is attached. Please use the AssignConnectionManager method to assign a Connection Manager.");
            }
        }

        #region default inits

        private void InitDefaults()
        {
            _numberOfOutputsAllowed = 1;
            _numberOfInputsAllowed = 0;
        }

        #endregion

        #endregion

        #region Properties

        #region CommandTimeOut

        public int CommandTimeout
        {
            get { return CustomPropertyGetter<int>("CommandTimeout"); }
            set { CustomPropertySetter<int>("CommandTimeout", value); }
        }

        #endregion

        #region OpenRowset

        public string OpenRowset
        {
            get { return CustomPropertyGetter<string>("OpenRowset"); }
            set
            {
                AccessMode = (int)OleDbSourceAccessMode.OpenRowSet;
                CustomPropertySetter<string>("OpenRowset", value);
            }
        }

        #endregion

        #region OpenRowsetVariable

        public string OpenRowsetVariable
        {
            get { return CustomPropertyGetter<string>("OpenRowsetVariable"); }
            set
            {
                AccessMode = (int)OleDbSourceAccessMode.OpenRowSet_Variable;
                CustomPropertySetter<string>("OpenRowsetVariable", value);
            }
        }

        #endregion

        #region SqlCommand

        public string SqlCommand
        {
            get { return CustomPropertyGetter<string>("SqlCommand"); }
            set
            {
                AccessMode = (int)OleDbSourceAccessMode.SqlCommand;
                CustomPropertySetter<string>("SqlCommand", value);
                if (_connectionAssgined == true)
                    RetrieveMetaData();
            }
        }

        #endregion

        #region SqlCommandVariable

        public string SqlCommandVariable
        {
            get { return CustomPropertyGetter<string>("SqlCommandVariable"); }
            set
            {
                AccessMode = (int)OleDbSourceAccessMode.SqlCommand_Variable;
                CustomPropertySetter<string>("SqlCommandVariable", value);
            }
        }

        #endregion

        #region DefaultCodePage

        public int DefaultCodePage
        {
            get { return CustomPropertyGetter<int>("DefaultCodePage"); }
            set { CustomPropertySetter<int>("DefaultCodePage", value); }
        }

        #endregion

        #region AlwaysUseDefaultCodePage

        public bool AlwaysUseDefaultCodePage
        {
            get { return CustomPropertyGetter<bool>("AlwaysUseDefaultCodePage"); }
            set { CustomPropertySetter<bool>("AlwaysUseDefaultCodePage", value); }
        }

        #endregion

        #region AccessMode

        public int AccessMode
        {
            get { return CustomPropertyGetter<int>("AccessMode"); }
            set { CustomPropertySetter<int>("AccessMode", value); }
        }

        #endregion

        #region ParameterMapping

        public string ParameterMapping
        {
            get { return CustomPropertyGetter<string>("ParameterMapping"); }
            set { CustomPropertySetter<string>("ParameterMapping", value); }
        }

        private string ReturnParameterString(int paramPosition, string direction, string variable)
        {
            //ParameterMapping
            return "\"Parameter" + paramPosition.ToString() + ":" + direction + "\"," + variable + ";";
        }

        public void AddParameter(int paramPosition, string direction, string variable)
        {

            if (string.IsNullOrEmpty(ParameterMapping))
            {
                ParameterMapping = ReturnParameterString(paramPosition, direction, variable);
            }
            else
            {
                ParameterMapping = ParameterMapping + ReturnParameterString(paramPosition, direction, variable);
            }
        }

        #endregion

        #endregion

        #region Assign Connection Manager

        private void AssignConnectionManager(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
            _connectionAssgined = true;
        }

        public void AssignConnectionManager(ISConnectionManager cm)
        {
            AssignConnectionManager(cm.ConnectionManager);
        }

        #endregion

        #region Implicit Operator

        //public static implicit operator ISOleDbSourceComponent(IDTSComponentMetaData100 component)
        //{
        //    if (component == null)
        //        return null;

        //    return new ISOleDbSourceComponent(component);
        //}

        #endregion
    }
}

