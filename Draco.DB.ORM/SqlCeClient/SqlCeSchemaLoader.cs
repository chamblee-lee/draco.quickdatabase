using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.SqlServerClient;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqlCeClient
{
    class SqlCeSchemaLoader : SqlSchemaLoader
    {
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public override string DataServerType { get { return m_Provider.DbContext.ConnectionInfo.ConnectionString; } }
        private IDataBaseHandler m_Provider;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="Handler"></param>
        public SqlCeSchemaLoader(IDataBaseHandler Handler)
            : base(Handler)
        { m_Provider = Handler; }

    }
}
