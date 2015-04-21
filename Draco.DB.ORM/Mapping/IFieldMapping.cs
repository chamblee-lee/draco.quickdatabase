using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 类属性与数据库字段的映射接口
    /// </summary>
    public interface IFieldMapping
    {
        /// <summary>
        /// 类属性名称
        /// </summary>
        string PropertyName{get;set;}
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        string ColumnName { get; set; }
        /// <summary>
        /// 字段是否可以为空
        /// </summary>
        bool IsNullable { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        FieldType FieldType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        int FieldLength { get; set; }
        /// <summary>
        /// 字段默认值
        /// </summary>
        string DefauleValue { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        string Remark { get; set; }
    }
}
