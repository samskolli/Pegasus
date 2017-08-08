using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISDerivedColumnComponent : ISPipelineComponent
    {
        #region ctor

        /// <summary>
        /// ctor that accepts the parent data flow task and a name for the row component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        public ISDerivedColumnComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.DerivedColumn", componentName)
        {
            _numberOfInputsAllowed = 1;
            _numberOfOutputsAllowed = 1;
            _input = ComponentMetaData.InputCollection[0];
        }

        /// <summary>
        /// an extended ctor that also connects to another component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        /// <param name="sourceComponent"></param>
        /// <param name="sourceOutputName"></param>
        public ISDerivedColumnComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent,
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

        #region Properties

        private IDTSInput100 _input;

        #endregion

        #region Add a Derived Column

        public ISOutputColumn AddDerivedColumn(string columnName, string expression, string expressionCols, SSISDataType dataType, int length = 0, int precision = 0, int scale = 0, int codePage = 0, RowDisposition errorRowDisposition = RowDisposition.RD_FailComponent, RowDisposition truncationRowDisposition = RowDisposition.RD_FailComponent)
        {
            if (!(String.IsNullOrEmpty(expressionCols)))
                SetUsageTypeToReadForExpressionCols(expressionCols);

            ISOutputColumn oc = new ISOutputColumn(this, this.GetOutputFromIndex(0).Name, columnName, errorRowDisposition, truncationRowDisposition);
            oc.SetDataTypeProperties(dataType, length, precision, scale, codePage);
            SetCustomPropertyToOutputColumn(oc.Output, oc.OutputColumn, "Expression", expression);
            SetCustomPropertyToOutputColumn(oc.Output, oc.OutputColumn, "FriendlyExpression", expression);
            return oc;
        }

        #endregion

        #region Replace with Derived Column

        public ISInputColumn ReplaceWithDerivedColumn(string columnName, string expression, string expressionCols, RowDisposition errorRowDisposition = RowDisposition.RD_FailComponent, RowDisposition truncationRowDisposition = RowDisposition.RD_FailComponent)
        {
            if (!(String.IsNullOrEmpty(expressionCols)))
                SetUsageTypeToReadForExpressionCols(expressionCols);
            ISInputColumn ic = new ISInputColumn(this, _input.Name, columnName, UsageType.UT_READWRITE, errorRowDisposition, truncationRowDisposition);
            SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, columnName), "Expression", expression);
            SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, columnName), "FriendlyExpression", expression);
            return ic;
        }

        #endregion

        #region Parse columns in expressionCols input and Set Usage Type

        private string[] GetIndividualCols(string expressionCols)
        {
            return expressionCols.Split(',');
        }

        internal void SetUsageTypeToReadForExpressionCols(string expressionCols)
        {
            foreach (string column in GetIndividualCols(expressionCols))
            {
                if (!(_readWriteCols.Contains(column)))
                    SetInputColumnDTSUsageType(_input, column.Trim(), UsageType.UT_READONLY);
            }
        }

        #endregion
    }

    public class ISDerivedColumn
    {
        #region Properties

        #region Parent Component

        private ISDerivedColumnComponent _dervColComponent;

        #endregion

        #region Input/Output

        private IDTSInput100 _input { get { return _dervColComponent.ComponentMetaData.InputCollection[0]; } }
        private IDTSOutput100 _output { get { return _dervColComponent.ComponentMetaData.OutputCollection[0]; } }

        public ISOutputColumn OutputColumn
        {
            get; set;
        }

        public ISInputColumn InputColumn
        {
            get; set;
        }

        #endregion

        #region Name

        private string _name;
        public string DerivedColumnName
        {
            get
            {
                if (OutputColumn != null)
                    _name = OutputColumn.Name;
                if (InputColumn != null)
                    _name = InputColumn.Name;
                return _name;
            }
            set
            {
                if (OutputColumn != null)
                    OutputColumn.Name = value;
                if (InputColumn != null)
                    InputColumn.Name = value;
            }
        }

        #endregion

        #region Expression

        private string _expression;
        public string Expression
        {
            #region Get

            get
            {
                if (OutputColumn != null)
                {
                    _expression = OutputColumn.OutputColumn.CustomPropertyCollection["FriendlyExpression"].Value;
                }
                if (InputColumn != null)
                {
                    foreach (IDTSCustomProperty100 prop in InputColumn.InputColumn.CustomPropertyCollection)
                    {
                        if (prop.Name == "FriendlyExpression")
                            _expression = prop.Value;
                    }
                }
                return _expression;
            }

            #endregion

            #region Set

            set
            {
                if (OutputColumn != null)
                {
                    List<string> _expCols = new List<string>();
                    foreach (IDTSVirtualInputColumn100 vColumn in _input.GetVirtualInput().VirtualInputColumnCollection)
                    {
                        if (value.Contains(vColumn.Name))
                            _expCols.Add(vColumn.Name);
                    }
                    if (_expCols.Count > 0)
                    {
                        string expressionCols = string.Join("|", _expCols);
                        _dervColComponent.SetUsageTypeToReadForExpressionCols(expressionCols);
                    }
                    _dervColComponent.SetCustomPropertyToOutputColumn(_output, OutputColumn.OutputColumn, "Expression", value, true);
                    _dervColComponent.SetCustomPropertyToOutputColumn(_output, OutputColumn.OutputColumn, "FriendlyExpression", value);
                }

                if (InputColumn != null)
                {
                    List<string> _expCols = new List<string>();
                    foreach (IDTSVirtualInputColumn100 vColumn in _input.GetVirtualInput().VirtualInputColumnCollection)
                    {
                        if (value.Contains(vColumn.Name) && vColumn.Name != InputColumn.Name)
                            _expCols.Add(vColumn.Name);
                    }
                    if (_expCols.Count > 0)
                    {
                        string expressionCols = string.Join("|", _expCols);
                        _dervColComponent.SetUsageTypeToReadForExpressionCols(expressionCols);
                    }

                    _dervColComponent.SetCustomPropertyToInputColumn(_input, InputColumn.InputColumn, "Expression", value, true);
                    _dervColComponent.SetCustomPropertyToInputColumn(_input, InputColumn.InputColumn, "FriendlyExpression", value);
                }
            }

            #endregion
        }

        #endregion

        #region Error Row Disposition

        private RowDisposition _errorRowDisposition;
        public RowDisposition ErrorRowDisposition
        {
            get
            {
                if (OutputColumn != null)
                    _errorRowDisposition = OutputColumn.ErrorRowDisposition;
                if (InputColumn != null)
                    _errorRowDisposition = InputColumn.ErrorRowDisposition;
                return _errorRowDisposition;
            }
            set
            {
                if (OutputColumn != null)
                    OutputColumn.ErrorRowDisposition = value;
                if (InputColumn != null)
                    InputColumn.ErrorRowDisposition = value;
            }
        }

        #endregion

        #region Truncation Row Disposition

        private RowDisposition _truncationRowDisposition;
        public RowDisposition TruncationRowDisposition
        {
            get
            {
                if (OutputColumn != null)
                    _truncationRowDisposition = OutputColumn.TruncationRowDisposition;
                if (InputColumn != null)
                    _truncationRowDisposition = InputColumn.TruncationRowDisposition;
                return _errorRowDisposition;
            }
            set
            {
                if (OutputColumn != null)
                    OutputColumn.TruncationRowDisposition = value;
                if (InputColumn != null)
                    InputColumn.TruncationRowDisposition = value;
            }
        }

        #endregion

        #region DataType Props

        #region CodePage

        public int CodePage
        {
            get { return OutputColumn.CodePage; }
            set { OutputColumn.CodePage = value; }
        }

        #endregion

        #region DataType

        public SSISDataType DataType
        {
            get { return OutputColumn.DataType; }
            set { OutputColumn.DataType = value; }
        }

        #endregion

        #region Length

        public int Length
        {
            get { return OutputColumn.Length; }
            set { OutputColumn.Length = value; }
        }

        #endregion

        #region Precision

        public int Precision
        {
            get { return OutputColumn.Precision; }
            set { OutputColumn.Precision = value; }
        }

        #endregion

        #region Scale

        public int Scale
        {
            get { return OutputColumn.Scale; }
            set { OutputColumn.Scale = value; }
        }

        #endregion

        #endregion

        #endregion

        #region ctor

        public ISDerivedColumn(ISDerivedColumnComponent parentComponent, DerivedColumnAction action, string columnName)
        {
            _dervColComponent = parentComponent;
            if (action == DerivedColumnAction.Replace)
            {
                InputColumn = new ISInputColumn(_dervColComponent, _input.Name, columnName, UsageType.UT_READWRITE);
                InputColumn.TruncationRowDisposition = RowDisposition.RD_FailComponent;
                InputColumn.ErrorRowDisposition = RowDisposition.RD_FailComponent;
            }
            if (action == DerivedColumnAction.New)
            {
                OutputColumn = new ISOutputColumn(_dervColComponent, _output.Name, columnName);
                OutputColumn.TruncationRowDisposition = RowDisposition.RD_FailComponent;
                OutputColumn.ErrorRowDisposition = RowDisposition.RD_FailComponent;
            }
        }
                
        #endregion

        #region Methods

        public void SetDataTypeProperties(SSISDataType dataType, int length, int precision, int scale, int codePage)
        {
            OutputColumn.SetDataTypeProperties(dataType, length, precision, scale, codePage);
        }

        #endregion
    }
}
