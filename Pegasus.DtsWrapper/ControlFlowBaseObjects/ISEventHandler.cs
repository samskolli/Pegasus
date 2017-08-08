using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISEventHandler : ISEventsProviderAsIDTSSequence
    {
        #region Object Model Property

        internal DtsEventHandler EventHandler { get; set; }
        //internal DtsEventHandler EventHandler { get { return (Executable as DtsEventHandler); } set { } }

        #endregion

        #region ctor

        /// <summary>
        /// ctor that takes in a Event Name and the container the event handler should belong to
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="attachedContainer"></param>
        public ISEventHandler(string eventName, ISEventsProvider attachedContainer) : base(eventName, attachedContainer)
        {
            EventHandler = (Executable as DtsEventHandler);
        }

        public ISEventHandler(SSISEventHandler eventName, ISEventsProvider attachedContainer) : this(eventName.ToString(), attachedContainer)
        {

        }

        #endregion

        #region Properties

        //  TODO: Wrap the following dts object properties if needed.

        //Executables
        //HasExpressions
        //PrecedenceConstraints
        //Properties


        #endregion
    }
}
