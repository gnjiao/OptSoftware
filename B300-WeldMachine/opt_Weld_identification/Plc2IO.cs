using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace FXPlcComm
{
    
    
    class Plc2IO
    {
        const int DICount = 1;//PLC 输入点个数
        
        
        private FXPlcComm.FXPlc FXPlcObject = new FXPlcComm.FXPlc();
        public byte[] DI = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };//16
        public bool bOpen = false;
        public bool bTerminate = false;
        private String[] DOName = new String[16];

        public Plc2IO()
        {
            InitDOName();
            //Thread myThread_1 = new Thread(new ThreadStart(RefreshDIProcess));
            //myThread_1.Start();
        }
        ~Plc2IO()  
        {
            bTerminate = true;
        }  
        private void InitDOName()
        {
            DOName[0] = "M32";
            DOName[1] = "M33";
            DOName[2] = "M34";
            DOName[3] = "M35";
            DOName[4] = "M36";
            DOName[5] = "M72";
            DOName[6] = "M71";

        }
        public bool _init(String ComPort, int baudrate, int databits, Parity parity, StopBits stopbits)
        {
            bool res =  FXPlcObject.Init(ComPort, baudrate, databits, parity, stopbits);
            if (res)
            {
                bOpen = true;
            }
            return res;
        }
        public void _port_close()
        {
            FXPlcObject.Port_close();

        }
        //
        public bool _analysisddata_(Byte[] recvDate, ref Byte[] DI, int DICount)
        {
            //if (FXPlcObject.ReadCmd("D123", DICount, ref recvDate) == FXPlcStatus.FX_OK)
            //{ 
            return FXPlcObject.AnalysisDData(recvDate, ref DI, DICount); 
            //}

        }
        ////没有使用
        //private void RefreshDIProcess()
        //{
        //    Byte[] Buff = new Byte[255];

        //    while (!bTerminate)
        //    {
        //        if (!bOpen)
        //        {
        //            Thread.Sleep(50);
        //            continue;
        //        }
        //        Byte[] recvDate = new Byte[50];
        //        if (FXPlcObject.ReadCmd("D72", DICount, ref recvDate) == FXPlcStatus.FX_OK)
        //        {
        //            FXPlcObject.AnalysisDData(recvDate, ref DI, DICount);
        //        }
        //        Thread.Sleep(20);
        //    }
        //}
        public FXPlcStatus WriteRegsiterD(String strUnit, int Value)
        {
            return FXPlcObject.WriteCmd(strUnit, Value);
        }
        public FXPlcStatus ReadRegsiterD(String strUnit, ref int Value)
        {
            Byte[] recvInt = new Byte[4];
            FXPlcStatus res = FXPlcObject.ReadCmd(strUnit, 2, ref  recvInt);
            if (res != FXPlcStatus.FX_OK)
            {
                return res;
            }

            //Byte[] temp8 = new Byte[8];
            //temp8[0] = recvInt[6];
            //temp8[1] = recvInt[7];
            //temp8[2] = recvInt[4];
            //temp8[3] = recvInt[5];
            ////
            //temp8[4] = recvInt[2];
            //temp8[5] = recvInt[3];
            //temp8[6] = recvInt[0];
            //temp8[7] = recvInt[1];
            //
            int i = 0;
            int j = 4;
            Byte[] tempx4 = new Byte[4];
            for (; i < 4; )
            {
                tempx4[i] = recvInt[j - 2];
                tempx4[i + 1] = recvInt[j - 1];
                i = i + 2;
                j = j - 2;

            }
            String s16DI = System.Text.Encoding.Default.GetString(tempx4);
            Value = Convert.ToInt32(s16DI, 16);
            return res;

        }
        //强制M寄存器
        public FXPlcStatus setM(string _M, bool Val)
        {
            return FXPlcObject.ForceCmd(_M, Val);
        }   
        //读取D寄存器
        public FXPlcStatus GetDO2(string IDX, ref Byte[] Val)//有问题地址转换不对
        {
            return FXPlcObject.ReadCmd(IDX, 1, ref Val);
        }
        //置位或复位M寄存器
        public FXPlcStatus SetDO(int Idx, bool Val)//有问题地址转换不对
        {
            return FXPlcObject.ForceCmd(DOName[Idx], Val);
        }
        public FXPlcStatus GetDO(int Idx, ref Byte[] Val)//有问题地址转换不对
        {
            return FXPlcObject.ReadCmd(DOName[Idx], 1, ref Val);
        }
        public int GetDI(int Idx)//D123寄存器
        {
            String strDI = System.Text.Encoding.Default.GetString(DI, Idx, 1);

            return int.Parse(strDI);
        }

    }
}
