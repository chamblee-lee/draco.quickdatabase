using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.SqlServerClient;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Adapter;

namespace Draco.DB.ORM.SqlCeClient
{
    class SQLBuilderSqlCe : SQLBuilderSQLServer
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="Adapter"></param>
        public SQLBuilderSqlCe(AbstractEntity entity, IORMAdapter Adapter)
            : base(entity, Adapter)
        {}
    }
}
