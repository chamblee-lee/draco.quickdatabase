using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Schema.Vendor.Implementation
{
    /// <summary>
    /// 数据表字段列
    /// </summary>
    [Serializable]
    public class DataTableColumn : DataType, IDataTableColumn
    {
        /// <summary>
        /// The column name
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The table to which belongs the column
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The table schema to which belongs the column
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Used to determine if the column is a primary key.
        /// May be null, because some vendors don't show this as a column property (Oracle for example) but as table constraints
        /// </summary>
        public bool? PrimaryKey { get; set; }

        /// <summary>
        /// The value assigned when nothing is specified in insert.
        /// Sometimes use to determine if a column is a sequence.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Determines if the column value is generated when there is no value given in insert
        /// </summary>
        public bool? Generated { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }
    }
}
