﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.SqLiteClient
{
    /// <summary>
    /// 自增
    /// </summary>
    public class PKGeneratorSqLiteAuto : IPKGenerator
    {
        /// <summary>
        /// 是否可复用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
        /// <summary>
        /// 获取下一主键值
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            //返回空的Hashtable，不对主键赋值
            return new Hashtable();
        }
    }
    /// <summary>
    /// 自定义序列
    /// </summary>
    public class PKGeneratorSqLiteORMSequence : IDBReferencePKGenerator
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

        #region IPKGenerator 成员
        /// <summary>
        /// 获取下一个数据库主键值
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
                string SQL = "select count(*) from sqlite_master where table='{0}'";
                SQL = String.Format(SQL, TableName);
                Object o = m_IDBHandler.ExecuteScalar(SQL);
                if (o == null || o==DBNull.Value || Convert.ToInt32(0)==0)
                {
                    //表不存在
                    string CreateSQL = "CREATE TABLE ORM_SEQUENCE(" +
                    "TableName varchar(255) Primary key," +
                    "NextValue INTEGER NULL)";

                    m_IDBHandler.ExecuteNonQuery(CreateSQL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
