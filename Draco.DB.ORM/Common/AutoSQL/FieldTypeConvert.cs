using System;
using System.Data;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema;

namespace Draco.DB.ORM.Common.AutoSQL
{
    /// <summary>
    /// 数据库字段类型转换
    /// </summary>
    public interface IFieldTypeConvert
    {
        /// <summary>
        /// 把FieldType转换为DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DbType ConvertToDbType(FieldType type);
        /// <summary>
        /// 把FieldType转换为Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Type ConvertToNETFxType(FieldType type);
    }

    /// <summary>
    /// IFieldTypeConvert实现
    /// </summary>
    public class FieldTypeConvert : IFieldTypeConvert
    {
        /// <summary>
        /// 把FieldType转换为DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DbType ConvertToDbType(FieldType type)
        {
            switch (type)
            {
                case FieldType.Int: return DbType.Int32;
                case FieldType.Long: return DbType.Int64;
                case FieldType.Decimal: return DbType.Decimal;
                case FieldType.String:
                case FieldType.Text:return DbType.String;
                case FieldType.DateTime: return DbType.DateTime;
                case FieldType.Binary: return DbType.Binary;
                case FieldType.Bit: return DbType.Boolean;
                case FieldType.Byte: return DbType.Byte;
                default: return DbType.String;
            }
        }
        /// <summary>
        /// 把FieldType转换为Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Type ConvertToNETFxType(FieldType type)
        {
            switch (type)
            {
                case FieldType.Int: return typeof(Int32);
                case FieldType.Long: return typeof(Int64);
                case FieldType.Decimal: return typeof(Double);
                case FieldType.String:
                case FieldType.Text: return typeof(String);
                case FieldType.DateTime: return typeof(DateTime);
                case FieldType.Binary: return typeof(byte[]);
                case FieldType.Bit: return typeof(Boolean);
                case FieldType.Byte: return typeof(Byte);
                default: return typeof(String);
            }
        }
    }
}
