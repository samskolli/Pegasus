using System;
using Pegasus.DtsWrapper;

namespace Pegasus.Demo
{
    // Only tested on Execute SQL Tasks in Control Flow Tasks
    // DOES NOT REPLACE connections used in Script Task code.Can code for it as needed
    public class ConnReplacer
    {
        public ConnReplacer()
        {

        }

        public void ReplaceConnection()
        {
            ISProject project = new ISProject(@"C:\Temp\Integration Services Project14.ispac", null);

            ISOledbConnectionManager prjOleConn = new ISOledbConnectionManager(@"Data Source=localhost;Initial Catalog=PegasusDemo;Provider=SQLNCLI11.1;Integrated Security=SSPI;Auto Translate=False;", "PrjConn", project, null);
            //prjOleConn.SetExpression("ServerName", "@[$Project::HostSqlServer]");
            //prjOleConn.SetExpression("InitialCatalog", "@[$Project::HostSqlServerCatalog]");

            foreach (ISPackage package in project.Packages)
            {
                ISExecuteSqlTask sqlTask = new ISExecuteSqlTask("Execute SQL Task", package);
                sqlTask.Connection = prjOleConn.Name;
                Console.WriteLine("SQL Task Conn: " + sqlTask.Connection);
                String connToReplace = "dbconn";
                String targetConnectionManagerId = "";
                foreach (ISConnectionManager connection in package.getConnections())
                {
                    Console.WriteLine(connection.Name + " -- " + connection.ID);
                    if (connection.Name == connToReplace)
                        targetConnectionManagerId = connection.ID;
                }

                Console.WriteLine("*******************");
                ISDataFlowTask dft = new ISDataFlowTask("Data Flow Task", package);
                
                foreach (ISPipelineComponent pc in dft.ComponentMetaDataCollection)
                {
                    Console.WriteLine(pc.Name + " -- " + pc.ComponentClassID);
                    //pc.updateRequiredConnection(targetConnectionManagerId, prjOleConn);
                    foreach (RuntimeConnection conn in pc.getListOfConnections())
                    {
                        Console.WriteLine("\t" + conn);
                        if(targetConnectionManagerId == conn.ConnectionManagerID)
                        {
                            pc.SetUpConnection(prjOleConn, conn.Index);
                            pc.RetrieveMetaData();
                            //Console.WriteLine("\t\t" + conn);
                        }
                    }
                }
            }

            project.SaveToDisk();
        }
    }
}
