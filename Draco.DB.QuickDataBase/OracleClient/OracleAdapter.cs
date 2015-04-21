using System;
using System.Data;
using System.Data.Common;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Utility;

namespace Draco.DB.QuickDataBase.OracleClient
{
    /// <summary>
    /// Oracle适配器
    /// </summary>
    public class OracleAdapter : DataBaseAdapter
    {
        private static String _invariant = "";
        private static String InvariantName
        {
            get 
            {
                if (String.IsNullOrEmpty(_invariant))
                {
                    DataTable dt = DbProviderFactories.GetFactoryClasses();
                    foreach (DataRow row in dt.Rows)
                    {
                        String invariant = Convert.ToString(row["InvariantName"]);
                        //优先采用ODP.NET
                        if ("Oracle.DataAccess.Client" == invariant)
                        {
                            _invariant = invariant;
                            break;
                        }
                    }
                    if (String.IsNullOrEmpty(_invariant))
                        _invariant = "System.Data.OracleClient";
                }
                return _invariant;
            }
        }

        /// <summary>
        /// 获取DbProviderFactory
        /// </summary>
        public override DbProviderFactory DbFactory
        {
            get 
            {
                String providerName = String.IsNullOrEmpty(this.DbProviderName) ? InvariantName : this.DbProviderName;
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
            if (IsKeyWord(paraName))
                paraName = paraName + "_p";
            if (!paraName.StartsWith(":"))
                return ":" + paraName;
            return paraName;
        }
        /// <summary>
        /// 适配列名
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public override string AdaptColumnName(string ColumnName)
        {
            if (!String.IsNullOrEmpty(ColumnName))
            {
                if (ColumnName.StartsWith("\"") && ColumnName.EndsWith("\""))
                    return ColumnName;
                //如果包含特殊字符，则添加引号(大小写敏感)
                char c = ColumnName[0];
                if(c=='_'||c=='$'||ColumnName.Contains(","))
                    return "\"" + ColumnName + "\"";
                //如果是关键字，则添加引号(大小写敏感)
                if(IsKeyWord(ColumnName))
                    return "\"" + ColumnName.ToUpper() + "\"";
            }
            return ColumnName;//普通字符就不处理，这样字段大小写就不敏感
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
            return new OracleSchemaHandler(Handler);
        }

        /// <summary>
        /// 创建DataParameter
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public override IDataParameter CreateParameter(string paraName, object val)
        {
            IDataParameter para = DbFactory.CreateParameter(); //new OracleParameter(paraName, val);
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
            return new OracleSQLGenerator(this);
        }

        private bool IsKeyWord(string FieldName)
        {
            return OracleKeyWords.Contains(" " + FieldName.ToUpper() + " ");
        }
        /// <summary>
        /// Oracle关键字
        /// </summary>
        private const string OracleKeyWords = @" ACCESS ADD  ALL  ALTER  AND  ANY  AS  ASC  AUDIT  BETWEEN 
            BY  CHAR CHECK  CLUSTER  COLUMN  COMMENT  COMPRESS  CONNECT  CREATE  CURRENT DATE  DECIMAL 
            DEFAULT  DELETE  DESC  DISTINCT  DROP  ELSE  EXCLUSIVE EXISTS  FILE  FLOAT FOR 
            FROM  GRANT  GROUP  HAVING  IDENTIFIED IMMEDIATE  IN  INCREMENT  INDEX  INITIAL 
            INSERT  INTEGER  INTERSECT INTO  IS  LEVEL  LIKE  LOCK  LONG  MAXEXTENTS  MINUS   
            MLSLABEL  MODE MODIFY  NOAUDIT  NOCOMPRESS  NOT  NOWAIT  NULL  NUMBER  OF  OFFLINE ON   
            ONLINE  OPTION  OR  ORDER P CTFREE PRIOR PRIVILEGES PUBLIC RAW RENAME RESOURCE REVOKE ROW  
            ROWID ROWNUM ROWS SELECT SESSION SET SHARE SIZE SMALLINT START SUCCESSFUL SYNONYM SYSDATE TABLE 
            THEN TO TRIGGER UID UNION UNIQUE UPDATE USER VALIDATE VALUES VARCHAR VARCHAR2 VIEW WHENEVER WHERE 
            WITH ";

    }
}
