using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using System.IO;
//using Common;

namespace opt_Weld_identification
{
    public partial class User_Form : Form
    {
        public User_Form()
        {
            InitializeComponent();
        }

        //private string encryptComputer = string.Empty;
        //private bool isRegist = false;
        //private void User_Form_Load(object sender, EventArgs e)
        //{
        //    string computer = ComputerInfo.GetComputerInfo();
        //    encryptComputer = new EncryptionHelper().EncryptString(computer);
        //    if (CheckRegist() == true)
        //    {
        //        toolStripStatusLabel1_注册.Text = "已注册";
        //    }
        //    else
        //    {
        //        toolStripStatusLabel1_注册.Text = "未注册";
        //        RegistFileHelper.WriteComputerInfoFile(encryptComputer);
        //    }
        //}

        //private bool CheckRegist()
        //{
        //    EncryptionHelper helper = new EncryptionHelper();
        //    string md5key = helper.GetMD5String(encryptComputer);
        //    return CheckRegistData(md5key);
        //}
        //private bool CheckRegistData(string key)
        //{
        //    if (RegistFileHelper.ExistRegistInfofile() == false)
        //    {
        //        isRegist = false;
        //        return false;
        //    }
        //    else
        //    {
        //        string info = RegistFileHelper.ReadRegistFile();
        //        var helper = new EncryptionHelper(EncryptionKeyEnum.KeyB);
        //        string registData = helper.DecryptString(info);
        //        if (key == registData)
        //        {
        //            isRegist = true;
        //            return true;
        //        }
        //        else
        //        {
        //            isRegist = false;
        //            return false;
        //        }
        //    }
        //}

        private void 用户登陆_Click(object sender, EventArgs e)
        {
            登陆主界面();
        }

        private void 登陆主界面()
        {
            Form2 Form2_ = new Form2();
            Form2_.groupBox1.Enabled = false;
            Form2_.groupBox2.Enabled = false;
            Form2_.groupBox_JK_MOD1.Enabled = false;
            Form2_.groupBox5.Enabled = false;
            //Form2_.label_OK.BackColor = Color.Lime; ;
            //Form2_.label_NG.BackColor = Color.Red; ;
            if (comboBox1.Text == "调试")
            {
                if (textBox1.Text == "1234567")
                {
                    this.Hide();

                    Form2_.groupBox_图像保存与分选调试.Enabled = true;
                    Form2_.groupBox_用户设置.Enabled = true;

                    Form2_.groupBox_焊接容错率.Enabled = true;
                    Form2_.tabControl1_主设置.Enabled = true;
                    Form2_.groupBox1.Enabled = true;
                    Form2_.groupBox2.Enabled = true;
                    Form2_.groupBox_JK_MOD1.Enabled = true;
                    Form2_.groupBox5.Enabled = true;

                    Form2_.Show();

                }
                else
                {
                    MessageBox.Show("密码错误");
                }
            }
            else if (comboBox1.Text == "admin")
            {
                if (textBox1.Text == "111")
                {
                    this.Hide();
                    //Form2.User_Form2 = null;

                    Form2_.相机参数.Enabled = true;
                    Form2_.相机参数.BackgroundImage = opt_Weld_identification.Properties.Resources.相机1;

                    Form2_.IMAGE_directshow.Visible = true;

                    Form2_.相机端口号.Visible = true;
                    Form2_.label_相机端口号.Visible = true;

                    Form2_.groupBox_图像保存与分选调试.Enabled = true;
                    Form2_.groupBox_用户设置.Enabled = true;
                    Form2_.groupBox_匹配坐标.Visible = true;
                    Form2_.groupBox_特征定位.Visible = true;
                    Form2_.groupBox_焊接端子定位.Visible = true;

                    Form2_.groupBox_焊接容错率.Enabled = true;

                    Form2_.groupBox_PLC类型.Enabled = true;
                    Form2_.tabControl1_主设置.Enabled = true;
                    Form2_.groupBox1.Enabled = true;
                    Form2_.groupBox2.Enabled = true;
                    Form2_.groupBox_JK_MOD1.Enabled = true;
                    Form2_.groupBox5.Enabled = true;

                    Form2_.button_DO_str_alert.Visible = true;//显示报警测试按钮
                    Form2_.button_alert_Handle.Visible = true;//显示急停模拟测试按钮

                    Form2_.Show();

                }
                else
                {
                    MessageBox.Show("密码错误");

                }
            }
            else
            {
                this.Hide();

                Form2_.Show();
            }
        }

        private void User_Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter /*&& e.Control*/)
            {
                e.Handled = true;
                //将Handled设置为true，指示已经处理过KeyPress事件
                登陆主界面();
            }

        }

        //private void toolStripStatusLabel1_注册_Click(object sender, EventArgs e)
        //{
        //    if (toolStripStatusLabel1_注册.Text != "已注册")
        //    {


        //    }


        //}


    }
}
