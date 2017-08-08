using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISLookupComponent : ISPipelineComponent
    {
        #region ctor

        public ISLookupComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.Lookup", componentName)
        {
            _numberOfInputsAllowed = 1;
            _numberOfOutputsAllowed = 2;
            _renamedInputCols = new List<string[]>();
            MatchOutput = new ISOutput(this, 0);
            NoMatchOutput = new ISOutput(this, 1);
        }

        public ISLookupComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent,
            string sourceOutputName = "") :
            this(parentDataFlowTask, componentName)
        {
            //  After adding hte derived column transformation, connect it to a prevoius component
            if (String.IsNullOrEmpty(sourceOutputName))
                ConnectToAnotherPipelineComponent(sourceComponent.Name);
            else
                ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceOutputName);
        }

        #endregion

        #region Component Related Dts Objects

        internal IDTSOutput100 MatchOutput_m { get { return ComponentMetaData.OutputCollection[0]; } }
        internal IDTSOutput100 NoMatchOutput_m { get { return ComponentMetaData.OutputCollection[1]; } }
        private IDTSInput100 _input { get { return ComponentMetaData.InputCollection[0]; } }

        public ISOutput MatchOutput { get; }
        public ISOutput NoMatchOutput { get; }

        #endregion

        #region Component Properties

        #region CacheType

        public LookupCacheType CacheType
        {
            get { return ((LookupCacheType)CustomPropertyGetter<int>("CacheType")); }
            set
            {
                if (value == LookupCacheType.Nocache || value == LookupCacheType.Partial)
                    ConnectionType = LookupConnectionType.OLEDB;
                CustomPropertySetter<int>("CacheType", (int)value);
            }
        }

        #endregion

        #region ConnectionType

        public LookupConnectionType ConnectionType
        {
            get { return ((LookupConnectionType)CustomPropertyGetter<int>("ConnectionType")); }
            set
            {
                if (CacheType == LookupCacheType.Nocache || CacheType == LookupCacheType.Partial)
                    CustomPropertySetter<int>("ConnectionType", (int)LookupConnectionType.OLEDB);
                else
                    CustomPropertySetter<int>("ConnectionType", (int)value);
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

        #region NoMatchCachePercentage

        public int NoMatchCachePercentage
        {
            get { return CustomPropertyGetter<int>("NoMatchCachePercentage"); }
            set { CustomPropertySetter<int>("NoMatchCachePercentage", value); }
        }

        #endregion

        #region SqlCommand

        public string SqlCommand
        {
            get { return CustomPropertyGetter<string>("SqlCommand"); }
            set
            {
                CustomPropertySetter<string>("SqlCommand", value);
                if (!(String.IsNullOrEmpty(Connection)))
                    RetrieveMetaData();
            }
        }

        #endregion

        #region Handle No Matches

        private LookupNoMatchHandle _noMatchHandleSpecification;
        public LookupNoMatchHandle NoMatchHandleSpecification
        {
            get { return _noMatchHandleSpecification; }
            set
            {
                _noMatchHandleSpecification = value;
                if (value == LookupNoMatchHandle.RedirectToNoMatchOutput)
                {
                    NoMatchBehavior = LookupNoMatchBehavior.SendToNoMatchOutput;
                }
                else
                {
                    NoMatchBehavior = LookupNoMatchBehavior.TreatAsError;
                    if (value == LookupNoMatchHandle.IgnoreFailure)
                    {
                        ErrorRowDisposition = RowDisposition.RD_IgnoreFailure;;
                    }

                    if (value == LookupNoMatchHandle.FailComponent)
                    {
                        ErrorRowDisposition = RowDisposition.RD_FailComponent;
                    }

                    if (value == LookupNoMatchHandle.RedirectToErrorOutput)
                    {
                        ErrorRowDisposition = RowDisposition.RD_RedirectRow;
                    }
                }
            }
        }

        #endregion

        #region No Match Behavior

        public LookupNoMatchBehavior NoMatchBehavior
        {
            get { return ((LookupNoMatchBehavior)CustomPropertyGetter<int>("NoMatchBehavior")); }
            private set { CustomPropertySetter<int>("NoMatchBehavior", (int) value); }
        }

        #endregion

        #region Error Row Disposition

        public RowDisposition ErrorRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(MatchOutput_m.ErrorRowDisposition); }
            private set { MatchOutput_m.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region MaxMemoryUsage

        public int MaxMemoryUsage
        {
            get { return CustomPropertyGetter<int>("MaxMemoryUsage"); }
            set { CustomPropertySetter<int>("MaxMemoryUsage", value); }
        }

        #endregion

        #region MaxMemoryUsage64

        public int MaxMemoryUsage64
        {
            get { return CustomPropertyGetter<int>("MaxMemoryUsage64"); }
            set { CustomPropertySetter<int>("MaxMemoryUsage64", value); }
        }

        #endregion

        #region ReferenceMetadataXml

        public string ReferenceMetadataXml
        {
            get { return CustomPropertyGetter<string>("ReferenceMetadataXml"); }
            set { CustomPropertySetter<string>("ReferenceMetadataXml", value); }
        }

        #endregion

        #region TreatDuplicateKeysAsError

        public bool TreatDuplicateKeysAsError
        {
            get { return CustomPropertyGetter<bool>("TreatDuplicateKeysAsError"); }
            set { CustomPropertySetter<bool>("TreatDuplicateKeysAsError", value); }
        }

        #endregion

        #region Connection

        public string Connection
        {
            get { return DtsConvert.GetWrapper(ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager).Name; }
            set
            {
                SetUpConnection(GetConnectionManager(value));
                if (!(String.IsNullOrEmpty(SqlCommand)))
                    RetrieveMetaData();
            }
        }

        #endregion

        #region Join Clause

        /// <summary>
        /// Specify the join Clause in the following format:
        /// Each equality clause are separated by a ";"
        /// Each Equality clause is of the format: input_column = look_up_column
        /// </summary>
        public string JoinClause
        {
            get { return GetJoinClause(); }
            set { SetJoinClause(value); }
        }

        #endregion
        
        #endregion

        #region Helper Properties

        private List<string[]> _renamedInputCols;

        #endregion

        #region Methods

        public void JoinInputToLookUpColumn(string inputColumn, string outputColumn)
        {
            ISInputColumn ic = new ISInputColumn(this, _input.Name, inputColumn, UsageType.UT_READONLY);
            SetCustomPropertyToInputColumn(ic.Input, ic.InputColumn, "JoinToReferenceColumn", outputColumn);
        }

        private void SetJoinClause(string joinClause)
        {
            string[] joinClauses = joinClause.Split(';');
            foreach (string jc in joinClauses)
            {
                string[] cols = jc.Split('=');
                JoinInputToLookUpColumn(cols[0].Trim(), cols[1].Trim());
            }
        }

        private string GetJoinClause()
        {
            StringBuilder jc = new StringBuilder();
            foreach (IDTSInputColumn100 inputColumn in _input.InputColumnCollection)
            {
                string leftName = "";
                foreach (IDTSVirtualInputColumn100 vc in _input.GetVirtualInput().VirtualInputColumnCollection)
                {
                    if (inputColumn.LineageID == vc.LineageID)
                    {
                        leftName = vc.Name;
                        jc.Append(leftName + " = " + inputColumn.CustomPropertyCollection["JoinToReferenceColumn"].Value + ";");
                    }
                }
            }
            return jc.ToString();
        }

        public void AddLookUpColumnIntoDataFlow(string lookupColumnName, string lookupColumnNameAlias, RowDisposition truncationRowDisposition, SSISDataType dataType, int length, int precision, int scale, int codePage, string inputColumnToReplace = null)
        {
            if (String.IsNullOrEmpty(inputColumnToReplace))
            {
                ISOutputColumn oc = new ISOutputColumn(this, MatchOutput_m.Name, lookupColumnName, RowDisposition.RD_NotUsed, truncationRowDisposition);
                oc.SetDataTypeProperties(dataType, length, precision, scale, codePage);
                SetCustomPropertyToOutputColumn(MatchOutput_m.Name, lookupColumnName, "CopyFromReferenceColumn", lookupColumnName);
                oc.Name = String.IsNullOrEmpty(lookupColumnNameAlias) ? lookupColumnName : lookupColumnNameAlias;
            }

            else
            {
                ISInputColumn ic = new ISInputColumn(this, _input.Name, inputColumnToReplace, UsageType.UT_READWRITE);
                SetCustomPropertyToInputColumn(ic.Input, ic.InputColumn, "CopyFromReferenceColumn", lookupColumnName);
                ic.Name = String.IsNullOrEmpty(lookupColumnNameAlias) ? lookupColumnName : lookupColumnNameAlias;
                _renamedInputCols.Add(new string[] { inputColumnToReplace, ic.Name });
            }
        }

        #endregion
    }
}
