using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 映射对象管理器
    /// </summary>
    public class MappingManager
    {
        private static MappingManager m_obj;
        /// <summary>
        /// 获取管理对象
        /// </summary>
        /// <param name="Loader"></param>
        /// <returns></returns>
        public static MappingManager GetMappingManager(IMappingLoader Loader)
        {
            if (m_obj == null)
            {
                m_obj = new MappingManager(Loader);
            }
            return m_obj;
        }

        private IMappingLoader m_Loader;

        private MappingManager(IMappingLoader Loader)
        {
            if (Loader == null)
                throw new MappingException("Loader不能为空");
            m_Loader = Loader;
        }

        /// <summary>
        /// 获取映射关系类
        /// </summary>
        /// <param name="Class"></param>
        /// <returns></returns>
        public ITableMapping GetMapping(Type Class)
        {
            string ClassName = Class.FullName;
            ITableMapping Mapping = MappingPool.Instance[ClassName] as ITableMapping;
            if (Mapping == null)
            {
                Mapping = CreateMapping(Class);
                MappingPool.Instance[ClassName] = Mapping;
            }
            return Mapping;
        }
        /// <summary>
        /// 根据类创建映射关系类
        /// </summary>
        /// <param name="Class"></param>
        /// <returns></returns>
        public ITableMapping CreateMapping(Type Class)
        {
            return m_Loader.LoadTableMapping(Class);
        }
    }
}
