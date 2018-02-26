using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections.Generic;
using System.Collections;

namespace Pegasus.DtsWrapper

{
    public struct RuntimeConnection{
        public String ConnectionManagerID { get; set; }
        public int Index { get; set; }
    }

    /// <summary>
    /// A Data Flow Component implements two interfaces: IDTSDesigntimeComponent100 and IDTSRuntimeComponent100.
    /// The IDTSRuntimeComponent100 interface defines the methods and properties that are called during execution of the component.
    /// The interface contains Properties such as BufferManager, EventInfos, LogEntryEinfos etc. 
    /// Specifically it also has a Property called ComponentMetaData (which is of use for generating components programmatically).
    /// The first step to create a Data Flow Component is:
    ///     1. Call the New method on ComponentMetaDataCollection of the Data Flow Task's MainPipe; which results in a IDTSComponentMetaData100 being created and returned
    ///     2. Set the ComponentClassID of the returned IDTSComponentMetaData100 object to the appropriate pipeline component type.
    ///     3. Call the Instantiate method on the IDTSComponentMetaData100 object which will then create the design time instance of the required component.
    ///     4. Then the ProvideComponentProperties method on the design time instance is called. The inputs/outputs and custom properties are then made available.
    ///     
    /// The IDTSDesigntimeComponent100 interface
    /// 
    /// As a general rule:
    ///     use hte design time component's methods when calling Object Modle methods that modify a component.
    ///     While some modifications can be done by directly modiying the ComponentMetaData, it is not recommended by Microsoft.
    /// 
    /// </summary>
    public class ISPipelineComponent
    {
        #region ctor

        #region ctor that accepts the parent data flow task, the component info (type and name)
                
        public ISPipelineComponent(ISDataFlowTask parentDataFlowTask, string componentMoniker, string componentname) :
            this(componentname, componentMoniker, parentDataFlowTask.MainPipe)
        {
            ParentDataFlowTask = parentDataFlowTask;
        }

        #endregion

        #region ctor that accepts a MainPipe, component info (type and name)

        internal ISPipelineComponent(string componentName, string componentMoniker, MainPipe mainPipe)
        {
            bool componentExists = false;
            _readWriteCols = new List<string>();
            foreach (IDTSComponentMetaData100 comp in mainPipe.ComponentMetaDataCollection)
            {
                if (comp.Name == componentName)
                {
                    componentExists = true;
                    ComponentMetaData = comp;
                    // Assign the IDTSDesignTimeCoomponent100 so that we can change the properties.
                    // But Do not call ProvideComponentProperties() on the IDTSDesignTimeCoomponent100 after the assignment.
                    // Since the IDTSComponentMetadata100 already exists, the properties were already made available.
                    // Calling ProvideComponentProperties will override the existing properties and set their values to default values; thus losing the existing properties
                    DesignTimeComponent = ComponentMetaData.Instantiate();
                }
            }
            if (!(componentExists))
            {
                ComponentMetaData = mainPipe.ComponentMetaDataCollection.New(); // Adds a new "component" to the Data Flow Task's Pipeline
                ComponentMetaData.ComponentClassID = componentMoniker; //
                DesignTimeComponent = ComponentMetaData.Instantiate();
                DesignTimeComponent.ProvideComponentProperties();
            }
            Name = componentName;
        }


        #endregion

        #region ctor that accepts a IDTSComponentMetadata100 object

        internal ISPipelineComponent(IDTSComponentMetaData100 component)
        {
            ComponentMetaData = component;
            DesignTimeComponent = ComponentMetaData.Instantiate();
        }

        #endregion

        #endregion

        #region Dts Wrapper Objects

        internal CManagedComponentWrapper DesignTimeComponent { get; set; }
        //internal IDTSDesigntimeComponent100 DesignTimeComponent { get; set; }
        internal IDTSComponentMetaData100 ComponentMetaData { get; set; }
        //internal IDTSRuntimeComponent100 RunTimeComponent { get; set; }

