using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//命名空间包含允许读写文件和数据流的类型以及提供基本文件和目录支持的类型。
using System.IO;
using System.IO.Ports ;
//包含着ArrayList，Hashtable，SortedList这三个类
using System.Collections;
//线程
using System.Threading;


namespace opt_Weld_identification
{
public class TMCL_Define
{
    
public static Byte TMCL_ROR = 1;
public static Byte TMCL_ROL = 2;
public static Byte TMCL_MST = 3;
public static Byte TMCL_MVP = 4;
public static Byte TMCL_SAP = 5;
public static Byte TMCL_GAP = 6;
public static Byte  TMCL_STAP= 7;
public static Byte  TMCL_RSAP= 8;
public static Byte  TMCL_SGP= 9;
public static Byte  TMCL_GGP= 10;
public static Byte  TMCL_STGP= 11;
public static Byte  TMCL_RSGP= 12;
public static Byte  TMCL_RFS= 13;
public static Byte  TMCL_SIO= 14;
public static Byte  TMCL_GIO= 15;
public static Byte  TMCL_SCO= 30;
public static Byte  TMCL_GCO= 31;
public static Byte  TMCL_CCO= 32;

//Opcodes of TMCL control functions (to be used to run or abort a TMCL program in the module)
public static Byte  TMCL_APPL_STOP= 128;
public static Byte  TMCL_APPL_RUN= 129;
public static Byte  TMCL_APPL_RESET= 131;

//Options for MVP commandds
public static Byte  MVP_ABS= 0;
public static Byte  MVP_REL= 1;
public static Byte  MVP_COORD= 2;

//Options for RFS command
public static Byte  RFS_START= 0;
public static Byte  RFS_STOP= 1;
public static Byte  RFS_STATUS= 2;


//Result codes for GetResult
static int  TMCL_RESULT_OK= 0;
static int  TMCL_RESULT_NOT_READY= 1;
static int  TMCL_RESULT_CHECKSUM_ERROR= 2;

//Result status
static int  TMCL_STATUS_SUCC= 100;
static int  TMCL_STATUS_PROG_OK= 101;
static int  TMCL_STATUS_WRONG_CHECKSUM= 1;
static int  TMCL_STATUS_INVALID_COMMAND= 2;
static int  TMCL_STATUS_INVALID_VALUE= 4;
static int  TMCL_STATUS_WRONG_TYPE= 3;
static int  TMCL_STATUS_EEPR_LOCK= 5;
static int  TMCL_STATUS_CMD_NOTAVAIABLE= 6;

public static Byte TMCL_PWMD_0_Bank = 1;
public static Byte TMCL_PWMD_0_Index = 0;
public static Byte TMCL_PWMD_1_Bank = 2;
public static Byte TMCL_PWMD_1_Index = 0;
public static Byte TMCL_PWMD_2_Bank = 3;
public static Byte TMCL_PWMD_2_Index = 0;

public static Byte TMCL_PWMU_0_Bank = 2;
public static Byte TMCL_PWMU_0_Index = 2;
public static Byte TMCL_PWMU_1_Bank = 3;
public static Byte TMCL_PWMU_1_Index = 2;
public static Byte TMCL_PWMU_2_Bank = 4;
public static Byte TMCL_PWMU_2_Index = 2;

public static Byte TMCL_OD_1_Bank = 0;
public static Byte TMCL_OD_1_Index = 2;
public static Byte TMCL_OD_2_Bank = 1;
public static Byte TMCL_OD_2_Index = 2;

};
    
    
    public  class CTMCLCtrl
    { 
        protected SerialPort TMCLPort = new SerialPort();
        static Mutex MTComm = new Mutex();
        
