using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Collections.Generic;

namespace Pegasus.DtsWrapper
{
    public class ISInput
    {
        #region ctor

        public ISInput(ISPipelineComponent parentComponent, int inputIndex = 0)
        {
            ParentComponent = parentComponent;
            if (ParentComponent.ComponentMetaData.InputCollection.Count > 0)
                Input = ParentComponent.GetInputFromIndex(inputIndex);
        }

        public ISInput(ISPipelineComponent parentComponent, string inputName)
        {
            ParentComponent = parentComponent;

            bool inputExists = false;
            for (int i = 0; i < ParentComponent.ComponentMetaData.InputCollection.Count; i++)
            {
                if (ParentComponent.ComponentMetaData.InputCollection[i].Name == inputName)
                {
                    inputExists = true;
                    Input = ParentComponent.ComponentMetaData.InputCollection[i];
                }
            }
            if (!(inputExists))
            {
                //Input = ParentComponent.AddInput("after", ParentComponent.GetInputFromIndex(0).Name);
                Input = ParentComponent.DesignTimeComponent.InsertInput(DtsUtility.EnumAToEnumB<InsertPlacement, DTSInsertPlacement>(InsertPlacement.IP_AFTER), ParentComponent.GetInputFromIndex(ParentComponent.InputCollection_m.Count - 1).ID);
                Input.Name = inputName;
            }
        }

        #endregion

        #region General Properties

        public ISPipelineComponent ParentComponent { get; set; }

        #endregion

        #region Object Model Properties

        #region IDTSInput100

        internal IDTSInput100 Input { get; set; }

        #endregion

        #region AreInputColumnsAssociatedWithOutputColumns

        public bool AreInputColumnsAssociatedWithOutputColumns
        {
            get { return Input.AreInputColumnsAssociatedWithOutputColumns; }
        }

        #endregion

        #region Component

        public string Component
        {
            get { return ParentComponent.ComponentMetaData.Name; }
        }

        #endregion

        #region Dangling

        public bool Dangling
        {
            get { return Input.Dangling; }
            set { Input.Dangling = value; }
        }

        #endregion

        #region Description

        public string Description { get { return Input.Description; } set { Input.Description = value; } }

        #endregion

        #region ErrorOrTruncationOperation

        public string ErrorOrTruncationOperation { get { return Input.ErrorOrTruncationOperation; } set { Input.ErrorOrTruncationOperation = value; } }

        #endregion

        #region ErrorRowDisposition

        public RowDisposition ErrorRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(Input.ErrorRowDisposition); }
            set { Input.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region HasSideEffects

        public bool HasSideEffects
        {
            get { return Input.HasSideEffects; }
            set { Input.HasSideEffects = value; }
        }

        #endregion

        #region id

        public int ID
        {
            get { return Input.ID; }
        }

        #endregion

        #region IdentificationString

        public string IdentificationString
        {
            get { return Input.IdentificationString; }
        }

        #endregion

        #region IsAttached

        public bool IsAttached
        {
            get { return Input.IsAttached; }
        }

        #endregion

        #region IsSorted

        public bool IsSorted
        {
            get { return Input.IsSorted; }
        }

        #endregion

        #region Name

        public string Name { get { return Input.Name; } set { Input.Name = value; } }

        #endregion

        #region TruncationRowDisposition
                
        public RowDisposition TruncationRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(Input.TruncationRowDisposition); }
            set { Input.TruncationRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #endregion

        public List<ISInputColumn> GetColumnCollection()
        {
            List<ISInputColumn> inputColumns = new List<ISInputColumn>();
            foreach (IDTSInputColumn100 outputColumn in Input.InputColumnCollection)
            {
                inputColumns.Add(new ISInputColumn(ParentComponent, Input.Name, outputColumn.Name, DtsUtility.EnumAToEnumB<DTSUsageType, UsageType>(outputColumn.UsageType)));
            }
            return inputColumns;
        }
    }
}
