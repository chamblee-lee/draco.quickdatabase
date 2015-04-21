using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Adapter;
using System.Data.Common;
using System.Collections;
using System.Data;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// IDataBaseAdapter抽象基类
    /// </summary>
    public abstract class DataBaseAdapter : IDataBaseAdapter
    {
        /// <summary>
        /// 数据库类型名称
        /// </summary>
        protected String m_DbTypeName;
        /// <summary>
        ///  获取数据库提供器名称
        /// </summary>
        protected String m_DbProviderName;

        /// <summary>
        /// 获取数据库类型名称(SQLSERVER/ORACLE/OLE...)
        /// </summary>
        public virtual String DbTypeName 
        { 
            get { return m_DbTypeName; }
            set { m_DbTypeName = value; }
        }
        /// <summary>
        /// 获取数据库提供器名称(System.Data.SqlClient/System.Data.OracleClient/Oracle.DataAccess.Client/...)
        /// </summary>
        public virtual String DbProviderName
        {
            get { return m_DbProviderName; }
            set { m_DbProviderName = value; }
        }

        /// <summary>
        /// 获取DbFactory
        /// </summary>
        public abstract DbProviderFactory DbFactory{get;}
        /// <summary>
        /// 获取GetDbHandler
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public abstract IDataBaseHandler GetDbHandler(IDataBaseContext Context);
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        public abstract string AdaptParameterName(string paraName);
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="reservedNames">保留的参数名(防止参数同名)</param>
        /// <returns></returns>
        public string AdaptParameterName(string paraName, String[] reservedNames)
        {
            String aName = AdaptParameterName(paraName);
            int counter = 0;
            String newParaName = aName;
            while (ArrayHelper.Contains(reservedNames, newParaName, true))
            {
                newParaName = aName + counter;
                counter++;
            }
            return newParaName;
        }
        /// <summary>
        /// 适配列名称
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public abstract string AdaptColumnName(string ColumnName);
        /// <summary>
        /// 适配SQL中的匿名参数
        /// </summary>
        /// <param name="SQL">包含匿名参数的SQL语句</param>
        /// <param name="param">参数对象列表</param>
        /// <returns></returns>
        public virtual ParameterizedSQL AdaptSQLAnonymousParams(string SQL, params object[] param)
        {
            
            System.Data.IDataParameter[] IParams;
            IParams = null;
            if (!String.IsNullOrEmpty(SQL) && param != null && param.Length > 0)
            {
                ParameterizedSQL ps = new ParameterizedSQL();
                if (SQL.EndsWith("?"))
                    SQL += " ";//这里添加一个空格，避免Split后，数组个数错误
                String[] parts = SQL.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length - 1 != param.Length)
                    throw new ArgumentException("SQL中匿名参数个数与参数列表不匹配");

                StringBuilder sBuilder = new StringBuilder(parts[0]);
                ArrayList arr = new ArrayList();
                for (int i = 1; i < parts.Length; i++)
                {
                    string paraName =this.AdaptParameterName("p" + i);
                    sBuilder.Append(paraName + parts[i]);
                    arr.Add(CreateParameter(paraName,param[i-1]));
                }
                IParams =(IDataParameter[]) arr.ToArray(typeof(IDataParameter));
                ps.SQL = sBuilder.ToString();
                ps.Parameters = IParams;
                return ps;
            }
            //没有加工
            ParameterizedSQL defaultps = new ParameterizedSQL();
            defaultps.SQL = SQL;
            return defaultps;
        }

        /// <summary>
        /// 获取数据库构架信息操作接口类型
        /// </summary>
        /// <param name="Handler"></param>
        /// <returns></returns>
        public abstract IDataBaseSchemaHandler GetSchemaHandler(IDataBaseHandler Handler);
        /// <summary>
        /// 创建DataParameter对象
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public abstract IDataParameter CreateParameter(string paraName, object val);
        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        public abstract ISQLGenerator GetSQLGenerator();
        
        private NamedSQLConfig m_OnlyOneNamedSQLConfig;
        /// <summary>
        /// 获取命名SQL配置对象
        /// </summary>
        public virtual INamedSQLConfig NamedSQLs
        {
            get
            {
                if (m_OnlyOneNamedSQLConfig == null)
                {
                    m_OnlyOneNamedSQLConfig = new NamedSQLConfig();
                    m_OnlyOneNamedSQLConfig.NoConfigurationLoad(this.DbTypeName);
                }
                return m_OnlyOneNamedSQLConfig;
            }
        }

        #region IDataBaseAdapter Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetIsolationLevelSql()
        {           
            return null;
        }

        #endregion
    }
}
