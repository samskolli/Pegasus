using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;
using runTimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISBaseFlatFileConnectionManager : ISBaseFileConnectionManager
    {
        #region ctor

        internal ISBaseFlatFileConnectionManager(string connectionString, string name, string creationName, ISProject project = null, ISPackage package = null)
            : base(connectionString, name, creationName, project, package)
        {

        }

        internal ISBaseFlatFileConnectionManager(ConnectionManager connMgr) : base(connMgr)
        {
            
        }

        #endregion

        #region Properties

        #region AlwaysCheckForDelimiters

        public bool AlwaysCheckForDelimiters
        {
            get { return (bool)GetConnectionManagerPropertyValue("AlwaysCheckForDelimiters"); }
            set { SetConnectionManagerPropertyValue("AlwaysCheckForDelimiters", value); }
        }

        #endregion

        #region CodePage

        public int CodePage
        {
            get { return (int)GetConnectionManagerPropertyValue("AlwaysCheckForDelimiters"); }
            set { SetConnectionManagerPropertyValue("AlwaysCheckForDelimiters", value); }
        }

        #endregion

        #region ColumnNamesInFirstDataRow

        public bool ColumnNamesInFirstDataRow
        {
            get { return (bool)GetConnectionManagerPropertyValue("ColumnNamesInFirstDataRow"); }
            set { SetConnectionManagerPropertyValue("ColumnNamesInFirstDataRow", value); }
        }

        #endregion

        #region Columns 

        internal runTimeWrapper.IDTSConnectionManagerFlatFileColumns100 Columns_m
        {
            get { return (runTimeWrapper.IDTSConnectionManagerFlatFileColumns100)GetConnectionManagerPropertyValue("Columns"); }
        }

        private List<ISFlatFileColumn> _columns = new List<ISFlatFileColumn>();
        public List<ISFlatFileColumn> Columns
        {
            get
            {
                _columns.Clear();
                foreach (runTimeWrapper.IDTSConnectionManagerFlatFileColumn100 column in Columns_m)
                {
                    _columns.Add(new ISFlatFileColumn(column));
                }
                return _columns;
            }
        }

        #endregion

        #region DataRowsToSkip 

        public int DataRowsToSkip
        {
            get { return (int)GetConnectionManagerPropertyValue("DataRowsToSkip"); }
            set { SetConnectionManagerPropertyValue("DataRowsToSkip", value); }
        }

        #endregion

        #region Format 

        public string Format
        {
            get { return (string)GetConnectionManagerPropertyValue("Format"); }
            set { SetConnectionManagerPropertyValue("Format", value); }
        }

        #endregion

        #region HeaderRowDelimiter 

        public string HeaderRowDelimiter
        {
            get { return (string)GetConnectionManagerPropertyValue("HeaderRowDelimiter"); }
            set { SetConnectionManagerPropertyValue("HeaderRowDelimiter", value); }
        }

        #endregion

        #region HeaderRowsToSkip 

        public int HeaderRowsToSkip
        {
            get { return (int)GetConnectionManagerPropertyValue("HeaderRowsToSkip"); }
            set { SetConnectionManagerPropertyValue("HeaderRowsToSkip", value); }
        }

        #endregion

        #region LocaleID 

        public int LocaleID
        {
            get { return (int)GetConnectionManagerPropertyValue("LocaleID"); }
            set { SetConnectionManagerPropertyValue("LocaleID", value); }
        }

        #endregion

        #region RowDelimiter 

        public string RowDelimiter
        {
            get { return (string)GetConnectionManagerPropertyValue("RowDelimiter"); }
            set { SetConnectionManagerPropertyValue("RowDelimiter", value); }
        }

        #endregion

        #region TextQualifier 

        public string TextQualifier
        {
            get { return (string)GetConnectionManagerPropertyValue("TextQualifier"); }
            set { SetConnectionManagerPropertyValue("TextQualifier", value); }
        }

        #endregion

        #region Unicode 

        public string Unicode
        {
            get { return (string)GetConnectionManagerPropertyValue("Unicode"); }
            set { SetConnectionManagerPropertyValue("Unicode", value); }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Helper Method
        /// </summary>
        /// <param name="fc"></param>
        /// <param name="name"></param>
        private void SetFlatFileColumnName(runTimeWrapper.IDTSConnectionManagerFlatFileColumn100 fc, string name)
        {
            ((runTimeWrapper.IDTSName100)fc).Name = name;
        }
        
        #endregion

        #region Implicit Operator

        /// <summary>
        /// Implicit operator to convert.
        /// </summary>
        /// <param name="connMgr"></param>
        public static implicit operator ISBaseFlatFileConnectionManager(ConnectionManager connMgr)
        {
            if (connMgr == null)
                return null;

            return new ISBaseFlatFileConnectionManager(connMgr);
        }

        #endregion
    }

    public class ISFlatFileConnectionManager : ISBaseFlatFileConnectionManager
    {
        #region ctor

        /// <summary>
        /// Creates a Flat File Connection Manager.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="package"></param>
        public ISFlatFileConnectionManager(
            string connectionString,
            string name, 
            ISProject project = null, 
            ISPackage package = null)
            : base(connectionString, name, "FLATFILE", project, package)
        {

        }

        #endregion
    }
}
