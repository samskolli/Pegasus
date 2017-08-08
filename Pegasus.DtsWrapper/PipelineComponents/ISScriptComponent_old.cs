using System;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISScriptComponent : ISPipelineComponent
    {
        #region ctor

        #region ctor that accepts the parent data flow task, and a name

        public ISScriptComponent(ISDataFlowTask parentDataFlowTask, string componentname) :
            base(componentname, "Microsoft.SqlServer.Dts.Pipeline.ScriptComponentHost, Microsoft.SqlServer.TxScript, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", parentDataFlowTask.MainPipe)
        {
            
        }

        #endregion

        #region ctor that accepts the parent data flow task, and a name and a component to be connected to

        public ISScriptComponent(ISDataFlowTask parentDataFlowTask, string componentname, string sourceComponentName, int sourceComponentOutputIndex = 0, int inputCollectionIndex = 0) :
            this(parentDataFlowTask, componentname)
        {
            ConnectToAnotherPipelineComponent(sourceComponentName, sourceComponentOutputIndex, inputCollectionIndex);
        }

        #endregion

        #endregion

        #region Component Properties

        #region Properties

        #region Script Component Type

        public ScriptComponentType ScriptComponentType
        {
            get
            {
                ScriptComponentType type = ScriptComponentType.Source;
                if (ComponentMetaData.InputCollection.Count > 0 && ComponentMetaData.OutputCollection.Count > 0)
                {
                    type = ScriptComponentType.Transform;
                    //_numberOfInputsAllowed = -1;
                    //_numberOfOutputsAllowed = -1;
                }
                else if (ComponentMetaData.InputCollection.Count > 0)
                {
                    type = ScriptComponentType.Destination;
                    //_numberOfInputsAllowed = 1;
                    //_numberOfOutputsAllowed = 0;
                }
                else
                {
                    type = ScriptComponentType.Source;
                    //_numberOfInputsAllowed = 0;
                    //_numberOfOutputsAllowed = -1;
                }
                return type;
            }
            set
            {
                SetupScriptComponentType(value);
            }
        }

        #endregion

        #region ScriptComponentHost

        private ScriptComponentHost _scriptComponentHost { get { return (DesignTimeComponent as IDTSManagedComponent100).InnerObject as ScriptComponentHost; } }

        #endregion

        #region ScriptingEngine

        private VSTAComponentScriptingEngine _currentScriptingEngine
        {
            get
            {
                return _scriptComponentHost.CurrentScriptingEngine;
            }
        }

        #endregion

        #region Source Code

        public string[] SourceCode
        {
            get { return CustomPropertyGetter<string[]>("SourceCode"); }
            set { DesignTimeComponent.SetComponentProperty("SourceCode", value); }
        }

        #endregion

        #region Standard Files To Ignore

        private string[] _ignoreFiles = new string[] { @"Properties\Resources.resx", "BufferWrapper.cs", "ComponentWrapper.cs", "Project", @"Properties\AssemblyInfo.cs", @"Properties\Settings.settings", @"Properties\Resources.Designer.cs" };

        #endregion

        #region ProjectName

        public string ProjectName
        {
            get { return VSTAProjectName; }
            set { AddProject(value); }
        }

        internal string VSTAProjectName
        {
            get { return CustomPropertyGetter<string>("VSTAProjectName"); }
            set { CustomPropertySetter<string>("VSTAProjectName", value); }
        }

        #endregion

        #region ProjectTemplatePath

        public string ProjectTemplatePath
        {
            get { return _scriptComponentHost.ProjectTemplatePath; }
        }

        #endregion

        #region ReadOnlyVariables

        public string ReadOnlyVariables
        {
            get { return CustomPropertyGetter<string>("ReadOnlyVariables"); }
            set
            {
                CustomPropertySetter<string>("ReadOnlyVariables", value);
                string[] previousSourceCode = new string[SourceCode.Length];
                for (int i = 0; i < SourceCode.Length; i++)
                {
                    previousSourceCode.SetValue(SourceCode[i], i);
                }
                RebuildProject(null);
            }
        }

        #endregion

        #region ReadWriteVariables

        public string ReadWriteVariables
        {
            get { return CustomPropertyGetter<string>("ReadWriteVariables"); }
            set
            {
                CustomPropertySetter<string>("ReadWriteVariables", value);
                string[] previousSourceCode = new string[SourceCode.Length];
                for (int i = 0; i < SourceCode.Length; i++)
                {
                    previousSourceCode.SetValue(SourceCode[i], i);
                }
                RebuildProject(null);
            }
        }

        #endregion

        #region ScriptLanguage

        public string ScriptLanguage
        {
            get { return CustomPropertyGetter<string>("ScriptLanguage"); }
        }

        #endregion

        #endregion

        #endregion

        #region Methods

        #region Set up Script Component Type

        private void SetupScriptComponentType(ScriptComponentType type)
        {
            if (type == ScriptComponentType.Source)
            {
                if (ComponentMetaData.InputCollection.Count > 0)
                    ComponentMetaData.InputCollection.RemoveAll();

                foreach (IDTSOutput100 output in ComponentMetaData.OutputCollection)
                {
                    output.SynchronousInputID = 0;
                    output.HasSideEffects = true;
                }
            }
            if (type == ScriptComponentType.Destination)
            {
                if (ComponentMetaData.OutputCollection.Count > 0)
                    ComponentMetaData.OutputCollection.RemoveAll();
            }
        }

        #endregion

        #region Connection

        internal IDTSRuntimeConnection100 AddConnection_m(string referenceConnectionName, string connectionNameInComponent)
        {

            bool connectionExists = false;
            IDTSRuntimeConnection100 conn = (IDTSRuntimeConnection100)null;
            for (int r = 0; r < ComponentMetaData.RuntimeConnectionCollection.Count; r++)
            {
                if (ComponentMetaData.RuntimeConnectionCollection[r].Name == connectionNameInComponent)
                {
                    connectionExists = true;
                    conn = ComponentMetaData.RuntimeConnectionCollection[r];
                    conn.Name = connectionNameInComponent;
                }
            }

            if (connectionExists == false)
            {
                conn = ComponentMetaData.RuntimeConnectionCollection.New();
                ConnectionManager cmg = GetConnectionManager(referenceConnectionName);
                conn.ConnectionManager = DtsConvert.GetExtendedInterface(cmg);
                conn.ConnectionManagerID = cmg.ID;
                conn.Name = connectionNameInComponent;
            }
            return conn;
        }

        public void AddConnection(string referenceConnectionName, string connectionNameInComponent)
        {
            AddConnection_m(referenceConnectionName, connectionNameInComponent);
        }

        #region Get Connection Manager from Name

        internal ConnectionManager GetConnectionManager(string connectionName)
        {
            ConnectionManager cm = null;

            for (int c = 0; c < ParentDataFlowTask.ParentProject.ConnectionManagerItems_m.Count; c++)
            {
                if (connectionName == ParentDataFlowTask.ParentProject.ConnectionManagerItems_m[c].ConnectionManager.Name)
                    cm = ParentDataFlowTask.ParentProject.ConnectionManagerItems_m[c].ConnectionManager;
            }

            return cm;
        }

        #endregion

        #endregion

        #region Project Manipulation

        #region Add Project

        private void AddProject_m(string projectName)
        {
            _scriptComponentHost.CreateNewProject(projectName, false, true);
            _scriptComponentHost.SaveScriptProject();
        }

        public void AddProject(string projectName)
        {
            bool projectExists = false;
            if (SourceCode.Length > 0)
            {
                projectExists = true;
            }

            if (!(projectExists))
            {
                AddProject_m(projectName);
                VSTAProjectName = projectName;
            }
        }

        #endregion

        #region AddFile

        private void AddFile(string fileName, string content)
        {
            _currentScriptingEngine.VstaHelper.AddFileToProject(fileName, content);
            _scriptComponentHost.SaveScriptProject();
        }

        public void Build(string buildConfig)
        {
            _currentScriptingEngine.VstaHelper.Build("");
        }

        #endregion

        #region Set Class File

        public void SetClassFile(string fileName, string content)
        {
            bool fileExists = false;
            foreach (string s in SourceCode)
            {
                if (s == fileName)
                {
                    fileExists = true;
                    break;
                }
            }

            if (fileExists)
            {
                RebuildProject(fileName);
                AddFile(fileName, content);
            }

            if (!(fileExists))
            {
                AddFile(fileName, content);
            }
        }

        #endregion

        #region RebuildProject

        internal void RebuildProject(string fileToExclude)
        {
            string[] previousSourceCode = GetCopyOfSourceCode();
            AddProject_m(ProjectName);
            int iter = 0;
            foreach (string s in previousSourceCode)
            {
                if (iter % 3 == 0)
                {
                    if (s == fileToExclude || s == @"Properties\Resources.resx" || s == "BufferWrapper.cs" || s == "ComponentWrapper.cs" || s.Contains(".csproj") || s == @"Properties\Settings.Designer.cs" || s == "Project" || s == @"Properties\AssemblyInfo.cs" || s == @"Properties\Settings.settings" || s == @"Properties\Resources.Designer.cs")
                    {

                    }
                    else
                    {
                        AddFile(s, previousSourceCode[iter + 2]);
                    }
                }
                iter = iter + 1;
            }
        }

        #endregion

        #region Get File Content

        public string GetFileContent(string fileName)
        {
            int iter = 0;
            string content = "";
            foreach (string s in SourceCode)
            {
                if (s == fileName)
                {
                    content = SourceCode[iter + 2];
                    break;
                }
                iter = iter + 1;
            }
            return content;
        }

        #endregion

        #region Get Previous Source Code

        private string[] GetCopyOfSourceCode()
        {
            string[] previousSourceCode = new string[SourceCode.Length];
            for (int i = 0; i < SourceCode.Length; i++)
            {
                previousSourceCode.SetValue(SourceCode[i], i);
            }
            return previousSourceCode;
        }

        #endregion

        #endregion
    }

    public class ISScriptComponentOutputColumn
    {
        #region Properties

        #region Parent Component

        private ISScriptComponent _parentComponent;

        #endregion

        #region Output

        private IDTSOutput100 _output { get; set; }

        #endregion

        #region OutputColumn

        public ISOutputColumn OutputColumn { get; set; }

        #endregion

        #region Error Row Disposition

        public RowDisposition ErrorRowDisposition
        {
            get { return OutputColumn.ErrorRowDisposition; }
            set { OutputColumn.ErrorRowDisposition = value; }
        }

        #endregion

        #region Truncation Row Disposition

        public RowDisposition TruncationRowDisposition
        {
            get { return OutputColumn.TruncationRowDisposition; }
            set { OutputColumn.TruncationRowDisposition = value; }
        }

        #endregion

        #endregion

        #region ctor

        public ISScriptComponentOutputColumn(ISScriptComponent parentComponent, string outputName, string outputColumnName)
        {
            _parentComponent = parentComponent;
            _output = parentComponent.GetOutputFromName(outputName);
            OutputColumn = new ISOutputColumn(parentComponent, _output.Name, outputColumnName);
            ErrorRowDisposition = RowDisposition.RD_NotUsed;
            TruncationRowDisposition = RowDisposition.RD_NotUsed;
        }

        #endregion

        #region Methods

        public void SetDataTypeProperties(string dataType, int length, int precision, int scale, int codePage)
        {
            OutputColumn.SetDataTypeProperties(dataType, length, precision, scale, codePage);
            if (_parentComponent.SourceCode.Length > 0)
            {
                _parentComponent.RebuildProject(null);
            }
        }

        #endregion
    }

    public class ISScriptComponentConnection
    {
        #region ctor

        public ISScriptComponentConnection(ISScriptComponent parentComponent, string referenceConnectionName, string connectionNameInComponent)
        {
            _connection = parentComponent.AddConnection_m(referenceConnectionName, connectionNameInComponent);

            if (parentComponent.SourceCode.Length > 0)
            {
                parentComponent.RebuildProject(null);
            }
        }

        #endregion

        #region Properties

        private IDTSRuntimeConnection100 _connection;

        public string Name
        {
            get { return _connection.Name; }
            set { _connection.Name = value; }
        }

        public string Description
        {
            get { return _connection.Description; }
            set { _connection.Description = value; }
        }

        #endregion
    }

    public class ISScriptComponentFile
    {
        #region ctor

        public ISScriptComponentFile(ISScriptComponent parentScriptComponent, string fileName, string content)
        {
            parentScriptComponent.SetClassFile(fileName, content);
        }

        //public TestScriptComponentFile(string parentLineage, string fileName, string content, ISProject ssisProject, ISPackage ssisPackage)
        //    : this(new ISScriptTask(ISContainer.GetContainer(ssisProject, ssisPackage, parentLineage).Container), fileName, content)
        //{

        //}

        #endregion
    }
}
