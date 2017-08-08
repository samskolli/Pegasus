using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISEventsProviderAsIDTSSequence : ISEventsProvider
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        internal ISEventsProviderAsIDTSSequence(string objectMoniker, string displayName, ISContainer immediateContainer):
            base(objectMoniker, displayName, immediateContainer)
        {
            EventsProviderAsIDTSSequence = (IDTSSequence)Container;
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object

        /// <summary>
        /// A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object
        /// </summary>
        /// <param name="eventsProvider"></param>
        internal ISEventsProviderAsIDTSSequence(EventsProvider eventsProvider) : base(eventsProvider)
        {
            EventsProviderAsIDTSSequence = (IDTSSequence)eventsProvider;
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
        public ISEventsProviderAsIDTSSequence(string packageName, ISProject project) : base(packageName, project)
        {
            
        }

        #endregion

        #region Event Handler related ctor

        /// <summary>
        /// This constructor is to facilitate adding/accessing Event Handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attachedContainer"></param>
        internal ISEventsProviderAsIDTSSequence(string eventName, ISEventsProvider attachedContainer) : base(eventName, attachedContainer)
        {
            EventsProviderAsIDTSSequence = (IDTSSequence)Container;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        /// <summary>
        /// The DtsObject that this Type wraps; which is Microsoft.SqlServer.Dts.Runtime.IDTSSequence
        /// </summary>
        internal IDTSSequence EventsProviderAsIDTSSequence { get; set; }

        #endregion

        #region Dts Object Properties

        /// <summary>
        /// The child executables contained in the container object. This property is only for internal use within this assembly.
        /// </summary>
        internal Executables Executables_m
        {
            get { return EventsProviderAsIDTSSequence.Executables; }
        }

        /// <summary>
        /// The child executables contained in the container object.
        /// </summary>
        public List<ISEventsProvider> Executables
        {
            get
            {
                List<ISEventsProvider> _eventsProviders = new List<ISEventsProvider>();
                foreach (Executable executable in Executables_m)
                {
                    _eventsProviders.Add(new ISEventsProvider((EventsProvider)executable));
                }
                return _eventsProviders;
            }
        }

        /// <summary>
        /// The precedence constraints contained in the container object. This property is only for internal use within this assembly.
        /// </summary>
        internal PrecedenceConstraints PrecedenceConstraints_m
        {
            get { return EventsProviderAsIDTSSequence.PrecedenceConstraints; }
        }

        /// <summary>
        /// The precedence constraints contained in the container object.
        /// </summary>
        public List<ISPrecedenceConstraint> PrecedenceConstraints
        {
            get
            {
                List<ISPrecedenceConstraint> _precedenceConstraints = new List<ISPrecedenceConstraint>();
                foreach(PrecedenceConstraint pc in PrecedenceConstraints_m)
                {
                    _precedenceConstraints.Add(new ISPrecedenceConstraint(pc));
                }
                return _precedenceConstraints;
            }
        }

        #endregion
    }
}
