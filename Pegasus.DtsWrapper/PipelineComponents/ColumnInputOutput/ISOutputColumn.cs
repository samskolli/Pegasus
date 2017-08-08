using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISOutputColumn
    {
        #region General Properties

        #region ISPipelineComponent

        public ISPipelineComponent ParentComponent { get; set; }
        internal IDTSOutput100 Output { get; set; }

        #endregion

        #region Wrapped Dts Object

        internal IDTSOutputColumn100 OutputColumn { get; set; }

        #endregion

        #region Output Column Name

        public string Name { get { return OutputColumn.Name; } set { OutputColumn.Name = value; } }

        #endregion

        #region Description

        public string Description
        {
            get { return OutputColumn.Description; }
            set { OutputColumn.Description = value; }
        }

        #endregion

        #region ErrorOrTruncationOperation

        public string ErrorOrTruncationOperation
        {
            get { return OutputColumn.ErrorOrTruncationOperation; }
            set { OutputColumn.ErrorOrTruncationOperation = value; }
        }

        #endregion

        #region ErrorRowDisposition

        public RowDisposition ErrorRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(OutputColumn.ErrorRowDisposition); }
            set { OutputColumn.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region ExternalMetadataColumnID

        public int ExternalMetadataColumnID
        {
            get { return OutputColumn.ExternalMetadataColumnID; }
            set { OutputColumn.ExternalMetadataColumnID = value; }
        }

        #endregion

        #region IdentificationString

        public string IdentificationString
        {
            get { return OutputColumn.IdentificationString; }
        }

        #endregion

        #region LineageId

        public int LineageID
        {
            get { return OutputColumn.LineageID; }
            set { OutputColumn.LineageID = value; }
        }

        #endregion

        #region MappedColumnID

        public int MappedColumnID
        {
            get { return OutputColumn.MappedColumnID; }
            set { OutputColumn.MappedColumnID = value; }
        }

        #endregion

        #region CodePage

        private int _codePage;
        public int CodePage
        {
            get
            {
                _codePage = OutputColumn.CodePage;
                return _codePage;
            }
            set
            {
                _codePage = value;
                SetDataTypeProperties();
            }
        }

        #endregion

        #region DataType

        private SSISDataType _dataType;
        public SSISDataType DataType
        {
            get
            {
                _dataType = DtsUtility.EnumAToEnumB<DataType, SSISDataType>(OutputColumn.DataType);
                return _dataType;
            }
            set
            {
                _dataType = value;
                SetDataTypeProperties();
            }
        }

        #endregion

        #region Length

        private int _length;
        public int Length
        {
            get
            {
                _length = OutputColumn.Length;
                return _length;
            }
            set
            {
                _length = value;
                SetDataTypeProperties();
            }
        }

        #endregion

        #region Precision

        private int _precision;
        public int Precision
        {
            get
            {
                _precision = OutputColumn.Precision;
                return _precision;
            }
            set
            {
                _precision = value;
                SetDataTypeProperties();
            }
        }

        #endregion

        #region Scale

        private int _scale;
        public int Scale
        {
            get
            {
                _scale = OutputColumn.Scale;
                return _scale;
            }
            set
            {
                _scale = value;
                SetDataTypeProperties();
            }
        }

        #endregion

        #region SortKeyPosition

        public int SortKeyPosition
        {
            get { return OutputColumn.SortKeyPosition; }
            set
            {
                if (value > 0)
                {
                    if (Output.IsSorted == false)
                        Output.IsSorted = true;
                    OutputColumn.SortKeyPosition = value;
                }
            }
        }

        #endregion

        #region TruncationRowDisposition

        public RowDisposition TruncationRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(OutputColumn.TruncationRowDisposition); }
            set { OutputColumn.TruncationRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        internal void SetDataTypeProperties()
        {
            SetDataTypeProperties(_dataType, _length, _precision, _scale, _codePage);
        }

        #endregion

        #region Methods

        public void SetDataTypeProperties(SSISDataType dataType, int length, int precision, int scale, int codePage)
        {
            if (dataType != SSISDataType.DT_EMPTY)
                OutputColumn.SetDataTypeProperties(DtsUtility.EnumAToEnumB<SSISDataType, DataType>(dataType), length, precision, scale, codePage);
        }

        #endregion

        #region ctor

        public ISOutputColumn(ISPipelineComponent parentComponent, string outputName, string outputColumnname)
        {
            ParentComponent = parentComponent;
            Output = ParentComponent.GetOutputFromName(outputName);

            bool colExists = false;
            for (int c = 0; c < Output.OutputColumnCollection.Count; c++)
            {
                if (Output.OutputColumnCollection[c].Name == outputColumnname)
                {
                    colExists = true;
                    OutputColumn = Output.OutputColumnCollection[c];
                }
            }

            if (colExists == false)
            {
                OutputColumn = ParentComponent.ComponentMetaData.OutputCollection[outputName].OutputColumnCollection.New();
                OutputColumn.Name = outputColumnname;
            }
        }

        public ISOutputColumn(ISPipelineComponent parentComponent, string outputName, string outputColumnname, 
            RowDisposition errorRowDisposition,
            RowDisposition truncationRowDisposition)
            : this(parentComponent, outputName, outputColumnname)
        {
            OutputColumn.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(errorRowDisposition);
            OutputColumn.TruncationRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(truncationRowDisposition);
        }
                        
        #endregion
    }
}
