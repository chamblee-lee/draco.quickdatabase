using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 公用方法集
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 把FieldType转换为字符
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        public static string ConvertFieldTypeToString(FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Binary: return "FieldType.Binary";
                case FieldType.Bit: return "FieldType.Bit";
                case FieldType.Byte: return "FieldType.Byte";
                case FieldType.DateTime: return "FieldType.DateTime";
                case FieldType.Decimal: return "FieldType.Decimal";
                case FieldType.Int: return "FieldType.Int";
                case FieldType.Long: return "FieldType.Long";
                case FieldType.String: return "FieldType.String";
                case FieldType.Text: return "FieldType.Text";
            }
            return "FieldType.String";
        }
        /// <summary>
        /// 把字符转换为字段类型
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static FieldType ParseFieldType(string fieldType)
        {
            switch (fieldType)
            {
                case "FieldType.Binary": return FieldType.Binary;
                case "FieldType.Bit": return FieldType.Bit;
                case "FieldType.Byte": return FieldType.Byte;
                case "FieldType.DateTime": return FieldType.DateTime;
                case "FieldType.Decimal": return FieldType.Decimal;
                case "FieldType.Int": return FieldType.Int;
                case "FieldType.Long": return FieldType.Long;
                case "FieldType.String": return FieldType.String;
                case "FieldType.Text": return FieldType.Text;
            }
            return FieldType.String;
        }
    }
}
