using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Draco.DB.QuickDataBase.Schema.Vendor;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;
using System.Collections;

namespace Draco.DB.QuickDataBase
{
    /// <summary>
    /// 数据库构架操作接口
    /// </summary>
    public interface IDataBaseSchemaHandler
    {
        /// <summary>
        /// 获取所有的表名称
        /// </summary>
        /// <returns></returns>
        List<String> GetTableNames();
        /// <summary>
        /// 获取单表的数据字段信息
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <returns></returns>
        IList<IDataTableColumn> ReadColumns(string TableName);
        /// <summary>
        /// 添加表字段
        /// </summary>
        /// <param name="strTblName">表名称</param>
        /// <param name="strFldName">字段名称</param>
        /// <param name="strFldDes"></param>
        /// <returns></returns>
        bool AddFieldToTable(string strTblName, string strFldName, string strFldDes);
        /// <summary>
        /// 获取对表的注释信息
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <returns></returns>
        String GetTableComment(String TableName);
        /// <summary>
        /// 获取字段的注释信息
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="ColumnName">字段名称</param>
        /// <returns></returns>
        String GetColumnComment(String TableName, String ColumnName);
        /// <summary>
        /// 获取字段的数据类型
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="ColumnName">字段名称</param>
        /// <returns></returns>
        String GetDataTypeByColumnName(String TableName, String ColumnName);
        /// <summary>
        /// 把字符表示的数据库类型转换为DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DbType ConvertToDbType(string type);
        /// <summary>
        /// 表是否存在序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        bool IsTableHasSequence(String TableName);
        /// <summary>
        /// 取得列的默认值
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        string GetColumnDefaultValue(string TableName, string ColumnName);
        /// <summary>
        /// 判断是否是子增长列
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="ColumnName">列名称</param>
        /// <returns></returns>
        bool IsAotuAddColumn(string TableName, string ColumnName);
        /// <summary>
        /// 同步序列
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="PKFieldName"></param>
        void SynchSequence(string TableName,string PKFieldName);

        /// <summary>
        /// 映射到ADO类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        int MapAdoType(IDataType dataType);

        /// <summary>
        /// 映射到DB类型
        /// </summary>
        /// <param name="datatablecolumn"></param>
        /// <returns></returns>
        void MapDBType(ref DataTableColumn datatablecolumn);
        
        /// <summary>
        /// 根据表结构列表创建建表的Sql语句
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        string GetDbServerScript(ArrayList List);

        /// <summary>
        /// 获取表信息sql
        /// </summary>
        /// <param name="tblname"></param>
        /// <returns></returns>
        string GetSqlOfTableInfo(string tblname);

        /// <summary>
        /// 获取修改字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string GetAlterSqlOfColumnInfo(DataTableColumn field);

        /// <summary>
        /// 获取添加字段的sql语句
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string GetAlterAddSqlOfColumnInfo(DataTableColumn field);
    }
}
