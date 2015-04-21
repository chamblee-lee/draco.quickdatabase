using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 查询运算操作
    /// </summary>
    public class QueryOperator
    {
        /// <summary>
        /// 内联
        /// </summary>
        public const string Inline = " Inline ";
        /// <summary>
        /// 等于
        /// </summary>
        public const string Equal = "=";
        /// <summary>
        /// 不等于
        /// </summary>
        public const string NotEqual = "!=";
        /// <summary>
        /// 小于
        /// </summary>
        public const string Less = "<";
        /// <summary>
        /// 小于等于
        /// </summary>
        public const string LessAndEqual = "<=";
        /// <summary>
        /// 大于
        /// </summary>
        public const string Great = ">";
        /// <summary>
        /// 大于等于
        /// </summary>
        public const string GreatAndEqual = ">=";
        /// <summary>
        /// 模糊
        /// </summary>
        public const string Like = " like ";
        /// <summary>
        /// 集合
        /// </summary>
        public const string In = " in ";
        /// <summary>
        /// 转换为操作符
        /// </summary>
        /// <param name="OperatorEnum"></param>
        /// <returns></returns>
        public static string ConvertToOperator(QueryOperatorEnum OperatorEnum)
        {
            switch (OperatorEnum)
            {
                case QueryOperatorEnum.Equal: return QueryOperator.Equal;
                case QueryOperatorEnum.Great: return QueryOperator.Great;
                case QueryOperatorEnum.GreatAndEqual: return QueryOperator.GreatAndEqual;
                case QueryOperatorEnum.In: return QueryOperator.In;
                case QueryOperatorEnum.Inline: return QueryOperator.Inline;
                case QueryOperatorEnum.Less: return QueryOperator.Less;
                case QueryOperatorEnum.LessAndEqual: return QueryOperator.LessAndEqual;
                case QueryOperatorEnum.Like: return QueryOperator.Like;
                case QueryOperatorEnum.NotEqual: return QueryOperator.NotEqual;
            }
            return "unknown Operator";
        }
    }
    /// <summary>
    /// 查询运算操作枚举
    /// </summary>
    public enum QueryOperatorEnum
    {
        /// <summary>
        /// 内联
        /// </summary>
        Inline,
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// 小于
        /// </summary>
        Less,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessAndEqual,
        /// <summary>
        /// 大于
        /// </summary>
        Great,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreatAndEqual,
        /// <summary>
        /// 模糊
        /// </summary>
        Like,
        /// <summary>
        /// 集合
        /// </summary>
        In
    }
}
