using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System;
using System.Collections.Generic;


namespace Pegasus.DtsWrapper
{
    public class ISDestinationComponent : ISPipelineComponent
    {
        #region ctor

        /// <summary>
        /// ctor that accepts a IDTSComponentMEtadata100
        /// </summary>
        /// <param name="component"></param>
        internal ISDestinationComponent(IDTSComponentMetaData100 component) : base(component)
        {

        }

        public ISDestinationComponent(ISDataFlowTask parentDataFlowTask, string componentMoniker, string componentname)
            : base(parentDataFlowTask, componentMoniker, componentname)
        {

        }

        #endregion

        #region Input

        private ISInput _input;
        public ISInput Input
        {
            get
            {
                if (_input == null)
                {
                    _input = new ISInput(this);
                }
                return _input;
            }
            set { }
        }

        internal IDTSInput100 DtsInput { get { return ComponentMetaData.InputCollection[0]; } }

        #endregion
    }

    public class ISOleDbDestinationComponent : ISDestinationComponent
    {
        #region ctor

        internal ISOleDbDestinationComponent(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISOleDbDestinationComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.OleDbDestination", componentname)
        {
            //InitDefaults();
            ExternalColumnInputColumnMap = new List<ExternalColumnInputMap>();

        }

        public ISOleDbDestinationComponent(ISDataFlowTask parentDataFlowTask, string componentName,
            ISPipelineComponent sourceComponent,
            string sourceOutputName = "") :
            this(parentDataFlowTask, componentName)
        {
            if (String.IsNullOrEmpty(sourceOutputName))
                ConnectToAnotherPipelineComponent(sourceComponent.Name);
            else
                ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceOutputName);

            InitDefaults();
        }

        #endregion

        #region default inits

        public void InitDefaults()
        {
            _numberOfOutputsAllowed = 0;
            _numberOfInputsAllowed = 1;
            Input = new ISInput(this, 0);
            ExternalColumnInputColumnMap = new List<ExternalColumnInputMap>();
        }

        #endregion

        #region Helper Properties

        public string DestinationPassword { get; set; }

        #endregion
        
        #region Dts Properties

        #region AccessMode

        //public OleDbDestinationAccessMode AccessMode
        //{
        //    get { return ((OleDbDestinationAccessMode)CustomPropertyGetter<int>("AccessMode")); }
        //    set { CustomPropertySetter<int>("OleDbDestinationAccessMode", (int)value); }
        //}

        public int AccessMode
        {
            get { return CustomPropertyGetter<int>("AccessMode"); }
            set { CustomPropertySetter<int>("AccessMode", value); }
        }

        #endregion

        #region AlwaysUseDefaultCodePage

        public bool AlwaysUseDefaultCodePage
        {
            get { return CustomPropertyGetter<bool>("AlwaysUseDefaultCodePage"); }
            set { CustomPropertySetter<bool>("AlwaysUseDefaultCodePage", value); }
        }

        #endregion

        #region CommandTimeOut

        public int CommandTimeout
        {
            get { return CustomPropertyGetter<int>("CommandTimeout"); }
            set { CustomPropertySetter<int>("CommandTimeout", value); }
        }

        #endregion

        #region Connection

        public ISConnectionManager ConnMgr
        {
            get;
            set;
        }