        #endregion

        #region Dts Wrapper Objects Properties

        bool AreInputColumnsValid { get { return ComponentMetaData.AreInputColumnsValid; } }
        
        public string ComponentClassID { get { return ComponentMetaData.ComponentClassID; } set { ComponentMetaData.ComponentClassID = value; } }

        public string ContactInfo { get { return ComponentMetaData.ContactInfo; } set { ComponentMetaData.ContactInfo = value; } }

        /// <summary>
        ///  The Custom Property Collection only contains the "custom" properties of a component.
        ///  Common properties of all components such as Name, Description etc are not available in this collection.
        ///  Therefore calling SetCustomProperty and GetCustomProperty on common properties will result in error.
        ///  Custom properties do not have a data type property. 
        ///  The data type of a custom property is set by the data type of the value that you assign to its Value property. 
        ///  However, after you have assigned an initial value to the custom property, you cannot assign a value with a different data type.
        /// </summary>
        internal IDTSCustomPropertyCollection100 CustomPropertyCollection_m { get { return ComponentMetaData.CustomPropertyCollection; } }

        public string Description { get { return ComponentMetaData.Description; } set { ComponentMetaData.Description = value; } }

        public int ID { get { return ComponentMetaData.ID; } set { ComponentMetaData.ID = value; } }

        public string IdentificationString { get { return ComponentMetaData.IdentificationString; } }

        internal IDTSInputCollection100 InputCollection_m { get { return ComponentMetaData.InputCollection; } }

        public bool IsDefaultLocale { get { return ComponentMetaData.IsDefaultLocale; } }

        public int LocaleID { get { return ComponentMetaData.LocaleID; } set { ComponentMetaData.LocaleID = value; } }

        public  string Name { get { return ComponentMetaData.Name; } set { ComponentMetaData.Name = value; } }

        public ObjectType ObjectType { get { return DtsUtility.EnumAToEnumB<DTSObjectType, ObjectType>(ComponentMetaData.ObjectType); } }

        internal IDTSOutputCollection100 OutputCollection_m { get { return ComponentMetaData.OutputCollection; } }

        public int PipelineVersion { get { return ComponentMetaData.PipelineVersion; } set { ComponentMetaData.PipelineVersion = value; } }

        internal IDTSRuntimeConnectionCollection100 RuntimeConnectionCollection_m { get { return ComponentMetaData.RuntimeConnectionCollection; } }

        public bool UsesDispositions { get { return ComponentMetaData.UsesDispositions; } set { ComponentMetaData.UsesDispositions = value; } }

        public bool ValidateExternalMetadata { get { return ComponentMetaData.ValidateExternalMetadata; } set { ComponentMetaData.ValidateExternalMetadata = value; } }

        int Version { get { return ComponentMetaData.Version; } set { ComponentMetaData.Version = value; } }

        #endregion

        #region General properties

        public ISDataFlowTask ParentDataFlowTask { get; set; }

        #endregion

        #region Helper Properties

        internal List<string> _readWriteCols;

        #endregion

        #region API Methods

        #region Connect to another pipeline component

        private IDTSPath100 ConnectToAnotherPipelineComponent(IDTSComponentMetaData100 sourceComponent, int sourceComponentOutputIndex = 0, int inputCollectionIndex = 0)
        {
            bool pathHasStartPoint = false;
            IDTSPath100 path = null;
            foreach (IDTSPath100 dtsPath in ParentDataFlowTask.MainPipe.PathCollection)
            {
                IDTSOutput100 output = dtsPath.StartPoint;
                if (dtsPath.StartPoint.Name == sourceComponent.OutputCollection[sourceComponentOutputIndex].Name
                    && dtsPath.StartPoint.Component.Name == sourceComponent.Name
                    )
                {
                    pathHasStartPoint = true;
                    path = dtsPath;
                }
            }
            if (pathHasStartPoint)
                ParentDataFlowTask.MainPipe.PathCollection.RemoveObjectByID(path.ID);

            path = ParentDataFlowTask.MainPipe.PathCollection.New();
            path.AttachPathAndPropagateNotifications(
                sourceComponent.OutputCollection[sourceComponentOutputIndex],
                ComponentMetaData.InputCollection[inputCollectionIndex]);

            return path;
        }

