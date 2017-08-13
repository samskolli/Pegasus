using System;
using System.IO;

namespace Pegasus.Demo
{
    class Program
    {
        
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("Start Time:  " + now.ToString() + "\n");

            CreateFolder(Constants.StorageFoldePath);

            RunDemoExample();

            Console.WriteLine("\n\nDone!!!" + "\n" + "*******************************");
            DateTime endTime = DateTime.Now;
            Console.WriteLine("End Time:  " + endTime.ToString());
            Console.WriteLine("Duration: " + (DateTime.Now - now).Minutes.ToString() + " mins");
            Console.WriteLine("Press any key to exit....");
            Console.ReadKey();
        }

        private static void RunDemoExample()
        {
            //  //  //  Project Examples
            //ProjectExample pe = new ProjectExample();

            //  Create and Modify a Project
            //pe.CreateAndModifyProjectWithProtectionLevel();

            //  Create a project and add packages to it
            //pe.CreateProjectAndAddPackage();

            //  Create a project and add Parameter
            //pe.CreateProjectAndAddParameter();



            //  //  //  Control Flow Example
            //ControlFlowTaskExample ce = new ControlFlowTaskExample();

            //  Add some tasks; assign some properties to those tasks; and assign precedence constraints to those tasks.
            //ce.AddTasksAndAssignPrecedenceConstraints();


            //  //  //  Data Flow Examples
            //DataFlowExample de = new DataFlowExample();

            //  Load all text files in a folder to a database
            //de.GenerateProjectToLoadTextFilesToSqlServerDatabase();



            //  //  //  AdoNet Source and Destination
            //AdoNetSourceAndDestination ado = new AdoNetSourceAndDestination();
            //ado.DFTWithAdoNetSourceAndDestination();



            //  //  //  Excel Example
            ExcelExample ee = new ExcelExample();
            ee.ExcelToSqlServerTable();
        }

        private static void CreateFolder(string folderPath)
        {
            if (!(Directory.Exists(folderPath)))
                Directory.CreateDirectory(folderPath);
        }
    }
}
