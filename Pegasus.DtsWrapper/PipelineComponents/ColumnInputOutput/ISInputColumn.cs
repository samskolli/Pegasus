using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISInputColumn
    {
        #region Properties

        #region ISPipelineComponent

        public ISPipelineComponent ParentComponent { get; set; }

        #endregion

        #region IDTSInput Holder

        private string _inputName;

        internal IDTSInput100 Input
        {
            get { return ParentComponent.GetInputFromName(_inputName); }
            set { Input = value; }
        }

        #endregion

        #region Dts Wrapper object

        //private int _columnId;
        //private IDTSInputColumn100 _inputColumn;

        internal IDTSInputColumn100 InputColumn { get; set; }

        #endregion

        #region Input Column Name
                
        public string Name { get { return InputColumn.Name; } set { InputColumn.Name = value; } }
        
        #endregion

        #region CodePage

        public int CodePage
        {
            get { return InputColumn.CodePage; }
        }

        #endregion

        #region DataType

        public SSISDataType DataType
        {
            get
            {
                return DtsUtility.EnumAToEnumB<DataType, SSISDataType>(InputColumn.DataType);
            }
        }

        #endregion

        #region Description

        public string Description
        {
            get { return InputColumn.Description; }
            set { InputColumn.Description = value; }
        }

        #endregion

        #region ErrorOrTruncationOperation

        public string ErrorOrTruncationOperation
        {
            get { return InputColumn.ErrorOrTruncationOperation; }
            set { InputColumn.ErrorOrTruncationOperation = value; }
        }

        #endregion

        #region ErrorRowDisposition

        public RowDisposition ErrorRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(InputColumn.ErrorRowDisposition); }
            set { InputColumn.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region ExternalMetadataColumnID

        public int ExternalMetadataColumnID
        {
            get { return InputColumn.ExternalMetadataColumnID; }
            set { InputColumn.ExternalMetadataColumnID = value; }
        }

        #endregion

        #region IdentificationString

        public string IdentificationString
        {
            get { return InputColumn.IdentificationString; }
        }

        #endregion

        #region Length

        public int Length
        {
            get { return InputColumn.Length; }
        }

        #endregion

        #region ID

        public int ID
        {
            get { return InputColumn.ID; }
        }

        #endregion

        #region LineageId

        public int LineageID
        {
            get { return InputColumn.LineageID; }
            set { InputColumn.LineageID = value; }
        }

        #endregion

        #region MappedColumnID

        public int MappedColumnID
        {
            get { return InputColumn.MappedColumnID; }
            set { InputColumn.MappedColumnID = value; }
        }

        #endregion

        #region Precision

        public int Precision
        {
            get { return InputColumn.Precision; }
        }

        #endregion

        #region Scale

        public int Scale
        {
            get { return InputColumn.Scale; }
        }

        #endregion

        #region SortKeyPosition

        public int SortKeyPosition
        {
            get { return InputColumn.SortKeyPosition; }
        }

        #endregion

        #region TruncationRowDisposition

        public RowDisposition TruncationRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(InputColumn.ErrorRowDisposition); }
            set { InputColumn.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region UpstreamComponentName

        public string UpstreamComponentName
        {
            get { return InputColumn.UpstreamComponentName; }
        }

        #endregion

        #region UsageType

        public UsageType UsageType
        {
            get { return DtsUtility.EnumAToEnumB<DTSUsageType, UsageType>(InputColumn.UsageType); }
        }

        #endregion

        #endregion

        #region ctor

        public ISInputColumn(ISPipelineComponent parentComponent, string inputName, string inputColumnName, UsageType usageType)
        {
            ParentComponent = parentComponent;
            _inputName = inputName;

            bool columnAddedToInput = true;
            foreach (IDTSVirtualInputColumn100 vc in Input.GetVirtualInput().VirtualInputColumnCollection)
            {
                if (vc.Name == inputColumnName)
                {
                    if (vc.UsageType == DTSUsageType.UT_IGNORED)
                        // this will expose the inputColumn in the InputColumnCollection; otherwise, the inputColumn wont be available in InputColumnCollection
                        columnAddedToInput = false;
                    else
                    {
                        if (vc.UsageType == DTSUsageType.UT_READWRITE)
                            usageType = UsageType.UT_READWRITE;
                        for (int c = 0; c < Input.InputColumnCollection.Count; c++)
                        {
                            if (Input.InputColumnCollection[c].LineageID == vc.LineageID) // the input column might be renamed...therefore match on lineage ids
                            {
                                // if a match is found, assign that input column to our variable.
                                InputColumn = Input.InputColumnCollection[c];
                            }
                        }
                    }
                }
            }
            if (!(columnAddedToInput))
                InputColumn = ParentComponent.SetInputColumnDTSUsageType(Input, inputColumnName, usageType);
        }

        public ISInputColumn(ISPipelineComponent parentComponent, string inputName, string inputColumnName, UsageType usageType, RowDisposition errorRowDisposition, RowDisposition truncationRowDisposition)
            : this(parentComponent, inputName, inputColumnName, usageType)
        {
            ErrorRowDisposition = errorRowDisposition;
            TruncationRowDisposition = truncationRowDisposition;
        }

        #endregion
    }
}
