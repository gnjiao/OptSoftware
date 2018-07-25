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

        ////保存参数记录
        //public void writeCSV(string path, double id, double data1, double data2, bool flag)
        //{
        //    lock (this)
        //    {
        //        string filePath = string.Format(" {0}\\{1}.csv", path, DateTime.Now.ToString("yyyy-MM-dd"));
        //        Console.WriteLine(filePath);

        //        if (!System.IO.File.Exists(filePath))//文件不存在时,创建新文件,并写入文件标题
        //        {
        //            //创建文件流对象
        //            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        //            //创建文件流写入对象,绑定文件流对象
        //            StreamWriter sw = new StreamWriter(fs);
        //            //创建数据对象
        //            StringBuilder sb = new StringBuilder();

        //            sb.Append("ID").Append(",").Append("Data1").Append(",").Append("Data2").Append(",").Append("Flag");
        //            //把标题内容写入到文件流中
        //            sw.WriteLine(sb);
        //            sw.Flush();
        //            sw.Close();
        //            fs.Close();
        //        }

        //        //向CSV文件中写入数据内容
        //        StreamWriter msw = new StreamWriter(filePath, true, Encoding.Default);
        //        //创建数据对象
        //        StringBuilder msb = new StringBuilder();
        //        msb.Append(id).Append(",").Append(data1).Append(",").Append(data2).Append(",").Append(flag);

        //        //把数据内容写入文件中
        //        msw.WriteLine(msb);
        //        msw.Flush();
        //        msw.Close();
        //    }
        //}

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
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SaveCSV(DataTable dt, string fullPath)//table数据写入csv
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            System.IO.FileStream fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Create,
                System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)//写入列名
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        /////////////////////////////////////////////////////////////////////////////
        public static DataTable OpenCSV(string filePath)//从csv读取数据返回table
        {
            System.Text.Encoding encoding = GetType(filePath); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
                System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }






















        ////////////////////////////////////////////////////////////////////////////////////////////////
    }


}
