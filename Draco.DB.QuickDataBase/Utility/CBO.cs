using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Draco.DB.QuickDataBase.Utility
{
    /// <summary>
    /// 自动填充工具
    /// </summary>
    public class CBO
    {
        /// <summary>
        /// 自动填充数据对象
        /// </summary>
        /// <param name="Dt">数据表</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static ArrayList Fill(DataTable Dt, Type type)
        {
            if (Dt == null || Dt.Rows.Count == 0)
                return null;
            if (type == null) return null;
            //开始填充
            ArrayList list = new ArrayList();
            string[] Colls = new string[Dt.Columns.Count];
            int i = 0;
            foreach(DataColumn Column in Dt.Columns)
            {
                Colls[i] = Column.ColumnName;
                i++;
            }
            foreach(DataRow row in Dt.Rows)
            {
                //创建对象实例
                object instance = type.Assembly.CreateInstance(type.FullName, false, BindingFlags.CreateInstance,
                Type.DefaultBinder, null, System.Globalization.CultureInfo.CurrentCulture, null);
                for(int j=0;j<Colls.Length;j++)
                {
                    object o = row[Colls[j]];
                    PropertyInfo pInfo = type.GetProperty(Colls[j]);
                    if (pInfo != null)
                    {
                        pInfo.SetValue(instance, o, null);
                    }
                }
                list.Add(instance);
            }
            return list;
        }
    }
}
