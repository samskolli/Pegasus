using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISTaskHost : ISEventsProvider
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        internal ISTaskHost(string objectMoniker, string displayName, ISContainer immediateContainer):
            base(objectMoniker, displayName, immediateContainer)
        {
            TaskHost = (TaskHost)EventsProvider;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal TaskHost TaskHost { get; set; }

        #endregion

        #region Dts Object Properties

        /// <summary>
        /// Result of the task execution. 
        /// </summary>
        public object ExecutionValue { get { return TaskHost.ExecutionValue; } }

        /// <summary>
        /// The custom Variable that contains the execution results of the task.
        /// </summary>
        public ISVariable ExecValueVariable
        {
            get { return new ISVariable(TaskHost.ExecValueVariable); }
            set {TaskHost.ExecValueVariable = value.Variable ; }
        }

        /// <summary>
        /// Indicates whether the TaskHost object has expressions.
        /// </summary>
        public bool HasExpressions { get { return TaskHost.HasExpressions; } }

        /// <summary>
        /// Describes the host of the container.
        /// </summary>
        public ObjectHostType HostType { get { return DtsUtility.EnumAToEnumB<DTSObjectHostType, ObjectHostType>(TaskHost.HostType);  } }

        /// <summary>
        /// The method used to access the task instance being hosted by the TaskHost.
        /// </summary>
        public object InnerObject { get { return TaskHost.InnerObject; } }

        /// <summary>
        /// The properties associated with the task. This is for internal use within this assembly.
        /// </summary>
        internal DtsProperties Properties_m { get { return TaskHost.Properties; } }

        /// <summary>
        /// The properties associated with the task.
        /// </summary>
        public List<ISProperty> Properties
        {
            get
            {
                List<ISProperty> _properties = new List<ISProperty>();
                foreach(DtsProperty property in Properties_m)
                {
                    _properties.Add(new ISProperty(property));
                }
                return _properties;
            }
        }

        #endregion

        #region Methods

        #region Expression Methods

        /// <summary>
        /// Set Expression to the Task
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetExpression(string property, string value)
        {
            TaskHost.Properties[property].SetExpression(TaskHost, value);
        }

        /// <summary>
        /// Get the Expression Value
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public string GetExpression(string property)
        {
            return TaskHost.Properties[property].GetExpression(TaskHost);
        }

        /// <summary>
        /// Remove an expression
        /// </summary>
        /// <param name="property"></param>
        public void RemoveExpression(string property)
        {
            SetExpression(property, null);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            TaskHost.Dispose();
        }

        #endregion

        #endregion
    }
}
