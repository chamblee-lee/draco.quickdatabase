using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.Compatible
{
    /// <summary>
    /// 兼容性的主键生成器
    /// </summary>
    public class PKGeneratorCompatible : IDBReferencePKGenerator
    {
        private IDataBaseHandler m_IDBHandler;
        /// <summary>
        /// 设置IDataBaseHandler
        /// </summary>
        public IDataBaseHandler DBHandler
        {
            set { m_IDBHandler = value; }
        }
        /// <summary>
        /// 是否可复用
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }
        /// <summary>
        /// 获取下一个主键值
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            string TableName = Mapping.TableName;
            String keyName = Mapping.PrimaryKeyCollection[0].ColumnName;
            long value = GeneratNextPrimaryKey(TableName, keyName);
            Hashtable hash = new Hashtable();
            hash.Add(Mapping.PrimaryKeyCollection[0].PropertyName, value);
            return hash;
        }
        /// <summary>
        /// 获取下一个主键值
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public long GeneratNextPrimaryKey(string TableName, string keyName)
        {
            long value = 1;
            string sql = "select CURRENTVALUE from SYSSEQUENCETBL where  TBLNAME='" + TableName + "'";
            object o = m_IDBHandler.ExecuteScalar(sql);
            string uSQL = "";
            if (o != null && o != DBNull.Value)
            {
                value = Convert.ToInt64(o) + 1;
                uSQL = " update SYSSEQUENCETBL set CURRENTVALUE=" + value + " where TBLNAME='" + TableName + "'";
            }
            else
            {
                sql = String.Format(" select max({0}) from {1}", keyName, TableName);
                object max = m_IDBHandler.ExecuteScalar(sql);
                long maxValue = 1;
                if (max != null && max != DBNull.Value)
                {
                    Int64.TryParse(Convert.ToString(max), out maxValue);
                    value = maxValue + 1;
                    maxValue = value;
                }
                uSQL = "insert into SYSSEQUENCETBL (TBLNAME,FIELDNAME,CURRENTVALUE)values('" + TableName + "','" + keyName + "'," + maxValue + ")";
            }
            //回写下一个值
            m_IDBHandler.ExecuteNonQuery(uSQL);
            return value;
        }
        /// <summary>
        /// 同步序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="keyName"></param>
        public void SynchPrimaryKey(string TableName, string keyName)
        {
            string sql = String.Format("delete from SYSSEQUENCETBL where TBLNAME='{0}' AND FIELDNAME='{1}'", TableName, keyName);
            m_IDBHandler.ExecuteNonQuery(sql);
        }
    }
}
