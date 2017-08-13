using Microsoft.SqlServer.Dts.Runtime;
using runTimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISExcelConnectionManager : ISConnectionManager
    {
        #region ctor

        /// <summary>
        /// Creates a Flat File Connection Manager.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISExcelConnectionManager(
            string name,
            ISProject project = null,
            ISPackage package = null)
            : base("", name, "EXCEL", project, package)
        {

        }

        #endregion

        #region ExcelFilePath

        public string ExcelFilePath
        {
            get { return (string)GetConnectionManagerPropertyValue("ExcelFilePath"); }
            set
            {
                SetConnectionManagerPropertyValue("ExcelFilePath", value);
            }
        }

        #endregion

        #region ExcelVersionNumber

        public ExcelVersion ExcelVersionNumber
        {
            get { return DtsUtility.StringToEnum<ExcelVersion>(GetConnectionManagerPropertyValue("ExcelVersionNumber").ToString()); }
            set
            {
                SetConnectionManagerPropertyValue("ExcelVersionNumber", DtsUtility.EnumAToEnumB<ExcelVersion, runTimeWrapper.DTSExcelVersion>(value));
            }
        }

        #endregion

        #region FirstRowHasColumnName

        public bool FirstRowHasColumnName
        {
            get { return (bool)GetConnectionManagerPropertyValue("FirstRowHasColumnName"); }
            set { SetConnectionManagerPropertyValue("FirstRowHasColumnName", value); }
        }

        #endregion
    }
}