        public bool Init(String ComPort)
        {
            TMCLPort.PortName = ComPort;
            TMCLPort.BaudRate = 9600;
            TMCLPort.DataBits = 8;
            TMCLPort.Parity = Parity.None;
            TMCLPort.StopBits = StopBits.One;
            TMCLPort.Open();
            return TMCLPort.IsOpen;
        }
        public int Connect()
        {
            return 0;
        }
        protected int SendCmd(Byte Address, Byte Command, Byte Type, Byte Motor, int Value)
        {
            byte[] TxBuffer = new byte[9];
	        
	        int i;
            byte[] BVal = System.BitConverter.GetBytes(Value);       
	        TxBuffer[0]=Address;
	        TxBuffer[1]=Command;
	        TxBuffer[2]=Type;
	        TxBuffer[3]=Motor;
            TxBuffer[4] = BVal[3];
            TxBuffer[5] = BVal[2];
            TxBuffer[6] = BVal[1];
            TxBuffer[7] = BVal[0];
	        TxBuffer[8]=0;
	        for(i=0; i<8; i++)
		        TxBuffer[8]+=TxBuffer[i];

            //MTComm.WaitOne();
            TMCLPort.Write(TxBuffer, 0, 9);

          

            for(int n = 0; n < 8;++n)
            {
                Thread.Sleep(20);
                if (TMCLPort.BytesToRead >= 9)
                    break;
            }

            byte[] resbuff = new byte[255];
            int res = TMCLPort.Read(resbuff, 0, 255);
           
            if (res >= 9)
            {
                 int state = resbuff[2];
                 if (state >= 100)
                 {
                     return 0;
                 }
            }
	
	       /* int res = 0;
	        for (int j=0; j<3; j++)
	        {
		        Thread.Sleep(50);
                byte[] resbuff = new byte[255];
                res = TMCLPort.Read(resbuff, 0, 255);
		        if (res > 3)
		        {
                    int state = resbuff[2];
                    if(state >= 100)
                    {
                        MTComm.ReleaseMutex();
                        return 0;
                    }
                    else
                   {
                        MTComm.ReleaseMutex();
                        return 1;
                   }
		        }
	        }
            MTComm.ReleaseMutex();
            * */
	        //Send the datagram
            return 1;
        }
        int GetResult(Byte Address, Byte Command, Byte Type, Byte Motor, ref int Value)
        {
            byte[] TxBuffer = new byte[9];
            Byte Checksum;
            
	            
	            int i;
	
	            TxBuffer[0]=Address;
	            TxBuffer[1]=Command;
	            TxBuffer[2]=Type;
	            TxBuffer[3]=Motor;
	            TxBuffer[4]= 0;
	            TxBuffer[5]= 0;
	            TxBuffer[6]= 0;
	            TxBuffer[7]= 0;
	            TxBuffer[8]=0;
	            for(i=0; i<8; i++)
		            TxBuffer[8]+=TxBuffer[i];
              //  MTComm.WaitOne();
	            TMCLPort.Write(TxBuffer, 0, 9);

                for (int n = 0; n < 8; ++n)
                {
                    Thread.Sleep(20);
                    if (TMCLPort.BytesToRead >= 9)
                        break;
                }

                byte[] resbuff = new byte[255];
                int res = TMCLPort.Read(resbuff, 0, 255);

                if (res >= 9)
                {
                    int state = resbuff[2]; 
                    
                    Byte[] rv = new byte[4];
                    for (int s = 0; s < 4; s++)
                    {
                        rv[s] = resbuff[7 - s];
                    }
                    Value = BitConverter.ToInt32(rv, 0);
                    return 0;
                    //if (state == 100)
                    //{
                    //    TableCtl.Log("气缸下压到位。");
                    //}
          
                }

             /*   int res = 0;
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(50);
                    byte[] resbuff = new byte[255];
                    res = TMCLPort.Read(resbuff, 0, 255);
                    if (res > 3)
                    {
                        int state = resbuff[2];
			            {
                            Byte[] rv = new byte[4];
                            for (int s = 0; s < 4; s++)
                            {
                                rv[s] = resbuff[7 - s];
                            }
                            Value = BitConverter.ToInt32(rv, 0);
                            MTComm.ReleaseMutex();
				            return 0;
			            }
		            }
		            else if (res <=3)
		            {
                        MTComm.ReleaseMutex();
                        return 1;//失败
		            }
			
	            }
                MTComm.ReleaseMutex();*/
	            return 1;//失败

	       
        }

        public int AbsMoveAxis(Byte Address, Byte axis, int value)
        {
	        return SendCmd((byte)Address, TMCL_Define.TMCL_MVP, TMCL_Define.MVP_ABS, (byte)axis, value);
        }
        public int RelMoveAxis(Byte Address, Byte axis, int value)
        {
	        return SendCmd(Address, TMCL_Define.TMCL_MVP, TMCL_Define.MVP_REL, axis, value);
        }
        public int StopAxis(Byte Address, Byte axis)
        {
	        return SendCmd(Address, TMCL_Define.TMCL_MST, 0, axis, 0);
        }
        public int SetVelAxis(Byte Address, Byte axis, int value)
        {
	        return SendCmd(Address, TMCL_Define.TMCL_SAP, 4, axis, value);
        }
        public int SetAccAxis(Byte Address, Byte axis, int value)
        {
	        return SendCmd(Address, TMCL_Define.TMCL_SAP, 5, axis, value);
        }
        public int GetGlobal(Byte Address, Byte bank,Byte Index, ref int value)
        {
            return GetResult(Address, TMCL_Define.TMCL_GGP, bank, Index, ref value);
        }
        public int SetGlobal(Byte Address, Byte bank, Byte Index, int value)
        {
            return SendCmd(Address, TMCL_Define.TMCL_SGP, bank, Index, value);
        }
        public int SetIO(Byte Address, Byte bank, Byte axis, int value)
        {
	        return SendCmd(Address, TMCL_Define.TMCL_SIO, bank, axis, value);
        }
        public int GetIO(Byte Address, Byte bank, Byte axis, ref int value)
        {
	        return GetResult(Address, TMCL_Define.TMCL_GIO, bank, axis, ref value);
        }
        public int GetInPos(Byte Address, Byte axis)
        {
            int value = 0;
            if (GetResult(Address, TMCL_Define.TMCL_GAP, 8, axis, ref value) == 0)
            {
                return value;
            }
            return 0;
        }
        public int Home(Byte Address, Byte axis)
        {
            SendCmd(Address, TMCL_Define.TMCL_SAP, 193, axis, 1);
            return SendCmd(Address, TMCL_Define.TMCL_RFS, 0, axis,0);
        }
        public int HomeCpt(Byte Address, Byte axis, ref int value)
        {
            return GetResult(Address, TMCL_Define.TMCL_RFS, 2, axis, ref value);
        }
        public int GetPos(Byte Address, Byte axis, ref int value)
        {
            return GetResult(Address, TMCL_Define.TMCL_GAP, 1, axis, ref value);
        }
        public int SetZero(Byte Address, Byte axis)
        {
            return SendCmd(Address, TMCL_Define.TMCL_SAP, 1, axis, 0);
        }
        
    };


}