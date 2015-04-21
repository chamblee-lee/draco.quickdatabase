using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 映射加载器
    /// </summary>
    public interface IMappingLoader
    {
        /// <summary>
        /// 从实体类型加载表映射对象
        /// </summary>
        /// <param name="EntityType"></param>
        /// <returns></returns>
        ITableMapping LoadTableMapping(Type EntityType);
    }
}
