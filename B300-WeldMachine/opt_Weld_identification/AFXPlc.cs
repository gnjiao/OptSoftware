using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;


//命名空间包含允许读写文件和数据流的类型以及提供基本文件和目录支持的类型。
using System.IO;
using System.IO.Ports;
//包含着ArrayList，Hashtable，SortedList这三个类
using System.Collections;
//线程
using System.Threading;

namespace FXPlcComm
{

    class Base_Register
    {
        public static int Force_M_Base_Address = 16384;//强制地址M变量地址
        public static int WR_D_Base_Address = 4096;//D寄存器读取
        public static int WR_M_Base_Address = 193;//D寄存器读取
    }
    
    
    enum FXPlcStatus
    {
        FX_OK = 0,                  //PLC返回正常指令 0X06
        FX_Error_SendFault = 2,     //发送失败
        FX_Error_TimeOut = 1,       //超时
        FX_Error_ForceFault = 3,    //PLC返回失败指令 0X15
        FX_Error_Unknow = 4,        //程序操作失败，未知错误
        FX_Error_ReadFault = 5,     //读取失败
        FX_Error_NotOpen = 6        //端口未打开
    }
    
    class FXPlc
    {
        
        
        protected SerialPort CommPort = new SerialPort();
        static Mutex MuTComm = new Mutex();
        static Mutex MuTUnit = new Mutex();
        public bool bOpen = false;///接收线程运行标记
        public bool bTerminate = false;
        Byte[] RecvMakeupBuff = new Byte[255];//报文接收缓存
        Byte[] ClearBuff = new Byte[255];//清0接收缓存
        Queue<Byte[]> RecvMsgQueue = new Queue<Byte[]>();//读指令反馈队列
        int RecvMakeupCount = 0;//内部使用读报文计数器
        int TimeOut = 1000;//通信超时时间*5ms 默认50000
        int CycleTime = 10;//接收线程循环时间ms
        int CmdType;//0是force；1是read
        public FXPlc()
        {
            for (int i = 0; i < 255; i++ )
            {
                ClearBuff[i] = 0;
            }

            Thread myThread_1 = new Thread(new ThreadStart(ReadProcess));
            myThread_1.Start();
            
        }
        ~FXPlc()  
        {
            bTerminate = true;
        } 
        //初始化端口
        public bool Init(String ComPort, int baudrate, int databits, Parity parity, StopBits stopbits)
        {
            try
            {
                CommPort.PortName = ComPort;
                CommPort.BaudRate = baudrate;
                CommPort.DataBits = databits;
                CommPort.Parity = parity;
                CommPort.StopBits = stopbits;
                CommPort.ReadBufferSize = 1024;
                CommPort.ReadTimeout = 20;
                CommPort.WriteTimeout = 500;
                CommPort.Open();
                CommPort.DiscardInBuffer();
                if (CommPort.IsOpen)
                {
                    bOpen = true;
                }
                return CommPort.IsOpen;
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("PLC端口打开失败");
                return false;
            }
            
        }
        //断开连接
        public void Port_close()
        {
            bOpen = false;//暂停信息线程
            Thread.Sleep(100);
            CommPort.Close();//关闭串口
            try
            {
                CommPort.DiscardInBuffer();//清除串口缓存
            }
            catch (System.Exception ex)
            {
            	//错误时不做处理
            }
            RecvMsgQueue.Clear();//清除队列数据
            Buffer.BlockCopy(ClearBuff, 0, RecvMakeupBuff, 0, 255);//清空缓存
            RecvMakeupCount = 0;//内部使用读报文计数器

        }
        //发送报文
        protected bool SendMsg(Byte[] Msg)
        {
            try
            {
                if (!CommPort.IsOpen)
                    return false;
                //MuTUnit.WaitOne();
                CommPort.DiscardInBuffer();
                CommPort.Write(Msg, 0, Msg.Length);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally
            {
                //MuTUnit.ReleaseMutex();
            }
            
        }
        //接收反馈报文
        protected int RecvMsg(Byte[] RecvBuff, int Count)
        {
            try
            {
                if (!CommPort.IsOpen)
                    return 0;
                //MuTUnit.WaitOne();
                return CommPort.Read(RecvBuff, 0, Count);
                
            }
            catch (System.Exception ex)
            {
                return 0;
            }
            finally
            {
                //MuTUnit.ReleaseMutex();
            }
        }
        //组成完整的反馈报文
        private Byte[] MakeupRecvMsg(Byte[] RecvBuff, int Count )
        {
            if (CmdType == 0) //强制指令反馈报文为1个字节
            {
                Byte[] NewMsg = new Byte[1];
                NewMsg[0] = RecvBuff[0];
                return NewMsg;
            }
            else if (CmdType == 1)
            {
                Buffer.BlockCopy(RecvBuff, 0, RecvMakeupBuff, RecvMakeupCount, Count);
                Byte BeginByte = 0x02;
                Byte EndByte = 0x03;
                int MsgBegin = Array.IndexOf(RecvMakeupBuff, BeginByte);
                int MsgEnd = Array.IndexOf(RecvMakeupBuff, EndByte);
                //判断是否包含头和尾。是则接受到完整报文，否则继续等待后续。由于是应答方式，不会出现多个反馈报文情况
                if ((MsgBegin >= 0) && (MsgBegin < MsgEnd))
                {
                    //获取到一个新的消息
                    Byte[] NewMsg = new Byte[MsgEnd - MsgBegin + 3];
                    Buffer.BlockCopy(RecvMakeupBuff, MsgBegin, NewMsg, 0, MsgEnd - MsgBegin + 3);
                    RecvMakeupCount = 0;
                    Buffer.BlockCopy(ClearBuff, 0, RecvMakeupBuff, 0, 255);//清空缓存
                    return NewMsg;
                }
                else
                {
                    RecvMakeupCount += Count;
                }
            }
            
            
            return null;
        }
        private void ReadProcess()//读取返回信息线程，非阻塞方式
        {
            Byte[] Buff = new Byte[255];
            
            while (!bTerminate)
            {
                if (!bOpen)
                {
                    Thread.Sleep(50);
                    continue;
                }
                
                int RecvCount = RecvMsg(Buff,  255);
                if (RecvCount>0)
                {
                    Byte[] NewMsg = MakeupRecvMsg(Buff, RecvCount);
                    if (NewMsg != null)
                    {
                        //收到完整报文后放入接收队列
                        RecvMsgQueue.Enqueue(NewMsg);
                    }
                }
                Thread.Sleep(CycleTime);
            }
        }
        //组成强制指令报文
        protected Byte[] MakeForceCmd(Byte[] Addr, bool Value)
        {
            try
            {
                Byte[] NewCmd = new Byte[10];
                NewCmd[0] = 0x02;
                NewCmd[1] = 0x45;
                if (Value)
                {
                    NewCmd[2] = 0x37;
                }
                else
                {
                    NewCmd[2] = 0x38;
                }
                NewCmd[3] = Addr[0];
                NewCmd[4] = Addr[1];
                NewCmd[5] = Addr[2];
                NewCmd[6] = Addr[3];
                NewCmd[7] = 0x03;
                SumCalc(ref NewCmd);
                return NewCmd;

            }
            catch (System.Exception ex)
            {
                return null;
            }   
        }

        //组成强制指令报文
        protected Byte[] MakeWriteCmd(Byte[] Addr, Byte[] Value)
        {
            try
            {
                if ((Value.Length % 2) > 0)//如果不能被2整除说明有问题
                    return null;

                Byte[] count = new Byte[4];
                ConvertIntToByteArray(Value.Length / 2, ref count);
                int MsgLen = 11 + Value.Length;
                Byte[] NewCmd = new Byte[MsgLen];
                NewCmd[0] = 0x02;
                NewCmd[1] = 0x31; 
                NewCmd[2] = Addr[0];
                NewCmd[3] = Addr[1];
                NewCmd[4] = Addr[2];
                NewCmd[5] = Addr[3];
                NewCmd[6] = count[2];
                NewCmd[7] = count[3];
                for (int i = 0; i < Value.Length;i++ )
                {
                    NewCmd[i + 8] = Value[i];
                }

                NewCmd[8 + Value.Length] = 0x03;
                SumCalc(ref NewCmd);
                return NewCmd;

            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        //组成读指令报文
        protected Byte[] MakeReadCmd(Byte[] Addr, short Lenght)
        {
            try
            {

                Byte[] len = new Byte[4];
                ConvertIntToByteArray(Lenght, ref len);
                     
                
                Byte[] NewCmd = new Byte[11];
                NewCmd[0] = 0x02;
                NewCmd[1] = 0x30;
                NewCmd[2] = Addr[0];
                NewCmd[3] = Addr[1];
                NewCmd[4] = Addr[2];
                NewCmd[5] = Addr[3];
                NewCmd[6] = len[2];
                NewCmd[7] = len[3];
                NewCmd[8] = 0x03;
                SumCalc(ref NewCmd);
                return NewCmd;

            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        //强制地址转换函数
        protected Byte[] Force_Unit2Address(String ObjectUnit)
        {
            if (ObjectUnit.Contains("M"))
            {
                int Num = int.Parse(ObjectUnit.Substring(1, ObjectUnit.Length - 1));
                return Force_M_Unit2Address(Num);
            }
            
            return null;
        }
        //读写地址转换
        protected Byte[] WR_Unit2Address(String ObjectUnit)
        {
            if (ObjectUnit.Contains("D"))
            {
                int Num = int.Parse(ObjectUnit.Substring(1, ObjectUnit.Length - 1));
                return WR_D_Unit2Address(Num);
            }
            if (ObjectUnit.Contains("M"))
            {
                int Num = int.Parse(ObjectUnit.Substring(1, ObjectUnit.Length - 1));
                return WR_M_Unit2Address(Num);
            }

            return null;
        }
        protected Byte[] Force_M_Unit2Address(int iUnit)//强制M寄存器地址OK
        {
            int iFullUnit = Base_Register.Force_M_Base_Address + iUnit;
            String strFullUnit = iFullUnit.ToString("X4");
            Byte[] addr = new Byte[4];
            addr = System.Text.Encoding.ASCII.GetBytes(strFullUnit);
            Byte tmp;
            tmp = addr[0];
            addr[0] = addr[2];
            addr[2] = tmp;
            tmp = addr[1];
            addr[1] = addr[3];
            addr[3] = tmp; 
            return addr;
        }
        protected Byte[] WR_D_Unit2Address(int iUnit)//D寄存器地址换算OK
        {
            int iFullUnit = Base_Register.WR_D_Base_Address + iUnit*2;
            String strFullUnit = iFullUnit.ToString("X4");
            Byte[] addr = new Byte[4];
            addr = System.Text.Encoding.ASCII.GetBytes(strFullUnit);
            
            return addr;
        }
        protected Byte[] WR_M_Unit2Address(int iUnit)//地址换算有问题
        {
            int iFullUnit = Base_Register.WR_M_Base_Address + iUnit;
            String strFullUnit = iFullUnit.ToString("X4");
            Byte[] addr = new Byte[4];
            addr = System.Text.Encoding.ASCII.GetBytes(strFullUnit);

            return addr;
        }
        //非自动计算，人工转换地址函数
        protected Byte[] Manual_Unit2Address(String ObjectUnit)
        {

            if (ObjectUnit == "M32")
            {
                Byte[] addr =  {0x32,0x30,0x34,0x30};
                return addr;
            }
            else if (ObjectUnit == "M33")
            {
                Byte[] addr = { 0x32, 0x31, 0x34, 0x30 };
                return addr;
            }
            else if (ObjectUnit == "M34")
            {
                Byte[] addr = { 0x32, 0x32, 0x34, 0x30 };
                return addr;
            }
            else if (ObjectUnit == "M35")
            {
                Byte[] addr = { 0x32, 0x33, 0x34, 0x30 };
                return addr;
            }
            else if (ObjectUnit == "M36")
            {
                Byte[] addr = { 0x32, 0x34, 0x34, 0x30 };
                return addr;
            }
            else if (ObjectUnit == "M37")
            {
                Byte[] addr = { 0x32, 0x35, 0x34, 0x30 };
                return addr;
            }
            else if (ObjectUnit == "M72")
            {
                Byte[] addr = { 0x30, 0x31, 0x30, 0x39 };
                return addr;
            }
            else if (ObjectUnit == "D123")
            {
                Byte[] addr = { 0x31, 0x30, 0x46, 0x36 };
                return addr;
            } 
            else
            {
            }
            return null;
        }
        //转换INT与BYTE 格式
        bool ConvertIntToByteArray(Int32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;

            String strSum = m.ToString("X4");
            
            Char[] Sum = strSum.ToCharArray(0, 4);
            arry[0] = (Byte)Sum[0];
            arry[1] = (Byte)Sum[1];
            arry[2] = (Byte)Sum[2];
            arry[3] = (Byte)Sum[3];
            return true;
        }
        //计算奇偶校验
        protected void SumCalc(ref Byte[] Cmd)
        {
            int isum = 0;
            
            for (int i=1; i<(Cmd.Length-2); i++)
            {
                isum += Cmd[i];
            }
            Byte[] Sum = new Byte[4];
            ConvertIntToByteArray(isum, ref Sum);
            Cmd[Cmd.Length - 2] = Sum[2];
            Cmd[Cmd.Length - 1] = Sum[3];

        }
        //写指令操作-
        public FXPlcStatus WriteCmd(String ObjectUnit, int iVal)
        {
            if (!CommPort.IsOpen)
            {
                return FXPlcStatus.FX_Error_NotOpen;
            }
            Byte[] SendData = new Byte[4];
            ConvertIntToByteArray(iVal, ref SendData);
            //转地址
            Byte[] addr = WR_Unit2Address(ObjectUnit);
            //组织报文
            Byte[] Cmd = MakeWriteCmd(addr, SendData);
            try
            {
                MuTComm.WaitOne();
                CmdType = 0;
                bool bSend = SendMsg(Cmd);
                if (!bSend)
                {
                    return FXPlcStatus.FX_Error_SendFault;
                }
                int TimeOutTimer = 0;
                while (true)
                {
                    if (RecvMsgQueue.Count > 0)
                        break;
                    if (TimeOutTimer > TimeOut)
                    {
                        //超时
                        return FXPlcStatus.FX_Error_TimeOut;
                    }
                    TimeOutTimer += 5;
                    Thread.Sleep(5);
                }
                Byte[] recvCmd = RecvMsgQueue.Dequeue();
                if (recvCmd.Length < 3)
                {
                    if (ContainCmd(recvCmd, 0x06))
                    {
                        //反馈-指令有效
                        return FXPlcStatus.FX_OK;
                    }
                    else if (ContainCmd(recvCmd, 0x15))
                    {
                        //反馈-指令有效
                        return FXPlcStatus.FX_Error_ForceFault;
                    }

                }
                return FXPlcStatus.FX_Error_Unknow;
            }
            catch (System.Exception ex)
            {
                return FXPlcStatus.FX_Error_Unknow;
            }
            finally
            {
                 RecvMsgQueue.Clear();
                 Buffer.BlockCopy(ClearBuff, 0, RecvMakeupBuff, 0, 255);//清空缓存
                RecvMakeupCount = 0;//内部使用读报文计数器
                MuTComm.ReleaseMutex();
            }
        }
        //读指令操作
        public FXPlcStatus ReadCmd(String ObjectUnit, short Lenght, ref Byte[] RecvData)
        {
            if (!CommPort.IsOpen)
            {
                return FXPlcStatus.FX_Error_NotOpen;
            }
            //转地址
            Byte[] addr = WR_Unit2Address(ObjectUnit);
            //组织报文
            Byte[] Cmd = MakeReadCmd(addr, Lenght);
            
            try
            {
                MuTComm.WaitOne();
                CmdType = 1;
                bool bSend = SendMsg(Cmd);
                if (!bSend)//发送失败
                {
                    return FXPlcStatus.FX_Error_SendFault;
                }
                int TimeOutTimer = 0;
                while (true)
                {
                    if (RecvMsgQueue.Count > 0)
                        break;
                    if (TimeOutTimer > TimeOut)
                    {
                        //超时
                        return FXPlcStatus.FX_Error_TimeOut;
                    }
                    TimeOutTimer += 5;
                    Thread.Sleep(5);
                }
                Byte[] recvCmd = RecvMsgQueue.Dequeue();
                if (recvCmd.Length > 3)
                {
                    Byte BeginByte = 0x02;
                    Byte EndByte = 0x03;
                    int MsgBegin = Array.IndexOf(recvCmd, BeginByte);
                    int MsgEnd = Array.IndexOf(recvCmd, EndByte);
                    //判断报文是否有效
                    if ((MsgBegin >= 0) && (MsgBegin < MsgEnd))
                    {
                        //获取数据段
                        Buffer.BlockCopy(recvCmd, MsgBegin+1, RecvData, MsgBegin, MsgEnd - MsgBegin -1);
                        return FXPlcStatus.FX_OK;
                    }
                    else
                        return FXPlcStatus.FX_Error_ReadFault;


                }
                return FXPlcStatus.FX_Error_Unknow;
            }
            catch (System.Exception ex)
            {
                return FXPlcStatus.FX_Error_Unknow;
            }
            finally
            {
                RecvMsgQueue.Clear();
                Buffer.BlockCopy(ClearBuff, 0, RecvMakeupBuff, 0, 255);//清空缓存
                RecvMakeupCount = 0;//内部使用读报文计数器
                MuTComm.ReleaseMutex();
            }
            //return true;
        }
        protected bool ContainCmd(Byte[] Cmd, Byte Val)
        {
            for (int i=0;i<Cmd.Length; i++)
            {
                if (Cmd[i] == Val)
                {
                    return true;
                }
            }
            return false;
        }

        //强制指令操作
        public FXPlcStatus ForceCmd(String ObjectUnit, bool Value)
        {
            if (!CommPort.IsOpen)
            {
                return FXPlcStatus.FX_Error_NotOpen;
            }
            //转地址
            Byte[] addr = Force_Unit2Address(ObjectUnit);
            //组织报文
            Byte[] Cmd = MakeForceCmd(addr, Value);
            try 
            {
                MuTComm.WaitOne();
                CmdType = 0;
                bool bSend = SendMsg(Cmd);
                if (!bSend)
                {
                    return FXPlcStatus.FX_Error_SendFault;
                }
                int TimeOutTimer = 0; 
                while (true)
                {
                    if(RecvMsgQueue.Count > 0)
                        break;
                    if (TimeOutTimer > TimeOut)
                    {
                        //超时
                        return FXPlcStatus.FX_Error_TimeOut;
                    }
                    TimeOutTimer += 5;
                    Thread.Sleep(5);
                }
                Byte[] recvCmd = RecvMsgQueue.Dequeue();
                if (recvCmd.Length < 3)
                {
                    if (ContainCmd(recvCmd, 0x06))
                    {
                        //反馈-指令有效
                        return FXPlcStatus.FX_OK;
                    }
                    else if (ContainCmd(recvCmd, 0x15))
                    {
                        //反馈-指令有效
                        return FXPlcStatus.FX_Error_ForceFault;
                    }

                }
                return FXPlcStatus.FX_Error_Unknow;
            }
            catch (System.Exception ex)
            {
                return FXPlcStatus.FX_Error_Unknow;
            }
            finally
            {
                MuTComm.ReleaseMutex();
            }
        }
        //解析D寄存器数据
        //将数据转为ASC码01
        public bool AnalysisDData(Byte[] DData, ref Byte[] Dist, int Count)
        {
            try
            {
                String s16DI = System.Text.Encoding.Default.GetString(DData);
                int x = 0;
                for (int j = 0; j < Count; j++)
                {
                    String tmp = "0x" + s16DI.Substring(j * 2, 2);

                    Byte portData = Convert.ToByte(tmp, 16);
                    string s2DI = Convert.ToString(portData, 2);

                    for (int i = s2DI.Length - 1; i >= 0; i--)
                    {
                        if (s2DI.Substring(i, 1) == "1")
                        {
                            Dist[x] = 0x31;
                        }
                        else if (s2DI.Substring(i, 1) == "0")
                        {
                            Dist[x] = 0x30;
                        }

                        x++;
                    }
                }

                for (int i = x; i < 16; i++)
                {
                    Dist[i] = 0x30;
                }

                return true;

            }
            catch (Exception)
            {
                return false;

            }

        }

    }
}
