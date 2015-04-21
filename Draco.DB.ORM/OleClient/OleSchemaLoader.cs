using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Collections;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema.Vendor.Implementation;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Utility;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.ORM.OleClient
{
    /// <summary>
    /// Ole构架加载器
    /// </summary>
    public class OleSchemaLoader : SchemaLoader
    {
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public override string DataServerType { get { return m_Handler.DbContext.ConnectionInfo.DataServerType; } }

        private IDataBaseHandler m_Handler;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public OleSchemaLoader(IDataBaseHandler handler)
            : base(handler)
        {
            m_Handler = handler;
        }

        /// <summary>
        /// 把数据类型映射到ADO数据类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected override int MapAdoType(IDataType dataType)
        {
            if (dataType == null)
                return (int)ADOType.adInteger;
            ADOType AdoType = (ADOType)Enum.Parse(typeof(ADOType), dataType.Type);
            if (AdoType == ADOType.adNumeric || AdoType== ADOType.adDecimal)
            {
                if ((dataType.Scale ?? 0) == 0)
                    AdoType = ADOType.adInteger;//没有小数位
                else if (((dataType.Scale ?? 0) <= 6))//6位一下的小数
                    AdoType = ADOType.adDouble;
                else
                    AdoType = ADOType.adNumeric;
            }
            return (int)AdoType;
        }

        /// <summary>
        /// 由构架信息创建数据库表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="SchemaProvider"></param>
        /// <returns></returns>
        protected override bool CreateTableFromSchemal(Draco.DB.ORM.Schema.Dbml.Table table, string SchemaProvider)
        {
            throw new NotImplementedException();
        }
    }
}
