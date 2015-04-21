using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Draco.DB.ORM.Utility
{
    /// <summary>
    /// 关于MemberInfo的方法扩展
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Returns the type of the specified member
        /// </summary>
        /// <param name="memberInfo">member to get type from</param>
        /// <returns>Member type</returns>
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;
            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;
            if (memberInfo is MethodInfo)
                return ((MethodInfo)memberInfo).ReturnType;
            if (memberInfo is ConstructorInfo)
                return null;
            throw new ArgumentException();
        }

        /// <summary>
        /// Gets a field/property
        /// </summary>
        /// <param name="memberInfo">The memberInfo specifying the object</param>
        /// <param name="o">The object</param>
        public static object GetMemberValue(MemberInfo memberInfo, object o)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).GetValue(o);
            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).GetGetMethod().Invoke(o, new object[0]);
            throw new ArgumentException();
        }

        /// <summary>
        /// Sets a field/property
        /// </summary>
        /// <param name="memberInfo">The memberInfo specifying the object</param>
        /// <param name="o">The object</param>
        /// <param name="value">The field/property value to assign</param>
        public static void SetMemberValue(MemberInfo memberInfo, object o, object value)
        {
            if (memberInfo is FieldInfo)
                ((FieldInfo)memberInfo).SetValue(o, value);
            else if (memberInfo is PropertyInfo)
                ((PropertyInfo)memberInfo).GetSetMethod().Invoke(o, new[] { value });
            else throw new ArgumentException();
        }
    }
}
