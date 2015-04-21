using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 表映射实现
    /// </summary>
    public class TableMapping : ITableMapping
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        protected string m_TypeName;
        /// <summary>
        /// 表名称
        /// </summary>
        protected string m_TableName;
        /// <summary>
        /// 主键生成器名称
        /// </summary>
        protected string m_Generator;
        /// <summary>
        /// 是否是表
        /// </summary>
        protected bool m_IsTable = true;
        /// <summary>
        /// 主键字段列表
        /// </summary>
        protected List<IPrimaryKeyMapping> m_PrimaryKeyMappings = new List<IPrimaryKeyMapping>();
        /// <summary>
        /// 非主键字段列表
        /// </summary>
        protected List<IFieldMapping> m_FieldMappings = new List<IFieldMapping>();

        #region ITableMapping 成员
        /// <summary>
        /// 类型名称
        /// </summary>
        public virtual string TypeName
        {
            get
            {
                return m_TypeName;
            }
            set
            {
                m_TypeName = value;
            }
        }
        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return m_TableName;
            }
            set
            {
                m_TableName = value;
            }
        }
        /// <summary>
        /// 主键生成器
        /// </summary>
        public string Generator 
        {
            get { return m_Generator; }
            set { m_Generator = value; } 
        }
        /// <summary>
        /// 是否为表
        /// </summary>
        public virtual bool IsTable
        {
            get
            {
                return m_IsTable;
            }
            set
            {
                m_IsTable = value;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public virtual List<IPrimaryKeyMapping> PrimaryKeyCollection
        {
            get
            {
                return m_PrimaryKeyMappings;
            }
            set
            {
                m_PrimaryKeyMappings = value;
            }
        }
        /// <summary>
        /// 字段集合
        /// </summary>
        public virtual List<IFieldMapping> FieldMappingCollection
        {
            get { return m_FieldMappings; }
        }

        /// <summary>
        /// 获取字段映射
        /// </summary>
        /// <param name="PropertyName">属性名称</param>
        /// <returns></returns>
        public IFieldMapping GetFieldMapping(string PropertyName)
        {
            if (m_FieldMappings.Count > 0)
            {
                foreach (IFieldMapping mapping in m_FieldMappings)
                {
                    if (String.Compare(mapping.PropertyName, PropertyName, true) == 0)
                    {
                        return mapping;
                    }
                }
            }
            if (m_PrimaryKeyMappings.Count > 0)
            {
                foreach (IFieldMapping mapping in m_PrimaryKeyMappings)
                {
                    if (String.Compare(mapping.PropertyName, PropertyName, true) == 0)
                    {
                        return mapping;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
