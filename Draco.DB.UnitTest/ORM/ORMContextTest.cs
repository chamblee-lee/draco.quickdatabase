using System;
using System.Collections.Generic;
using Draco.DB.ORM;
using Draco.DB.ORM.Common.AutoSQL;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.UnitTest.ORM.DAL;
using NUnit.Framework;

namespace Draco.DB.UnitTest.ORM
{
    [TestFixture]
    public class ORMContextTest
    {
        private IORMContext ctx = null;

        [TestFixtureSetUp]
        public void setup()
        {
            String connectionString = "Data Source=.;Initial Catalog=Nunit;User Id=sasa;Password=sasa;";
            String dataBaseType = "SQLServer";
            ConnectionInfo connInfo = new ConnectionInfo(connectionString, dataBaseType);
            ctx = new ORMContext(connInfo);
        }

        [Test]
        public void TestORMContext()
        {
            USEREntity entity = new USEREntity();
            entity.Name = "TestName";
            entity.Age = 20;
            entity.Alias = "我的别名";
            entity.Birthday = DateTime.Parse("1987-07-07");
            entity.IsLocked = 0;
            DAO4USEREntity DAO = new DAO4USEREntity(entity, ctx.Handler);
            DAO.Save();
            Assert.NotNull(entity.UID);

            //复用使用过的Entity之前清空已更改的属性
            entity.ClearChangedProperty();
            entity.Name = "TestName";
            List<USEREntity> list = DAO.GetArrayList();
            Assert.True(list!=null && list.Count>0);

            //复用使用过的DAO对象，重新设置Entity对象
            USEREntity condition = new USEREntity();
            condition.Age = 20;
            DAO.SetEntity(condition);
            list = DAO.GetArrayList();
            Assert.True(list != null && list.Count > 0);

            //使用SQL表达式查询,复用对象之前清空
            condition.ClearChangedProperty();
            DAO.ClearExpression();
            DAO.AddExpression(Expression.Gt("Age", 0));
            DAO.AddExpression(Expression.Lt("Age", 30));
            list = DAO.GetArrayList();
            Assert.True(list != null && list.Count > 0);

            //使用SQL语句查询，复用前清除之前的SQL表达式
            DAO.ClearExpression();
            list = DAO.GetArrayList("Age>0 and IsLocked<>1");
            Assert.True(list != null && list.Count > 0);
        }
    }
}
