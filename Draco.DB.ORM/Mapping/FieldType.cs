using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// ORM字段类型
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 整形
        /// </summary>
        Int, 
        /// <summary>
        /// 长整形
        /// </summary>
        Long, 
        /// <summary>
        /// 小数型
        /// </summary>
        Decimal, 
        /// <summary>
        /// 字符串型
        /// </summary>
        String, 
        /// <summary>
        /// 长文本型
        /// </summary>
        Text, 
        /// <summary>
        /// 日期时间类型
        /// </summary>
        DateTime, 
        /// <summary>
        /// 二进制类型
        /// </summary>
        Binary,
        /// <summary>
        /// 位类型
        /// </summary>
        Bit,
        /// <summary>
        /// 字节类型
        /// </summary>
        Byte
    }
}
