using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Schema;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;

namespace Draco.DB.QuickDataBase.OracleClient
{
    /// <summary>
    /// Oracle数据库结构查询
    /// </summary>
    public class OracleSchemaHandler : IDataBaseSchemaHandler
    {
        private IDataBaseHandler m_DBHelper = null;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public OracleSchemaHandler(IDataBaseHandler Handler)
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
            // note: the ReadDataNameAndSchema relies on information order
            string SQL = @" SELECT table_name FROM user_tables order by table_name ";
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
        public IList<IDataTableColumn> ReadColumns(string TableName)
        {
            string SQL = @"SELECT table_name, column_name, data_type, data_length, data_precision, data_scale, nullable, data_default
                            FROM user_tab_columns
                            WHERE lower(table_name) = :tableName
                            ORDER BY column_id";
            IDataParameter para2 = m_DBHelper.DbAdapter.CreateParameter(":tableName", TableName.ToLower());
            DataSet ds = m_DBHelper.ExecuteQuery(SQL, para2);
            Hashtable hs = GetColumnComment(TableName);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                IList<IDataTableColumn> list = new List<IDataTableColumn>();
                //唯一的自增值
                bool isGenerated = true;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    IDataTableColumn Col = new DataTableColumn();
                    Col.SimpleType = "0"; //默认的公共类型

                    Col.ColumnName = Convert.ToString(row["column_name"]);
                    Col.Nullable = Convert.ToString(row["nullable"]) == "Y";
                    Col.TableName = TableName;
                    Col.TableSchema = TableName;

                    Col.DefaultValue = Convert.ToString(row["data_default"]);

                    Col.Type = Convert.ToString(row["data_type"]);
                    Col.FullType = Col.Type;

                    string odata_length = Convert.ToString(row["data_length"]);
                    string odata_precision = Convert.ToString(row["data_precision"]);
                    string odata_scale = Convert.ToString(row["data_scale"]);

                    int x = 0;
                    if (Int32.TryParse(odata_length, out x))
                        Col.Length = x;
                    if (Int32.TryParse(odata_precision, out x))
                        Col.Precision = x;
                    if (Int32.TryParse(odata_scale, out x))
                        Col.Scale = x;

                    DataRow[] rows = AllConstraint.Select(String.Format("table_name='{0}' AND column_name='{1}' AND constraint_type='P'", TableName, Col.ColumnName));
                    if (rows != null && rows.Length > 0)
                    {
                        Col.PrimaryKey = true;
                        if (Col.Type == "NUMBER" || Col.Type == "INTEGER")
                        {
                            /*
                            //针对需要触发器字段的特殊处理
                            if (Col.TableName.ToUpper().Equals("TMPLABELTODBFIELD") ||
                                    Col.TableName.ToUpper().Equals("UPFILESLIST") ||
                                    Col.TableName.ToUpper().Equals("CASEMATERIALLIST") ||
                                    Col.TableName.ToUpper().Equals("FLOW_DEPOIST") ||
                                    Col.TableName.ToUpper().Equals("FLOW_HOLIDAY") ||
                                    Col.TableName.ToUpper().Equals("FLOW_INSTANCE_LOG") ||
                                    Col.TableName.ToUpper().Equals("FLOW_USER_HISTORY") ||
                                    Col.TableName.ToUpper().Equals("SYSPAGETOOLS"))   
                            
                                Col.Generated = true;
                             */
                            if (this.IsHaveSequenceOra(Col.TableName) && isGenerated)
                            {
                                Col.Generated = true;
                                isGenerated = false;
                            }
                        }
                    }
                    if (hs != null && hs[Col.ColumnName] != null)//注释
                        Col.Comment = hs[Col.ColumnName].ToString();
                    list.Add(Col);
                }
                return list;
            }
            return null;
        }
        #region 加载所有表约束
        private DataTable LoadAllConstraint()
        {
            string SQL = @"SELECT UCC.owner, UCC.constraint_name, UCC.table_name, UCC.column_name, UC.constraint_type, UC.R_constraint_name
                        FROM user_cons_columns UCC, user_constraints UC
                        WHERE UCC.constraint_name=UC.constraint_name
                        AND UCC.table_name=UC.table_name
                        AND UC.CONSTRAINT_TYPE!='C'";
            DataSet ds = m_DBHelper.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }
        private DataTable m_Constraint;
        private DataTable AllConstraint
        {
            get
            {
                if (m_Constraint == null)
                {
                    m_Constraint = LoadAllConstraint();
                }
                return m_Constraint;
            }
        }
        #endregion
        /// <summary>
        /// 获取表中所有字段的注释
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        protected Hashtable GetColumnComment(string TableName)
        {
            string SQL = " SELECT  *  FROM USER_COL_COMMENTS WHERE table_name='{0}'";
            SQL = String.Format(SQL, TableName);
            DataSet ds = m_DBHelper.ExecuteQuery(SQL);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Hashtable hs = new Hashtable();
                foreach (DataRow row in ds.Tables[0].Rows)
                    hs.Add(Convert.ToString(row["COLUMN_NAME"]), Convert.ToString(row["COMMENTS"]));
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
                strFldDes = System.Text.RegularExpressions.Regex.Replace(
                     strFldDes, @"nvarchar", @"nvarchar2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                strFldDes = strFldDes.Replace("nvarchar22", "nvarchar2");
                strSQL = "alter table " + strTblName + " add  \"" + strFldName.ToUpper() + "\"  " + strFldDes;
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
            string Sql = "select comments from user_tab_comments where table_name = '"
                + TableName.Replace("'", "''") + "'";

            object obj = m_DBHelper.ExecuteScalar(Sql);
            if (obj == null)
                return null;
            return obj.ToString();
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

            string Sql = "select  COMMENTS FROM USER_COL_COMMENTS where " +
                "table_name='" + TableName.Replace("'", "''") +
                "' and COLUMN_NAME = '" + ColumnName.Replace("'", "''") + "'";
            object obj = m_DBHelper.ExecuteScalar(Sql);
            if (obj != null && obj != DBNull.Value)
            {
                return obj.ToString();
            }
            else
                return "";
        }
        /// <summary>
        /// 查询数据字段的数据类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <returns>数据类型</returns>
        public string GetDataTypeByColumnName(string tableName, string columnName)
        {
            string strSql = string.Format("select DATA_TYPE from all_tab_cols where table_name='{0}' and COLUMN_NAME='{1}'", tableName, columnName);
            DataSet set = m_DBHelper.ExecuteQuery(strSql);
            if ((set.Tables.Count == 1) && (set.Tables[0].Rows.Count == 1))
            {
                return Convert.ToString(set.Tables[0].Rows[0][0]);
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
            switch (type.ToUpper())
            {
                case "NUMBER": return DbType.Int32;
                case "DATE": return DbType.DateTime;
                case "BLOB": return DbType.Binary;
                case "CLOB": return DbType.String;
                case "BFILE": return DbType.Binary;
                case "CHAR": return DbType.String;
                case "LONGRAW": return DbType.Binary;
                case "NCHAR": return DbType.String;
                case "NCLOB": return DbType.Binary;
                case "VARCHAR2": return DbType.String;
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
            string SeqName = "SEQ_" + TableName;
            string strSql = "select count(*) from user_sequences  where SEQUENCE_NAME = :pSeqName ";
            IDataParameter pSeqName = m_DBHelper.DbAdapter.CreateParameter("pSeqName", SeqName);
            //判断是否有序列
            object o = m_DBHelper.ExecuteScalar(strSql, pSeqName);
            if (o == null || o == DBNull.Value)
                return false;
            else
                return Convert.ToInt32(o, System.Globalization.NumberFormatInfo.CurrentInfo) == 1;
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

            string strSql = "select DATA_DEFAULT " +
                "	from user_tab_columns where  " +
                "TABLE_NAME = '" + tblName.Replace("'", "''") +
                "' and  COLUMN_NAME = '" + ColumnName.Replace("'", "''") + "'";
            object o = m_DBHelper.ExecuteScalar(strSql);
            if (o == null || o == DBNull.Value)
                return null;
            else
                return o.ToString();
        }
        /// <summary>
        /// 是否是自增长列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public bool IsAotuAddColumn(string TableName, string ColumnName)
        {
            return false;
        }
        /// <summary>
        /// 取得表中序列(SEQ_ + TblName)的下一个值
        /// </summary>
        /// <param name="TblName"></param>
        /// <returns></returns>
        public long GetSEQColumNextValue(string TblName)
        {
            string SelectSql = "Select SEQ_" + TblName + ".nextval from DUAL";
            object o;
            o = m_DBHelper.ExecuteScalar(SelectSql);
            if (o == null || o == DBNull.Value)
                return 1;
            else
                return Convert.ToInt64(o, System.Globalization.NumberFormatInfo.CurrentInfo);
        }
        /// <summary>
        /// 同步序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="PKFieldName"></param>
        public void SynchSequence(string TableName, string PKFieldName)
        {
            //求最大值
            string sql = string.Format("select max({0}) from {1}", PKFieldName, TableName);
            long max = 0;
            object o = m_DBHelper.ExecuteScalar(sql);
            if (o != null && o != DBNull.Value)
                Int64.TryParse(o.ToString(), out max);

            long NextValue = max + 1; //这个是数据表的下一个主键值

            string SeqName = "SEQ_" + TableName.ToUpper();
            string SelectSeq = "SELECT count(*) FROM USER_SEQUENCES where SEQUENCE_NAME='" + SeqName + "'";
            object count = m_DBHelper.ExecuteScalar(SelectSeq);
            if (Convert.ToInt32(count) == 1)
            {
                //序列存在,删除序列
                string DropSQL = "drop sequence " + SeqName;
                m_DBHelper.ExecuteNonQuery(DropSQL);
            }
            //重新创建序列
            string CreateSeq = String.Format("create sequence {0} minvalue 1 maxvalue 999999999999999 start with {1} increment by 1 cache 20 cycle ",
                SeqName, NextValue);
            m_DBHelper.ExecuteNonQuery(CreateSeq);
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
                    return (int)ADOType.adBoolean;
                case "INT":
                case "INTEGER":
                case "SMALLINT":
                    return (int)ADOType.adInteger;
                case "FLOAT":
                    return (int)ADOType.adSingle;
                case "DECIMAL":
                case "NUMBER":
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
                case "VARCHAR2":
                case "NVARCHAR":
                case "NVARCHAR2":
                    return (int)ADOType.adChar;
                case "TEXT":
                case "NTEXT":
                case "CLOB":
                case "NCLOB":
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
                //adDouble
                case "5":
                //adNumeric
                case "131":
                    {
                        datatablecolumn.SimpleType = "NUMBER";
                        if (datatablecolumn.Scale > 0)
                        {}
                        else
                            datatablecolumn.Length = 0;
                        return;
                    }
                //adChar
                case "129":
                    {
                        datatablecolumn.SimpleType = "VARCHAR2";
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
                        datatablecolumn.SimpleType = "CLOB";
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
                        datatablecolumn.SimpleType = "NUMBER";
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
                return "";

            StringBuilder rtn = new StringBuilder();

            try
            {
                foreach (Table table in List)
                {
                    Table tbl = table;
                    List<DataTableColumn> columnList = tbl.Columns;
                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");

                    rtn.Append("declare \n");
                    rtn.Append("	n   number;\n");
                    rtn.Append("begin \n");
                    rtn.Append("	select count(*) into n from user_tables where table_name='" + tbl.TableName.ToUpper() + "'; \n");
                    rtn.Append("	if n>0 then \n");
                    rtn.Append("		execute immediate 'drop table '||'" + tbl.TableName.ToUpper() + "'; \n");
                    rtn.Append("	end if; \n");
                    rtn.Append("end;\n");

                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");

                    rtn.Append("CREATE TABLE \"" + tbl.TableName.ToUpper() + "\" (");
                    string pk = "PRIMARY KEY (";
                    string unique = "UNIQUE (";
                    string AutoAddSEQ = "";
                    string IdentityName = null;

                    //开始添加每个字段
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        DataTableColumn dtc = columnList[i];
                        string FieldName = dtc.ColumnName.ToUpper();
                        rtn.Append("\"" + FieldName + "\"   ");

                        long? FidldLength = dtc.Length;
                        int? Precision = dtc.Precision;
                        int? Scale = dtc.Scale;

                        if (FidldLength > 0)
                        {
                            if (Scale > 0)
                            {
                                rtn.Append(dtc.SimpleType + "(" + Precision.ToString() + "," + Scale + ")");
                            }
                            else
                            {
                                rtn.Append(dtc.SimpleType + "(" + FidldLength.ToString() + ")");
                            }
                        }
                        else
                        {
                            rtn.Append(dtc.SimpleType);
                        }

                        if (dtc.DefaultValue != null && dtc.DefaultValue.Trim().Length > 0)
                        {
                            string defaultVal = dtc.DefaultValue;
                            if (String.Compare(defaultVal, "(GETDATE())", true) == 0)
                            {
                                defaultVal = "(SYSDATE)";
                            }
                            rtn.Append("  DEFAULT  " + defaultVal + "  ");
                        }
                        if (dtc.PrimaryKey != null && (bool)dtc.PrimaryKey)
                        {
                                pk += "\"" + FieldName + "\" , ";
                        }
                        if (!dtc.Nullable)
                        {
                            rtn.Append("   NOT NULL ");
                        }
                        if (dtc.Generated != null && (bool)dtc.Generated)
                        {
                            if (dtc.PrimaryKey != null && (bool)dtc.PrimaryKey)
                            { }
                            else
                            {
                                unique += "\"" + FieldName + "\" , ";
                            }
                            IdentityName = dtc.ColumnName;
                            AutoAddSEQ = "create sequence SEQ_" + tbl.TableName.ToUpper() + " \n" +
                                " minvalue 1 \n" +
                                " maxvalue 999999999999999999999999999 \n" +
                                " start with 1 \n" +
                                " increment by 1 \n" +
                                " cache 20";
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

                    if (AutoAddSEQ != null)
                    {
                        //所有的Oracle表产生新的主键值时全用序列
                        rtn.Append("declare \n" +
                            "	n   number;\n" +
                            "begin \n" +
                            "	select count(*) into n from user_sequences  where SEQUENCE_NAME = 'SEQ_" + tbl.TableName.ToUpper() + "'; \n" +
                            "	if n>0 then \n" +
                            "		execute immediate ' DROP SEQUENCE '||'SEQ_" + tbl.TableName.ToUpper() + "'; \n" +
                            "	end if;  \n" +
                            "end;");
                        rtn.Append("\n");
                        rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                        rtn.Append("\n");

                        rtn.Append(AutoAddSEQ);

                        rtn.Append("\n");
                        rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                        rtn.Append("\n");

                        if (IdentityName != null && IdentityName.Trim().Length > 0)
                        {
                            rtn.Append("\n");
                            rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                            rtn.Append("\n");

                            rtn.Append(CreateTrigerSqlOfTable(tbl.TableName, IdentityName));
                        }
                    }

                    rtn.Append("\n");
                    rtn.Append(ExecuteSqlFile.SqlSplitChars);//添加分割字符串以供执行时分割
                    rtn.Append("\n");

                    //添加注释
                    StringBuilder FieldComment = new StringBuilder();
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        if (columnList[i].Comment == null) continue;
                        string FieldName = columnList[i].ColumnName.ToUpper();
                        string Comment = columnList[i].Comment.Replace(";", "；").Replace("'", "''");

                        FieldComment.Append("\n comment on column \"" + tbl.TableName.ToUpper() + "\".\"" + FieldName.ToUpper() + "\"" +
                            "  is '" + Comment + "'");

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
        /// 创建表主键的自增触发器
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static string CreateTrigerSqlOfTable(string tblName, string keyName)
        {
            if (tblName == null || tblName.Length == 0)
                return null;
            tblName = tblName.ToUpper();
            StringBuilder sb = new StringBuilder();

            string SEQName = "SEQ_" + tblName;

            string TriName = "TRI_" + tblName;
            string checkTri = "declare n   number;\n" +
                "begin \n" +
                "	select count(*) into n from user_triggers  where trigger_Name = '" + TriName + "'; \n" +
                "	if n>0 then \n" +
                "		execute immediate ' DROP trigger '||'" + TriName + "'; \n" +
                "	end if;  \n" +
                "end;";
            string createTri = "		create trigger " + TriName + " \n" +
                "		before insert on " + tblName + "\n" +
                "		referencing new as new_value\n" +
                "		for each row\n" +
                "		begin\n" +
                "		if :new_value." + keyName + " is null then\n" +
                "			select " + SEQName + ".nextval into :new_value." + keyName + " from dual;\n" +
                "		end if;\n" +
                "		end;";
            sb.Append(checkTri);
            sb.Append("\n");
            sb.Append(ExecuteSqlFile.SqlSplitChars);
            sb.Append("\n");

            sb.Append(createTri);

            sb.Append("\n");
            sb.Append(ExecuteSqlFile.SqlSplitChars);
            sb.Append("\n");
            return sb.ToString();
        }

        /// <summary>
        /// 获取表信息sql
        /// </summary>
        /// <param name="tblname"></param>
        /// <returns></returns>
        public string GetSqlOfTableInfo(string tblname)
        {
            return "	select count(*)  from user_tables where table_name='" + tblname.ToUpper() + "'";
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
            if (type.ToUpper().Equals("VARCHAR2"))
                type += "(" + len + ")";
            if (type.ToUpper().Equals("NUMBER"))
            {
                if (field.Scale > 0)
                {
                    type += "(" + field.Precision + "," + field.Scale + ")";
                }
            }
            return "ALTER TABLE " + field.TableName + " MODIFY( " + field.ColumnName + " " + type + ")";
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
            if (type.ToUpper().Equals("VARCHAR2"))
                type += "(" + len + ")";
            if (type.ToUpper().Equals("NUMBER"))
            {
                if (field.Scale > 0)
                {
                    type += "(" + field.Precision + "," + field.Scale + ")";
                }
            }
            if (field.DefaultValue != null && field.DefaultValue.Length > 0)
            {

                string defaultVal = field.DefaultValue;
                if (String.Compare(defaultVal, "(GETDATE())", true) == 0)
                {
                    defaultVal = "(SYSDATE)";
                }
                type += "  DEFAULT  " + defaultVal + "  ";
            }
            if (field.Generated!=null && (bool)field.Generated)
            {
                type += "  UNIQUE  ";
            }
            if (field.Nullable)
                type += " null ";
            else
                type += " not null ";

            return "ALTER TABLE " + field.TableName + " ADD( " + field.ColumnName + " " + type + ")";
        }

       /// <summary>
        ///  判断是否有序列
       /// </summary>
       /// <param name="TblName"></param>
       /// <returns></returns>
        private bool IsHaveSequenceOra(string TblName)
        {
            string SeqName = "SEQ_" + TblName;
            string strSql = "select count(*) from user_sequences  where SEQUENCE_NAME = :pSeqName ";
            IDataParameter pSeqName = m_DBHelper.DbAdapter.CreateParameter("pSeqName", SeqName);
            //判断是否有序列
            object o;
            o = this.m_DBHelper.ExecuteScalar(strSql, pSeqName);
            if (o == null || o == DBNull.Value)
                return false;
            else
                return Convert.ToInt32(o, System.Globalization.NumberFormatInfo.CurrentInfo) == 1;
        }
    }
}
