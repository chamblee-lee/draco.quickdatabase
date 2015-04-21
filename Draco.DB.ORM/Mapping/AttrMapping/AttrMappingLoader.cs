using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Draco.DB.ORM.Mapping.AttrMapping
{
    /// <summary>
    /// 属性映射加载器
    /// </summary>
    public class AttrMappingLoader : IMappingLoader
    {
        /// <summary>
        /// 从属性加载表映射关系
        /// </summary>
        /// <param name="Class">实体类型</param>
        /// <returns></returns>
        public ITableMapping LoadTableMapping(Type Class)
        {
            object[] TAttrs = Class.GetCustomAttributes(typeof(TableMappingAttribute), false);
            if (TAttrs == null || TAttrs.Length == 0)
            {
                throw new MappingException("无法识别的实体类型[" + Class.FullName + "],找不到表映射关系");
            }
            TableMappingAttribute TblMappingAttr = TAttrs[0] as TableMappingAttribute;
            TableMapping TabMapping = new TableMapping();
            TabMapping.TypeName = TblMappingAttr.TableName;
            TabMapping.TableName = TblMappingAttr.TableName;
            TabMapping.Generator = TblMappingAttr.KeyGenerator;
            TabMapping.IsTable = TblMappingAttr.IsTable;

            //开始查找属性映射
            PropertyInfo[] Properties = Class.GetProperties();
            if (Properties != null && Properties.Length > 0)
            {
                foreach (PropertyInfo Property in Properties)
                {
                    object[] FAttrs = Property.GetCustomAttributes(typeof(FieldMappingAttribute), false);
                    if (FAttrs != null && FAttrs.Length > 0)
                    {
                        FieldMappingAttribute FMAttr = FAttrs[0] as FieldMappingAttribute;
                        FieldMapping FM = new FieldMapping();
                        FM.PropertyName = FMAttr.PropertyName;
                        FM.ColumnName = FMAttr.ColumnName;
                        FM.DefauleValue = FMAttr.DefauleValue;
                        FM.FieldLength = FMAttr.FieldLength;
                        FM.FieldType = FMAttr.FieldType;
                        FM.IsNullable = FMAttr.IsNullable;
                        FM.Remark = FMAttr.Remark;
                        TabMapping.FieldMappingCollection.Add(FM);
                    }
                    object[] KeyAttrs = Property.GetCustomAttributes(typeof(PrimaryKeyMappingAttribute), false);
                    if (KeyAttrs != null && KeyAttrs.Length > 0)
                    {
                        PrimaryKeyMappingAttribute FMAttr = KeyAttrs[0] as PrimaryKeyMappingAttribute;
                        PrimaryKeyMapping FM = new PrimaryKeyMapping();
                        FM.PropertyName = FMAttr.PropertyName;
                        FM.ColumnName = FMAttr.ColumnName;
                        FM.FieldLength = FMAttr.FieldLength;
                        FM.FieldType = FMAttr.FieldType;
                        FM.Remark = FMAttr.Remark;
                        TabMapping.PrimaryKeyCollection.Add(FM);
                    }
                }
            }
            return TabMapping;
        }
    }
}