        public void ConnectToAnotherPipelineComponent(string sourceComponentName, string sourceComponentOutputName = "", int inputCollectionIndex = 0)
        {
            int srcOutIdx = 0;
            for (int i = 0; i < ParentDataFlowTask.ComponentMetaDataCollection_m.Count; i++)
            {
                if (ParentDataFlowTask.ComponentMetaDataCollection_m[i].Name == sourceComponentName)
                {
                    for (int outputIdx = 0; outputIdx < ParentDataFlowTask.ComponentMetaDataCollection_m[i].OutputCollection.Count; outputIdx++)
                    {
                        if (ParentDataFlowTask.ComponentMetaDataCollection_m[i].OutputCollection[outputIdx].Name == sourceComponentOutputName)
                        {
                            srcOutIdx = outputIdx;
                        }
                    }
                }
            }

            ConnectToAnotherPipelineComponent(
                ParentDataFlowTask.ComponentMetaDataCollection_m[sourceComponentName],
                srcOutIdx,
                inputCollectionIndex
                );
        }

        public void ConnectToAnotherPipelineComponent(ISPipelineComponent sourceComponent, ISOutput sourceComponentOutput, int inputIndex = 0)
        {
            ConnectToAnotherPipelineComponent(sourceComponent.Name, sourceComponentOutput.Name, inputIndex);
        }

    

        #endregion

        #region RetrieveMetaData

        /// <summary>
        /// Retrieve Metadata for a given source/destination when it is available and connectable.
        /// </summary>
        public bool RetrieveMetaData_Old()
        {
            bool success = false;
            try
            {
                DesignTimeComponent.AcquireConnections(null);
                DesignTimeComponent.ReinitializeMetaData();
                DesignTimeComponent.ReleaseConnections();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Retrieve Metadata; Error:: " + e.Message);
                success = false;
            }
            return success;
        }

        public bool RetrieveMetaData()
        {
            bool success = false;
            
            try
            {
                DesignTimeComponent.AcquireConnections(null);
                success = true;
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                Console.WriteLine("AcquireConn Error:: " + e.Message + " -- " + e.HResult.ToString());
                success = false;
            }

            if (success)
            {
                try
                {
                    DesignTimeComponent.ReinitializeMetaData();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReinitializeMetaData Error:: " + e.Message);
                    success = false;
                }
            }

            if (success)
            {
                try
                {
                    DesignTimeComponent.ReleaseConnections();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ReleaseConnections Error:: " + e.Message);
                    success = false;
                }
            }


            return success;
        }

        public bool RetrieveMetaData(ISConnectionManager connMgr, string password)
        {
            string origConn = ComponentMetaData.RuntimeConnectionCollection[0].Name;
            ConnectionManager cm = ParentDataFlowTask.RootPackage.Connections.Add(connMgr.CreationName) as ConnectionManager;
            cm.Name = "DummyConnToForce";
            cm.ConnectionString = connMgr.ConnectionString;
            cm.Properties["Password"].SetValue(cm, password);
            SetUpConnection(cm);
            bool success = RetrieveMetaData();
            SetUpConnection(GetConnectionManager(origConn));
            ParentDataFlowTask.RootPackage.Connections.Remove("DummyConnToForce");
            return success;
        }

