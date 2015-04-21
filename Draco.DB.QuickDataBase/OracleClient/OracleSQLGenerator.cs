using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.OracleClient
{
    /// <summary>
    /// SQL生成器
    /// </summary>
    public class OracleSQLGenerator : SQLGenerator
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="adapter"></param>
        public OracleSQLGenerator(IDataBaseAdapter adapter)
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
        public override ParameterizedSQL CreatePagedSQL(string SQL, int PageIndex, int PageSize, string KeyField, string OrderField, bool OrderByDesc, params System.Data.IDataParameter[] paras)
        {
            if (!String.IsNullOrEmpty(OrderField) && SQL.IndexOf("order")<0)
            {
                string order = " order by " + OrderField;
                order = OrderByDesc ? order + " desc " : order + " asc ";
                SQL += order;
            }
            int CountBeforeFirst = PageSize * PageIndex;        //结果集的第一条记录在原结果集的计数
            int CountBeforeLast = PageSize * (PageIndex + 1);   //结果集的最后一条记录在原结果集的计数
            string InnerSQL = "select x_pageTb_0.*,rownum as x_pagefd_0 from (" + SQL + ") x_pageTb_0 where  rownum <" + (CountBeforeLast + 1);
            string SQlOuter = "select * from (" + InnerSQL + ") where x_pagefd_0>" + CountBeforeFirst;
            return new ParameterizedSQL(SQlOuter, paras);
        }

        /// <summary>
        /// 查询数据库当前时间的SQL
        /// </summary>
        /// <returns></returns>
        public override string GetDataBaseTimeSQL()
        {
            return " select sysdate from dual";
        }

        /// <summary>
        /// 把时间转换为SQL中可识别的时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override string ConvertDateTimeToSQL(DateTime dt)
        {
            string dateString = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return "to_date('" + dateString + "','yyyy-mm-dd hh24:mi:ss') ";
        }
    }
}
