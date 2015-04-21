using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.QuickDataBase.Configuration;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 获取数据操作对象
    /// </summary>
    public class DataBaseContext : IDataBaseContext
    {
        /// <summary>
        /// 数据库链接信息
        /// </summary>
        protected ConnectionInfo m_ConnectionInfo;

        /// <summary>
        /// 获取数据库链接信息
        /// </summary>
        public ConnectionInfo ConnectionInfo { get { return m_ConnectionInfo; } }
        /// <summary>
        /// 数据库链接信息
        /// </summary>
        /// <param name="connectionInfo"></param>
        public DataBaseContext(ConnectionInfo connectionInfo)
        {
            m_ConnectionInfo = connectionInfo;
        }

        #region 实例方法
        /// <summary>
        /// 获取数据库类型适配器
        /// </summary>
        public virtual IDataBaseAdapter DBAdapter
        {
            get { return new DataBaseAdapterCreator().GetDataBaseAdapter(m_ConnectionInfo); }
        }
        /// <summary>
        /// 获取数据库操作类
        /// </summary>
        /// <returns></returns>
        public virtual IDataBaseHandler Handler
        {
            get
            {
                return DBAdapter.GetDbHandler(this); 
            }
        }

        /// <summary>
        /// 获取参数集合对象
        /// </summary>
        /// <returns></returns>
        public virtual IDataParameters CreateDataParameters()
        {
            return new DataParameters(DBAdapter);
        }

        #endregion
    }
}
