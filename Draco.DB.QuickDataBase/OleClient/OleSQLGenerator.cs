using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.OleClient
{
    /// <summary>
    /// SQL生成器
    /// </summary>
    public class OleSQLGenerator : SQLGenerator
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="adapter"></param>
        public OleSQLGenerator(IDataBaseAdapter adapter)
            : base(adapter){}

        /// <summary>
        /// 创建分页SQL
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="PageIndex">分页索引(从0开始)</param>
        /// <param name="PageSize">分页大小</param>
        /// <param name="KeyField">关键字段</param>
        /// <param name="OrderField">排序字段</param>
        /// <param name="OrderByDesc">降序排列</param>
        /// <param name="paras">参数列表</param>
        /// <returns></returns>
        public override ParameterizedSQL CreatePagedSQL(string SQL, int PageIndex, int PageSize, string KeyField, string OrderField, bool OrderByDesc, params System.Data.IDataParameter[] paras)
        {
            return new ParameterizedSQL(SQL, paras);
        }
        /// <summary>
        /// 获取查询数据库时间的SQL
        /// </summary>
        /// <returns></returns>
        public override string GetDataBaseTimeSQL()
        {
            return "";
        }
    }
}
