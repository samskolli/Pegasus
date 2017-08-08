using Microsoft.SqlServer.Dts.Tasks.ScriptTask;
using Microsoft.SqlServer.VSTAHosting;
using System.Linq;

namespace Pegasus.DtsWrapper
{
    public class ISScriptTask : ISTaskHost
    {
        #region ctor

        #region Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISScriptTask(string displayName, ISContainer immediateContainer):
            base("STOCK:ScriptTask", displayName, immediateContainer)
        {
            ScriptTask = TaskHost.InnerObject as ScriptTask;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal ScriptTask ScriptTask { get; set; }

        #endregion

        #region Dts Object Properties

        #region DefaultActiveItem

        /// <summary>
        /// Gets the default script.
        /// </summary>
        public string DefaultActiveItem
        {
            get { return ScriptTask.DefaultActiveItem; }
        }

        #endregion

        #region EntryPoint

        /// <summary>
        /// Gets or sets the name of the class that is executed as the entry point.
        /// </summary>
        public string EntryPoint
        {
            get { return ScriptTask.EntryPoint; }
            set { ScriptTask.EntryPoint = value; }
        }

        #endregion

        #region ProjectTemplatePath

        /// <summary>
        /// Gets the path of the specified project template.
        /// </summary>
        public string ProjectTemplatePath
        {
            get { return ScriptTask.ProjectTemplatePath; }
        }

        #endregion

        #region ReadOnlyVariables

        /// <summary>
        /// Gets or sets the comma-delimited list of existing variables made available to the package by the user for read-only access.
        /// </summary>
        public string ReadOnlyVariables
        {
            get { return ScriptTask.ReadOnlyVariables; }
            set { ScriptTask.ReadOnlyVariables = value; }
        }

        #endregion

        #region ReadWriteVariables

        /// <summary>
        /// Gets or sets the comma-delimited list of existing variables made available to the package by the user for read/write access.
        /// </summary>
        public string ReadWriteVariables
        {
            get { return ScriptTask.ReadWriteVariables; }
            set { ScriptTask.ReadWriteVariables = value; }
        }

        #endregion

        #region ScriptingEngine

        /// <summary>
        /// Gets the VSTATaskScriptingEngine class.
        /// </summary>
        internal VSTAScriptingEngine ScriptingEngine
        {
            get { return ScriptTask.ScriptingEngine; }
        }

        #endregion

        #region ScriptLanguage

        /// <summary>
        /// Gets or sets the programming language in which the script is written.
        /// </summary>
        public ScriptTaskLanguage ScriptLanguage
        {
            get { return DtsUtility.StringToEnum<ScriptTaskLanguage>(ScriptTask.ScriptLanguage); }
            set { ScriptTask.ScriptLanguage = VSTAScriptLanguages.GetDisplayName(value.ToString()); }
        }

        #endregion

        #region ScriptLoaded

        /// <summary>
        /// Gets a value indicating whether the task contains custom script.
        /// </summary>
        public bool ScriptLoaded
        {
            get { return ScriptTask.ScriptLoaded; }
        }

        #endregion

        #region ScriptProjectName

        /// <summary>
        /// Gets or sets the name of the script project.
        /// </summary>
        public string ScriptProjectName
        {
            get { return ScriptTask.ScriptProjectName; }
            set
            {
                //ScriptTask.ScriptProjectName = value;
                AddProject(value);
            }
        }

        #endregion

        #region ScriptStorage

        /// <summary>
        /// Gets the VSTAScriptProjectStorage for the script.
        /// </summary>
        internal VSTAScriptProjectStorage ScriptStorage
        {
            get { return ScriptTask.ScriptStorage; }
        }

        #endregion

        #region ScriptMain

        /// <summary>
        /// Gets or sets the content of the ScriptMain.cs file
        /// </summary>
        public string ScriptMain
        {
            get { return (ScriptStorage.ScriptFiles["ScriptMain.cs"] as VSTAScriptProjectStorage.VSTAScriptFile).Data; }
            set
            {
                //ScriptStorage.ScriptFiles["ScriptMain.cs"] = new VSTAScriptProjectStorage.VSTAScriptFile(VSTAScriptProjectStorage.Encoding.UTF8, value);
                //ScriptingEngine.LoadProjectFromStorage();
                //SaveProject();
                //ReplaceFileContent("ScriptMain.cs", value);
                SetClassFile("ScriptMain.cs", value);
            }
        }

        #endregion

        #region References

        /// <summary>
        /// A list of references to remove; the references should be passed in as a delimited string with the '|' (pipe) as the delimiter.
        /// </summary>
        public string ReferencesToRemove
        {
            set
            {
                bool projectLoaded = ScriptingEngine.VstaHelper.LoadVSTA2Project(ScriptStorage, value.Split('|').ToList(), null);
                SaveProject();
            }
        }

        /// <summary>
        /// A list of references to Add; the references should be passed in as a delimited string with the '|' (pipe) as the delimiter.
        /// </summary>
        public string ReferencesToAdd
        {
            set
            {
                bool projectLoaded = ScriptingEngine.VstaHelper.LoadVSTA2Project(ScriptStorage, null, value.Split('|').ToList());
                SaveProject();
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Add a Project.
        /// </summary>
        /// <param name="projectName"></param>
        public void AddProject(string projectName)
        {
            SetExpression("ScriptProjectName", "\"" + projectName + "\"");
            bool projectLoaded = ScriptingEngine.VstaHelper.LoadVSTA2Project(ScriptStorage, null, null);
            if (projectLoaded)
            {

            }
            else
            {
                ScriptingEngine.VstaHelper.LoadNewProject(ProjectTemplatePath, null, projectName);
            }
            ScriptingEngine.ProjectName = projectName;
            ScriptTask.ScriptProjectName = projectName;
            SaveProject();
        }

        /// <summary>
        /// Save the Project
        /// </summary>
        public void SaveProject()
        {
            ScriptingEngine.VstaHelper.Build("Debug");
            bool s = ScriptingEngine.VstaHelper.SaveProjectToStorage(ScriptStorage);
        }

        /// <summary>
        /// Replace the contents of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public void ReplaceFileContent(string fileName, string content)
        {
            ScriptStorage.ScriptFiles[fileName] = new VSTAScriptProjectStorage.VSTAScriptFile(VSTAScriptProjectStorage.Encoding.UTF8, content);
            ScriptingEngine.LoadProjectFromStorage();
            SaveProject();
        }

        /// <summary>
        /// Given a file name and its content, this method will add that file (with that content) to the project (if not exists) and updates the content if exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public void SetClassFile(string fileName, string content)
        {
            ScriptingEngine.VstaHelper.Initalize("", true);
            bool fileAdded = ScriptingEngine.VstaHelper.AddFileToProject(fileName, content);
            if (fileAdded)
            {

            }
            else
            {
                ReplaceFileContent(fileName, content);
            }
            SaveProject();
        }

        #endregion
    }
}
