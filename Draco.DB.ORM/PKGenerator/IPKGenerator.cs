using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.ORM.Mapping;
using System.Collections;
using Draco.DB.QuickDataBase;

namespace Draco.DB.ORM.PKGenerator
{
    /// <summary>
    /// 主键生成器接口
    /// </summary>
    public interface IPKGenerator
    {
        /// <summary>
        /// 是否可复用
        /// </summary>
        bool IsReusable { get; }
        /// <summary>
        /// 获取下一个主键
        /// </summary>
        /// <param name="Mapping">表映射对象</param>
        /// <returns></returns>
        Hashtable GeneratNextPrimaryKey(ITableMapping Mapping);
    }
    /// <summary>
    /// 数据库相关的主键生成器接口
    /// </summary>
    public interface IDBReferencePKGenerator : IPKGenerator
    {
        /// <summary>
        /// 数据库操作接口
        /// </summary>
        IDataBaseHandler DBHandler { set; }
    }
}
