using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Configuration;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// DataBaseContext接口
    /// </summary>
    public interface IDataBaseContext
    {
        /// <summary>
        /// 获取ConnectionInfo
        /// </summary>
        ConnectionInfo ConnectionInfo { get; }
        /// <summary>
        /// 获取数据库操作类
        /// </summary>
        /// <returns></returns>
        IDataBaseHandler Handler { get; }
    }
}
