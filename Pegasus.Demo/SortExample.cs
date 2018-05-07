using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Pegasus.DtsWrapper;

namespace Pegasus.Demo
{
    
    public class SortExample
    {
        /*
         EXAMPLE FILE TO USE; File should be saved at the location of _sourceFile var.

col_a,col_b,col_c
a,b,c
g,h,i
d,e,f
d,e,f
a,c,b
         * */
        private string _ispacFileName = "TestProject";
        private string _sourceFile;

        public SortExample()
        {
            _sourceFile = @"C:\Temp\Pegasus_SSISGenerator\SampleData\SortInput.txt"; // Copy the ExampleFiles to the path specified here
        }

        public void CreatePackageWithSort()
        {
            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            Console.WriteLine("Creating Project");
            ISProject mainProject = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            // Flat File Connection
            //  use the lumenworks library to get the column names, lenghts, data types etc. This metadata is later used to configure the ssis project
            FileHelper fh = new FileHelper(_sourceFile);
            string[] columnNames = fh._columnNames;
            int[] columnLengths = fh.InferFlatFileColumnLengths(10);
            string[] columnDataTypes = fh.InferFlatColumnSSISDataType();
            string[] tableDataTypes = fh.GetSqlServerDataTypes();

            Console.WriteLine("Creating flatfile conn");

            ISOledbConnectionManager oleConn = new ISOledbConnectionManager(@"Data Source=localhost;Initial Catalog=PegasusDemo;Provider=SQLNCLI11.1;Integrated Security=SSPI;Auto Translate=False;", "SourceDB", mainProject);
            
            ISFlatFileConnectionManager fConn = new ISFlatFileConnectionManager(_sourceFile, "Flat_File", mainProject);
            fConn.ColumnNamesInFirstDataRow = true;
            //fConn.TextQualifier = "\"";
            fConn.Format = "Delimited";
            fConn.RowDelimiter = "\r\n"; // check for LF/CRLF if using git
            string columnDelimiter = ",";
            
            //  create a FlatFile column for each column the in the source file
            for (int i = 0; i < columnNames.Length; i++)
            {
                ISFlatFileColumn fc = new ISFlatFileColumn(fConn, columnNames[i], ((i == columnNames.Length - 1) ? true : false));
                fc.SetColumnProperties(DtsUtility.StringToEnum<SSISDataType>(columnDataTypes[i]), "Delimited", columnDelimiter, 0, columnLengths[i], 0, 0);
            }

            //  Add a Package
            Console.WriteLine("Creating package");
            ISPackage parentPackage = new ISPackage("ParentPackage", mainProject);

            // Add a data flow
            Console.WriteLine("Creating dft");
            ISDataFlowTask dft = new ISDataFlowTask("SortTest", parentPackage);
            dft.DelayValidation = true;
            dft.ParentPackage = parentPackage;
            dft.ParentProject = mainProject;

            // Configure the data flow
            //  Add a flat file source
            Console.WriteLine("Creating source compo");
            ISFlatFileSourceComponent sourceComponent = new ISFlatFileSourceComponent(dft, "SortInput", fConn);


            // Sort Component
            Console.WriteLine("Creating sort compo");
            ISSortComponent sortComponent = new ISSortComponent(dft, "Sorter", sourceComponent);
            sortComponent.EliminateDuplicates = true; // Should the componet remove duplicates or not?
            sortComponent.MaximumThreads = -1; // How many Threads should it use
            // The next two lines shows sorting the data by col_a first and then by col_c
            sortComponent.SetColumnSortInformation(columnName: "col_a", sortKeyPosition: 1, stringComparisonFlag: StringComparisonFlag.IGNORE_CASE);
            sortComponent.SetColumnSortInformation("col_c", 2, StringComparisonFlag.IGNORE_CASE);


            // Hook it to a Row count Component for testing
            Console.WriteLine("Creating a rowcount component");
            ISVariable rcVar = new ISVariable("RCount", false, "User", 0, null, dft, VariableDataType.Int32);
            ISRowCountComponent rowComponent = new ISRowCountComponent(dft, "RC", sortComponent);
            rowComponent.AssignVariable(rcVar);

            Console.WriteLine("saving package to disk at " + Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");
            mainProject.SaveToDisk();
        }
    }
}