        public bool RetrieveMetaData_AnoOld(string connection, string password)
        {
            ConnectionManager origConnectionManager = GetConnectionManager(connection);
            
            ConnectionManager cm = ParentDataFlowTask.RootPackage.Connections.Add(origConnectionManager.CreationName) as ConnectionManager;
            cm.Name = "DummyConnToForce";
            cm.ConnectionString = origConnectionManager.ConnectionString + ";Password=" + password + ";";
            SetUpConnection(cm);
            bool success = RetrieveMetaData();
            SetUpConnection(GetConnectionManager(connection));
            ParentDataFlowTask.RootPackage.Connections.Remove("DummyConnToForce");
            return success;
        }

        public bool RetrieveMetaData(string connection, string password)
        {
            ConnectionManager origConnectionManager = GetConnectionManager(connection);
            bool success = false;
            var connMgr = ParentDataFlowTask.ParentPackage.Project.Project.ConnectionManagerItems.Add(origConnectionManager.CreationName, "DummyConnToForce.conmgr").ConnectionManager;
            connMgr.ConnectionString = origConnectionManager.ConnectionString;
            connMgr.Properties["Password"].SetValue(connMgr, password);
            SetUpConnection(connMgr);
            success = RetrieveMetaData();
            SetUpConnection(GetConnectionManager(connection));
            ParentDataFlowTask.ParentPackage.Project.Project.ConnectionManagerItems.Remove("DummyConnToForce.conmgr");
            return success;
        }

        public bool RetrieveMetaData_Package(string connection, string password)
        {
            ConnectionManager origConnectionManager = GetConnectionManager(connection);
            bool success = false;
            var connMgr = ParentDataFlowTask.RootPackage.Connections.Add(origConnectionManager.CreationName) as ConnectionManager;
            connMgr.Name = "DummyConnToForce";
            connMgr.ConnectionString = origConnectionManager.ConnectionString;
            connMgr.Properties["Password"].SetValue(connMgr, password);
            connMgr.Properties["InitialCatalog"].SetValue(connMgr, origConnectionManager.Properties["InitialCatalog"].GetValue(origConnectionManager));
            SetUpConnection(connMgr);
            success = RetrieveMetaData();
            SetUpConnection(GetConnectionManager(connection));
            ParentDataFlowTask.RootPackage.Connections.Remove("DummyConnToForce");
            return success;
        }


        #endregion

        #region Get Output

        #region Get IDTSOutput100

        #region Get IDTSOutput100 from Output Name

        /// <summary>
        /// Get IDTSOutput100 from Name
        /// </summary>
        /// <param name="outputName"></param>
        /// <returns></returns>
        internal IDTSOutput100 GetOutputFromName(string outputName)
        {
            return ComponentMetaData.OutputCollection[outputName];
        }

        #endregion

        #region Get IDTSOutput100 from Output ID

        /// <summary>
        /// Get IDTSOutput100 from output Id
        /// </summary>
        /// <param name="outputId"></param>
        /// <returns></returns>
        internal IDTSOutput100 GetOutputFromID(int outputId)
        {
            return ComponentMetaData.OutputCollection.GetObjectByID(outputId);
        }

        #endregion

        #region Get IDTSOutput100 from Output Index

        /// <summary>
        /// Get IDTSOutput100 from Output Index in the OutputCollection
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        internal IDTSOutput100 GetOutputFromIndex(int idx)
        {
            return ComponentMetaData.OutputCollection[idx];
        }

        #endregion

        #region Get Output Index from Name

        public int GetOutputIndexFromName(string outputName)
        {
            //int id = 0;
            //for (int i = 0; i < ComponentMetaData.OutputCollection.Count; i++)
            //{
            //    if (outputName == ComponentMetaData.OutputCollection[i].Name)
            //        id = i;
            //}
            //return id;
            
            return ComponentMetaData.OutputCollection.GetObjectIndexByID(GetOutputFromName(outputName).ID);
        }

        /// <summary>
        /// Get the Name of an IDTSOutput100 given its Index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string GetOutputNameFromIndex(int idx)
        {
            return GetOutputFromIndex(idx).Name;
        }

