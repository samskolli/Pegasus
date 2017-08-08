using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISMulticastComponent : ISPipelineComponent
    {
        #region ctor

        /// <summary>
        /// ctor that accepts the parent data flow task and a name for the row component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        public ISMulticastComponent(ISDataFlowTask parentDataFlowTask, string componentName) :
            base(parentDataFlowTask, "Microsoft.Multicast", componentName)
        {
            _numberOfInputsAllowed = 1;
            _numberOfOutputsAllowed = -1;
        }

        /// <summary>
        /// an extended ctor that also connects to another component
        /// </summary>
        /// <param name="parentDataFlowTask"></param>
        /// <param name="componentName"></param>
        /// <param name="sourceComponent"></param>
        /// <param name="sourceOutputName"></param>
        public ISMulticastComponent(ISDataFlowTask parentDataFlowTask, string componentName, ISPipelineComponent sourceComponent,
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

        #region Add Output

        public ISOutput AddOutput(string outputName)
        {
            return new ISOutput(this, outputName, this.ComponentMetaData.OutputCollection.Count - 1, InsertPlacement.IP_AFTER);
        }

        #endregion
    }

    public class ISMulticastOutput
    {
        #region Parent Component

        private ISMulticastComponent _parentComponent;

        #endregion

        #region Output

        private ISOutput _output;

        #endregion

        #region Name

        public string Name
        {
            get { return _output.Name; }
            set { _output.Name = value; }
        }

        #endregion

        #region ctor

        public ISMulticastOutput(ISMulticastComponent parentComponent, string outputName)
        {
            _parentComponent = parentComponent;
            _output = _parentComponent.AddOutput(outputName);
        }

        #endregion
    }
}