using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// SQL构建
    /// </summary>
    public abstract class SQLGenerator : ISQLGenerator
    {
        /// <summary>
        /// 数据库适配器
        /// </summary>
        protected IDataBaseAdapter m_Adapter;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Adapter"></param>
        public SQLGenerator(IDataBaseAdapter Adapter)
        {
            m_Adapter = Adapter;
        }

        /// <summary>
        /// 创建查询SQL
        /// </summary>
        /// <param name="paramz"></param>
        /// <param name="Fields"></param>
        /// <param name="TableName"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public virtual string CreateSelectSQL(out System.Data.IDataParameter[] paramz, List<string> Fields, string TableName, Dictionary<string, object> Condition)
        {
            paramz = null;
            if(String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TableName is null");
            StringBuilder StrFileds = new StringBuilder("*");
            if (Fields != null && Fields.Count > 0)
            {
                StrFileds.Remove(0,1);
                foreach (String f in Fields)
                {
                    StrFileds.Append(f + ",");
                }
                StrFileds.Remove(StrFileds.Length - 1, 1);
            }
            StringBuilder StrWhere = new StringBuilder();
            if (Condition != null && Condition.Count > 0)
            {
                IDataParameters paras = CreateDataParameters();
                int index = 0;
                foreach (String key in Condition.Keys)
                {
                    String pName = paras.AdaptParameterName(key) + index;
                    paras.AddParameterValue(pName, Condition[key]);
                    String Segment = key + "=" + pName ;
                    StrWhere.Append(Segment + CommonSQL.AND);
                    index++;
                }
                paramz = paras.Parameters;
                StrWhere.Remove(StrWhere.ToString().LastIndexOf(CommonSQL.AND), CommonSQL.AND.Length);
            }
            if (StrWhere.Length == 0)
                StrWhere.Append("1<2");
            String SQL = String.Format(CommonSQL.SQL_SELECT, StrFileds, TableName, StrWhere);
            return SQL;
        }
        /// <summary>
        /// 创建插入SQL
        /// </summary>
        /// <param name="paramz"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldValues"></param>
        /// <returns></returns>
        public virtual string CreateInsertSQL(out System.Data.IDataParameter[] paramz, string TableName, Dictionary<string, object> FieldValues)
        {
            paramz = null;
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TableName is null");
            if (FieldValues == null || FieldValues.Count == 0)
                throw new ArgumentException("FieldValues is empty");
            StringBuilder StrFileds = new StringBuilder();
            StringBuilder StrValues = new StringBuilder();

            IDataParameters paras = CreateDataParameters();
            int index = 0;
            foreach (String key in FieldValues.Keys)
            {
                StrFileds.Append(m_Adapter.AdaptColumnName(key) + ",");
                String pName = paras.AdaptParameterName(key) + index;
                paras.AddParameterValue(pName, FieldValues[key]);
                StrValues.Append(pName + ",");
                index++;
            }
            paramz = paras.Parameters;
            StrFileds.Remove(StrFileds.Length - 1, 1);
            StrValues.Remove(StrValues.Length - 1, 1);
            String SQL = String.Format(CommonSQL.SQL_INSERT, TableName, StrFileds, StrValues);
            return SQL;
        }
        /// <summary>
        /// 创建更新SQL
        /// </summary>
        /// <param name="paramz"></param>
        /// <param name="TableName"></param>
        /// <param name="FieldValues"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public virtual string CreateUpdateSQL(out System.Data.IDataParameter[] paramz, string TableName, Dictionary<string, object> FieldValues, Dictionary<string, object> Condition)
        {
            paramz = null;
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TableName is null");
            if (FieldValues == null || FieldValues.Count == 0)
                throw new ArgumentException("FieldValues is empty");
            StringBuilder StrFiledValues = new StringBuilder();
            IDataParameters paras = CreateDataParameters();
            int index = 0;
            foreach (String key in FieldValues.Keys)
            {
                String pName = paras.AdaptParameterName(key)+index;
                paras.AddParameterValue(pName, FieldValues[key]);
                String Segment = key + "=" + pName;
                StrFiledValues.Append(Segment + ",");
                index++;
            }
            StrFiledValues.Remove(StrFiledValues.Length - 1, 1);
            StringBuilder StrWhere = new StringBuilder();
            if (Condition != null && Condition.Count > 0)
            {
                foreach (String key in Condition.Keys)
                {
                    String pName = paras.AdaptParameterName(key)+index;
                    paras.AddParameterValue(pName, Condition[key]);
                    String Segment = key + "=" + pName;
                    StrWhere.Append(Segment + CommonSQL.AND);
                    index++;
                }
                StrWhere.Remove(StrWhere.ToString().LastIndexOf(CommonSQL.AND), CommonSQL.AND.Length);
            }
            if (StrWhere.Length == 0)
                StrWhere.Append("1<2");
            paramz = paras.Parameters;
            String SQL = String.Format(CommonSQL.UPDATE, TableName, StrFiledValues, StrWhere);
            return SQL;
        }
        /// <summary>
        /// 常见删除SQL
        /// </summary>
        /// <param name="paramz"></param>
        /// <param name="TableName"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public virtual string CreateDeleteSQL(out System.Data.IDataParameter[] paramz, string TableName, Dictionary<string, object> Condition)
        {
            paramz = null;
            if (String.IsNullOrEmpty(TableName))
                throw new ArgumentNullException("TableName is null");
            StringBuilder StrWhere = new StringBuilder();
            if (Condition != null && Condition.Count > 0)
            {
                IDataParameters paras = CreateDataParameters();
                int index = 0;
                foreach (String key in Condition.Keys)
                {
                    String pName = paras.AdaptParameterName(key) + index;
                    paras.AddParameterValue(pName, Condition[key]);
                    String Segment = key + "=" + pName;
                    StrWhere.Append(Segment + CommonSQL.AND);
                    index++;
                }
                StrWhere.Remove(StrWhere.ToString().LastIndexOf(CommonSQL.AND), CommonSQL.AND.Length);
                paramz = paras.Parameters;
            }
            if (StrWhere.Length == 0)
                StrWhere.Append("1<2");
            String SQL = String.Format(CommonSQL.SQL_DELETE, TableName, StrWhere);
            return SQL;
        }
        private IDataParameters CreateDataParameters()
        {
            return new DataParameters(m_Adapter);
        }

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
        public abstract ParameterizedSQL CreatePagedSQL(string SQL, int PageIndex, int PageSize, string KeyField, string OrderField, bool OrderByDesc, params System.Data.IDataParameter[] paras);

        /// <summary>
        /// 获取查询数据库时间的SQL
        /// </summary>
        /// <returns></returns>
        public abstract string GetDataBaseTimeSQL();

        /// <summary>
        /// 把时间转换为SQL中可识别的时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual string ConvertDateTimeToSQL(DateTime dt)
        {
            string dateString = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return "'" + dateString + "'";
        }
        
        /// <summary>
        /// 创建时间查询的SQL片段
        /// </summary>
        /// <param name="FieldName">查询字段名称</param>
        /// <param name="Op">查询操作符</param>
        /// <param name="dt">日期时间</param>
        /// <param name="IgnoreTime">是否忽略时间</param>
        /// <returns></returns>
        public virtual string CreateDateTimeSQLSegment(string FieldName, QueryOperatorEnum Op, DateTime dt, bool IgnoreTime)
        {
            if(String.IsNullOrEmpty(FieldName))
                throw new ArgumentNullException("FieldName is empty");
            if(Op== QueryOperatorEnum.In || Op== QueryOperatorEnum.Inline ||Op== QueryOperatorEnum.Like)
                throw new ArgumentException("日期查询不支持当前操作符:"+QueryOperator.ConvertToOperator(Op));
            if(!FieldName.EndsWith(" "))//添加一个空格
                FieldName+=" ";
            
            //按照时间精确查询
            if (!IgnoreTime)
            {
                return FieldName + QueryOperator.ConvertToOperator(Op) + ConvertDateTimeToSQL(dt);
            }

            //忽略时间，按照日期查询
            string Date = dt.ToString("yyyy-MM-dd");
            DateTime DtBegin = Convert.ToDateTime(Date + " 00:00:00");
            DateTime DtEnd = Convert.ToDateTime(Date + " 23:59:59");
            if (Op == QueryOperatorEnum.Equal)  //在这一天之内
                return "(" + FieldName + ">=" + ConvertDateTimeToSQL(DtBegin) + " AND " +
                     FieldName + "<=" + ConvertDateTimeToSQL(DtEnd) + ")";
            if (Op == QueryOperatorEnum.Great)  //比这一天大
                return FieldName + ">" + ConvertDateTimeToSQL(DtEnd);
            if(Op== QueryOperatorEnum.Less)     //比这一天小
                return FieldName + "<" + ConvertDateTimeToSQL(DtBegin);
            if (Op == QueryOperatorEnum.GreatAndEqual)//比这一天大，包括这一天
                return FieldName + ">=" + ConvertDateTimeToSQL(DtBegin);
            if (Op == QueryOperatorEnum.LessAndEqual)//比这一天小，包括这一天
                return FieldName + "<=" + ConvertDateTimeToSQL(DtEnd);
            if (Op == QueryOperatorEnum.NotEqual)//比这一天小或者比这一天大
                return "(" + FieldName + "<" + ConvertDateTimeToSQL(DtBegin) + " OR " +
                     FieldName + ">" + ConvertDateTimeToSQL(DtEnd) + ")";
            else
                throw new ArgumentException("日期查询不支持当前操作符:" + QueryOperator.ConvertToOperator(Op));
        }
    }
}
