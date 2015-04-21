using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word;
using Draco.DB.QuickDataBase.Schema;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase;

namespace CodeGenerator
{
    /// <summary>
    /// 表结构信息到word文档
    /// </summary>
    class TableSchemaToWord : System.IDisposable
    {
        private IDataBaseHandler m_handler;

        private object nouseref = Type.Missing;
        private Microsoft.Office.Interop.Word.ApplicationClass m_wdapp;
        private Microsoft.Office.Interop.Word.Document m_wddoc;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="handler"></param>
        public TableSchemaToWord(IDataBaseHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException();
            m_handler = handler;

            m_wdapp = new Microsoft.Office.Interop.Word.ApplicationClass();
            m_wdapp.Visible = false;
            m_wddoc = new Microsoft.Office.Interop.Word.DocumentClass();
        }

        /// <summary>
        /// 导出部分表结构到WORD文档
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tblnamelist"></param>
        public void SetTableSchemaSomeToWord(string filename, List<string> tblnamelist)
        {
            TabStruCollection tsc = new TabStruCollection();
            TableSchemaInOut tsio = new TableSchemaInOut(this.m_handler);
            tsc = tsio.getTabStruSomeCollection(tblnamelist);
            this.ConvertInfoToWord(filename, tsc);
        }

        /// <summary>
        /// 导出全部表结构到WORD文档
        /// </summary>
        /// <param name="filename"></param>
        public void SetTableSchemaToWord(string filename)
        {
            TabStruCollection tsc = new TabStruCollection();
            TableSchemaInOut tsio = new TableSchemaInOut(this.m_handler);
            tsc = tsio.getTabStruAllCollection();
            this.ConvertInfoToWord(filename, tsc);
        }