        #endregion

        #region Get OutputID from Name

        public int GetOutputIDFromName(string outputName)
        {
            return GetOutputFromName(outputName).ID;
        }

        #endregion

        #endregion

        #endregion

        #region Get Input

        internal IDTSInput100 GetInputFromIndex(int idx)
        {
            return ComponentMetaData.InputCollection[idx];
        }

        internal IDTSInput100 GetInputFromName(string inputName)
        {
            return ComponentMetaData.InputCollection[inputName];
        }

        internal int GetInputIDFromName(string inputName)
        {
            return GetInputFromName(inputName).ID;
        }

        #endregion

        #region Set Usage Type

        internal IDTSInputColumn100 SetInputColumnDTSUsageType(IDTSInput100 input, string columnName, UsageType dtsUsageType)
        {
            //  keep track of hte columns whose usage type is set to ut_readwrite...for these cols, we want to prevent a change to ut_readonly
            if (dtsUsageType == UsageType.UT_READWRITE)
                _readWriteCols.Add(columnName);

            IDTSVirtualInput100 virtualInput = input.GetVirtualInput();
            IDTSInputColumn100 inputColumn = DesignTimeComponent.SetUsageType(
                input.ID,
                virtualInput,
                virtualInput.VirtualInputColumnCollection[columnName].LineageID,
                DtsUtility.EnumAToEnumB<UsageType, DTSUsageType>(dtsUsageType)
                );

            return inputColumn;
        }

        #endregion

        #region SetCustomPropertyToOutput

        /// <summary>
        /// Add a custom property to an output
        /// For internal use from within this library. Not for use from outside libraries
        /// </summary>
        /// <param name="output"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        internal IDTSCustomProperty100 SetCustomPropertyToOutput(IDTSOutput100 output, string propertyName, object propertyValue)
        {
            bool propertyExists = false;
            IDTSCustomProperty100 prop = null;
            for (int i = 0; i < output.CustomPropertyCollection.Count; i++)
            {
                if (output.CustomPropertyCollection[i].Name == propertyName)
                {
                    propertyExists = true;
                    break;
                }
            }

            if (propertyExists)
            {
                prop = DesignTimeComponent.SetOutputProperty(output.ID, propertyName, propertyValue);
            }
            else
            {
                prop = output.CustomPropertyCollection.New();
                prop.Name = propertyName;
                prop.Value = propertyValue;
            }
            return prop;
        }

        #endregion

        #region Get IDTSOutputColumn100

        internal IDTSOutputColumn100 GetOutputColumn(string outputName, string columnName)
        {
            return GetOutputFromName(outputName).OutputColumnCollection[columnName];
        }

        #endregion

        #region Set Custom Property to Output Column

        /// <summary>
        /// Sets a custom property to an output column; adds it if the property does not exist.
        /// For internal use within this library. Not for use from outside libraries
        /// </summary>
        /// <param name="output"></param>
        /// <param name="outputColumn"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="containsId"></param>
        /// <returns></returns>
        internal IDTSCustomProperty100 SetCustomPropertyToOutputColumn(IDTSOutput100 output, IDTSOutputColumn100 outputColumn, string propertyName, object propertyValue, bool containsId = false)
        {
            bool propertyExists = false;
            IDTSCustomProperty100 customProperty = null;
            for (int i = 0; i < outputColumn.CustomPropertyCollection.Count; i++)
            {
                if (outputColumn.CustomPropertyCollection[i].Name == propertyName)
                {
                    propertyExists = true;
                    break;
                }
            }
            if (propertyExists)
            {
                customProperty = DesignTimeComponent.SetOutputColumnProperty(output.ID, outputColumn.ID, propertyName, propertyValue);
                customProperty.ContainsID = containsId;
            }
            else
            {
                customProperty = outputColumn.CustomPropertyCollection.New();
                customProperty.Name = propertyName;
                customProperty.Value = propertyValue;
                customProperty.ContainsID = containsId;
            }

            return customProperty;
        }

