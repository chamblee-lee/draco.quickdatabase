using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Schema.Vendor;
using System.Collections;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.QuickDataBase.SqlClient
{
    /// <summary>
    /// 数据库结构查询
    /// </summary>
    public class SqlSchemaHandler : IDataBaseSchemaHandler
    {
        private IDataBaseHandler m_DBHelper = null;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public SqlSchemaHandler(IDataBaseHandler Handler)
        {
            m_DBHelper = Handler;
        }

        #region 加载表信息
        /// <summary>
        /// 获取所有的表名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            string SQL = "SELECT * FROM INFORMATION_SCHEMA.TABLES where table_type='BASE TABLE' or table_type='TABLE' ";//select [id], [name] from [sysobjects] where [type] = 'u' order by [name]";
            DataSet ds = m_DBHelper.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                List<String> list = new List<String>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(Convert.ToString(row["table_name"]));
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
        public virtual IList<IDataTableColumn> ReadColumns(string TableName)
        {
            string SQL = String.Format(SelectFieldSQL, TableName);
            DataSet ds = m_DBHelper.ExecuteQuery(SQL);
            Hashtable hs = GetColumnComment(TableName);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                IList<IDataTableColumn> list = new List<IDataTableColumn>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    IDataTableColumn Col = new DataTableColumn();
                    Col.SimpleType = "0"; //默认的公共类型

                    Col.ColumnName = Convert.ToString(row["字段名"]);
                    Col.Nullable = !String.IsNullOrEmpty(Convert.ToString(row["允许空"]));
                    Col.TableName = TableName;
                    Col.TableSchema = TableName;
                    Col.Type = Convert.ToString(row["类型"]);
                    if (!String.IsNullOrEmpty(Convert.ToString(row["主键"])))
                        Col.PrimaryKey = true;
                    if (!String.IsNullOrEmpty(Convert.ToString(row["标识"])))
                        Col.Generated = true;

                    Col.Length = Convert.ToInt32(row["长度"]);
                    Col.Precision = Convert.ToInt32(row["长度"]);//整数长度
                    Col.Scale = Convert.ToInt32(row["小数位数"]);

                    Col.FullType = Col.Type;
                    Col.DefaultValue = Convert.ToString(row["默认值"]);
                    if (hs != null && hs[Col.ColumnName] != null)//注释
                        Col.Comment = hs[Col.ColumnName].ToString();
                    list.Add(Col);
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 查询字段信息的SQL
        /// </summary>
        protected string SelectFieldSQL
        {
            get
            {
                return @"SELECT 
    表名       = case when a.colorder=1 then d.name else '' end,
    字段序号   = a.colorder,
    字段名     = a.name,
    标识       = case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '√'else '' end,
    主键       = case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
                     SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then '√' else '' end,
    类型       = b.name,
    占用字节数 = a.length,
    长度       = COLUMNPROPERTY(a.id,a.name,'PRECISION'),
    小数位数   = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0),
    允许空     = case when a.isnullable=1 then '√'else '' end,
    默认值     = isnull(e.text,'')
FROM 
    syscolumns a
left join 
    systypes b 
on 
    a.xusertype=b.xusertype
inner join 
    sysobjects d 
on 
    a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
left join 
    syscomments e 
on 
    a.cdefault=e.id
where 
    d.name='{0}'
order by 
    a.id,a.colorder";
            }
        }
        /// <summary>
        /// 获取表中所有字段的注释
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        protected Hashtable GetColumnComment(string TableName)
        {
            string SQL = "SELECT * FROM ::fn_listextendedproperty ('MS_DESCRIPTION', 'user', 'dbo', 'table','{0}','column',null) ";
            SQL = String.Format(SQL, TableName);
            DataSet ds = m_DBHelper.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Hashtable hs = new Hashtable();
                foreach (DataRow row in ds.Tables[0].Rows)
                    hs.Add(Convert.ToString(row["objname"]), Convert.ToString(row["value"]));
                return hs;
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

            DataSet ds = m_DBHelper.ExecuteQuery(strSQL);
            if (ds == null || ds.Tables[0] == null)
                return false;

            if (ds.Tables[0].Columns.IndexOf(strFldName) == -1)
            {
                strSQL = "alter table " + strTblName + " add  [" + strFldName.ToUpper() + "]  " + System.Text.RegularExpressions.Regex.Replace(
                strFldDes, "nvarchar2", "nvarchar", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                m_DBHelper.ExecuteNonQuery(strSQL);
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
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TblName");

            string Sql = "SELECT value FROM ::fn_listextendedproperty ('MS_DESCRIPTION', 'user', 'dbo', 'table'," +
                "'" + TableName.Replace("'", "''") + "',null,null) ";

            object rtn = m_DBHelper.ExecuteScalar(Sql);
            if (rtn != null)
                return rtn.ToString();
            else
                return null;
        }
        /// <summary>
        /// 获取列注释信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetColumnComment(string TableName, string ColumnName)
        {
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TblName");
            if (String.IsNullOrEmpty(ColumnName))
                throw new ArgumentNullException("ColumnName");

            string Sql = "SELECT value FROM ::fn_listextendedproperty ('MS_DESCRIPTION', 'user', 'dbo', 'table'," +
                "'" + TableName.Replace("'", "''") + "',N'column', N'" + ColumnName.Replace("'", "''") + "') ";

            object rtn = m_DBHelper.ExecuteScalar(Sql);
            if (rtn != null)
                return rtn.ToString();
            else
                return null;
        }
        /// <summary>
        /// 查询数据字段的数据类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <returns>数据类型</returns>
        public string GetDataTypeByColumnName(string tableName, string columnName)
        {
            string strSql = string.Format("select DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{0}' and COLUMN_NAME='{1}'", tableName, columnName);
            DataSet set = m_DBHelper.ExecuteQuery(strSql);
            if ((set.Tables.Count == 1) && (set.Tables[0].Rows.Count == 1))
            {
                return set.Tables[0].Rows[0][0].ToString().ToLower();
            }
            return "";
        }
        /// <summary>
        /// 转换为DbType
        /// </summary>
        /// <param name="type"></param>
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
                case "float": return DbType.Single;
                case "real": return DbType.Decimal;
                case "datetime": return DbType.DateTime;
                case "smalldatetime": return DbType.DateTime;
                case "char": return DbType.Byte;
                case "nchar": return DbType.String;
                case "varchar": return DbType.String;
                case "nvarchar": return DbType.String;
                case "text": return DbType.String;
                case "ntext": return DbType.String;
                case "binary": return DbType.Binary;
                case "varbinary": return DbType.Binary;
                case "image": return DbType.Binary;
                case "bit": return DbType.Byte;
                case "uniqueidentifier": return DbType.Guid;
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
        /// 取得表的列的默认值
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetColumnDefaultValue(string tblName, string ColumnName)
        {
            if (String.IsNullOrEmpty(tblName))
                throw new ArgumentNullException("tblName");
            if (String.IsNullOrEmpty(ColumnName))
                throw new ArgumentNullException("ColumnName");

            string strSql = "SELECT [Default] = d.text FROM syscolumns C " +
                "INNER JOIN sysobjects O " +
                "ON C.id = O.id " +
                "AND O.xtype = 'U' " +
                "AND O.status >= 0 " +
                "LEFT JOIN syscomments D " +
                "ON C.cdefault = D.id " +
                "where o.name = '" + tblName.Replace("'", "''") + "' and  c.name = '" + ColumnName.Replace("'", "''") + "'";
            object o = m_DBHelper.ExecuteScalar(strSql, null, null);
            if (o == null || o == DBNull.Value)
                return null;
            else
                return o.ToString();
        }
        /// <summary>
        /// 判断是否是自增列(Sqlserver)
        /// </summary>
        /// <param name="TblName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public virtual bool IsAotuAddColumn(string TblName, string keyName)
        {
            //判断是否具有自动增长的属性
            SqlParameter[] Paras = new SqlParameter[2];
            Paras[0] = new SqlParameter("tblName", TblName);
            Paras[1] = new SqlParameter("keyName", keyName);
            string SelectSql = "select COLUMNPROPERTY(object_id(@tblName),@keyName,'IsIdentity')";
            string tmpValue;
            object o = m_DBHelper.ExecuteScalar(SelectSql,Paras);
            if (o == null || o == DBNull.Value)
                tmpValue = string.Empty;
            else
                tmpValue = o.ToString();
            return tmpValue == "1";
        }
        
        /// <summary>
        ///	取得Sql自动增长列插入后的值(Sql Server 专用)
        /// </summary>
        /// <param name="rtn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string GetAotuAddColValue(out int rtn, SqlCommand cmd)
        {
            rtn = -1;
            string rtnValue = null;
            object o = m_DBHelper.ExecuteScalar("SELECT @@IDENTITY AS 'Identity'");
            if (o == null || o == DBNull.Value)
                rtnValue = string.Empty;
            else
                rtnValue = o.ToString();
            return rtnValue;
        }
        /// <summary>
        /// 同步序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="PKFieldName"></param>
        public void SynchSequence(string TableName, string PKFieldName)
        {
            return;
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
                case "BIGINT":
                    return (int)ADOType.adBigInt;
                case "INTEGER":
                case "INT":
                case "SMALLINT":
                    return (int)ADOType.adSmallInt;
                case "FLOAT":
                case "REAL":
                    return (int)ADOType.adDouble;
                case "DECIMAL":
                    return (int)ADOType.adNumeric;
                case "NUMERIC":
                    {
                        if ((dataType.Scale ?? 0) == 0)
                            return (int)ADOType.adInteger;//没有小数位
                        else if (((dataType.Scale ?? 0) <= 6))//6位以下的小数
                            return (int)ADOType.adDouble;
                        else
                            return (int)ADOType.adNumeric;
                    }
                case "BOOLEAN":
                case "BIT":
                    return (int)ADOType.adBoolean;
                case "CHAR":
                case "VARCHAR":
                case "NVARCHAR":
                    return (int)ADOType.adChar;
                case "TIME":
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                case "TIMESTAMP":
                    return (int)ADOType.adDBTimeStamp;
                case "TEXT":
                case "NTEXT":
                    return (int)ADOType.adLongVarChar;
                case "BLOB":
                case "BINARY":
                case "VARBINARY":
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
        public virtual void MapDBType(ref DataTableColumn datatablecolumn)
        {
            string typeid = datatablecolumn.SimpleType;
            switch (typeid)
            {
                //adBigInt
                case "20":
                    {
                        datatablecolumn.SimpleType = "BigInt";
                        datatablecolumn.Length = 0;
                        return;
                    }
                    
                //adSmallInt
                case "2":
                    {
                        datatablecolumn.SimpleType = "Int";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adInteger
                case "3":
                    {
                        /*
                        string name=datatablecolumn.ColumnName.Trim();
                        //针对ORA导入的表结构的特殊情况处理
                        if (name.Equals("延迟时间") ||
                            name.Equals("流程时限") ||
                            name.Equals("延迟时间") ||                              
                            name.Equals("活动时限") ||                               
                            name.Equals("剩余时限") ||                               
                            name.Equals("办理时限") ||                              
                            name.Equals("X") ||                              
                            name.Equals("Y") ||                             
                            name.Equals("显示高度") ||                              
                            name.Equals("显示宽度") ||                              
                            name.Equals("字符尺寸") ||                             
                            name.Equals("字体高") ||                             
                            name.Equals("字体宽") ||                               
                            name.Equals("延迟时间") ||                               
                            name.Equals("PRESET_HOUR"))
                        {
                            datatablecolumn.SimpleType = "Decimal";
                            datatablecolumn.Length = 0;
                            return;
                        }
                        */
                        if (datatablecolumn.Scale > 0)
                        {
                            datatablecolumn.SimpleType = "Decimal";
                            if (datatablecolumn.Length >18)
                            {
                                datatablecolumn.Length = 18;
                            }
                            return;
                        }
                        if (datatablecolumn.Length > 9)
                        {
                            datatablecolumn.SimpleType = "BigInt";
                            datatablecolumn.Length = 0;
                        }
                        else if (datatablecolumn.Length < 10)
                        {
                            datatablecolumn.SimpleType = "Int";
                            datatablecolumn.Length = 0;
                        }
                        else
                        {
                            datatablecolumn.SimpleType = "Int";
                            datatablecolumn.Length = 0;
                        }
                        return;
                    }
                //adNumeric
                case "131":
                    {
                        datatablecolumn.SimpleType = "Decimal";
                        //datatablecolumn.Length = 0;
                        return;
                    }
                //adDouble
                case "5":
                    {
                        datatablecolumn.SimpleType = "Float";
                        datatablecolumn.Length = 0;
                        return;
                    }

                //adChar
                case "129":
                    {
                        if (datatablecolumn.Type.ToUpper().Equals("NVARCHAR"))
                            datatablecolumn.SimpleType = "nvarchar";
                        else
                        {
                            datatablecolumn.SimpleType = "VarChar";
                        }
                        return;
                    }
                //adDBTimeStamp  
                case "135":
                    {
                        datatablecolumn.SimpleType = "DateTime";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adLongVarChar
                case "201":
                    {
                        if (datatablecolumn.Type.ToUpper().Equals("NTEXT"))
                            datatablecolumn.SimpleType = "ntext";
                        else
                        {
                            datatablecolumn.SimpleType = "Text";
                            datatablecolumn.Length = 0;
                        }
                        return;
                    }
                //adBinary
                case "128":
                    {
                        datatablecolumn.SimpleType = "Image";
                        datatablecolumn.Length = 0;
                        return;
                    }
                default:
                    {
                        datatablecolumn.SimpleType = "Int";
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
        public virtual string GetDbServerScript(ArrayList List)
        {
            if (List == null || List.Count == 0)
            {
                return "";
            }
            StringBuilder rtn = new StringBuilder();//"BEGIN TRANSACTION ;\n";
            try
            {
                foreach (Table table in List)
                {
                    Table tbl = table;
                    List<DataTableColumn> columnList = tbl.Columns;
                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");
                    //if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[BLD_14_01_TDQSQKHZB]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
                    //drop table [dbo].[BLD_14_01_TDQSQKHZB]
                    rtn.Append("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                        tbl.TableName.ToUpper() + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)\n");
                    rtn.Append("drop table [dbo].[" + tbl.TableName.ToUpper() + "] \n");

                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");

                    rtn.Append("CREATE TABLE [" + tbl.TableName.ToUpper() + "] (");
                    string pk = "PRIMARY KEY (";
                    string unique = "UNIQUE (";

                    //标记唯一的自增主键
                    bool enidentity = true;

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
                        if (dtc.PrimaryKey != null && (bool)dtc.PrimaryKey)
                        {
                            pk += "[" + FieldName + "] , ";
                        }
                        if (!dtc.Nullable)
                        {
                            rtn.Append("   NOT NULL");
                        }
                        if (dtc.Generated != null && (bool)dtc.Generated)
                        {
                            if (dtc.PrimaryKey != null && (bool)dtc.PrimaryKey)
                            { }
                            else
                            {
                                unique += "[" + FieldName + "] , ";
                            }
                            if (enidentity)
                            {
                                /*
                                //只有这8个表有自增字段
                                if (dtc.TableName.ToUpper().Equals("TMPLABELTODBFIELD")||
                                    dtc.TableName.ToUpper().Equals("UPFILESLIST")||
                                    dtc.TableName.ToUpper().Equals("CASEMATERIALLIST")||
                                    dtc.TableName.ToUpper().Equals("FLOW_DEPOIST")||
                                    dtc.TableName.ToUpper().Equals("FLOW_HOLIDAY")||
                                    dtc.TableName.ToUpper().Equals("FLOW_INSTANCE_LOG")||
                                    dtc.TableName.ToUpper().Equals("FLOW_USER_HISTORY")||
                                    dtc.TableName.ToUpper().Equals("SYSPAGETOOLS"))      
                                                  */ 
                                rtn.Append("   IDENTITY(1,1)");
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
                    int p = pk.LastIndexOf(",");
                    if (p != -1)
                    {
                        pk = pk.Substring(0, p) + ")";//去掉第一个','
                        rtn.Append("\n" + pk);
                        HavePk = true;
                    }
                    bool HaveUnique = false;
                    p = unique.LastIndexOf(",");
                    if (p != -1)
                    {
                        if (HavePk) rtn.Append(",");

                        unique = unique.Substring(0, p) + ")";//去掉第一个','
                        rtn.Append("\n" + unique);
                        HaveUnique = true;
                    }
                    if (!HavePk && !HaveUnique)
                    {//当没有主键和约束的时候，
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

                    //添加字段注释
                    StringBuilder FieldComment = new StringBuilder();
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        if (columnList[i].Comment == null) continue;
                        string FieldName = columnList[i].ColumnName.ToUpper();
                        string Comment = columnList[i].Comment.Replace(";", "；").Replace("'", "''");

                        FieldComment.Append("\n  if EXISTS(SELECT   * " +
                            "FROM   ::fn_listextendedproperty ('MS_DESCRIPTION', 'user', 'dbo', 'table', '" + tbl.TableName.ToUpper() +
                            "', 'column', '" + FieldName + "')) \n " +
                            " exec sp_dropextendedproperty 'MS_DESCRIPTION', 'user', dbo, 'table', '" + tbl.TableName.ToUpper() +
                            "', 'column', '" + FieldName + "' ");

                        FieldComment.Append("\n");
                        FieldComment.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割	
                        FieldComment.Append("\n");
	
                        FieldComment.Append("\n  exec sp_addextendedproperty N'MS_Description', N'" +
                            Comment + "', N'user', N'dbo', N'table', N'" + tbl.TableName.ToUpper() + "', N'column', N'" + FieldName + "'");
                        
                        FieldComment.Append("\n");
                        FieldComment.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割	
                        FieldComment.Append("\n");
                    }
                    rtn.Append(FieldComment.ToString() + "\n ");
                }
            }
            catch
            {
                throw;
            }
            return rtn.ToString();
        }

        /// <summary>
        /// 获取表信息sql
        /// </summary>
        /// <param name="tblname"></param>
        /// <returns></returns>
        public virtual string GetSqlOfTableInfo(string tblname)
        {
            return "select count(*) from dbo.sysobjects where id = object_id(N'[" + tblname + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
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
            if (type.ToUpper().Equals("DECIMAL"))
            {
                if (field.Scale > 0)
                {
                    type += "(" + field.Precision + "," + field.Scale + ")";
                }
            }
            return "ALTER TABLE " + field.TableName + " alter column " + field.ColumnName + " " + type;
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

            if (type.ToUpper().Equals("DECIMAL"))
            {
                if (field.Scale > 0)
                {
                    type += "(" + field.Precision + "," + field.Scale + ")";
                }
            }
            if (field.DefaultValue != null && field.DefaultValue.Length > 0)
            {

                string defaultVal = field.DefaultValue;
                if (String.Compare(defaultVal, "(SYSDATE)", true) == 0)
                {
                    defaultVal = "(GETDATE())";
                }
                type += "  DEFAULT  " + defaultVal + "  ";
            }

            if (field.Nullable)
                type += " null ";
            else
                type += " not null ";

            if (field.Generated != null && (bool)field.Generated)
            {
                type += "  UNIQUE  ";
            }

            return "ALTER TABLE " + field.TableName + " ADD " + field.ColumnName + " " + type;
        }
    }
}
