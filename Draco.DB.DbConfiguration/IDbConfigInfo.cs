using System;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase;
using Draco.DB.ORM;
namespace Draco.DB.DbConfiguration
{
    /// <summary>
    /// 数据库配置信息接口
    /// </summary>
    public interface IDbConfigInfo
    {
        /// <summary>
        /// 
        /// </summary>
        ConnectionInfo ConnectionInfo { get; }
        /// <summary>
        /// 
        /// </summary>
        IDataBaseContext DataBaseContext { get; }
        /// <summary>
        /// 
        /// </summary>
        IORMContext ORMContext { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connInfo"></param>
        void InjectConnectionInfo(ConnectionInfo connInfo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        DateTime GetDateTimeFromDB(IDataBaseHandler handler);
    }
}
