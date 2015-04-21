using System;
using System.Collections.Generic;
using System.Xml;
namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 命名SQL接口
    /// </summary>
    public interface INamedSQLConfig
    {
        /// <summary>
        /// 添加SQL
        /// </summary>
        /// <param name="nconf"></param>
        void AddNamedSQL(INamedSQLConfig nconf);
        /// <summary>
        /// 添加SQL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="SQL"></param>
        void AddNamedSQL(string name, string SQL);
        /// <summary>
        /// 获取数目
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 获取所有SQL
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetNamedSQL();
        /// <summary>
        /// 移除一条SQL
        /// </summary>
        /// <param name="name"></param>
        void RemoveNamedSQL(string name);
        /// <summary>
        /// 获取一条SQL
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string this[string name] { get; }
        /// <summary>
        /// 从XML文档加载命名SQL
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        INamedSQLConfig Load(XmlDocument xDoc);
    }
}
