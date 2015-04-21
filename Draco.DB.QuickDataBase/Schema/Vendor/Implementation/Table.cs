using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Schema.Vendor.Implementation
{
    /// <summary>
    /// 表结构接口实现
    /// </summary>
    [Serializable]
    public class Table : ITable
    {
        private string m_TableName;
        private List<DataTableColumn> m_Columns = new List<DataTableColumn>();

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName 
        {
            get { return m_TableName; }
            set { m_TableName = value; } 
        }
        /// <summary>
        /// 数据列
        /// </summary>
        public List<DataTableColumn> Columns 
        { 
            get { return m_Columns; }
            set { m_Columns = value; }
        }
    }
}
