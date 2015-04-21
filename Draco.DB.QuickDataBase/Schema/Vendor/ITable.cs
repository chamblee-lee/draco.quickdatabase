using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;

namespace Draco.DB.QuickDataBase.Schema.Vendor
{
    /// <summary>
    /// 表结构接口
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// 表名称
        /// </summary>
        string TableName { get; set; }
        /// <summary>
        /// 数据列
        /// </summary>
        List<DataTableColumn> Columns { get; set; }
    }
}
