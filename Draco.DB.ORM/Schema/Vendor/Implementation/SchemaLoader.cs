#region MIT license
// 
// Copyright (c) 2007-2008 Jiri Moudry
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using Draco.DB.ORM.Schema.Dbml;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Utility;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.ORM.Schema.Vendor.Implementation
{
    /// <summary>
    /// 构架加载器基类
    /// </summary>
    public abstract partial class SchemaLoader : ISchemaLoader
    {
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public abstract string DataServerType { get; }

        IDataBaseHandler m_handler;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        protected SchemaLoader(IDataBaseHandler handler)
        {
            this.m_handler=handler;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="contextNamespace"></param>
        /// <param name="entityNamespace"></param>
        /// <returns></returns>
        public virtual Database Load(string contextNamespace, string entityNamespace)
        {
            return Load(contextNamespace, entityNamespace,null);
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="contextNamespace"></param>
        /// <param name="entityNamespace"></param>
        /// <param name="TargetTableNames"></param>
        /// <returns></returns>
        public virtual Database Load(string contextNamespace, string entityNamespace,string[] TargetTableNames)
        {
            Database schema = new Database();
            schema.Name = "schemaName";//采用数据库实例名称
            schema.Class = "AutoDataBaseContext";
            schema.BaseType = typeof(Object).FullName;
            schema.ContextNamespace = contextNamespace;
            schema.EntityNamespace = entityNamespace;
            schema.Provider = DataServerType;

            // order is important, we must have:
            // 1. tables
            // 2. columns
            // 3. constraints
            LoadTables(schema, TargetTableNames);//加载表
            LoadColumns(schema);//加载列

            // generate backing fields name (since we have here correct names)
            GenerateStorageFields(schema);                                              //生成存储字段

            return schema;
        }

        /// <summary>
        /// 加载表
        /// </summary>
        /// <param name="schema"></param>
        protected virtual void LoadTables(Database schema)
        {
            LoadTables(schema,null);
        }
         /// <summary>
        /// 读取数据表构架
        /// </summary>
        /// <returns></returns>
        protected virtual IList<String> ReadTables()
        {
            return m_handler.DbAdapter.GetSchemaHandler(m_handler).GetTableNames();
        }
        /// <summary>
        /// 读取数据列信息
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        protected virtual IList<IDataTableColumn> ReadColumns(string TableName)
        {
            return m_handler.DbAdapter.GetSchemaHandler(m_handler).ReadColumns(TableName);
        }
        /// <summary>
        /// 加载表
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="TableNames"></param>
        protected virtual void LoadTables(Database schema,string[] TableNames)
        {
            var tables = ReadTables();
            if (tables != null)
            {
                foreach (var row in tables)
                {
                    bool add = false;
                    if (TableNames != null && TableNames.Length > 0)
                    {
                        foreach (string tarName in TableNames)
                        {
                            if (String.Compare(row, tarName, true) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                    else
                    {
                        add = true;
                    }

                    if (add)
                    {
                        var table = new Table();
                        table.Name = row;
                        table.Member = row;
                        table.Type.Name = row;
                        schema.Tables.Add(table);
                    }
                }
            }
        }

        /// <summary>
        /// 加载列
        /// </summary>
        /// <param name="schema"></param>
        protected virtual void LoadColumns(Database schema)
        {
            if (schema.Table == null) return;
            foreach (var table in schema.Table)
            {
                string TableName = table.Name;
                var columnRows = ReadColumns(TableName);
                foreach (var columnRow in columnRows)
                {
                    Column column = new Column();
                    column.Name = columnRow.ColumnName;                 //字段名称
                    column.Member = GeneratePropertyName(columnRow.ColumnName);
                    column.DbType = columnRow.FullType;                 //数据类型(数据库类型相关)

                    if (columnRow.PrimaryKey.HasValue)                  //主键
                        column.IsPrimaryKey = columnRow.PrimaryKey.Value;

                    if (columnRow.Generated.HasValue)                   //自动
                        column.IsDbGenerated = columnRow.Generated.Value;

                    if (column.IsDbGenerated)                           //自动
                        column.Expression = columnRow.DefaultValue;

                    column.CanBeNull = columnRow.Nullable;              //是否可以为空

                    if (columnRow.Length.HasValue)                      //字段长度
                        column.FieldLength = columnRow.Length.Value;
                    column.ADOType = MapAdoType(columnRow);             //映射到ADO类型
                    var columnType = MapDotNETType(column.ADOType);//映射到.Net类型
                    column.Type = columnType.ToString();
                    column.Comment = columnRow.Comment;                 //注释

                    table.Type.Columns.Add(column);
                    /*关于数据类型的处理
                     * 现有各种加载器把数据库特有的数据类型映射到ADO数据类型
                     * 然后把ADO数据类型映射到.NET数据类型
                     * 这样可以保证不同的数据库构架会产生相同的.NET代码
                     * */
                }
            }
        }

        #region 抽象方法
        /// <summary>
        /// 从构架创建表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="SchemaProvider"></param>
        /// <returns></returns>
        protected abstract bool CreateTableFromSchemal(Table table, string SchemaProvider); 
        /// <summary>
        /// 把数据类型映射到ADO数据类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected abstract int MapAdoType(IDataType dataType);
        #endregion

        #region 虚方法
        /// <summary>
        /// 映射到.NET类型
        /// </summary>
        /// <param name="AdoType"></param>
        /// <returns></returns>
        protected virtual System.Type MapDotNETType(int AdoType)
        {
            return DataTypeConvert.ConvertToNETFxType((ADOType)AdoType);
        }
        /// <summary>
        /// 生成存储字段名称
        /// </summary>
        /// <param name="schema"></param>
        protected virtual void GenerateStorageFields(Database schema)
        {
            foreach (var table in schema.Tables)
            {
                foreach (var column in table.Type.Columns)
                {
                    column.Storage = "_" + column.Member;
                }
            }
        }
        /// <summary>
        /// 生成属性名称
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected virtual string GeneratePropertyName(string Name)
        {
            string strName = CharacterHelper.ConvertToLetter(Name);
            if (strName[0].CompareTo(strName.ToUpper()[0]) != 0)
            {
                //不是以大写字母开头的，转换一把
                strName = strName.ToUpper()[0] + strName.Substring(1);
            }
            return strName;
        }
        #endregion

        /// <summary>
        /// 确认填充ADOType值
        /// </summary>
        /// <param name="schema"></param>
        public void EnSureADOType(Database schema)
        {
            if (schema != null && schema.Tables != null)
            {
                foreach (var tbl in schema.Tables)
                {
                    if (tbl.Type.Columns != null)
                    {
                        foreach (var Col in tbl.Type.Columns)
                        {
                            var Column = new DataTableColumn();
                            Column.Type = Col.DbType;
                            Col.ADOType = MapAdoType(Column);
                        }
                    }
                }
            }
        }
    }
}
