using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Threading
{
    /// <summary>
    /// 线程存储
    /// </summary>
    public interface IThreadStorage
    {
        /// <summary>
        /// 释放线程存储对象
        /// </summary>
        /// <param name="name"></param>
        void FreeNamedDataSlot(string name);
        /// <summary>
        /// 获取线程存储对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetData(string name);
        /// <summary>
        /// 设置线程存储对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetData(string name, object value);
    }
}