        /// <summary>
        /// Sets a custom property to an output column; adds it if the property does not exist.
        /// For use from outside libraries
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="outputColumnName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetCustomPropertyToOutputColumn(string outputName, string outputColumnName, string propertyName, string propertyValue)
        {
            SetCustomPropertyToOutputColumn(GetOutputFromName(outputName), GetOutputColumn(outputName, outputColumnName), propertyName, propertyValue);
        }

        #endregion

        #region Get IDTSInputColumn100

        internal IDTSInputColumn100 GetInputColumn(string inputName, string inputColumnName)
        {
            return GetInputFromName(inputName).InputColumnCollection[inputColumnName];
        }

        internal IDTSInputColumn100 GetInputColumn(string inputName, int lineageId)
        {
            return GetInputFromName(inputName).InputColumnCollection[lineageId];
        }

        #endregion

        #region Set Custom Property to Input Column

        /// <summary>
        /// Sets a custom property to an output column; adds it if the property does not exist.
        /// For internal use within this library. Not for use from outside libraries
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputColumn"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        internal void SetCustomPropertyToInputColumn(IDTSInput100 input, IDTSInputColumn100 inputColumn, string propertyName, object propertyValue, bool containsId = false)
        {
            bool propertyExists = false;
            IDTSCustomProperty100 prop = null;
            for (int i = 0; i < inputColumn.CustomPropertyCollection.Count; i++)
            {
                if (inputColumn.CustomPropertyCollection[i].Name == propertyName)
                {
                    propertyExists = true;
                    //prop = 
                    break;
                }
            }
            if (propertyExists)
            {
                prop = DesignTimeComponent.SetInputColumnProperty(input.ID, inputColumn.ID, propertyName, propertyValue);
                prop.ContainsID = containsId;

            }
            else
            {
                prop = inputColumn.CustomPropertyCollection.New();
                prop.Name = propertyName;
                prop.Value = propertyValue;
                prop.ContainsID = containsId;
            }
        }

        /// <summary>
        /// Sets a custom property to an output column; adds it if the property does not exist.
        /// For use from outside libraries
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="inputColumnName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetCustomPropertyToInputColumn(string inputName, string inputColumnName, string propertyName, object propertyValue, bool containsId = false)
        {
            SetCustomPropertyToInputColumn(GetInputFromName(inputName), GetInputColumn(inputName, inputColumnName), propertyName, propertyValue, containsId);
        }

        #endregion

        #region SetUp a Connection Manager

        internal void SetUpConnection(ConnectionManager cmg)
        {
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = cmg.ID;
        }

        internal void SetUpConnection(ConnectionManager cmg, int index)
        {
            ComponentMetaData.RuntimeConnectionCollection[index].ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
            ComponentMetaData.RuntimeConnectionCollection[index].ConnectionManagerID = cmg.ID;
        }

        public void SetUpConnection(ISConnectionManager connectionManager, int index)
        {
            SetUpConnection(connectionManager.ConnectionManager, index);
        }

        #endregion

        #region Get Connection Manager from Name

        internal ConnectionManager GetConnectionManager(string connectionName)
        {
            ConnectionManager cm = ParentDataFlowTask.RootPackage.Connections[connectionName];
            
            //for (int c = 0; c < ParentDataFlowTask.RootPackage.Connections.Count; c++)
            //{
            //    if (connectionName == ParentDataFlowTask.RootPackage.Connections[c].Name)
            //        cm = ParentDataFlowTask.RootPackage.Connections[c];
            //}

            //for (int c = 0; c < ParentDataFlowTask.ParentProject.ConnectionManagerItems_m.Count; c++)
            //{
            //    if (connectionName == ParentDataFlowTask.ParentProject.ConnectionManagerItems_m[c].ConnectionManager.Name)
            //        cm = ParentDataFlowTask.ParentProject.ConnectionManagerItems_m[c].ConnectionManager;
            //}

            return cm;
        }

