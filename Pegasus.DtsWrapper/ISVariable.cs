using Microsoft.SqlServer.Dts.Runtime;
using System;

namespace Pegasus.DtsWrapper
{
    /// <summary>
    /// This type is a wrapper/helper class to create/read/write Variables in an SSIS package.
    /// </summary>
    public class ISVariable
    {
        #region ctor

        #region Constructor that accepts a Name, whether Readonly or not, Namespace, Value, Expression and the parent to which the variables needs to be added.

        /// <summary>
        /// Constructor that accepts a Name, whether Readonly or not, Namespace, Value, Expression and the parent to which the variables needs to be added.
        /// Also, there is a default parameter that accepts the data type 
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <param name="readOnly">whether Readonly or not</param>
        /// <param name="nameSpace">Namespace</param>
        /// <param name="val">Value</param>
        /// <param name="expression">Expression</param>
        /// <param name="parentContainer">Parent Container</param>
        /// <param name="userSpecifiedDataType"> A data type to be specified by the user. Use the VariableDataType enum for accepted values.</param>
        public ISVariable(string variableName, bool readOnly, string nameSpace, object val, string expression, ISContainer parentContainer, VariableDataType userSpecifiedDataType = VariableDataType.String)
        {
            bool variableExists = parentContainer.Variables_m.Contains(variableName);
            if (variableExists)
            {
                Variable = parentContainer.Variables_m[variableName];
            }
            if (!(variableExists))
            {
                switch (userSpecifiedDataType.ToString().ToLower())
                {
                    case "boolean":
                        val = Convert.ToBoolean(val);
                        break;
                    case "byte":
                        val = Convert.ToByte(val);
                        break;
                    case "char":
                        val = Convert.ToChar(val);
                        break;
                    case "datetime":
                        val = Convert.ToDateTime(val);
                        break;
                    case "decimal":
                        val = Convert.ToDecimal(val);
                        break;
                    case "double":
                        val = Convert.ToDouble(val);
                        break;
                    case "int16":
                        val = Convert.ToInt16(val);
                        break;
                    case "int32":
                        val = Convert.ToInt32(val);
                        break;
                    case "int64":
                        val = Convert.ToInt64(val);
                        break;
                    case "sbyte":
                        val = Convert.ToSByte(val);
                        break;
                    case "single":
                        val = Convert.ToSingle(val);
                        break;
                    case "string":
                        val = Convert.ToString(val);
                        break;
                    case "uint32":
                        val = Convert.ToUInt32(val);
                        break;
                    case "uint64":
                        val = Convert.ToUInt64(val);
                        break;
                    default:
                        break;
                }
                Variable = parentContainer.Variables_m.Add(variableName, readOnly, nameSpace, val);
                Variable.Expression = expression;
                Variable.EvaluateAsExpression = String.IsNullOrEmpty(expression) ? false : true;
            }
        }

        #endregion

        #region Constructor that accepts a Microsoft.SqlServer.Dts.RunTime.Variable object

        /// <summary>
        /// Constructor that accepts a Microsoft.SqlServer.Dts.Runtime.Variable object. Mostly for use within this assembly.
        /// </summary>
        /// <param name="variable">The Microsoft.SqlServer.Dts.Runtime.Variable object</param>
        internal ISVariable(Variable variable)
        {
            Variable = variable;
        }

        #endregion

        #endregion

        #region Wrapped DtsObject

        /// <summary>
        /// The DtsObject that this Type wraps; which is Microsoft.SqlServer.Dts.Runtime.Variable
        /// </summary>
        internal Variable Variable;

        #endregion

        #region DtsObject Properties

        /// <summary>
        /// Expression contained in a variable.
        /// </summary>
        public string Expression { get { return Variable.Expression; } set { Variable.Expression = value; } }

        /// <summary>
        /// String that the runtime engine gives when it creates an instance of the Variable object and adds the object to the Variables collection.
        /// </summary>
        public string CreationName { get { return Variable.CreationName; } }

        /// <summary>
        /// Describes the data type of the variable.
        /// </summary>
        public string DataType { get { return Variable.DataType.ToString(); } }

        /// <summary>
        /// Description of the Variable.
        /// </summary>
        public string Description { get { return Variable.Description; } set { Variable.Description = value; } }

        /// <summary>
        /// Indicates whether the variable contains an expression.
        /// </summary>
        public bool EvaluateAsExpression { get { return Variable.EvaluateAsExpression; } set { Variable.EvaluateAsExpression = value; } }

        /// <summary>
        /// GUID assigned to the variable.
        /// </summary>
        public string ID { get { return Variable.ID; } }

        /// <summary>
        /// Name of the Variable.
        /// </summary>
        public string Name { get { return Variable.Name; } set { Variable.Name = value; } }

        /// <summary>
        /// Namespace to which the variable belongs to.
        /// </summary>
        public string Namespace { get { return Variable.Namespace; } set { Variable.Namespace = value; } }

        /// <summary>
        /// The parent on which the variable is declared/assigned to. This property is the internal version and is to be used internally in this assembly.
        /// </summary>
        internal DtsContainer Parent_m { get { return Variable.Parent; } }

        /// <summary>
        /// The parent on which the variable is declared/assigned to. 
        /// </summary>
        public ISContainer Parent { get { return (ISContainer)Parent_m; } }

        /// <summary>
        /// Collection of DtsProperty objects.
        /// </summary>
        internal DtsProperties Properties_m { get { return Variable.Properties; } }

        /// <summary>
        /// The fully qualified name of the variable, including the namespace.
        /// </summary>
        public string QualifiedName { get { return Variable.QualifiedName; } }

        /// <summary>
        /// Indicates if the variable has been flagged to raise the OnVariableValueChanged event when the value of the variable changes.
        /// </summary>
        public bool RaiseChangedEvent { get { return Variable.RaiseChangedEvent; } set { Variable.RaiseChangedEvent = value; } }

        /// <summary>
        /// Indicates that the variable is read-only and cannot have its value modified.
        /// </summary>
        public bool ReadOnly { get { return Variable.ReadOnly; } set { Variable.ReadOnly = value; } }

        /// <summary>
        /// Indicates whether the variable is a system variable.
        /// </summary>
        public bool SystemVariable { get { return Variable.SystemVariable; } }

        /// <summary>
        /// Value assigned to the Variable.
        /// </summary>
        public object Value
        {
            get { return Variable.Value; }
            set
            {
                if (ReadOnly == false)
                    Variable.Value = value;
            }
        }

        #endregion

        #region Implicit Operator

        /// <summary>
        /// For casting an Variable object to an ISVariable object
        /// </summary>
        /// <param name="variable"></param>
        public static implicit operator ISVariable(Variable variable)
        {
            if (variable == null)
                return null;

            return new ISVariable(variable);
        }

        #endregion
    }
}
