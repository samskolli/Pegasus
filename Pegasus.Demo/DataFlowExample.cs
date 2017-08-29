using System.IO;
using System.Data;
using System.Collections.Generic;
using Pegasus.DtsWrapper;

namespace Pegasus.Demo
{

    public struct SqlServerTable
    {
        public string Schema { get; set; }
        public string Table { get; set; }
    }

    public class DataFlowExample
    {

        private string _ispacFileName = "TestProject";
        private string _sourceFolder;
        private string[] _fileCollection;

        public DataFlowExample()
        {
            _sourceFolder = @"C:\Temp\Pegasus_SSISGenerator\SampleData"; // Copy the ExampleFiles to the path specified here
            ////////_sourceFolder = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName + @"\ExampleFiles";
        }

        public void GenerateProjectToLoadTextFilesToSqlServerDatabase()
        {
            // Showcases how other C# code and libraries can be used to generate packages

            /* The objective is as follows:
             *  For all the txt files in a folder
             *      1. Load each text file into a table
             *      2. Before loading trim all string values
             *  
             *  We will use the lumenworks.framework.io library to parse a text file and help infer data types
             *  
             *  The package design needs to be as follows:
             *      1. A package for each file in the folder
             *      2. A master package that executes each of the above packages thru a ExecutePackage Task
             *      3. In the master package, put all Execute Package Tasks inside a Sequence container, in serial.
             */

            //  Get files
            _fileCollection = Directory.GetFiles(_sourceFolder);

            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject mainProject = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  Add a project connection to the sql server we are loading into
            ISOledbConnectionManager oleConn = new ISOledbConnectionManager(@"Provider=SQLNCLI11.1;Auto Translate=False;", "TargetDB", mainProject);
            oleConn.ServerName = "localhost";
            oleConn.InitialCatalog = "PegasusDemo";
            oleConn.UserName = "my_login";
            oleConn.Password = "password123";

            //  Add a Package
            ISPackage parentPackage = new ISPackage("ParentPackage", mainProject);

            //  Add a sequence container; this container will contain the individual data flows for each text file
            ISSequence childPackageContainer = new ISSequence("Child Packages", parentPackage);

            //  Iterate thru our folder and do the following for each file
            foreach (string file in _fileCollection)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                //  use the lumenworks library to get the column names, lenghts, data types etc. This metadata is later used to configure the ssis project
                FileHelper fh = new FileHelper(file);
                string[] columnNames = fh._columnNames;
                int[] columnLengths = fh.InferFlatFileColumnLengths(10);
                string[] columnDataTypes = fh.InferFlatColumnSSISDataType();
                string[] tableDataTypes = fh.GetSqlServerDataTypes();

                // A child package for each file; added to the main project
                ISPackage packageForFile = new ISPackage(fileName, mainProject);

                // A execute package task in the master package for each file
                ISExecutePackageTask ept = new ISExecutePackageTask("Exec " + fileName, childPackageContainer); 
                ept.UseProjectReference = true;
                ept.PackageName = packageForFile.Name; // this execute package task will call the child package

                // A connection manager for each file; added to the main project
                ISFlatFileConnectionManager fConn = new ISFlatFileConnectionManager(file, fileName, mainProject); 
                fConn.ColumnNamesInFirstDataRow = true;
                //fConn.TextQualifier = "\"";
                fConn.Format = "Delimited";
                fConn.RowDelimiter = "\r\n"; // check for LF/CRLF if using git
                
                //  create a FlatFile column for each column the in the source file
                for (int i = 0; i < columnNames.Length; i++)
                {
                    ISFlatFileColumn fc = new ISFlatFileColumn(fConn, columnNames[i], ((i == columnNames.Length - 1) ? true : false));
                    fc.SetColumnProperties(DtsUtility.StringToEnum<SSISDataType>(columnDataTypes[i]), "Delimited", ",", 0, columnLengths[i], 0, 0);
                }

                //  Add a execute sql task which will create the table in the destiantion server. The file helper class gives the create statement to use
                ISExecuteSqlTask createTable = new ISExecuteSqlTask("Create Target Table", packageForFile);
                createTable.Connection = oleConn.Name;
                createTable.SqlStatementSourceType = SqlStatementSourceType.DirectInput;
                createTable.SqlStatementSource = fh.GetCreateStatement();

                //  Add a data flow for each file
                ISDataFlowTask dft = new ISDataFlowTask("Load Data From " + fileName, packageForFile);
                dft.DelayValidation = true;
                dft.ParentPackage = packageForFile;
                dft.ParentProject = mainProject;

                //  Add a precedence constraint that executes the data flow after the create table sql task is a success
                ISPrecedenceConstraint pc1 = new ISPrecedenceConstraint(packageForFile, createTable, dft, PrecedenceEvalOp.Constraint, ExecResult.Success);

                //  //  //  Now configure the data flow

                //  Add a flat file source
                ISFlatFileSourceComponent sourceComp = new ISFlatFileSourceComponent(dft, fileName, fConn);

                //  Add a derive column component that trims a column (in place) if it is of String data type
                ISDerivedColumnComponent dCom = new ISDerivedColumnComponent(dft, "Trim Columns", sourceComp);
                foreach (ISFlatFileColumn column in fConn.Columns)
                {
                    if (column.DataType == SSISDataType.DT_STR)
                    {
                        ISDerivedColumn dCol = new ISDerivedColumn(dCom, DerivedColumnAction.Replace, column.Name);
                        dCol.Expression = "TRIM(" + column.Name + ")";
                    }
                }
                
                //  Add a destination table in the target sql server.
                ISOleDbDestinationComponent destination = new ISOleDbDestinationComponent(dft, "Target Table", dCom, dCom.GetOutputNameFromIndex(0));
                destination.Connection = oleConn.Name;
                destination.OpenRowset = "dbo." + fileName;

                //  Because, the table may not be available during package generation time, we will manually map the destination columns
                // To do that, we will create one ExternalMetadataColumn for each column the source file.
                // Becuase we are creating the target table with the same column names, we will directly map on the name
                List<ExternalColumnInputMap> externalInputMap = new List<ExternalColumnInputMap>();
                for (int i = 0; i < columnNames.Length; i++)
                {
                    SSISDataTypeWithProperty sdt = Converter.TranslateSqlServerDataTypeToSSISDataTypeWithProperty(tableDataTypes[i], columnLengths[i].ToString());
                    ExternalMetadataColumn externalColumn = new ExternalMetadataColumn();
                    externalColumn.ExternalColumnName = columnNames[i];
                    externalColumn.DataType = sdt.DataType;
                    externalColumn.Length = sdt.Length;
                    externalColumn.Precision = sdt.Precision;
                    externalColumn.Scale = sdt.Scale;
                    externalColumn.CodePage = sdt.CodePage;
                    externalInputMap.Add(new ExternalColumnInputMap { ExternalColumn = externalColumn, InputColumnName = columnNames[i] }); // // Becuase we are creating the target table with the same column names, we will directly map on the name
                }
                destination.ExternalColumnInputColumnMap = externalInputMap;

                // Now perform the manual mapping. Otherwise, SSIS will complain that atleast one column should be mapped.
                destination.ManualMapToTargetColumns();
            }