        /// <summary>
        /// 转化表数据集合为Word文档
        /// </summary>
        /// <param name="filewordPath"></param>
        /// <param name="tsc"></param>
        public void ConvertInfoToWord(string filewordPath, TabStruCollection tsc)
        {
            if (tsc.List == null || tsc.List.Count == 0)
                return;
            StringBuilder rtn = new StringBuilder();
            object wdtype = WdNewDocumentType.wdNewBlankDocument;
            this.m_wddoc = this.m_wdapp.Documents.Add(ref nouseref, ref nouseref, ref wdtype,
                ref nouseref);

            try
            {
                foreach (Draco.DB.QuickDataBase.Schema.Vendor.Implementation.Table table in tsc.List)
                {
                    Draco.DB.QuickDataBase.Schema.Vendor.Implementation.Table tbl = table;
                    Microsoft.Office.Interop.Word.Range range = this.m_wdapp.Selection.Range;
                    object tblbehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object fitbehavite = WdAutoFitBehavior.wdAutoFitFixed;
                    Microsoft.Office.Interop.Word.Table wdtbl = this.m_wddoc.Tables.Add(range, tbl.Columns.Count + 3, 4,
                        ref tblbehavior,
                        ref fitbehavite);

                    MergeTblCell(wdtbl, 1, 2, 4);
                    Microsoft.Office.Interop.Word.Cell cell = wdtbl.Cell(1, 1);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "表名：");

                    SetCellValue(wdtbl.Cell(1, 2), tbl.TableName);
                    MergeTblCell(wdtbl, 2, 2, 4);
                    cell = wdtbl.Cell(2, 1);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "表名含义：");
                    SetCellValue(wdtbl.Cell(2, 2), tbl.TableName);

                    cell = wdtbl.Cell(3, 1);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "字段名称");
                    cell = wdtbl.Cell(3, 2);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "字段类型（长度）");
                    cell = wdtbl.Cell(3, 3);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "字段含义");
                    cell = wdtbl.Cell(3, 4);
                    SetTblCellShading(cell);
                    SetCellValue(cell, "备注");

                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        SetCellValue(wdtbl.Cell(i + 4, 1), tbl.Columns[i].ColumnName);
                        string typeAndLen = tbl.Columns[i].FullType.ToString();
                        if (tbl.Columns[i].Length != null && tbl.Columns[i].Length.ToString().Length > 0)
                            typeAndLen += "(" + tbl.Columns[i].Length + ")";
                        SetCellValue(wdtbl.Cell(i + 4, 2), typeAndLen);
                        SetCellValue(wdtbl.Cell(i + 4, 3), tbl.Columns[i].Comment);
                        string fieldAtt = "";
                        if (tbl.Columns[i].PrimaryKey != null && (bool)tbl.Columns[i].PrimaryKey)
                            fieldAtt += "主键;";
                        else
                        {
                            if (!tbl.Columns[i].Nullable)
                                fieldAtt += "非空;";
                            if (tbl.Columns[i].Generated != null && (bool)tbl.Columns[i].Generated)
                                fieldAtt += "索引;";
                        }

                        if (tbl.Columns[i].Generated != null && (bool)tbl.Columns[i].Generated)
                            fieldAtt += "自增;";

                        if (tbl.Columns[i].Generated != null && (bool)tbl.Columns[i].Generated)
                            fieldAtt += "唯一;";

                        if (tbl.Columns[i].DefaultValue != null && tbl.Columns[i].DefaultValue.Trim().Length > 0)
                            fieldAtt += "默认=" + tbl.Columns[i].DefaultValue + ";";

                        SetCellValue(wdtbl.Cell(i + 4, 4), fieldAtt);
                    }
                    object unit = Microsoft.Office.Interop.Word.WdUnits.wdCharacter;
                    object count = 1;
                    wdtbl.Select();
                    this.m_wdapp.Selection.MoveRight(ref unit, ref count, ref nouseref);

                    object br = Microsoft.Office.Interop.Word.WdBreakType.wdLineBreak;
                    this.m_wdapp.Selection.InsertBreak(ref br);
                }
                SaveDoc(filewordPath);
            }
            catch (Exception ex)
            {
                //释放资源
                if (this.m_wdapp != null)
                {
                    m_wdapp.Quit(ref nouseref, ref nouseref, ref nouseref);
                }
                throw ex;
            }
        }
        /// <summary>
        /// 合并同行中的单元格
        /// </summary>
        /// <param name="wdtbl"></param>
        /// <param name="rownum"></param>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        private void MergeTblCell(Microsoft.Office.Interop.Word.Table wdtbl, int rownum, int col1, int col2)
        {
            wdtbl.Rows[rownum].Cells[col1].Merge(wdtbl.Cell(rownum, col2));
        }
        /// <summary>
        /// 设置单元格背景
        /// </summary>
        /// <param name="cell"></param>
        private void SetTblCellShading(Microsoft.Office.Interop.Word.Cell cell)
        {
            cell.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorGray25;
        }
        /// <summary>
        /// 设置某单元格内的数据
        /// </summary>
        /// <param name="cell">要读取的Cell</param>
        /// <param name="val">值</param>
        /// <returns>单元格内的数据</returns>
        private void SetCellValue(Microsoft.Office.Interop.Word.Cell cell, string val)
        {
            object cellstart = cell.Range.Start;
            object cellend = cell.Range.End;

            this.m_wddoc.Range(ref cellstart, ref cellend).Text = val;
        }

        /// <summary>
        /// 保存Word
        /// </summary>
        /// <param name="wordPath"></param>
        public void SaveDoc(string wordPath)
        {
            object path = wordPath;
            this.m_wddoc.SaveAs(ref path, ref nouseref, ref nouseref, ref nouseref, ref nouseref,
                ref nouseref, ref nouseref, ref nouseref, ref nouseref, ref nouseref, ref nouseref, ref nouseref, ref nouseref,
                ref nouseref, ref nouseref, ref nouseref);
            //释放资源
            if (this.m_wdapp != null)
            {
                m_wdapp.Quit(ref nouseref, ref nouseref, ref nouseref);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.m_wdapp != null)
            {
                m_wdapp.Quit(ref nouseref, ref nouseref, ref nouseref);
            }
        }
    }
}
