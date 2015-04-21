using System;
using System.Data;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.OracleClient
{
    /// <summary>
    /// Oracle的SQLBuilder实现
    /// </summary>
    public class SQLBuilderOracle : SQLBuilder
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Adapter"></param>
        public SQLBuilderOracle(AbstractEntity entity, IORMAdapter Adapter)
            : base(entity, Adapter)
        {}

        #region 查询SQL部分
        /// <summary>
        /// 组建SQL
        /// </summary>
        /// <param name="ReturnField">返回字段</param>
        /// <param name="Where">查询条件</param>
        /// <returns></returns>
        protected override string AssemblySelectSQL(string ReturnField, string Where)
        {
            const string SQLTemp = " SELECT {0} FROM {1} WHERE {2} {3} ";
            if (String.IsNullOrEmpty(Where))
                Where = "1=1";
            string SQL = "";
            string OrderBy = "";
            //排序子句
            if (!String.IsNullOrEmpty(m_OrderBy))//需要排序
            {
                IFieldMapping Mapping = m_Mapping.GetFieldMapping(m_OrderBy);
                OrderBy = GeneratorOrderSyntax(Mapping.ColumnName, m_OrderByDesc);
            }
            //基本SQL语句
            SQL = String.Format(SQLTemp, ReturnField, AdaptedTableName, Where, OrderBy);

            if (m_FirstResult > 0 && m_MaxResult > 0)//分页SQL
            {
                const string PagerTemplate = "select  * from (select t.*,rownum as ra from ({0})t where  rownum <{1}) t2  where  ra>={2} ";
                int Count = m_MaxResult;
                int First = m_FirstResult;
                int Last = First + Count;
                SQL = String.Format(PagerTemplate, SQL, Last, First);
            }
            return SQL;
        }
        
        /// <summary>
        /// 创建排序子句
        /// </summary>
        /// <returns></returns>
        private string GeneratorOrderSyntax(string ColumnName,bool Desc)
        {
            string ColName = m_DBAdapter.AdaptColumnName(ColumnName);
            string orderSentence = " ORDER BY " + ColName;
            if (Desc)
                orderSentence += " DESC ";
            else
                orderSentence += " ASC ";
            return orderSentence;
        }
        #endregion

        #region 构造数据库相关的主键生成器
        /// <summary>
        /// 构造数据库相关的主键生成器
        /// </summary>
        /// <param name="DbHandle"></param>
        /// <returns></returns>
        public override IPKGenerator CreatePKGenerator(IDataBaseHandler DbHandle)
        {
            switch (m_Mapping.Generator.ToUpper())
            {
                case PKGeneratorEnum.LOCAL:
                    PKGeneratorOraSequence pg = new PKGeneratorOraSequence();
                    pg.DBHandler = DbHandle;
                    return pg;
                case PKGeneratorEnum.ORM_SEQUENCE:
                    PKGeneratorOraORMSequence pgo = new PKGeneratorOraORMSequence();
                    pgo.DBHandler = DbHandle;
                    return pgo;
                default: return base.CreatePKGenerator(DbHandle);
            }
        }
        #endregion
    }
}