            mainProject.SaveToDisk();
        }

        public void GenerateProjectToLoadTextFilesFromSqlServerDatabase()
        {
            // Showcases how other C# code and libraries can be used to generate packages

            /* The objective is as follows:
             *  For a given collection of tables
             *      1. Load each table into a text file
             *  
             *  We will use SQL Server system tables to get the column metadata
             *  
             *  The package design needs to be as follows:
             *      1. A package for each file in the folder
             *      2. A master package that executes each of the above packages thru a ExecutePackage Task
             *      3. In the master package, put all Execute Package Tasks inside a Sequence container, in serial.
             */

            //  Get List of Tables
            List<SqlServerTable> sqlServerTables = new List<SqlServerTable>();
            sqlServerTables.Add(new SqlServerTable { Schema = "dbo", Table = "customer" });
            sqlServerTables.Add(new SqlServerTable { Schema = "dbo", Table = "geo_location" });

            //  Where should the final files be stored?
            string destinationFolder = @"C:\Temp_new\Pegasus_SSISGenerator";

            string baseMetadataQuery = @"select c.name as column_name, c.column_id as column_position,
		ty.name as data_type, 
		case 
			when ty.name in ('decimal', 'numeric') then '(' + convert(varchar(10), c.precision) + ', ' + convert(varchar(10), c.scale) + ')' 
			when ty.name in ('varchar', 'char') then '(' + case c.max_length when -1 then 'max' else convert(varchar(10), c.max_length) end + ')'
			when ty.name in ('nvarchar', 'nchar') then '(' + case c.max_length when -1 then 'max' else convert(varchar(10), c.max_length/2) end + ')'
			else ''
		end as data_type_length
	from sys.objects o
		inner join sys.columns c on o.object_id = c.object_id
		inner join sys.types ty on c.user_type_id = ty.user_type_id
	where o.type ='U'";

            /***********************    Start SSIS Related Instructions ****************************************/

            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject mainProject = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  Add a project connection to the sql server we are loading into
            ISOledbConnectionManager oleConn = new ISOledbConnectionManager(@"Provider=SQLNCLI11.1;Auto Translate=False;", "SourceDB", mainProject);
            oleConn.ServerName = "192.168.1.107";
            oleConn.InitialCatalog = "PegasusDemo";
            oleConn.UserName = "my_login";
            oleConn.Password = "password123";

            //  Add a Package
            ISPackage parentPackage = new ISPackage("ParentPackage", mainProject);

            //  Add a sequence container; this container will contain the individual data flows for each text file
            ISSequence childPackageContainer = new ISSequence("Child Packages", parentPackage);
            

            //  Iterate thru our table collection and do the following for each table
            foreach (SqlServerTable table in sqlServerTables)
            {
                System.Console.WriteLine("\nWorking on table:: " + table.Schema + "." + table.Table);


                System.Console.WriteLine("\tGetting metadata");
                //  using the oledb conn, get the metadata; the example assumes you can connect to the target database from this application
                SqlServerHelper sh = new SqlServerHelper(oleConn.ServerName, oleConn.InitialCatalog, oleConn.UserName, "password123");
                // most of the time you dont want to constrcut the sql statement like belwo to account for best practices
                DataSet ds = sh.GetDataSet(baseMetadataQuery + " and o.name = '" + table.Table + "' and schema_name(o.schema_id) = '" + table.Schema + "'");
                DataTable dt = ds.Tables[0];
                string[] columnNames = new string[dt.Rows.Count];
                SSISDataTypeWithProperty[] columnDataTypes = new SSISDataTypeWithProperty[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    columnNames.SetValue(dt.Rows[i]["column_name"].ToString(), i);
                    columnDataTypes.SetValue(Converter.TranslateSqlServerDataTypeToSSISDataTypeWithProperty(dt.Rows[i]["data_type"].ToString(), dt.Rows[i]["data_type_length"].ToString()), i);
                }

                // A child package for each file; added to the main project
                System.Console.WriteLine("\tCreating child package");
                ISPackage packageForFile = new ISPackage(table.Schema + "_" + table.Table, mainProject);

                // A execute package task in the master package for each table
                System.Console.WriteLine("\tCreating exec pkg task to call child package");
                ISExecutePackageTask ept = new ISExecutePackageTask("Exec " + table.Schema + "_" + table.Table, childPackageContainer);
                ept.UseProjectReference = true;
                ept.PackageName = packageForFile.Name; // this execute package task will call the child package

                // A connection manager for each file; added to the main project
                System.Console.WriteLine("\tCreating Flat File Conn Mgr");
                ISFlatFileConnectionManager flatFileConn = new ISFlatFileConnectionManager(destinationFolder + @"\" + table.Schema + "__" + table.Table + ".txt", table.Schema + "__" + table.Table + "_Conn", mainProject);
                flatFileConn.ColumnNamesInFirstDataRow = true;
                //fConn.TextQualifier = "\"";
                flatFileConn.Format = "Delimited";
                flatFileConn.RowDelimiter = "\r\n"; // check for LF/CRLF if using git

                //  create a FlatFile column for each column the in the source file
                for (int i = 0; i < columnNames.Length; i++)
                {
                    ISFlatFileColumn fc = new ISFlatFileColumn(flatFileConn, columnNames[i], ((i == columnNames.Length - 1) ? true : false));
                    fc.SetColumnProperties(columnDataTypes[i].DataType, "Delimited", ",", 0, columnDataTypes[i].Length, 0, 0);
                }

                //  Add a data flow for each file
                System.Console.WriteLine("\tData Flow in ach chld pkg ");
                ISDataFlowTask dft = new ISDataFlowTask("Load Data From " + table.Schema + "__" + table.Table, packageForFile);
                dft.DelayValidation = true;
                dft.ParentPackage = packageForFile;
                dft.ParentProject = mainProject;

                //  //  //  Now configure the data flow

                //  Add a flat file source
                System.Console.WriteLine("\t\tCreating Oledb Src Component");
                ISOleDbSourceComponent sourceComp = new ISOleDbSourceComponent(dft, "Source  Table", oleConn);
                string columns = string.Join(", ", columnNames);
                sourceComp.SqlCommand = "select " + columns + " from " + table.Schema + "." + table.Table;


                //  Add a destination
                System.Console.WriteLine("\t\tCreating Flat File Dest Component");
                ISFlatFileDestination ffDest = new ISFlatFileDestination(dft, "Destination", sourceComp, sourceComp.GetOutputNameFromIndex(0));
                ffDest.Overwrite = true;
                ffDest.Connection = flatFileConn.Name;

                // Now we will manually map the destination columns
                // To do that, we will create one ExternalMetadataColumn for each column in the source table.
                // Becuase we are creating the destination text file with the same column names, we will directly map on the name
                List<ExternalColumnInputMap> externalInputMap = new List<ExternalColumnInputMap>();
                for (int i = 0; i < columnNames.Length; i++)
                {
                    SSISDataTypeWithProperty sdt = columnDataTypes[i];
                    ExternalMetadataColumn externalColumn = new ExternalMetadataColumn();
                    externalColumn.ExternalColumnName = columnNames[i];
                    externalColumn.DataType = sdt.DataType;
                    externalColumn.Length = sdt.Length;
                    externalColumn.Precision = sdt.Precision;
                    externalColumn.Scale = sdt.Scale;
                    externalColumn.CodePage = sdt.CodePage;
                    externalInputMap.Add(new ExternalColumnInputMap { ExternalColumn = externalColumn, InputColumnName = columnNames[i] }); // // Becuase we are creating the target table with the same column names, we will directly map on the name
                }
                ffDest.ExternalColumnInputColumnMap = externalInputMap;

                // Now perform the manual mapping. Otherwise, SSIS will complain that atleast one column should be mapped.
                ffDest.ManualMapToTargetColumns();
            }

            //  Finally, save the project ispac to disk
            System.Console.WriteLine("\n\nSaving ispac to disk");
            mainProject.SaveToDisk();
        }
    }
}
