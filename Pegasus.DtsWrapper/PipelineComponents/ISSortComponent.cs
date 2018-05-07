using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    
    public class ISSortComponent : ISPipelineComponent
    {
        #region Props

        private IDTSInput100 _input;

        #endregion

        #region ctor

        /// <summary>
        /// ctor that accepts the parent data flow task and a name for the row component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        private ISSortComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.Sort", componentName)
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
        public ISSortComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent,
            string sourceOutputName = "") :
            this(parentDataFlowTask, componentName)
        {
            //  After adding hte derived column transformation, connect it to a prevoius component
            if (String.IsNullOrEmpty(sourceOutputName))
                ConnectToAnotherPipelineComponent(sourceComponent.Name);
            else
                ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceOutputName);

            // set all usage to READONLY for all;
            string[] viCols = new string[_input.GetVirtualInput().VirtualInputColumnCollection.Count];
            for (int i = 0; i < viCols.Length; i++)
            {
                ISInputColumn ic = new ISInputColumn(this, _input.Name, _input.GetVirtualInput().VirtualInputColumnCollection[i].Name, UsageType.UT_READONLY, RowDisposition.RD_NotUsed, RowDisposition.RD_NotUsed);
                SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, ic.Name), "NewComparisonFlags", 0);
                SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, ic.Name), "NewSortKeyPosition", 0);
            }

            /*foreach (IDTSVirtualInputColumn100 vc in _input.GetVirtualInput().VirtualInputColumnCollection)
            {
                Console.WriteLine("Working on " + vc.Name);
                ISInputColumn ic = new ISInputColumn(this, _input.Name, vc.Name, UsageType.UT_READONLY, RowDisposition.RD_NotUsed, RowDisposition.RD_NotUsed);
                SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, ic.Name), "NewComparisonFlags", 0);
                SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, ic.Name), "NewSortKeyPosition", 0);
            }*/
        }

        #endregion

        #region Component Properties

        #region Eliminate Duplicates

        /// <summary>
        /// Specifies if duplicates should be eliminated
        /// </summary>
        public bool EliminateDuplicates
        {
            get { return CustomPropertyGetter<bool>("EliminateDuplicates"); }
            set { CustomPropertySetter<bool>("EliminateDuplicates", value); }
        }

        #endregion

        #region MaximumThreads

        /// <summary>
        /// Specifies if duplicates should be eliminated
        /// </summary>
        public int MaximumThreads
        {
            get { return CustomPropertyGetter<int>("MaximumThreads"); }
            set { CustomPropertySetter<int>("MaximumThreads", value); }
        }

        #endregion

        #endregion

        #region Methods

        public void SetColumnSortInformation(String columnName, int sortKeyPosition, StringComparisonFlag stringComparisonFlag)
        {
            ISInputColumn inputColumn = new ISInputColumn(this, _input.Name, columnName, UsageType.UT_READONLY, RowDisposition.RD_NotUsed, RowDisposition.RD_NotUsed);
            SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, columnName), "NewComparisonFlags", (int)stringComparisonFlag);
            SetCustomPropertyToInputColumn(_input, GetInputColumn(_input.Name, columnName), "NewSortKeyPosition", sortKeyPosition);
        }

        #endregion
    }
}
