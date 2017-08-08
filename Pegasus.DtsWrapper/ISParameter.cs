using Microsoft.SqlServer.Dts.Runtime;
using System;

namespace Pegasus.DtsWrapper
{
    public class ISParameter
    {
        #region ctor

        #region ctor that accepts a Microsoft.Dts.Runtime.Parameter object

        /// <summary>
        /// A constructor that accepts a Microsoft.Dts.Runtime.Parameter object
        /// </summary>
        /// <param name="parameter"></param>
        internal ISParameter(Parameter parameter)
        {
            Parameter = parameter;
        }

        #endregion

        #region A constructor that accepts a ISPackage object

        /// <summary>
        /// ctor that accepts a ISPackage object
        /// </summary>
        /// <param name="package">The package to which the parameter is to be added or is already added</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="paramTypeCode">Typecode of the parameter</param>
        public ISParameter(ISPackage package, string parameterName, TypeCode paramTypeCode = TypeCode.Empty)
        {
            if (package.Parameters_m != null)
            {
                if (package.Parameters_m.Contains(parameterName))
                {
                    Parameter = package.Parameters_m[parameterName];
                }
                else
                {
                    Parameter = package.Parameters_m.Add(parameterName, paramTypeCode);
                }
            }
            else
            {
                Parameter = package.Parameters_m.Add(parameterName, paramTypeCode);
            }
        }

        #endregion

        #region A constructor that accepts a ISProject object

        /// <summary>
        /// ctor that accepts a ISProject object
        /// </summary>
        /// <param name="project">The project to which the parameter is to be added or is already added</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="paramTypeCode">Typecode of the parameter</param>
        public ISParameter(ISProject project, string parameterName, TypeCode paramTypeCode = TypeCode.Empty)
        {
            if(project.Parameters_m != null)
            {
                if (project.Parameters_m.Contains(parameterName))
                {
                    Parameter = project.Parameters_m[parameterName];
                }
                else
                {
                    Parameter = project.Parameters_m.Add(parameterName, paramTypeCode);
                }
            }
            else
            {
                Parameter = project.Parameters_m.Add(parameterName, paramTypeCode);
            }

        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal Parameter Parameter { get; set; }

        #endregion

        #region Dts Object Properties

        /// <summary>
        /// Name used to create the Parameter object.
        /// </summary>
        public string CreationName { get { return Parameter.CreationName; } }

        /// <summary>
        /// Data type of the Parameter object.
        /// </summary>
        public TypeCode DataType { get { return Parameter.DataType; } set { Parameter.DataType = value; } }

        /// <summary>
        /// Description of the Parameter object.
        /// </summary>
        public string Description { get { return Parameter.Description; } set { Parameter.Description = value; } }

        /// <summary>
        /// Identifier of the Parameter object.
        /// </summary>
        public string ID { get { return Parameter.ID; } }

        /// <summary>
        /// Value that indicates whether the Parameter object is included in a debug dump.
        /// </summary>
        public bool IncludeInDebugDump { get { return Parameter.IncludeInDebugDump; } set { Parameter.IncludeInDebugDump = value; } }

        /// <summary>
        /// Name of the Parameter object.
        /// </summary>
        public string Name { get { return Parameter.Name; } set { Parameter.Name = value; } }

        /// <summary>
        /// Indicates whether the Parameter object is a required parameter. If this value is true, a value must be assigned before a project or package can be executed.
        /// </summary>
        public bool Required { get { return Parameter.Required; } set { Parameter.Required = value; } }

        /// <summary>
        /// Indicates whether the Parameter object contains sensitive data. If this value is true, the value is encrypted in the Integration Services catalog.
        /// </summary>
        public bool Sensitive { get { return Parameter.Sensitive; } set { Parameter.Sensitive = value; } }

        /// <summary>
        /// Value of the Parameter object.
        /// </summary>
        public object Value { get { return Parameter.Value; } set { Parameter.Value = value; } }

        #endregion
    }
}
