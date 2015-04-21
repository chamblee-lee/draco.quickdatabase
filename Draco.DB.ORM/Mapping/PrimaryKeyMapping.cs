using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.ORM.Mapping
{
    /// <summary>
    /// 主键映射实现
    /// </summary>
    public class PrimaryKeyMapping : FieldMapping,IPrimaryKeyMapping
    {
        /// <summary>
        /// 不可为空
        /// </summary>
        public override bool IsNullable
        {
            get{return false;}
            set{}
        }
        /// <summary>
        /// 缺省值为空
        /// </summary>
        public override string DefauleValue
        {
            get{return "";}
            set{}
        }
    }
}
