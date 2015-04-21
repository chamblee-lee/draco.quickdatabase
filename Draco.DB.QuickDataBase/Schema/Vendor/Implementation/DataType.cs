using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Schema.Vendor.Implementation
{
    /// <summary>
    /// Default IDataType implementation (see IDataType for details)
    /// </summary>
    [Serializable]
    public class DataType : IDataType
    {
        /// <summary>
        /// 公共类型变量
        /// </summary>
        public virtual string SimpleType { get; set; }
        /// <summary>
        /// The base type, like 'number', 'varchar'
        /// </summary>
        public virtual string Type { get; set; }
        /// <summary>
        /// For all types, the possibility to have a NULL
        /// </summary>
        public virtual bool Nullable { get; set; }
        /// <summary>
        /// On non numeric data types, the length (for strings or blobs)
        /// </summary>
        public virtual long? Length { get; set; }
        /// <summary>
        /// On numeric data types, the number of digits in the integer part
        /// </summary>
        public virtual int? Precision { get; set; }
        /// <summary>
        /// On numeric data types, the number of digits in the decimal part
        /// </summary>
        public virtual int? Scale { get; set; }
        /// <summary>
        /// On numeric data types, if there is a sign
        /// </summary>
        public virtual bool? Unsigned { get; set; }
        /// <summary>
        /// The original (or domain) type, returned raw by column information.
        /// Is also used to generated the database.
        /// </summary>
        public string FullType { get; set; }
    }
}
