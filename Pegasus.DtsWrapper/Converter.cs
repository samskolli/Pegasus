using System;   

namespace Pegasus.DtsWrapper
{
    public static class Converter
    {
        #region Sql Server Data Type to SSIS Data Type

        public static SSISDataTypeWithProperty TranslateSqlServerDataTypeToSSISDataTypeWithProperty(string sqlServerDataType, string dataTypeLength)
        {
            SSISDataType dt;
            int codepage = 0;
            int length = 0;
            if (dataTypeLength == "(max)" || dataTypeLength == "max")
            {
                switch (sqlServerDataType.ToLower())
                {
                    case "nvarchar":
                        dt = SSISDataType.DT_NTEXT;
                        break;
                    case "varchar":
                        dt = SSISDataType.DT_TEXT;
                        codepage = 1252;
                        break;
                    default:
                        dt = SSISDataType.DT_NTEXT;
                        break;
                }
            }
            else
            {
                switch (sqlServerDataType.ToLower())
                {
                    case "nvarchar":
                        dt = SSISDataType.DT_WSTR;
                        break;
                    case "nchar":
                        dt = SSISDataType.DT_WSTR;
                        break;
                    case "bigint":
                        dt = SSISDataType.DT_I8;
                        break;
                    case "int":
                        dt = SSISDataType.DT_I4;
                        break;
                    case "tinyint":
                        dt = SSISDataType.DT_I2;
                        break;
                    case "datetime":
                        dt = SSISDataType.DT_DBTIMESTAMP;
                        break;
                    case "date":
                        dt = SSISDataType.DT_DBDATE;
                        break;
                    case "bit":
                        dt = SSISDataType.DT_BOOL;
                        break;
                    case "varchar":
                        dt = SSISDataType.DT_STR;
                        break;
                    case "char":
                        dt = SSISDataType.DT_STR;
                        break;
                    case "time":
                        dt = SSISDataType.DT_DBTIME2;
                        break;
                    case "float":
                        dt = SSISDataType.DT_R8;
                        break;
                    default:
                        dt = SSISDataType.DT_STR;
                        break;
                }

                switch (sqlServerDataType.ToLower())
                {
                    case "varchar":
                        length = Convert.ToInt32(dataTypeLength.Replace("(", "").Replace(")", ""));
                        break;
                    case "char":
                        length = Convert.ToInt32(dataTypeLength.Replace("(", "").Replace(")", ""));
                        break;
                    case "nvarchar":
                        length = Convert.ToInt32(dataTypeLength.Replace("(", "").Replace(")", ""));
                        break;
                    case "nchar":
                        length = Convert.ToInt32(dataTypeLength.Replace("(", "").Replace(")", ""));
                        break;
                }

                if (sqlServerDataType.ToLower() == "varchar" || sqlServerDataType.ToLower() == "char")
                {
                    codepage = 1252;
                }
            }

            return new SSISDataTypeWithProperty { DataType = dt, CodePage = codepage, Length = length, Precision = 0, Scale = 0 };
        }

        #endregion

        #region Sql Server Data Type to .Net Get Data Type

        public static string TranslateSqlServerDataTypeToNetCastDataType(string sqlServerDataType)
        {
            string dt = "";
            switch (sqlServerDataType.ToLower())
            {
                //case "varchar":
                //    dt = "GetString";
                //    break;
                //case "char":
                //    dt = "GetString";
                //    break;
                //case "nvarchar":
                //    dt = "GetString";
                //    break;
                //case "nchar":
                //    dt = "GetString";
                //    break;
                case "int":
                    dt = "Int32";
                    break;
                case "bigint":
                    dt = "Int64";
                    break;
                case "tinyint":
                    dt = "Byte";
                    break;
                case "bit":
                    dt = "Boolean";
                    break;
                case "time":
                    dt = "DateTime";
                    break;
                case "datetime":
                    dt = "DateTime";
                    break;
                case "date":
                    dt = "DateTime";
                    break;
                case "float":
                    dt = "Double";
                    break;
            }
            return dt;
        }

