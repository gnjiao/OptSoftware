using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data;

namespace WindowsJinKo
{
    public class RunLog
    {
        //运行记录
        public void RunRecord(string msg)
        {
            lock (this)
            {
                //获取当前程序目录
                string logPath = Application.StartupPath + "\\Log\\ErrorLog.txt";
                //新建文件夹
                System.IO.StreamWriter sw = System.IO.File.AppendText(logPath);
                //写入日志信息
                sw.WriteLine(DateTime.Now.ToString("时间:   yyyy-MM-dd HH:mm:ss:fff   ") + "异常信息:   " + msg);

                //关闭文件
                sw.Close();
                //释放内存
                sw.Dispose();
            }
        }

        //保存模板参数
        public void RunModel(string msg)
        {
            lock (this)
            {
                //获取当前程序目录
                string logPath = Application.StartupPath + "\\Log\\Model.txt";
                //新建文件夹
                System.IO.StreamWriter sw = System.IO.File.AppendText(logPath);
                //写入日志信息
                sw.WriteLine(DateTime.Now.ToString("时间:   yyyy-MM-dd HH:mm:ss:fff   ") + "模板匹配时间 :   " + msg);

                //关闭文件
                sw.Close();
                //释放内存
                sw.Dispose();
            }
        }

        //创建文件夹
        public void CreateFile()
        {
            //定义路劲
            string FilePathData;
            string FilePathLog;
            string FilePathImage;

            //文件路径
            FilePathData = Application.StartupPath + "\\Data\\";
            FilePathLog = Application.StartupPath + "\\Log\\";
            FilePathImage = Application.StartupPath + "\\Model\\";

            //判断文件是否存在
            bool FileData = FileExist(FilePathData);
            bool FileLog = FileExist(FilePathLog);
            bool FileImage = FileExist(FilePathImage);

            //如果文件夹不存在，则创建文件夹
            if (!FileData)
            {
                DirectoryInfo DInfo = new DirectoryInfo(FilePathData);//创建DirectoryInfo对象
                DInfo.Create();//创建文件夹
            }
            if (!FileLog)
            {
                DirectoryInfo DInfo = new DirectoryInfo(FilePathLog);//创建DirectoryInfo对象
                DInfo.Create();//创建文件夹
            }
            if (!FileImage)
            {
                DirectoryInfo DInfo = new DirectoryInfo(FilePathImage);//创建DirectoryInfo对象
                DInfo.Create();//创建文件夹
            }

        }

        //判断文件是否存在
        public bool FileExist(string FileName)
        {
            HTuple hv_flag;
            HOperatorSet.FileExists(FileName, out hv_flag);
            return hv_flag;
        }

    }


}
