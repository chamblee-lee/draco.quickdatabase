using System;
using System.Data;
using Draco.DB.ORM;
using Draco.DB.ORM.Generator;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Configuration;

namespace Draco.DB.UnitTest.Test
{
    class Class1
    {
        public void cc()
        {
            String connectionString = "Data Source=.;Initial Catalog=MyDB;User Id=sa;Password=sa;";
            String dataBaseType = "SQLServer";
            Draco.DB.QuickDataBase.Configuration.ConnectionInfo connInfo = new ConnectionInfo(connectionString, dataBaseType);
            
            Draco.DB.QuickDataBase.IDataBaseContext ctx = new Draco.DB.QuickDataBase.DataBaseContext(connInfo);
            Draco.DB.QuickDataBase.IDataBaseHandler handler = ctx.Handler;
            //开始数据库操作
            //......

            //以IDataBaseHandler对象获取Draco.DB.QuickDataBase.IDataBaseContext对象
            Draco.DB.QuickDataBase.Adapter.IDataBaseAdapter adp = handler.DbAdapter;
            //创建单一参数对象
            System.Data.IDataParameter para = adp.CreateParameter("@Para", "abc");
            //快速创建参数化对象数组
            Draco.DB.QuickDataBase.IDataParameters parameters = new Draco.DB.QuickDataBase.Common.DataParameters(adp);
            parameters.AddParameterValue("FiledNameX", 1);
            parameters.AddParameterValue("FiledNameY", "abc");
            parameters.AddParameterValue("FiledNameZ", DateTime.Now);
            System.Data.IDataParameter[] _Parameters = parameters.Parameters;


            String SQL = "select * from tab where field1=? and (field2=? or field3>?)";
            Draco.DB.QuickDataBase.ParameterizedSQL param = handler.DbAdapter.AdaptSQLAnonymousParams(SQL, 100, "ABC", DateTime.Now);
            DataSet ds =handler.ExecuteQuery(param.SQL, param.Parameters);

            Draco.DB.QuickDataBase.ISQLGenerator SQLGen = adp.GetSQLGenerator();

            Draco.DB.QuickDataBase.IDataBaseSchemaHandler schemaHandler = adp.GetSchemaHandler(handler);

            String timeSQL = adp.NamedSQLs["Test.TimeSQL"];
            Object obj = handler.ExecuteScalar(timeSQL);

            //
            ctx.Handler.StartTransaction();
            ctx.Handler.CommitTransaction();

            handler.StartTransaction();
            handler.CommitTransaction();

            using(Draco.DB.QuickDataBase.Common.DbTransactionScope scope = new DbTransactionScope(handler))
            {
                try
                {
                    //....
                    scope.Complete();//
                }
                catch { }
            }

            Draco.DB.ORM.IORMContext octx = new ORMContext(connInfo);
            Draco.DB.ORM.Schema.Vendor.ISchemaLoader Loader = octx.SchemaLoader;//获取加载器
            Draco.DB.ORM.Schema.Dbml.Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer", null);//加载数据库构架
            Draco.DB.ORM.Generator.EntityCodeGenerator Generator = new EntityCodeGenerator(dataBase);//创建代码生成器
            string Code = Generator.GenerateCode("MyEntity", "GUID",true);//创建C#代码
            System.IO.File.WriteAllText("D:\\MyEntity.cs", Code, System.Text.Encoding.UTF8);//输出C#代码到文件
        }
    }
}
