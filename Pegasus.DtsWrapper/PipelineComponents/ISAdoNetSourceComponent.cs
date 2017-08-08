using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Data.SqlClient;

namespace Pegasus.DtsWrapper
{
    public class ISAdoNetSourceComponent : ISSourceComponent
    {

        #region ctor

        internal ISAdoNetSourceComponent(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISAdoNetSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.SqlServer.Dts.Pipeline.DataReaderSourceAdapter, Microsoft.SqlServer.ADONETSrc, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", componentname)
        {
            InitDefaults();
        }

        public ISAdoNetSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname, ISConnectionManager cm, bool connectionIsOffline = false) :
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

        #region AccessMode

        public int AccessMode
        {
            get { return CustomPropertyGetter<int>("AccessMode"); }
            private set { CustomPropertySetter<int>("AccessMode", value); }
        }

        #endregion

        #region AllowImplicitStringConversion

        public bool AllowImplicitStringConversion
        {
            get { return CustomPropertyGetter<bool>("AllowImplicitStringConversion"); }
            set { CustomPropertySetter<bool>("AllowImplicitStringConversion", value); }
        }

        #endregion

        #region CommandTimeOut

        public int CommandTimeout
        {
            get { return CustomPropertyGetter<int>("CommandTimeout"); }
            set { CustomPropertySetter<int>("CommandTimeout", value); }
        }

        #endregion

        #region SqlCommand

        public string SqlCommand
        {
            get { return CustomPropertyGetter<string>("SqlCommand"); }
            set
            {
                AccessMode = (int)AdoNetSourceAccessMode.SqlCommand;
                CustomPropertySetter<string>("SqlCommand", value);
                if (_connectionAssgined == true)
                    RetrieveMetaData();
            }
        }

        #endregion

        #region TableOrViewName

        public string TableOrViewName
        {
            get { return CustomPropertyGetter<string>("TableOrViewName"); }
            set
            {
                AccessMode = (int)AdoNetSourceAccessMode.TableOrViewName;
                CustomPropertySetter<string>("TableOrViewName", value);
                if (_connectionAssgined == true)
                    RetrieveMetaData();
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
    }
}
