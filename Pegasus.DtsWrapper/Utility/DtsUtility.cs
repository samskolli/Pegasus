using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.DtsWrapper
{
    public static class DtsUtility
    {
        #region String to Enum

        public static T StringToEnum<T>(string strValue)
        {
            var content = (T)Enum.Parse(typeof(T), strValue);
            return content;
        }

        #endregion

        #region From Dts Enum to Pegasus Enum or vice versa

        public static EB EnumAToEnumB<EA, EB>(EA value)
        {
            var convValue = StringToEnum<EB>(value.ToString());
            return convValue;
        }

        #endregion
    }
}