        #endregion

        public static string TranslateAdoNetDataTypeToGETDataType(string adoNetDataType)
        {
            string dt = "";
            switch (adoNetDataType.ToLower())
            {
                case "boolean":
                    dt = "GetBoolean";
                    break;
                case "char":
                    dt = "GetString";
                    break;
                case "datetime":
                    dt = "GetDateTime";
                    break;
                case "decimal":
                    dt = "GetDouble";
                    break;
                case "double":
                    dt = "GetDouble";
                    break;
                case "guid":
                    dt = "GetString";
                    break;
                case "int16":
                    dt = "GetInt16";
                    break;
                case "int32":
                    dt = "GetInt32";
                    break;
                case "int64":
                    dt = "GetInt64";
                    break;
                //SByte
                //single
                case "string":
                    dt = "GetString";
                    break;
                case "timespan":
                    dt = "GetTime";
                    break;
                case "uint16":
                    dt = "GetInt16";
                    break;
                case "uint32":
                    dt = "GetInt32";
                    break;
                case "uint64":
                    dt = "GetInt64";
                    break;
            }
            return dt;
        }

        public static SSISDataTypeWithProperty GetSSISDataTypeFromADONetDataType(string adoNetDataType, int dataTypeLength)
        {
            SSISDataType dt = SSISDataType.DT_EMPTY;
            int codepage = 0;
            int length = 0;

            #region conv

            switch (adoNetDataType.ToLower())
            {
                case "boolean":
                    dt = SSISDataType.DT_BOOL;
                    break;
                case "char":
                    dt = SSISDataType.DT_STR;
                    break;
                case "datetime":
                    dt = SSISDataType.DT_DBTIMESTAMP;
                    break;
                case "decimal":
                    dt = SSISDataType.DT_NUMERIC;
                    break;
                case "double":
                    dt = SSISDataType.DT_R8;
                    break;
                case "guid":
                    dt = SSISDataType.DT_GUID;
                    break;
                case "int16":
                    dt = SSISDataType.DT_I2;
                    break;
                case "int32":
                    dt = SSISDataType.DT_I4;
                    break;
                case "int64":
                    dt = SSISDataType.DT_I8;
                    break;
                //SByte
                //single
                case "string":
                    dt = SSISDataType.DT_WSTR;
                    break;
                case "timespan":
                    dt = SSISDataType.DT_DBTIME;
                    break;
                case "uint16":
                    dt = SSISDataType.DT_UI2;
                    break;
                case "uint32":
                    dt = SSISDataType.DT_UI4;
                    break;
                case "uint64":
                    dt = SSISDataType.DT_UI8;
                    break;
            }

            #endregion

            if (adoNetDataType.ToLower() == "string")
            {
                length = dataTypeLength;
            }

            SSISDataTypeWithProperty sdt = new SSISDataTypeWithProperty();
            sdt.DataType = dt;
            sdt.Length = length;
            sdt.Precision = 0;
            sdt.Scale = 0;
            sdt.CodePage = codepage;

            return sdt;
        }

        public static string TranslateNetDataTypeToSSISDataType(string netDataType)
        {
            switch (netDataType.ToLower())
            {
                case "system.string":
                    return "DT_WSTR";
                case "system.int64":
                    return "DT_I8";
                case "system.int32":
                    return "DT_I4";
                case "system.int16":
                    return "DT_I2";
                case "system.datetime":
                    return "DT_DBTIMESTAMP";
                case "system.timespan":
                    return "DT_DBTIME2";
                case "system.double":
                    return "DT_R8";
                default:
                    return "DT_STR";
            }
        }
    }

    public struct SSISDataTypeWithProperty
    {
        public SSISDataType DataType { get; set; }
        public int CodePage { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
        public int Precision { get; set; }
    }
}
