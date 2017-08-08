using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISPrecedenceConstraint
    {
        #region ctor

        #region Constructor that accepts the parent container, the preceding and succeeding executables

        /// <summary>
        /// A ctor that accepts the parent container, the preceding and succeeding executables.
        /// </summary>
        /// <param name="parentContainer">The parent container in which the precedence constraint is to be added.</param>
        /// <param name="precedingExecutable">The preceding executable.</param>
        /// <param name="succeedingExecutable">The constrained or the succeeding executable.</param>
        public ISPrecedenceConstraint(ISEventsProviderAsIDTSSequence parentContainer, ISExecutable precedingExecutable, ISExecutable succeedingExecutable)
        {
            bool precedenceConstraintExists = false;
            for (int p = 0; p < parentContainer.PrecedenceConstraints_m.Count; p++)
            {
                PrecedenceConstraint pc = parentContainer.PrecedenceConstraints_m[p];
                if (pc.ConstrainedExecutable == succeedingExecutable.Executable && pc.PrecedenceExecutable == precedingExecutable.Executable)
                {
                    precedenceConstraintExists = true;
                    PrecedenceConstraint = pc;
                }
            }

            if (!(precedenceConstraintExists))
            {
                PrecedenceConstraint = parentContainer.PrecedenceConstraints_m.Add(precedingExecutable.Executable, succeedingExecutable.Executable);
            }
        }

        #endregion

        #region Constructor that accepts a Microsoft.SqlServer.Dts.Runtime.PrecedenceConstraint object

        /// <summary>
        /// A ctor that accepts a Microsoft.SqlServer.Dts.Runtime.PrecedenceConstraint object.
        /// </summary>
        /// <param name="precedenceConstraint">The Microsoft.SqlServer.Dts.Runtime.PrecedenceConstraint object.</param>
        internal ISPrecedenceConstraint(PrecedenceConstraint precedenceConstraint)
        {
            PrecedenceConstraint = precedenceConstraint;
        }

        #endregion

        #region Extended Constructor that also accepts a EvalOp and Value

        public ISPrecedenceConstraint(ISEventsProviderAsIDTSSequence parentContainer, ISExecutable precedingExecutable, ISExecutable succeedingExecutable, PrecedenceEvalOp evalOp, ExecResult value)
            : this(parentContainer, precedingExecutable, succeedingExecutable)
        {
            EvalOp = evalOp;
            Value = value;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal PrecedenceConstraint PrecedenceConstraint;

        #endregion

        #region Dts Object Properties

        /// <summary>
        /// The "constrained" (or succeding) container. This property is only for internal use within this assembly.
        /// </summary>
        internal Executable ConstrainedExecutable_m { get { return PrecedenceConstraint.ConstrainedExecutable; } }

        /// <summary>
        /// The string used by the Integration Services runtime to create an instance of the PrecedenceConstraint object and add the object to the PrecedenceConstraints collection.
        /// </summary>
        public string CreationName { get { return PrecedenceConstraint.CreationName; } }

        /// <summary>
        /// Description for the constraint.
        /// </summary>
        public string Description { get { return PrecedenceConstraint.Description; } set { PrecedenceConstraint.Description = value; } }

        /// <summary>
        /// The evaluation operations used by the precedence constraint.
        /// </summary>
        public PrecedenceEvalOp EvalOp
        {
            get {
                    return DtsUtility.EnumAToEnumB<DTSPrecedenceEvalOp, PrecedenceEvalOp>(PrecedenceConstraint.EvalOp);
                }
            set {
                    PrecedenceConstraint.EvalOp = DtsUtility.EnumAToEnumB<PrecedenceEvalOp, DTSPrecedenceEvalOp>(value);
                }
        }

        /// <summary>
        /// Indicates whether the specified Value property evaluates to true. 
        /// </summary>
        public bool EvaluatesTrue { get { return PrecedenceConstraint.EvaluatesTrue; } }

        /// <summary>
        ///  The expression that the precedence constraint uses if EvalOp is set to Expression, ExpressionAndConstraint, or ExpressionOrConstraint.
        /// </summary>
        public string Expression { get { return PrecedenceConstraint.Expression; } set { PrecedenceConstraint.Expression = value; } }

        /// <summary>
        /// GUID of the precedence constraint.
        /// </summary>
        public string ID { get { return PrecedenceConstraint.ID; } }

        /// <summary>
        /// Indicates whether multiple constraints work together.
        /// </summary>
        public bool LogicalAnd { get { return PrecedenceConstraint.LogicalAnd; } set { PrecedenceConstraint.LogicalAnd = value; } }

        /// <summary>
        /// The unique name of the precedence constraint.
        /// </summary>
        public string Name { get { return PrecedenceConstraint.Name; } set { PrecedenceConstraint.Name = value; } }

        /// <summary>
        /// The parent container of the PrecedenceConstraint. This property is only for internal use within this assembly.
        /// </summary>
        internal DtsContainer Parent_m { get { return PrecedenceConstraint.Parent; } }

        /// <summary>
        /// /// The "preceding" container. This property is only for internal use within this assembly.
        /// </summary>
        public Executable PrecedenceExecutable_m { get { return PrecedenceConstraint.PrecedenceExecutable; } }

        /// <summary>
        /// The constraint type of Success, Failure, or Completion.
        /// </summary>
        public ExecResult Value
        {
            get {return DtsUtility.EnumAToEnumB<DTSExecResult, ExecResult>(PrecedenceConstraint.Value); }
            set {PrecedenceConstraint.Value = DtsUtility.EnumAToEnumB<ExecResult, DTSExecResult>(value); }
        }

        #endregion
    }
}
