using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.SqlClient;
using Draco.DB.QuickDataBase.Adapter;

namespace Draco.DB.QuickDataBase.SqlCeClient
{
    class SqlCeSQLGenerator : SQLServerSQLGenerator
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="adapter"></param>
        public SqlCeSQLGenerator(IDataBaseAdapter adapter)
            : base(adapter){}
    }
}
