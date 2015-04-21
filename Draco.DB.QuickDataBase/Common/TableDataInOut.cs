using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Draco.DB.QuickDataBase.Common
{
    /// <summary>
    /// 数据库表的导入和导出
    /// </summary>
    public class TableDataInOut
    {
        private IDataBaseHandler m_handler;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public TableDataInOut(IDataBaseHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException();
            m_handler = handler;
        }

        /// <summary>
        /// 保存所有数据表数据到指定的文件中
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public void SaveAllTableDataToXml(string filepath)
        {
            DataSet ds = new DataSet();
            BuildTblList(ref ds);
            SaveDataSetDataToXml(ds, filepath);
        }

        /// <summary>
        /// 保存指定数据表数据到指定的文件中
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="tblnamelist"></param>
        public void SaveSomeTableDataToXml(string filepath, List<string> tblnamelist)
        {
            DataSet ds = new DataSet();
            BuildTblList(ref ds, tblnamelist);
            SaveDataSetDataToXml(ds, filepath);
        }

        /// <summary>
        /// 保存DataSet数据到指定的文件中
        /// </summary>
        /// <param name="ds">DataSet数据集</param>
        /// <param name="filepath">文件路径</param>
        public void SaveDataSetDataToXml(DataSet ds, string filepath)
        {
            if (!String.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                string xmlfile = filepath.Substring(0, filepath.LastIndexOf('.')) + ".xml";
                ds.WriteXml(xmlfile, System.Data.XmlWriteMode.WriteSchema);
            }
        }

        /// <summary>
        /// 构建所有数据表的DataSet数据集
        /// </summary>
        /// <param name="ds">构建出的DataSet数据集</param>
        private void BuildTblList(ref DataSet ds)
        {
            List<string> tnamelist = m_handler.DbAdapter.GetSchemaHandler(m_handler).GetTableNames();
            foreach (object item in tnamelist)
            {
                string tblName = item.ToString();
                ds.Tables.Add(GetDataFromTbl(tblName));
            }
        }

       
        /// <summary>
        /// 构建指定数据表的DataSet数据集
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="tblnamelist"></param>
        private void BuildTblList(ref DataSet ds, List<string> tblnamelist)
        {
            List<string> tnamelist = tblnamelist;
            foreach (object item in tnamelist)
            {
                string tblName = item.ToString();
                ds.Tables.Add(GetDataFromTbl(tblName));
            }
        }

        /// <summary>
        /// 取得表中所有的数据
        /// </summary>
        /// <param name="Tblname">待取得数据的表名</param>
        /// <returns></returns>
        public DataTable GetDataFromTbl(string Tblname)
        {
            if (Tblname == null || Tblname.Length == 0)
                throw new ArgumentNullException("Tblname");
            string Sql = Tblname;

            Sql = "select * from " + Sql;
            DataSet ds_SysMenu = this.m_handler.ExecuteQuery(Sql);
            ds_SysMenu.Tables[0].TableName = Tblname;
            return ds_SysMenu.Tables[0].Copy();
        }

        /// <summary>
        /// 从文件中获取数据至DataSet数据集
        /// </summary>
        /// <param name="filepath">xml文件路径</param>
        /// <returns></returns>
        public DataSet LoadXmlDataToDataSet(string filepath)
        {
            DataSet ds = new DataSet();
            if (!String.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                string XmlFileFullName = filepath;
                ds.ReadXml(XmlFileFullName);
            }
            return ds;
        }

        /// <summary>
        /// 从DataSet中获取数据至数据表
        /// </summary>
        /// <param name="ds">DataSet数据集</param>
        /// <returns></returns>
        public Boolean LoadDataSetToTable(DataSet ds)
        {
            string rtn = null;
            try
            {
                rtn = SetDataTblValueFromDataSet(ds);
                return true;
            }
            catch (Exception e)
            {
                System.Console.Write(e.ToString());
                throw e;
            }
        }

        /// <summary>
        /// 判断数据表是否插入时加上主键字段
        /// </summary>
        /// <param name="tbl">DataTable数据表</param>
        /// <returns></returns>
        private Boolean isInsertWithId(DataTable tbl)
        {
            bool b1 = tbl.Columns[0].DataType == typeof(Int32);
            bool b2 = tbl.Columns[0].DataType == typeof(Int64);
            IDataBaseSchemaHandler idbs = this.m_handler.DbAdapter.GetSchemaHandler(m_handler);

            if (idbs.IsAotuAddColumn(tbl.TableName, tbl.Columns[0].ColumnName))
                return false;

            if (b1 || b2)
            {
                return !idbs.IsAotuAddColumn(tbl.TableName, tbl.Columns[0].ColumnName);
            }
            return true;
        }

        /// <summary>
        /// 从DataSet中设置数据至数据表
        /// </summary>
        /// <param name="ds">DataSet数据集</param>
        /// <returns></returns>
        public string SetDataTblValueFromDataSet(DataSet ds)
        {
            string Tbllist = "";
            string tblName = null;
            try
            {
                //遍历每一个数据表
                foreach (DataTable tbl in ds.Tables)
                {
                    tblName = tbl.TableName;
                    Tbllist += tbl.TableName + "\n";
                    int rowsnum = tbl.Rows.Count;
                    int colsnum = tbl.Columns.Count;

                    //删除原有数据
                    this.m_handler.ExecuteNonQuery("delete from " + tblName);

                    //判断是否为自增主键
                    Boolean isinsertwithid = this.isInsertWithId(tbl);

                    //循环插入数据
                    for (int i = 0; i < rowsnum; i++)
                    {
                        ISQLGenerator gen = m_handler.DbAdapter.GetSQLGenerator();
                        IDataParameter[] paras = null;
                        Dictionary<String, Object> dic = new Dictionary<string, object>();

                        //判断是否是自增字段从而确定插入时是否加上主键字段
                        int jstart = (isinsertwithid ? 0 : 1);

                        for (int j = jstart; j < colsnum; j++)
                        {
                            object o = tbl.Rows[i][j];
                            if (o != null && o.ToString().Length > 0)
                            {
                                dic.Add(tbl.Columns[j].ColumnName, tbl.Rows[i][j]);
                            }
                        }
                        string sql = gen.CreateInsertSQL(out paras, tblName, dic);
                        m_handler.ExecuteNonQuery(sql, paras);
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception(tblName, ee);
            }
            return Tbllist;
        }

        /// <summary>
        /// 从外部xml文件中导入数据信息
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public void LoadDataFromXml(string filename)
        {
            DataSet ds = this.LoadXmlDataToDataSet(filename);
            this.LoadDataSetToTable(ds);
        }
    }
}
