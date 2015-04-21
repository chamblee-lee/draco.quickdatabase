using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 通用标准SQL操作
    /// </summary>
    public class DataBaseHandlerEx : DataBaseHandler
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="context"></param>
        /// <param name="Adapter"></param>
        public DataBaseHandlerEx(IDataBaseContext context, IDataBaseAdapter Adapter)
            : base(context, Adapter)
        {}

        #region 删除数据
        //*********************************************************************
        /// <summary>功能：删除表中Key值为KeywordValue的纪录
        ///   </summary>
        ///   <param name="tblName">表名</param>
        ///   <param name="Keyword">表的主键</param>,
        ///   <param name="KeywordValue">要查询的主键的值</param>
        ///   <param name="Tra">事务对象</param>
        //*********************************************************************
        public virtual bool DeleteBase(string tblName, string Keyword, string KeywordValue, IDbTransaction Tra)
        {
            return DeleteBase(tblName, new string[] { Keyword }, new string[] { KeywordValue }, Tra);
        }
        /// <summary>
        /// 功能：删除表中Key值为KeywordValue的纪录
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="Keyword">表的主键</param>
        /// <param name="KeywordValue">要查询的主键的值</param>
        /// <param name="Tra">事务对象</param>
        public virtual bool DeleteBase(string tblName, string[] Keyword, string[] KeywordValue, IDbTransaction Tra)
        {
            if (String.IsNullOrEmpty(tblName))
                throw new ArgumentNullException("tblName");
            if (Keyword == null || Keyword.Length == 0)
                throw new ArgumentNullException("Keyword");
            if (KeywordValue == null || KeywordValue.Length == 0)
                throw new ArgumentNullException("KeywordValue");
            if (Keyword.Length != KeywordValue.Length)
            {
                throw new ArgumentException("无效的主键值" + "(Keyword KeywordValue)");
            }
            //开始创建SQL
            StringBuilder sBuilder = new StringBuilder();
            IDataParameters paras = new DataParameters(m_DBaseAdapter);
            for (int i = 0; i < Keyword.Length; i++)
            {
                string pName = m_DBaseAdapter.AdaptParameterName("KeywordValue" + i);
                sBuilder.Append(Keyword[i] + " = " + pName + CommonSQL.AND);
                paras.AddParameterValue(pName, KeywordValue[i]);
            }
            sBuilder = sBuilder.Remove(sBuilder.Length - CommonSQL.AND.Length, CommonSQL.AND.Length);//去掉最后的AND
            string DelSql = String.Format(CommonSQL.SQL_DELETE, tblName, sBuilder.ToString());
            int rtnFlag = ExecuteNonQuery(Tra,DelSql,paras.Parameters);
            return rtnFlag == 1;
        }
        #endregion

        #region 生成插入语更新语句
        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="Columns">Hash</param>
        /// <returns>SQL语句</returns>
        public virtual string GetInsertSql(string tblName, string[] Columns)
        {
            if (String.IsNullOrEmpty(tblName)) throw new ArgumentException("tblName is null");
            if (Columns == null || Columns.Length == 0) throw new ArgumentException("Columns is null");
            //开始计算
            StringBuilder sBuilderFields = new StringBuilder();
            StringBuilder sBuilderVaules = new StringBuilder();
            foreach (string key in Columns)
            {
                sBuilderFields.Append(key.ToString() + ",");
                string ParaName = m_DBaseAdapter.AdaptParameterName(key.ToString());
                sBuilderVaules.Append(ParaName + ",");
            }
            sBuilderFields.Remove(sBuilderFields.Length - 1, 1);
            sBuilderVaules.Remove(sBuilderVaules.Length - 1, 1);
            string SQL = String.Format(CommonSQL.SQL_INSERT, tblName, sBuilderFields.ToString(), sBuilderVaules.ToString());
            return SQL;
        }
        /// <summary>
        /// 获取更新SQL
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="Columns"></param>
        /// <param name="PrimaryKey"></param>
        /// <returns></returns>
        public virtual string GetUpdateSql(string tblName, string[] Columns, string PrimaryKey)
        {
            if (String.IsNullOrEmpty(tblName)) throw new ArgumentException("tblName is null");
            if (Columns == null || Columns.Length == 0) throw new ArgumentException("Columns is null");
            if (String.IsNullOrEmpty(PrimaryKey)) throw new ArgumentException("PrimaryKey is null");

            StringBuilder sBuilderPair = new StringBuilder();
            foreach (string key in Columns)
            {
                string ParaName = m_DBaseAdapter.AdaptParameterName(key);
                sBuilderPair.Append(key + "=" + ParaName + ",");
            }
            sBuilderPair = sBuilderPair.Remove(sBuilderPair.Length - 1, 1);

            string where = "1=1";
            if (!String.IsNullOrEmpty(PrimaryKey))
            {
                //处理主键
                string PParaName = m_DBaseAdapter.AdaptParameterName(PrimaryKey);
                where = PrimaryKey + "=" + PParaName;
            }
            string UpdateSQL = String.Format(CommonSQL.SQL_UPDATE, tblName, sBuilderPair.ToString(), where);
            return UpdateSQL;
        }

        #endregion

        #region 检查数据是否存在
        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="PrimaryKey"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public virtual bool CheckDataExist(string tblName, string PrimaryKey, string KeyValue)
        {
            string pName = m_DBaseAdapter.AdaptParameterName("p1");
            string where = PrimaryKey + "=" + pName;
            string SQL = String.Format(CommonSQL.SQL_SELECT,"COUNT(*)",tblName,where);
            IDataParameter para = m_DBaseAdapter.DbFactory.CreateParameter();
            para.ParameterName = m_DBaseAdapter.AdaptParameterName(pName);
            para.DbType = DbType.String;
            object o = ExecuteScalar(SQL, null, para);
            return Convert.ToInt32(o) > 0;
        }

        #endregion

        #region 填充参数(hash实现有问题，暂时注释)
        ///// <summary>
        ///// 填充参数
        ///// </summary>
        ///// <param name="ColValues">参数名称-参数值</param>
        ///// <param name="ColTypes">参数名称-参数类型</param>
        ///// <returns></returns>
        //public IDataParameter[] FillDataParameters(Hashtable ColValues, Hashtable ColTypes)
        //{
        //    if (ColValues != null && ColValues.Count > 0)
        //    {
        //        if (ColTypes == null || ColTypes.Count != ColValues.Count)
        //            throw new ArgumentException("未知的字段类型，无法填充形参![FillDataParameters]");
        //        IDataParameters paras = new DataParameters(m_DBaseAdapter);
        //        foreach (string ColName in ColValues.Keys)
        //        {
        //            DbType type = (DbType)System.Enum.Parse(typeof(DbType), Convert.ToString(ColTypes[ColName]));
        //            paras.AddParameterValue(ColName, ColValues[ColName], type);
        //        }
        //        return paras.Parameters;
        //    }
        //    return null;
        //}
        #endregion
    }
}
