using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 抽象数据库事物
    /// </summary>
    public class DataBaseTransaction : DbTransaction, IDisposable
    {
        private IDbConnection m_Connection;//事物链接对象
		private IDbTransaction m_tra;//相对于当前数据库类型相应的数据库对象
		private string m_ConnectionString;//数据库连接串
        private IsolationLevel m_IsolationLevel = IsolationLevel.ReadCommitted;//事物隔离级别
        private bool m_disposed;//是否已释放
        private IDataBaseAdapter m_DBFactory;//对象工厂
        private int m_ReferenceCount = 0;//外部引用计数，由外部使用

        #region 属性
        /// <summary>
        /// 获取或设置外部引用计数
        /// </summary>
        public int ReferenceCount 
        {
            get { return m_ReferenceCount; }
            set { m_ReferenceCount = value; }
        }
        /// <summary>
		/// 完整的数据库连接串
		/// </summary>
		public virtual string ConnectionString
		{
			get
			{
				return m_ConnectionString;
			}
		}
        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public override System.Data.IsolationLevel IsolationLevel
        {
            get
            {
                return this.m_IsolationLevel;
            }
        }
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        protected override DbConnection DbConnection
        {
            get
            {
                return m_Connection as DbConnection;
            }
        }
        /// <summary>
        /// 事务对象（IDbTransaction类型），需要时自己强制转换成对应的事务对象，只读
        /// </summary>
        public virtual IDbTransaction Tra 
        { get { return m_tra; } }
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ConnString">连接串</param>
        /// <param name="Factory">工厂对象</param>
        public DataBaseTransaction(string ConnString, IDataBaseAdapter Factory)
        {
            this.m_ConnectionString = ConnString;
            m_DBFactory = Factory;//创建对象工厂
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Conn">数据库链接对象</param>
        public DataBaseTransaction(IDbConnection Conn)
        {
            this.m_Connection = Conn;
        }

        #endregion

        #region 开始、提交、回滚事物
        /// <summary>
		/// 开始执行事务，默认级别IsolationLevel.ReadCommitted
		/// </summary>
		/// <returns>事务对象</returns>
		public virtual IDbTransaction StartTransaction()
		{
            return StartTransaction(m_IsolationLevel);
		}
		/// <summary>
		/// 开始执行事务
		/// </summary>
		/// <param name="Level">枚举型的事务隔离级别</param>
		/// <returns>事务对象</returns>
		public virtual IDbTransaction StartTransaction(IsolationLevel Level)
		{
			this.m_IsolationLevel = Level;
            if (m_Connection == null)
            {
                this.m_Connection = m_DBFactory.DbFactory.CreateConnection();
                this.m_Connection.ConnectionString = ConnectionString;
            }
            if (m_Connection.State != ConnectionState.Open)
                this.m_Connection.Open();

            this.m_tra = m_Connection.BeginTransaction(Level);
            return this.m_tra;
		}
		/// <summary>
		/// 提交事务，并关闭连接，释放资源
		/// </summary>
		public override  void Commit()
		{
            this.m_tra.Commit();
            this.m_Connection.Close();
            this.m_tra.Dispose();
            this.m_Connection.Dispose();
            this.m_Connection = null;
            this.m_tra = null;
		}
		
		/// <summary>
		/// 回滚事务。
		/// </summary>
		public override void Rollback()
		{
            if (this.m_tra != null)
			{
                if (this.m_tra.Connection != null && this.m_tra.Connection.State == ConnectionState.Open)
                    this.m_tra.Rollback();
                this.m_tra.Dispose();
                this.m_tra = null;
			}
            if (this.m_Connection != null)
			{
                this.m_Connection.Close();
                this.m_Connection.Dispose();
                this.m_Connection = null;
			}
		}
		/// <summary>
		/// 释放资源
		/// </summary>
		/// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
		{
            base.Dispose(disposing);
			if(!this.m_disposed)
			{
				if(disposing)
				{
                    if (this.m_Connection != null) this.m_Connection.Dispose();
                    if (this.m_tra != null)
                    {
                        m_tra.Commit();
                        this.m_tra.Dispose();
                    }
					m_disposed = true;         
				}
			}
        }
        #endregion
    }
}
