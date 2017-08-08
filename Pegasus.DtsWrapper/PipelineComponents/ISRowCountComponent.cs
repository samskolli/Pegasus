using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.DtsWrapper
{
    public class ISRowCountComponent : ISPipelineComponent
    {
        #region ctor

        /// <summary>
        /// ctor that accepts the parent data flow task and a name for the row component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        public ISRowCountComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.RowCount", componentName)
        {
            _numberOfInputsAllowed = 1;
            _numberOfOutputsAllowed = 1;
        }

        /// <summary>
        /// an extended ctor that also connects to another component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        /// <param name="sourceComponent"></param>
        /// <param name="sourceOutputName"></param>
        public ISRowCountComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent, 
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

        #region Component Properties

        #region Variable Name

        /// <summary>
        /// Sets/Returns the VariableName assigned to the Row Count component
        /// When setting the VariableName, it shud be in the format of nameSpace::variableName.
        /// E.g.: rowComp.VariableName = "User::Delete_Row_Count"; where Delete_Row_Count is a variable in the User namespace.
        /// You can use the AssignVariable() method to specify the namespace and variable name separately.
        /// </summary>
        public string VariableName
        {
            get { return CustomPropertyGetter<string>("VariableName"); }
            set { CustomPropertySetter<string>("VariableName", value); }
        }

        private ISVariable _variable;
        public ISVariable Variable
        {
            get { return _variable; }
            set
            {
                _variable = value;
                CustomPropertySetter<string>("VariableName", _variable.QualifiedName);
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Assign Variable

        public void AssignVariable(string nameSpace, string variableName)
        {
            VariableName = nameSpace + "::" + variableName;
        }

        public void AssignVariable(ISVariable variable)
        {
            VariableName = variable.QualifiedName;
        }

        #endregion

        #endregion
    }
}