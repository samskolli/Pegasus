using Pegasus.DtsWrapper;
using System.IO;

namespace Pegasus.Demo
{
    public class ControlFlowTaskExample
    {

        private string _ispacFileName = "TestProject";

        #region Add a Execute SQL Task

        #region Create a Project and Add a Package

        public void AddTasksAndAssignPrecedenceConstraints()
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
            packageA.CreatorName = "Me";

            ISPackage packageB = new ISPackage("Package_B", project);
            packageB.Description = "My Package B";
            packageB.CreatorName = "Me again";

            //  Add a container to Package_A
            ISSequence mySqlTaskContainer = new ISSequence("My_Container", packageA);

            //  Add a Project connection for the Sql Tasks
            ISOledbConnectionManager oleConn = new ISOledbConnectionManager("Provider=SQLNCLI11.1;Persist Security Info=True;Auto Translate=False;", "my_database_server", project, null);
            oleConn.ServerName = "localhost";
            oleConn.UserName = "my_user_name";
            oleConn.Password = "my_password.";
            oleConn.InitialCatalog = "my_database";


            //  Add a execute sql task to the above sequence container
            ISExecuteSqlTask sqlTask = new ISExecuteSqlTask("Some Sql Task", mySqlTaskContainer);
            sqlTask.SqlStatementSourceType = SqlStatementSourceType.DirectInput;
            sqlTask.SqlStatementSource = "insert into test_table (id) values (10), (20)";
            sqlTask.Connection = oleConn.Name;

            //  Add another execute sql task to the above sequence container
            ISExecuteSqlTask anotherSqlTask = new ISExecuteSqlTask("Another Sql Task", mySqlTaskContainer);
            anotherSqlTask.SqlStatementSourceType = SqlStatementSourceType.DirectInput;
            anotherSqlTask.SqlStatementSource = "insert into test_table_two (id) values (10), (20)";
            anotherSqlTask.Connection = oleConn.Name;

            //  Add a precedence constraint that says execute the second sqltask task after hte successful completion of the first sql task
            ISPrecedenceConstraint pc1 = new ISPrecedenceConstraint(mySqlTaskContainer, sqlTask, anotherSqlTask, PrecedenceEvalOp.Constraint, ExecResult.Success);


            //  Add a Execute Package Task to the parent package (Package A)
            ISExecutePackageTask ept = new ISExecutePackageTask("Execute Package B", packageA);
            ept.PackageName = packageB.Name;
            ept.UseProjectReference = true;

            //  Add a precedence constraint that says execute the Package B after the sql tasks are completed
            ISPrecedenceConstraint pc2 = new ISPrecedenceConstraint(packageA, mySqlTaskContainer, ept, PrecedenceEvalOp.Constraint, ExecResult.Completion);

            //  save the project
            project.SaveToDisk();
        }

        #endregion

        #endregion
    }
}
