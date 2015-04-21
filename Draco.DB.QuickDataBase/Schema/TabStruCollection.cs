using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using System.IO;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.QuickDataBase.Schema
{
    /// <summary>
    /// 数据表结构集合
    /// </summary>
    public class TabStruCollection
    {
        /// <summary>
        /// 表信息列表
        /// </summary>
        [XmlArrayItem(typeof(Table))]
        public ArrayList List;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TabStruCollection()
        {
            List = new ArrayList();
        }
        /// <summary>
        /// Save the object in XML format to a stream
        /// </summary>
        /// <param name="s">Stream to save the object to</param>
        public void SaveAsXML(Stream s)
        {
            XmlSerializer sr = new XmlSerializer(this.GetType());
            sr.Serialize(s, this);
        }
        /// <summary>
        /// Save the object in XML format to a stream
        /// </summary>
        /// <param name="s"></param>
        public void SaveAsXML(TextWriter s)
        {
            XmlSerializer sr = new XmlSerializer(this.GetType());
            sr.Serialize(s, this);
        }

        /// <summary>
        /// Create a new bject initialised from XML data
        /// </summary>
        /// <param name="s">Stream to load the XML from</param>
        /// <returns>CommBaseSettings object</returns>
        public static TabStruCollection LoadFromXML(Stream s)
        {
            XmlSerializer sr = new XmlSerializer(typeof(TabStruCollection),
                new Type[] { typeof(Table), typeof(DataTableColumn) });
            return (TabStruCollection)sr.Deserialize(s);
        }

        /// <summary>
        /// Create a new bject initialised from XML data
        /// </summary>
        /// <param name="s">Stream to load the XML from</param>
        /// <returns>CommBaseSettings object</returns>
        public static TabStruCollection LoadFromXML(TextReader s)
        {
            XmlSerializer sr = new XmlSerializer(typeof(TabStruCollection),
                new Type[] { typeof(Table), typeof(DataTableColumn) });
            return (TabStruCollection)sr.Deserialize(s);
        }

        /// <summary>
        /// 根据表结构文档生成协调此连接下表结构Sql语句
        /// </summary>
        /// <returns></returns>
        public string GetFixTableStuSqlByFile(IDataBaseHandler handler)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                Table tbl = (Table)this.List[i];
                List<DataTableColumn> dtcl = tbl.Columns;
                for (int j = 0; j < dtcl.Count; j++)
                {
                    DataTableColumn dtc = dtcl[j];
                    //映射到当前的数据库类型下
                    handler.DbAdapter.GetSchemaHandler(handler).MapDBType(ref dtc);
                }
            }

            int NotExist = 0;
            int NotMatch = 0;
            System.Text.StringBuilder sbSql = new System.Text.StringBuilder();//要执行的Sql
            ArrayList tblList = new ArrayList();//不存在的表信息
            ArrayList BuildList = new ArrayList();//不存在的表名

            #region 对比表结构
            foreach (Table tbl in this.List)
            {
                #region 从数据库表中取得信息
                string chetblExist = null;
                chetblExist = handler.DbAdapter.GetSchemaHandler(handler).GetSqlOfTableInfo(tbl.TableName);
                object existflag = handler.ExecuteScalar(chetblExist);
                if (existflag == null || existflag == DBNull.Value || Convert.ToInt32(existflag) == 0)
                {//表不存在
                    tblList.Add(tbl);
                    BuildList.Add(tbl.TableName);
                    continue;
                }
                //数据库表结构信息
                TableSchemaInOut tsio = new TableSchemaInOut(handler);
                //数据库中的表结构信息
                Table dbtbl = tsio.getOneTableStruByTableName(tbl.TableName);
                MapTableColToDbType(ref dbtbl, handler);//转化到数据库类型

                ArrayList difflist = new ArrayList();//保存字段不一致的字段
                ArrayList NoExistlist = new ArrayList();//保存字段存在的字段
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    DataTableColumn field = tbl.Columns[i];
                    int j = 0;
                    bool IsthisFieldExist = false;//字段是否存在
                    for (; j < dbtbl.Columns.Count; j++)
                    {
                        if (String.Compare(field.ColumnName, dbtbl.Columns[j].ColumnName, true) == 0)
                        {
                            IsthisFieldExist = true;
                            //添加到不一样的字段
                            if (field.SimpleType != dbtbl.Columns[j].SimpleType || field.Length != dbtbl.Columns[j].Length)
                            //当字段类型不匹配
                            {
                                difflist.Add(field);
                            }
                            break;
                        }
                    }
                    //添加到不存在的
                    if (!IsthisFieldExist)
                    {
                        NoExistlist.Add(field);
                    }
                }
                #endregion

                #region 数据类型改变的字段
                NotMatch = difflist.Count;
                if (difflist.Count > 0)
                {
                    foreach (DataTableColumn field in difflist)
                    {
                        sbSql.Append(handler.DbAdapter.GetSchemaHandler(handler).GetAlterSqlOfColumnInfo(field));
                        sbSql.Append(ExecuteSqlFile.SqlSplitChars);
                    }
                }
                #endregion

                #region 新添加的字段
                NotExist = NoExistlist.Count;
                if (NoExistlist.Count > 0)
                {
                    foreach (DataTableColumn field in NoExistlist)
                    {
                        sbSql.Append(handler.DbAdapter.GetSchemaHandler(handler).GetAlterAddSqlOfColumnInfo(field));
                        sbSql.Append(ExecuteSqlFile.SqlSplitChars);
                    }
                }
                #endregion
            }
            #endregion

            #region 不存在的表
            if (BuildList.Count > 0)
            {
                sbSql.Append(handler.DbAdapter.GetSchemaHandler(handler).GetDbServerScript(tblList));
            }
            #endregion
            return sbSql.ToString();
        }

        /// <summary>
        /// 根据表结构文档生成此连接下表结构Sql语句(无协调功能)
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string GetFixTableStuSqlByFile2(IDataBaseHandler handler)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                Table tbl = (Table)this.List[i];
                List<DataTableColumn> dtcl = tbl.Columns;
                for (int j = 0; j < dtcl.Count; j++)
                {
                    DataTableColumn dtc = dtcl[j];
                    //映射到当前的数据库类型下
                    handler.DbAdapter.GetSchemaHandler(handler).MapDBType(ref dtc);
                }
            }
            //相当于全部以新的不存在的数据表添加进去
            return handler.DbAdapter.GetSchemaHandler(handler).GetDbServerScript(this.List);
        }

        /// <summary>
        /// 适应表结构字段到数据库类型
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="handler"></param>
        private void MapTableColToDbType(ref Table tbl,IDataBaseHandler handler)
        {
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                DataTableColumn dtc = tbl.Columns[i];
                handler.DbAdapter.GetSchemaHandler(handler).MapDBType(ref dtc);
            }
        }
    }
}