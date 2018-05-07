using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper; // Data Flow Task
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISDataFlowTask : ISTaskHost
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// In addition to assigning the MainPipe object, we also get a reference to the actual Package that contains this data flow task
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISDataFlowTask(string displayName, ISContainer immediateContainer, ISProject project = null, ISPackage package = null):
            base("Microsoft.Pipeline", displayName, immediateContainer)
        {
            MainPipe = TaskHost.InnerObject as MainPipe;

            /*if (MainPipe == null)
            {
                Console.WriteLine("Main Pipe is null");
            } else
            {
                Console.WriteLine("MainPipe NOT null");
            }*/

            if(package == null)
            {
                RootPackage = (ISContainer.GetActualPackage(immediateContainer).Container) as Microsoft.SqlServer.Dts.Runtime.Package;
                ParentPackage = (ISPackage)(RootPackage as Microsoft.SqlServer.Dts.Runtime.Package);
            }
            else
            {
                ParentPackage = package;
                RootPackage = package.Package;
            }
            if(project == null)
            {
                if(ParentPackage.Project == null)
                {
                    ParentPackage.Project = project;
                }
            }
            else
            {
                ParentPackage.Project = project;
            }
        }

        #endregion

        [Obsolete]
        private ISContainer GetPackage(ISContainer container)
        {
            if (container.Parent_m == null)
                return container;
            else
            {
                return GetPackage(container.Parent);
            }
        }

        #endregion

        #region Wrapped Dts Object

        internal MainPipe MainPipe { get; set; }

        #endregion

        #region Dts Object Properties

        public bool AutoGenerateIDForNewObjects
        {
            get { return MainPipe.AutoGenerateIDForNewObjects; }
            set { MainPipe.AutoGenerateIDForNewObjects = value; }
        }

        //BLOBTempStoragePath
        public string BLOBTempStoragePath
        {
            get { return MainPipe.BLOBTempStoragePath; }
            set { MainPipe.BLOBTempStoragePath = value; }
        }

        //BufferManager
        internal IDTSBufferManager100 BufferManager_m
        {
            get { return MainPipe.BufferManager; }
        }

        //BufferTempStoragePath
        public string BufferTempStoragePath
        {
            get { return MainPipe.BufferTempStoragePath; }
            set { MainPipe.BufferTempStoragePath = value; }
        }

        //ComponentMetaDataCollection
        internal IDTSComponentMetaDataCollection100 ComponentMetaDataCollection_m
        {
            get { return MainPipe.ComponentMetaDataCollection; }
        }

        //private List<ISPipelineComponent> _pipelineComponentCollection = new List<ISPipelineComponent>();
        //public List<ISPipelineComponent> ComponentMetaDataCollection
        //{
        //    get
        //    {
        //        _pipelineComponentCollection.Clear();
        //        foreach (IDTSComponentMetaData100 component in ComponentMetaDataCollection_m)
        //        {
        //            ISPipelineComponent pc = new ISPipelineComponent(component.Name, component.ComponentClassID, MainPipe);
        //            _pipelineComponentCollection.Add(pc);
        //        }
        //        return _pipelineComponentCollection;
        //    }
        //}

        //DefaultBufferMaxRows
        public int DefaultBufferMaxRows
        {
            get { return MainPipe.DefaultBufferMaxRows; }
            set { MainPipe.DefaultBufferMaxRows = value; }
        }

        //DefaultBufferSize
        public int DefaultBufferSize
        {
            get { return MainPipe.DefaultBufferSize; }
            set { MainPipe.DefaultBufferSize = value; }
        }

        //EnableCacheUpdate
        public bool EnableCacheUpdate
        {
            get { return MainPipe.EnableCacheUpdate; }
            set { MainPipe.EnableCacheUpdate = value; }
        }

        //EnableDisconnectedColumns
        public bool EnableDisconnectedColumns
        {
            get { return MainPipe.EnableDisconnectedColumns; }
            set { MainPipe.EnableDisconnectedColumns = value; }
        }

        //EngineThreads
        public int EngineThreads
        {
            get { return MainPipe.EngineThreads; }
            set { MainPipe.EngineThreads = value; }
        }

        internal IDTSComponentEvents100 Events_m
        {
            set { MainPipe.Events = value; }
        }

        //IsSavingXml
        public bool IsSavingXml
        {
            get { return MainPipe.IsSavingXml; }
            set { MainPipe.IsSavingXml = value; }
        }

        //  PathCollection
        internal IDTSPathCollection100 PathCollection_m
        {
            get { return MainPipe.PathCollection; }
        }

        //RunInOptimizedMode
        public bool RunInOptimizedMode
        {
            get { return MainPipe.RunInOptimizedMode; }
            set { MainPipe.RunInOptimizedMode = value; }
        }

        //VariableDispenser
        internal IDTSVariableDispenser100 VariableDispenser_m
        {
            set { MainPipe.VariableDispenser = value; }
        }

        #endregion

        #region General Properties

        public ISProject ParentProject { get; set; }
        public ISPackage ParentPackage { get; set; }
        internal Microsoft.SqlServer.Dts.Runtime.Package RootPackage { get; set; }
        
        #endregion
    }
}
