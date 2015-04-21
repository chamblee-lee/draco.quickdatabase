using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 参数化SQL
    /// </summary>
    public class ParameterizedSQL
    {
        private String strsql = "";
        private IDataParameter[] m_parameters;

        /// <summary>
        /// 空构造
        /// </summary>
        public ParameterizedSQL() { }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="paras"></param>
        public ParameterizedSQL(String SQL,IDataParameter[] paras)
        {
            strsql = SQL;
            m_parameters = paras;
        }

        /// <summary>
        /// SQL语句
        /// </summary>
        public String SQL
        {
            get { return strsql; }
            set { strsql = value; }
        }
        
        /// <summary>
        /// 参数数组
        /// </summary>
        public IDataParameter[] Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }
    }
}
