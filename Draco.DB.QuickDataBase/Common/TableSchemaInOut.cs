using System;
using System.Collections.Generic;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema;
using System.Collections;
using System.IO;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 表结构导入和导出
    /// </summary>
    public class TableSchemaInOut
    {
        private IDataBaseHandler m_handler;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public TableSchemaInOut(IDataBaseHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException();
            m_handler = handler;
        }

        /// <summary>
        /// 获取所有数据库表名
        /// </summary>
        /// <returns></returns>
        public List<string> getAllTableNames()
        {
            return this.m_handler.DbAdapter.GetSchemaHandler(m_handler).GetTableNames();
        }

        /// <summary>
        /// 根据表名获取表结构的完整信息(数据库无关)
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public Table getOneTableStruByTableName(string tablename)
        {
            Table tarTable = new Table();
            tarTable.TableName = tablename;
            IList<IDataTableColumn> cilist = this.m_handler.DbAdapter.GetSchemaHandler(m_handler).ReadColumns(tablename);
            List<DataTableColumn> clist = new List<DataTableColumn>();
            for (int i = 0; i < cilist.Count; i++)
            {
                DataTableColumn dtc = new DataTableColumn();
                IDataTableColumn idtc = cilist[i];

                dtc.ColumnName = idtc.ColumnName;
                dtc.TableName = idtc.TableName;
                dtc.TableSchema = idtc.TableSchema;
                dtc.PrimaryKey = idtc.PrimaryKey;
                dtc.DefaultValue = idtc.DefaultValue;
                dtc.Generated = idtc.Generated;
                dtc.Comment = idtc.Comment;

                dtc.SimpleType = idtc.SimpleType;
                dtc.Type = idtc.Type;
                dtc.Nullable = idtc.Nullable;
                dtc.Length = idtc.Length;
                dtc.Precision = idtc.Precision;
                dtc.Scale = idtc.Scale;
                dtc.Unsigned = idtc.Unsigned;
                dtc.FullType = idtc.FullType;

                clist.Add(dtc);
            }

            for (int i = 0; i < clist.Count; i++)
            {
                IDataTableColumn idc = clist[i];
                this.MapSimpleType(ref idc);
            }
            tarTable.Columns = clist;
            return tarTable;
        }

        /// <summary>
        /// 映射到与数据库无关的类型
        /// </summary>
        /// <param name="Column"></param>
        public void MapSimpleType(ref IDataTableColumn Column)
        {
            IDataType dt = new DataType();

            //公共字段
            dt.SimpleType = Column.SimpleType;
            //辅助字段
            dt.Type = Column.Type;
            dt.Nullable = Column.Nullable;
            dt.Length = Column.Length;
            dt.Precision = Column.Precision;
            dt.Scale = Column.Scale;
            dt.Unsigned = Column.Unsigned;
            dt.FullType = Column.FullType;

            Column.SimpleType = this.MapSimpleType(dt);
        }

        /// <summary>
        /// 映射到与数据库无关的类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string MapSimpleType(IDataType dataType)
        {
            return this.m_handler.DbAdapter.GetSchemaHandler(m_handler).MapAdoType(dataType).ToString();
        }

        /// <summary>
        /// 获取全部表结构List(数据库无关)
        /// </summary>
        /// <returns></returns>
        public List<Table> getAllTableStru()
        {
            List<Table> tlist = new List<Table>();
            List<string> tnamelist = this.getAllTableNames();

            if (tnamelist == null)
                return tlist;

            for (int i = 0; i < tnamelist.Count; i++)
            {
                Table tb = this.getOneTableStruByTableName(tnamelist[i]);
                tlist.Add(tb);
            }
            return tlist;
        }

        /// <summary>
        /// 获取指定表结构List
        /// </summary>
        /// <returns></returns>
        public List<Draco.DB.QuickDataBase.Schema.Vendor.Implementation.Table> getSomeTableStru(List<string> tblnamelist)
        {
            List<Table> tlist = new List<Table>();
            List<string> tnamelist = tblnamelist;

            if (tnamelist == null)
                return tlist;

            for (int i = 0; i < tnamelist.Count; i++)
            {
                Table tb = this.getOneTableStruByTableName(tnamelist[i]);
                tlist.Add(tb);
            }
            return tlist;
        }

        /// <summary>
        /// 获取全部待序列化的表结构集合
        /// </summary>
        /// <returns></returns>
        public TabStruCollection getTabStruAllCollection()
        {
            List<Table> tlist = this.getAllTableStru();
            TabStruCollection tsc = new TabStruCollection();
            for (int i = 0; i < tlist.Count; i++)
            {
                Table tb = tlist[i];
                tsc.List.Add(tb);
            }
            return tsc;
        }

        /// <summary>
        /// 获取部分待序列化的表结构集合
        /// </summary>
        /// <returns></returns>
        public TabStruCollection getTabStruSomeCollection(List<string> tblnamelist)
        {
            List<Table> tlist = this.getSomeTableStru(tblnamelist);
            TabStruCollection tsc = new TabStruCollection();
            for (int i = 0; i < tlist.Count; i++)
            {
                Table tb = tlist[i];
                tsc.List.Add(tb);
            }
            return tsc;
        }

        /// <summary>
        /// 从配置文件中读取表结构数据到表结构集合中（反序列化）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public TabStruCollection LoadDataFromXMLToTabStruCollection(Stream s)
        {
            return TabStruCollection.LoadFromXML(s);
        }

        /// <summary>
        /// 从配置文件中读取表结构数据到表结构集合中（反序列化）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public TabStruCollection LoadDataFromXMLToTabStruCollection(TextReader s)
        {
            return TabStruCollection.LoadFromXML(s);
        }

        /// <summary>
        /// 导入表结构（未协调）
        /// </summary>
        /// <param name="filename"></param>
        public void tableSchemaInput2(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("无法找到文件：" + filename);
            }
            StreamReader fs = new StreamReader(filename);
            TabStruCollection tsc = this.LoadDataFromXMLToTabStruCollection(fs);
            string sql = tsc.GetFixTableStuSqlByFile2(this.m_handler);

            fs.Dispose();
            ExecuteSqlFile esf = new ExecuteSqlFile(this.m_handler);
            esf.ExecuteSqlScript(sql, ExecuteSqlFile.SqlSplitChars);
        }

        /// <summary>
        /// 协调导入表结构
        /// </summary>
        /// <param name="filename"></param>
        public void tableSchemaInput(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("无法找到文件：" + filename);
            }
            StreamReader fs = new StreamReader(filename);
            TabStruCollection tsc = this.LoadDataFromXMLToTabStruCollection(fs);
            string sql = tsc.GetFixTableStuSqlByFile(this.m_handler);
            fs.Dispose();
            ExecuteSqlFile esf = new ExecuteSqlFile(this.m_handler);
            esf.ExecuteSqlScript(sql, ExecuteSqlFile.SqlSplitChars);
        }

        /// <summary>
        /// 导出全部表结构
        /// </summary>
        /// <param name="filename"></param>
        public void tableSchemaOutput(string filename)
        {
            if (!File.Exists(filename))
                File.Create(filename);
            if (filename.Trim().Length == 0)
                return;
            StreamWriter fs = new StreamWriter(filename);
            TabStruCollection tsc = this.getTabStruAllCollection();
            tsc.SaveAsXML(fs);
            fs.Dispose();
        }

        /// <summary>
        /// 导出部分表结构
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tblnamelist"></param>
        public void tableSchemaSomeOutput(string filename, List<string> tblnamelist)
        {
            if (!File.Exists(filename))
                File.Create(filename);
            if (filename.Trim().Length == 0)
                return;
            StreamWriter fs = new StreamWriter(filename);
            TabStruCollection tsc = this.getTabStruSomeCollection(tblnamelist);
            tsc.SaveAsXML(fs);
            fs.Dispose();
        }
    }
}
