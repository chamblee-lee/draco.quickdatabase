using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Draco.DB.ORM;
using Draco.DB.ORM.Generator;
using Draco.DB.ORM.Schema.Dbml;
using Draco.DB.ORM.Schema.Vendor;
using Draco.DB.QuickDataBase;
using Draco.DB.QuickDataBase.Common;
using Draco.DB.QuickDataBase.Configuration;
using Draco.DB.QuickDataBase.Schema;
using Draco.DB.QuickDataBase.Schema.Vendor.Implementation;


namespace CodeGenerator
{
    public partial class MainDevForm1 : XtraForm
    {
        ORMContext m_ctx = null;
        private Boolean comflag = false;//控制数据库连接类型下拉框的切换标记

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainDevForm1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载窗口
        /// </summary>
        private void MainDevForm1_Load(object sender, EventArgs e)
        {
            this.Activate();

            //禁止修改的cmb
            this.ComBoxDBTYpe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.DbTypeEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            
            //设置皮肤
            ConnectionConfig c1 = new ConnectionConfig();
            this.defaultLookAndFeel1.LookAndFeel.SkinName = c1.LoadTheme("TemConfig.xml");
            //this.barCheckItem3.Checked = true;

            //读取使用过的数据库配置
            comflag = true;
            ConnectionInfo c_info = null;//数据库连接信息
            ConnectionConfig cc = new ConnectionConfig();
            c_info = cc.LoadFromFile("TemConfig.xml");//保存数据库连接的临时文件
            if (c_info == null)
            {
                this.TxtConnect.Text = "Server = (local); User id = sa; Pwd = sa; Database = msdb;";
                this.DbConectStrEdit.Text = "Server = (local); User id = sa; Pwd = sa; Database = msdb;";
                this.ComBoxDBTYpe.SelectedIndex = 0;
                this.DbTypeEdit.SelectedIndex = 0;
            }
            else
            {
                this.TxtConnect.Text = c_info.ConnectionString;
                this.DbConectStrEdit.Text = c_info.ConnectionString;
                this.ComBoxDBTYpe.SelectedIndex = Convert.ToInt32(c_info.DataServerType);
                this.DbTypeEdit.SelectedIndex = Convert.ToInt32(c_info.DataServerType);

            }
            comflag = false;

            this.TxtContext.Text = cc.LoadContext("TemConfig.xml");
            this.TxtNameSpace.Text = cc.LoadNamespace("TemConfig.xml");
            this.combPKGenerator.Text = cc.LoadSpkgenerator("TemConfig.xml");
            string isdao = cc.LoadIsdao("TemConfig.xml");
            if (isdao.Equals("1"))
            {
                this.radioGroup3.SelectedIndex = 0;
            }
            else
            {
                this.radioGroup3.SelectedIndex = 1;
            }
            this.FilePath.Text = cc.LoadOutputpath("TemConfig.xml");

            //设置未连接状态
            toLinkStateOff();
        }



        /// <summary>
        /// 设置实体生成工具界面为已连接状态
        /// </summary>
        private void toLinkStateOk()
        {
            this.groupControl1.Enabled = false;
            this.groupControl2.Enabled = true;

            this.BtnXml.Enabled = true;
            this.BtnUnSelect.Enabled = true;
            this.BtnSelectAll.Enabled = true;
            this.BtnOpSelect.Enabled = true;
            this.OffConBtn.Enabled = true;
            this.BtnCS.Enabled = true;
            this.CheckBoxTables.Enabled = true;
            this.labelControl1.Enabled = true;
        }

        /// <summary>
        /// 设置实体生成工具界面为未连接状态
        /// </summary>
        private void toLinkStateOff()
        {
            this.groupControl1.Enabled = true;
            this.groupControl2.Enabled = false;

            this.BtnXml.Enabled = false;
            this.BtnUnSelect.Enabled = false;
            this.BtnSelectAll.Enabled = false;
            this.BtnOpSelect.Enabled = false;
            this.OffConBtn.Enabled = false;
            this.BtnCS.Enabled = false;
            this.CheckBoxTables.Enabled = false;
            this.labelControl1.Enabled = false;
        }

