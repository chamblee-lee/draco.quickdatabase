using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace Draco.DB.ORM.Common.AutoSQL
{
    /// <summary>
    /// 条件表达式
    /// </summary>
    public class Expression
    {
        private string m_PropertyName;
        private OP m_OP = OP.Eq;
        private object m_value;
        private object m_value2;
        private ArrayList m_Or_Expressions;

        /// <summary>
        /// 实体类属性
        /// </summary>
        public string PropertyName
        {
            get { return m_PropertyName; }
        }
        /// <summary>
        /// 运算符
        /// </summary>
        public OP OP
        {
            get { return m_OP; }
        }
        /// <summary>
        /// 值1
        /// </summary>
        public object value
        {
            get { return m_value; }
        }
        /// <summary>
        /// 值2
        /// </summary>
        public object value2
        {
            get { return m_value2; }
        }
        /// <summary>
        /// 或运算
        /// </summary>
        public ArrayList Or_Expressions
        {
            get { return m_Or_Expressions; }
        }

        #region 私有构造
        /// <summary>
        /// 私有构造
        /// </summary>
        /// <param name="op"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private Expression(OP op,string propertyName, object value)
        {
            m_PropertyName = propertyName;
            m_value = value;
            m_OP = op;
        }
        private Expression(OP op, string propertyName)
        {
            m_PropertyName = propertyName;
            m_OP = op;
        }
        private Expression(OP op, string propertyName,object value1,object value2)
        {
            m_PropertyName = propertyName;
            m_value = value1;
            m_value2 = value2;
            m_OP = op;
        }
        private Expression()
        {
 
        }
        private Expression AddExpression(Expression exp)
        {
            if (m_Or_Expressions == null) 
                m_Or_Expressions = new ArrayList();
            m_Or_Expressions.Add(exp);
            return this;
        }
        #endregion

        #region 创建表达式
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Eq(string PropertyName, string value)
        {
            return new Expression(OP.Eq,PropertyName, value);
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Eq(string PropertyName, int value)
        {
            return new Expression(OP.Eq, PropertyName, value);
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Eq(string PropertyName, long value)
        {
            return new Expression(OP.Eq, PropertyName, value);
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Eq(string PropertyName, DateTime value)
        {
            return new Expression(OP.Eq, PropertyName, value);
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Eq(string PropertyName, object value)
        {
            return new Expression(OP.Eq, PropertyName, value);
        }
        /// <summary>
        /// like操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Like(string PropertyName, string value)
        {
            return new Expression(OP.Like,PropertyName, value);
        }
        /// <summary>
        /// 大于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Gt(string PropertyName, int value)
        {
            return new Expression(OP.Gt,PropertyName, value);
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Gt(string PropertyName, long value)
        {
            return new Expression(OP.Gt, PropertyName, value);
        }
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Gt(string PropertyName, DateTime value)
        {
            return new Expression(OP.Gt, PropertyName, value);
        }
        /// <summary>
        /// 小于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Lt(string PropertyName, int value)
        {
            return new Expression(OP.Lt,PropertyName, value);
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Lt(string PropertyName, long value)
        {
            return new Expression(OP.Lt, PropertyName, value);
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Lt(string PropertyName, DateTime value)
        {
            return new Expression(OP.Lt, PropertyName, value);
        }
        /// <summary>
        /// 大于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GtAndEq(string PropertyName, int value)
        {
            return new Expression(OP.Gt, PropertyName, value);
        }
        /// <summary>
        /// 大于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GtAndEq(string PropertyName, long value)
        {
            return new Expression(OP.Gt, PropertyName, value);
        }
        /// <summary>
        /// 大于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GtAndEq(string PropertyName, DateTime value)
        {
            return new Expression(OP.Gt, PropertyName, value);
        }
        /// <summary>
        /// 小于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression LtAndEq(string PropertyName, int value)
        {
            return new Expression(OP.Lt, PropertyName, value);
        }
        /// <summary>
        /// 小于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression LtAndEq(string PropertyName, long value)
        {
            return new Expression(OP.Lt, PropertyName, value);
        }
        /// <summary>
        /// 小于等于操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression LtAndEq(string PropertyName, DateTime value)
        {
            return new Expression(OP.Lt, PropertyName, value);
        }
        /// <summary>
        /// Between操作
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="Value1"></param>
        /// <param name="Value2"></param>
        /// <returns></returns>
        public static Expression Between(string PropertyName, int Value1, int Value2)
        {
            return new Expression(OP.Between, PropertyName, Value1, Value2);
        }
        /// <summary>
        /// Between
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="Value1"></param>
        /// <param name="Value2"></param>
        /// <returns></returns>
        public static Expression Between(string PropertyName, long Value1, long Value2)
        {
            return new Expression(OP.Between, PropertyName, Value1, Value2);
        }
        /// <summary>
        /// 空
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static Expression IsNull(string PropertyName)
        {
            return new Expression(OP.IsNull,PropertyName);
        }
        /// <summary>
        /// 非空
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static Expression IsNotNull(string PropertyName)
        {
            return new Expression(OP.IsNotNull,PropertyName);
        }
        /// <summary>
        /// Or
        /// </summary>
        /// <param name="ExpressionA"></param>
        /// <param name="ExpressionB"></param>
        /// <returns></returns>
        public static Expression Or(Expression ExpressionA, Expression ExpressionB)
        {
            return new Expression().AddExpression(ExpressionA).AddExpression(ExpressionB);
        }
        #endregion
    }
    /// <summary>
    /// 运算枚举
    /// </summary>
    public enum OP
    {
        /// <summary>
        /// 等于
        /// </summary>
        Eq, 
        /// <summary>
        /// 大于
        /// </summary>
        Gt,
        /// <summary>
        /// 小于
        /// </summary>
        Lt, 
        /// <summary>
        /// 大于等于
        /// </summary>
        GtAndEq,
        /// <summary>
        /// 小于等于
        /// </summary>
        LtAndEq,
        /// <summary>
        /// 空
        /// </summary>
        IsNull,
        /// <summary>
        /// 非空
        /// </summary>
        IsNotNull, 
        /// <summary>
        /// Like
        /// </summary>
        Like, 
        /// <summary>
        /// Between
        /// </summary>
        Between, 
        /// <summary>
        /// 或
        /// </summary>
        Or
    }
}
