using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Schema
{
    /// <summary>
    /// ADO数据类型
    /// http://www.w3school.com.cn/ado/ado_datatypes.asp
    /// </summary>
    public enum ADOType
    {
        /// <summary>
        /// 空
        /// </summary>
        adEmpty = 0,
        /// <summary>
        /// 小整型
        /// </summary>
		adTinyInt = 16,
        /// <summary>
        /// 短整型
        /// </summary>
		adSmallInt = 2,
        /// <summary>
        /// 整型
        /// </summary>
		adInteger = 3,
        /// <summary>
        /// 长整形
        /// </summary>
		adBigInt = 20,
        /// <summary>
        /// 无符号小整型
        /// </summary>
		adUnsignedTinyInt = 17,
        /// <summary>
        /// 无符号短整型
        /// </summary>
		adUnsignedSmallInt = 18,
        /// <summary>
        /// 无符号整形
        /// </summary>
		adUnsignedInt = 19,
        /// <summary>
        /// 无符号长整形
        /// </summary>
		adUnsignedBigInt = 21,
        /// <summary>
        /// 单精度型
        /// </summary>
		adSingle = 4,
        /// <summary>
        /// 浮点型
        /// </summary>
		adDouble = 5,
        /// <summary>
        /// 
        /// </summary>
		adCurrency = 6,
        /// <summary>
        /// 小数型
        /// </summary>
		adDecimal = 14,
        /// <summary>
        /// 数字型
        /// </summary>
		adNumeric = 131,
        /// <summary>
        /// 布尔型
        /// </summary>
		adBoolean = 11,
        /// <summary>
        /// 错误
        /// </summary>
		adError = 10,
        /// <summary>
        /// 自定义型
        /// </summary>
		adUserDefined = 132,
        /// <summary>
        /// 
        /// </summary>
		adVariant = 12,
        /// <summary>
        /// 接口型
        /// </summary>
		adIDispatch = 9,
        /// <summary>
        /// 未知
        /// </summary>
		adIUnknown = 13,
        /// <summary>
        /// GUID型
        /// </summary>
		adGUID = 72,
        /// <summary>
        /// 日期型
        /// </summary>
		adDate = 7,
        /// <summary>
        /// 数据库日期型
        /// </summary>
		adDBDate = 133,
        /// <summary>
        /// 数据库时间型
        /// </summary>
		adDBTime = 134,
        /// <summary>
        /// 时间戳型
        /// </summary>
		adDBTimeStamp = 135,
        /// <summary>
        /// COM字符
        /// </summary>
		adBSTR = 8,
        /// <summary>
        /// 字符型
        /// </summary>
		adChar = 129,
        /// <summary>
        /// 可变字符型
        /// </summary>
		adVarChar = 200,
        /// <summary>
        /// 可变长字符型
        /// </summary>
		adLongVarChar = 201,
        /// <summary>
        /// 宽字符型
        /// </summary>
		cadWChar = 130,
        /// <summary>
        /// 可变宽字符型
        /// </summary>
		adVarWChar = 202,
        /// <summary>
        /// 可变长宽字符型
        /// </summary>
		adLongVarWChar = 203,
        /// <summary>
        /// 二进制型
        /// </summary>
		adBinary = 128,
        /// <summary>
        /// 可变二进制型
        /// </summary>
		adVarBinary = 204,
        /// <summary>
        /// 可变长二进制型
        /// </summary>
		adLongVarBinary = 205,
        /// <summary>
        /// 
        /// </summary>
		adChapter = 136,
        /// <summary>
        /// 文件时间
        /// </summary>
		adFileTime = 64,
        /// <summary>
        /// 
        /// </summary>
		adPropVariant = 138,
        /// <summary>
        /// 可变数字
        /// </summary>
		adVarNumeric = 139
    }
}
