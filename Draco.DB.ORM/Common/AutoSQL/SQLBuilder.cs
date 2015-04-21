using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.PKGenerator;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Common;

namespace Draco.DB.ORM.Common.AutoSQL
{
    /// <summary>
    /// 生成标准SQL
    /// </summary>
    public abstract class SQLBuilder : ExpSQLBuilder
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        protected AbstractEntity m_Entity;

        #region 查询条件子句
        /// <summary>
        /// 排序属性名称
        /// </summary>
        protected string m_OrderBy;
        /// <summary>
        /// 降序排序，缺省为降序
        /// </summary>
        protected bool m_OrderByDesc = true;
        /// <summary>
        /// 第一条记录索引(从1开始)
        /// </summary>
        protected int m_FirstResult = -1;
        /// <summary>
        /// 最大记录数
        /// </summary>
        protected int m_MaxResult = -1;
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="DbAdapter">数据库类型</param>
        public SQLBuilder(AbstractEntity entity, IORMAdapter DbAdapter)
            : base(entity,DbAdapter)
        {
            m_Entity = entity;
        }
        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void SetEntity(AbstractEntity entity)
        {
            this.Clear();
            m_Entity = entity;
            m_Mapping = ORMAdapterCreator.MappingManager.GetMapping(entity.GetType());
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 表名称
        /// </summary>
        public virtual string TableName
        {
            get { return m_Mapping.TableName; }
        }
        /// <summary>
        /// 经过适配的表名称(避免关键字冲突)
        /// </summary>
        public virtual string AdaptedTableName
        {
            get { return this.AdaptTableName(TableName); }
        }
        /// <summary>
        /// 获取所有数据库字段
        /// </summary>
        public virtual string AllFields
        {
            get
            {
                if (m_Mapping.PrimaryKeyCollection.Count > 0 || m_Mapping.FieldMappingCollection.Count > 0)
                {
                    StringBuilder sBuilder = new StringBuilder();
                    foreach (FieldMapping key in m_Mapping.PrimaryKeyCollection)
                    {
                        sBuilder.Append(m_DBAdapter.AdaptColumnName(key.ColumnName) + ",");
                    }
                    foreach (FieldMapping fMapping in m_Mapping.FieldMappingCollection)
                    {
                        sBuilder.Append(m_DBAdapter.AdaptColumnName(fMapping.ColumnName)+ ",");
                    }
                    sBuilder.Remove(sBuilder.Length - 1, 1);//去掉最后一个逗号
                    return sBuilder.ToString();
                }
                return null;
            }
        }
        #endregion

        #region 查询部分
        /// <summary>
        /// 创建查询SQL
        /// </summary>
        /// <param name="Paras">参数化查询的参数列表(输出)</param>
        /// <returns>SQL</returns>
        public virtual string CreateSelectSQL(out IDataParameter[] Paras)
        {
            return CreateSelectSQL(AllFields, out Paras);
        }
        /// <summary>
        /// 创建查询SQL，附带查询条件
        /// </summary>
        /// <param name="Paras">参数化查询的参数列表(输入输出)</param>
        /// <param name="Where">附加查询条件</param>
        /// <returns>SQL</returns>
        public virtual string CreateSelectSQL(ref IDataParameter[] Paras, string Where)
        {
            return CreateSelectSQL(AllFields,Where,ref Paras);
        }
        /// <summary>
        /// 创建查询SQL,附带返回值字段
        /// </summary>
        /// <param name="ReturnField">返回值字段</param>
        /// <param name="Paras">参数化查询的参数列表(输出)</param>
        /// <returns>SQL</returns>
        public virtual string CreateSelectSQL(string ReturnField, out IDataParameter[] Paras)
        {
            Paras = null;
            return CreateSelectSQL(ReturnField, String.Empty, ref Paras);
        }
        /// <summary>
        /// 创建查询SQL，附带返回值字段，附带外部查询条件
        /// </summary>
        /// <param name="ReturnField">返回值字段</param>
        /// <param name="Where">附加查询条件</param>
        /// <param name="Paras">参数化查询的参数列表(输入输出)</param>
        /// <returns>SQL</returns>
        public virtual string CreateSelectSQL(string ReturnField, string Where, ref IDataParameter[] Paras)
        {
            //查询条件
            IDataParameter[] InnerParas = null;
            string InnerWhere = GeneratorWhereSyntax(out InnerParas);
            string WhereSentence = CombineWhereSyntax(InnerWhere, Where);//
            Paras = CombineWhereParas(InnerParas, Paras);

            //组装查询语句
            string SelectSql = AssemblySelectSQL(ReturnField, WhereSentence);
            Console.WriteLine(SelectSql);
            return SelectSql;
        }
        /// <summary>
        /// 根据实体类创建Where子句
        /// </summary>
        /// <param name="Paras"></param>
        /// <returns></returns>
        protected virtual string GeneratorWhereSyntax(out IDataParameter[] Paras)
        {
            Paras = null;
            AbstractEntity Entity = m_Entity as AbstractEntity;
            if (Entity.ChangedProperties.Count > 0)
            {
                foreach (string Property in Entity.ChangedProperties.Keys)
                {
                    object PropertyValue = Entity.ChangedProperties[Property];
                    this.Add(Expression.Eq(Property, PropertyValue));
                }
            }
            return ConvertExpressToSQL(out Paras);
        }
        /// <summary>
        /// 组建SQL
        /// </summary>
        protected abstract string AssemblySelectSQL(string ReturnField, string Where);
        #endregion

        #region 更新SQL部分
        /// <summary>
        /// 创建更新SQL语句
        /// </summary>
        /// <param name="Paras"></param>
        /// <returns></returns>
        public string CreateUpdataSQL(out IDataParameter[] Paras)
        {
            Paras = null;
            AbstractEntity Entity = m_Entity as AbstractEntity;
            if (Entity.ChangedProperties.Count > 0)
            {
                StringBuilder sBuilderPairs = new StringBuilder();
                IDataParameters list = CreateDataParameters(m_DBAdapter);
                foreach (string Property in Entity.ChangedProperties.Keys)
                {
                    object PropertyValue = Entity.ChangedProperties[Property];
                    if (!IsPrimaryKeyProperty(Property))//更新的时候不更新主键
                    {
                        string segment = ConvertOneExpressToSQL(Expression.Eq(Property, PropertyValue), list);
                        sBuilderPairs.Append(segment + ",");
                    }
                }
                sBuilderPairs.Remove(sBuilderPairs.Length - 1, 1);//去掉最后的逗号;

                //Where 子句
                string where = GetPrimaryKeySQL(ref list);

                //组装SQL
                string UpdateSQL = String.Format(CommonSQL.SQL_UPDATE, AdaptedTableName, sBuilderPairs.ToString(), where);
                Paras = list.Parameters;
                return UpdateSQL;
            }
            return "";
        }
        #endregion

        #region 插入SQL部分
        /// <summary>
        /// 创建插入SQL
        /// </summary>
        /// <returns></returns>
        public virtual string CreateInsertSQL(out IDataParameter[] Paras)
        {
            Paras = null;
            AbstractEntity Entity = m_Entity as AbstractEntity;
            if (Entity.ChangedProperties.Count > 0)
            {
                IDataParameters paras = CreateDataParameters(m_DBAdapter);
                ArrayList Columns = new ArrayList();
                foreach (string Property in Entity.ChangedProperties.Keys)
                {
                    IFieldMapping FiledMapping = m_Mapping.GetFieldMapping(Property);
                    object PropertyValue = Entity.ChangedProperties[Property];
                    //填充参数
                    paras.AddParameterValue(FiledMapping.ColumnName, PropertyValue, InnerFieldTypeConvert.ConvertToDbType(FiledMapping.FieldType));
                    Columns.Add(FiledMapping.ColumnName);
                }
                Paras = paras.Parameters;
                string InsertSQL = GeneratInsertSql(AdaptedTableName, Columns.ToArray(typeof(string)) as String[]);
                return InsertSQL;
            }
            return "";
        }
        /// <summary>
        /// 获取插入SQL
        /// </summary>
        /// <param name="tblName">表名</param>
        /// <param name="Columns">列数组</param>
        /// <returns>SQL语句</returns>
        protected virtual string GeneratInsertSql(string tblName, string[] Columns)
        {
            if (String.IsNullOrEmpty(tblName)) throw new ArgumentException("tblName is null");
            if (Columns == null || Columns.Length == 0) throw new ArgumentException("Columns is null");
            //开始计算
            StringBuilder sBuilderFields = new StringBuilder();
            StringBuilder sBuilderVaules = new StringBuilder();
            foreach (string key in Columns)
            {
                sBuilderFields.Append(m_DBAdapter.AdaptColumnName(key.ToString()) + ",");
                string ParaName = m_DBAdapter.AdaptParameterName(key.ToString());
                sBuilderVaules.Append(ParaName + ",");
            }
            sBuilderFields.Remove(sBuilderFields.Length - 1, 1);
            sBuilderVaules.Remove(sBuilderVaules.Length - 1, 1);
            string SQL = String.Format(CommonSQL.SQL_INSERT, tblName, sBuilderFields.ToString(), sBuilderVaules.ToString());
            return SQL;
        }
        #endregion

        #region 删除部分
        /// <summary>
        /// 以主键删除记录
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public string CreateDeleteSQL(out IDataParameter[] para)
        {
            IDataParameters paras = CreateDataParameters(m_DBAdapter);
            string where = GetPrimaryKeySQL(ref paras);
            para = paras.Parameters;
            string DeleteSQL = String.Format(CommonSQL.SQL_DELETE, AdaptedTableName, where);
            return DeleteSQL;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 合并查询条件
        /// </summary>
        /// <param name="Where1">Where子句1</param>
        /// <param name="Where2">Where子句2</param>
        /// <returns></returns>
        protected virtual string CombineWhereSyntax(string Where1, string Where2)
        {
            if (!String.IsNullOrEmpty(Where1) && !String.IsNullOrEmpty(Where2))//实施合并
            {
                return Where1 + CommonSQL.AND + Where2;
            }
            if (String.IsNullOrEmpty(Where1))
                return Where2;
            if (String.IsNullOrEmpty(Where2))
                return Where1;
            return "";
        }
        /// <summary>
        /// 合并查询参数
        /// </summary>
        /// <param name="Paras1">参数列表1</param>
        /// <param name="Paras2">参数列表2</param>
        /// <returns></returns>
        protected virtual IDataParameter[] CombineWhereParas(IDataParameter[] Paras1, IDataParameter[] Paras2)
        {
            if (Paras1 != null && Paras1.Length > 0 && Paras2 != null && Paras2.Length > 0)//实施合并
            {
                ArrayList list = new ArrayList();
                foreach (IDataParameter pa in Paras1)
                    list.Add(pa);
                foreach (IDataParameter pa in Paras2)
                    list.Add(pa);
                return list.ToArray(typeof(IDataParameter)) as IDataParameter[];
            }
            if (Paras1 != null && Paras1.Length > 0)
                return Paras1;
            if (Paras2 != null && Paras2.Length > 0)
                return Paras2;
            return null;
        }
        /// <summary>
        /// 判断属性是不是主键属性
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        protected virtual bool IsPrimaryKeyProperty(string PropertyName)
        {
            foreach (var key in m_Mapping.PrimaryKeyCollection)
            {
                if (key.PropertyName == PropertyName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 适配表名称，避免关键字冲突
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual String AdaptTableName(String tableName)
        {
            //使用同样的算法适配表名称和列名称
            return m_DBAdapter.AdaptColumnName(tableName);
        }
        #endregion

        #region 排序及分页
        /// <summary>
        /// 设置第一条记录索引(从1开始,用于分页)
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public SQLBuilder SetFirstResult(int Index)
        {
            m_FirstResult = Index;
            return this;
        }
        /// <summary>
        /// 设置最大返回记录数
        /// </summary>
        /// <param name="count">最大返回记录数</param>
        /// <returns></returns>
        public SQLBuilder SetMaxResult(int count)
        {
            m_MaxResult = count;
            return this;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="PropertyName">排序属性名称</param>
        /// <param name="Desc"></param>
        /// <returns></returns>
        public SQLBuilder Order(string PropertyName, bool Desc)
        {
            m_OrderBy = PropertyName;
            m_OrderByDesc = Desc;
            return this;
        }
        #endregion

        #region  以主键检测数据是否存在的 SQL
        /// <summary>
        /// 获取确认主键是否存在的SQL语句
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public virtual string CheckDataExistSQL(out IDataParameter[] para)
        {
            IDataParameters paras = CreateDataParameters(m_DBAdapter);
            string where = GetPrimaryKeySQL(ref paras);
            string SQL = String.Format(CommonSQL.SQL_SELECT, "COUNT(*)", AdaptedTableName, where);

            para = paras.Parameters;
            return SQL;
        }
        #endregion

        #region 构建插入SQL之前，主键值检测确认
        /// <summary>
        /// 确认主键是否有值
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckePrimaryKey()
        {
            AbstractEntity Entity = m_Entity as AbstractEntity;
            if (Entity.ChangedProperties != null && m_Mapping.PrimaryKeyCollection.Count > 0)
            {
                bool HasValue = true;
                foreach (IPrimaryKeyMapping mapping in m_Mapping.PrimaryKeyCollection)
                {
                    if (!Entity.ChangedProperties.ContainsKey(mapping.PropertyName))
                    {
                        //进一步检查主键属性是否有值，因为如果对象是由Handle查询创建的,那么修改的属性中是没有主键的
                        PropertyInfo property = Entity.GetType().GetProperty(mapping.PropertyName);
                        object val = property.GetValue(Entity, null);
                        if (val == null || val == DBNull.Value || Convert.ToString(val) == "" || Convert.ToString(val) == "0")
                            HasValue = false;//有一个主键没有值就要重新生成主键
                    }
                }
                if (HasValue)//所有的主键都有值，那就返回了
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="Generator"></param>
        public virtual void CreatePrimaryKey(IPKGenerator Generator)
        {
            AbstractEntity Entity = m_Entity as AbstractEntity;
            //创建主键
            if (Generator == null)
                throw new MappingException("创建主键生成器失败，主键生成器必须实现Draco.DB.ORM.AutoSQL.PKGenerator.IPKGenerator接口，请检查映射文件中主键生成器的配置信息!");
            Hashtable Next = Generator.GeneratNextPrimaryKey(m_Mapping);
            if (Next != null && Next.Count > 0)
            {
                foreach (string PropertyName in Next.Keys)
                {
                    object NextValue = Next[PropertyName];
                    PropertyInfo Property = Entity.GetType().GetProperty(PropertyName);
                    if (Property != null)
                    {
                        object _value = Convert.ChangeType(NextValue, Property.PropertyType);
                        Property.SetValue(Entity, _value, null);
                    }
                }
            }
        }
        /// <summary>
        /// 构建主键生成器
        /// </summary>
        /// <param name="DbHandle"></param>
        /// <returns></returns>
        public virtual IPKGenerator CreatePKGenerator(IDataBaseHandler DbHandle)
        {
            if (String.IsNullOrEmpty(m_Mapping.Generator))
                throw new MappingException("主键生成器配置为空，请检查映射文件中主键生成器的配置信息!");
            IPKGenerator Generator = null;
            try
            {
                Generator = PKGeneratorFactory.GetPKGenerator(m_Mapping.Generator, DbHandle);
            }
            catch (Exception ex)
            {
                throw new MappingException("创建主键生成器失败，请检查映射文件中主键生成器的配置信息!详细信息:" + ex.ToString());
            }
            if (Generator == null)
                throw new MappingException("创建主键生成器失败，主键生成器必须实现Draco.DB.ORM.PKGenerator.IPKGenerator接口，请检查映射文件中主键生成器的配置信息!");
            return Generator;
        }

        /// <summary>
        /// 处理传出参数
        /// </summary>
        /// <param name="paras"></param>
        public virtual void ProcessOutputParameters(IDataParameter[] paras)
        {
            if (paras != null && paras.Length > 0)
            {
                foreach (IDataParameter para in paras)
                {
                    if (para.Direction == ParameterDirection.Output || para.Direction == ParameterDirection.InputOutput)
                    {
                        PropertyInfo Property = m_Entity.GetType().GetProperty(para.ParameterName);
                        if (Property != null)
                        {
                            object _value = Convert.ChangeType(para.Value, Property.PropertyType);
                            Property.SetValue(m_Entity, _value, null);
                        } 
                    }
                }
            }
        }
        #endregion

        #region 多主键SQL
        private String GetPrimaryKeySQL(ref IDataParameters paras)
        {
            if(paras==null)
                paras = CreateDataParameters(m_DBAdapter);
            StringBuilder sBuilderKeys = new StringBuilder();
            foreach (IPrimaryKeyMapping PK in m_Mapping.PrimaryKeyCollection)
            {
                PropertyInfo KeyProperty = m_Entity.GetType().GetProperty(PK.PropertyName);         //主键属性
                object KeyValue = KeyProperty.GetValue(m_Entity, null);                              //主键值

                string KeyParaName = m_DBAdapter.AdaptParameterName("pK_" + PK.PropertyName);  //主键字段名称
                string keyWhere = m_DBAdapter.AdaptColumnName(PK.ColumnName) + "=" + KeyParaName;       //[ID]=@pK_ID
                sBuilderKeys.Append(keyWhere + " AND ");
                paras.AddParameterValue(KeyParaName, KeyValue);
            }
            sBuilderKeys.Remove(sBuilderKeys.ToString().LastIndexOf("AND"), 3);//去掉最后一个AND
            return sBuilderKeys.ToString();
        }
        #endregion
    }
}
