using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opt_Weld_identification
{
    class UnitDefine
    {
        //PLC通讯协议：张
        public static string IO_D37 = "D37";

        //研华IO卡
        //public static int DO_Redo_Air = 7;
    //    public static int DO_Red_Light = 5;//报警信号M37
        public static int DO_LineOutOK = 6;//允许流出信号M38
        public static int DO_LineOutNG = 7;//允许流出信号M39
        //public static int DO_Green_Light = 0;
        public static int DO_Complet = 3;//焊接完成信号M35
        public static int DO_StartWeld = 0;//预留高频焊加热M32
        //public static int DO_WeldEmergency = 1;
        public static int DO_Blowing_Cooling = 4;//冷却吹起M36
        //public static int DO_Vis_Light2 = 6;
        public static int DO_BusPress = 2;//汇流条压针M34
  //      public static int DO_WeldDown = 9;
        public static int DO_FlowOver = 8;
        public static int DO_FixedAir = 1;//接线盒固定气缸 m33
        //public static int DO_Red_Light = 4;
        //报警走PLC
        public static int DI_X_Alarm = 1;//报警m73
        public static int DI_Y_Alarm = 2;//m74
        public static int DI_Module_InPos = 0;//组件到位M72
        public static int DI_PLC_Alarm = 4;//plc报警m76
        public static int DI_SafeSensor = 3;//门开关m75
        public static int DI_Left_SolderingTin = 5;//门开关m77
        public static int DI_Right_SolderingTin = 6;//门开关m78
        public static int DI_PressBox_Up_Air = 7;//M79 给下压接线盒原位信号
        public static int DI_PressBox_Down_Air = 8;//M80 给下压接线盒到位信号 
        //
        public static int DI_Emergency_stop = 9;//获取设备急停状态M81
        public static int DO_str_alert = 5;//报警信号M37

        //轴
        public static Byte XAxis_Addr = 1;
        public static Byte XAxis_Idx = 1;

        public static Byte YAxis_Addr = 1;
        public static Byte YAxis_Idx = 2;

        public static Byte RAxis_Addr = 2;
        public static Byte RAxis_Idx = 1;

        public static Byte TAxis_Addr1 = 1;
        public static Byte TAxis_Idx1 = 0;
        public static Byte TAxis_Addr2 = 2;
        public static Byte TAxis_Idx2 = 0;
        public static Byte TAxis_Addr3 = 3;
        public static Byte TAxis_Idx3 = 0;
        public static Byte TAxis_Addr4 = 3;
        public static Byte TAxis_Idx4 = 1;
    }
}
