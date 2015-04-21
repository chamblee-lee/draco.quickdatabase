using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase;
using System.Data;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Mapping;

namespace Draco.DB.ORM.Common.AutoSQL
{
    /// <summary>
    /// 表达式SQL生成器
    /// </summary>
    public abstract class ExpSQLBuilder
    {
        /// <summary>
        /// 数据库适配器
        /// </summary>
        protected IORMAdapter m_DBAdapter;
        /// <summary>
        /// 映射关系
        /// </summary>
        protected Draco.DB.ORM.Mapping.ITableMapping m_Mapping;
        /// <summary>
        /// 查询表达式集合
        /// </summary>
        protected List<Expression> m_Expressions = new List<Expression>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="DbAdapter"></param>
        public ExpSQLBuilder(AbstractEntity entity,IORMAdapter DbAdapter)
        {
            m_DBAdapter = DbAdapter;
            m_Mapping = ORMAdapterCreator.MappingManager.GetMapping(entity.GetType());
        }

        /// <summary>
        /// 添加查询表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public virtual ExpSQLBuilder Add(Expression exp)
        {
            this.m_Expressions.Add(exp);
            return this;
        }
        /// <summary>
        /// 清空查询条件
        /// </summary>
        public virtual void Clear()
        {
            this.m_Expressions.Clear();
        }

        #region Express操作
        /// <summary>
        /// 把查询表达式集合转换为SQL
        /// </summary>
        /// <param name="Paras"></param>
        /// <returns></returns>
        public virtual string ConvertExpressToSQL(out IDataParameter[] Paras)
        {
            Paras = null;
            if (m_Expressions.Count > 0)
            {
                StringBuilder sBuilder = new StringBuilder();
                IDataParameters Pas = CreateDataParameters(m_DBAdapter);
                foreach (Expression exp in m_Expressions)
                {
                    sBuilder.Append(ConvertOneExpressToSQL(exp, Pas) + CommonSQL.AND);
                }
                sBuilder.Remove(sBuilder.Length - CommonSQL.AND.Length, CommonSQL.AND.Length);
                Paras = Pas.Parameters;
                return sBuilder.ToString();
            }
            return "";
        }
        /// <summary>
        /// 把一个条件表达式转换为SQL
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertOneExpressToSQL(Expression exp, IDataParameters Paras)
        {
            IFieldMapping FiledMapping = m_Mapping.GetFieldMapping(exp.PropertyName) as IFieldMapping;//字段映射
            switch (exp.OP)
            {
                case OP.Eq: return ConvertEqToSQL(FiledMapping, exp.value, Paras);
                case OP.Gt: return ConvertGtToSQL(FiledMapping, exp.value, Paras);
                case OP.GtAndEq: return ConvertGtAndEqToSQL(FiledMapping, exp.value, Paras);
                case OP.Lt: return ConvertLtToSQL(FiledMapping, exp.value, Paras);
                case OP.LtAndEq: return ConvertLtAndEqToSQL(FiledMapping, exp.value, Paras);
                case OP.Like: return ConvertLikeToSQL(FiledMapping, exp.value, Paras);
                case OP.Between: return ConvertBetweenToSQL(FiledMapping, exp.value, exp.value2, Paras);
                case OP.IsNotNull: return ConvertIsNotNullToSQL(FiledMapping);
                case OP.IsNull: return ConvertIsNullToSQL(FiledMapping);
                case OP.Or: return ConvertOrToSQL(exp, Paras);
                default: throw new Exception("不被支持的运算类型:" + exp.OP.ToString());
            }
        }
        /// <summary>
        /// 转换一个OR运算
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertOrToSQL(Expression exp, IDataParameters Paras)
        {
            if (exp.Or_Expressions != null && exp.Or_Expressions.Count > 0)
            {
                StringBuilder sBuilder = new StringBuilder();
                foreach (Expression iexp in exp.Or_Expressions)
                {
                    sBuilder.Append(ConvertOneExpressToSQL(exp, Paras) + CommonSQL.OR);
                }
                sBuilder.Remove(sBuilder.Length - CommonSQL.OR.Length, CommonSQL.OR.Length);
                return "(" + sBuilder.ToString() + ")";
            }
            return "";
        }
        #endregion

        #region SQL片段转换

        /// <summary>
        /// 转换一个等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "=" + ParameterName;           //等于SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个大于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertGtToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + ">" + ParameterName;           //大于SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个小于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertLtToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "<" + ParameterName;           //小于SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个大于等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertGtAndEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + ">=" + ParameterName;           //大于等于SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个小于等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertLtAndEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "<=" + ParameterName;           //小于等于SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个空运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <returns></returns>
        protected virtual string ConvertIsNullToSQL(IFieldMapping FiledMapping)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " IS NULL ";                   //空SQL子句
            return Sentence;
        }
        /// <summary>
        /// 转换一个非空运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <returns></returns>
        protected virtual string ConvertIsNotNullToSQL(IFieldMapping FiledMapping)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " IS NOT NULL ";                   //空SQL子句
            return Sentence;
        }
        /// <summary>
        /// 转换一个Like运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertLikeToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = Paras.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " LIKE " + ParameterName;           //Like SQL子句

            Paras.AddParameterValue(ParameterName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个Between运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string ConvertBetweenToSQL(IFieldMapping FiledMapping, object value1, object value2, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName1 = Paras.AdaptParameterName(ColName + "_1");                         //参数1名称
            string ParameterName2 = Paras.AdaptParameterName(ColName + "_2");                         //参数2名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " Between " + ParameterName1 + " AND " + ParameterName2;           //Like SQL子句

            Paras.AddParameterValue(ParameterName1, value1, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            Paras.AddParameterValue(ParameterName2, value2, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        #endregion

        /// <summary>
        /// 创建IDataParameters
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        protected IDataParameters CreateDataParameters(IORMAdapter ad)
        {
            return new DataParameters(ad);
        }
        /// <summary>
        /// 创建IFieldTypeConvert
        /// </summary>
        protected virtual IFieldTypeConvert InnerFieldTypeConvert
        {
            get { return new FieldTypeConvert(); }
        }
    }
}
