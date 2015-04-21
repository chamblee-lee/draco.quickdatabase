using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Schema;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;

namespace Draco.DB.QuickDataBase.SqLiteClient
{
    /// <summary>
    /// SqLite数据库结构查询
    /// </summary>
    public class SQLiteSchemaHandler : IDataBaseSchemaHandler
    {
        private IDataBaseHandler m_Handler = null;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public SQLiteSchemaHandler(IDataBaseHandler Handler)
        {
            m_Handler = Handler;
        }
        #region 加载表信息
        /// <summary>
        /// 获取所有的表名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            string SQL = @"SELECT name, 'main' FROM sqlite_master WHERE type='table' order by tbl_name";
            DataSet ds = m_Handler.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                List<String> list = new List<String>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(Convert.ToString(row["name"]));
                }
                return list;
            }
            return null;
        }
        public IList<Draco.DB.QuickDataBase.Schema.Vendor.IDataTableColumn> ReadColumns(string TableName)
        {
            string SQL = String.Format("PRAGMA table_info('{0}')", TableName);
            DataSet ds = m_Handler.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                IList<IDataTableColumn> list = new List<IDataTableColumn>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    IDataTableColumn Col = new DataTableColumn();
                    Col.SimpleType = "0"; //默认的公共类型

                    Col.ColumnName = Convert.ToString(row["name"]);
                    Col.Nullable = Convert.ToInt32(row["notnull"]) == 0;
                    Col.TableName = TableName;
                    Col.TableSchema = TableName;
                    if (Convert.ToInt32(row["pk"]) == 1)//主键
                        Col.PrimaryKey = true;
                    Col.DefaultValue = Convert.ToString(row["dflt_value"]);//缺省值
                    string DType = Convert.ToString(row["type"]);
                    if (DType.Contains("("))//VARCHAR和NVARCHAR类型是包含数据长度的
                    {
                        int LeftBracket = DType.IndexOf("(");
                        int RightBracket = DType.IndexOf(")");
                        string sLen = DType.Substring(LeftBracket + 1, RightBracket - LeftBracket - 1);//取出长度
                        String[] sLenAndPrecision = sLen.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        Col.Length = Convert.ToInt32(sLenAndPrecision[0]);
                        if (sLenAndPrecision.Length > 1)//除了长度，还有精度
                        {
                            Col.Precision = Convert.ToInt32(sLenAndPrecision[1]);
                        }
                        DType = DType.Substring(0, LeftBracket);
                    }
                    Col.Type = DType;
                    Col.FullType = Col.Type;

                    list.Add(Col);
                }
                return list;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 添加字段到表中
        /// </summary>
        /// <param name="strTblName"></param>
        /// <param name="strFldName"></param>
        /// <param name="strFldDes"></param>
        /// <returns></returns>
        public bool AddFieldToTable(string strTblName, string strFldName, string strFldDes)
        {
            string strSQL = "select * from " + strTblName + "  where 1>2";

            DataSet ds = m_Handler.ExecuteQuery(strSQL);
            if (ds == null || ds.Tables[0] == null)
                return false;

            if (ds.Tables[0].Columns.IndexOf(strFldName) == -1)
            {
                strSQL = "alter table " + strTblName + " add  " + strFldName.ToUpper() + " " + strFldDes;
                m_Handler.ExecuteNonQuery(strSQL);
            }
            return true;
        }
        /// <summary>
        /// 获取表的注释信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string GetTableComment(string TableName)
        {
            return "";
        }
        /// <summary>
        /// 获取列注释信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetColumnComment(string TableName, string ColumnName)
        {
            return "";
        }
        /// <summary>
        /// 获取字段类型
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="ColumnName">字段名称</param>
        /// <returns></returns>
        public string GetDataTypeByColumnName(string TableName, string ColumnName)
        {
            string SQL = String.Format("PRAGMA table_info('{0}')", TableName);
            DataSet ds = m_Handler.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (String.Compare(Convert.ToString(row["name"]), ColumnName, true) == 0)
                        return Convert.ToString(row["type"]);
                }
            }
            return "";
        }
        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public DbType ConvertToDbType(string type)
        {
            if (String.IsNullOrEmpty(type)) return DbType.String;
            switch (type.ToLower())
            {
                case "bigint": return DbType.Int64;
                case "int": return DbType.Int32;
                case "smallint": return DbType.Int16;
                case "tinyint": return DbType.Int16;
                case "decimal": return DbType.Decimal;
                case "float": return DbType.Double;
                case "real": return DbType.Decimal;
                case "datetime": return DbType.DateTime;
                case "smalldatetime": return DbType.DateTime;
                case "char": return DbType.String;
                case "nchar": return DbType.String;
                case "varchar": return DbType.String;
                case "nvarchar": return DbType.String;
                case "text": return DbType.String;
                case "ntext": return DbType.String;
                case "binary": return DbType.Binary;
                case "varbinary": return DbType.Binary;
                case "image": return DbType.Binary;
                case "bit": return DbType.Byte;
                case "uniqueidentifier": return DbType.String;
                default: return DbType.String;
            }
        }
        /// <summary>
        /// 判断是否存在序列值
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool IsTableHasSequence(string TableName)
        {
            return false;
        }
        /// <summary>
        /// 获取字段的缺省值
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetColumnDefaultValue(string TableName, string ColumnName)
        {
            string SQL = String.Format("PRAGMA table_info('{0}')", TableName);
            DataSet ds = m_Handler.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (String.Compare(Convert.ToString(row["name"]), ColumnName, true) == 0)
                        return Convert.ToString(row["dflt_value"]);
                }
            }
            return "";
        }
        /// <summary>
        /// 判断是否是自增列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public bool IsAotuAddColumn(string TableName, string ColumnName)
        {
            string SQL = String.Format("PRAGMA table_info('{0}')", TableName);
            DataSet ds = m_Handler.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (String.Compare(Convert.ToString(row["name"]), ColumnName, true) == 0)
                    {
                        if (Convert.ToInt32(row["pk"]) == 1)//主键
                        {
                            String type = Convert.ToString(row["type"]);
                            if (String.Compare(type, "INTEGER", true) == 0 ||
                                String.Compare(type, "INT", true) == 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="PKFieldName"></param>
        public void SynchSequence(string TableName, string PKFieldName)
        {

        }

        /// <summary>
        /// 映射到ADO类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public int MapAdoType(IDataType dataType)
        {
            if (dataType == null)
                return (int)ADOType.adInteger;
            switch (dataType.Type.ToUpper())
            {
                case "BOOLEAN":
                case "BIT":
                    return (int)ADOType.adTinyInt;
                case "INTEGER":
                case "INT":
                case "SMALLINT":
                    return (int)ADOType.adInteger;
                case "BIGINT":
                    return (int)ADOType.adBigInt;
                case "FLOAT":
                    {
                        //return (int)ADOType.adSingle;
                        return (int)ADOType.adDouble;
                    }
                case "REAL":
                case "NUMERIC":
                    {
                        if ((dataType.Scale ?? 0) == 0)
                            return (int)ADOType.adInteger;//没有小数位
                        else if (((dataType.Scale ?? 0) <= 6))//6位以下的小数
                            return (int)ADOType.adDouble;
                        else
                            return (int)ADOType.adNumeric;
                    }
                case "TIME":
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                case "TIMESTAMP":
                    return (int)ADOType.adDBTimeStamp;
                case "CHAR":
                case "VARCHAR":
                case "NVARCHAR":
                    return (int)ADOType.adChar;
                case "TEXT":
                case "NTEXT":
                    return (int)ADOType.adLongVarChar;
                case "BLOB":
                case "BINARY":
                case "IMAGE":
                    return (int)ADOType.adBinary;
                default:
                    return (int)ADOType.adInteger;
            }
        }

        /// <summary>
        /// 映射到DB类型
        /// </summary>
        /// <param name="datatablecolumn"></param>
        /// <returns></returns>
        public void MapDBType(ref DataTableColumn datatablecolumn)
        {
            string typeid = datatablecolumn.SimpleType;
            switch (typeid)
            {
                //adSmallInt
                case "2":
                //adInteger
                case "3":
                //adBigInt
                case "20":
                    {
                        datatablecolumn.SimpleType = "INTEGER";  
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adDouble
                case "5":
                //adNumeric
                case "131":
                    {
                        //datatablecolumn.SimpleType = "REAL";
                        datatablecolumn.SimpleType = "FLOAT";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adChar
                case "129":
                    {
                        datatablecolumn.SimpleType = "VARCHAR";
                        return;
                    }
                //adDBTimeStamp  
                case "135":
                    {
                        datatablecolumn.SimpleType = "DATE";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adLongVarChar
                case "201":
                    {
                        datatablecolumn.SimpleType = "TEXT";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adBinary
                case "128":
                    {
                        datatablecolumn.SimpleType = "BLOB";
                        datatablecolumn.Length = 0;
                        return;
                    }
                default:
                    {
                        datatablecolumn.SimpleType = "INTEGER";
                        datatablecolumn.Length = 0;
                        return;
                    }
            }
        }

        /// <summary>
        /// 根据表结构列表创建建表的Sql语句
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public string GetDbServerScript(ArrayList List)
        {
            if (List == null || List.Count == 0)
            {
                return "";
            }
            StringBuilder rtn = new StringBuilder();//"BEGIN TRANSACTION ;\n";
            rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
            rtn.Append("\n");

            try
            {
                foreach (Table table in List)
                {
                    Table tbl = table;
                    List<DataTableColumn> columnList = tbl.Columns;
                    
                    rtn.Append("CREATE TABLE [" + tbl.TableName.ToUpper() + "] (");
                    string pk = " PRIMARY KEY ";

                    //标记唯一的自增主键
                    bool enidentity = true;
                    bool enprimary = true;

                    //开始添加每一个字段
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        DataTableColumn dtc = columnList[i];
                        string FieldName = dtc.ColumnName.ToUpper();
                        rtn.Append("[" + FieldName + "]   ");
                        string FieldType = dtc.SimpleType;

                        long? FidldLength = dtc.Length;//长度
                        int? Precision = dtc.Precision;//整数部分
                        int? Scale = dtc.Scale;//小数部分

                        if (FidldLength > 0)
                        {
                            if (Scale > 0)
                            {
                                rtn.Append(FieldType + "(" + Precision + "," + Scale + ")");
                            }
                            else
                            {
                                rtn.Append(FieldType + "(" + FidldLength + ")");
                            }
                        }
                        else
                        {
                            rtn.Append(FieldType);
                        }
                        if (!dtc.Nullable)
                        {
                            rtn.Append(" NOT NULL ");
                        }
                        if (dtc.PrimaryKey != null && (bool)dtc.PrimaryKey)
                        {
                            if (enprimary)
                            {
                                rtn.Append(pk);
                                enprimary = false;
                            }
                        }
                        if (dtc.Generated != null && (bool)dtc.Generated)
                        {
                            if (enidentity)
                            {
                                rtn.Append("   AUTOINCREMENT ");
                                enidentity = false;//唯一的
                            }
                        }
                        if (dtc.DefaultValue != null && dtc.DefaultValue.Trim().Length > 0)
                        {
                            string defaultVal = dtc.DefaultValue;
                            if (String.Compare(defaultVal, "(SYSDATE)", true) == 0)
                            {
                                defaultVal = "(GETDATE())";
                            }
                            rtn.Append("  DEFAULT  " + defaultVal + "  ");
                        }
                        rtn.Append(",\n");
                    }

                    bool HavePk = false;    
                    bool HaveUnique = false;
                    if (!HavePk && !HaveUnique)
                    {//当没有主键和约束的时候
                        int n = rtn.ToString().LastIndexOf(",");
                        if (n != -1)
                        {
                            rtn = rtn.Remove(n, 1);//去掉最后一个','
                        }
                    }
                    rtn.Append("\n ) \n");

                    rtn.Append("\n");
                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");
                }
            }
            catch
            {
                throw;
            }
            return rtn.ToString();    
            //Encoding utf8 = Encoding.GetEncoding(65001);
            //Encoding gb2312 = Encoding.GetEncoding("gb2312");
            //byte[] temp = utf8.GetBytes(rtn.ToString());
            //byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
            //string result = gb2312.GetString(temp1);
            //return result;
        }

        /// <summary>
        /// 获取表信息sql
        /// </summary>
        /// <param name="tblname"></param>
        /// <returns></returns>
        public string GetSqlOfTableInfo(string tblname)
        {
            return "select count(*) from sqlite_master where type='table' AND name='" + tblname + "' order by tbl_name";
        }

        /// <summary>
        /// 获取修改字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetAlterSqlOfColumnInfo(DataTableColumn field)
        {
            long? len = field.Length;
            string type = field.SimpleType;
            if (type.ToUpper().Equals("VARCHAR"))
                type += "(" + len + ")";
            //return "ALTER TABLE " + field.TableName + " alter column " + field.ColumnName + " " + type;
            return "";//不允许修改可删除字段，只能修改表名和添加字段
        }

        /// <summary>
        /// 获取添加字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetAlterAddSqlOfColumnInfo(DataTableColumn field)
        {
            long? len = field.Length;
            string type = field.SimpleType;
            if (type.ToUpper().Equals("VARCHAR"))
                type += "(" + len + ")";
            if (field.DefaultValue != null && field.DefaultValue.Length > 0)
            {
                string defaultVal = field.DefaultValue;
                if (String.Compare(defaultVal, "(SYSDATE)", true) == 0)
                {
                    defaultVal = "(GETDATE())";
                }
                type += "  DEFAULT  " + defaultVal + "  ";
                if (field.Nullable)
                    type += " null ";
                else
                    type += " not null ";
            }
            if (field.Generated != null && (bool)field.Generated)
            {
                //type += "  UNIQUE  ";
            }
            return "ALTER TABLE " + field.TableName + " ADD " + field.ColumnName + " " + type;
        }
    }
}
