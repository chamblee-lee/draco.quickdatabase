using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 表映射接口
    /// </summary>
    public interface ITableMapping
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        string TypeName { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; set; }
        /// <summary>
        /// 主键生成器
        /// </summary>
        string Generator { get; set; }
        /// <summary>
        /// 是表
        /// </summary>
        bool IsTable { get; set; }
        /// <summary>
        /// 主键映射
        /// </summary>
        List<IPrimaryKeyMapping> PrimaryKeyCollection { get; set; }
        /// <summary>
        /// 字段集合
        /// </summary>
        List<IFieldMapping> FieldMappingCollection { get; }

        /// <summary>
        /// 获取字段映射
        /// </summary>
        /// <param name="PropertyName">属性名称</param>
        /// <returns></returns>
        IFieldMapping GetFieldMapping(string PropertyName);
    }
}