        public ISConnectionManager GetProjectConnectionManager(string connectionName)
        {
            ConnectionManager cm = ParentDataFlowTask.RootPackage.Connections[connectionName];
            return (ISConnectionManager)cm;
        }

        #endregion

        #region Update Connection



        public void updateRequiredConnection(String connectionManagerId, ISConnectionManager newConnectionManager)
        {
            foreach (IDTSRuntimeConnection100 conn in ComponentMetaData.RuntimeConnectionCollection)
            {
                Console.WriteLine("Working on " + conn.ConnectionManagerID);
                if (connectionManagerId == conn.ConnectionManagerID)
                {
                    Console.WriteLine("\t" + "Found Match " + conn.ConnectionManagerID);
                    conn.ConnectionManager = DtsConvert.GetExtendedInterface(newConnectionManager.ConnectionManager);
                    ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManagerID = newConnectionManager.ConnectionManager.ID;

                }
            }
        }

        public List<RuntimeConnection> getListOfConnections()
        {
            List<RuntimeConnection> conns = new List<RuntimeConnection>();
            for(int i = 0; i < ComponentMetaData.RuntimeConnectionCollection.Count; i++)
            {
                conns.Add(new RuntimeConnection { ConnectionManagerID = ComponentMetaData.RuntimeConnectionCollection[i].ConnectionManagerID, Index = i });
            }

            /*
            List<String> conns = new List<string>();
            Console.WriteLine(ComponentMetaData.RuntimeConnectionCollection.Count);
            foreach (IDTSRuntimeConnection100 conn in ComponentMetaData.RuntimeConnectionCollection)
            {
                //ISConnectionManager connMgr = new ISConnectionManager(conn.ConnectionManager);
                //(conn.ConnectionManager as Microsoft.SqlServer.Dts.Runtime.ConnectionManager).Name
                conns.Add(conn.ConnectionManagerID);
            }
            */

            return conns;
        }

        #endregion

        #endregion

        #region Helper Methods

        #region GetSet Custom Property value

        /// <summary>
        ///  The Custom Property Collection only contains the "custom" properties of a component.
        ///  Common properties of all components such as Name, Description etc are not available in this colection.
        ///  Therefore calling SetCustomProperty and GetCustomProperty on common properties will result in error.
        /// </summary>
        internal T CustomPropertyGetter<T>(string propertyName)
        {
            return (T)ComponentMetaData.CustomPropertyCollection[propertyName].Value;
        }

        /// <summary>
        ///  The Custom Property Collection only contains the "custom" properties of a component.
        ///  Common properties of all components such as Name, Description etc are not available in this colection.
        ///  Therefore calling SetCustomProperty and GetCustomProperty on common properties will result in error.
        /// </summary>
        internal void CustomPropertySetter<T>(string propertyName, T value)
        {
            DesignTimeComponent.SetComponentProperty(propertyName, value);
        }

        #endregion

        #endregion

        #region Helper Properties

        internal int _numberOfOutputsAllowed = -1;
        internal int _numberOfInputsAllowed = -1;

        #endregion

        #region Output Collection

        public List<ISOutput> GetOutputCollection()
        {
            List<ISOutput> _outputCollection = new List<ISOutput>();
            for(int i =0; i < ComponentMetaData.OutputCollection.Count; i++)
            {
                _outputCollection.Add(new ISOutput(this, i));
            }
            return _outputCollection;
        }

        #endregion

        #region Output Collection

        public List<ISInput> GetInputCollection()
        {
            List<ISInput> _inputCollection = new List<ISInput>();
            for (int i = 0; i < ComponentMetaData.InputCollection.Count; i++)
            {
                _inputCollection.Add(new ISInput(this, i));
            }
            return _inputCollection;
        }

        #endregion

    }
}
