using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.PKGenerator
{
    /// <summary>
    /// 内部主键生成器类型枚举
    /// </summary>
    public class PKGeneratorEnum
    {
        /// <summary>
        /// Guid方式主键
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 简单Guid主键
        /// </summary>
        public const string SIMPLEGUID = "SIMPLEGUID";
        /// <summary>
        /// 时间戳主键
        /// </summary>
        public const string TIMESTAMP = "TIMESTAMP";
        /// <summary>
        /// 数据库定义的主键
        /// </summary>
        public const string LOCAL = "LOCAL";
        /// <summary>
        /// 自定义序列
        /// </summary>
        public const string ORM_SEQUENCE = "ORM_SEQUENCE";
    }
}
