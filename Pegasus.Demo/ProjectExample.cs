using Pegasus.DtsWrapper;
using System.IO;

namespace Pegasus.Demo
{
    public class ProjectExample
    {
        private string _ispacFileName = "TestProject";

        #region ctor

        public ProjectExample()
        {
            //CreateAndModifyProject();
            //CreateProjectAndAddPackage();
            //CreateProjectAndAddParameter();
        }

        #endregion

        #region Examples

        #region Create a Project

        public void CreateAndModifyProjectWithProtectionLevel()
        {
            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject project = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  change some project properties
            project.Name = "Test";
            project.Description = "This project is created from Pegasus.DtsWrapper code";
            project.TargetServerVersion = TargetServerVersion.SQLServer2016;
            project.ProtectionLevel = ProtectionLevel.EncryptSensitiveWithPassword;
            project.Password = "mypassword";

            project.VersionBuild = 1;
            project.VersionComments = "The first version of the " + project.Name + " project";
            project.VersionMajor = 1;
            project.VersionMinor = 0;

            //  save the project...otherwise modifications wont be persisted to the disk
            project.SaveToDisk();

            System.Console.WriteLine(project.VersionComments);


        }

        #endregion

        #region Create a Project and Add a Package

        public void CreateProjectAndAddPackage()
        {
            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject project = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  change some project properties
            project.Name = "Test";
            project.Description = "This project is created from Pegasus.DtsWrapper code";

            //  create a package using the ISPackage's constructor that uses the ISProject parameter
            ISPackage packageA = new ISPackage("Package_A", project);
            packageA.Description = "My Package A";
            packageA.CreatorName = "Sam the Man";

            //  create a package using the ISPackage's constructor that does NOT use ISProject parameter. 
            //  Later call the AddToProject method on the package to add it to a project.
            ISPackage packageB = new ISPackage("Package_B");
            packageB.CreatorName = "SamIAm";
            packageB.AddToProject(project); // Note: if the reuse flag is true, then any changes made to pacakge variable before this call are replaced with the found package's properties (if a package with the same name exists)
            packageB.CreatorComputerName = "SomeOtherMachine";
            packageB.Description = "My Package B";

            ISPackage packageC = new ISPackage("Package_C");
            packageC.Description = "Hi I am Package C";
            project.AddPackage(packageC, false);

            //  save the project
            project.SaveToDisk();
        }

        #endregion

        #region Create project and Add a parameter

        public void CreateProjectAndAddParameter()
        {
            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject project = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  changesome project properties
            project.Name = "Test";
            project.Description = "This project is created from Pegasus.DtsWrapper code";

            // Add a project parameter using the ISParameter constructor
            ISParameter paramA = new ISParameter(project, "AParam", System.TypeCode.Boolean);
            paramA.Required = false;
            paramA.Sensitive = false;

            // Add more project parameters using the ISProject's AddParameter instance method
            project.AddParameter("BParam", System.TypeCode.String);

            ISParameter paramC = project.AddParameter("CParam", System.TypeCode.Int32);
            paramC.Description = "This is parameter c";

            //  save the project
            project.SaveToDisk();
        }

        #endregion

        #endregion
    }
}
