using System.IO;
using System;
using System.Collections.Generic;
using Pegasus.DtsWrapper;

namespace Pegasus.Demo
{

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
    }
}
