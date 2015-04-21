using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM;
using Draco.DB.ORM.PKGenerator;

namespace Draco.DB.ORM.Common
{
    /// <summary>
    /// 实体操作类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityHandler<T> : IEntityHandler<T> where T : AbstractEntity, new()
    {
        //日志记录器
        private static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(EntityHandler<T>).Assembly,typeof(EntityHandler<T>));

        //SQL构造器
        private SQLBuilder m_SQLBuilder;
        private IDataBaseHandler m_Handle;
        private T m_Entity;

        #region 构造方法
        /// <summary>
        /// 构造Hadnler
        /// </summary>
        /// <param name="Entity">实体类型</param>
        /// <param name="context">上下文对象</param>
        public EntityHandler(T Entity, IDataBaseContext context)
        {
            IORMAdapter Adapter = ORMAdapterCreator.GetORMAdapter(context.ConnectionInfo.DataServerType);
            m_SQLBuilder = Adapter.CreateSQLBuilder(Entity, Adapter);
            m_Handle = Adapter.GetDbHandler(context);
            m_Entity = Entity;
        }
        /// <summary>
        /// 构造Hadnler
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="handler"></param>
        public EntityHandler(T Entity,IDataBaseHandler handler)
        {
            IORMAdapter Adapter = ORMAdapterCreator.GetORMAdapter(handler.DbContext.ConnectionInfo.DataServerType);
            m_SQLBuilder = Adapter.CreateSQLBuilder(Entity, Adapter);
            m_Handle = handler ; 
            m_Entity = Entity;
        }
        #endregion

        /// <summary>
        /// SQL构造器
        /// </summary>
        public SQLBuilder SQLBuilder
        {
            get { return m_SQLBuilder; }
        }

        #region 查询方法
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet()
        {
            IDataParameter[] paras = null;
            string SQL = m_SQLBuilder.CreateSelectSQL(out paras);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteQuery(SQL, paras);
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="Where">Where子句</param>
        /// <param name="paras">参数对象</param>
        /// <returns></returns>
        public DataSet GetDataSet(string Where,params IDataParameter[] paras)
        {
            string SQL = m_SQLBuilder.CreateSelectSQL(ref paras, Where);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteQuery(SQL, paras);
        }
        
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <returns></returns>
        public T GetOne()
        {
            List<T> list = GetArrayList();
            if(list!=null && list.Count>0)
                return list[0];
            return null;
        }
        /// <summary>
        /// 获取数据实体列表
        /// </summary>
        /// <returns></returns>
        public List<T> GetArrayList()
        {
            DataSet ds = GetDataSet();
            return DataLoader.LoadEntity<T>(ds,typeof(T));
        }
        /// <summary>
        /// 获取数据实体列表
        /// </summary>
        /// <param name="Where">Where子句</param>
        /// <param name="paras">参数对象</param>
        /// <returns></returns>
        public List<T> GetArrayList(string Where, params IDataParameter[] paras)
        {
            DataSet ds = GetDataSet(Where, paras);
            List<T> list = DataLoader.LoadEntity<T>(ds, m_Entity.GetType());
            return list;
        }
        /// <summary>
        /// 获取一个字段
        /// </summary>
        /// <param name="ReturnField">返回值</param>
        /// <returns></returns>
        public object GetOneField(string ReturnField)
        {
            IDataParameter[] paras = null;
            string SQL = m_SQLBuilder.CreateSelectSQL(ReturnField,out paras);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteScalar(SQL, paras);
        }
        /// <summary>
        /// 获取一个字段
        /// </summary>
        /// <param name="ReturnField"></param>
        /// <param name="Where"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public object GetOneField(string ReturnField, string Where, params IDataParameter[] paras)
        {
            string SQL = m_SQLBuilder.CreateSelectSQL(ReturnField,Where, ref paras);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteScalar(SQL, paras);
        }
        #endregion

        #region 保存方法
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            bool IsInsert = true;//缺省执行插入操作
            string SQL = "";
            IDataParameter[] paras = null;
            if (m_SQLBuilder.CheckePrimaryKey())
            {
                //主键已经有值了，就检查记录是否存在
                SQL = m_SQLBuilder.CheckDataExistSQL(out paras);
                object o = m_Handle.ExecuteScalar(SQL, paras);
                int Count = Convert.ToInt32(o);
                IsInsert = Count == 0;//没有找到记录就插入，找到记录就更新
            }
            if (IsInsert)
            {
                return Insert();
            }
            else
            {
                return Update();
            }
        }
        /// <summary>
        /// 插入记录
        /// </summary>
        /// <returns></returns>
        public int Insert()
        {
            string SQL = "";
            IDataParameter[] paras = null;
            if (!m_SQLBuilder.CheckePrimaryKey())
            {
                //没有设定主键，就先确认主键
                IPKGenerator Generator = m_SQLBuilder.CreatePKGenerator(m_Handle);
                m_SQLBuilder.CreatePrimaryKey(Generator);
            }
            SQL = m_SQLBuilder.CreateInsertSQL(out paras);
            ToLogout(SQL, paras);
            int i = m_Handle.ExecuteNonQuery(SQL, paras);
            m_SQLBuilder.ProcessOutputParameters(paras);
            return i;
        }
        /// <summary>
        /// 更新记录
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            IDataParameter[] paras = null;
            string SQL = m_SQLBuilder.CreateUpdataSQL(out paras);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteNonQuery(SQL, paras);
        }
        #endregion

        #region 删除方法
        /// <summary>
        /// 删除方法
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            IDataParameter[] paras = null;
            string SQL = m_SQLBuilder.CreateDeleteSQL(out paras);
            ToLogout(SQL, paras);
            return m_Handle.ExecuteNonQuery(SQL, paras);
        }
        #endregion

        #region 表达式操作
        /// <summary>
        /// 添加实体属性查询表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public EntityHandler<T> AddExpression(Expression exp)
        {
            m_SQLBuilder.Add(exp);
            return this;
        }
        /// <summary>
        /// 清空实体属性表达式
        /// </summary>
        public void ClearExpression()
        {
            m_SQLBuilder.Clear();
            m_Entity.ClearChangedProperty();
        }
        #endregion

        #region 其它方法
        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="Entity"></param>
        public IEntityHandler<T> SetEntity(T Entity)
        {
            m_Entity = Entity;
            m_SQLBuilder.SetEntity(Entity);
            return this;
        }
        #endregion


        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="paras"></param>
        private void ToLogout(string SQL, params IDataParameter[] paras)
        {
            string info = "SQL语句:"+SQL+"\r\n";
            if (paras != null && paras.Length > 0)
            {
                info+=" 参数列表:";
                foreach (IDataParameter para in paras)
                {
                    info += para.ParameterName + "=" + Convert.ToString(para.Value) + ";";
                }
            }
            _log.Debug(info);
        }
    }
}
