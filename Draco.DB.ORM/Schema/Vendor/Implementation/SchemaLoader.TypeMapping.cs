#region MIT license
// 
// Copyright (c) 2007-2008 Jiri Moudry
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor;

namespace Draco.DB.ORM.Schema.Vendor.Implementation
{
    public partial class SchemaLoader
    {
        /// <summary>
        /// This class is used as fallback when no matching type was found.
        /// If we have the case, then something is missing from DbMetal
        /// </summary>
        public class UnknownType
        {
        }

        /// <summary>
        /// Default IDataType implementation (see IDataType for details)
        /// </summary>
        public class DataType : IDataType
        {
            /// <summary>
            /// 简单类型
            /// </summary>
            public virtual string SimpleType { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            public virtual string Type { get; set; }
            /// <summary>
            /// 是否可为空
            /// </summary>
            public virtual bool Nullable { get; set; }
            /// <summary>
            /// 长度
            /// </summary>
            public virtual long? Length { get; set; }
            /// <summary>
            /// 精度
            /// </summary>
            public virtual int? Precision { get; set; }
            /// <summary>
            /// 小数
            /// </summary>
            public virtual int? Scale { get; set; }
            /// <summary>
            /// 无符号
            /// </summary>
            public virtual bool? Unsigned { get; set; }
            /// <summary>
            /// 全类型
            /// </summary>
            public string FullType { get; set; }
        }
        /// <summary>
        /// 映射字段类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected virtual Type MapDbType(IDataType dataType)
        {
            if (dataType == null)
                return typeof(UnknownType);

            switch (dataType.Type.ToLower())
            {
            // string
            case "c":
            case "char":
            case "character":
            case "character varying":
            case "inet":
            case "long":
            case "longtext":
            case "long varchar":
            case "nchar":
            case "nvarchar":
            case "nvarchar2":
            case "string":
            case "text":
            case "varchar":
            case "varchar2":
            case "clob":
            case "nclob":
                return typeof(String);

            // bool
            case "bit":
            case "bool":
            case "boolean":
                return typeof(Boolean);

            // int8
            case "tinyint":
                if (dataType.Length == 1)
                    return typeof(Boolean);
                return typeof(Byte);

            // int16
            case "short":
            case "smallint":
                if (dataType.Unsigned ?? false)
                    return typeof(UInt16);
                return typeof(Int16);

            // int32
            case "int":
            case "integer":
            case "mediumint":
                if (dataType.Unsigned ?? false)
                    return typeof(UInt32);
                return typeof(Int32);

            // int64
            case "bigint":
                return typeof(Int64);

            // single
            case "float":
            case "float4":
            case "real":
                return typeof(float);

            // double
            case "double":
            case "double precision":
                return typeof(Double);

            // decimal
            case "numeric":
                return typeof(Decimal);
            case "decimal":
            case "number": // special oracle type
                if (!dataType.Precision.HasValue && !dataType.Scale.HasValue)//既无精度，也无有效位数，NUMBER类型
                {
                    return typeof(Int32);
                }
                if (dataType.Precision.HasValue && (dataType.Scale ?? 0) == 0)//有精度，且有效位数为0
                {
                    if (dataType.Precision.Value == 1)
                        return typeof(Boolean);
                    if (dataType.Precision.Value <= 4)
                        return typeof(Int16);
                    if (dataType.Precision.Value <= 9)
                        return typeof(Int32);
                    if (dataType.Precision.Value <= 19)
                        return typeof(Int64);
                }
                return typeof(Decimal);

            // time interval
            case "interval":
                return typeof(TimeSpan);

            //enum
            case "enum":
                return MapEnumDbType(dataType);

            // date
            case "date":
            case "datetime":
            case "ingresdate":
            case "timestamp":
            case "timestamp without time zone":
            case "time":
            case "time without time zone": //reported by twain_bu...@msn.com,
            case "time with time zone":
                return typeof(DateTime);

            // byte[]
            case "blob":
            case "bytea":
            case "byte varying":
            case "longblob":
            case "long byte":
            case "oid":
            case "sytea":
            case "image":
                return typeof(Byte[]);

            case "void":
                return null;

            // if we fall to this case, we must handle the type
            default:
                return typeof(UnknownType);
            }
        }

