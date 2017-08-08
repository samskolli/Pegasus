using Microsoft.SqlServer.Dts.Runtime;
using runTimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISBaseFileConnectionManager : ISConnectionManager
    {
        #region ctor

        /// <summary>
        /// Base constructor for File connection managers; used only within this assembly. Accepts a connection String, Name, Creation Name and a Project and Package (One of which can be Optional).
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="creationName"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>

        internal ISBaseFileConnectionManager(string connectionString, string name, string creationName, ISProject project = null, ISPackage package = null) 
            : base(connectionString, name, creationName, project, package)
        {

        }

        /// <summary>
        /// Base constructor for FIle connection managers; used only within this assembly. Accepts a Connection Manager
        /// </summary>
        /// <param name="connMgr"></param>
        internal ISBaseFileConnectionManager (ConnectionManager connMgr) : base(connMgr)
        {

        }

        #endregion

        #region Properties

        #region FileUsageType

        /// <summary>
        /// Gets or sets the file connection usage type.
        /// </summary>
        public FileConnectionUsageType FileUsageType
        {
            get
            {
                return (
                    DtsUtility.EnumAToEnumB<runTimeWrapper.DTSFileConnectionUsageType, FileConnectionUsageType>(
                        (runTimeWrapper.DTSFileConnectionUsageType)GetConnectionManagerPropertyValue("FileUsageType"))
                        );
            }
            set { SetConnectionManagerPropertyValue("FileUsageType", DtsUtility.EnumAToEnumB<FileConnectionUsageType, runTimeWrapper.DTSFileConnectionUsageType>(value)); }
        }

        #endregion

        #endregion

        #region Implicit Operator

        /// <summary>
        /// Cast DTS Object Connection Manager to this assembly's version
        /// </summary>
        /// <param name="connMgr"></param>
        public static implicit operator ISBaseFileConnectionManager(ConnectionManager connMgr)
        {
            if (connMgr == null)
                return null;

            return new ISBaseFileConnectionManager(connMgr);
        }

        #endregion
    }
}
