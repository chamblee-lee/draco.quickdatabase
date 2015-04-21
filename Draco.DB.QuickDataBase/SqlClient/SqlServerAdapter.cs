using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.SqlClient
{
    /// <summary>
    /// SQLServer适配器
    /// </summary>
    public class SqlServerAdapter : DataBaseAdapter
    {
        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        public override DbProviderFactory DbFactory
        {
            get
            {
                String providerName = String.IsNullOrEmpty(this.DbProviderName) ? "System.Data.SqlClient" : this.DbProviderName;
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
            if (String.IsNullOrEmpty(paraName))
                throw new ArgumentNullException("paraName");
            paraName = CharacterHelper.ConvertToLetter(paraName);
            if (!paraName.StartsWith("@"))
                return "@" + paraName;
            return paraName;
        }
        /// <summary>
        /// 适配数据列
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public override string AdaptColumnName(string ColumnName)
        {
            if (!String.IsNullOrEmpty(ColumnName))
            {
                if (!ColumnName.StartsWith("[") && !ColumnName.EndsWith("]"))
                    return "[" + ColumnName + "]";
            }
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
        /// 获取IDataBaseSchemaHandler
        /// </summary>
        /// <param name="Handler"></param>
        /// <returns></returns>
        public override IDataBaseSchemaHandler GetSchemaHandler(IDataBaseHandler Handler)
        {
            return new SqlSchemaHandler(Handler);
        }
        /// <summary>
        /// 创建IDataParameter
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public override IDataParameter CreateParameter(string paraName, object val)
        {
            return new SqlParameter(paraName, val);
        }
        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        public override ISQLGenerator GetSQLGenerator()
        {
            return new SQLServerSQLGenerator(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetIsolationLevelSql()
        {
            //return IsolationLevel.Unspecified;
        //    return null;
            return "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";

            //return null;
            //throw new NotImplementedException();
        }
    }
}
