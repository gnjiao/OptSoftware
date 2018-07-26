using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Common;

namespace login_generate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;

            if (RegistFileHelper.ExistComputerInfofile() == false || RegistFileHelper.ExistRegistInfofile() == false)
            {
                MessageBox.Show("注册文件不存在");
            }
            else
            {
                //openFileDialog1.Filter = "|Optech_Login.key";
                //if (openFileDialog1.ShowDialog() == DialogResult.OK)
                //{
                //    fileName = openFileDialog1.FileName;//得到选择的路径
                //}
                //else
                //{
                //    return;
                //}

                //string localFileName = string.Concat(
                //    Environment.CurrentDirectory,
                //    Path.DirectorySeparatorChar,
                //    RegistFileHelper.ComputerInfofile);

                //if (fileName != localFileName)
                //    File.Copy(fileName, localFileName, true);

                string computer = RegistFileHelper.ReadComputerInfoFile();
                EncryptionHelper help = new EncryptionHelper(EncryptionKeyEnum.KeyB);
                string md5String = help.GetMD5String(computer);
                string registInfo = help.EncryptString(md5String);
                RegistFileHelper.WriteRegistFile(registInfo);
                MessageBox.Show("注册码已生成");
            }

            
        }
    }
}
