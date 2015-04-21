using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 通用SQL
    /// </summary>
    public class CommonSQL
    {
        /// <summary>
        /// SELECT {0} FROM {1} WHERE {2} 
        /// </summary>
        public const string SQL_SELECT = " SELECT {0} FROM {1} WHERE {2} ";
        /// <summary>
        /// INSERT INTO {0}({1})VALUES({2})
        /// </summary>
        public const string SQL_INSERT = " INSERT INTO {0}({1})VALUES({2})";
        /// <summary>
        /// UPDATE {0}  SET {1} WHERE {2}
        /// </summary>
        public const string SQL_UPDATE = " UPDATE {0}  SET {1} WHERE {2} ";
        /// <summary>
        ///  DELETE FROM {0} WHERE {1} 
        /// </summary>
        public const string SQL_DELETE = " DELETE FROM {0} WHERE {1} ";

        /// <summary>
        /// {0}={1}
        /// </summary>
        public const string KeyValuePair = " {0}={1} ";

        /// <summary>
        /// INSERT
        /// </summary>
        public const string INSERT = " INSERT ";
        /// <summary>
        /// UPDATE
        /// </summary>
        public const string UPDATE = " UPDATE ";
        /// <summary>
        /// DELETE
        /// </summary>
        public const string DELETE = " DELETE ";
        /// <summary>
        /// WHERE
        /// </summary>
        public const string WHERE = " WHERE ";
        /// <summary>
        /// AND
        /// </summary>
        public const string AND = " AND ";
        /// <summary>
        /// OR
        /// </summary>
        public const string OR = " OR ";
    }
}
