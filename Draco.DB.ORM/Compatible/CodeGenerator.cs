using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.Generator;
using Draco.DB.ORM.Schema.Dbml;
using Draco.DB.ORM.Mapping.AttrMapping;

namespace Draco.DB.ORM.Compatible
{
    /// <summary>
    /// 兼容旧版本的代码生成器
    /// </summary>
    public class CodeGenerator : EntityCodeGenerator
    {
        private string __Using =//using
 @"using System;
using System.Collections;
using Draco.DB.ORM.Compatible;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.Mapping.AttrMapping;
using Draco.DB.QuickDataBase;";
        private string m_CSharpTemplate = "" +//主体模板
@"{0}
{1}
namespace {2}
{{
{3}
{4}
}}";
        

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataBase">数据库构架</param>
        public CodeGenerator(Database dataBase)
            : base(dataBase)
        { 
        }
        /// <summary>
        /// 构造代码
        /// </summary>
        /// <param name="table"></param>
        /// <param name="PKGenerator"></param>
        /// <param name="GenerateDAO"></param>
        /// <returns></returns>
        protected override string GenerateCSharpCode(Table table,String PKGenerator,bool GenerateDAO)
        {
            if (table != null)
            {
                //Draco.DB.ORM.Compatible.PKGeneratorCompatible,Draco.DB.ORM
                string CommentHead = String.Format(CommentHeadTemplate, DateTime.Now);
                string NameSpace = m_Database.EntityNamespace;
                string ClassCode1 = GenerateClass(table, "INTEGER-SEQUENCE-COMPATIBLE");
                string ClassCode2 = GenerateClass2(table, "INTEGER-SEQUENCE-COMPATIBLE");
                string CSCode = string.Format(m_CSharpTemplate, CommentHead, __Using, NameSpace, ClassCode1, ClassCode2);
                return CSCode;
            }
            return null;
        }
        private string m_ClassTemplate = "" +
@"    /// <summary>
    /// {0}
    /// </summary>
    [TableMapping({1})]
    public partial class {2} : EntityBase
    {{
{3}
{4}
{5}
{6}
{7}
{8}
    }}";
        private string m_MethodString1 =
@"
        /// <summary>
        /// 根据关键字得到对象
        /// </summary>
        /// <returns></returns>
        public static {0} GetObjectByKey(int nkey, IDataBaseHandler handler)
        {{
            {0}[] list = GetObjectsByKeys(new int[]{{nkey}},handler);
            if (list != null && list.Length > 0)
                return list[0];
            return null;
        }}";
        private string m_MethodString2 =
@"      /// <summary>
        /// 根据关键字(ID)集合得到数据对象集合
        /// </summary>
        /// <param name='Keys'></param>
        /// <param name='handler'></param>
        /// <returns></returns>
        public static {0}[] GetObjectsByKeys(int[] Keys, IDataBaseHandler handler)
        {{
            ArrayList list = GetEntitiesByKeys(typeof({0}),{1},Keys, handler);
            if (list != null && list.Count > 0)
                return list.ToArray(typeof({0})) as {0}[];
            return null;
        }}
";
        /// <summary>
        /// 构造Class类
        /// </summary>
        /// <param name="table"></param>
        /// <param name="PKGenerator"></param>
        /// <returns></returns>
        protected override string GenerateClass(Table table, String PKGenerator)
        {
            string ClassComment = "映射实体类:" + table.Name;
            string AttributeText = GenerateClassAttribute(table, PKGenerator);
            string ClassName = GenerateClassName(table);
            string FieldsCode = GenerateFields(table.Type.Columns);
            string PropertiesCode = GenerateProperties(table.Type.Columns);
            string ConstructCode = String.Format(ConstructTemplate, ClassName);
            string StaticTableName = String.Format(TableNameProperty, "表名称:" + table.Name, "\"" + table.Name + "\"");
            string method1 = String.Format(m_MethodString1, ClassName);
            string method2 = String.Format(m_MethodString2, ClassName, "\"" + table.Name + "\"");
            string ClassCode = String.Format(m_ClassTemplate, ClassComment, AttributeText, ClassName, StaticTableName, FieldsCode, ConstructCode, PropertiesCode, method1, method2);
            return ClassCode;
        }

        string m_ClassTemplate2 =
@"public partial class {0}
{{
    /// <summary>
    /// {1}
    /// </summary>
    [TableMapping({2})]
    public partial class TblStru : TblStruBase
    {{
{3}
{4}
{5}
        /// <summary>
        /// 获取真实实体类类型
        /// </summary>
        public override Type __EntityType
        {{
            get {{ return typeof({6}); }}
        }}
    }}
}}";

        /// <summary>
        /// 构造内部类
        /// </summary>
        /// <param name="table"></param>
        /// <param name="PKGenerator"></param>
        /// <returns></returns>
        private string GenerateClass2(Table table, String PKGenerator)
        {
            string ClassName = GenerateClassName(table);
            string ClassComment = "映射实体类:" + table.Name;
            string AttributeText = GenerateClassAttribute(table, PKGenerator);
            string FieldsCode = GenerateFields(table.Type.Columns);
            string PropertiesCode = GenerateProperties(table.Type.Columns);
            string StaticTableName = String.Format(TableNameProperty, "表名称:" + table.Name, "\"" + table.Name + "\"");

            string ClassCode = String.Format(m_ClassTemplate2, ClassName, ClassComment, AttributeText, FieldsCode, StaticTableName, PropertiesCode, ClassName);
            return ClassCode;
        }
    }
}
