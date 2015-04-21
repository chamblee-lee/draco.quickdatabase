using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace Draco.DB.ORM.PKGenerator.Common
{
    /// <summary>
    /// GUID主键生成器
    /// </summary>
    public class GUIDPKGenerator : IPKGenerator
    {
        /// <summary>
        /// 是否可复用
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }
        /// <summary>
        /// 获取下一个主键值
        /// </summary>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public Hashtable GeneratNextPrimaryKey(Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            Hashtable table = new Hashtable();
            foreach (var key in Mapping.PrimaryKeyCollection)
            {
                table.Add(key.PropertyName, Guid.NewGuid().ToString());
            }
            return table;
        }
    }
    
}
