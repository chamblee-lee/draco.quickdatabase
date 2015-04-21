using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Reflection;
using Draco.DB.ORM.Adapter;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Mapping;

namespace Draco.DB.ORM.Common
{
    /// <summary>
    /// 数据填充
    /// </summary>
    public class DataLoader
    {
        /// <summary>
        /// 把DataSet填充到实体列表
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<T> LoadEntity<T>(DataSet ds, Type type) where T : AbstractEntity
        {
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return null;
            //开始填充
            List<T> list = new List<T>();
            Draco.DB.ORM.Mapping.ITableMapping Mapping = ORMAdapterCreator.MappingManager.GetMapping(type);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //创建对象实例
                T instance = LoadEntity(row, type, Mapping) as T;
                list.Add(instance);
            }
            return list;
        }
        /// <summary>
        /// 加载一个实体对象数据
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="type"></param>
        /// <param name="Mapping"></param>
        /// <returns></returns>
        public static AbstractEntity LoadEntity(DataRow dr, Type type, Draco.DB.ORM.Mapping.ITableMapping Mapping)
        {
            if (!type.IsSubclassOf(typeof(AbstractEntity)))
            {
                throw new ArgumentException(type.FullName + "不是实体类型");
            }
            AbstractEntity instance = type.Assembly.CreateInstance(type.FullName, false, BindingFlags.CreateInstance,
                Type.DefaultBinder, null, System.Globalization.CultureInfo.CurrentCulture, null) as AbstractEntity;
            foreach (FieldMapping f in Mapping.FieldMappingCollection)
            {
                PropertyInfo pInfo = type.GetProperty(f.PropertyName);
                object value = dr[f.ColumnName];

                //设置
                if (value != DBNull.Value)
                {
                    object _value = Convert.ChangeType(value, pInfo.PropertyType);
                    pInfo.SetValue(instance, _value, null);
                }
            }
            foreach (FieldMapping f in Mapping.PrimaryKeyCollection)
            {
                PropertyInfo pInfo = type.GetProperty(f.PropertyName);
                object value = dr[f.ColumnName];

                //设置
                if (value != DBNull.Value)
                {
                    object _value = Convert.ChangeType(value, pInfo.PropertyType);
                    pInfo.SetValue(instance, _value, null);
                }
            }
            instance.ClearChangedProperty();
            return instance;
        }
    }
}
