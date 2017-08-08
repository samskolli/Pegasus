using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISFlatFileSourceComponent : ISPipelineComponent
    {
        #region ctor

        public ISFlatFileSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.FlatFileSource", componentname)
        {
            InitDefaults();
        }

        public ISFlatFileSourceComponent(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISFlatFileSourceComponent(ISDataFlowTask parentDataFlowTask, string componentname, ISConnectionManager cm, bool connectionIsOffline = false) :
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

        #region default inits

        private void InitDefaults()
        {
            _numberOfOutputsAllowed = 1;
            _numberOfInputsAllowed = 0;
        }

        #endregion

        #region Properties

        public string FileNameColumnName
        {
            get { return CustomPropertyGetter<string>("FileNameColumnName"); }
            set { CustomPropertySetter<string>("FileNameColumnName", value); }
        }

        public bool RetainNulls
        {
            get { return CustomPropertyGetter<bool>("RetainNulls"); }
            set { CustomPropertySetter<bool>("RetainNulls", value); }
        }

        #endregion

        #region Assign Connection Manager

        private void AssignConnectionManager(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
            RetrieveMetaData();
        }

        public void AssignConnectionManager(ISConnectionManager cm)
        {
            AssignConnectionManager(cm.ConnectionManager);
        }

        #endregion

        #region Set Fast Parse

        public void SetFastParse()
        {
            foreach (IDTSOutputColumn100 c in ComponentMetaData.OutputCollection[0].OutputColumnCollection)
                SetCustomPropertyToOutputColumn(ComponentMetaData.OutputCollection[0].Name, c.Name, "FastParse", "True");
        }

        #endregion


    }
}
