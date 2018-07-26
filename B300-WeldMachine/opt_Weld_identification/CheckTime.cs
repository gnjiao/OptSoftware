using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;




namespace opt_Weld_identification
{
    class CheckTime:IDisposable
    {
        private string m_Dest;
        private DateTime m_Start;

        public CheckTime(string str)
        {
            Console.WriteLine("Begin:" + str + "====");
            m_Dest = str;
            m_Start = DateTime.Now;
        }

        public void TotalCheckTime()
        {
            long  strTime = (long)(DateTime.Now - m_Start).TotalMilliseconds;

             //Console.WriteLine("Desc:" + m_Dest + "   time:" + strTime + "ms");

             //获取当前程序目录
             string logPath = Path.GetDirectoryName(Application.ExecutablePath);
             //新建文件夹
             System.IO.StreamWriter sw = System.IO.File.AppendText(logPath + "/运行日志.txt");
             //写入日志信息
             sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff ") + "    动作描述: " + m_Dest + "   运行时间: " + strTime + "ms");
             //关闭文件
             sw.Close();
             //释放内存
             sw.Dispose();



        }

         public void Dispose()
         {
             TotalCheckTime();
         }
    }

 
}
