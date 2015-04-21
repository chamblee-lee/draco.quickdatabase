using System;
using System.Data;
using System.Data.Common;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.SqLiteClient
{
    /// <summary>
    /// SQLite适配器
    /// </summary>
    public class SQLiteAdapter : DataBaseAdapter
    {
        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        public override DbProviderFactory DbFactory
        {
            get 
            {
                String providerName = String.IsNullOrEmpty(this.DbProviderName) ? "System.Data.SQLite" : this.DbProviderName;
                return DbProviderFactories.GetFactory(providerName);
            }
        }
        /// <summary>
        /// 适配列名
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
        /// 适配参数名
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public override string AdaptColumnName(string ColumnName)
        {
            if (!String.IsNullOrEmpty(ColumnName))
            {
                if (!ColumnName.StartsWith("\"") && !ColumnName.EndsWith("\""))
                    return "\"" + ColumnName + "\"";
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
            return new SQLiteSchemaHandler(Handler);
        }
        /// <summary>
        /// 创建IDataParameter
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public override IDataParameter CreateParameter(string paraName, object val)
        {
            IDataParameter para = DbFactory.CreateParameter();
            para.ParameterName = paraName;
            para.Value = val;
            return para;
        }
        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        public override ISQLGenerator GetSQLGenerator()
        {
            return new SQLiteSQLGenerator(this);
        }
    }
}
