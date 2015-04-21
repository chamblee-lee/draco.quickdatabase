using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping.AttrMapping
{
    /// <summary>
    /// 表映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class TableMappingAttribute : Attribute 
    {
        private TableMapping m_Mapping = new TableMapping();

        /// <summary>
        /// 类名称
        /// </summary>
        public string TypeName
        {
            get { return m_Mapping.TypeName; }
            set { m_Mapping.TypeName = value; }
        }
        /// <summary>
        /// 主键生成器
        /// </summary>
        public string KeyGenerator
        {
            get { return m_Mapping.Generator; }
            set { m_Mapping.Generator = value; }
        }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get { return m_Mapping.TableName; }
            set { m_Mapping.TableName = value; }
        }
        /// <summary>
        /// 是表
        /// </summary>
        public bool IsTable
        {
            get { return m_Mapping.IsTable; }
            set { m_Mapping.IsTable = value; }
        }
        /// <summary>
        /// 构造
        /// </summary>
        public TableMappingAttribute()
        { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="className"></param>
        /// <param name="tableName"></param>
        public TableMappingAttribute(string className, string tableName)
        {
            m_Mapping.TypeName = className;
            m_Mapping.TableName = tableName;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            const string Template = "TypeName = \"{0}\",TableName =\"{1}\", IsTable = {2},KeyGenerator=\"{3}\"";
            return String.Format(Template, TypeName, TableName, IsTable.ToString().ToLower(), KeyGenerator);
        }
    }
}
