using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Alert_handle_from
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }
        int tempadd = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((tempadd & 0x01)==0) { label1.ForeColor = Color.Blue; }
            else { label1.ForeColor = Color.Fuchsia; }
            tempadd++;
            //if (tempadd>5)
            //{
            //    System.Diagnostics.Process _Process = new System.Diagnostics.Process();
            //    _Process.StartInfo.FileName = "opt_Weld_identification.exe";
            //    _Process.StartInfo.WorkingDirectory = "\\opt_Weld_identification.exe";
            //    _Process.StartInfo.CreateNoWindow = true;
            //    _Process.Start();
            //    Environment.Exit(1);
            //}
        }

        private void button_rest_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process _Process = new System.Diagnostics.Process();
            _Process.StartInfo.FileName = "opt_Weld_identification.exe";
            _Process.StartInfo.WorkingDirectory = "\\opt_Weld_identification.exe";
            _Process.StartInfo.CreateNoWindow = true;
            _Process.Start();
            Environment.Exit(1);
        }



    }
}
