using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data.Common;
using System.Data;
using Draco.DB.QuickDataBase.Adapter;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.OleClient
{
    /// <summary>
    /// Ole适配器
    /// </summary>
    public class OleAdapter : DataBaseAdapter
    {
        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        public override DbProviderFactory DbFactory 
        { 
            get 
            {
                String providerName = String.IsNullOrEmpty(this.DbProviderName) ? "System.Data.OleDb" : this.DbProviderName;
                return DbProviderFactories.GetFactory(providerName); 
            } 
        }
        /// <summary>
        /// 适配参数名称
        /// </summary>
        /// <param name="paraName"></param>
        /// <returns></returns>
        public override string AdaptParameterName(string paraName)
        {
            return "?";
        }
        /// <summary>
        /// 适配列名
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public override string AdaptColumnName(string ColumnName)
        {
            return ColumnName;
        }
        /// <summary>
        /// 获取IDataBaseHandler
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public override IDataBaseHandler GetDbHandler(IDataBaseContext Context)
        {
            return new DataBaseHandler(Context, this);
        }
        /// <summary>
        /// 获取匿名参数SQL
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ParameterizedSQL AdaptSQLAnonymousParams(string SQL, params object[] param)
        {
            System.Data.IDataParameter[] IParams;
           // IParameter IParams = null;
            ParameterizedSQL pSql = new ParameterizedSQL();
            if (param != null && param.Length > 0)
            {
                int i = 1;
                IDataParameters ps = new DataParameters(this);
                foreach (object p in param)
                {
                    if (p != null)
                    {
                        ps.AddParameterValue("p" + i, p);
                        i++;
                    }
                }
                IParams = ps.Parameters;
                pSql.Parameters = IParams;
            }
            pSql.SQL = SQL;
            return pSql;
        }
        /// <summary>
        /// 获取IDataBaseSchemaHandler
        /// </summary>
        /// <param name="Handler"></param>
        /// <returns></returns>
        public override IDataBaseSchemaHandler GetSchemaHandler(IDataBaseHandler Handler)
        {
            return new OleSchemaHandler(Handler);
        }
        /// <summary>
        /// 创建IDataParameter
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public override IDataParameter CreateParameter(string paraName, object val)
        {
            return new OleDbParameter(paraName, val);
        }
        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        public override ISQLGenerator GetSQLGenerator()
        {
            return new OleSQLGenerator(this);
        }
    }
}
