using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Threading
{
    /// <summary>
    /// 线程上下文存储类
    /// </summary>
    public sealed class LogicalThreadContext
    {
        // Fields
        private static IThreadStorage threadStorage = new CallContextStorage();

        // Methods
        private LogicalThreadContext()
        {
            throw new NotSupportedException("must not be instantiated");
        }
        /// <summary>
        /// 释放上下文存储对象
        /// </summary>
        /// <param name="name"></param>
        public static void FreeNamedDataSlot(string name)
        {
            threadStorage.FreeNamedDataSlot(name);
        }
        /// <summary>
        /// 获取上下文存储对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetData(string name)
        {
            return threadStorage.GetData(name);
        }
        /// <summary>
        /// 设置上下文存储对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetData(string name, object value)
        {
            threadStorage.SetData(name, value);
        }
        /// <summary>
        /// 设置上下文存储对象
        /// </summary>
        /// <param name="storage"></param>
        public static void SetStorage(IThreadStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException("storage is null");
            threadStorage = storage;
        }
    }
}
