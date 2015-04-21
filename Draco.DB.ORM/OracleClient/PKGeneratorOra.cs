using System;
using System.Collections;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;
using System.Data;

namespace Draco.DB.ORM.OracleClient
{
    /// <summary>
    /// 序列
    /// </summary>
    public class PKGeneratorOraSequence : IDBReferencePKGenerator
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
            if (Mapping == null)
                throw new ArgumentNullException("GeneratNextPrimaryKey(Mapping)");
            if (Mapping.PrimaryKeyCollection == null || Mapping.PrimaryKeyCollection.Count != 1)
            {
                throw new MappingException("Orale序列仅支持整形单主键,请修改映射配置文件");
            }
            long Sequence = GetSEQColumNextValueOra(Mapping.TableName);
            if (Sequence > 0)
            {
                Hashtable hash = new Hashtable();
                hash.Add(Mapping.PrimaryKeyCollection[0].PropertyName, Sequence);
                return hash;
            }
            return new Hashtable();
        }


        /// <summary>
        /// 取得表中序列(SEQ_ + TblName)的下一个值
        /// </summary>
        /// <param name="TblName"></param>
        /// <returns></returns>
        public long GetSEQColumNextValueOra(string TblName)
        {
            string SelectSql = "Select SEQ_" + TblName + ".nextval from DUAL";

            object o=null;
            o = m_IDBHandler.ExecuteScalar(SelectSql);
            if (o == null || o == DBNull.Value)
                return 1;
            else
                return Convert.ToInt64(o, System.Globalization.NumberFormatInfo.CurrentInfo);
        }
        /// <summary>
        /// 判断是否有序列
        /// </summary>
        /// <param name="TblName"></param>
        /// <returns></returns>
        public bool IsHaveSequenceOra(string TblName)
        {
            string SeqName = "SEQ_" + TblName;
            string strSql = "select count(*) from user_sequences  where SEQUENCE_NAME = :pSeqName ";
            IDataParameter pSeqName = m_IDBHandler.DbAdapter.CreateParameter("pSeqName", SeqName);
            //判断是否有序列
            object o = null ;
            o = m_IDBHandler.ExecuteScalar(strSql, pSeqName);
            if (o == null || o == DBNull.Value)
                return false;
            else
                return Convert.ToInt32(o, System.Globalization.NumberFormatInfo.CurrentInfo) == 1;
        }
    }

    /// <summary>
    /// 自定义序列
    /// </summary>
    public class PKGeneratorOraORMSequence : IDBReferencePKGenerator
    {
        private static long RequestCount = 0;
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
        /// 生成下一个主键值
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            string TableName = Mapping.TableName;
            if (RequestCount == 0)
            {
                //第一次调用,确认表是否存在
                InitTable(TableName);
            }
            long value = 1;
            string sql = "select NextValue from ORM_SEQUENCE where  TableName='" + TableName + "'";
            object o = m_IDBHandler.ExecuteScalar(sql);
            string uSQL = "";
            if (o != null && o != DBNull.Value)
            {
                value = Convert.ToInt64(o);
                long next = value + 1;
                uSQL = " update ORM_SEQUENCE set NextValue=" + next + " where TableName='" + TableName + "'";
            }
            else
            {
                uSQL = "insert into ORM_SEQUENCE (TableName,NextValue)values('" + TableName + "',2)";
            }
            //回写下一个值
            m_IDBHandler.ExecuteNonQuery(uSQL);
            RequestCount++;

            Hashtable hash = new Hashtable();
            hash.Add(Mapping.PrimaryKeyCollection[0].PropertyName, value);
            return hash;
        }

        /// <summary>
        /// 初始化表
        /// </summary>
        /// <param name="TableName"></param>
        private void InitTable(string TableName)
        {
            try
            {
                string SQL = "select count(*) from user_tab_columns where table_name=upper('{0}')";
                SQL = String.Format(SQL, TableName);
                object o = m_IDBHandler.ExecuteScalar(SQL);
                if (o == null || o ==DBNull.Value || Convert.ToInt32(o)==0)
                {
                    //表不存在
                    string CreateSQL = "CREATE TABLE ORM_SEQUENCE(" +
                    "TABLENAME VARCHAR2(255) NOT NULL ENABLE, " +
                    "NEXTVALUE NUMBER," +
                    "PRIMARY KEY (\"TABLENAME\"))";
                    m_IDBHandler.ExecuteNonQuery(CreateSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
