using Pegasus.DtsWrapper;
using System.IO;
using System.Collections.Generic;

namespace Pegasus.Demo
{

    public struct SourceTargetTableMap
    {
        public string SourceSchema { get; set; }
        public string SourceTable { get; set; }
        public string TargetSchema { get; set; }
        public string TargetTable { get; set; }
    }

    public class AdoNetSourceAndDestination
    {
        private string _ispacFileName = "TestProject";

        /// <summary>
        /// This example shows how to populate a ado net destination table from a ado net source; also, if there is a collection, it can be looped over.
        /// </summary>
        public void DFTWithAdoNetSourceAndDestination()
        {
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject mainProject = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  create a package
            ISPackage package = new ISPackage("ExamplePackage", mainProject);
            
            //  Create a Source ADO.net connection manager
            ISAdoNetConnectionManager srcAdoNetConn = new ISAdoNetConnectionManager(@"Application Name=mySsisApp;", "SourceADONetConnection", mainProject);
            srcAdoNetConn.InitialCatalog = "PegasusDemo";
            srcAdoNetConn.ServerName = "192.168.1.106";
            srcAdoNetConn.UserName = "my_login";
            srcAdoNetConn.Password = "password123";

            //  Create a Target ADO.net connection manager
            ISAdoNetConnectionManager trgAdoNetConn = new ISAdoNetConnectionManager(@"Application Name=mySsisApp;", "TargetADONetConnection", mainProject);
            trgAdoNetConn.InitialCatalog = "PegasusDemo";
            trgAdoNetConn.ServerName = "192.168.1.106";
            trgAdoNetConn.UserName = "my_login";
            trgAdoNetConn.Password = "password123";

            List<SourceTargetTableMap> srcTrgMap = new List<SourceTargetTableMap>();
            srcTrgMap.Add(
                new SourceTargetTableMap { SourceSchema = "dbo", SourceTable = "customer", TargetSchema = "dbo", TargetTable = "customer" }
                );
            srcTrgMap.Add(
                new SourceTargetTableMap { SourceSchema = "dbo", SourceTable = "shipment", TargetSchema = "dbo", TargetTable = "shipment" }
                );

            ISDataFlowTask prevDft = (ISDataFlowTask)null;
            foreach(SourceTargetTableMap st in srcTrgMap)
            {
                //  Create a DataFlow Task
                ISDataFlowTask dft = new ISDataFlowTask(st.SourceTable + " Data Flow", package);
                dft.ParentPackage = package;
                dft.ParentProject = mainProject;

                // In the Data Flow, create a ADO.net Source
                ISAdoNetSourceComponent adoNetSrc = new ISAdoNetSourceComponent(dft, "AdoNetSource", srcAdoNetConn);
                adoNetSrc.TableOrViewName = "\"" + st.SourceSchema + "\"" + "." + "\"" + st.SourceTable + "\"";

                ISAdoNetDestinationComponent dest = new ISAdoNetDestinationComponent(dft, "AdoNetDestination", adoNetSrc, adoNetSrc.GetOutputNameFromIndex(0));
                dest.Connection = trgAdoNetConn.Name;
                dest.TableOrViewName = "\"" + st.TargetSchema + "\"" + "." + "\"" + st.TargetTable + "\"";

                if (prevDft == null)
                {
                    // NO precedence assingment to make
                }
                else
                {
                    ISPrecedenceConstraint pc = new ISPrecedenceConstraint(package, prevDft, dft, PrecedenceEvalOp.Constraint, ExecResult.Success);
                }
                prevDft = dft;
            }

            

            mainProject.SaveToDisk();

        }
    }
}