        /// <summary>
        /// 修改数据库类型选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComBoxDBTYpe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comflag)//判断外部开关标记
                return;
            switch (ComBoxDBTYpe.SelectedIndex)
            {
                case 0://SqlServer
                    {
                        this.TxtConnect.Text = "Server=(local);User id=sa;Pwd=sa;Database=Draco_x;";
                        break;
                    }

                case 1://Oracle
                    {
                        this.TxtConnect.Text = "User ID=sa;Password=sa;Data Source=ORA_local;";
                        break;
                    }
                case 3://OleDB
                    {
                        this.TxtConnect.Text = "Provider=SQLOLEDB; Data Source=(local); Initial Catalog=Dracox; User ID=sa; Password=sa;";
                        break;
                    }

                case 2://SQLite
                    {
                        OpenFileDialog fd = new OpenFileDialog();
                        fd.FileName = "";
                        fd.Title = "请选择SQLite3文件";
                        fd.Filter = "SQLite3文件|*.s3db|所有文件|*.*";
                        fd.ShowDialog();
                        this.TxtConnect.Text = "data source=" + fd.FileName + ";";
                        break;
                    }
                case 4://Custom(自定义)
                    {
                        this.TxtConnect.Text = "请输入自定义数据库连接字符串";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 查看示例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowExampleBtn_Click(object sender, EventArgs e)
        {
            if (this.ComBoxDBTYpe.SelectedIndex == -1)
            {
                MessageBox.Show("请选择数据库连接类型！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                ShowDbCase cksl = new ShowDbCase(this.ComBoxDBTYpe.SelectedIndex);
                cksl.Show();
            }
        }

        /// <summary>
        /// 加载表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLink_Click(object sender, EventArgs e)
        {
            /*
            if (this.radioGroup1.SelectedIndex == 0)//默认配置连接
            {
                try
                {
                    ConnectionInfo info = new ConnectionInfoLoader().LoadFromAppSetting();
                    m_ctx = new ORMContext(info);

                    ISchemaLoader Loader = m_ctx.SchemaLoader;
                    Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer");
                    this.CheckBoxTables.Items.Clear();

                    if (dataBase.Tables.Count == 0)
                    {
                        MessageBox.Show("很抱歉，您所选的数据库中没有任何数据表！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        foreach (var table in dataBase.Table)
                        {
                            this.CheckBoxTables.Items.Add(table.Name);
                        }

                        //连接成功后
                        MessageBox.Show("数据库连接成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.TxtContext.Text = "ContextName";
                        this.TxtNameSpace.Text = "Draco.DataTblCtrlLayer";

                        toLinkStateOk();
                    }
                }
                catch (Exception ex)
                {
                    if (DialogResult.OK == MessageBox.Show("对不起，未能正确连接数据库！\n需要显示错误的更多信息么？", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        ShowErrorInf sei = new ShowErrorInf(ex.ToString());
                        sei.Show();
                    }
                }
            }
            */
            //自定义连接else

            if (this.TxtConnect.Text.Length <= 1 || this.ComBoxDBTYpe.SelectedItem.ToString().Length <= 1)
            {
                MessageBox.Show("请输入正确的数据库连接信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!this.TestDbConect(this.TxtConnect.Text, this.ComBoxDBTYpe.SelectedItem.ToString()))  
                return;


            ConnectionInfo info = new ConnectionInfo(this.TxtConnect.Text, this.ComBoxDBTYpe.SelectedItem.ToString());
            m_ctx = new ORMContext(info);

            try
            {
                this.Enabled = false;
                ISchemaLoader Loader = m_ctx.SchemaLoader;
                Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer");
                this.CheckBoxTables.Items.Clear();

                if (dataBase.Tables.Count == 0)
                {
                    MessageBox.Show("很抱歉，您所选的数据库中没有任何数据表信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //return;
                }
                else
                {
                    //加入排序
                    SortedList sl = new SortedList();
                    foreach (var table in dataBase.Table)
                    {
                        sl.Add(table.Name, table.Name);
                        //this.CheckBoxTables.Items.Add(table.Name);
                    }
                    int length = sl.Count;
                    this.CheckBoxTables.Items.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        this.CheckBoxTables.Items.Add(sl.GetByIndex(i).ToString());
                    }

                    //连接成功后  
                    MessageBox.Show("数据库连接成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //保存到临时数据库配置信息中以便下次使用
                    ConnectionConfig cc = new ConnectionConfig();
                    cc.SaveToXml(this.TxtConnect.Text, this.ComBoxDBTYpe.SelectedIndex, Application.StartupPath + @"\TemConfig.xml");

                    //this.TxtContext.Text = "ContextName";
                    //this.TxtNameSpace.Text = "Draco.DataTblCtrlLayer";

                    toLinkStateOk();
                }
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，未能正确连接数据库，请重新检查数据库连接设置！");
                this.Enabled = true;

                this.TxtContext.Text = "";
                this.TxtNameSpace.Text = "";
                this.CheckBoxTables.Items.Clear();

                toLinkStateOff();
            }

        }

        /// <summary>
        /// 选择CS文件输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectPathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            DialogResult r = Dialog.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                this.FilePath.Text = Dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 选择所有表—实体工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.CheckBoxTables.Items.Count; i++)
            {
                CheckBoxTables.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 取消选择所有表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUnSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.CheckBoxTables.Items.Count; i++)
            {
                CheckBoxTables.SetItemChecked(i, false);
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OffConBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("确认断开数据库的连接吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                //this.TxtContext.Text = "";
                //this.TxtNameSpace.Text = "";
                this.CheckBoxTables.Items.Clear();

                toLinkStateOff();
                MessageBox.Show("数据库已断开，请重新连接数据库！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// 生成XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXml_Click(object sender, EventArgs e)
        {
            if (!this.checkXmlInf())
            {
                return;
            }

            string tcontext = this.TxtContext.Text;
            string tnamespace = this.TxtNameSpace.Text;

            try
            {
                ISchemaLoader Loader = m_ctx.SchemaLoader;
                Database dataBase = Loader.Load(tcontext, tnamespace, GetSelectedTableNames);

                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.Filter = "XML文件|*.xml|所有文件|*.*";
                Dialog.DefaultExt = ".xml";
                DialogResult r = Dialog.ShowDialog(this);
                if (r == DialogResult.OK)
                {
                    string FileName = Dialog.FileName;
                    using (Stream dbmlFile = File.OpenWrite(FileName))
                    {
                        DbmlSerializer.Write(dbmlFile, dataBase);
                    }
                    MessageBox.Show("XML文件输出已经完成！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ConnectionConfig cc = new ConnectionConfig();
                    string isdao = "1";
                    if (this.radioGroup3.SelectedIndex == 1)
                    {
                        isdao = "0";
                    }
                    cc.SaveToXml2(this.TxtContext.Text, this.TxtNameSpace.Text, this.combPKGenerator.Text, isdao, this.FilePath.Text, "TemConfig.xml");
                }
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，生成XML文件出错！");
            }
        }

        /// <summary>
        /// 生成CS源文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCS_Click(object sender, EventArgs e)
        {
            if (!checkCsInf())
            {
                return;
            }
            try
            {
                if (DialogResult.Yes == MessageBox.Show("确认生成CS文件吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    //获取输出设置
                    string fp = FilePath.Text;
                    bool dodao = (this.radioGroup3.SelectedIndex == 0) ? true : false;
                    string spkgenerator = this.combPKGenerator.Text;
                    string tcontext = this.TxtContext.Text;
                    string tnamespace = this.TxtNameSpace.Text;

                    ISchemaLoader Loader = m_ctx.SchemaLoader;
                    Database dataBase = Loader.Load(tcontext, tnamespace, GetSelectedTableNames);

                    EntityCodeGenerator Generator = new EntityCodeGenerator(dataBase);
                    foreach (var table in dataBase.Tables)
                    {
                        string f = Draco.DB.ORM.Utility.CharacterHelper.ConvertToLetter(table.Name);
                        f = f.ToUpper();
                        string fileName = Path.Combine(fp, f + ".cs");
                        string Code = Generator.GenerateCode(table.Name, spkgenerator, dodao);
                        File.WriteAllText(fileName, Code, System.Text.Encoding.UTF8);
                    }
                    MessageBox.Show("CS文件输出已经完成！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ConnectionConfig cc = new ConnectionConfig();
                    string isdao = "1";
                    if (this.radioGroup3.SelectedIndex == 1)
                    {
                        isdao = "0";
                    }
                    cc.SaveToXml2(this.TxtContext.Text, this.TxtNameSpace.Text, this.combPKGenerator.Text, isdao, this.FilePath.Text, "TemConfig.xml");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，生成CS文件出错！");
            }
        }

        /// <summary>
        /// 检测生成cs代码的输入信息的错误
        /// </summary>
        private Boolean checkCsInf()
        {
            if (CheckBoxTables.CheckedItems.Count == 0)
            {
                MessageBox.Show("请选择有效的数据表！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.FilePath.Text.Length <= 1)
            {
                MessageBox.Show("请选择文件输出目录！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            /*
            if (this.TxtContext.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的Context信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            */
            if (this.TxtNameSpace.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的NameSpace信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.combPKGenerator.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的主键生成器信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检测生成xml的输入信息的错误
        /// </summary>
        private Boolean checkXmlInf()
        {
            /*
            if (CheckBoxTables.CheckedItems.Count == 0)
            {
                MessageBox.Show("请选择有效的数据表！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (this.FilePath.Text.Length <= 1)
            {
                MessageBox.Show("请选择文件输出目录！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            */
            if (this.TxtContext.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的Context信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.TxtNameSpace.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的NameSpace信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.combPKGenerator.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的主键生成器信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检测生成cs2代码的输入信息的错误
        /// </summary>
        private Boolean checkCs2Inf()
        {

            if (CheckBoxTables.CheckedItems.Count == 0)
            {
                MessageBox.Show("请选择有效的数据表！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (this.FilePath.Text.Length <= 1)
            {
                MessageBox.Show("请选择文件输出目录！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            /*
            if (this.TxtContext.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的Context信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            */
            if (this.TxtNameSpace.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的NameSpace信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (this.combPKGenerator.Text.Length <= 1)
            {
                MessageBox.Show("请输入正确的主键生成器信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        private void showTip(object sender, MouseEventArgs e)
        {
            //命名空间
            if (sender.Equals(this.TxtNameSpace))
            {
                this.barHelpTip.Caption = "生成实体所属的命名空间";
            }

            //Context
            if (sender.Equals(this.TxtContext))
            {
                this.barHelpTip.Caption = "设置输出XML文件的Context属性";
            }

            //目标文件夹
            if (sender.Equals(this.FilePath))
            {
                this.barHelpTip.Caption = "生成CS文件所在的目标文件夹";
            }

            //数据库连接类型
            if (sender.Equals(this.ComBoxDBTYpe) || sender.Equals(this.DbTypeEdit))
            {
                this.barHelpTip.Caption = "所要连接的数据库的种类";
            }

            //连接字符
            if (sender.Equals(this.TxtConnect) || sender.Equals(this.DbConectStrEdit))
            {
                this.barHelpTip.Caption = "数据库连接字符串，查看连接示例可给您更多帮助";
            }

            //数据表排列
            if (sender.Equals(this.CheckBoxTables) || sender.Equals(this.checkedListBoxControl1))
            {
                this.barHelpTip.Caption = "数据库中的存在的数据表";
            }

            //生成XML
            if (sender.Equals(this.BtnXml))
            {
                this.barHelpTip.Caption = "生成数据表列中全部数据表的XML文档";
            }

            //生成CS文件
            if (sender.Equals(this.BtnCS))
            {
                this.barHelpTip.Caption = "根据设置生成您所需要的CS文件";
            }

            //断开连接
            if (sender.Equals(this.OffConBtn) || sender.Equals(this.OffLinkBtn))
            {
                this.barHelpTip.Caption = "断开后可以连接到其他数据库";
            }

            //显示事例
            if (sender.Equals(this.ShowExampleBtn) || sender.Equals(this.ShowLinkExampleBtn))
            {
                this.barHelpTip.Caption = "提供常见的数据库连接字符串";
            }

            //连接数据库
            if (sender.Equals(this.BtnLink) || sender.Equals(this.LinkDbBtn))
            {
                this.barHelpTip.Caption = "根据设置连接数据库";
            }

            //全选
            if (sender.Equals(this.BtnSelectAll) || sender.Equals(this.AllSelectBtn))
            {
                this.barHelpTip.Caption = "选中全部的数据表";
            }

            //清空
            if (sender.Equals(this.BtnUnSelect) || sender.Equals(this.UnSelectAllBtn))
            {
                this.barHelpTip.Caption = "取消选择所有的数据表";
            }

            //反向选择
            if (sender.Equals(this.OppSelectBtn) || sender.Equals(this.BtnOpSelect))
            {
                this.barHelpTip.Caption = "反向选择数据表";
            }

            //选择路径
            if (sender.Equals(this.SelectPathBtn))
            {
                this.barHelpTip.Caption = "设置输出CS文件的路径";
            }

            //是否生成dao
            if (sender.Equals(this.radioGroup3))
            {
                this.barHelpTip.Caption = "设置是否生成DAO";
            }

            //主键生成器的设置
            if (sender.Equals(this.combPKGenerator))
            {
                this.barHelpTip.Caption = "设置使用的主键生成器";
            }

            //测试连接
            if (sender.Equals(this.ConnectTestBtn))
            {
                this.barHelpTip.Caption = "测试设置的数据库连接是否正确";
            }

            //刷新连接
            if (sender.Equals(this.RefreshBtn))
            {
                this.barHelpTip.Caption = "刷新当前连接的表数据";
            }

            //导入表结构
            if (sender.Equals(this.ImportSchBtn))
            {
                this.barHelpTip.Caption = "从外部文件向数据库中导入新的表结构";
            }

            //导出表结构
            if (sender.Equals(this.ExportSchBtn))
            {
                this.barHelpTip.Caption = "将选择的数据表结构导出保存至外部文件";
            }

            //协调表结构
            if (sender.Equals(this.FisTableSchBtn))
            {
                this.barHelpTip.Caption = "从外部导入表结构并协调现有的数据表";
            }

            //导出word
            if (sender.Equals(this.ToWordBtn))
            {
                this.barHelpTip.Caption = "输出所选择表的表字段说明（word文档形式）";
            }

            //导入表数据
            if (sender.Equals(this.ImportDataBtn))
            {
                this.barHelpTip.Caption = "从外部文件中导入表数据";
            }

            //导出表数据
            if (sender.Equals(this.ExportDataBtn))
            {
                this.barHelpTip.Caption = "导出当前所选择表数据到外部文件中";
            }

            //导出sql语句
            if (sender.Equals(this.ExportSqlBtn))
            {
                this.barHelpTip.Caption = "导出当前所选择表的创建表sql语句到文件中";
            }

            //执行sql语句
            if (sender.Equals(this.ExeSqlFileBtn))
            {
                this.barHelpTip.Caption = "执行外部sql语句文件";
            }

            //协调主键值
            if (sender.Equals(this.MachIdBtn))
            {
                this.barHelpTip.Caption = "协调Oracle数据库序列值";
            }
        }

        /// <summary>
        /// 取消显示提示信息
        /// </summary>
        private void offTips(object sender, EventArgs e)
        {
            this.barHelpTip.Caption = "欢迎使用";
        }

        /// <summary>
        /// 返回选择的数据表_实体生成工具信息
        /// </summary>
        private string[] GetSelectedTableNames
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (var item in this.CheckBoxTables.CheckedItems)
                {
                    string s = item.ToString();
                    list.Add(s);
                    //list.Add(item);
                }
                return list.ToArray(typeof(string)) as string[];
            }
        }

        /// <summary>
        /// 返回选择的数据表_数据库维护信息
        /// </summary>
        private List<string> GetSelectedTableNames2
        {
            get
            {
                List<string> tblnamelist = new List<string>();
                foreach (var item in this.checkedListBoxControl1.CheckedItems)
                {
                    string s = item.ToString();
                    tblnamelist.Add(s);
                }
                return tblnamelist;
            }
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonExit(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 更换皮肤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Boolean cflag = true;//锁
        private void barCheckItem1_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!cflag)
            {
                return;
            }

            this.cflag = false;//解锁
            DevExpress.XtraBars.BarCheckItem bci = (DevExpress.XtraBars.BarCheckItem)sender;
            this.defaultLookAndFeel1.LookAndFeel.SkinName = bci.Caption;

            this.barCheckItem1.Checked = false;
            this.barCheckItem2.Checked = false;
            this.barCheckItem3.Checked = false;
            this.barCheckItem4.Checked = false;
            this.barCheckItem5.Checked = false;
            this.barCheckItem6.Checked = false;
            this.barCheckItem7.Checked = false;
            this.barCheckItem8.Checked = false;

            bci.Checked = true;

            this.cflag = true;//开锁

            //保存皮肤设置
            ConnectionConfig c1 = new ConnectionConfig();
            c1.SaveToXml3(bci.Caption, Application.StartupPath + @"\TemConfig.xml");
        }

        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.CheckBoxTables.Items.Count; i++)
            {
                if (this.CheckBoxTables.GetItemChecked(i))
                    CheckBoxTables.SetItemChecked(i, false);
                else
                    CheckBoxTables.SetItemChecked(i, true);

            }
        }

        /// <summary>
        /// 执行sql语句文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "SQL文件|*.sql|所有文件|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string filename = openFileDialog1.FileName;

            if (filename == null || filename.Length <= 0 || !File.Exists(filename))
                return;

            try
            {
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                ExecuteSqlFile esc = new ExecuteSqlFile(idb);
                int count = Math.Abs(esc.ExecuteFileSql(filename));
                MessageBox.Show("成功执行了" + count + "条语句", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，执行sql语句时出错！");
            }
        }

        /// <summary>
        /// 导入表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportDataBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML文件|*.xml|所有文件|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string filename = openFileDialog1.FileName;

            if (filename == null || filename.Length <= 0 || !File.Exists(filename))
                return;
            try
            {
                this.Enabled = false;
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableDataInOut tdo = new TableDataInOut(idb);
                tdo.LoadDataFromXml(filename);
                MessageBox.Show("恭喜您！导入表数据成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，导入表数据出错！");
                this.Enabled = true;
            }
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectTestBtn_Click(object sender, EventArgs e)
        {
            if (this.TestDbConect(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString()))
            {
                MessageBox.Show("恭喜您！数据库设置正确！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        private bool TestDbConect(string cstring, string ctype)
        {
            if (cstring == null || ctype == null)
            {
                return false;
            }
            if (cstring.Length <= 1 || ctype.Length <= 1)
            {
                MessageBox.Show("请输入正确的数据库连接信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                String Provider = ""; 
                if (ctype.ToUpper().Equals("SQLSERVER"))
                {
                    Provider = "System.Data.SqlClient";
                }
                else if (ctype.ToUpper().Equals("ORACLE"))
                {
                    Provider = "System.Data.OracleClient";
                }
                else if (ctype.ToUpper().Equals("SQLITE"))
                {
                    Provider = "System.Data.SQLite";
                }
                else if (ctype.ToUpper().Equals("SQLSERVERCE"))
                {
#if NET2_0
                    Provider = "System.Data.SqlServerCe.3.5";
#elif NET4_0
                    Provider = "System.Data.SqlServerCe.4.0";
#endif
                }
                if (!String.IsNullOrEmpty(Provider))
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(Provider);
                    using (DbConnection conn = factory.CreateConnection())
                    {
                        conn.ConnectionString = cstring;
                        conn.Open();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，当前的数据库配置存在问题！");
                return false;
            }
        }

        /// <summary>
        /// 数据库维护工具的数据库类型更改的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DbTypeEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comflag)//判断外部开关标记
                return;
            switch (this.DbTypeEdit.SelectedIndex)
            {
                case 0://SqlServer
                    {
                        this.DbConectStrEdit.Text = "Server=(local);User id=sa;Pwd=sa;Database=Dracox;";
                        break;
                    }

                case 1://Oracle
                    {
                        this.DbConectStrEdit.Text = "User ID=sa;Password=sa;Data Source=ORA_local;";
                        break;
                    }
                case 2://SQLite
                    {
                        OpenFileDialog fd = new OpenFileDialog();
                        fd.FileName = "";
                        fd.Title = "请选择SQLite3文件";
                        fd.Filter = "SQLite3文件|*.s3db|所有文件|*.*";
                        fd.ShowDialog();
                        this.DbConectStrEdit.Text = "data source=" + fd.FileName + ";";
                        break;
                    }
                case 3://SQLSERVERCE
                    {
                        OpenFileDialog fd = new OpenFileDialog();
                        fd.FileName = "";
                        fd.Title = "请选择sdf数据库文件";
                        fd.Filter = "SQLSERVERCE文件|*.sdf|所有文件|*.*";
                        fd.ShowDialog();
                        
                        this.DbConectStrEdit.Text = "data source=" + fd.FileName + ";";
                        break;
                    }
                case 4://Custom(自定义)
                    {
                        this.DbConectStrEdit.Text = "请输入自定义数据库连接字符串";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 示例显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (this.DbTypeEdit.SelectedIndex == -1)
            {
                MessageBox.Show("请选择数据库连接类型！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                ShowDbCase cksl = new ShowDbCase(this.DbTypeEdit.SelectedIndex);
                cksl.Show();
            }
        }

        /// <summary>
        /// 维护—连接数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.DbConectStrEdit.Text.Length <= 1 || this.DbTypeEdit.SelectedItem.ToString().Length <= 1)
            {
                MessageBox.Show("请输入正确的数据库连接信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ConnectionInfo info = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
            m_ctx = new ORMContext(info);

            try
            {
                this.Enabled = false;
                ISchemaLoader Loader = m_ctx.SchemaLoader;

                Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer");
                this.checkedListBoxControl1.Items.Clear();

                if (dataBase.Tables.Count == 0)
                {
                    MessageBox.Show("连接成功，但您所选的数据库中没有任何数据表信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //连接成功后 
                    //保存到临时数据库配置信息中以便下次使用
                    ConnectionConfig cc = new ConnectionConfig();
                    cc.SaveToXml(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedIndex, Application.StartupPath + @"\TemConfig.xml");

                    toLinkStateOk2();
                    //return;
                }
                else
                {
                    //加入排序
                    SortedList sl = new SortedList();
                    foreach (var table in dataBase.Table)
                    {
                        sl.Add(table.Name, table.Name);
                    }
                    int length = sl.Count;
                    this.checkedListBoxControl1.Items.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        this.checkedListBoxControl1.Items.Add(sl.GetByIndex(i).ToString());
                    }

                    //连接成功后  
                    MessageBox.Show("数据库连接成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //保存到临时数据库配置信息中以便下次使用
                    ConnectionConfig cc = new ConnectionConfig();
                    cc.SaveToXml(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedIndex, Application.StartupPath + @"\TemConfig.xml");

                    toLinkStateOk2();

                    if (this.DbTypeEdit.SelectedIndex != 1)
                        this.MachIdBtn.Enabled = false;
                    else
                        this.MachIdBtn.Enabled = true;
                }
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，未能正确连接数据库，请重新检查数据库连接设置！");
                this.Enabled = true;
                this.checkedListBoxControl1.Items.Clear();
                toLinkStateOff2();
            }

        }

        /// <summary>
        /// 切换至连接状态_数据库维护
        /// </summary>
        private void toLinkStateOk2()
        {
            this.groupControl3.Enabled = false;
            this.groupControl7.Enabled = true;

            this.checkedListBoxControl1.Enabled = true;

            this.AllSelectBtn.Enabled = true;
            this.UnSelectAllBtn.Enabled = true;
            this.OppSelectBtn.Enabled = true;
            this.RefreshBtn.Enabled = true;
            this.OffLinkBtn.Enabled = true;

            this.labelControl11.Enabled = true;
        }
        /// <summary>
        /// 切换至未连接状态_数据库维护
        /// </summary>
        private void toLinkStateOff2()
        {
            this.groupControl3.Enabled = true;
            this.groupControl7.Enabled = false;

            this.checkedListBoxControl1.Enabled = false;

            this.AllSelectBtn.Enabled = false;
            this.UnSelectAllBtn.Enabled = false;
            this.OppSelectBtn.Enabled = false;
            this.RefreshBtn.Enabled = false;
            this.OffLinkBtn.Enabled = false;

            this.labelControl11.Enabled = false;
        }

        /// <summary>
        /// 导入表结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportSchBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "TBLST文件|*.tblst|所有文件|*.*";
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string filename = openFileDialog1.FileName;

                if (filename == null || filename.Length <= 0 || !File.Exists(filename))
                    return;
                this.Enabled = false;
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableSchemaInOut tsio = new TableSchemaInOut(idb);
                tsio.tableSchemaInput2(filename);
                MessageBox.Show("恭喜，导入表结构成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Enabled = true;
                //return;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，导入表结构失败！");
                this.Enabled = true;
            }
        }

        /// <summary>
        /// 导出创建表sql语句
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSqlBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkedListBoxControl1.CheckedItemsCount <= 0)
                {
                    MessageBox.Show("请选择需要导出的数据表", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;
                TableSchemaInOut tsio = new TableSchemaInOut(idb);

                TabStruCollection tsc = tsio.getTabStruSomeCollection(this.GetSelectedTableNames2);
                string sql = tsc.GetFixTableStuSqlByFile2(idb);

                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.Filter = "SQL文件|*.sql";
                Dialog.DefaultExt = ".sql";
                DialogResult r = Dialog.ShowDialog(this);
                if (r == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(Dialog.FileName);
                    sw.Write(sql);
                    sw.Dispose();
                    MessageBox.Show("导出sql语句成功", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，导出sql语句失败！");
            }
        }

        /// <summary>
        /// 导出表结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSchBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkedListBoxControl1.CheckedItemsCount <= 0)
                {
                    MessageBox.Show("请选择需要导出的数据表", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "TBLST文件|*.tblst|所有文件|*.*";
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string filename = saveFileDialog1.FileName;

                if (filename == null || filename.Length <= 0)
                    return;

                if (!File.Exists(filename))
                    File.Create(filename).Close();
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableSchemaInOut tsio = new TableSchemaInOut(idb);
                tsio.tableSchemaSomeOutput(filename, this.GetSelectedTableNames2);
                MessageBox.Show("恭喜您！成功导出 " + GetSelectedTableNames2.Count + " 张表结构！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，导出表结构失败！");

            }
        }

        /// <summary>
        /// 导出表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkedListBoxControl1.CheckedItemsCount <= 0)
                {
                    MessageBox.Show("请选择需要导出的数据表", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "XML文件|*.xml|所有文件|*.*";
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string filename = saveFileDialog1.FileName;

                if (filename == null || filename.Length <= 0)
                    return;

                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();//需要解除占用
                    /*
                    using (FileStream fs = File.Create(filename))
                    {}
                    */
                }
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableDataInOut tdo = new TableDataInOut(idb);
                tdo.SaveSomeTableDataToXml(filename, this.GetSelectedTableNames2);
                MessageBox.Show("成功导出 " + GetSelectedTableNames2.Count + " 张表数据", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，导出表数据失败！");
            }
        }

        /// <summary>
        /// 导出word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToWordBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkedListBoxControl1.CheckedItemsCount <= 0)
                {
                    MessageBox.Show("请选择需要导出的数据表", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "word文档|*.doc|所有文件|*.*";
                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string filename = saveFileDialog1.FileName;
                if (filename == null || filename.Length <= 0)
                    return;
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();//需要解除占用
                }

                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableSchemaToWord tstw = new TableSchemaToWord(idb);
                tstw.SetTableSchemaSomeToWord(filename, this.GetSelectedTableNames2);
                MessageBox.Show("成功导出 " + GetSelectedTableNames2.Count + " 个表的字段说明信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowExceptionInfo(ex, "对不起，导出word文件失败！");
            }
        }

        /// <summary>
        /// 数据表全选—数据库维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBoxControl1.Items.Count; i++)
            {
                checkedListBoxControl1.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 清空选择数据表_数据库维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBoxControl1.Items.Count; i++)
            {
                checkedListBoxControl1.SetItemChecked(i, false);
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBoxControl1.Items.Count; i++)
            {
                if (this.checkedListBoxControl1.GetItemChecked(i))
                    checkedListBoxControl1.SetItemChecked(i, false);
                else
                    checkedListBoxControl1.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// 刷新连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            ConnectionInfo info = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
            m_ctx = new ORMContext(info);

            try
            {
                this.Enabled = false;
                ISchemaLoader Loader = m_ctx.SchemaLoader;
                Database dataBase = Loader.Load("Name", "Draco.DataTblCtrlLayer");
                this.checkedListBoxControl1.Items.Clear();

                if (dataBase.Tables.Count == 0)
                {
                    MessageBox.Show("连接成功，但您所选的数据库中没有任何数据表信息！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //连接成功后 
                    //保存到临时数据库配置信息中以便下次使用
                    ConnectionConfig cc = new ConnectionConfig();
                    cc.SaveToXml(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedIndex, "TemConfig.xml");

                    toLinkStateOk2();
                    //return;
                }
                else
                {
                    //加入排序
                    SortedList sl = new SortedList();
                    foreach (var table in dataBase.Table)
                    {
                        sl.Add(table.Name, table.Name);
                    }
                    int length = sl.Count;
                    this.checkedListBoxControl1.Items.Clear();
                    for (int i = 0; i < length; i++)
                    {
                        this.checkedListBoxControl1.Items.Add(sl.GetByIndex(i).ToString());
                    }

                    //连接成功后  
                    MessageBox.Show("刷新数据表成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //保存到临时数据库配置信息中以便下次使用
                    ConnectionConfig cc = new ConnectionConfig();
                    cc.SaveToXml(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedIndex, "TemConfig.xml");

                    toLinkStateOk2();
                }
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                this.ShowExceptionInfo(ex, "对不起，未能正确连接数据库，请重新检查数据库连接设置！");
                this.Enabled = true;
                this.checkedListBoxControl1.Items.Clear();
                toLinkStateOff2();
            }
        }

        /// <summary>
        /// 断开连接-维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OffLinkBtn_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("确认断开数据库的连接吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                this.checkedListBoxControl1.Items.Clear();
                toLinkStateOff2();
                MessageBox.Show("数据库已断开，请重新连接数据库！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 协调表结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton8_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "TBLST文件|*.tblst|所有文件|*.*";
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                string filename = openFileDialog1.FileName;

                if (filename == null || filename.Length <= 0 || !File.Exists(filename))
                    return;
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;

                TableSchemaInOut tsio = new TableSchemaInOut(idb);
                tsio.tableSchemaInput(filename);
                MessageBox.Show("恭喜，协调表结构成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch (Exception ex)
            {
                ShowExceptionInfo(ex, "对不起，导入表结构失败！");
            }
        }

        /// <summary>
        /// 显示异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="errorstr"></param>
        private void ShowExceptionInfo(Exception ex, string errorstr)
        {
            if (DialogResult.OK == MessageBox.Show(errorstr + "\n需要显示错误或异常的更多信息么？", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
            {
                ShowErrorInf sei = new ShowErrorInf(ex.ToString());
                sei.Show();
            }
        }

        /// <summary>
        /// 协调主键值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachIdBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.checkedListBoxControl1.CheckedItemsCount <= 0)
                {
                    MessageBox.Show("请选择需要协调的数据表", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.Enabled = false;
                ConnectionInfo ci = new ConnectionInfo(this.DbConectStrEdit.Text, this.DbTypeEdit.SelectedItem.ToString());
                DataBaseContext dbctx = new DataBaseContext(ci);
                IDataBaseHandler idb = dbctx.Handler;


                for (int i = 0; i < this.checkedListBoxControl1.CheckedItemsCount; i++)
                {
                    string ntbl = this.GetSelectedTableNames2[i];

                    TableSchemaInOut tsio = new TableSchemaInOut(idb);
                    Draco.DB.QuickDataBase.Schema.Vendor.Implementation.Table tbl = tsio.getOneTableStruByTableName(ntbl);

                    List<DataTableColumn> clist = tbl.Columns;

                    for (int j = 0; j < clist.Count; j++)
                    {
                        if (clist[j].Generated != null && (bool)clist[j].Generated)
                            idb.DbAdapter.GetSchemaHandler(idb).SynchSequence(ntbl, clist[j].ColumnName);
                    }
                }
                MessageBox.Show("协调主键值完成！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowExceptionInfo(ex, "对不起，协调表主键值失败！");
                this.Enabled = true;
            }
        }

        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWinBtn_Click(object sender, EventArgs e)
        {
            /*
            string path = Application.StartupPath + @"\test.sdf";
            if (!File.Exists(path))
            {

                SqlCeEngine sce = new SqlCeEngine("Data Source=" + path);
                sce.CreateDatabase();
            }
            */

            this.Close();
            this.Dispose();
            Application.Exit();
        }

        /// <summary>
        /// 实体工具的连接测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            if (this.TestDbConect(this.TxtConnect.Text, this.ComBoxDBTYpe.SelectedItem.ToString()))
            {
                MessageBox.Show("恭喜，测试连接成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
