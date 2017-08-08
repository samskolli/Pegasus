using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISEventsProvider : ISContainer
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISEventsProvider(string objectMoniker, string displayName, ISContainer immediateContainer):
            base(objectMoniker, displayName, immediateContainer)
        {
            EventsProvider = (EventsProvider)Container;
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object

        /// <summary>
        /// A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object
        /// </summary>
        /// <param name="eventsProvider"></param>
        internal ISEventsProvider(EventsProvider eventsProvider) : base((DtsContainer)eventsProvider)
        {
            EventsProvider = eventsProvider;
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
        public ISEventsProvider(string packageName, ISProject project) : base(packageName, project)
        {
            
        }

        #endregion

        #region Event Handler related ctor

        /// <summary>
        /// This constructor is to facilitate adding/accessing Event Handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attachedContainer"></param>
        internal ISEventsProvider(string eventName, ISEventsProvider attachedContainer) : base(eventName, attachedContainer)
        {
            //EventsProvider = (EventsProvider)Container;
        }

        #endregion

        #endregion

        #region Wrapped DtsObject

        /// <summary>
        /// The DtsObject that this Type wraps; which is Microsoft.SqlServer.Dts.Runtime.EventsProvider
        /// </summary>
        internal EventsProvider EventsProvider { get; set; }

        #endregion

        #region Dts Object Properties
        
        /// <summary>
        /// Indicates whether the event handlers on task are disabled. 
        /// </summary>
        public bool DisableEventHandlers { get { return EventsProvider.DisableEventHandlers; } set { EventsProvider.DisableEventHandlers = value; } }

        /// <summary>
        /// A collection of event handler objects. This property is for internal use within this assembly.
        /// </summary>
        internal DtsEventHandlers EventHandlers_m { get { return EventsProvider.EventHandlers; } }

        /// <summary>
        /// A collection of event info objects. This property is for internal use within this assembly.
        /// </summary>
        internal EventInfos EventInfos_m { get { return EventsProvider.EventInfos; } }
        
        #endregion


    }
}
