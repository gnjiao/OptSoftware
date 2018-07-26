using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace opt_Weld_identification
{
    class C_3AxisTable
    {
        public int XScale;
        public int YScale;
        public int RScale;
        public int TScale;
        public int TScale2;
        public int TScale3;
        public int TScale4;
        public double XVel;
        public double YVel;
        public double RVel;
        public double TVel;
        public double XAcc;
        public double YAcc;
        public double RAcc;
        public double TAcc;

        public double XErrorL1;
        public double YErrorL1;
        public double RErrorL1;
        public double XErrorL2;
        public double YErrorL2;
        public double RErrorL2;
        public double XErrorR2;
        public double YErrorR2;
        public double RErrorR2;
        public double XErrorR1;
        public double YErrorR1;
        public double RErrorR1;
        public double XErrorSN;
        public double YErrorSN;
        public double RErrorSN;
        public double XDefPos;
        public double YDefPos;
        public double RDefPos;
        public double TDistance;


        protected CTMCLCtrl TMCLCtl = new CTMCLCtrl();

        
        public int Init(String ComPort)
        {
            try
            {
                if (TMCLCtl.Init(ComPort))
                    return 0;
                else
                {
                    MessageBox.Show("3轴控制卡连接失败！");
                    return 1;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("3轴控制卡连接失败！");
                return 1;
            }
        }
        public int SetParam()
        {
            //设置各个轴电机速度？
            TMCLCtl.SetAccAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, (int)(XAcc));
            TMCLCtl.SetAccAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, (int)(YAcc));
            TMCLCtl.SetAccAxis(UnitDefine.TAxis_Addr1, UnitDefine.TAxis_Idx1, (int)(TAcc));
            TMCLCtl.SetAccAxis(UnitDefine.TAxis_Addr2, UnitDefine.TAxis_Idx2, (int)(TAcc));

            TMCLCtl.SetVelAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, (int)(XVel));
            TMCLCtl.SetVelAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, (int)(YVel));
            TMCLCtl.SetVelAxis(UnitDefine.TAxis_Addr1, UnitDefine.TAxis_Idx1, (int)(TVel));
            TMCLCtl.SetVelAxis(UnitDefine.TAxis_Addr2, UnitDefine.TAxis_Idx2, (int)(TVel));
            TMCLCtl.SetVelAxis(UnitDefine.TAxis_Addr3, UnitDefine.TAxis_Idx3, (int)(TVel));
            TMCLCtl.SetVelAxis(UnitDefine.TAxis_Addr4, UnitDefine.TAxis_Idx4, (int)(TVel));
            TMCLCtl.SetVelAxis(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, (int)(RVel*RScale));
            WeldMotorSetVel((int)(1));
            return 0;
        }
        public int MoveAbsXYR(double Xval, double Yval, double Rval)
        {
            Xval = Xval * XScale;
            int res = TMCLCtl.AbsMoveAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, (int)Xval);

            Yval = Yval * YScale;
            res &= TMCLCtl.AbsMoveAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, (int)Yval);

            //Rval = Rval * RScale;
            //res &= TMCLCtl.AbsMoveAxis(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, (int)Rval);
            return res;
        }
        public int MoveRefXYR(double Xval, double Yval, double Rval)
        {
            Xval = Xval * XScale;
            int res = TMCLCtl.RelMoveAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, (int)Xval);

            Yval = Yval * YScale;
            res &= TMCLCtl.RelMoveAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, (int)Yval);

            Rval = Rval * RScale;
            res &= TMCLCtl.RelMoveAxis(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, (int)Rval);
            return res;
        
        }
        public int InPosX()//到位
        {
            int val = 0;
            if (TMCLCtl.GetIO(UnitDefine.XAxis_Addr, TMCL_Define.TMCL_PWMD_0_Bank, TMCL_Define.TMCL_PWMD_0_Index, ref val) == 0)
            {
      //          Log("InPosX" + val.ToString());
                return val;
            }
            return 0;
            
        }
        public int InPosY()//到位
        {
            int val = 0;
            if (TMCLCtl.GetIO(UnitDefine.YAxis_Addr, TMCL_Define.TMCL_PWMD_1_Bank, TMCL_Define.TMCL_PWMD_1_Index, ref val) == 0)
            {
       //         Log("InPosY" + val.ToString());
                return val;
            }
            return 0;

        }
        public int InPosR()//到位
        {
            int val = 0;
            if (TMCLCtl.GetInPos(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx) == 1)
            {
                Log("InPosR" + val.ToString());
                return 1;
            }
            return 0;

        }

        public int InPosXYR()//到位
        {
            if ((InPosX() == 1) && (InPosY() == 1) && (InPosR() == 1))
                return 1;
            return 0;
        }
        public int MoveRefX(double Xval)
        {
            Xval = Xval * XScale;
            return TMCLCtl.RelMoveAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, (int)Xval);
        }
        public int MoveRefY(double Yval)
        {
            Yval = Yval * YScale;
            return TMCLCtl.RelMoveAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, (int)Yval);
        }
        public int MoveRefR(double Rval)
        {
            Rval = Rval * RScale;
            return TMCLCtl.RelMoveAxis(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, (int)Rval);
        }
        public int MoveAbsR(double Rval)
        {
            Rval = Rval * RScale ;
            return TMCLCtl.AbsMoveAxis(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, (int)Rval);
        }
        public int MoveRefT(double Tval)
        {
            double Tval1 = Tval * TScale;
            TMCLCtl.RelMoveAxis(UnitDefine.TAxis_Addr1, UnitDefine.TAxis_Idx1, (int)Tval1);

            double Tval2 = Tval * TScale2;
            TMCLCtl.RelMoveAxis(UnitDefine.TAxis_Addr2, UnitDefine.TAxis_Idx2, (int)Tval2);

            double Tval3 = Tval * TScale3;
            TMCLCtl.RelMoveAxis(UnitDefine.TAxis_Addr3, UnitDefine.TAxis_Idx3, (int)Tval3);

            double Tval4 = Tval * TScale4;
            return TMCLCtl.RelMoveAxis(UnitDefine.TAxis_Addr4, UnitDefine.TAxis_Idx4, (int)Tval4);
        }

        public int Home()
        {
            TMCLCtl.SetVelAxis(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, 20);
            TMCLCtl.SetVelAxis(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, 20);
            WeldMotorUp();
            TMCLCtl.SetIO(UnitDefine.XAxis_Addr, TMCL_Define.TMCL_PWMU_0_Bank, TMCL_Define.TMCL_PWMU_0_Index, 0);
            TMCLCtl.SetIO(UnitDefine.YAxis_Addr, TMCL_Define.TMCL_PWMU_2_Bank, TMCL_Define.TMCL_PWMU_2_Index, 0);
            Thread.Sleep(200);
            TMCLCtl.SetIO(UnitDefine.XAxis_Addr, TMCL_Define.TMCL_PWMU_0_Bank, TMCL_Define.TMCL_PWMU_0_Index, 1);
            TMCLCtl.SetIO(UnitDefine.YAxis_Addr, TMCL_Define.TMCL_PWMU_2_Bank, TMCL_Define.TMCL_PWMU_2_Index, 1);
            Thread.Sleep(200);
            TMCLCtl.SetIO(UnitDefine.XAxis_Addr, TMCL_Define.TMCL_PWMU_0_Bank, TMCL_Define.TMCL_PWMU_0_Index, 0);
            TMCLCtl.SetIO(UnitDefine.YAxis_Addr, TMCL_Define.TMCL_PWMU_2_Bank, TMCL_Define.TMCL_PWMU_2_Index, 0);
            Thread.Sleep(500);
        //    Log("回零");
            return 0;
        }
        public int HomeCpt()
        {

            int val = 0;
            if (TMCLCtl.GetIO(1, TMCL_Define.TMCL_PWMD_0_Bank, TMCL_Define.TMCL_PWMD_0_Index, ref val) == 0)
                return val;
            return 0;
        }
        public int HomeClear()
        {
            TMCLCtl.SetZero(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx);
            TMCLCtl.SetZero(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx);
      //      Log("回零清零");
            return 0;
        }
        public int GetPosX(ref double value)
        {
            int ivalue = 0;
            if (TMCLCtl.GetPos(UnitDefine.XAxis_Addr, UnitDefine.XAxis_Idx, ref ivalue) == 0)
            {
                value = (double)ivalue / XScale;
                return 0;
            }
            return 1;
        }
        public int GetPosR(ref double value)
        {
            int ivalue = 0;
            if (TMCLCtl.GetPos(UnitDefine.RAxis_Addr, UnitDefine.RAxis_Idx, ref ivalue) == 0)
            {
                value = (double)ivalue / RScale;
                return 0;
            }
            return 1;
        }
        public int GetPosY(ref double value)
        {
            int ivalue = 0;
            if (TMCLCtl.GetPos(UnitDefine.YAxis_Addr, UnitDefine.YAxis_Idx, ref ivalue) == 0)
            {
                value = (double)ivalue / YScale;
                return 0;
            }
            return 1;
        }
        public int WeldAir(int flag)//原气缸下压信号，现无用
        {

            int val = 1;
            if (TMCLCtl.SetIO(1, TMCL_Define.TMCL_PWMU_1_Bank, TMCL_Define.TMCL_PWMU_1_Index, flag) == 0)
                return val;
            return 1;
        }
        public int WeldMotorDown()
        {

            int val = 0;
            if (TMCLCtl.SetGlobal(2, 0,2, 1) == 0)
                return val;
            return 1;
        }
        public int WeldMotorUp()
        {
            int val = 0;
            if (TMCLCtl.SetGlobal(2, 1, 2, 1) == 0)
                return val;
            return 1;
        }
        public int WeldMotorCpt()
        {

            int val = 0;
            if (TMCLCtl.GetGlobal(2, 21, 2, ref val) == 0)
                return val;
            return 0;
        }
        public int WeldMotorSet(int flag)
        {

            int val = 0;
            //if (TMCLCtl.SetGlobal(2, 2,2, flag*RScale) == 0)
            //    return val;
            return 1;
        }
        public int WeldMotorSetVel(double Val)
        {

            int val = Math.Abs((int)Val*RScale);
            if (TMCLCtl.SetGlobal(2, 2,2, val) == 0)
                return 0;
            return 1;
        }
        public void Log(string contents)
        {
            try
            {
                DateTime  _Begin = DateTime.Now;
                //获取时间
                DateTime _DATATIME_ = DateTime.Now;//20:16:16
                string _DATATIME_M_ = _DATATIME_.ToString("yyyy-MM-dd HH:mm:ss:fff ");
               // contents = DateTime.Now.ToShortTimeString() + "    " + contents + "\n\r";
                contents = _DATATIME_M_ + "    " + contents + "\r\n";
                System.IO.File.AppendAllText(Application.StartupPath + "\\RunLog.txt", contents);
            }
            catch (Exception e)
            {
                //System::Windows::Forms::MessageBox::Show("证书打开失败");
            }

        }
    }
}
