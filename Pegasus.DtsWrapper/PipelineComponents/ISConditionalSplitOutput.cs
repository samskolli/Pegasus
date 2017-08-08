using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISConditionalSplitOutput
    {
        #region Properties

        #region parent component

        private ISConditionalSplitComponent _parentComponent;

        #endregion

        #region ISOutput

        private ISOutput _output;

        #endregion

        #endregion

        #region ctor

        public ISConditionalSplitOutput(ISConditionalSplitComponent parentComponent, string outputName, int evaluationOrder, string condition, string conditionColumns = "")
        {
            _parentComponent = parentComponent;
            _output = new ISOutput(parentComponent, outputName);
            EvaluationOrder = evaluationOrder;
            SetFriendlyExpression(condition, conditionColumns);            
        }

        #endregion

        #region Methods

        internal void SetFriendlyExpression(string condition, string conditionColumns = "")
        {
            IDTSInput100 _input = _parentComponent.ComponentMetaData.InputCollection[0];
            List<string> colArray = new List<string>();
            if (String.IsNullOrEmpty(conditionColumns))
            {
                foreach (IDTSVirtualInputColumn100 column in _input.GetVirtualInput().VirtualInputColumnCollection)
                    if (condition.Contains(column.Name))
                        colArray.Add(column.Name);
            }
            else
                colArray = conditionColumns.Split(',').ToList();

            foreach (string column in colArray)
                _parentComponent.SetInputColumnDTSUsageType(_input, column, UsageType.UT_READONLY);

            //_output.SetCustomProperty("FriendlyExpression", condition);
            _output.SetCustomProperty("Expression", condition);
        }

        #endregion

        #region Output Properties

        public int EvaluationOrder
        {
            get { return (int)_output.GetCustomPropertyValue("EvaluationOrder"); }
            set { _output.SetCustomProperty("EvaluationOrder", value); }
        }

        public string Expression
        {
            get { return (string)_output.GetCustomPropertyValue("Expression"); }
            set { SetFriendlyExpression(value, ""); }
        }
        
        #endregion
    }
}
