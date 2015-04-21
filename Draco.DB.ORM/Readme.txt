关于Draco.DB.ORM的使用说明：
1.Draco.DB.ORM的主要设计目的与功能是什么？
	本程序集的主要设计目的是实现简单的ORM模型，实现根据数据库自动生成数据库构架描述对象，数据库构架序列化反序列化、自动生成数据访问实体类型C#代码、
	自动生成简单SQL，基于对象的数据库访问等功能。
	本程序集同时支持数据库类型扩展，对于未被默认支持的数据库类型，可以通过实现接口扩展的方式实现相应的数据库操作及ORM操作。

2.默认支持的数据库类型有哪些？
	默认支持SQLServer、Oracle、OleDb、Sqlite四种类型数据库链接
	
3.如何配置数据库链接信息？
	数据库链接信息包含数据库类型与数据库连接串；
		其中数据库类型缺省支持类型为：SQLSERVER、ORACLE、OLEDB、SQLITE(四者可任选其一，大小写不敏感)
		数据库连接串请参考“数据库连接串说明”
	数据库链接信息的配置包括“配置文件配置”与“编程方式配置”两种方式。
	1)配置文件配置的配置示例，在App.config或者web.config的configuration/appSettings节中作如下配置：
	<add key="ConnectionString" value="Data Source=&quot;(local)&quot;;Initial Catalog=&quot;Dracox&quot;;User Id=&quot;sa&quot;;Password=&quot;sa&quot;;" />
	<add key="DataServerType" value="SQLSERVER" />
	2)编程配置方式可参考使用示例
	
3.数据库操作示例：
	MyEntity MyEntityObj = new MyEntity();
	MyEntityObj.Name="name";
	MyEntityObj.Tag = "tag";
	Draco.DB.ORM.Common.EntityHandle<MyEntity> Handle = Draco.DB.ORM.ORMContext.Default.CreateHandle<MyEntity>(MyEntityObj);
	Handle.Save();//保存数据
	
	System.Data.DataSet ds = Handle.GetDataSet();//获取DataSet
	System.Object obj = Handle.GetOneField("Name");
	System.Collections.Generic.List<MyEntity> list = Handle.AddExpression(Expression.Eq("type",2)).GetArrayList();//附加查询条件查询
	int i = Handle.Delete();//删除
	
4.创建Entity类型C#源代码
	Draco.DB.ORM.Schema.Vendor.ISchemaLoader Loader = ORMContext.Default.SchemaLoader;//获取加载器
    Draco.DB.ORM.Schema.Dbml.Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer", null);//加载数据库构架
    EntityCodeGenerator Generator = new EntityCodeGenerator(dataBase);//创建代码生成器
    string Code = Generator.GenerateCode("MyTableName");//创建C#代码
    File.WriteAllText("D:\\MyTableName.cs", Code, System.Text.Encoding.UTF8);//输出C#代码到文件
	
5.序列化数据库构架
	Draco.DB.ORM.Schema.Vendor.ISchemaLoader Loader = ORMContext.Default.SchemaLoader;//获取加载器
    Draco.DB.ORM.Schema.Dbml.Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer");//加载数据库构架
    using (Stream dbmlFile = File.OpenWrite("D://dbml.xml"))
    {
		Draco.DB.ORM.Schema.Dbml.DbmlSerializer.Write(dbmlFile, dataBase);
    }

6.反序列化数据库构架
	Draco.DB.ORM.Schema.Dbml.Database dataBase = null;
    using (Stream dbmlFile = File.OpenRead("D://dbml.xml"))
    {
		dataBase = Draco.DB.ORM.Schema.Dbml.DbmlSerializer.Read(dbmlFile);
    }
    
7.Entity主键生成策略
	GUID:	全球唯一标识，例：B5EB53AD-F865-4b8e-AEC6-B1D11437383E
	SIMPLEGUID:简单Guid数据库主键,在保证唯一的前提下，比Guid更短，例：1e003ff4ceda1f0e77115197
	TIMESTAMP:时间戳数据库主键，例：201010151456311550062788
	LOCAL:	数据库本地支持的数据库主键策略，SQLServer则是自增，Oracle则是表序列
	ORM_SEQUENCE:ORM实现的自定义序列主键，整形主键，由ORM_SEQUENCE表维护各个表的主键序列值
	
8.扩展数据库类型支持
	1)实现Draco.DB.ORM.Common.IDBTypeFactory接口，以匹配新的数据库类型工厂
	2)实现Draco.DB.ORM.Common.IDBHandler接口，以实现新的数据库类型的数据库操作
	3)实现Draco.DB.ORM.Schema.Vendor.ISchemaLoader接口，以实现新的数据库构架加载器
	4)实现Draco.DB.ORM.AutoSQL.SQLBuilder<T>抽象类，以实现新的数据库类型SQL自动化
	5)实现Draco.DB.ORM.Factory.IDBFactory接口，以构建以上新实现的接口类型
	6)实现Draco.DB.ORM.AutoSQL.PKGenerator.IPKGenerator接口，以实现新的数据库主键生成策略