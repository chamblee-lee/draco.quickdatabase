using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase.SqlClient;

namespace Draco.DB.QuickDataBase.SqlCeClient
{
    class SqlCeSchemaHandler : SqlSchemaHandler
    {
        private IDataBaseHandler m_DBHelper = null;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public SqlCeSchemaHandler(IDataBaseHandler Handler): base(Handler)
        {
            m_DBHelper = Handler;
        }

        /// <summary>
        /// 获取单表的数据字段信息
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <returns></returns>
        public override IList<IDataTableColumn> ReadColumns(string TableName)
        {

            IList<IDataTableColumn> list = new List<IDataTableColumn>();

            IDataTableColumn Col = new DataTableColumn();
            Col.SimpleType = "0"; //默认的公共类型

            Col.ColumnName = "字段名";
            Col.Nullable = true;
            Col.TableName = TableName;
            Col.TableSchema = TableName;
            Col.Type = "类型";


            Col.Length = 8;
            Col.Precision = 0;//整数长度
            Col.Scale = 0;

            Col.FullType = Col.Type;
            Col.DefaultValue = "默认值";

            Col.Comment = "注释";
            list.Add(Col);

            return list;

        }

        /// <summary>
        /// 判断是否是自增列(SqlserverCe)
        /// </summary>
        /// <param name="TblName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public override bool IsAotuAddColumn(string TblName, string keyName)
        {
            if(TblName.ToUpper().Equals("TMPLABELTODBFIELD")||
                TblName.ToUpper().Equals("UPFILESLIST") ||
                TblName.ToUpper().Equals("CASEMATERIALLIST") ||
                TblName.ToUpper().Equals("FLOW_DEPOIST") ||
                TblName.ToUpper().Equals("FLOW_HOLIDAY") ||
                TblName.ToUpper().Equals("FLOW_INSTANCE_LOG") ||
                TblName.ToUpper().Equals("FLOW_USER_HISTORY") ||
                TblName.ToUpper().Equals("SYSPAGETOOLS") ) 
                return true;
            else        
                return false;
        }

        /// <summary>
        /// 映射到DB类型
        /// </summary>
        /// <param name="datatablecolumn"></param>
        /// <returns></returns>
        public override void MapDBType(ref DataTableColumn datatablecolumn)
        {
            string typeid = datatablecolumn.SimpleType;
            switch (typeid)
            {
                //adBigInt
                case "20":
                    {
                        datatablecolumn.SimpleType = "bigint";
                        datatablecolumn.Length = 0;
                        return;
                    }

                //adSmallInt
                case "2":
                    {
                        datatablecolumn.SimpleType = "int";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adInteger
                case "3":
                    {                  
                        datatablecolumn.SimpleType = "int";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adNumeric
                case "131":
                    {
                        datatablecolumn.SimpleType = "float";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adDouble
                case "5":
                    {
                        datatablecolumn.SimpleType = "float";
                        datatablecolumn.Length = 0;
                        return;
                    }

                //adChar
                case "129":
                    {  
                        datatablecolumn.SimpleType = "nvarchar";
                        return;
                    }
                //adDBTimeStamp  
                case "135":
                    {
                        datatablecolumn.SimpleType = "datetime";
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adLongVarChar
                case "201":
                    {                       
                        datatablecolumn.SimpleType = "ntext";                       
                        datatablecolumn.Length = 0;
                        return;
                    }
                //adBinary
                case "128":
                    {
                        datatablecolumn.SimpleType = "image";
                        datatablecolumn.Length = 0;
                        return;
                    }
                default:
                    {
                        datatablecolumn.SimpleType = "int";
                        datatablecolumn.Length = 0;
                        return;
                    }
            }
        }

        public override string GetDbServerScript(ArrayList List)
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
                }
            }
            catch
            {
                throw;
            }
            return rtn.ToString();
        }
    }
}
