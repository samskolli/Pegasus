using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISLookupOutputColumn
    {
        #region Properties

        #region Parent Component

        private ISLookupComponent _lookupComponent;

        #endregion

        #region Input/Output

        private IDTSInput100 _input { get { return _lookupComponent.ComponentMetaData.InputCollection[0]; } }
        private IDTSOutput100 _output { get { return _lookupComponent.ComponentMetaData.OutputCollection[0]; } }

        public ISInputColumn InputColumn
        {
            get; set;
        }

        public ISOutputColumn OutputColumn
        {
            get; set;
        }

        #endregion

        #region LookupColumn

        public string LookupColumn
        {
            get
            {
                string _lookupCol = "";
                if (OutputColumn != null)
                    _lookupCol = OutputColumn.OutputColumn.CustomPropertyCollection["CopyFromReferenceColumn"].Value;
                if (InputColumn != null)
                    _lookupCol = InputColumn.Input.CustomPropertyCollection["CopyFromReferenceColumn"].Value;
                return _lookupCol;
            }
            set
            {
                if (OutputColumn != null)
                    _lookupComponent.SetCustomPropertyToOutputColumn(_output, OutputColumn.OutputColumn, "CopyFromReferenceColumn", value);
                if (InputColumn != null)
                    _lookupComponent.SetCustomPropertyToInputColumn(_input, InputColumn.InputColumn, "CopyFromReferenceColumn", value);
            }
        }

        #endregion

        #region Lookup Operation

        public LookupOutputColumnOperation LookupOperation { get; set; }

        #endregion

        #region Output Alias

        public string OutputAlias
        {
            get
            {
                string _outputAlias = "";
                if (OutputColumn != null)
                    _outputAlias = OutputColumn.Name;
                if (InputColumn != null)
                    _outputAlias = InputColumn.Name;

                return _outputAlias;
            }
            set
            {
                if (OutputColumn != null)
                    OutputColumn.Name = value;
                if (InputColumn != null)
                    InputColumn.Name = value;
            }
        }

        #endregion

        #region DataType Props

        #region CodePage

        public int CodePage
        {
            get { return OutputColumn.CodePage; }
            set { OutputColumn.CodePage = value; }
        }

        #endregion

        #region DataType

        public SSISDataType DataType
        {
            get { return OutputColumn.DataType; }
            set { OutputColumn.DataType = value; }
        }

        #endregion

        #region Length

        public int Length
        {
            get { return OutputColumn.Length; }
            set
            {
                OutputColumn.Length = value;
            }
        }

        #endregion

        #region Precision

        public int Precision
        {
            get { return OutputColumn.Precision; }
            set { OutputColumn.Precision = value; }
        }

        #endregion

        #region Scale

        public int Scale
        {
            get { return OutputColumn.Scale; }
            set { OutputColumn.Scale = value; }
        }

        #endregion

        #endregion

        #region TruncationRowDisposition

        public RowDisposition TruncationRowDisposition
        {
            get
            {
                RowDisposition _disposition = RowDisposition.RD_NotUsed;
                if (OutputColumn != null)
                    _disposition = OutputColumn.TruncationRowDisposition;
                if (InputColumn != null)
                    _disposition = InputColumn.TruncationRowDisposition;

                return _disposition;
            }
            set
            {
                if (OutputColumn != null)
                    OutputColumn.TruncationRowDisposition = value;
                if (InputColumn != null)
                    InputColumn.TruncationRowDisposition = value;
            }
        }

        #endregion

        #endregion

        #region ctor

        public ISLookupOutputColumn(ISLookupComponent parentComponent, LookupOutputColumnOperation lookupColumnOperation, string lookupColumnName, string outputAlias = "", string columnToReplace = "")
        {
            LookupOperation = lookupColumnOperation;
            _lookupComponent = parentComponent;

            if (LookupOperation == LookupOutputColumnOperation.Add)
            {
                OutputColumn = new ISOutputColumn(_lookupComponent, _output.Name, String.IsNullOrEmpty(outputAlias) ? LookupColumn : outputAlias);
                LookupColumn = lookupColumnName;
                OutputColumn.ErrorRowDisposition = RowDisposition.RD_NotUsed;
                TruncationRowDisposition = RowDisposition.RD_FailComponent;
            }

            if (LookupOperation == LookupOutputColumnOperation.Replace)
            {
                if (String.IsNullOrEmpty(columnToReplace))
                {

                }
                else
                {
                    InputColumn = new ISInputColumn(_lookupComponent, _input.Name, columnToReplace, UsageType.UT_READWRITE);
                    InputColumn.Name = outputAlias;
                    LookupColumn = lookupColumnName;
                    InputColumn.ErrorOrTruncationOperation = "Copy Column";
                    InputColumn.ErrorRowDisposition = RowDisposition.RD_NotUsed;
                    TruncationRowDisposition = RowDisposition.RD_FailComponent;
                }
            }
        }
        
        
        #endregion

        #region Methods

        public void SetDataTypeProperties(SSISDataType dataType, int length, int precision, int scale, int codePage)
        {
            OutputColumn.SetDataTypeProperties(dataType, length, precision, scale, codePage);
        }

        #endregion
    }
}
