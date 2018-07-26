using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace opt_Weld_identification
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
                    //t.TotalCheckTime();
            bool rst;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true,Application.ProductName,out rst);
            if (rst)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("OPT程序已经运行！");
                System.Environment.Exit(1);
            }

           
        }
    }
}
