using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Utility
{
    /// <summary>
    /// Array辅助类
    /// </summary>
    public class ArrayHelper
    {
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="array"></param>
        /// <param name="element"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool Contains(String[] array,string element,bool ignoreCase)
        {
            if (array != null && array.Length > 0)
                foreach (String n in array)
                    if (String.Compare(n, element, ignoreCase) == 0)
                        return true;
            return false;
        }
    }
}