        /// <summary>
        /// 枚举
        /// </summary>
        protected class EnumType : Type
        {
            /// <summary>
            /// 构造
            /// </summary>
            public EnumType()
            {
                EnumValues = new Dictionary<string, int>();
            }
            /// <summary>
            /// 名称
            /// </summary>
            public string EnumName { get; set; }

            /// <summary>
            /// 值
            /// </summary>
            public IDictionary<string, int> EnumValues;

            #region Type overrides - the ones who make sense
            /// <summary>
            /// 
            /// </summary>
            public override string Name
            {
                get { return EnumName; }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Type BaseType
            {
                get { return typeof(Enum); }
            }
            /// <summary>
            /// 
            /// </summary>
            public override string FullName
            {
                get { return Name; } // this is a dynamic type without any qualification (namespace or assembly)
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool IsArrayImpl()
            {
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool IsByRefImpl()
            {
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool IsCOMObjectImpl()
            {
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool IsPointerImpl()
            {
                return false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool IsPrimitiveImpl()
            {
                return true;
            }

            #endregion

            #region Type overrides - the ones we don't care about
            /// <summary>
            /// 获取自定义属性
            /// </summary>
            /// <param name="attributeType"></param>
            /// <param name="inherit"></param>
            /// <returns></returns>
            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 获取自定义属性
            /// </summary>
            /// <param name="inherit"></param>
            /// <returns></returns>
            public override object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="attributeType"></param>
            /// <param name="inherit"></param>
            /// <returns></returns>
            public override bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            public override Assembly Assembly
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// 
            /// </summary>
            public override string AssemblyQualifiedName
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override TypeAttributes GetAttributeFlagsImpl()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <param name="binder"></param>
            /// <param name="callConvention"></param>
            /// <param name="types"></param>
            /// <param name="modifiers"></param>
            /// <returns></returns>
            protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder,
                                                                  CallingConventions callConvention, Type[] types,
                                                                  ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override Type GetElementType()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override EventInfo[] GetEvents(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override FieldInfo GetField(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override FieldInfo[] GetFields(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="ignoreCase"></param>
            /// <returns></returns>
            public override Type GetInterface(string name, bool ignoreCase)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override Type[] GetInterfaces()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="bindingAttr"></param>
            /// <param name="binder"></param>
            /// <param name="callConvention"></param>
            /// <param name="types"></param>
            /// <param name="modifiers"></param>
            /// <returns></returns>
            protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder,
                                                        CallingConventions callConvention, Type[] types,
                                                        ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override Type GetNestedType(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override Type[] GetNestedTypes(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bindingAttr"></param>
            /// <returns></returns>
            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="bindingAttr"></param>
            /// <param name="binder"></param>
            /// <param name="returnType"></param>
            /// <param name="types"></param>
            /// <param name="modifiers"></param>
            /// <returns></returns>
            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder,
                                                            Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            public override Guid GUID
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected override bool HasElementTypeImpl()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="invokeAttr"></param>
            /// <param name="binder"></param>
            /// <param name="target"></param>
            /// <param name="args"></param>
            /// <param name="modifiers"></param>
            /// <param name="culture"></param>
            /// <param name="namedParameters"></param>
            /// <returns></returns>
            public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target,
                                                object[] args, ParameterModifier[] modifiers, CultureInfo culture,
                                                string[] namedParameters)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            public override Module Module
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// 
            /// </summary>
            public override string Namespace
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Type UnderlyingSystemType
            {
                get { throw new NotImplementedException(); }
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        protected static Regex DefaultEnumDefinitionEx = new Regex(@"\s*enum\s*\((?<values>.*)\s*\)\s*", RegexOptions.Compiled);
        /// <summary>
        /// 
        /// </summary>
        protected static Regex EnumValuesEx = new Regex(@"\'(?<value>\w*)\'\s*,?\s*", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected virtual EnumType MapEnumDbType(IDataType dataType)
        {
            var enumType = new EnumType();
            // MySQL represents enums as follows:
            // enum('value1','value2')
            Match outerMatch = DefaultEnumDefinitionEx.Match(dataType.FullType);
            if (outerMatch.Success)
            {
                string values = outerMatch.Groups["values"].Value;
                var innerMatches = EnumValuesEx.Matches(values);
                int currentValue = 1;
                foreach (Match innerMatch in innerMatches)
                {
                    var value = innerMatch.Groups["value"].Value;
                    enumType.EnumValues[value] = currentValue++;
                }
            }
            return enumType;
        }
    }
}
