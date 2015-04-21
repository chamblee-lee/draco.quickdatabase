using System;
using System.Data;
using Draco.DB.ORM.Mapping;
using Draco.DB.ORM.Common;
using Draco.DB.ORM.Schema;
using Draco.DB.QuickDataBase.Schema;

namespace Draco.DB.ORM.Schema
{
    /// <summary>
    /// 数据类型转换类
    /// </summary>
    public class DataTypeConvert 
    {
        /// <summary>
        /// ADO类型转换为.NET类型
        /// </summary>
        /// <param name="adoType">ADO类型</param>
        /// <returns></returns>
        public static Type ConvertToNETFxType(ADOType adoType)
        {
            switch (adoType)
            {
                case ADOType.adTinyInt: return typeof(SByte);
                case ADOType.adSmallInt: return typeof(Int16);
                case ADOType.adInteger: return typeof(Int32);
                case ADOType.adBigInt: return typeof(Int64);

                case ADOType.adUnsignedTinyInt: return typeof(Int16);
                case ADOType.adUnsignedSmallInt: return typeof(Int32);
                case ADOType.adUnsignedInt: return typeof(Int64);
                case ADOType.adUnsignedBigInt: return typeof(Decimal);

                case ADOType.adDecimal: return typeof(Decimal);
                case ADOType.adNumeric: return typeof(Decimal);
                case ADOType.adSingle: return typeof(Single);
                case ADOType.adDouble: return typeof(Double);

                case ADOType.adDate:
                case ADOType.adDBDate:
                case ADOType.adDBTime:
                case ADOType.adDBTimeStamp:
                case ADOType.adFileTime: return typeof(DateTime);

                case ADOType.adBoolean: return typeof(Byte);

                case ADOType.adBSTR:
                case ADOType.adChar:
                case ADOType.adVarChar:
                case ADOType.adVarWChar:
                case ADOType.adLongVarWChar:
                case ADOType.cadWChar:
                case ADOType.adLongVarChar: return typeof(String);

                case ADOType.adVarBinary:
                case ADOType.adLongVarBinary:
                case ADOType.adBinary: return typeof(byte[]);

                case ADOType.adEmpty:
                case ADOType.adUserDefined:
                case ADOType.adVariant:
                case ADOType.adIDispatch:
                case ADOType.adIUnknown:
                case ADOType.adGUID:
                case ADOType.adCurrency:
                case ADOType.adChapter:
                case ADOType.adPropVariant:
                default:
                    {
                        throw (new ArgumentException("不被支持的数据类型(" + adoType + ")！", "ConvertToNETFxType"));
                    }
            }
        }
        /// <summary>
        /// ADO类型转换为FieldType类型
        /// </summary>
        /// <param name="adoType">ADO类型</param>
        /// <returns></returns>
        public static FieldType ConvertToFieldType(ADOType adoType)
        {
            switch (adoType)
            {
                case ADOType.adTinyInt: return FieldType.Int;
                case ADOType.adSmallInt: return FieldType.Int;
                case ADOType.adInteger: return FieldType.Int;
                case ADOType.adBigInt: return FieldType.Long;

                case ADOType.adUnsignedTinyInt: 
                case ADOType.adUnsignedSmallInt:
                case ADOType.adUnsignedInt: return FieldType.Int;
                case ADOType.adUnsignedBigInt: return FieldType.Long;

                case ADOType.adDecimal:
                case ADOType.adNumeric: 
                case ADOType.adSingle: 
                case ADOType.adDouble: return FieldType.Decimal;

                case ADOType.adDate:
                case ADOType.adDBDate:
                case ADOType.adDBTime:
                case ADOType.adDBTimeStamp:
                case ADOType.adFileTime: return FieldType.DateTime;

                case ADOType.adBoolean: return FieldType.Bit;

                case ADOType.adBSTR:
                case ADOType.adChar:
                case ADOType.adVarChar:
                case ADOType.adVarWChar:
                case ADOType.adLongVarWChar:
                case ADOType.cadWChar:
                case ADOType.adLongVarChar: return FieldType.String;

                case ADOType.adVarBinary:
                case ADOType.adLongVarBinary:
                case ADOType.adBinary: return FieldType.Binary;

                case ADOType.adEmpty:
                case ADOType.adUserDefined:
                case ADOType.adVariant:
                case ADOType.adIDispatch:
                case ADOType.adIUnknown:
                case ADOType.adGUID:
                case ADOType.adCurrency:
                case ADOType.adChapter:
                case ADOType.adPropVariant:
                default:
                    {
                        throw (new ArgumentException("不被支持的数据类型(" + adoType + ")！", "ConvertToNETFxType"));
                    }
            }
        }
    }
}
