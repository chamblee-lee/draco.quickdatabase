using System;
using System.Data;
using System.Reflection;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqLiteClient
{
    /// <summary>
    /// SQLBuilder的SqLite实现
    /// </summary>
    public class SQLBuilderSQLite : SQLBuilder 
    {
        private IORMAdapter _Adapter;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Adapter"></param>
        public SQLBuilderSQLite(AbstractEntity entity, IORMAdapter Adapter)
            : base(entity, Adapter)
        {
            this._Adapter = Adapter;
        }

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
            if (Where == null || Where.Trim().Length <= 0)
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
                const string PagerTemplate = "{0} limit {1},{2} ";
                int Count = m_MaxResult;
                int First = m_FirstResult;
                SQL = String.Format(PagerTemplate, SQL, First - 1, Count);
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

        #region 插入SQL
        /// <summary>
        /// 创建插入SQL
        /// </summary>
        /// <param name="Paras"></param>
        /// <returns></returns>
        public override string CreateInsertSQL(out IDataParameter[] Paras)
        {
            string SQL = base.CreateInsertSQL(out Paras);
            if (String.Compare(m_Mapping.Generator, PKGeneratorEnum.LOCAL, true) == 0)
            {
                //自增
                IDataParameter para = _Adapter.CreateParameter("@IDENTITY", 0);
                para.Direction = ParameterDirection.Output;
                base.CombineWhereParas(Paras, new IDataParameter[] { para });
                SQL += " set @IDENTITY=(select last_insert_rowid()) ";
            }
            return SQL;
        }
        /// <summary>
        /// 处理传出参数中的自增主键
        /// </summary>
        /// <param name="paras"></param>
        public override void ProcessOutputParameters(IDataParameter[] paras)
        {
            base.ProcessOutputParameters(paras);
            if (String.Compare(m_Mapping.Generator, PKGeneratorEnum.LOCAL, true) == 0)
            {
                if (paras != null && paras.Length > 0)
                {
                    foreach (IDataParameter para in paras)
                    {
                        if (para.Direction == ParameterDirection.Output || para.Direction == ParameterDirection.InputOutput)
                        {
                            //准备输出，这里只处理ID
                            if (String.Compare(para.ParameterName, "@IDENTITY", true) == 0)
                            {
                                string primaryPropertyName = m_Mapping.PrimaryKeyCollection[0].PropertyName;
                                PropertyInfo Property = m_Entity.GetType().GetProperty(primaryPropertyName);
                                if (Property != null)
                                {
                                    Property.SetValue(m_Entity, para.Value, null);
                                }
                            }
                        }
                    }
                }
            }
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
                    return new PKGeneratorSqLiteAuto();
                case PKGeneratorEnum.ORM_SEQUENCE:
                    PKGeneratorSqLiteORMSequence pg = new PKGeneratorSqLiteORMSequence();
                    pg.DBHandler = DbHandle;
                    return pg;
                default: return base.CreatePKGenerator(DbHandle);
            }
        }
        #endregion
    }
}
