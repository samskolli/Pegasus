using System;
using System.Linq;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISConditionalSplitComponent : ISPipelineComponent
    {
        #region ctor

        public ISConditionalSplitComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.ConditionalSplit", componentName)
        {
            _numberOfInputsAllowed = 1;
            _numberOfOutputsAllowed = -1;
            this.DefaultOutputName = "DefaultOutput";
        }

        public ISConditionalSplitComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent,
            string sourceOutputName = "") :
            this(parentDataFlowTask, componentName)
        {
            //  After adding the transformation, connect it to a previous component
            if (String.IsNullOrEmpty(sourceOutputName))
                ConnectToAnotherPipelineComponent(sourceComponent.Name);
            else
                ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceOutputName);
        }

        public ISConditionalSplitComponent(ISDataFlowTask parentDataFlowTask, string componentName, 
            ISPipelineComponent sourceComponent,
            ISOutput sourceOutput) :
            this(parentDataFlowTask, componentName, sourceComponent, sourceOutput.Name)
        {
            
        }

        #endregion
            
        #region Component Properties

        #region Default Output

        private IDTSOutput100 GetDefaultOutput()
        {
            IDTSOutput100 op = (IDTSOutput100)null;
            foreach (IDTSOutput100 o in ComponentMetaData.OutputCollection)
            {
                for (int i = 0; i < o.CustomPropertyCollection.Count; i++)
                {
                    if (o.CustomPropertyCollection[i].Name == "IsDefaultOut")
                    {
                        if ((bool)o.CustomPropertyCollection["IsDefaultOut"].Value == true)
                        {
                            op = o;
                            break;
                        }
                    }
                }
            }
            return op;
        }

        public string DefaultOutputName
        {
            get { return GetDefaultOutput().Name; }
            set { GetDefaultOutput().Name = value; }
        }

        #endregion

        #endregion

        #region Outputs
                
        public ISOutput AddConditionalOutput(string outputName, int evaluationOrder, string condition, string conditionColumns = "")
        {
            ISOutput condOutput = new ISOutput(this, outputName);
            condOutput.SetCustomProperty("EvaluationOrder", evaluationOrder);
            SetFriendlyExpression(condOutput, condition, conditionColumns);
            return condOutput;
        }

        internal void SetFriendlyExpression(ISOutput condOutput, string condition, string conditionColumns = "")
        {
            IDTSInput100 _input = ComponentMetaData.InputCollection[0];
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
                SetInputColumnDTSUsageType(_input, column, UsageType.UT_READONLY);

            condOutput.SetCustomProperty("FriendlyExpression", condition);
        }

        #endregion
    }
}
