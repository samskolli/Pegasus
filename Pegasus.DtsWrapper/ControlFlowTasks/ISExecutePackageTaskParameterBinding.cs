using Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;

namespace Pegasus.DtsWrapper
{
    public class ISExecutePackageTaskParameterBinding
    {
        #region DtsObject

        private IDTSParameterAssignment _paramAss;

        #endregion

        #region Properties
        /// <summary>
        /// The Parameter in the CHild Package to which assignment needs to be made
        /// </summary>
        public string ChildParameterName
        {
            get { return _paramAss.ParameterName; }
            set { _paramAss.ParameterName = value; }
        }

        /// <summary>
        /// The Variable or the Parameter in the Calling Package whose value gets passed to the Child Package's parameter
        /// </summary>
        public string BindedVariableOrParameterName
        {
            get { return _paramAss.BindedVariableOrParameterName; }
            set { _paramAss.BindedVariableOrParameterName = value; }
        }

        #endregion

        #region ctor

        /// <summary>
        /// A ctor that accepts a IDTSParameterAssignment object
        /// </summary>
        /// <param name="parameterAssignment"></param>
        internal ISExecutePackageTaskParameterBinding(IDTSParameterAssignment parameterAssignment)
        {
            _paramAss = parameterAssignment;
        }

        /// <summary>
        /// A ctor that accepts the Execute Package Task, the Variable/Parameter of the Calling Package and the Parameter from the Child Package
        /// </summary>
        /// <param name="executePackageTask"></param>
        /// <param name="bindedVariableOrParameterName"></param>
        /// <param name="childParameterName"></param>
        public ISExecutePackageTaskParameterBinding(ISExecutePackageTask executePackageTask, string bindedVariableOrParameterName, string childParameterName)
        {
            //System.Console.WriteLine("Received " + executePackageTask.Name + " -- " + bindedVariableOrParameterName + " -- " + childParameterName);
            //bool paramAssExists = false;
            //foreach (IDTSParameterAssignment pa in executePackageTask.ParameterAssignments_m)
            //{
            //    if (pa.ParameterName == childParameterName)
            //    {
            //        paramAssExists = true;
            //        _paramAss = pa;
            //        _paramAss.BindedVariableOrParameterName = bindedVariableOrParameterName;
            //    }
            //}
            //if (!(paramAssExists))
            //{
            //    _paramAss = executePackageTask.AddParameterAssignment(bindedVariableOrParameterName, childParameterName);
            //}
            executePackageTask.AddParameterAssignment(bindedVariableOrParameterName, childParameterName);
        }

        #endregion
    }
}
