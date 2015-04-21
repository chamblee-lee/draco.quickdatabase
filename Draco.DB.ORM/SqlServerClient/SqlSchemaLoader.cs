using System;
using System.Text;
using Draco.DB.ORM.Schema;
using Draco.DB.ORM.Schema.Dbml;
using Draco.DB.ORM.Schema.Vendor.Implementation;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.ORM.SqlServerClient
{
    /// <summary>
    /// Sql构架加载器
    /// </summary>
    public class SqlSchemaLoader : SchemaLoader
    {
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        public override string DataServerType { get { return m_Provider.DbContext.ConnectionInfo.ConnectionString; } }
        private IDataBaseHandler m_Provider;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public SqlSchemaLoader(IDataBaseHandler handler)
            : base(handler)
        { m_Provider = handler; }

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
                case "BIGINT":
                    return (int)ADOType.adBigInt;
                case "INTEGER":
                case "INT":
                case "SMALLINT":
                    return (int)ADOType.adInteger;
                case "FLOAT":
                case "REAL":
                    return (int)ADOType.adDouble;
                case "DECIMAL":
                case "NUMERIC":
                    {
                        if ((dataType.Scale ?? 0) == 0)
                            return (int)ADOType.adInteger;//没有小数位
                        else if (((dataType.Scale ?? 0) <= 6))//6位一下的小数
                            return (int)ADOType.adDouble;
                        else
                            return (int)ADOType.adNumeric;
                    }
                case "BOOLEAN":
                case "BIT":
                    return (int)ADOType.adBoolean;
                case "CHAR":
                case "VARCHAR":
                case "NVARCHAR":
                    return (int)ADOType.adChar;
                case "TIME":
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                case "TIMESTAMP":
                    return (int)ADOType.adDBTimeStamp;
                case "TEXT":
                case "NTEXT":
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
        /// 从Dbml创建表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="SchemaProvider"></param>
        /// <returns></returns>
        protected override bool CreateTableFromSchemal(Table table, string SchemaProvider)
        {
            string SQLTemplate = "Create Table {0}({1})";
            StringBuilder sBuilder = new StringBuilder();
            foreach (var Col in table.Type.Columns)
            {
                string Field = Col.Name;
                string TypeName = GetFiledTypeString(Col, SchemaProvider);
                string NULLAble = Col.CanBeNull ? "NULL" : "NOT NULL";
                string Primary = Col.IsPrimaryKey ? "Primary Key" : "";
                sBuilder.Append(Field + " " + TypeName + " " + Primary + " " + NULLAble + ",");
            }
            sBuilder = sBuilder.Remove(sBuilder.Length - 1, 1);//去掉最后一个逗号
            string sql = String.Format(SQLTemplate, table.Name, sBuilder.ToString());
            m_Provider.ExecuteNonQuery(sql);
            return true;
        }

        private string GetFiledTypeString(Column Col, string SchemaProvider)
        {
            bool UseSame = String.Compare(SchemaProvider, DataServerType) == 0;
            UseSame = false;
            if (UseSame)
            {

            }
            else
            {
                switch (Col.ADOType)
                {
                    case (int)ADOType.adBigInt: return "bigint";
                    case (int)ADOType.adInteger: return "int";
                    case (int)ADOType.adDouble: return "decimal(18,6)";
                    case (int)ADOType.adNumeric: return "decimal(18,9)";
                    case (int)ADOType.adBoolean: return "bit";
                    case (int)ADOType.adDBTimeStamp: return "datetime";
                    case (int)ADOType.adChar: 
                    case (int)ADOType.adLongVarChar: 
                        {
                            if (Col.FieldLength < 8000)
                                return "varchar(" + Col.FieldLength + ")";
                            else
                                return "text";
                        }
                    case (int)ADOType.adBinary:
                        {
                            if (Col.FieldLength < 255)
                                return "binary(" + Col.FieldLength + ")";
                            else
                                return "image";
                        }
                    default:
                        return "varchar(255)";
                }
            }
            return "varchar";
        }
    }
}
