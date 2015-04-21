using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Configuration;
using System.Data;
using Draco.DB.QuickDataBase;

namespace Draco.DB.UnitTest.QuickDataBase
{
    /// <summary>
    /// 测试
    /// </summary>
    [TestFixture]
    public class TestSqlServerNoLock
    {
        DataBaseContext dbctx;// = new DataBaseContext(ci);
        /// <summary>
        /// 
        /// </summary>
        [TestFixtureSetUp]
        public void setup()
        {
            ConnectionInfo ci = new ConnectionInfo("Server=192.168.81.102;database=northwind;uid=sa;pwd=sa", "SQLSERVER");
            dbctx = new DataBaseContext(ci);
        }

        /// <summary>
        ///          
        /// </summary>
        [Test]
        
        public void TestNoLock()
        { 
            IDataBaseHandler idb=dbctx.Handler;
            using (DbTransactionScope dbt = new DbTransactionScope(idb))
            {
                Object obj = idb.ExecuteScalar("select [CompanyName] from Customers  where   [CustomerID] ='ANATR'");
              //  obj.ToString().Substring(0, obj.ToString().Length - 2) + DateTime.Now.Ticks.ToString().Substring(0, 2);
                String val = obj.ToString();
                char[] charArray = val.ToCharArray();
                Array.Reverse(charArray);
                val = new string(charArray);//
                
                idb.ExecuteNonQuery("update Customers set [CompanyName]='"+val+"' where [CustomerID] ='ANATR'");
                IDataBaseHandler idb2=dbctx.Handler;

               // idb2.ExecuteNonQuery(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");

                obj = idb2.ExecuteScalar("select [CompanyName] from Customers  where   [CustomerID] ='ANATR'");
                System.Console.WriteLine(obj.ToString());
                DataSet ds = idb2.ExecuteQuery("select * from Customers");
                System.Console.WriteLine(ds.Tables[0].Rows.Count);
                dbt.Complete();
            }
        }
    }
}
