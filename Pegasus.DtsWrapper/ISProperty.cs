using Microsoft.SqlServer.Dts.Runtime;
using System;

namespace Pegasus.DtsWrapper
{
    public class ISProperty
    {
        #region ctor

        /// <summary>
        /// A ctor that accepts the Microsoft.SqlServer.Dts.Runtime.DtsProperty object.
        /// </summary>
        /// <param name="property">the Microsoft.SqlServer.Dts.Runtime.DtsProperty object</param>
        internal ISProperty(DtsProperty property)
        {
            Property = property;
        }

        #endregion

        #region Wrapped Dts Object

        internal DtsProperty Property;

        #endregion

        #region Dts Object Properties

        /// <summary>
        /// The name of the connection manager used to create the connection. 
        /// </summary>
        public string ConnectionType { get { return Property.ConnectionType; } }

        /// <summary>
        /// Indicates whether a property value can be read.
        /// </summary>
        public bool Get { get { return Property.Get; } }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get { return Property.Name; } }

        /// <summary>
        /// Contains information about the custom attributes on a property. 
        /// </summary>
        public PropertyKind PropertyKind { get { return DtsUtility.EnumAToEnumB<DTSPropertyKind, PropertyKind>(Property.PropertyKind); } }

        /// <summary>
        /// Indicates whether the referenced object property is changeable.
        /// </summary>
        public bool Set { get { return Property.Set; } }

        /// <summary>
        /// The data type of the parameter.
        /// </summary>
        public TypeCode Type { get { return Property.Type; } }

        /// <summary>
        /// The assembly-qualified type name of the type converter object for the property.
        /// </summary>
        public string TypeConverter { get {return Property.TypeConverter; } }

        /// <summary>
        /// The assembly-qualified type name of the graphical editor for the property. 
        /// </summary>
        public string UITypeEditor { get { return Property.UITypeEditor; } }

        #endregion
    }
}
