using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using DevExpress.XtraEditors;

namespace CodeGenerator
{
    public partial class ShowDbCase : XtraForm
    {
        public ShowDbCase(int i)
        {
            InitializeComponent();
            this.index = i;
        }

        private void cksl_Load(object sender, EventArgs e)
        {
            switch (index)
            {
                case 0://SqlServer
                    {
                        this.richTextBox1.Text =
                            ">>Server=(local);User id=sa;Pwd=sa;Database=Dracox;" + "\n" +
                            ">>Integrated Security=SSPI;Server='host-name';Database=master;" + "\n" +

                            ">>Server='host-name\\SQL2005';User id=sa;Pwd=sa;Database=msdb;" + "\n" +
                            ">>Server='host-name\\SQL2008';User id=sa;Pwd=sa;Database=master;" + "\n" +
                            ">>Integrated Security=SSPI;Server='host-name\\SQL2005';Database=master;" + "\n" +
                            ">>Data Source=host-name\\SQLEXPRESS;Initial Catalog=model;Integrated Security=True;";
                        break;
                    }

                case 1://Oracle
                    {
                        this.richTextBox1.Text=
                            ">>User ID=scott;Password=tiger;Data Source=oracle92;" + "\n" + 
                            ">>User ID=scott;Password=tiger;Data Source=oracle92;max pool size=30;" + "\n" + 
                            ">>User ID=scott;Password=tiger;Data Source=oracle92;Persist Security Info=True;Unicode=True;Omit Oracle Connection Name=True;";
                        break;
                    }
                case 2://OleDB
                    {
                        this.richTextBox1.Text =
                            "Access :" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0;Data Source=;User Id=;Password=;" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0;Data Source=mdb_filefullpath;User Id=admin;Password=;" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\somepath\\mydb.mdb; Jet OLEDB:System Database=system.mdw;" + "\n\n" +

                            "Excel :" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:Database Password=;Data Source=ExcelFileFullPath;Extended Properties='Excel 8.0;HDR=YES;IMEX=1';" + "\n\n" +

                            "Oracle :" + "\n" +
                            ">>Provider='MSDAORA.1';User ID=scott;Password=tiger;Data Source=oracle92;" + "\n" +
                            ">>Provider=MSDAORA; User ID=scott;Password=tiger;Data Source=oracle92;" + "\n" +
                            ">>Provider=OraOLEDB.Oracle; User ID=scott;Password=tiger;Data Source=oracle92;" + "\n\n" +

                            "SqlServer :" + "\n" +
                            ">>Provider=SQLOLEDB; Data Source=(local); Initial Catalog=pubs; User ID=sa; Password=sa;" + "\n" +
                            ">>Provider=SQLOLEDB; Data Source='host-name'; Initial Catalog=pubs; User ID=sa; Password=sa;" + "\n\n" +

                            "SqlServer 2005 :" + "\n" +
                            ">>Provider=SQLOLEDB; Data Source='host-name\\SQL2005'; Initial Catalog=pubs; User ID=sa; Password=sasa;" + "\n" +
                            ">>Provider=SQLOLEDB; Data Source='host-name\\SQL2005';Integrated Security=SSPI; Initial Catalog=pubs;" + "\n\n" +

                            "MySQL :" + "\n" +
                            ">>Provider=MySQLProv; Data Source=mydb; User Id=UserName; Password=asdasd; " + "\n\n" +

                            "Interbase :" + "\n" +
                            ">>provider=sibprovider; location=localhost:; data source=c:\\databases\\gdbs\\mygdb.gdb; user id=SYSDBA; password=masterkey;" + "\n\n" +

                            "IBM DB2 :" + "\n" +
                            ">>Provider=DB2OLEDB; Network Transport Library=TCPIP; Network Address=XXX.XXX.XXX.XXX; Initial Catalog=MyCtlg; Package Collection=MyPkgCol; Default Schema=Schema; User ID=MyUser; Password=MyPW;" + "\n\n" +

                            "Sybase :" + "\n" +
                            ">>Provider=ASAProv; Data source=myASA;" + "\n\n" +

                            "Informix :" + "\n" +
                            ">>Provider=Ifxoledbc.2; password=myPw; User ID=myUser; Data Source=dbName@serverName; Persist Security Info=true;" + "\n\n" +

                            "Paradox :" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\\myDb; Extended Properties=Paradox 5.x;" + "\n\n" +

                            "DBF / FoxPro :" + "\n" +
                            ">>Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\\folder; Extended Properties=dBASE IV; User ID=Admin; Password=admin;" + "\n\n" +

                            "AS/400 (iSeries) :" + "\n" +
                            ">>PROVIDER=IBMDA400; DATA SOURCE=MY_SYSTEM_NAME; USER ID=myUserName; PASSWORD=myPwd;" + "\n" +
                            ">>ExOLEDB.DataSource;" + "\n\n" +

                            "Visual FoxPro :" + "\n" +
                            ">>Provider=vfpoledb.1; Data Source=C:\\MyDbFolder\\MyDbContainer.dbc; Collating Sequence=machine;" + "\n" +
                            ">>Provider=vfpoledb.1; Data Source=C:\\MyDataDirectory\\; Collating Sequence=general;" + "\n" +
                            ">>Pervasive :" + "\n" +
                            ">>Provider=PervasiveOLEDB; Data Source=C:\\path;";

                        break;
                    }
       
                case 3://SQLite
                    {
                        this.richTextBox1.Text =
                            ">>data source=SQLiteTest.db;" + "\n" +
                            ">>data source=C:\\SQLlite\\DB\\test.db;password=Admin;" + "\n" +
                            ">>Data Source=Data\\SQLiteDemo.db;Pooling=true;FailIfMissing=false;";
                        break;
                    }
 
                case 4://Custom(自定义)
                    {
                        this.richTextBox1.Text =
                            "请出入自定义连接字符串";
                        break;
                    }
                default:
                    {
                        this.richTextBox1.Text = "";
                        break;
                    }
            }
        }
    }
}
