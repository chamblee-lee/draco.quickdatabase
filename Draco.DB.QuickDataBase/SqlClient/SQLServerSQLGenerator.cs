using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Adapter;
using System.Data;

namespace Draco.DB.QuickDataBase.SqlClient
{
    /// <summary>
    /// SQL生成器
    /// </summary>
    public class SQLServerSQLGenerator : SQLGenerator
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="adapter"></param>
        public SQLServerSQLGenerator(IDataBaseAdapter adapter)
            : base(adapter){}
        /// <summary>
        /// 创建分页SQL
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="PageIndex">分页索引(从0开始)</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="KeyField">关键字段</param>
        /// <param name="OrderField">排序字段</param>
        /// <param name="OrderByDesc">是否为降序</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        public override ParameterizedSQL CreatePagedSQL(string SQL, int PageIndex, int PageSize, string KeyField,string OrderField,bool OrderByDesc, params System.Data.IDataParameter[] paras)
        {
            string order = "";
            if (!String.IsNullOrEmpty(OrderField))
            {
                order = " order by " + OrderField;
                order = OrderByDesc ? order + " desc " : order + " asc ";
            }
            int CountBeforeFirst = PageSize * PageIndex;        //结果集的第一条记录在原结果集的计数
            int CountBeforeLast = PageSize * (PageIndex + 1);   //结果集的最后一条记录在原结果集的计数
            string SQLInner1 = " select top " + CountBeforeFirst + " " + KeyField + " from (" + SQL + ") pagetb_01 " + order;
            string SQLInner2 = "select top " + CountBeforeLast + " * from (" + SQL + ") pagetb_00" + order;
            String SQLOuter = " select * from (" + SQLInner2 + ") pagetb_0 where pagetb_0." + KeyField + " not in (" + SQLInner1 + ")";

            IDataParameter[] outParas = paras;
            if (SQLOuter.IndexOf("?") > 0)//匿名SQL
            {
                if (paras != null && paras.Length > 0)
                {
                    outParas = new IDataParameter[paras.Length * 2];
                    for (int i = 0; i < outParas.Length; i++)
                    {
                        outParas[i] = paras[i % paras.Length];
                    }
                }
            }
            return new ParameterizedSQL(SQLOuter, outParas);
        }

        /// <summary>
        /// 查询数据库时间的SQL
        /// </summary>
        /// <returns></returns>
        public override string GetDataBaseTimeSQL()
        {
            return "select getdate() as d ";
        }

        /// <summary>
        /// 把时间转换为SQL中可识别的时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override string ConvertDateTimeToSQL(DateTime dt)
        {
            string dateString = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return "'" + dateString + "'";
        }
    }
}
