using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISOutput
    {
        #region Properties

        #region ISPipelineComponent

        public ISPipelineComponent ParentComponent { get; set; }

        #endregion

        #region IDTSOutput Holder

        internal IDTSOutput100 Output { get; set; }

        #endregion

        #region DeleteOutputOnPathDetached

        public bool DeleteOutputOnPathDetached
        {
            get { return Output.DeleteOutputOnPathDetached; }
            set { Output.DeleteOutputOnPathDetached = value; }
        }

        #endregion

        #region Description

        public string Description
        {
            get { return Output.Description; }
            set { Output.Description = value; }
        }

        #endregion

        #region ErrorOrTruncationOperation

        public string ErrorOrTruncationOperation
        {
            get { return Output.ErrorOrTruncationOperation; }
            set { Output.ErrorOrTruncationOperation = value; }
        }

        #endregion

        #region ExclusionGroup

        public int ExclusionGroup
        {
            get { return Output.ExclusionGroup; }
            set { Output.ExclusionGroup = value; }
        }

        #endregion

        #region HasSideEffects

        public bool HasSideEffects
        {
            get { return Output.HasSideEffects; }
            set { Output.HasSideEffects = value; }
        }

        #endregion

        #region id

        private int _id;
        public int ID
        {
            get { return Output.ID; }
        }

        #endregion

        #region IdentificationString

        public string IdentificationString
        {
            get { return Output.IdentificationString; }
        }

        #endregion

        #region IsAttached

        public bool IsAttached
        {
            get { return Output.IsAttached; }
        }

        #endregion

        #region IsErrorOut

        public bool IsErrorOut
        {
            get { return Output.IsErrorOut; }
            set { Output.IsErrorOut = value; }
        }

        #endregion

        #region IsSorted

        public bool IsSorted
        {
            get { return Output.IsSorted; }
            set { Output.IsSorted = value; }
        }

        #endregion

        #region Name

        public string Name { get { return Output.Name; } set { Output.Name = value; } }

        #endregion

        #region SynchronousInputID

        public int SynchronousInputID
        {
            get { return Output.SynchronousInputID; }
            set { Output.SynchronousInputID = value; }
        }

        #endregion

        #region ErrorRowDisposition

        public RowDisposition ErrorRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(Output.ErrorRowDisposition); }
            set { Output.ErrorRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region TruncationRowDisposition
                
        public RowDisposition TruncationRowDisposition
        {
            get { return DtsUtility.EnumAToEnumB<DTSRowDisposition, RowDisposition>(Output.TruncationRowDisposition); }
            set { Output.TruncationRowDisposition = DtsUtility.EnumAToEnumB<RowDisposition, DTSRowDisposition>(value); }
        }

        #endregion

        #region External Metadata Column Collection

        internal IDTSExternalMetadataColumnCollection100 ExternalMetadataColumnCollection_m
        {
            get { return Output.ExternalMetadataColumnCollection; }
        }

        //private List<ISExternalMetadataColumn> _externalMetadataColumnCollection = new List<ISExternalMetadataColumn>();
        //public List<ISExternalMetadataColumn> ExternalMetadataColumnCollection
        //{
        //    get
        //    {
        //        _externalMetadataColumnCollection.Clear();
        //        foreach (IDTSExternalMetadataColumn100 emc in ExternalMetadataColumnCollection_m)
        //        {
        //            _externalMetadataColumnCollection.Add(new ISExternalMetadataColumn(emc));
        //        }
        //        return _externalMetadataColumnCollection;
        //    }
        //}

        #endregion

        #region ColumnCollection

        //public List<ISOutputColumn> ColumnCollection
        //{
        //    get
        //    {
        //        List<ISOutputColumn> _outputColumns = new List<ISOutputColumn>();
        //        foreach (IDTSOutputColumn100 col in Output.OutputColumnCollection)
        //        {
        //            _outputColumns.Add(new ISOutputColumn(ParentComponent, Output.Name, col.Name));
        //        }
        //        return _outputColumns;
        //    }
        //    set
        //    {

        //    }
        //}

        #endregion

        #endregion

        #region ctor

        public ISOutput(ISPipelineComponent parentComponent, int outputIndex = 0)
        {
            ParentComponent = parentComponent;
            if (ParentComponent.ComponentMetaData.OutputCollection.Count > 0)
                Output = ParentComponent.GetOutputFromIndex(outputIndex);
        }

        /// <summary>
        /// First check if an Output with the given name exists on the Component.
        ///     If it exists, then assign that output to the Output property
        ///     If it does not exist:
        ///         Check if more outputs can be added or not.
        ///         If more outputs can be added, then add a new output
        ///         If more outputs cannot be added, then assign the first (non error ) output in the component's collection to the Output property
        /// </summary>
        /// <param name="parentComponent"></param>
        /// <param name="name"></param>
        public ISOutput(ISPipelineComponent parentComponent, string name, int referenceOutputIndex = 0, InsertPlacement beforeOrAfter = InsertPlacement.IP_BEFORE)
        {
            ParentComponent = parentComponent;

            bool outputExists = false;
            for (int i = 0; i < ParentComponent.ComponentMetaData.OutputCollection.Count; i++)
            {
                if (ParentComponent.ComponentMetaData.OutputCollection[i].Name == name)
                {
                    Output = ParentComponent.ComponentMetaData.OutputCollection[i];
                    outputExists = true;
                }
            }

            //  check if more outputs can be added
            int existingOutputCount = parentComponent.ComponentMetaData.OutputCollection.Count;

            if (ParentComponent._numberOfOutputsAllowed == -1)
            {
                if (!(outputExists))
                {
                    Output = parentComponent.DesignTimeComponent.InsertOutput(DtsUtility.EnumAToEnumB<InsertPlacement, DTSInsertPlacement>(beforeOrAfter), ParentComponent.GetOutputFromIndex(referenceOutputIndex).ID);
                    Name = name;
                }
            }
            else
            {
                if (existingOutputCount < parentComponent._numberOfOutputsAllowed)
                {
                    if (!(outputExists))
                    {
                        Output = parentComponent.DesignTimeComponent.InsertOutput(DTSInsertPlacement.IP_BEFORE, ParentComponent.GetOutputFromIndex(referenceOutputIndex).ID);
                        Name = name;
                    }
                }
                else
                {
                    Console.WriteLine("WARN::: Only {1} output(s) are allowed. A new output with the name '{0}' cannot be added. Therefore, the name '{0}' is assigned to the first non error output in the collection", name, existingOutputCount.ToString());
                    Output = parentComponent.ComponentMetaData.OutputCollection[0];
                    Name = name;
                }
            }
        }
        
        #endregion

        #region Methods

        public void SetCustomProperty(string propertyName, object propertyValue)
        {
            ParentComponent.SetCustomPropertyToOutput(Output, propertyName, propertyValue);
        }

        public object GetCustomPropertyValue(string propertyName)
        {
            return Output.CustomPropertyCollection[propertyName].Value;
        }

        public List<ISOutputColumn> GetColumnCollection()
        {
            List<ISOutputColumn> _outputColumns = new List<ISOutputColumn>();
            foreach(IDTSOutputColumn100 outputColumn in Output.OutputColumnCollection)
            {
                _outputColumns.Add(new ISOutputColumn(this.ParentComponent, Output.Name, outputColumn.Name));
            }
            return _outputColumns;
        }

        #endregion
    }
}
