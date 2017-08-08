using System.IO;
using LumenWorks.Framework.IO.Csv;
using Pegasus.DtsWrapper;
using System;

namespace Pegasus.Demo
{
    public class FileHelper
    {
        #region ctor

        private string _file;
        private CsvReader _reader;
        private int _fieldCount;
        internal string[] _columnNames;
        public FileHelper(string file)
        {
            _file = file;
            _reader = new CsvReader(new StreamReader(file), true, ',');
            _columnNames = GetFileColumnNames();
            _fieldCount = _columnNames.Length;
        }

        #endregion

        #region Methods

        #region Get Column Names

        internal string[] GetFileColumnNames()
        {
            return _reader.GetFieldHeaders();
        }

        #endregion

        #region Get Column Lengths

        internal int[] InferFlatFileColumnLengths(int numRowsToUse = 1)
        {
            CsvReader rdr = new CsvReader(new StreamReader(_file), true, ',');

            int iter = 0;
            int[] arrayOfSize = new int[_fieldCount];

            while (rdr.ReadNextRecord() && (numRowsToUse == -1 ? true : iter < numRowsToUse))
            {
                for (int i = 0; i < _fieldCount; i++)
                {
                    if (i == 0)
                    {
                        arrayOfSize.SetValue(rdr[i].Length, i);
                    }
                    if (rdr[i].Length > arrayOfSize[i])
                    {
                        arrayOfSize.SetValue(rdr[i].Length, i);
                    }
                }
                iter = iter + 1;
            }

            for (int i = 0; i < arrayOfSize.Length; i++)
                if (arrayOfSize[i] == 0)
                    arrayOfSize[i] = 1024;

            return arrayOfSize;
        }

        #endregion

        #region Get Column SSIS Data Type

        internal string[] InferFlatColumnSSISDataType()
        {
            int intVal;
            DateTime dtVal;
            CsvReader rdr = new CsvReader(new StreamReader(_file), true, ',');

            string[] dataTypes = new string[_fieldCount];

            rdr.ReadNextRecord();
            for (int i = 0; i < _fieldCount; i++)
            {
                if (Int32.TryParse(rdr[i], out intVal))
                {
                    dataTypes.SetValue(Converter.TranslateNetDataTypeToSSISDataType("system.int32"), i);
                }
                else if (DateTime.TryParse(rdr[i], out dtVal))
                {
                    dataTypes.SetValue(Converter.TranslateNetDataTypeToSSISDataType("system.datetime"), i);
                }
                else
                {
                    dataTypes.SetValue(Converter.TranslateNetDataTypeToSSISDataType("plain string"), i);
                }
            }

            return dataTypes;
        }

        #endregion

        #region Get Column Sql Server Data Type

        internal string[] InferFlatColumnSqlServerDataType()
        {
            int intVal;
            DateTime dtVal;

            string[] dataTypes = new string[_fieldCount];

            _reader.ReadNextRecord();
            for (int i = 0; i < _fieldCount; i++)
            {
                if (Int32.TryParse(_reader[i], out intVal))
                {
                    dataTypes.SetValue("int", i);
                }
                else if (DateTime.TryParse(_reader[i], out dtVal))
                {
                    dataTypes.SetValue("datetime", i);
                }
                else
                {
                    dataTypes.SetValue("varchar", i);
                }
            }

            return dataTypes;
        }

        #endregion

        #region Get Create Statement

        internal string[] GetSqlServerDataTypes()
        {
            string[] dataTypes = InferFlatColumnSqlServerDataType();
            int[] cWidths = InferFlatFileColumnLengths(10);
            string[] combinedDataTypes = new string[_fieldCount];

            for (int i = 0; i < _fieldCount; i++)
            {
                combinedDataTypes.SetValue(dataTypes[i] + (dataTypes[i] == "varchar" ? "(" + cWidths[i].ToString() + ")" : ""), i);
            }
            return combinedDataTypes;
        }

        internal string GetCreateStatement()
        {
            string tableName = Path.GetFileNameWithoutExtension(_file);
            string s = "if object_id('" + tableName + "') is not null drop table dbo." + tableName + "\n" + "go" + "\n";
            s = s + "create table " + tableName + "(" + "\n";

            string[] cNames = GetFileColumnNames();
            string[] dataTypes = InferFlatColumnSqlServerDataType();
            int[] cWidths = InferFlatFileColumnLengths(10);

            for (int i = 0; i < cNames.Length; i++)
            {
                if (i == cNames.Length - 1)
                    s = s + cNames[i] + " " + dataTypes[i] + (dataTypes[i] == "varchar" ? "(" + cWidths[i].ToString() + ")" : "");
                else
                    s = s + cNames[i] + " " + dataTypes[i] + (dataTypes[i] == "varchar" ? "(" + cWidths[i].ToString() + ")" : "") + ",\n";
            }
            s = s + "\n" + ")";
            return s;
        }

        #endregion

        #endregion


    }
}
