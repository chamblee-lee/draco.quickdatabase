using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.ORM.Mapping;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqlServerClient
{
    /// <summary>
    /// SQLBuilder的SQLServer实现
    /// </summary>
    public class SQLBuilderSQLServer : SQLBuilder 
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Adapter"></param>
        public SQLBuilderSQLServer(AbstractEntity entity, IORMAdapter Adapter)
            : base(entity, Adapter)
        {}

        /// <summary>
        /// 所有字段
        /// </summary>
        public override string AllFields
        {
            get { return "*"; }
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
            if (String.IsNullOrEmpty(Where))
                Where = "1=1";
            if (m_FirstResult > 0 && m_MaxResult > 0)//分页SQL
            {
                int Count = m_MaxResult;
                int First = m_FirstResult;
                if (m_Mapping.PrimaryKeyCollection.Count == 0 || m_Mapping.PrimaryKeyCollection.Count > 1)
                {
                    throw new Exception("分页SQL要求数据表必须存在单主键");
                }
                string KeyField = m_Mapping.PrimaryKeyCollection[0].ColumnName;
                string OrderBy = GeneratorOrderSyntax(KeyField, true);
                string PagerSQL = " SELECT TOP {0} {1} FROM {2} WHERE ({3}) AND ({4} NOT IN (SELECT TOP {5} {6} FROM {7} WHERE {8} {9})){10} ";
                string SQL = String.Format(PagerSQL, Count, ReturnField, AdaptedTableName, Where, KeyField, First - 1,
                                    KeyField, AdaptedTableName, Where, OrderBy, OrderBy);
                return SQL;
            }
            else//
            {
                string OrderBy = "";
                //排序子句
                if (!String.IsNullOrEmpty(m_OrderBy))//需要排序
                {
                    IFieldMapping Mapping = m_Mapping.GetFieldMapping(m_OrderBy);
                    OrderBy = GeneratorOrderSyntax(Mapping.ColumnName, m_OrderByDesc);
                }
                string SQLTemp = " SELECT {0} FROM {1} WHERE {2} {3} ";
                string SQL = String.Format(SQLTemp, ReturnField, AdaptedTableName, Where, OrderBy);
                return SQL;
            }
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
                IDataParameter para = new SqlParameter("@IDENTITY", 0);
                para.Direction = ParameterDirection.Output;
                Paras = base.CombineWhereParas(Paras, new IDataParameter[] { para });
                SQL += " set @IDENTITY=(select @@IDENTITY) ";
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
                                    object _value = Convert.ChangeType(para.Value, Property.PropertyType);
                                    Property.SetValue(m_Entity, _value, null);
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
                case PKGeneratorEnum.LOCAL: return new PKGeneratorSQLServerAuto();
                case PKGeneratorEnum.ORM_SEQUENCE:
                    PKGeneratorSQLServerORMSequence pkg = new PKGeneratorSQLServerORMSequence();
                    pkg.DBHandler = DbHandle;
                    return pkg;
                default: return base.CreatePKGenerator(DbHandle);
            }
        }
        #endregion
    }
}