        private string _connection;
        public string Connection
        {
            get
            {
                //return ComponentMetaData.RuntimeConnectionCollection[0].Name;
                return _connection;
            }
            set
            {
                _connection = value;
                AssignConnectionManager(value);
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

        #region FastLoadKeepIdentity

        public bool FastLoadKeepIdentity
        {
            get { return CustomPropertyGetter<bool>("FastLoadKeepIdentity"); }
            set { CustomPropertySetter<bool>("FastLoadKeepIdentity", value); }
        }

        #endregion

        #region FastLoadKeepNulls

        public bool FastLoadKeepNulls
        {
            get { return CustomPropertyGetter<bool>("FastLoadKeepNulls"); }
            set { CustomPropertySetter<bool>("FastLoadKeepNulls", value); }
        }

        #endregion

        #region FastLoadMaxInsertCommitSize

        public int FastLoadMaxInsertCommitSize
        {
            get { return CustomPropertyGetter<int>("FastLoadMaxInsertCommitSize"); }
            set { CustomPropertySetter<int>("FastLoadMaxInsertCommitSize", value); }
        }

        #endregion

        #region FastLoadOptions

        public string FastLoadOptions
        {
            get { return CustomPropertyGetter<string>("FastLoadOptions"); }
            set { CustomPropertySetter<string>("FastLoadOptions", value); }
        }

        private bool _tableLock;
        public bool TableLock
        {
            get { return FastLoadOptions.ToLower().Contains("TABLOCK".ToLower()) ? true : false; }
            set
            {
                _tableLock = true;
                FastLoadOptions = FastLoadOptions + (FastLoadOptions.Length == 0 ? "TABLOCK".ToUpper() : (FastLoadOptions.ToLower().Contains("TABLOCK".ToLower()) ? "" : ",TABLOCK".ToUpper()));
            }
        }

        private bool _checkConstraints;
        public bool CheckConstraints
        {
            get { return FastLoadOptions.ToLower().Contains("CHECK_CONSTRAINTS".ToLower()) ? true : false; }
            set
            {
                _checkConstraints = true;
                FastLoadOptions = (FastLoadOptions.Length == 0 ? FastLoadOptions + "CHECK_CONSTRAINTS".ToUpper() : (FastLoadOptions.ToLower().Contains("CHECK_CONSTRAINTS".ToLower()) ? "" : "CHECK_CONSTRAINTS".ToUpper() + "," + FastLoadOptions));
            }
        }

        #endregion

        #region OpenRowset

        public string OpenRowset
        {
            get { return CustomPropertyGetter<string>("OpenRowset"); }
            set
            {
                //AccessMode = OleDbDestinationAccessMode.OpenRowSet_FastLoad;
                AccessMode = (int)OleDbDestinationAccessMode.OpenRowSet_FastLoad;
                CustomPropertySetter<string>("OpenRowset", value);
                if (!(String.IsNullOrEmpty(Connection)))
                {
                    bool retrieved = false;
                    if (!(String.IsNullOrEmpty(DestinationPassword)))
                    {
                        retrieved = RetrieveMetaData(Connection, DestinationPassword);
                    }
                    else
                    {
                        retrieved = RetrieveMetaData();
                    }
                    if (retrieved)
                    {
                        MapInputColumnsToExternalColumns();
                    }
                }       
            }
        }

        private void MapInputColumnsToExternalColumns()
        {
            if (ExternalColumnInputColumnMap.Count > 0)
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }

                foreach (ExternalColumnInputMap map in ExternalColumnInputColumnMap)
                {
                    for (int mc = 0; mc < DtsInput.ExternalMetadataColumnCollection.Count; mc++)
                    {
                        if (DtsInput.ExternalMetadataColumnCollection[mc].Name == map.ExternalColumn.ExternalColumnName)
                        {
                            //IDTSInputColumn100 ic = GetInputColumn(DtsInput.Name, map.Item2);
                            //ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                            for (int vi = 0; vi < viCols.Length; vi++)
                            {
                                if (viCols[vi] == map.InputColumnName)
                                {
                                    ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[vi], UsageType.UT_READONLY);
                                    //ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                                    //MapInputColumn(DtsInput.Name, DtsInput.ExternalMetadataColumnCollection[mc].Name);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }
                for (int i = 0; i < viCols.Length; i++)
                {
                    for (int mc = 0; mc < DtsInput.ExternalMetadataColumnCollection.Count; mc++)
                    {
                        if (DtsInput.ExternalMetadataColumnCollection[mc].Name == viCols[i])
                        {
                            ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[i], UsageType.UT_READONLY);
                            ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                        }
                    }
                }
            }
        }

        private void PopulateMetadata()
        {
            bool retrieved = RetrieveMetaData();
        }

        #endregion

        #region OpenRowsetVariable

        public string OpenRowsetVariable
        {
            get { return CustomPropertyGetter<string>("OpenRowsetVariable"); }
            set
            {
                //AccessMode = OleDbDestinationAccessMode.OpenRowSet_Variable;
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
                //AccessMode = OleDbDestinationAccessMode.SqlCommand;
                CustomPropertySetter<string>("SqlCommand", value);
                //if (_connectionAssgined == true)
                //    RetrieveMetaData();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Assign Connection Manager

        private void AssignConnectionManager(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
            //cmg.Properties["Password"].SetValue(cmg, ConnectionPassword);
            //_connectionAssgined = true;
            //PopulateMetadata();
        }

        public void AssignConnectionManager(ISConnectionManager cm)
        {
            AssignConnectionManager(cm.ConnectionManager);
            if (!(String.IsNullOrEmpty(OpenRowset)))
                RetrieveMetaData(Connection, DestinationPassword);
        }

        public void AssignConnectionManager(string connection)
        {
            ConnMgr = GetConnectionManager(connection);
            AssignConnectionManager(ConnMgr);
        }

        #endregion

        #region Column Mapping

        public List<ExternalColumnInputMap> ExternalColumnInputColumnMap;

        public void ManualMapToTargetColumns()
        {
            if (ExternalColumnInputColumnMap.Count > 0)
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }

                foreach (ExternalColumnInputMap map in ExternalColumnInputColumnMap)
                {
                    if (String.IsNullOrEmpty(map.InputColumnName))
                    {

                    }
                    else
                    {
                        ISExternalMetadataColumn extCol = new ISExternalMetadataColumn(this, DtsInput.Name, map.ExternalColumn.ExternalColumnName, true);
                        for (int vi = 0; vi < viCols.Length; vi++)
                        {
                            if (viCols[vi].ToLower() == map.InputColumnName.ToLower())
                            {
                                ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[vi], UsageType.UT_READONLY);
                                DesignTimeComponent.MapInputColumn(DtsInput.ID, ic.ID, extCol.ID);
                                extCol.DataType = ic.DataType;
                                extCol.CodePage = ic.CodePage;
                                extCol.Length = ic.Length;
                                extCol.Precision = ic.Precision;
                                extCol.Scale = ic.Scale;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

    }

    public class ISFlatFileDestination : ISDestinationComponent
    {
        #region ctor

        internal ISFlatFileDestination(IDTSComponentMetaData100 component) : base(component)
        {
            InitDefaults();
        }

        public ISFlatFileDestination(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(parentDataFlowTask, "Microsoft.FlatFileDestination", componentname)
        {
            //InitDefaults();
            ExternalColumnInputColumnMap = new List<ExternalColumnInputMap>();

        }

        public ISFlatFileDestination(ISDataFlowTask parentDataFlowTask, string componentName,
            ISPipelineComponent sourceComponent,
            string sourceOutputName = "") :
            this(parentDataFlowTask, componentName)
        {
            if (String.IsNullOrEmpty(sourceOutputName))
                ConnectToAnotherPipelineComponent(sourceComponent.Name);
            else
                ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceOutputName);

            InitDefaults();
        }

        #endregion

        #region default inits

        public void InitDefaults()
        {
            _numberOfOutputsAllowed = 0;
            _numberOfInputsAllowed = 1;
            Input = new ISInput(this, 0);
            ExternalColumnInputColumnMap = new List<ExternalColumnInputMap>();
        }

        #endregion

        #region Helper Properties

        public string DestinationPassword { get; set; }

        #endregion

        #region Dts Properties

        #region Header

        public string Header
        {
            get { return CustomPropertyGetter<string>("Header"); }
            set { CustomPropertySetter<string>("Header", value); }
        }

        #endregion

        #region Overwrite

        public bool Overwrite
        {
            get { return CustomPropertyGetter<bool>("Overwrite"); }
            set { CustomPropertySetter<bool>("Overwrite", value); }
        }

        #endregion

        #region CommandTimeOut

        public int CommandTimeout
        {
            get { return CustomPropertyGetter<int>("CommandTimeout"); }
            set { CustomPropertySetter<int>("CommandTimeout", value); }
        }

        #endregion

        #region Connection

        public ISConnectionManager ConnMgr
        {
            get;
            set;
        }

        private string _connection;
        public string Connection
        {
            get
            {
                //return ComponentMetaData.RuntimeConnectionCollection[0].Name;
                return _connection;
            }
            set
            {
                _connection = value;
                AssignConnectionManager(value);
            }
        }

        #endregion

        

        private void MapInputColumnsToExternalColumns()
        {
            if (ExternalColumnInputColumnMap.Count > 0)
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }

                foreach (ExternalColumnInputMap map in ExternalColumnInputColumnMap)
                {
                    for (int mc = 0; mc < DtsInput.ExternalMetadataColumnCollection.Count; mc++)
                    {
                        if (DtsInput.ExternalMetadataColumnCollection[mc].Name == map.ExternalColumn.ExternalColumnName)
                        {
                            //IDTSInputColumn100 ic = GetInputColumn(DtsInput.Name, map.Item2);
                            //ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                            for (int vi = 0; vi < viCols.Length; vi++)
                            {
                                if (viCols[vi] == map.InputColumnName)
                                {
                                    ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[vi], UsageType.UT_READONLY);
                                    //ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                                    //MapInputColumn(DtsInput.Name, DtsInput.ExternalMetadataColumnCollection[mc].Name);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }
                for (int i = 0; i < viCols.Length; i++)
                {
                    for (int mc = 0; mc < DtsInput.ExternalMetadataColumnCollection.Count; mc++)
                    {
                        if (DtsInput.ExternalMetadataColumnCollection[mc].Name == viCols[i])
                        {
                            ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[i], UsageType.UT_READONLY);
                            ic.ExternalMetadataColumnID = DtsInput.ExternalMetadataColumnCollection[mc].ID;
                        }
                    }
                }
            }
        }

        private void PopulateMetadata()
        {
            bool retrieved = RetrieveMetaData();
        }

        #endregion

        #region Methods

        #region Assign Connection Manager

        private void AssignConnectionManager(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
            //cmg.Properties["Password"].SetValue(cmg, ConnectionPassword);
            //_connectionAssgined = true;
            //PopulateMetadata();
        }

        public void AssignConnectionManager(ISConnectionManager cm)
        {
            AssignConnectionManager(cm.ConnectionManager);
            RetrieveMetaData();
        }

        public void AssignConnectionManager(string connection)
        {
            ConnMgr = GetConnectionManager(connection);
            AssignConnectionManager(ConnMgr);
        }

        #endregion

        #region Column Mapping

        public List<ExternalColumnInputMap> ExternalColumnInputColumnMap;

        public void ManualMapToTargetColumns()
        {
            if (ExternalColumnInputColumnMap.Count > 0)
            {
                string[] viCols = new string[Input.Input.GetVirtualInput().VirtualInputColumnCollection.Count];
                for (int i = 0; i < viCols.Length; i++)
                {
                    viCols.SetValue(Input.Input.GetVirtualInput().VirtualInputColumnCollection[i].Name, i);
                }

                foreach (ExternalColumnInputMap map in ExternalColumnInputColumnMap)
                {
                    if (String.IsNullOrEmpty(map.InputColumnName))
                    {

                    }
                    else
                    {
                        ISExternalMetadataColumn extCol = new ISExternalMetadataColumn(this, DtsInput.Name, map.ExternalColumn.ExternalColumnName, true);
                        for (int vi = 0; vi < viCols.Length; vi++)
                        {
                            if (viCols[vi].ToLower() == map.InputColumnName.ToLower())
                            {
                                ISInputColumn ic = new ISInputColumn(this, DtsInput.Name, viCols[vi], UsageType.UT_READONLY);
                                DesignTimeComponent.MapInputColumn(DtsInput.ID, ic.ID, extCol.ID);
                                extCol.DataType = ic.DataType;
                                extCol.CodePage = ic.CodePage;
                                extCol.Length = ic.Length;
                                extCol.Precision = ic.Precision;
                                extCol.Scale = ic.Scale;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

    }

    public struct ExternalMetadataColumn
    {
        public string ExternalColumnName;
        public SSISDataType DataType;
        public int Length;
        public int Precision;
        public int Scale;
        public int CodePage;
    }

    public struct ExternalColumnInputMap
    {
        public ExternalMetadataColumn ExternalColumn { get; set; }
        public string InputColumnName { get; set; }
    }
}
