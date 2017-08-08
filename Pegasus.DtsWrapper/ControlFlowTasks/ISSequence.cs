using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISSequence : ISEventsProviderAsIDTSSequence
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISSequence(string displayName, ISContainer immediateContainer):
            base("STOCK:Sequence", displayName, immediateContainer)
        {
            Sequence = EventsProvider as Sequence;
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.Sequence object

        /// <summary>
        /// A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.EventsProvider object
        /// </summary>
        /// <param name="sequence"></param>
        internal ISSequence(Sequence sequence) : base((EventsProvider)sequence)
        {
            Sequence = sequence;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        public Sequence Sequence { get; set; }

        #endregion

        #region Expression Methods

        public void SetExpression(string property, string value)
        {
            Sequence.Properties[property].SetExpression(Sequence, value);
        }

        public string GetExpression(string property, string value)
        {
            return Sequence.Properties[property].GetExpression(Sequence);
        }

        public void RemoveExpression(string property)
        {
            SetExpression(property, null);
        }

        #endregion
    }
}
