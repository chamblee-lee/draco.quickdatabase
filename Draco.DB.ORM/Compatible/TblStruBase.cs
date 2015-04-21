using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.Common;
using System.Collections;
using System.Data;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.ORM.Compatible
{
    /// <summary>
    /// 查询实体基类
    /// </summary>
    public class TblStruBase : AbstractEntity
    {
        private string m_ParaSql = "";
        private Hashtable m_Parameters = new Hashtable();
        /// <summary>
        /// 获取真实实体类类型
        /// </summary>
        public virtual Type __EntityType
        {
            get { return typeof(TblStruBase); }
        }
        /// <summary>
        /// 自定义sql
        /// </summary>
        public virtual string ParameterSQL
        {
            get
            {
                return m_ParaSql;
            }
            set { m_ParaSql = value; }
        }
        /// <summary>
        /// 获取添加的参数列表
        /// </summary>
        public virtual Hashtable __Parameters
        {
            get
            {
                return m_Parameters;
            }
        }
        /// <summary>
        /// 向查询对象中添加参数值
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        public virtual void AddQueryParameterValue(string paraName, object val)
        {
            m_Parameters[paraName] = val;
        }
        
    }
}
