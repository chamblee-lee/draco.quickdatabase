using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 数据库事物域对象
    /// </summary>
    public class DbTransactionScope:IDisposable
    {
        private bool m_Complete = false;
        private IDataBaseHandler m_handler;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public DbTransactionScope(IDataBaseHandler handler)
        {
            m_handler = handler;
            m_handler.StartTransaction();
        }
        /// <summary>
        /// 提交完成
        /// </summary>
        public void Complete()
        {
            m_Complete = true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            if (m_Complete)
                m_handler.CommitTransaction();
            else
                m_handler.RollbackTransaction();
        }
    }
}
