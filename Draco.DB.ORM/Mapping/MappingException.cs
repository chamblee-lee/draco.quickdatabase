using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 映射异常
    /// </summary>
    public class MappingException:Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="msg"></param>
        public MappingException(string msg):base(msg){}
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public MappingException(string msg,Exception ex) : base(msg,ex) { }
    }
}
