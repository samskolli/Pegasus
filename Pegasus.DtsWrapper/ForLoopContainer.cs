using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ForLoopContainer : ISEventsProviderAsIDTSSequence
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ForLoopContainer(string displayName, ISContainer immediateContainer):
            base("STOCK:FORLOOP", displayName, immediateContainer)
        {
            ForLoop = EventsProvider as ForLoop;
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.Sequence object

        /// <summary>
        /// A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object
        /// </summary>
        /// <param name="sequence"></param>
        internal ForLoopContainer(ForLoop forLoop) : base((EventsProvider)forLoop)
        {
            ForLoop = forLoop;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        public ForLoop ForLoop { get; set; }

        #endregion

        #region Expression Methods

        public void SetExpression(string property, string value)
        {
            ForLoop.Properties[property].SetExpression(ForLoop, value);
        }

        public string GetExpression(string property, string value)
        {
            return ForLoop.Properties[property].GetExpression(ForLoop);
        }

        public void RemoveExpression(string property)
        {
            SetExpression(property, null);
        }

        #endregion

        #region DtsObjectProperties

        #region AssignExpression

        /// <summary>
        /// The name of the computer on which the package was created.
        /// </summary>
        public string AssignExpression
        {
            get { return ForLoop.AssignExpression; }
            set { ForLoop.AssignExpression = value; }
        }

        #endregion

        #region EvalExpression

        /// <summary>
        /// The name of the computer on which the package was created.
        /// </summary>
        public string EvalExpression
        {
            get { return ForLoop.EvalExpression; }
            set { ForLoop.EvalExpression = value; }
        }

        #endregion

        #region HasExpressions

        /// <summary>
        /// The name of the computer on which the package was created.
        /// </summary>
        public bool HasExpressions
        {
            get { return ForLoop.HasExpressions; }
        }

        #endregion

        #region InitExpression

        /// <summary>
        /// The name of the computer on which the package was created.
        /// </summary>
        public string InitExpression
        {
            get { return ForLoop.InitExpression; }
            set { ForLoop.InitExpression = value; }
        }

        #endregion

        #endregion


    }
}
