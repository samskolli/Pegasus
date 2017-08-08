using Pegasus.DtsWrapper;
using System.IO;

namespace Pegasus.Demo
{
    public class AdoNetSourceAndDestination
    {
        private string _ispacFileName = "TestProject";

        public void DFTWithAdoNetSourceAndDestination()
        {
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject mainProject = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  create a package
            ISPackage package = new ISPackage("ExamplePackage", mainProject);

            //  Create a DataFlow Task
            ISDataFlowTask dft = new ISDataFlowTask("ExampleDataFlow", package);
            dft.ParentPackage = package;
            dft.ParentProject = mainProject;

            //  Create a ADO.net connection manager
            ISAdoNetConnectionManager adoNetConn = new ISAdoNetConnectionManager(@"Application Name=mySsisApp;", "ADONetConnection", mainProject);
            adoNetConn.InitialCatalog = "PegasusDemo";
            adoNetConn.ServerName = "192.168.1.106";
            adoNetConn.UserName = "my_login";
            adoNetConn.Password = "password123";

            // In the Data Flow, create a ADO.net Source
            ISAdoNetSourceComponent adoNetSrc = new ISAdoNetSourceComponent(dft, "AdoNetSource", adoNetConn);
            adoNetSrc.TableOrViewName = "\"dbo\"" + "." + "\"customer\"";

            ISAdoNetDestinationComponent dest = new ISAdoNetDestinationComponent(dft, "AdoNetDestination", adoNetSrc, adoNetSrc.GetOutputNameFromIndex(0));
            dest.Connection = adoNetConn.Name;
            dest.TableOrViewName = "\"dbo\"" + "." + "\"customer\"";

            mainProject.SaveToDisk();

        }
    }
}
