using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISContainer : ISExecutable
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        internal ISContainer(string objectMoniker, string displayName, ISContainer immediateContainer):
            base(objectMoniker, displayName, immediateContainer)
        {
            Container = (DtsContainer)Executable;
            Name = displayName;
        }

        #endregion

        #region Constructor that accepts a DtsContainer object

        /// <summary>
        /// Constructor that accepts a DtsContainer object. Primarily for implicit operations from DtsContainer objects to ISContainer objects.
        /// </summary>
        /// <param name="container"></param>
        internal ISContainer(DtsContainer container): base((Executable)container)
        {
            Container = container;
        }

        #endregion

        #region ctor that accepts a package name and project

        /// <summary>
        /// A constructor that accepts a Package name and parent project. 
        /// If a package with the same name already exists in the Project, it is associated with the variable on which the constructor is called.
        /// If the package does not exist, then a new package is created and added to the project.
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="project"></param>
        public ISContainer(string packageName, ISProject project) : base(packageName, project)
        {
            
        }

        #endregion

        #region Event Handler related ctor

        /// <summary>
        /// This constructor is to facilitate adding/accessing Event Handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attachedContainer"></param>
        internal ISContainer(string eventName, ISEventsProvider attachedContainer) : base(eventName, attachedContainer)
        {
            Container = (DtsContainer)Executable;
        }

        #endregion

        #endregion

        #region Wrapped DtsObject

        /// <summary>
        /// The DtsObject that this Type wraps; which is Microsoft.SqlServer.Dts.Runtime.DtsContainer
        /// </summary>
        internal DtsContainer Container { get; set; }

        #endregion

        #region DtsObject Properties

        #region CreationName
        /// <summary>
        /// String that is used to create an instance of the DtsContainer object.
        /// </summary>
        public string CreationName
        {
            get { return Container.CreationName; }
        }

        #endregion

        #region DebugMode
        /// <summary>
        /// Indicates whether the DtsContainer object is in debug mode and whether it should fire the OnBreakpointHit event while running.
        /// </summary>
        public bool DebugMode
        {
            get { return Container.DebugMode; }
            set { Container.DebugMode = value; }
        }

        #endregion

        #region DelayValidation
        /// <summary>
        /// Indicates whether validation of the task is delayed until run time. The default value is false.
        /// </summary>
        public bool DelayValidation
        {
            get { return Container.DelayValidation; }
            set { Container.DelayValidation = value; }
        }

        #endregion

        #region Description
        /// <summary>
        /// Description of the DtsContainer object.
        /// </summary>
        public string Description
        {
            get { return Container.Description; }
            set { Container.Description = value; }
        }

        #endregion

        #region Disable
        /// <summary>
        /// Indicates if the DtsContainer object is disabled. 
        /// </summary>
        public bool Disable
        {
            get { return Container.Disable; }
            set { Container.Disable = value; }
        }

        #endregion

        #region ExecutionDuration
        /// <summary>
        /// Value that indicates the amount of time that the container spent in execution, in milliseconds.
        /// </summary>
        public int ExecutionDuration
        {
            get { return Container.ExecutionDuration; }
        }

        #endregion

        #region Execution Result
        /// <summary>
        /// Value that indicates the success or failure of the execution of the DtsContainer object. 
        /// </summary>
        public string ExecutionResult
        {
            get { return Container.ExecutionResult.ToString(); }
        }

        #endregion

        #region ExecutionStatus
        /// <summary>
        /// Value that indicates the current status of the execution of the DtsContainer object at the time of the call.
        /// </summary>
        public string ExecutionStatus
        {
            get { return Container.ExecutionStatus.ToString(); }
        }

        #endregion

        #region FailPackageOnFailure
        /// <summary>
        /// Indicates whether the package fails when a child container fails. This property is used on containers, not the package itself.
        /// </summary>
        public bool FailPackageOnFailure
        {
            get { return Container.FailPackageOnFailure; }
            set { Container.FailPackageOnFailure = value; }
        }

        #endregion

        #region FailParentOnFailure
        /// <summary>
        /// Defines whether the parent container fails when a child container fails.
        /// </summary>
        public bool FailParentOnFailure
        {
            get { return Container.FailParentOnFailure; }
            set { Container.FailParentOnFailure = value; }
        }

        #endregion

        #region ForcedExecutionValue 
        /// <summary>
        /// Value that specifies the optional execution value that the package returns. Only IF the ForceExecutionValue property is set to True.
        /// </summary>
        public object ForcedExecutionValue
        {
            get { return Container.ForcedExecutionValue; }
            set { Container.ForcedExecutionValue = value; }
        }

        #endregion

        #region ForceExecutionResult
        /// <summary>
        /// Value that specifies the forced execution result of the container. 
        /// </summary>
        public string ForceExecutionResult
        {
            get { return Container.ForceExecutionResult.ToString(); }
            set { Container.ForceExecutionResult = DtsUtility.StringToEnum<DTSForcedExecResult>(value); }
        }

        #endregion

        #region ForceExecutionValue
        /// <summary>
        /// Indicates whether the execution value of the container should be forced to contain a particular value.
        /// </summary>
        public bool ForceExecutionValue
        {
            get { return Container.ForceExecutionValue; }
            set { Container.ForceExecutionValue = value; }
        }

        #endregion

        #region ID
        /// <summary>
        /// Gets the GUID of the DtsObject
        /// </summary>
        public string ID
        {
            get { return Container.ID; }
        }

        #endregion

        #region IsDefaultLocaleID
        /// <summary>
        /// Indicates whether the container uses the default locale.
        /// </summary>
        public bool IsDefaultLocaleID
        {
            get { return Container.IsDefaultLocaleID; }
        }

        #endregion

        #region IsolationLevel
        /// <summary>
        /// The isolation level of the transaction in the DtsContainer object. 
        /// </summary>
        public string IsolationLevel
        {
            get { return Container.IsolationLevel.ToString(); }
            set { Container.IsolationLevel = DtsUtility.StringToEnum<System.Data.IsolationLevel>(value); }
        }

        #endregion

        #region LocaleID
        /// <summary>
        /// Indicates the Microsoft Win32® localeID to use when the DtsContainer object is executed.
        /// </summary>
        public int LocaleID
        {
            get { return Container.LocaleID; }
            set { Container.LocaleID = value; }
        }

        #endregion

        #region LoggingMode
        /// <summary>
        /// Indicates the logging mode of the container.
        /// </summary>
        public string LoggingMode
        {
            get { return Container.LoggingMode.ToString(); }
            set { Container.LoggingMode = DtsUtility.StringToEnum<DTSLoggingMode>(value); }
        }

        #endregion

        #region MaximumErrorCount
        /// <summary>
        /// Indicates the maximum number of errors that can occur before the DtsContainer object stops running.
        /// </summary>
        public int MaximumErrorCount
        {
            get { return Container.MaximumErrorCount; }
            set { Container.MaximumErrorCount = value; }
        }

        #endregion

        #region Name
        /// <summary>
        /// Name of the DtsContainer.
        /// </summary>
        public string Name
        {
            get { return Container.Name; }
            set { Container.Name = value; }
        }

        #endregion

        #region Parent
        /// <summary>
        /// The parent container. This property is meant to be used internally within this assembly only.
        /// </summary>
        internal DtsContainer Parent_m
        {
            get { return Container.Parent; }
        }
        
        /// <summary>
        /// The parent container.
        /// </summary>
        public ISContainer Parent
        {
            get { return (ISContainer)Parent_m; }
        }
        
        #endregion

        #region SuspendRequired
        /// <summary>
        /// Indicates if tasks should suspend when they encounter a breakpoint. This value is set by the runtime engine for tasks and containers when a breakpoint is encountered.
        /// </summary>
        public bool SuspendRequired
        {
            get { return Container.SuspendRequired; }
            set { Container.SuspendRequired = value; }
        }

        #endregion

        #region TransactionOption
        /// <summary>
        /// Indicates whether the container participates in transactions.
        /// </summary>
        public string TransactionOption
        {
            get { return Container.TransactionOption.ToString(); }
            set { Container.TransactionOption = DtsUtility.StringToEnum<DTSTransactionOption>(value); }
        }

        #endregion

        #region Variables
        
        /// <summary>
        /// The Variable collection associated with the container object. This property is internal and is meant for use within this assembly.
        /// </summary>
        internal Variables Variables_m
        {
            get { return Container.Variables; }
        }

        /// <summary>
        /// The Variable collection associated with the container object.
        /// </summary>
        public List<ISVariable> Variables
        {
            get
            {
                List<ISVariable> _variables = new List<ISVariable>();
                foreach (Variable variable in Variables_m)
                {
                    _variables.Add(new ISVariable(variable));
                }
                return _variables;
            }
        }

        #endregion

        #endregion

        #region implicit operators

        /// <summary>
        /// For casting an DtsContainer object to an ISContainer object
        /// </summary>
        /// <param name="container"></param>
        public static implicit operator ISContainer(DtsContainer container)
        {
            if (container == null)
                return null;

            return new ISContainer(container);
        }

        #endregion

        #region Methods

        #region Variable Related

        /// <summary>
        /// Adds a variable to the Container object.
        /// </summary>
        /// <param name="variableName">The name of hte variable</param>
        /// <param name="readOnly">Whether readonly or not</param>
        /// <param name="nameSpace">The namespace to which the variable belongs to</param>
        /// <param name="val">The value assigned to the Variable</param>
        /// <param name="expression">The expression with the Variable</param>
        /// <param name="userSpecifiedDataType"> A data type to be specified by the user. Use the VariableDataType enum for accepted values.</param>
        /// <returns></returns>
        public ISVariable AddVariable(string variableName, bool readOnly, string nameSpace, object val, string expression, VariableDataType userSpecifiedDataType = VariableDataType.String)
        {
            return new ISVariable(variableName, readOnly, nameSpace, val, expression, this, userSpecifiedDataType);
        }

        /// <summary>
        /// Access a variable, given the Variable's name
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public ISVariable GetVariable(string variableName)
        {
            return new ISVariable(Variables_m[variableName]);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// This method will return the actual Package object for the inputted ISContainer object.
        /// In some cases:
        ///     this actual Package object will be the immediate parent and 
        ///     in some cases it can be an ancestor at any number of levels high (depending on how the containers are nested).
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        internal static ISContainer GetActualPackage(ISContainer container)
        {
            if (container.Parent_m == null)
                return container;
            else
            {
                return GetActualPackage(container.Parent);
            }
        }

        public static ISContainer FindChildContainer(ISContainer parent, string childContainerName)
        {
            ISContainer childContainer = null;
            for (int c = 0; c < (parent.Container as IDTSSequence).Executables.Count; c++)
            {
                if (((parent.Container as IDTSSequence).Executables[c] as DtsContainer).Name == childContainerName)
                    childContainer = (ISContainer)((parent.Container as IDTSSequence).Executables[c] as DtsContainer);
            }
            return childContainer;
        }

        #endregion

        #endregion
    }
}
