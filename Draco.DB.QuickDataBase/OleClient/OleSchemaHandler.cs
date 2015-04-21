using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Adapter;
using System.Collections;
using Draco.DB.QuickDataBase.Utility;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase.Schema;
using System.Security.Permissions;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.QuickDataBase.OleClient
{
    /// <summary>
    /// Ole数据库结构查询
    /// </summary>
    public class OleSchemaHandler : IDataBaseSchemaHandler
    {
        private IDataBaseHandler m_DBHandler = null;
        private string m_ConnectString = null;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public OleSchemaHandler(IDataBaseHandler Handler)
        {
            m_DBHandler = Handler;
            m_ConnectString = Handler.DbContext.ConnectionInfo.ConnectionString;
        }

        #region 加载表信息
        /// <summary>
        /// 获取所有的表名称
        /// </summary>
        /// <returns></returns>
        public List<String> GetTableNames()
        {
            DataTable tbl = null;
            string DBOwner = GetDBOwner(m_ConnectString);
            using (OleDbConnection conn = new OleDbConnection(m_ConnectString))
            {
                conn.Open();
                tbl = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, DBOwner, null, "TABLE" });
                /*
                 * 
                 *	限制列			CLR 数据类型	描述 
                    TABLE_CATALOG	string			目录名称。如果提供程序不支持目录，则为空值。 
                    TABLE_SCHEMA	string			非限定的架构名称。如果提供程序不支持架构，则为空值。 
                    TABLE_NAME		string			表名称。返回的列不能包含空值。 
                    TABLE_TYPE		string			表类型。以下之一或提供程序特定的值：“ALIAS”、“TABLE”、“SYNONYM”、“SYSTEM TABLE”、“VIEW”、“GLOBAL TEMPORARY”、“LOCAL TEMPORARY”或“SYSTEM VIEW”。返回的列不能包含空值。 
                 * 
                 * */
                conn.Close();
            }
            if (tbl != null && tbl.Rows.Count > 0)
            {
                List<String> list = new List<String>();
                foreach (DataRow row in tbl.Rows)
                {
                    list.Add(Convert.ToString(row["TABLE_NAME"]));
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 获取单表的数据字段信息
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <returns></returns>
        //[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public IList<IDataTableColumn> ReadColumns(string TableName)
        {
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("tablename");
            string DBOwner = GetDBOwner(m_ConnectString);
            using (OleDbConnection conn = new OleDbConnection(m_ConnectString))
            {
                conn.Open();
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new Object[] { null, null, TableName, null });
                DataTable PK_dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new Object[] { null, DBOwner, TableName });

                /*
                 * 限制列			CLR 数据类型		描述 
                 * TABLE_CATALOG	string				目录名称。如果提供程序不支持目录，则为空值。 
                 * TABLE_SCHEMA		string				非限定的架构名称。如果提供程序不支持架构，则为空值。 
                 * TABLE_NAME		string				表名称。返回的列不能包含空值。 
                 * COLUMN_NAME		string				列的名称；它可能不唯一。如果无法确定该名称，则返回空值。此列与 COLUMN_GUID 和 COLUMN_PROPID 列一起形成列 ID。根据提供程序使用的 DBID 结构元素，其中的一个或多个列将为空值。如果可能，结果列 ID 应持久保持。然而，某些提供程序不支持列的持久性标识符。基表的列 ID 应该在视图下保持不变。 

                 * */
                //先获取主键名称
                string PKName = null;
                if (PK_dt != null && PK_dt.Rows.Count > 0)
                {
                    PKName = Convert.ToString(PK_dt.Rows[0]["COLUMN_NAME"]);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    //给字段排序
                    DataView dv = dt.DefaultView;
                    dv.Sort = "ORDINAL_POSITION Asc";
                    DataTable newDt = dv.ToTable();
                    IList<IDataTableColumn> list = new List<IDataTableColumn>();
                    foreach (DataRow row in newDt.Rows)
                    {
                        IDataTableColumn Col = new DataTableColumn();
                        Col.SimpleType = "0"; //默认的公共类型

                        Col.ColumnName = Convert.ToString(row["COLUMN_NAME"]);//列名
                        Col.TableName = TableName;  //表名
                        Col.TableSchema = TableName;
                        //主键
                        bool IsPrimaryKey = String.Compare(Col.ColumnName, PKName, true) == 0;
                        if (IsPrimaryKey)
                            Col.PrimaryKey = true;
                        //是否可以为空
                        Col.Nullable = Convert.ToBoolean(row["IS_NULLABLE"]);
                        //缺省值
                        Col.DefaultValue = Convert.ToString(row["COLUMN_DEFAULT"]);
                        //字段类型
                        int DType = Convert.ToInt32(row["DATA_TYPE"]);
                        Col.Type = Enum.GetName(typeof(ADOType), DType);
                        Col.FullType = Col.Type;
                        //长度
                        int x = 0;
                        if (Int32.TryParse(Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]), out x))
                            Col.Length = x;
                        if (Int32.TryParse(Convert.ToString(row["NUMERIC_PRECISION"]), out x))//精度
                            Col.Precision = x;
                        if (Int32.TryParse(Convert.ToString(row["NUMERIC_SCALE"]), out x))//有效位数
                            Col.Scale = x;

                        list.Add(Col);
                    }
                    return list;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 新增添加一个字段到指定表中
        /// </summary>
        /// <param name="strTblName">表名</param>
        /// <param name="strFldName">字段名</param>
        /// <param name="strFldDes">字段信息描述</param>
        /// <returns></returns>
        public bool AddFieldToTable(string strTblName, string strFldName, string strFldDes)
        {
            string strSQL = "select * from " + strTblName + "  where 1>2";

            DataSet ds = m_DBHandler.ExecuteQuery(strSQL);
            if (ds == null || ds.Tables[0] == null)
                return false;

            if (ds.Tables[0].Columns.IndexOf(strFldName) == -1)
            {
                 strSQL = "alter table " + strTblName + " add  " + strFldName.ToUpper() + " " + strFldDes;
                 m_DBHandler.ExecuteNonQuery(strSQL);
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
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetDataTypeByColumnName(string TableName, string ColumnName)
        {
            OleDbConnection connection = new OleDbConnection(m_ConnectString);
            try
            {
                connection.Open();
                object[] restrictions = new object[4];
                restrictions[2] = TableName;
                restrictions[3] = ColumnName;
                DataTable oleDbSchemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);
                connection.Close();
                connection.Dispose();
                return Convert.ToString(oleDbSchemaTable.Rows[0]["DATA_TYPE"]);
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                return "-1";
            }
        }
        /// <summary>
        /// 转换为DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DbType ConvertToDbType(String type)
        {
            if(String.IsNullOrEmpty(type))
                return DbType.String;
            int n = 0;
            if (Int32.TryParse(type, out n))
                return ConvertToDbType(n);
            string ntype = type.ToLower().Trim();
            DbType DType = DbType.String;
            switch (ntype)
            {
                case "adTinyInt": DType = DbType.Byte; break;
                case "adBoolean": 
                case "adUnsignedTinyInt":
                case "adSmallInt": DType = DbType.Int16; break;
                case "adUnsignedSmallInt":
                case "adInteger": DType = DbType.Int32; break;
                case "adUnsignedInt":
                case "adBigInt": DType = DbType.Int64; break;
                case "adDouble": DType = DbType.Double; break;
                case "adSingle": DType = DbType.Single;break;
                case "adCurrency":
                case "adDecimal": 
                case "adUnsignedBigInt":
                case "adNumeric": DType = DbType.Decimal; break;
                case "adDate": 
                case "adDBDate":
                case "adDBTimeStamp":
                case "adFileTime":
                case "adDBTime": DType = DbType.DateTime; break;
                case "adChar":
                case "adWChar":
                case "adBSTR": DType = DbType.String; break;
                case "adGUID": DType = DbType.Guid; break;
                case "adBinary": DType = DbType.Binary; break;
                case "adPropVariant":
                case "adIDispatch":
                case "adIUnknown":
                case "adVariant": DType = DbType.Object; break;
            }
            return DType;
        }
        private DbType ConvertToDbType(int type)
        {
            switch (type)
            {
                case 2: return DbType.Int16;
                case 3: return DbType.Int32;
                case 4: return DbType.Single;
                case 5: return DbType.Double;
                case 6: return DbType.Currency;
                case 7: return DbType.Date;
                case 8: return DbType.String;
                case 10: return DbType.String;
                case 11: return DbType.Boolean;
                case 12: return DbType.AnsiString;
                case 14: return DbType.Decimal;
                case 16: return DbType.Int16;
                case 17: return DbType.UInt16;
                case 18: return DbType.UInt16;
                case 19: return DbType.UInt32;
                case 20: return DbType.Int64;
                case 21: return DbType.UInt64;
                case 64: return DbType.DateTime;
                case 128: return DbType.Binary;
                case 129: return DbType.String;
                case 130: return DbType.String;
                case 131: return DbType.Decimal;
                case 133: return DbType.DateTime;
                case 134: return DbType.DateTime;
                case 135: return DbType.DateTime;
                case 138: return DbType.AnsiString;
                case 139: return DbType.VarNumeric;
                case 200: return DbType.String;
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
        /// 
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetColumnDefaultValue(string TableName, string ColumnName)
        {
            //不被支持的方法
            throw new NotImplementedException();
        }
        /// <summary>
        /// 是否是自增列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public bool IsAotuAddColumn(string TableName, string ColumnName)
        {
            //不被支持的方法
            throw new NotImplementedException();
        }
        /// <summary>
        /// 同步序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="PKFieldName"></param>
        public void SynchSequence(string TableName, string PKFieldName)
        {
            //不被支持的方法
            throw new NotImplementedException();
        }

        //从连接串中获取数据库所有者
        private string GetDBOwner(string ConnString)
        {
            string DbOwner = null;
            Hashtable hsb = ConnectionStringAnalyzer.GetConnctionStringAtt(ConnString);
            if (hsb != null && hsb.ContainsKey("Provider"))
            {
                string Provider = hsb["Provider"].ToString().ToUpper();
                if (Provider.StartsWith("ORAOLEDB") || Provider.StartsWith("MSDAORA"))
                {
                    DbOwner = hsb["USER ID"].ToString().ToUpper();
                }
                else if (Provider.StartsWith("SQLOLEDB"))
                {
                    DbOwner = "DBO";
                }
            }
            return DbOwner;
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
            ADOType AdoType = (ADOType)Enum.Parse(typeof(ADOType), dataType.Type);
            if (AdoType == ADOType.adNumeric || AdoType == ADOType.adDecimal)
            {
                if ((dataType.Scale ?? 0) == 0)
                    AdoType = ADOType.adInteger;//没有小数位
                else if (((dataType.Scale ?? 0) <= 6))//6位一下的小数
                    AdoType = ADOType.adDouble;
                else
                    AdoType = ADOType.adNumeric;
            }
            return (int)AdoType;
        }

        /// <summary>
        /// 映射到DB类型
        /// </summary>
        /// <param name="datatablecolumn"></param>
        /// <returns></returns>
        public void MapDBType(ref DataTableColumn datatablecolumn)
        {
            string typeid = datatablecolumn.SimpleType;

        }

        /// <summary>
        /// 根据表结构列表创建建表的Sql语句
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public string GetDbServerScript(ArrayList List)
        {
            return "";
        }

        /// <summary>
        /// 获取表信息sql
        /// </summary>
        /// <param name="tblname"></param>
        /// <returns></returns>
        public string GetSqlOfTableInfo(string tblname)
        {
            return "";
        }

        /// <summary>
        /// 获取修改字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetAlterSqlOfColumnInfo(DataTableColumn field)
        {
            return "";
        }

        /// <summary>
        /// 获取添加字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetAlterAddSqlOfColumnInfo(DataTableColumn field)
        {
            return "";
        }
        
    }
}
