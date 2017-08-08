using Microsoft.SqlServer.Dts.Tasks.ExecutePackageTask;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISExecutePackageTask : ISTaskHost
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the a name for the executable and the immediate parent.
        /// </summary>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISExecutePackageTask(string displayName, ISContainer immediateContainer):
            base("Microsoft.ExecutePackageTask", displayName, immediateContainer)
        {
            ExecutePackageTask = TaskHost.InnerObject as ExecutePackageTask;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal ExecutePackageTask ExecutePackageTask { get; set; }

        #endregion

        #region Dts Object Properties

        //Connection
        public string Connection
        {
            get { return ExecutePackageTask.Connection; }
            set { ExecutePackageTask.Connection = value; }
        }

        //Execute Out of Process
        public bool ExecuteOutOfProcess
        {
            get { return ExecutePackageTask.ExecuteOutOfProcess; }
            set { ExecutePackageTask.ExecuteOutOfProcess = value; }
        }

        //PackageId
        public string PackageID
        {
            get { return ExecutePackageTask.PackageID; }
            set { ExecutePackageTask.PackageID = value; }
        }

        //PackageName
        public string PackageName
        {
            get { return ExecutePackageTask.PackageName; }
            set
            {
                if (value.EndsWith(".dtsx"))
                    ExecutePackageTask.PackageName = value;
                else
                    ExecutePackageTask.PackageName = value + ".dtsx";
            }
        }

        //PackagePassword
        public string PackagePassword
        {
            set { ExecutePackageTask.PackagePassword = value; }
        }

        //UseProjectReference
        public bool UseProjectReference
        {
            get { return ExecutePackageTask.UseProjectReference; }
            set { ExecutePackageTask.UseProjectReference = value; }
        }

        //ParameterAssignments
        internal IDTSParameterAssignments ParameterAssignments_m
        {
            get { return ExecutePackageTask.ParameterAssignments; }
        }

        public List<ISExecutePackageTaskParameterBinding> ParameterAssignments
        {
            get
            {
                List<ISExecutePackageTaskParameterBinding> pms = new List<ISExecutePackageTaskParameterBinding>();
                foreach (IDTSParameterAssignment pa in ParameterAssignments_m)
                {
                    pms.Add(new ISExecutePackageTaskParameterBinding(pa));
                }
                return pms;
            }
        }

        //VersionID
        public string VersionID
        {
            get { return ExecutePackageTask.VersionID; }
            set { ExecutePackageTask.VersionID = value; }
        }

        #endregion

        #region Methods

        #region Add a Parameter Assignment

        internal IDTSParameterAssignment AddParameterAssignment(string bindedVariableOrParameterName, string childParameterName)
        {
            IDTSParameterAssignment prmAss = ExecutePackageTask.ParameterAssignments.Add();
            prmAss.BindedVariableOrParameterName = bindedVariableOrParameterName;
            prmAss.ParameterName = childParameterName;
            return prmAss;
        }

        #endregion

        #endregion
    }
}
