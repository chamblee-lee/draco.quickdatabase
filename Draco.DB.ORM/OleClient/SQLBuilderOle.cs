using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.ORM.OleClient
{
    /// <summary>
    /// SQLBuilder的Ole实现
    /// </summary>
    public class SQLBuilderOle : SQLBuilder 
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Adapter"></param>
        public SQLBuilderOle(AbstractEntity entity,IORMAdapter Adapter)
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

        #region SQL片段转换
        /// <summary>
        /// 转换一个等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected override string ConvertEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "=" + ParameterName;           //等于SQL子句

            Paras.AddParameterValue(ColName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个大于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected override string ConvertGtToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
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
        protected override string ConvertLtToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "<" + ParameterName;           //小于SQL子句

            Paras.AddParameterValue(ColName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个大于等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected override string ConvertGtAndEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + ">=" + ParameterName;           //大于等于SQL子句

            Paras.AddParameterValue(ColName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个小于等于运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected override string ConvertLtAndEqToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + "<=" + ParameterName;           //小于等于SQL子句

            Paras.AddParameterValue(ColName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
        }
        /// <summary>
        /// 转换一个Like运算
        /// </summary>
        /// <param name="FiledMapping"></param>
        /// <param name="value"></param>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected override string ConvertLikeToSQL(IFieldMapping FiledMapping, object value, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName = m_DBAdapter.AdaptParameterName(ColName);                         //参数名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " LIKE " + ParameterName;           //Like SQL子句

            Paras.AddParameterValue(ColName, value, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
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
        protected override string ConvertBetweenToSQL(IFieldMapping FiledMapping, object value1, object value2, IDataParameters Paras)
        {
            string ColName = FiledMapping.ColumnName;                                   //数据库列名
            string ParameterName1 = m_DBAdapter.AdaptParameterName(ColName + "_1");                         //参数1名称
            string ParameterName2 = m_DBAdapter.AdaptParameterName(ColName + "_2");                         //参数2名称
            string Sentence = m_DBAdapter.AdaptColumnName(ColName) + " Between " + ParameterName1 + " AND " + ParameterName2;           //Like SQL子句

            Paras.AddParameterValue(ColName + "_1", value1, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            Paras.AddParameterValue(ColName + "_2", value2, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
            return Sentence;
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
                case PKGeneratorEnum.LOCAL: return new PKGeneratorOleAuto();
                case PKGeneratorEnum.ORM_SEQUENCE:
                    PKGeneratorOleORMSequence pkg = new PKGeneratorOleORMSequence();
                    pkg.DBHandler = DbHandle;
                    return pkg;
                default: return base.CreatePKGenerator(DbHandle);
            }
        }
        #endregion
    }
}
