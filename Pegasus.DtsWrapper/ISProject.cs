using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;

namespace Pegasus.DtsWrapper
{
    public class ISProject
    {
        #region ctor

        #region ctor that accepts a filepath

        public ISProject(string filePath, string password)
        {
            if(Path.GetExtension(filePath) == ".ispac")
            {
                IspacFile = filePath;
                InferFileNameAndPath();
                if (File.Exists(IspacFile))
                {
                    _ispacAlreadyExists = true;
                    Project = Project.OpenProject(IspacFile);
                }
                else
                {
                    Project = Project.CreateProject();
                }
            }

            else
            {
                System.Console.WriteLine("Please provide an ispac file");
            }
        }

        #endregion

        #region A constructor that accepts a Microsoft.SqlServer.Dts.Runtime.Project object

        /// <summary>
        /// a ctor that accepts a Microsoft.SqlServer.Dts.Runtime.Project object
        /// </summary>
        /// <param name="project"></param>
        internal ISProject(Project project)
        {
            Project = project;
        }

        #endregion

        #endregion

        #region Wrapped Dts Object

        internal Project Project { get; set; }

        #endregion

        #region DtsObject Properties

        /// <summary>
        /// Collection of the project Connection Manager Items. This property is only for internal use within this assembly.
        /// </summary>
        internal ConnectionManagerItems ConnectionManagerItems_m
        {
            get { return Project.ConnectionManagerItems; }
        }

        /// <summary>
        /// The date and time that the project was created.
        /// </summary>
        public DateTimeOffset CreationDate
        {
            get { return Project.CreationDate; }
            set { Project.CreationDate = value; }
        }

        /// <summary>
        /// The name of the computer on which the project was created.
        /// </summary>
        public string CreatorComputerName
        {
            get { return Project.CreatorComputerName; }
            set { Project.CreatorComputerName = value; }
        }

        /// <summary>
        /// The name of the individual who created the project.
        /// </summary>
        public string CreatorName
        {
            get { return Project.CreatorName; }
            set { Project.CreatorName = value; }
        }

        /// <summary>
        /// The description of the Project object.
        /// </summary>
        public string Description
        {
            get { return Project.Description; }
            set { Project.Description = value; }
        }

        /// <summary>
        /// The format version of the project.
        /// </summary>
        public int FormatVersion
        {
            get { return Project.FormatVersion; }
        }

        /// <summary>
        /// The project ID, which is GUID.
        /// </summary>
        public string ID
        {
            get { return Project.ID; }
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name
        {
            get { return Project.Name; }
            set { Project.Name = value; }
        }

        /// <summary>
        /// Value that indicates whether the project is on offline mode.
        /// </summary>
        public bool OfflineMode
        {
            get { return Project.OfflineMode; }
            set { Project.OfflineMode = value; }
        }

        /// <summary>
        /// Collection of the package items for the project. This property is only for internal use within this assembly.
        /// </summary>
        internal PackageItems PackageItems_m
        {
            get { return Project.PackageItems; }
        }

        /// <summary>
        /// Collection of the packages for the project.
        /// </summary>
        public List<ISPackage> Packages
        {
            get
            {
                List<ISPackage> packageItems = new List<ISPackage>();
                foreach(PackageItem pi in PackageItems_m)
                {
                    packageItems.Add(new ISPackage(pi.Package));
                }
                return packageItems;
            }
        }

        /// <summary>
        /// Collection of the project parameters. This property is only for internal use within this assembly.
        /// </summary>
        internal Parameters Parameters_m
        {
            get { return Project.Parameters; }
        }

        /// <summary>
        /// Collection of the parameters for the project.
        /// </summary>
        public List<ISParameter> Parameters
        {
            get
            {
                List<ISParameter> parameters = new List<ISParameter>();
                foreach (Parameter param in Parameters_m)
                {
                    parameters.Add(new ISParameter(param));
                }
                return parameters;
            }
        }

        /// <summary>
        /// The password used to encrypt or decrypt project and packages.
        /// </summary>
        public string Password
        {
            set { Project.Password = value; }
        }

        /// <summary>
        /// level of protection on the package.
        /// </summary>
        public ProtectionLevel ProtectionLevel
        {
            get { return DtsUtility.EnumAToEnumB<DTSProtectionLevel, ProtectionLevel>(Project.ProtectionLevel); }
            set { Project.ProtectionLevel = DtsUtility.EnumAToEnumB<ProtectionLevel, DTSProtectionLevel>(value); }
        }

        /// <summary>
        /// level of protection on the package.
        /// </summary>
        public TargetServerVersion TargetServerVersion
        {
            get { return DtsUtility.EnumAToEnumB<DTSTargetServerVersion, TargetServerVersion>(Project.TargetServerVersion); }
            set { Project.TargetServerVersion = DtsUtility.EnumAToEnumB<TargetServerVersion, DTSTargetServerVersion>(value); }
        }

        #region VersionBuild

        /// <summary>
        /// Build version of the package.
        /// </summary>
        public int VersionBuild
        {
            get { return Project.VersionBuild; }
            set { Project.VersionBuild = value; }
        }

        #endregion

        #region VersionComments

        /// <summary>
        /// Version comments associated with the package.
        /// </summary>
        public string VersionComments
        {
            get { return Project.VersionComments; }
            set { Project.VersionComments = value; }
        }

        #endregion

        #region VersionMajor

        /// <summary>
        /// Major build version of the package.
        /// </summary>
        public int VersionMajor
        {
            get { return Project.VersionMajor; }
            set { Project.VersionMajor = value; }
        }

        #endregion

        #region VersionMinor

        /// <summary>
        /// Minor build version of the package.
        /// </summary>
        public int VersionMinor
        {
            get { return Project.VersionMinor; }
            set { Project.VersionMinor = value; }
        }

        #endregion

        #endregion

        #region General Properties and Fields

        public string IspacFile { get; set; }
        private string _filePath;
        private string _fileName;
        private bool _ispacAlreadyExists;

        #endregion

        #region Methods

        #region Save Project

        public void SaveToDisk()
        {
            if (_ispacAlreadyExists)
                Project.SaveTo(_filePath + @"\" + "zz_" + _fileName + "__Modified" + ".ispac");
            else
                Project.SaveTo(IspacFile);

            Project.Dispose();
        }

        #endregion

        #region Infer Ispac file name and path from IspacFile

        private void InferFileNameAndPath()
        {
            _filePath = Path.GetDirectoryName(IspacFile);
            _fileName = Path.GetFileNameWithoutExtension(IspacFile);
        }

        #endregion

        #region Add a package to project

        internal ISPackage AddPackage(string packageName, bool reusePackageIfExists = true)
        {
            return new ISPackage(packageName, this, reusePackageIfExists);
        }

        public void AddPackage(ISPackage package, bool reusePackageIfExists = true)
        {
            package.AddToProject(this, reusePackageIfExists);
        }

        #endregion

        #region Add a parameter to project

        public ISParameter AddParameter(string parameterName, System.TypeCode typeCode = TypeCode.Empty)
        {
            return new ISParameter(this, parameterName, typeCode);
        }

        #endregion

        #endregion
    }
}
