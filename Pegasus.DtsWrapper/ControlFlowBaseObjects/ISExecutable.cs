using Microsoft.SqlServer.Dts.Runtime; // uses the Microsoft.SqlServer.ManagedDTS assembly

namespace Pegasus.DtsWrapper
{
    public class ISExecutable
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// If immediateContainer is null, then it is assumed that we want to create the Package object itself.
        ///     Otherwise: If an executable does not already exist in the parent container, it is created and added to the parent container
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        internal ISExecutable(string objectMoniker, string displayName, ISContainer immediateContainer)
        {
            _objectMoniker = objectMoniker;
            _displayName = displayName;
            _immediateContainer = immediateContainer;

            if (_immediateContainer != null)
                if (!(_immediateContainer.Container as IDTSSequence).Executables.Contains(displayName))
                    Executable = ((IDTSSequence)_immediateContainer.Container).Executables.Add(objectMoniker);
                else
                {
                    ExecutableAlreadyExists = true;
                    Executable = ((IDTSSequence)_immediateContainer.Container).Executables[displayName];
                }
                    
            else
                Executable = new Package();
        }

        #endregion

        #region ctor that accepts a Executable object

        internal ISExecutable(Executable executable)
        {
            Executable = executable;
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
        public ISExecutable(string packageName, ISProject project)
        {
            
        }

        #endregion

        #region Event Handler related ctor

        /// <summary>
        /// This constructor is to facilitate adding/accessing Event Handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attachedContainer"></param>
        internal ISExecutable(string eventName, ISEventsProvider attachedContainer)
        {
            bool eventHandlerExists = false;
            for (int e = 0; e < attachedContainer.EventHandlers_m.Count; e++)
            {
                if (attachedContainer.EventHandlers_m[e].CreationName == eventName)
                {
                    eventHandlerExists = true;
                    Executable = (attachedContainer.EventHandlers_m[e] as Executable);
                }
            }
            if (eventHandlerExists == false)
                Executable = (attachedContainer.EventHandlers_m.Add(eventName) as Executable);
        }

        #endregion

        #endregion

        #region Wrapped DtsObject

        /// <summary>
        /// The DtsObject that this Type wraps; which is Microsoft.SqlServer.Dts.Runtime.Executable
        /// </summary>
        internal Executable Executable;

        #endregion

        #region Properties

        /// <summary>
        /// Field to hold the object moniker; Object moniker is the unique string that differentiates between various Control Flow Tasks
        /// </summary>
        private string _objectMoniker;

        /// <summary>
        /// The name that the Control Flow object gets. This is what gets displayed in the UI.
        /// </summary>
        private string _displayName;

        /// <summary>
        /// The parent object to which the Executable belongs to. These are objects such as Sequence container or a Package etc.
        /// </summary>
        private ISContainer _immediateContainer;

        /// <summary>
        /// The root SSIS Project that this executable ultimately belongs to. Valid only in cases where an SSIS project is used.
        /// </summary>
        private ISProject _ssisProject;

        internal bool ExecutableAlreadyExists;

        #endregion

    }
}
