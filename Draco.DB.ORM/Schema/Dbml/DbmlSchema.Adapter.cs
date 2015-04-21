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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Draco.DB.ORM.Utility;

namespace Draco.DB.ORM.Schema.Dbml
{
    #region 基本集合接口
    /// <summary>
    /// 简单列表接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISimpleList<T> : IEnumerable<T>
    {
        /// <summary>
        /// sort of light IList
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sorter"></param>
        void Sort(IComparer<T> sorter);
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        List<T> FindAll(Predicate<T> match);
    }
    /// <summary>
    /// 名称类型接口
    /// </summary>
    public interface INamedType
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
    }
    #endregion

    #region 集合接口实现
    /// <summary>
    /// 枚举类型
    /// </summary>
    public class EnumType : IDictionary<string, int>, INamedType
    {
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                UpdateMember();
            }
        }

        private readonly IDictionary<string, int> dictionary;
        private readonly object owner;
        private readonly MemberInfo memberInfo;

        internal static bool IsEnum(string literalType)
        {
            string enumName;
            IDictionary<string, int> values;
            return Extract(literalType, out enumName, out values);
        }

        /// <summary>
        /// Extracts enum name and value from a given string.
        /// The string is in the following form:
        /// enumName key1[=value1]{,keyN[=valueN]}
        /// if enumName is 'enum', then the enum is anonymous
        /// </summary>
        /// <param name="literalType"></param>
        /// <param name="enumName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static bool Extract(string literalType, out string enumName, out IDictionary<string, int> values)
        {
            enumName = null;
            values = new Dictionary<string, int>();

            if (string.IsNullOrEmpty(literalType))
                return false;

            var nameValues = literalType.Split(new[] { ' ' }, 2);
            if (nameValues.Length == 2)
            {
                // extract the name
                string name = nameValues[0].Trim();
                if (!CharacterHelper.IsIdentifier(name))
                    return false;

                // now extract the values
                IDictionary<string, int> readValues = new Dictionary<string, int>();
                int currentValue = 1;
                var keyValues = nameValues[1].Split(',');
                foreach (var keyValue in keyValues)
                {
                    // a value may indicate its numeric equivalent, or not (in this case, we work the same way as C# enums, with an implicit counter)
                    var keyValueParts = keyValue.Split(new[] { '=' }, 2);
                    var key = keyValueParts[0].Trim();

                    if (!CharacterHelper.IsIdentifier(key))
                        return false;

                    if (keyValueParts.Length > 1)
                    {
                        if (!int.TryParse(keyValueParts[1], out currentValue))
                            return false;
                    }
                    readValues[key] = currentValue++;
                }
                if (name == "enum")
                    enumName = string.Empty;
                else
                    enumName = name;
                values = readValues;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Does the opposite: creates a literal string from values
        /// </summary>
        private void UpdateMember()
        {
            //var pairs = from kvp in dictionary orderby kvp.Value select kvp;//按值排序,在这里忽略，因为不会使用本方法
            //var pairs = dictionary.
            int currentValue = 1;
            var keyValues = new List<string>();
            foreach (var pair in dictionary)
            {
                string keyValue;
                if (pair.Value == currentValue)
                    keyValue = pair.Key;
                else
                {
                    currentValue = pair.Value;
                    keyValue = string.Format("{0}={1}", pair.Key, pair.Value);
                }
                keyValues.Add(keyValue);
                currentValue++;
            }
            string literalType = string.IsNullOrEmpty(Name) ? "enum" : Name;
            literalType += " ";
            literalType += string.Join(", ", keyValues.ToArray());
            MemberInfoExtensions.SetMemberValue(memberInfo,owner, literalType);
        }
        
        internal EnumType(object owner, MemberInfo memberInfo)
        {
            this.owner = owner;
            this.memberInfo = memberInfo;
            string name;
            Extract((string)MemberInfoExtensions.GetMemberValue(memberInfo,owner), out name, out dictionary);
            Name = name;
        }

        #region IDictionary implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, int> item)
        {
            dictionary.Add(item);
            UpdateMember();
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            UpdateMember();
        }
        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, int> item)
        {
            return dictionary.Contains(item);
        }
        /// <summary>
        /// 拷贝
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, int> item)
        {
            bool removed = dictionary.Remove(item);
            UpdateMember();
            return removed;
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }
        /// <summary>
        /// 只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }
        /// <summary>
        /// 包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, int value)
        {
            dictionary.Add(key, value);
            UpdateMember();
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            bool removed = dictionary.Remove(key);
            UpdateMember();
            return removed;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out int value)
        {
            return dictionary.TryGetValue(key, out value);
        }
        /// <summary>
        /// 索引获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int this[string key]
        {
            get { return dictionary[key]; }
            set
            {
                dictionary[key] = value;
                UpdateMember();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<int> Values
        {
            get { return dictionary.Values; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, int>>)this).GetEnumerator();
        }
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion
    }

    [DebuggerDisplay("{reflectedMember}")]
    internal class ArrayHelper<T> : ISimpleList<T>
    {
        private object owner;
        private MemberInfo memberInfo;
        // just to be debugger friendly
        private object reflectedMember { get { return MemberInfoExtensions.GetMemberValue(memberInfo,owner); } }

        protected IEnumerable GetValue()
        {
            return (IEnumerable)MemberInfoExtensions.GetMemberValue(memberInfo, owner);
        }

        protected System.Type GetValueType()
        {
            return MemberInfoExtensions.GetMemberType(memberInfo);
        }

        protected void SetValue(IEnumerable value)
        {
            MemberInfoExtensions.SetMemberValue(memberInfo,owner, value);
        }

        protected List<T> GetDynamic()
        {
            List<T> list = new List<T>();
            var fieldValue = GetValue();
            if (fieldValue != null)
            {
                foreach (object o in fieldValue)
                {
                    if (o is T)
                        list.Add((T)o);
                }
            }
            return list;
        }

        protected void SetStatic(IList<T> list)
        {
            var others = new ArrayList();
            var fieldValue = GetValue();
            if (fieldValue != null)
            {
                foreach (object o in fieldValue)
                {
                    if (!(o is T))
                        others.Add(o);
                }
            }
            var array = Array.CreateInstance(GetValueType().GetElementType(), others.Count + list.Count);
            others.CopyTo(array);
            for (int listIndex = 0; listIndex < list.Count; listIndex++)
            {
                array.SetValue(list[listIndex], others.Count + listIndex);
            }
            SetValue(array);
        }

        public ArrayHelper(object o, string fieldName)
        {
            owner = o;
            memberInfo = o.GetType().GetMember(fieldName)[0];
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return GetDynamic().IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            IList<T> dynamicArray = GetDynamic();
            dynamicArray.Insert(index, item);
            SetStatic(dynamicArray);
        }

        public void RemoveAt(int index)
        {
            IList<T> dynamicArray = GetDynamic();
            dynamicArray.RemoveAt(index);
            SetStatic(dynamicArray);
        }

        public T this[int index]
        {
            get
            {
                return GetDynamic()[index];
            }
            set
            {
                IList<T> dynamicArray = GetDynamic();
                dynamicArray[index] = value;
                SetStatic(dynamicArray);
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            IList<T> dynamicArray = GetDynamic();
            dynamicArray.Add(item);
            SetStatic(dynamicArray);
        }

        public void Clear()
        {
            SetStatic(new T[0]);
        }

        public bool Contains(T item)
        {
            return GetDynamic().Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            GetDynamic().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return GetDynamic().Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            IList<T> dynamicArray = GetDynamic();
            bool removed = dynamicArray.Remove(item);
            SetStatic(dynamicArray);
            return removed;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return GetDynamic().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetDynamic().GetEnumerator();
        }

        #endregion

        public void Sort(IComparer<T> sorter)
        {
            List<T> list = GetDynamic();
            list.Sort(sorter);
            SetStatic(list);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return GetDynamic().FindAll(match);
        }
    }

    /// <summary>
    /// The schema generates *Specified properties that we must set when the related property changes
    /// </summary>
    internal static class SpecifiedHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notify"></param>
        public static void Register(INotifyPropertyChanged notify)
        {
            notify.PropertyChanged += Notify_PropertyChanged;
        }

        private static void Notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.EndsWith("Specified"))
                return;

            PropertyInfo propertyInfo = sender.GetType().GetProperty(e.PropertyName);
            bool? changed = true;
            if (changed.HasValue)
            {
                PropertyInfo specifiedPropertyInfo = sender.GetType().GetProperty(e.PropertyName + "Specified");
                if (specifiedPropertyInfo != null)
                    specifiedPropertyInfo.GetSetMethod().Invoke(sender, new object[] { changed.Value });
            }
        }
    }
    #endregion

    public partial class Database
    {
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ISimpleList<Table> Tables;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ISimpleList<Function> Functions;

        /// <summary>
        /// 
        /// </summary>
        public Database()
        {
            SpecifiedHelper.Register(this);
            Tables = new ArrayHelper<Table>(this, "Table");
            Functions = new ArrayHelper<Function>(this, "Function");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// 
        /// </summary>
        public Table()
        {
            Type = new Type();
            SpecifiedHelper.Register(this);
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool _isChild;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} ({1}), {2}", Member, Name, Type);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Type
    {
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ISimpleList<Column> Columns;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ISimpleList<Association> Associations;

        /// <summary>
        /// 
        /// </summary>
        public Type()
        {
            SpecifiedHelper.Register(this);
            Columns = new ArrayHelper<Column>(this, "Items");
            Associations = new ArrayHelper<Association>(this, "Items");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Columns.Count + " Columns";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Function
    {
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool BodyContainsSelectStatement;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public ISimpleList<Parameter> Parameters;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public Return Return
        {
            get
            {
                if (Items == null)
                    return null;
                foreach (object item in Items)
                {
                    var r = item as Return;
                    if (r != null)
                        return r;
                }
                return null;
            }
            set
            {
                if (Items == null)
                {
                    Items = new[] { value };
                    return;
                }
                for (int index = 0; index < Items.Length; index++)
                {
                    if (Items[index] is Return)
                    {
                        Items[index] = value;
                        return;
                    }
                }
                List<object> items = new List<object>(Items);
                items.Add(value);
                Items = items.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public object ElementType;

        /// <summary>
        /// 
        /// </summary>
        public Function()
        {
            SpecifiedHelper.Register(this);
            Parameters = new ArrayHelper<Parameter>(this, "Parameter");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool DirectionIn
        {
            get { return Direction == ParameterDirection.In || Direction == ParameterDirection.InOut; }
        }
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool DirectionOut
        {
            get { return Direction == ParameterDirection.Out || Direction == ParameterDirection.InOut; }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Association
    {
        /// <summary>
        /// 
        /// </summary>
        public Association()
        {
            SpecifiedHelper.Register(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Column
    {
        private int m_ADOType;
        private long m_FieldLength;
        private string m_Comment;
        private INamedType extendedType;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public INamedType ExtendedType
        {
            get
            {
                if (extendedType == null)
                {
                    if (EnumType.IsEnum(Type))
                        extendedType = new EnumType(this, TypeMemberInfo);
                }
                return extendedType;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EnumType SetExtendedTypeAsEnumType()
        {
            return new EnumType(this, TypeMemberInfo);
        }
        /// <summary>
        /// 
        /// </summary>
        private MemberInfo TypeMemberInfo
        {
            get
            {
                return GetType().GetMember("Type")[0];
            }
        }
        /// <summary>
        /// 列
        /// </summary>
        public Column()
        {
            SpecifiedHelper.Register(this);
        }
        /// <summary>
        /// ADOType
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int ADOType//LQB
        {
            get { return m_ADOType; }
            set { m_ADOType=value; }
        }
        /// <summary>
        /// 字段长度
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long FieldLength//LQB
        {
            get { return m_FieldLength; }
            set { m_FieldLength = value; }
        }
        /// <summary>
        /// 注释
        /// </summary>
        internal string Comment//LQB
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} ({1}): {2} ({3})", Member, Name, Type, DbType);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Connection
    {
        /// <summary>
        /// 
        /// </summary>
        public Connection()
        {
            SpecifiedHelper.Register(this);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        public Parameter()
        {
            SpecifiedHelper.Register(this);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class Return
    {
        /// <summary>
        /// 
        /// </summary>
        public Return()
        {
            SpecifiedHelper.Register(this);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class TableFunction
    {
        /// <summary>
        /// 
        /// </summary>
        public TableFunction()
        {
            SpecifiedHelper.Register(this);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class TableFunctionParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public TableFunctionParameter()
        {
            SpecifiedHelper.Register(this);
        }
    }

    /// <summary>
    /// 表函数返回值对象
    /// </summary>
    public partial class TableFunctionReturn
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TableFunctionReturn()
        {
            SpecifiedHelper.Register(this);
        }
    }

}
