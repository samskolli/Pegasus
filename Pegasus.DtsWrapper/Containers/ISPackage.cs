using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using wrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Pegasus.DtsWrapper
{
    public class ISPackage : ISEventsProviderAsIDTSSequence
    {
        #region ctor

        #region Constructor that accepts a name of the Package

        /// <summary>
        /// Constructor that accepts the type of executable (Object Moniker), a name of the executable and the immediate parent.
        /// </summary>
        /// <param name="objectMoniker">The type of the Executable</param>
        /// <param name="displayName">The name of the executable</param>
        /// <param name="immediateContainer">The immediate parent of the executable</param>
        public ISPackage(string displayName):
            base(null, displayName, null)
        {
            Package = (Package)Executable;
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.Package object

        internal ISPackage(Package package) : base (package)
        {
            Package = package;
        }

        #endregion

        #region A constructor that accepts a Package name and parent Project

        /// <summary>
        /// A constructor that accepts a Package name and parent project. 
        /// Check if a package with the given name already exist in the package.
        /// If it exists:
        ///     if the reuse flag is true, then assign the found Package to the relevant package variable.
        ///  If the package does not exist (with the same name) or the reuse flag is flag, then a new package is created
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="project"></param>
        public ISPackage(string packageName, ISProject project, bool reusePackageIfExists = true) : base(packageName, project)
        {
            AddToProject(packageName, project, reusePackageIfExists);
            Project = project;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal Package Package { get; set; }

        #endregion

        #region DtsObject Properties

        #region CertificateContext

        /// <summary>
        /// Certificate Context
        /// </summary>
        public long CertificateContext
        {
            get { return Package.CertificateContext; }
            set { Package.CertificateContext = value; }
        }

        #endregion

        #region CheckpointFileName

        /// <summary>
        /// The name of the file that captures the checkpoint information, which enables a package to restart.
        /// </summary>
        public string CheckpointFileName
        {
            get { return Package.CheckpointFileName; }
            set { Package.CheckpointFileName = value; }
        }

        #endregion

        #region CheckpointUsage

        /// <summary>
        /// Value that specifies if or when a package is restarted. 
        /// </summary>
        public CheckpointUsage CheckpointUsage
        {
            get { return DtsUtility.EnumAToEnumB<DTSCheckpointUsage, CheckpointUsage>(Package.CheckpointUsage); }
            set { Package.CheckpointUsage = DtsUtility.EnumAToEnumB<CheckpointUsage, DTSCheckpointUsage>(value); }
        }

        #endregion

        #region CheckSignatureOnLoad

        /// <summary>
        /// Value indicating whether the digital signature is checked when a package is loaded.
        /// </summary>
        public bool CheckSignatureOnLoad
        {
            get { return Package.CheckSignatureOnLoad; }
            set { Package.CheckSignatureOnLoad = value; }
        }

        #endregion

        #region CreationDate

        /// <summary>
        /// the date and time that the package was created.
        /// </summary>
        public DateTime CreationDate
        {
            get { return Package.CreationDate; }
            set { Package.CreationDate = value; }
        }

        #endregion

        #region CreatorComputerName

        /// <summary>
        /// The name of the computer on which the package was created.
        /// </summary>
        public string CreatorComputerName
        {
            get { return Package.CreatorComputerName; }
            set { Package.CreatorComputerName = value; }
        }

        #endregion

        #region CreatorName

        /// <summary>
        /// Individual who created the package.
        /// </summary>
        public string CreatorName
        {
            get { return Package.CreatorName; }
            set { Package.CreatorName = value; }
        }

        #endregion

        #region DumpOnAnyError

        /// <summary>
        /// Specify if a dump file should be generated when any error occurs during package execution.
        /// </summary>
        private bool DumpOnAnyError
        {
            get { return Package.DumpOnAnyError; }
            set { Package.DumpOnAnyError = value; }
        }

        #endregion

        #region EnableConfigurations

        /// <summary>
        /// value that indicates whether the package loads configurations.
        /// </summary>
        public bool EnableConfigurations
        {
            get { return Package.EnableConfigurations; }
            set { Package.EnableConfigurations = value; }
        }

        #endregion

        #region EnableDump

        /// <summary>
        /// Specify if the dump file generation is enabled.
        /// </summary>
        public bool EnableDump
        {
            get { return Package.EnableDump; }
            set { Package.EnableDump = value; }
        }

        #endregion

        #region EncryptCheckpoints

        /// <summary>
        /// Value that indicates whether the checkpoint files are encrypted.
        /// </summary>
        public bool EncryptCheckpoints
        {
            get { return Package.EncryptCheckpoints; }
            set { Package.EncryptCheckpoints = value; }
        }

        #endregion

        #region HasExpressions

        /// <summary>
        /// Value that indicates whether the package has expressions.
        /// </summary>
        public bool HasExpressions
        {
            get { return Package.HasExpressions; }
        }

        #endregion

        #region IgnoreConfigurationsOnLoad
        /// <summary>
        /// Value that indicates whether the package ignores configurations when the package is loaded.
        /// </summary>
        public bool IgnoreConfigurationsOnLoad
        {
            get { return Package.IgnoreConfigurationsOnLoad; }
            set { Package.IgnoreConfigurationsOnLoad = value; }
        }

        #endregion

        #region InteractiveMode

        /// <summary>
        /// Value that indicates whether the tasks should show user interface objects while executing.
        /// </summary>
        public bool InteractiveMode
        {
            get { return Package.InteractiveMode; }
            set { Package.InteractiveMode = value; }
        }

        #endregion

        #region MaxConcurrentExecutables

        /// <summary>
        /// The number of threads that a package can create.
        /// </summary>
        public int MaxConcurrentExecutables
        {
            get { return Package.MaxConcurrentExecutables; }
            set { Package.MaxConcurrentExecutables = value; }
        }

        #endregion

        #region OfflineMode

        /// <summary>
        /// Value that indicates whether the package is working in offline mode.
        /// </summary>
        public bool OfflineMode
        {
            get { return Package.OfflineMode; }
            set { Package.OfflineMode = value; }
        }

        #endregion

        #region PackagePassword

        /// <summary>
        /// Value of the password for the package.
        /// </summary>
        public string PackagePassword
        {
            set { Package.PackagePassword = value; }
        }

        #endregion

        #region PackagePriorityClass

        /// <summary>
        /// The Win32 thread priority class of the package thread.
        /// </summary>
        public PriorityClass PackagePriorityClass
        {
            get { return DtsUtility.EnumAToEnumB<DTSPriorityClass, PriorityClass>(Package.PackagePriorityClass); }
            set { Package.PackagePriorityClass = DtsUtility.EnumAToEnumB<PriorityClass, DTSPriorityClass>(value); }
        }

        #endregion

        #region PackageType

        /// <summary>
        /// Value that identifies the tool that created the package.
        /// </summary>
        public PackageType PackageType
        {
            get { return DtsUtility.EnumAToEnumB<DTSPackageType, PackageType>(Package.PackageType); }
            set { Package.PackageType = DtsUtility.EnumAToEnumB<PackageType, DTSPackageType>(value); }
        }

        #endregion

        #region Parameters

        /// <summary>
        /// The parameters collection for the package. This property is for internal use within this assembly.
        /// </summary>
        internal Parameters Parameters_m
        {
            get { return Package.Parameters; }
        }

        private List<ISParameter> _parameters = new List<ISParameter>();

        /// <summary>
        /// The parameters collection for the package. This property is for internal use within this assembly.
        /// </summary>
        public List<ISParameter> Parameters
        {
            get
            {
                _parameters.Clear();
                for (int p = 0; p < Parameters_m.Count; p++)
                {
                    _parameters.Add(new ISParameter(Parameters_m[p]));
                }
                return _parameters;
            }
        }

        #endregion

        #region Project

        internal wrapper.IDTSProject100 Project_m
        {
            get { return Package.Project; }
        }

        /// <summary>
        /// The specified project associated with the package.
        /// </summary>
        private ISProject _project;
        public ISProject Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
            }
        }

        //internal IDTSProject100 Project
        //{
        //    get { return Package.Project; }
        //    //set { Package.Project = value; }
        //}

        #endregion

        #region ProtectionLevel

        /// <summary>
        /// level of protection on the package.
        /// </summary>
        public ProtectionLevel ProtectionLevel
        {
            get { return DtsUtility.EnumAToEnumB<DTSProtectionLevel, ProtectionLevel>(Package.ProtectionLevel); }
            set { Package.ProtectionLevel = DtsUtility.EnumAToEnumB<ProtectionLevel, DTSProtectionLevel>(value); }
        }

        #endregion

        #region SafeRecursiveProjectPackageExecution

        /// <summary>
        /// Indicates whether recursive execution of the package is safe.
        /// </summary>
        public bool SafeRecursiveProjectPackageExecution
        {
            get { return Package.SafeRecursiveProjectPackageExecution; }
            set { Package.SafeRecursiveProjectPackageExecution = value; }
        }

        #endregion

        #region SaveCheckpoints

        /// <summary>
        /// Indicates whether the package will use checkpoints during package execution.
        /// </summary>
        public bool SaveCheckpoints
        {
            get { return Package.SaveCheckpoints; }
            set { Package.SaveCheckpoints = value; }
        }

        #endregion

        #region SuppressConfigurationWarnings

        /// <summary>
        /// Value that indicates whether the warnings generated by configurations are suppressed.
        /// </summary>
        public bool SuppressConfigurationWarnings
        {
            get { return Package.SuppressConfigurationWarnings; }
            set { Package.SuppressConfigurationWarnings = value; }
        }

        #endregion

        #region VersionBuild

        /// <summary>
        /// Build version of the package.
        /// </summary>
        public int VersionBuild
        {
            get { return Package.VersionBuild; }
            set { Package.VersionBuild = value; }
        }

        #endregion

        #region VersionComments

        /// <summary>
        /// Version comments associated with the package.
        /// </summary>
        public string VersionComments
        {
            get { return Package.VersionComments; }
            set { Package.VersionComments = value; }
        }

        #endregion

        #region VersionGUID

        /// <summary>
        /// Version comments associated with the package.
        /// </summary>
        public string VersionGUID
        {
            get { return Package.VersionGUID; }
        }

        #endregion

        #region VersionMajor

        /// <summary>
        /// Major build version of the package.
        /// </summary>
        public int VersionMajor
        {
            get { return Package.VersionMajor; }
            set { Package.VersionMajor = value; }
        }

        #endregion

        #region VersionMinor

        /// <summary>
        /// Minor build version of the package.
        /// </summary>
        public int VersionMinor
        {
            get { return Package.VersionMinor; }
            set { Package.VersionMinor = value; }
        }

        #endregion

        #region Connections

        public List<ISConnectionManager> getConnections()
        {
            List<ISConnectionManager> connections = new List<ISConnectionManager>();
            var conns = Package.Connections;

            for (int i = 0; i < Package.Connections.Count; i++)
            {
                connections.Add(Package.Connections[i]);
            }
            return connections;
        }

        #endregion

        #endregion

        #region Methods

        #region Add Parameter

        /// <summary>
        /// Method to add a parameter.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public ISParameter AddParameter(string paramName, TypeCode dataType)
        {
            ISParameter param = new ISParameter(Parameters_m.Add(paramName, dataType));
            return param;
        }

        #endregion

        #region Get a parameter with name

        /// <summary>
        /// Method to access a parameter given its name. You can also use the ISParameter constructor directly for the same result.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public ISParameter GetParameter(string parameterName)
        {
            return new ISParameter(Parameters_m[parameterName]);
        }

        #endregion

        #region Add Package to a Project

        /// <summary>
        /// Check if a package with the given name already exist in the package.
        /// If it exists:
        ///     if the reuse flag is true, then assign the found Package to the relevant package variable.
        ///         (in case of reuse = true) since the package variable is assgined to an existing package, that means that whatever changes made to the package variable before calling this method are overwritten with the existing package's properties
        ///  If the package does not exist (with the same name) or the reuse flag is flag, then a new package is created
        /// </summary>
        /// <param name="project"></param>
        /// <param name="reusePackageIfExists"></param>
        public void AddToProject(ISProject project, bool reusePackageIfExists = true)
        {
            AddToProject(Name, project, reusePackageIfExists);
        }

        /// <summary>
        /// Check if a package with the given name already exist in the package.
        /// If it exists:
        ///     if the reuse flag is true, then assign the found Package to the relevant package variable.
        ///         (in case of reuse = true) since the package variable is assgined to an existing package, that means that whatever changes made to the package variable before calling this method are overwritten with the existing package's properties
        ///  If the package does not exist (with the same name) or the reuse flag is flag, then a new package is created
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="project"></param>
        /// <param name="reusePackageIfExists"></param>
        private void AddToProject(string packageName, ISProject project, bool reusePackageIfExists = true)
        {
            bool packageExistsInProject = false;
            for (int p = 0; p < project.PackageItems_m.Count; p++)
            {
                if (project.PackageItems_m[p].Package.Name == packageName)
                {
                    packageExistsInProject = true;
                    if (reusePackageIfExists)
                    {
                        Package = project.PackageItems_m[p].Package;
                    }
                }
            }

            if (packageExistsInProject && reusePackageIfExists == false)
            {
                project.PackageItems_m.Remove(packageName + ".dtsx");
                packageExistsInProject = false;
            }

            if (!(packageExistsInProject))
            {
                if (Package == null)
                {
                    Package = new Package();
                    Package.Name = packageName;
                }                
                project.PackageItems_m.Add(Package, packageName + ".dtsx");
            }
            Executable = (Executable)Package;
            Container = (DtsContainer)Package;
            EventsProvider = (EventsProvider)Package;
            EventsProviderAsIDTSSequence = (IDTSSequence)Package;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Package.Dispose();
        }

        #endregion

        #endregion

        public static implicit operator ISPackage(Package package)
        {
            if (package == null)
                return null;

            return new ISPackage(package);
        }

    }
}
