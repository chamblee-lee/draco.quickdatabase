using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 字段映射
    /// </summary>
    public class FieldMapping : IFieldMapping
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        protected string m_PropertyName;
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        protected string m_ColumnName;
        /// <summary>
        /// 是否可为空
        /// </summary>
        protected bool m_IsNullable;
        /// <summary>
        /// 字段类型
        /// </summary>
        protected FieldType m_FieldType;
        /// <summary>
        /// 字段长度
        /// </summary>
        protected int m_FieldLength;
        /// <summary>
        /// 缺省值
        /// </summary>
        protected string m_DefauleValue;
        /// <summary>
        /// 备注
        /// </summary>
        protected string m_Remark;

        #region IFieldMapping 成员
        /// <summary>
        /// 类属性名称
        /// </summary>
        public virtual string PropertyName
        {
            get
            {
                return m_PropertyName;
            }
            set
            {
                m_PropertyName = value;
            }
        }
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public virtual string ColumnName
        {
            get
            {
                return m_ColumnName;
            }
            set
            {
                m_ColumnName = value;
            }
        }
        /// <summary>
        /// 字段是否可以为空
        /// </summary>
        public virtual bool IsNullable
        {
            get
            {
                return m_IsNullable;
            }
            set
            {
                m_IsNullable = value;
            }
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        public virtual FieldType FieldType
        {
            get
            {
                return m_FieldType;
            }
            set
            {
                m_FieldType = value;
            }
        }
        /// <summary>
        /// 字段长度
        /// </summary>
        public virtual int FieldLength
        {
            get
            {
                return m_FieldLength;
            }
            set
            {
                m_FieldLength = value;
            }
        }
        /// <summary>
        /// 字段默认值
        /// </summary>
        public virtual string DefauleValue
        {
            get
            {
                return m_DefauleValue;
            }
            set
            {
                m_DefauleValue = value;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark
        {
            get
            {
                return m_Remark;
            }
            set
            {
                m_Remark = value;
            }
        }

        #endregion
    }
}
