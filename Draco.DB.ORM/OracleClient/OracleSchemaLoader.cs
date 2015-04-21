using System;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.ORM.OracleClient
{
    /// <summary>
    /// Oracle构架加载器
    /// </summary>
    public class OracleSchemaLoader : SchemaLoader
    {
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public override string DataServerType { get { return m_handler.DbContext.ConnectionInfo.DataServerType; } }
        private IDataBaseHandler m_handler;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public OracleSchemaLoader(IDataBaseHandler handler)
            : base(handler)
        { m_handler = handler; }
        /// <summary>
        /// 把数据类型映射到ADO数据类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected override int MapAdoType(IDataType dataType)
        {
            if (dataType == null)
                return (int)ADOType.adInteger;
            switch (dataType.Type.ToUpper())
            {
                case "BOOLEAN":
                case "BIT":
                    return (int)ADOType.adBoolean;
                case "INT":
                case "INTEGER":
                case "SMALLINT":
                    return (int)ADOType.adInteger;
                case "FLOAT":
                    return (int)ADOType.adSingle;
                case "DECIMAL":
                case "NUMBER":
                case "NUMERIC":
                    {
                        if ((dataType.Scale ?? 0) == 0)
                            return (int)ADOType.adInteger;//没有小数位
                        else if (((dataType.Scale ?? 0) <= 6))//6位一下的小数
                            return (int)ADOType.adDouble;
                        else
                            return (int)ADOType.adNumeric;
                    }
                case "TIME":
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                case "TIMESTAMP":
                    return (int)ADOType.adDBTimeStamp;
                case "CHAR":
                case "VARCHAR":
                case "VARCHAR2":
                case "NVARCHAR":
                case "NVARCHAR2":
                    return (int)ADOType.adChar;
                case "TEXT":
                case "NTEXT":
                case "CLOB":
                case "NCLOB":
                    return (int)ADOType.adLongVarChar;
                case "BLOB":
                case "BINARY":
                case "VARBINARY":
                case "IMAGE":
                    return (int)ADOType.adBinary;
                default:
                    return (int)ADOType.adInteger;
            }
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
