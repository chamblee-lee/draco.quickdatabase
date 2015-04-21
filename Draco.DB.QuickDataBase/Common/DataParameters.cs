using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 参数集合
    /// </summary>
    public class DataParameters : IDataParameters
    {
        /// <summary>
        /// 数据库适配器
        /// </summary>
        protected IDataBaseAdapter m_Adapter;
        /// <summary>
        /// 参数列表
        /// </summary>
        protected List<IDataParameter> m_Params;

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Adapter"></param>
        public DataParameters(IDataBaseAdapter Adapter)
        {
            m_Adapter = Adapter;
            m_Params = new List<IDataParameter>();
        }
        #endregion

        /// <summary>
        /// 参数集合的新拷贝
        /// </summary>
        public IDataParameter[] Parameters
        {
            get
            {
                if (m_Params.Count > 0)
                    return m_Params.ToArray();
                return null;
            }
        }

        /// <summary>
        /// 获取适配形参
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        public string AdaptParameterName(string paraName)
        {
            String[] existNames = null;
            if (this.m_Params != null && this.m_Params.Count > 0)
            {
                existNames = new String[this.m_Params.Count];
                for (int i = 0; i < this.m_Params.Count; i++)
                    existNames[i] = this.m_Params[i].ParameterName;
            }
            return m_Adapter.AdaptParameterName(paraName,existNames);
        }

        #region 添加、清空 参数
        /// <summary>
        /// 向查询对象中添加参数值
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public virtual void AddParameterValue(string paraName, object val, DbType dataType)
        {
            if (String.IsNullOrEmpty(paraName))
                throw new ArgumentNullException("paraName");
            String pName = m_Adapter.AdaptParameterName(paraName);
            IDataParameter para = m_Adapter.CreateParameter(pName, val);
            para.DbType = dataType;
            m_Params.Add(para);
        }
        /// <summary>
        /// 向查询对象中添加参数值
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        public virtual void AddParameterValue(string paraName, object val)
        {
            if (String.IsNullOrEmpty(paraName))
                throw new ArgumentNullException("paraName");
            String pName = m_Adapter.AdaptParameterName(paraName);
            IDataParameter para = m_Adapter.CreateParameter(pName, val);
            m_Params.Add(para);
        }
        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            m_Params.Clear();
        }
        #endregion
    }
}
