using Pegasus.DtsWrapper;
using System.IO;
using System.Collections.Generic;

namespace Pegasus.Demo
{

    public struct SourceColumn
    {
        public string Name { get; set; }
    }

    public struct TargetColumn
    {
        public string Name { get; set; }
        public SSISDataType DataType { get; set; }
        public int CodePage { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
        public int Precision { get; set; }
    }

    public struct SourceColumnTargetColumnMap
    {
        public SourceColumn SourceColumn { get; set; }
        public TargetColumn TargetColumn { get; set; }
    }

    public class ExcelExample
    {
        private string _ispacFileName = "TestProject";

        public void ExcelToSqlServerTable()
        {
            // In this example,we are loading an excel spresheet to a sql server table.
            // The excel sheet has three columns named col_a(unicode string 255),col_b (unicode_string 255) and col_c(double precision float)
            // The target table has three columns named my_first_col (nvarchar(255)), my_second_col (nvarchar(255)), my_third_col(float(53))
            // Load as follows: col_a -> my_first_col; col_b -> my_second_col; col_c -> my_third_col


            // <Mapping info that says which excel source column should go to which sql server table column
            List<SourceColumnTargetColumnMap> sourceTargetMappings = new List<SourceColumnTargetColumnMap>();
            // In this example, I am manullay populating the source to target map. 
            // Otherwise, this information can be gathered programmatically for both source and target from external config files/mapping tables etc.
            // For programmatic translation of other data types to SSIS data types, refer to the code in Converter class in the DtsWrapper library
            sourceTargetMappings.Add(new SourceColumnTargetColumnMap
            {
                SourceColumn = new SourceColumn { Name = "col_a" }, // excel source column
                TargetColumn = new TargetColumn { Name = "my_first_col", DataType = SSISDataType.DT_WSTR, Length = 255, CodePage = 0, Precision = 0, Scale = 0 } // target column into which source should go to
            });
            sourceTargetMappings.Add(new SourceColumnTargetColumnMap
            {
                SourceColumn = new SourceColumn { Name = "col_b" }, // excel source column
                TargetColumn = new TargetColumn { Name = "my_second_col", DataType = SSISDataType.DT_WSTR, Length = 255, CodePage = 0, Precision = 0, Scale = 0 } // target column into which source should go to
            });
            sourceTargetMappings.Add(new SourceColumnTargetColumnMap
            {
                SourceColumn = new SourceColumn { Name = "col_c" }, // excel source column
                TargetColumn = new TargetColumn { Name = "my_third_col", DataType = SSISDataType.DT_R8, Length = 0, CodePage = 0, Precision = 0, Scale = 0 } // target column into which source should go to
            });



            //  //  //  Package creation related code

            //  Delete the ispac if exists; otherwise the code will modify the existing ispac. For clarity in showcasing the demo we will delete the existitng ispac
            if (File.Exists(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac"))
                File.Delete(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac");

            //  create a project
            ISProject project = new ISProject(Constants.StorageFoldePath + @"\" + _ispacFileName + ".ispac", null);

            //  change some project properties
            project.Name = "Test";
            project.Description = "This project is created from Pegasus.DtsWrapper code";

            //  Excel Connection Manager
            string file = @"C:\Temp\abc.xls";
            ISExcelConnectionManager excelConn = new ISExcelConnectionManager("excelConn", project);
            excelConn.ExcelFilePath = file;
            excelConn.ExcelVersionNumber = ExcelVersion.DTSExcelVer_8;
            excelConn.FirstRowHasColumnName = true;

            //  Target SQL Server Connection Manager
            ISOledbConnectionManager oleConn = new ISOledbConnectionManager(@"Provider=SQLNCLI11.1;Auto Translate=False;", "TargetDB", project);
            oleConn.ServerName = "192.168.1.107";
            oleConn.InitialCatalog = "PegasusDemo";
            oleConn.UserName = "my_login";
            oleConn.Password = "password123";

            //  Create a package
            ISPackage packageA = new ISPackage("Package_A", project);
            packageA.Description = "Package demoing loading an excel file to a sql server table with mappings specified";
            packageA.CreatorName = "Me";

            //  Add a Data Flow
            ISDataFlowTask dft = new ISDataFlowTask("ExcelDFT", packageA);
            dft.DelayValidation = true;
            dft.ParentPackage = packageA;
            dft.ParentProject = project;


            //  //  //  Configure the data flow
            //  Add a excel source component
            ISExcelSourceComponent es = new ISExcelSourceComponent(dft, "ExSrc", excelConn);
            es.OpenRowset = "Sheet1$";

            //  Add a sql server destination
            ISOleDbDestinationComponent dest = new ISOleDbDestinationComponent(dft, "Target Table", es, es.GetOutputNameFromIndex(0));
            dest.Connection = oleConn.Name;
            dest.OpenRowset = "dbo.ExcelToSqlServer";

            for (int i = 0; i < sourceTargetMappings.Count; i++)
            {
                ExternalMetadataColumn externalColumn = new ExternalMetadataColumn();
                externalColumn.ExternalColumnName = sourceTargetMappings[i].TargetColumn.Name; // the name of the column in the target?
                externalColumn.DataType = sourceTargetMappings[i].TargetColumn.DataType;
                externalColumn.Length = sourceTargetMappings[i].TargetColumn.Length;
                externalColumn.Precision = sourceTargetMappings[i].TargetColumn.Precision;
                externalColumn.Scale = sourceTargetMappings[i].TargetColumn.Scale;
                externalColumn.CodePage = sourceTargetMappings[i].TargetColumn.CodePage;

                dest.ExternalColumnInputColumnMap.Add(new ExternalColumnInputMap { ExternalColumn = externalColumn, InputColumnName = sourceTargetMappings[i].SourceColumn.Name });
            }
            // Now perform the manual mapping. Otherwise, SSIS will complain that atleast one column should be mapped.
            dest.ManualMapToTargetColumns();

            project.SaveToDisk();
        }
    }
}
