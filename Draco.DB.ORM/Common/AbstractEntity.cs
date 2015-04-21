using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Draco.DB.ORM.Common
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class AbstractEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性已改变
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 所有改变的属性已清空
        /// </summary>
        public event EventHandler ChangedPropertiesCleared;
        /// <summary>
        /// 改变的属性
        /// </summary>
        private Hashtable m_ChangedProperty = new Hashtable();

        /// <summary>
        /// 已改变的属性值
        /// </summary>
        public Hashtable ChangedProperties
        {
            get
            {
                return m_ChangedProperty;
            }
        }

        /// <summary>
        /// 属性已改变
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="Value"></param>
        protected void OnPropertyChanged(string Property,object Value)
        {
            if(Value!=null)
                m_ChangedProperty[Property] = Value;
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }

        /// <summary>
        /// 清空改变的属性
        /// </summary>
        public void ClearChangedProperty()
        {
            this.m_ChangedProperty.Clear();
            if (ChangedPropertiesCleared != null)
            {
                this.ChangedPropertiesCleared(this, new EventArgs());
            }
        }
    }
}
