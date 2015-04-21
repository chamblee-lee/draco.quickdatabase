using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM.Common;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.ORM.Compatible
{
    /// <summary>
    /// 兼容实体类型基类，这是一个标记类型，用来兼容泛型实现
    /// </summary>
    public class CompatibleEntityBase : AbstractEntity
    {
    }
    /// <summary>
    /// 本类只是为了兼容旧版本的实体操作类而存在
    /// </summary>
    public class EntityBase : CompatibleEntityBase
    {
        /// <summary>
        /// 根据查询条件取得数据集
        /// </summary>
        /// <param name="Stru"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static DataSet GetDataSetBy(TblStruBase Stru, IDataBaseHandler handler)
        {
            ORMContext ctx = new ORMContext(handler.DbContext.ConnectionInfo);
            IEntityHandler<TblStruBase> hand = ctx.CreateHandler<TblStruBase>(Stru, handler);
            if (!String.IsNullOrEmpty(Stru.ParameterSQL))
            {
                IDataParameter[] paras = null;
                Hashtable hash = Stru.__Parameters;
                if (hash != null && hash.Count > 0)
                {
                    IDataParameters dps = new DataParameters(handler.DbAdapter);
                    foreach (String key in hash)
                    {
                        dps.AddParameterValue(key, hash[key]);
                    }
                    paras = dps.Parameters;
                }
                return hand.GetDataSet(Stru.ParameterSQL,paras);
            }
            return hand.GetDataSet();
        }
        /// <summary>
        /// 根据查询条件取得数据集
        /// </summary>
        /// <param name="Stru"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ArrayList GetArrayListBy(TblStruBase Stru, IDataBaseHandler handler)
        {
            DataSet ds = GetDataSetBy(Stru, handler);
            List<EntityBase> list = DataLoader.LoadEntity<EntityBase>(ds, Stru.__EntityType);
            ArrayList alist = new ArrayList();
            alist.AddRange(list);
            return alist;
        }

        /// <summary>
        /// 根据查询条件取得第一条记录的第一个字段值
        /// </summary>
        /// <param name="ReturnField"></param>
        /// <param name="Stru"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static object GetOneFieldBy(string ReturnField, TblStruBase Stru, IDataBaseHandler handler)
        {
            ORMContext ctx = new ORMContext(handler.DbContext.ConnectionInfo);
            IEntityHandler<TblStruBase> hand = ctx.CreateHandler<TblStruBase>(Stru,handler);
            if (!String.IsNullOrEmpty(Stru.ParameterSQL))
            {
                IDataParameter[] paras = null;
                Hashtable hash = Stru.__Parameters;
                if (hash != null && hash.Count > 0)
                {
                    IDataParameters dps = new DataParameters(handler.DbAdapter);
                    foreach (String key in hash)
                    {
                        dps.AddParameterValue(key, hash[key]);
                    }
                    paras = dps.Parameters;
                }
                return hand.GetOneField(ReturnField, Stru.ParameterSQL, paras);
            }
            return hand.GetOneField(ReturnField);
        }

        /// <summary>
        /// 删除记录为此主键值的记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ID0Value"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static bool Delete(String tableName,string ID0Value, IDataBaseHandler handler)
        {
            if (String.IsNullOrEmpty(ID0Value))
                return false;
            String sql = "delete from " + tableName + " where ID0 ='" + ID0Value + "'";
            int i =handler.ExecuteNonQuery(sql);
            return i > 0;
        }
        /// <summary>
        /// 根据关键字(ID)集合得到数据对象集合
        /// </summary>
        /// <param name="entiType"></param>
        /// <param name="tableName"></param>
        /// <param name="Keys"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected static ArrayList GetEntitiesByKeys(Type entiType, String tableName,int[] Keys, IDataBaseHandler handler)
        {
            if (Keys == null || Keys.Length == 0)
                return null;
            String sql = "select * from " + tableName + " where ID0 in(";
            foreach (int i in Keys)
            {
                sql += i + ",";
            }
            sql = sql.Substring(0,sql.Length - 1);
            sql += ")";
            DataSet ds  = handler.ExecuteQuery(sql);
            List<EntityBase> list = DataLoader.LoadEntity<EntityBase>(ds, entiType);
            ArrayList alist = new ArrayList();
            alist.AddRange(list);
            return alist;
            
        }/// <summary>
        /// 保存信息到数据库
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool Save(IDataBaseHandler handler)
        {
            ORMContext ctx = new ORMContext(handler.DbContext.ConnectionInfo);
            IEntityHandler<EntityBase> hand = ctx.CreateHandler<EntityBase>(this, handler);
            int i = hand.Save();
            return i > 0;
        }
        /// <summary>
        /// 删除此记录
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete(IDataBaseHandler handler)
        {
            ORMContext ctx = new ORMContext(handler.DbContext.ConnectionInfo);
            IEntityHandler<EntityBase> hand = ctx.CreateHandler<EntityBase>(this, handler);
            int i = hand.Delete();
            return i > 0;
        }
        
    }
}
