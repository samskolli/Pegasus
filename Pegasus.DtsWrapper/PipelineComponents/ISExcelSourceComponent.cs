using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISExcelSourceComponent : ISSourceComponent
    {

        #region ctor

        public ISExcelSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.ExcelSource", componentname)
        {
            InitDefaults();
        }

        public ISExcelSourceComponent(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISExcelSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname, ISConnectionManager cm, bool connectionIsOffline = false) :
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


        #endregion

        #region Properties

        #region AccessMode

        public int AccessMode
        {
            get { return CustomPropertyGetter<int>("AccessMode"); }
            set { CustomPropertySetter<int>("AccessMode", value); }
        }

        #endregion

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
                RetrieveMetaData();
            }
        }

        #endregion

        #region OpenRowsetVariable

        public string OpenRowsetVariable
        {
            get { return CustomPropertyGetter<string>("OpenRowsetVariable"); }
            set
            {
                AccessMode = (int)ExcelSourceAccessMode.OpenRowSet_Variable;
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
                AccessMode = (int)ExcelSourceAccessMode.SqlCommand;
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
                AccessMode = (int)ExcelSourceAccessMode.SqlCommand_Variable;
                CustomPropertySetter<string>("SqlCommandVariable", value);
            }
        }

        #endregion

        #endregion

        #region default inits

        private void InitDefaults()
        {
            _numberOfOutputsAllowed = 1;
            _numberOfInputsAllowed = 0;
        }

        #endregion

        #region Assign Connection Manager

        private void AssignConnectionManager(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
            if (string.IsNullOrEmpty(OpenRowset) && string.IsNullOrEmpty(OpenRowsetVariable) && string.IsNullOrEmpty(SqlCommand) && string.IsNullOrEmpty(SqlCommandVariable))
            {
                
            }
            else
            {
                RetrieveMetaData();
            }
        }

        public void AssignConnectionManager(ISConnectionManager cm)
        {
            AssignConnectionManager(cm.ConnectionManager);
        }

        #endregion

    }
}
