using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Configuration;
using NUnit.Framework;

namespace Draco.DB.UnitTest.QuickDataBase.SqlCeClient
{
    
    [TestFixture]
   public  class SqlCeTest
    {
       
        DataBaseContext dbctx;// = new DataBaseContext(ci);
        [TestFixtureSetUp]
        public void setup()
        {
            if (!File.Exists("test.sdf"))
            {
                SqlCeEngine sce = new SqlCeEngine("Data Source=Test.sdf");
                sce.CreateDatabase();
            }
            ConnectionInfo ci = new ConnectionInfo("Data Source=NorthWind.sdf", "SQLSERVERCE");
            dbctx = new DataBaseContext(ci);
        }
        [Test]
        public void DbContextCreate()
        {

            //    DataBaseContext dbctx = new DataBaseContext(ci);
            IDataBaseHandler idbh = dbctx.Handler;
            DataSet dt = idbh.ExecuteQuery("select * from Customers");

            Assert.IsTrue(dt.Tables[0].Rows.Count > 0);
        }

        [Test]
        public void GetAllTables()
        {

            //    DataBaseContext dbctx = new DataBaseContext(ci);
            IDataBaseHandler idbh = dbctx.Handler;
            System.Collections.Generic.List<String> tbls = dbctx.DBAdapter.GetSchemaHandler(idbh).GetTableNames();//.DbAdapter..ExecuteQuery("select * from Customers");
            //  System.Console.WriteLine(tbls.Count);
            Assert.IsTrue(tbls != null && tbls.Count > 0);
            System.Console.WriteLine("GetAllTables:" + tbls.Count);
        }

    }
}
