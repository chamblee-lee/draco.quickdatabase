using System;
using System.Data;
using System.Data.Common;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.SqlClient;

namespace Draco.DB.QuickDataBase.SqlCeClient
{
    public class SqlCeAdapter : SqlServerAdapter
    {
        public override System.Data.Common.DbProviderFactory DbFactory
        {
            get
            {
                String invariant = this.DbProviderName;
                if (String.IsNullOrEmpty(invariant))
                {
#if NET2_0
                    invariant = "System.Data.SqlServerCe.3.5";
#elif NET4_0
                    invariant = "System.Data.SqlServerCe.4.0";
#endif
                }
                return DbProviderFactories.GetFactory(invariant);
            }
        }

        public override string GetIsolationLevelSql()
        {
            return null;
        }

        /// <summary>
        /// 获取SQL生成器
        /// </summary>
        /// <returns></returns>
        public override ISQLGenerator GetSQLGenerator()
        {
            return new SqlCeSQLGenerator(this);
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
            return new SqlCeSchemaHandler(Handler);
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
    }
}
