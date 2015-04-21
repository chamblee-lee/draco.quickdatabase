using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 映射程序池
    /// </summary>
    public class MappingPool : Hashtable
    {
        private MappingPool() { }
        private  static MappingPool m_Instance;

        /// <summary>
        /// 实例
        /// </summary>
        public static MappingPool Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new MappingPool();
                return m_Instance;
            }
        }
    }
}
