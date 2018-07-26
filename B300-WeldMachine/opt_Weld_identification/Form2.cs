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
//XML文件
using System.Xml;
//包含着ArrayList，Hashtable，SortedList这三个类
using System.Collections;
//线程
using System.Threading;
//图像转换
using System.Drawing.Imaging;
using HalconDotNet;

//系统参数保存命名空间
using opt_Weld_identification.Properties;

//
using Common;

namespace opt_Weld_identification
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            using (CheckTime t = new CheckTime("打开软件！"))
            {
                InitializeComponent();
            }
        }
        private String[] CamVector = new String[8];
        VisionParam_JK VisionParam_JKObject = new VisionParam_JK();
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Read_SN_XmlNode("sn_config.config");//条码参数
                Read_Z2_Positive_XmlNode("Z2_Positive_Config.config");//正面焊后检测参数读取
                Read_hq_zm1_recognition_XmlNode("hqzm1_recognition.config");//读取晶科焊接模式2参数
                Read_Location_recognition_XmlNode("Q_Location_recognition.config");
                Read_zm1_Location_recognition_XmlNode("zm1_Location_recognition.config");

                //寿命计时按钮
                btnLifeCount.Enabled = false;
                //设置按钮上的图像
                启动.Image = global::opt_Weld_identification.Properties.Resources.启动1;
                停止.Image = global::opt_Weld_identification.Properties.Resources.停止2;
                退出.Image = global::opt_Weld_identification.Properties.Resources.退出1;
                //按钮功能
                启动.Enabled = true;
                停止.Enabled = false;
                退出.Enabled = true;

                using (CheckTime t = new CheckTime("配置默认参数所需时间！"))
                {
                    //晶科
                    tabPage5.Parent = null;
                    //英利
                    tabPage16.Parent = null;
                    //协鑫
                    tabPage4.Parent = null;
                    //JinKO
                    tabPage22.Parent = null;

                    //修改20180205
                    //隐藏或显示厂家焊后正面识别调试界面
                    if (ImgProcess.Z2_CcbRecognize == 0)//晶科
                    {
                        //隐藏焊后正面识别调试界面
                        //晶科
                        this.tabPage5.Parent = this.tabControl2;//显示
                        //英利
                        tabPage16.Parent = null;
                        //协鑫
                        tabPage4.Parent = null;
                        //JinKO识别
                        tabPage22.Parent = null;
                        cbRecognize.Text = "晶科识别";
                    }
                    else if (ImgProcess.Z2_CcbRecognize == 1)
                    {
                        //隐藏焊后正面识别调试界面
                        //晶科
                        tabPage5.Parent = null;
                        //英利
                        tabPage16.Parent = null;
                        //协鑫
                        this.tabPage4.Parent = this.tabControl2;//显示
                        //JinKO识别
                        tabPage22.Parent = null;
                        cbRecognize.Text = "协鑫识别";
                    }
                    else if (ImgProcess.Z2_CcbRecognize == 2)
                    {
                        //隐藏焊后正面识别调试界面
                        //晶科
                        tabPage5.Parent = null;
                        //英利
                        this.tabPage16.Parent = this.tabControl2;//显示
                        //协鑫
                        tabPage4.Parent = null;
                        //JinKO识别
                        tabPage22.Parent = null;
                        cbRecognize.Text = "英利识别";
                    }
                    else if (ImgProcess.Z2_CcbRecognize == 3)
                    {
                        //隐藏焊后正面识别调试界面
                        //晶科
                        tabPage5.Parent = null;
                        //英利
                        tabPage16.Parent = null;
                        //协鑫
                        tabPage4.Parent = null;
                        //JinKO识别
                        this.tabPage22.Parent = this.tabControl2;//显示
                        cbRecognize.Text = "JinKO识别";
                    }




                    //
                    ImgProcess.start_form2 = true;//焊后正面参数初始化等待标记


                    //*******************初始化PLC*********************
                    端口号.Text = Settings.Default.PortNumber_;//COM

                    //波特率.Text = Settings.Default.BaudRate_;//
                    选择PLC类型.Text = Settings.Default.PLC_CpuType_;//

                    //PLC_Open();
                    //plc_com_init();
                    //******************色差参数读取*******************
                    //相机端口号.Value = Convert.ToInt32(Settings.Default.Camera_port_);//
                    //Camera_ID = 相机端口号.Value.ToString();//
                    //*******************测试相机**********************
                    //camera_init();
                    maskedTextBox1.Text = Settings.Default.Image_save_;//图像保存地址
                    maskedTextBox2.Text = Settings.Default.user_save_read_;//用户配置文件保存地址


                    _INIT_GenEmptyObj();
                    ReadXmlNode(maskedTextBox2.Text);//
                    //Read_hq_zm1_recognition_XmlNode("hqzm1_recognition.config");//读取晶科焊接模式2参数

                    XmlHelper.xmlFileName = "system_Config.xml";

                    CamVector[0] = XmlHelper.GetValueString("Values1", "value0", "Cam1");
                    CamVector[1] = XmlHelper.GetValueString("Values1", "value0", "Cam2");
                    CamVector[2] = XmlHelper.GetValueString("Values1", "value0", "Cam3");
                    CamVector[3] = XmlHelper.GetValueString("Values1", "value0", "Cam4");
                    CamVector[4] = XmlHelper.GetValueString("Values1", "value0", "Cam5");

                    tabControl1.SelectedIndex = 1;
                    tabControl1.SelectedIndex = 2;
                    tabControl1.SelectedIndex = 0;



                    propertyGrid_VisionParam.PropertySort = PropertySort.Categorized;
                    propertyGrid_VisionParam.SelectedObject = VisionParam_JKObject;


                    //新的软件
                    if (TableCtl.Init(端口号.Text) == 0)//控制卡？
                        通讯打开标志位 = true;
                    //PLC
                    //FXPlc2IO.Init(选择PLC类型.Text, 9600, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.One);
                    bool red_plc = FXPlc2IO._init(选择PLC类型.Text, 9600, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.One);
                    if (red_plc)
                    {
                        PLC连接状态1.ForeColor = Color.Black;
                        PLC连接状态1.Text = "PLC：FX";
                    }
                    else
                    {
                        MessageBox.Show("通讯端口打开失败！");
                    }

                    ImgProcess.zm1_soldering_HWindow = hWindowControl6;//焊前正面检测
                    HOperatorSet.SetDraw(ImgProcess.zm1_soldering_HWindow.HalconWindow, "margin");

                    ImgProcess.HPosWindow = hWindowControl1;//定位
                    hTplCrt.HTplWindow = ImgProcess.HPosWindow;//
                    HOperatorSet.SetDraw(ImgProcess.HPosWindow.HalconWindow, "margin");

                    ImgProcess.Q_lr_HWindow = hWindowControl7;//焊接前侧面检测,有无按压汇流条
                    HOperatorSet.SetDraw(ImgProcess.Q_lr_HWindow.HalconWindow, "margin");

                    ImgProcess.Z2_Positive_HWindow = hWindowControl8;//焊后正面检测
                    HOperatorSet.SetDraw(ImgProcess.Z2_Positive_HWindow.HalconWindow, "margin");

                    //     ImgProcess.C2_Side_HWindow = hWindowControl9;//焊后侧面检测

                    ImgProcess.SN_HWindow = hWindowControl10;//条码窗口
                    HOperatorSet.SetDraw(ImgProcess.SN_HWindow.HalconWindow, "margin");

                    //细节窗口
                    ImgProcess.zm1_HWindow = ZM1_hWindow;//定位
                    ImgProcess.Q_cm_HWindow = CM1_hWindow;//焊前侧面

                    //修改20180319
                    //CM1_hWindow = hWindowControl7;

                    ImgProcess.zm2_HWindow = ZM2_hWindow;//焊后正面
                    ImgProcess.sn_HWindow = SN_hWindow;//条码
                    //
                    ImgProcess.TplPath = _模板目录;
                    //
                    if (!ImgProcess.Init(CamVector))
                    {
                        toolStripStatusLabel6.Text = "未连接";
                        MessageBox.Show("相机初始化失败！");
                    }
                    else
                    {
                        toolStripStatusLabel6.Text = "已连接";
                    }
                    //
                    ImgProcess.VisionParam_JKObject = VisionParam_JKObject;
                    hTplCrt.CurX = ImgProcess.TPLXDef;
                    hTplCrt.CurY = ImgProcess.TPLYDef;
                    //IOCtl.WriteDOPort(0, UnitDefine.DO_WeldDown, false);
                    //       FXPlc2IO.SetDO(UnitDefine.DO_WeldDown, false);
                    //焊接流程总线程
                    Thread myThread_1 = new Thread(new ThreadStart(RefreshProcess));
                    myThread_1.Start();

                    #region //创建目录 张吉良老Code
                    ////获取日期
                    //string 日期 = DateTime.Now.ToLongDateString().ToString();// 2014年11月09日
                    //目录 = maskedTextBox1.Text + "\\" + 日期;
                    ////
                    //ImgProcess._Save_image_directory_ = 目录;
                    ////
                    //ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
                    ////   创建目录(目录);
                    #endregion

                    #region //创建目录 区分白班夜班
                    //启动定时器（定时创建文件夹）
                    tmrPath.Start();
                    //获取当前小时
                    int JudgHour = DateTime.Now.Hour;
                    if (JudgHour >= 8 && JudgHour <= 20)
                    {
                        //对当前日期进行格式化
                        string strName = DateTime.Now.ToString("yyyy年MM月dd日 白班");
                        //创建DirectoryInfo对象
                        //DirectoryInfo DInfo = new DirectoryInfo(maskedTextBox1.Text + strName);
                        ImgProcess._Save_image_directory_ = maskedTextBox1.Text + "\\" + strName;
                        //判断白班文件夹是否存在
                        if (Directory.Exists(@"maskedTextBox1.Text + strName"))
                        {
                            return;
                        }
                        else
                        {
                            //创建文件夹
                            //DInfo.Create();
                            ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
                        }
                    }
                    else if (JudgHour < 8)
                    {
                        //DateTime.Now.AddDays(-1) .ToString("yyyy-MM-dd HH:mm:ss");
                        //对当前日期进行格式化
                        string strName = DateTime.Now.AddDays(-1).ToString("yyyy年MM月dd日 夜班");
                        //创建DirectoryInfo对象
                        //DirectoryInfo DInfo = new DirectoryInfo(maskedTextBox1.Text + strName);
                        ImgProcess._Save_image_directory_ = maskedTextBox1.Text + "\\" + strName;
                        //判断夜班文件夹是否存在
                        if (Directory.Exists(@"maskedTextBox1.Text + strName"))
                        {
                            return;
                        }
                        else
                        {
                            //创建文件夹
                            //DInfo.Create();
                            ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
                        }
                    }

                    else
                    {
                        //对当前日期进行格式化
                        string strName = DateTime.Now.ToString("yyyy年MM月dd日 夜班");
                        //创建DirectoryInfo对象
                        //DirectoryInfo DInfo = new DirectoryInfo(maskedTextBox1.Text + strName);
                        ImgProcess._Save_image_directory_ = maskedTextBox1.Text + "\\" + strName;
                        //判断夜班文件夹是否存在
                        if (Directory.Exists(@"maskedTextBox1.Text + strName"))
                        {
                            return;
                        }
                        else
                        {
                            //创建文件夹
                            //DInfo.Create();
                            ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
                        }
                    }



                    #endregion

                    bWeldDebug = checkBox_WeldDebug.Checked;
                    //
                    //Read_Location_recognition_XmlNode("Q_Location_recognition.config");
                    //Read_zm1_Location_recognition_XmlNode("zm1_Location_recognition.config");
                    //
                    checkBox_WeldDebug.Checked = false;//取消调试模式
                    button_TABLE_PARAM_SET_Click(null, null);//参数应用
                    //Z2_Positive_Config.config


                    //
                    //
                    ImgProcess.start_form2 = false;//焊后正面参数初始化完成标记

                    //刷新下窗体
                    tabControl1.SelectedTab = tabControl1.TabPages[5];
                    tabControl1.SelectedTab = tabControl1.TabPages[4];
                    tabControl1.SelectedTab = tabControl1.TabPages[3];
                    tabControl1.SelectedTab = tabControl1.TabPages[2];
                    tabControl1.SelectedTab = tabControl1.TabPages[1];
                    tabControl1.SelectedTab = tabControl1.TabPages[0];

                    Read_ER_number_XmlNode("data_record.config");//读取并刷新界面数据
                    //
                    //Read_SN_XmlNode("sn_config.config");
                    Thread thread_Star_IO_PLC = new Thread(new ThreadStart(Star_IO_PLC));
                    thread_Star_IO_PLC.Start();
                    //   _star_read_plc = false;//直接启动PLC线程，一般用于和PLC通讯调试
                }
                //用户登录权限设置 20180604
                #region //调试界面
                //显示信息
                this.tabPage1.Parent = this.tabControl1_主设置;
                //显示参数设置
                this.tabPage7.Parent = this.tabControl1_主设置;
                //隐藏相机设置
                tabPage23.Parent = null;
                //隐藏焊接参数
                tabPage_rightcam.Parent = null;
                //隐藏定位平台
                TabP_Table.Parent = null;
                //隐藏通讯设置
                tabPage2.Parent = null;
                //隐藏定位模板
                tabPage_IO.Parent = null;
                //隐藏焊前正面
                tabPage17.Parent = null;
                //隐藏焊前侧面
                tabPage6.Parent = null;
                //隐藏焊后正面
                tabPage9.Parent = null;
                //隐藏焊后侧面
                tabPage10.Parent = null;
                //隐藏条码设置
                tabPage14.Parent = null;
                #endregion

                #region //显示窗口
                ////显示细节窗口
                //this.tabPage_Left.Parent = this.tabControl1;
                ////显示焊后正面窗口
                //this.tabPage11.Parent = this.tabControl1;
                ////隐藏正面图像定位窗口
                //tabPage3.Parent = null;
                // //隐藏正面焊前识别窗口
                //tabPage18.Parent = null;
                // //隐藏侧面焊前识别窗口
                //tabPage8.Parent = null;
                // //隐藏条码识别窗口
                //tabPage13.Parent = null;
                // //隐藏未使用窗口
                //tabPage12.Parent = null;
                #endregion

            }
            catch (Exception)
            {
            }
        }

        #region 创建目录
        private void 创建目录(string _目录_)
        {
            try
            {
                if (Directory.Exists(目录) == false)//如果不存在就创建file文件夹
                {
                    System.IO.Directory.CreateDirectory(目录);
                    System.IO.Directory.CreateDirectory(目录 + "\\OK");
                    System.IO.Directory.CreateDirectory(目录 + "\\NG");
                    System.IO.Directory.CreateDirectory(目录 + "\\ERROR");

                }

            }
            catch (Exception e)
            {
                MessageBox.Show("创建目录失败 " + 目录 + "    " + e.Message);
            }

        }
        #endregion
        #region 设置退出窗口无效
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion
        #region 退出
        private void 退出_Click(object sender, EventArgs e)
        {
            using (CheckTime t = new CheckTime("关闭软件！"))
            {
                _star_PLC = true;//关闭通讯线程
                bNewMainStop = true;//停止流程
                bNewMainTerminate = true;//关闭主线程
                //
                HOperatorSet.CloseAllFramegrabbers();
                //暂停
                JHCap.CameraStop(g_index);
                //释放相机
                JHCap.CameraFree(g_index);
                tmrPath.Stop();
            }
            Environment.Exit(1);

        }
        #endregion
        #region 系统初始化
        //**********************三菱PLC***********************
        //ActFXCPU ACTMFX = new ActFXCPU();
        //******************色差相机窗体*******************
        public static Weld_Form Weld_Form1;
        //******************用户登陆窗体*******************
        public static User_Form User_Form2;

        // Local iconic variables 
        HObject ho_MODELS_Image = null;

        HObject ho_Image = null, ho_ImageZoom1 = null, ho_ImageEmphasize = null;
        HObject ho_Red = null, ho_Green = null, ho_Blue = null, ho_ROI_0 = null, ho_ROI_1 = null, ho_RegionDifference = null;
        HObject ho_ImageROIRing = null, ho_PyramidImage = null, ho_ModelRegionRing = null;
        HObject ho_ShapeModelRing = null, ho_Hue = null, ho_Saturation = null;
        HObject ho_Intensity = null, ho_Regions = null, ho_ConnectedRegions1 = null;
        HObject ho_SelectedRegions = null, ho_RegionFillUp1 = null;
        HObject ho_ConnectedRegions = null;//, ho_ImageReduced = null;
        HObject ho_XLDTrans = null;//, ho_ModelID_Region = null;
        //HObject ho_CR = null;

        HObject ho_SelectedXLD = null, ho_UnionContours = null;

        HObject ho_Rectangle_X1 = null, ho_Rectangle_X2 = null, ho_Rectangle_X3 = null, ho_Rectangle_X4 = null;
        HObject ho_RegionUnion1 = null, ho_RegionUnion2 = null, ho_RegionUnion3 = null, ho_Contours = null;

        HObject ho_ID_Region1 = null;
        HObject ho_ID_Region2 = null;
        HObject ho_ContoursAffinTrans1 = null;
        HObject ho_ContoursAffinTrans2 = null;

        HObject ho_ContoursAffinTrans = null;
        //HObject ho_ID_Region = null;

        //新测试参数
        HObject ho_RegionDilation = null;
        //HObject ho_BinImage = null;

        // Local control variables 
        HTuple hv_ErrorNum = null, hv_ErrorNum1 = null, hv_ErrorNum2 = null, hv_ErrorNum3 = null, hv_ErrorNum4 = null;//返回错误信息

        HTuple hv_ImageName = new HTuple(), hv_AcqHandle = new HTuple();
        HTuple hv_ModelIDRing = new HTuple(), hv_NumContoursRing = new HTuple();
        HTuple hv_S1 = new HTuple();
        HTuple hv_RowCheck = new HTuple(), hv_ColumnCheck = new HTuple(), hv_AngleCheck = new HTuple();
        HTuple hv_Score1 = new HTuple(), hv_Model = new HTuple();
        HTuple hv_Number = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
        HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
        HTuple hv_Min_1 = new HTuple(), hv_Max_1 = new HTuple(), hv_TEMP_X = new HTuple();
        HTuple hv_IDX = new HTuple(), hv_Index2 = new HTuple(), hv_Min_2 = new HTuple();
        HTuple hv_Max_2 = new HTuple(), hv_Index1 = new HTuple();
        HTuple hv_S2 = new HTuple(), hv_Time = new HTuple();

        HTuple hv_opt_str = new HTuple();

        //
        HTuple hv_Row_X1 = new HTuple(), hv_Column_X1 = new HTuple(), hv_Phi_X1 = new HTuple(), hv_Length1_X1 = new HTuple(), hv_Length2_X1 = new HTuple();
        HTuple hv_Row_X2 = new HTuple(), hv_Column_X2 = new HTuple(), hv_Phi_X2 = new HTuple(), hv_Length1_X2 = new HTuple(), hv_Length2_X2 = new HTuple();
        HTuple hv_Row_X3 = new HTuple(), hv_Column_X3 = new HTuple(), hv_Phi_X3 = new HTuple(), hv_Length1_X3 = new HTuple(), hv_Length2_X3 = new HTuple();
        HTuple hv_Row_X4 = new HTuple(), hv_Column_X4 = new HTuple(), hv_Phi_X4 = new HTuple(), hv_Length1_X4 = new HTuple(), hv_Length2_X4 = new HTuple();

        HTuple hv_MovementOfObject0 = new HTuple(), hv_MovementOfObject1 = new HTuple();
        HTuple hv_MovementOfObject2 = new HTuple(), hv_MovementOfObject3 = new HTuple();

        //***************相机重连（等同于拔插一次相机）次数****************
        int _IMAGE_ERROR_USB_RST_ = 0;
        bool _IMAGE_DirectShow_ = false;//获取图像模式
        private void IMAGE_directshow_CheckedChanged(object sender, EventArgs e)
        {
            _IMAGE_DirectShow_ = IMAGE_directshow.Checked;
        }

        //Int32 _图像缩小比例_ = 2;

        //文本显示
        private string ADT_text_flag = null;
        private string ADT_text_ERROR = null;
        //***************相机端口****************
        private string Camera_ID = "0";

        //启动线程或允许检测标志
        private bool Colours_Thread = false;
        //启动相机错误标记
        private bool OK_ERROR_Camera = false;

        //*******************相机序号**********************
        private static int g_index = 0;
        //private static string JHSM500m = "";
        //图像分辨率
        private static int len = 0, R_width = 2592, R_height = 1944;

        private static string 目录 = "";

        //配置文件
        private string _配置文件_ = "default_setting.config";

        //private bool MEAN_FLAG = true;//检测标记

        private void _INIT_GenEmptyObj()
        {
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_MODELS_Image);

            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageZoom1);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_ROI_0);
            HOperatorSet.GenEmptyObj(out ho_ROI_1);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ImageROIRing);
            HOperatorSet.GenEmptyObj(out ho_PyramidImage);
            HOperatorSet.GenEmptyObj(out ho_ModelRegionRing);
            HOperatorSet.GenEmptyObj(out ho_ShapeModelRing);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_X1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_X2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_X3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_X4);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_Hue);
            HOperatorSet.GenEmptyObj(out ho_Saturation);
            HOperatorSet.GenEmptyObj(out ho_Intensity);

            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans);
            //HOperatorSet.GenEmptyObj(out ho_ID_Region);

            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans1);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffinTrans2);
            HOperatorSet.GenEmptyObj(out ho_ID_Region1);
            HOperatorSet.GenEmptyObj(out ho_ID_Region2);

            HOperatorSet.GenEmptyObj(out ho_XLDTrans);
            //HOperatorSet.GenEmptyObj(out ho_ModelID_Region);
            //HOperatorSet.GenEmptyObj(out ho_CR);

            HOperatorSet.GenEmptyObj(out ho_Regions);

            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);

            //新测试参数
            //HOperatorSet.GenEmptyObj(out ho_RegionClosingX);
            //HOperatorSet.GenEmptyObj(out ho_BinImage);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);


            HOperatorSet.SetDraw(hWindowControl1.HalconWindow, "margin");
            set_display_font(hWindowControl1.HalconWindow, 28, "mono", "true", "false");
            HOperatorSet.SetLineWidth(hWindowControl1.HalconWindow, 1);


            HOperatorSet.SetDraw(hWindowControl7.HalconWindow, "margin");
            set_display_font(hWindowControl7.HalconWindow, 28, "mono", "true", "false");
            HOperatorSet.SetLineWidth(hWindowControl7.HalconWindow, 1);


            HOperatorSet.SetDraw(hWindowControl8.HalconWindow, "margin");
            set_display_font(hWindowControl8.HalconWindow, 28, "mono", "true", "false");
            HOperatorSet.SetLineWidth(hWindowControl8.HalconWindow, 1);

            HOperatorSet.SetDraw(hWindowControl9.HalconWindow, "margin");
            set_display_font(hWindowControl9.HalconWindow, 28, "mono", "true", "false");
            HOperatorSet.SetLineWidth(hWindowControl9.HalconWindow, 1);

            HOperatorSet.SetDraw(hWindowControl10.HalconWindow, "margin");
            set_display_font(hWindowControl10.HalconWindow, 28, "mono", "true", "false");
            HOperatorSet.SetLineWidth(hWindowControl10.HalconWindow, 1);





            //HOperatorSet.SetDraw(hWindowControl2.HalconWindow, "margin");
            //set_display_font(hWindowControl2.HalconWindow, 41, "mono", "true", "false");

            //HOperatorSet.SetDraw(hWindowControl2.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(hWindowControl2.HalconWindow, 1);

            //HOperatorSet.SetDraw(hWindowControl3.HalconWindow, "margin");
            //set_display_font(hWindowControl3.HalconWindow, 41, "mono", "true", "false");

            //HOperatorSet.SetDraw(hWindowControl3.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(hWindowControl3.HalconWindow, 1);

            //HOperatorSet.SetDraw(hWindowControl4.HalconWindow, "margin");
            //set_display_font(hWindowControl4.HalconWindow, 41, "mono", "true", "false");

            //HOperatorSet.SetDraw(hWindowControl4.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(hWindowControl4.HalconWindow, 1);
            //HOperatorSet.SetDraw(hWindowControl5.HalconWindow, "margin");
            //set_display_font(hWindowControl5.HalconWindow, 41, "mono", "true", "false");

            //HOperatorSet.SetDraw(hWindowControl5.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(hWindowControl5.HalconWindow, 1);
        }

        #endregion
        #region 测试相机
        private string encryptComputer = string.Empty;
        private bool CheckRegist()
        {
            EncryptionHelper helper = new EncryptionHelper();
            string md5key = helper.GetMD5String(encryptComputer);
            return CheckRegistData(md5key);
        }
        private bool CheckRegistData(string key)
        {
            if (RegistFileHelper.ExistRegistInfofile() == false)
            {
                return false;
            }
            else
            {
                string info = RegistFileHelper.ReadRegistFile();
                var helper = new EncryptionHelper(EncryptionKeyEnum.KeyB);
                string registData = helper.DecryptString(info);
                if (key == registData)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        //***********************获取相机**********************
        public static string _ID_ = "";
        private void _IDERROR()
        {
            启动.Enabled = false;
            启动.BackgroundImage = opt_Weld_identification.Properties.Resources.启动2;
            停止.Enabled = false;
            停止.BackgroundImage = opt_Weld_identification.Properties.Resources.停止2;
            OK_ERROR_Camera = false;
            相机参数.Enabled = false;
            相机参数.BackgroundImage = opt_Weld_identification.Properties.Resources.相机2;
            tabControl1_主设置.Enabled = false;
        }
        //***********************获取相机**********************
        private void camera_init()
        {
            /*if (JHCap.CameraInit(g_index) == 0)//(ComboName_admin.Text == "JHSM500(0)")
            {
                byte[] ID = new byte[12];
                if (JHCap.CameraReadSerialNumber(g_index, ID, 12) == 0)
                {
                    _ID_ = Encoding.ASCII.GetString(ID);//byte转换string
                    string computer = ComputerInfo.GetComputerInfo(_ID_);
                    encryptComputer = new EncryptionHelper().EncryptString(computer);
                    if (CheckRegist() == true)
                    {
                        //获取分辨率
                        JHCap.CameraGetResolution(g_index, 0, ref R_width, ref R_height);
                        JHCap.CameraGetImageBufferSize(g_index, ref len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24

                        toolStripStatusLabel1_注册.Text = "注册信息：已注册";
                        toolStripStatusLabel6.Text = "相机:OK";
                        _手动检测.Enabled = true;
                        OK_ERROR_Camera = true;
                        //暂停
                        JHCap.CameraStop(g_index);
                        //释放相机
                        JHCap.CameraFree(g_index);

                    }
                    else
                    {
                        RegistFileHelper.WriteComputerInfoFile(encryptComputer);
                        _IDERROR();
                        toolStripStatusLabel1_注册.Text = "注册信息：未注册";
                    }
                }
                else
                {
                    toolStripStatusLabel1_注册.Text = "注册信息：Optech";
                    _IDERROR();
                    MessageBox.Show("相机初始化失败");

                }

            }
            else
            {
                toolStripStatusLabel1_注册.Text = "注册信息：Optech";
                _IDERROR();
                MessageBox.Show("相机初始化失败");

            }*/
        }
        #endregion

        #region IF_init

        private bool IF_init()
        {
            Cloce_Camera();

            bool CAMERA_ERROR = false;
            return false;
            // Error variable 'hv_ErrorNum' activated
            hv_ErrorNum = 2;
            if (_IMAGE_DirectShow_)
            {
                //HOperatorSet.CloseAllFramegrabbers();

                //open first camera device with current video settings:
                try
                {
                    //hv_ErrorNum = 2;
                    //HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", -1,
                    //  "default", -1, "false", "default", Camera_ID, -1, -1, out hv_AcqHandle);
                }
                catch (HalconException e)
                {
                    hv_ErrorNum = e.GetErrorNumber();
                    //if ((int)hv_ErrorNum < 0)
                    //    throw e;
                    //MessageBox.Show("启动相机失败/n" + e.Message);
                    return false;
                }
            }
            else
            {
                /* if (JHCap.CameraInit(g_index) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                 {
                     //高速模式
                     //JHCap.CameraSetHighspeed(g_index, true);
                     //JHCap.CameraSetDelay(g_index, 0);
                     JHCap.CameraSetSnapMode(g_index,JHCap.CAMERA_SNAP_CONTINUATION);
                     //获取分辨率
                     JHCap.CameraGetResolution(g_index, 0, ref R_width, ref R_height);
                     JHCap.CameraGetImageBufferSize(g_index, ref len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                 }
                 else
                 {
                     CAMERA_ERROR = true;
                 }*/
            }

            /* if (((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0) && (!CAMERA_ERROR))
             {
                 toolStripStatusLabel6.Text = "相机:OK";
                 OK_ERROR_Camera = true;

                 return true;
             }
             else
             {
                 Cloce_Camera();
                 if (_IMAGE_DirectShow_)
                 {
                     MessageBox.Show("启动相机失败\n" + "directshow_error");
                 }
                 else
                 {
                     MessageBox.Show("启动相机失败");
                 }

                 启动.Enabled = true;
                 启动.BackgroundImage = opt_Weld_identification.Properties.Resources.启动2;
                 //停止.Enabled = true;
                 //停止.BackgroundImage = opt_Weld_identification.Properties.Resources.停止2;
                 return false;
             }*/
        }
        #endregion
        private void 相机端口号_ValueChanged(object sender, EventArgs e)
        {
            Camera_ID = 相机端口号.Value.ToString();
        }

        #region 在窗体显示文本框及写入本文信息。
        //****************
        /// <summary> 
        /// 在窗体显示文本框及写入本文信息。 
        /// </summary> 
        /// <param name="hv_WindowHandle">显示文本框及写入本文信息字窗体名称</param> 
        /// <param name="hv_String">String显示内容</param> 
        /// <param name="hv_CoordSystem">CoordSystem显示格式（image\window）</param> 
        /// <param name="hv_Row">Row显示内容X行位置</param> 
        /// <param name="hv_Column">Column显示内容Y行位置</param> 
        /// <param name="hv_Color">Color显示内容颜色（red\black\green\blue\yellow)</param> 
        /// <param name="hv_Box">Box背景框（'true'显示，'false'关闭）</param> 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem, HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            // Local control variables 

            HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
            HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
            HTuple hv_ColumnWin, hv_WidthWin = new HTuple(), hv_HeightWin;
            HTuple hv_MaxAscent, hv_MaxDescent, hv_MaxWidth, hv_MaxHeight;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_CurrentColor = new HTuple();

            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            //prepare window 
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part, out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin, out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            // 
            //default settings 
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            // 
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            // 
            //Estimate extentions of text depending on font size. 
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent, out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //transform image to window coordinates 
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            // 
            //display text box depending on text size 
            if ((int)(new HTuple(hv_Box.TupleEqual("true"))) != 0)
            {
                //calculate box extents 
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //display rectangles 
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3,
                    hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if ((int)(new HTuple(hv_Box.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
            }
            //Write text. 
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength())));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual("auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(hv_Index));
            }
            //reset changed window settings 
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part, hv_Column2Part);

            return;
        }
        #endregion
        #region 设置窗口文本中的字体
        /// <summary> 
        /// 设置窗口文本中的字体 
        /// </summary> 
        /// <param name="hv_WindowHandle">设置字窗体名称</param> 
        /// <param name="hv_Size">设置字体大小</param> 
        /// <param name="hv_Font">设置字体类型（类型为mono\sans\serif\Courier）</param> 
        /// <param name="hv_Bold">设置字粗体（true粗,false不粗</param> 
        /// <param name="hv_Slant">设置字体：false不倾斜，true倾斜</param> 
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)
        {
            // Local control variables 

            HTuple hv_OS, hv_Exception = new HTuple();
            HTuple hv_AllowedFontSizes = new HTuple(), hv_Distances = new HTuple();
            HTuple hv_Indices = new HTuple();

            HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
            HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
            HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
            HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();

            HOperatorSet.GetSystem("operating_system", out hv_OS);
            if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
            {
                hv_Size_COPY_INP_TMP = 16;
            }
            if ((int)(new HTuple((((hv_OS.TupleStrFirstN(2)).TupleStrLastN(0))).TupleEqual("Win"))) != 0)
            {
                //set font on Windows systems 
                if ((int)((new HTuple((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))).TupleOr(
                    new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Courier New";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Arial";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Times New Roman";
                }
                if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = 1;
                }
                else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = 0;
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = 1;
                }
                else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = 0;
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(hv_Exception);
                }
            }
            else
            {
                //set font for UNIX systems 
                hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
                hv_AllowedFontSizes = new HTuple();
                hv_AllowedFontSizes[0] = 11;
                hv_AllowedFontSizes[1] = 14;
                hv_AllowedFontSizes[2] = 17;
                hv_AllowedFontSizes[3] = 20;
                hv_AllowedFontSizes[4] = 25;
                hv_AllowedFontSizes[5] = 34;
                if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
                    -1))) != 0)
                {
                    hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
                    HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
                    hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
                        0));
                }
                if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
                    "Courier")))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "courier";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "helvetica";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "times";
                }
                if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = "bold";
                }
                else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = "medium";
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = "i";
                    }
                    else
                    {
                        hv_Slant_COPY_INP_TMP = "o";
                    }
                }
                else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = "r";
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(hv_Exception);
                }
            }

            return;
        }
        #endregion
        #region 保存本地配置
        private void 保存本地配置(string _TEXT_CONFIG, string _VALUE_)
        {
            // 保存Applicationi 范围的设置 
            //"Application"是不明确的引用，与Excel.Application excel;冲突系统不知道应该引用那个。解决办法使用全名引用。
            string configFileName = System.Windows.Forms.Application.ExecutablePath + ".config";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(configFileName);
            //string configString = @"configuration/applicationSettings/opt_color_identification.Properties.Settings/setting[@name='" + _TEXT_CONFIG + "']/value";
            string configString = @"configuration/applicationSettings/opt_Weld_identification.Properties.Settings/setting[@name='" + _TEXT_CONFIG + "']/value";
            System.Xml.XmlNode configNode = doc.SelectSingleNode(configString);
            if (configNode != null)
            {
                configNode.InnerText = _VALUE_;
                doc.Save(configFileName);
                // 刷新应用程序设置，这样下次读取时才能读到最新的值。
                Properties.Settings.Default.Reload();
            }
        }
        #endregion
        #region 创建节点
        // 创建节点    
        // <param name="xmldoc"></param>  xml文档  
        // <param name="parentnode"></param>父节点    
        // <param name="name"></param>  节点名  
        // <param name="value"></param>  节点值   
        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }
        #endregion
        #region 创建多节点多层级的XML文件
        //创建多节点多层级的XML文件
        public void CreateXmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 模板定位参数
            XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "star_models", null);
            CreateNode(xmlDoc, node1, "_ROI_0_C1", ROI_0_C1.ToString());
            CreateNode(xmlDoc, node1, "_ROI_0_R1", ROI_0_R1.ToString());
            CreateNode(xmlDoc, node1, "_ROI_0_C2", ROI_0_C2.ToString());
            CreateNode(xmlDoc, node1, "_ROI_0_R2", ROI_0_R2.ToString());
            CreateNode(xmlDoc, node1, "_ROI_1_C1", ROI_1_C1.ToString());
            CreateNode(xmlDoc, node1, "_ROI_1_R1", ROI_1_R1.ToString());
            CreateNode(xmlDoc, node1, "_ROI_1_C2", ROI_1_C2.ToString());
            CreateNode(xmlDoc, node1, "_ROI_1_R2", ROI_1_R2.ToString());
            CreateNode(xmlDoc, node1, "_Model_Threshold_1_L", Model_Threshold_1_L.ToString());
            CreateNode(xmlDoc, node1, "_Model_Threshold_1_H", Model_Threshold_1_H.ToString());

            //1
            CreateNode(xmlDoc, node1, "_hv_Row_X1_", _hv_Row_X1.ToString());
            CreateNode(xmlDoc, node1, "_hv_Column_X1_", _hv_Column_X1.ToString());
            //CreateNode(xmlDoc, node1, "_hv_Phi_X1_", _hv_Phi_X1.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length1_X1_", _hv_Length1_X1.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length2_X1_", _hv_Length2_X1.ToString());
            //2
            CreateNode(xmlDoc, node1, "_hv_Row_X2_", _hv_Row_X2.ToString());
            CreateNode(xmlDoc, node1, "_hv_Column_X2_", _hv_Column_X2.ToString());
            //CreateNode(xmlDoc, node1, "_hv_Phi_X2_", _hv_Phi_X2.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length1_X2_", _hv_Length1_X2.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length2_X2_", _hv_Length2_X2.ToString());
            //3
            CreateNode(xmlDoc, node1, "_hv_Row_X3_", _hv_Row_X3.ToString());
            CreateNode(xmlDoc, node1, "_hv_Column_X3_", _hv_Column_X3.ToString());
            //CreateNode(xmlDoc, node1, "_hv_Phi_X3_", _hv_Phi_X3.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length1_X3_", _hv_Length1_X3.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length2_X3_", _hv_Length2_X3.ToString());
            //4
            CreateNode(xmlDoc, node1, "_hv_Row_X4_", _hv_Row_X4.ToString());
            CreateNode(xmlDoc, node1, "_hv_Column_X4_", _hv_Column_X4.ToString());
            //CreateNode(xmlDoc, node1, "_hv_Phi_X4_", _hv_Phi_X4.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length1_X4_", _hv_Length1_X4.ToString());
            CreateNode(xmlDoc, node1, "_hv_Length2_X4_", _hv_Length2_X4.ToString());

            //保存临时坐标参数
            _RCPLL_();

            root.AppendChild(node1);
            #endregion
            #region 模板与特征参数
            XmlNode node2 = xmlDoc.CreateNode(XmlNodeType.Element, "shape_models_MaxOverlap", null);
            CreateNode(xmlDoc, node2, "_Emphasize_1_X", Emphasize_1_X.ToString());
            CreateNode(xmlDoc, node2, "_Emphasize_1_Y", Emphasize_1_Y.ToString());
            CreateNode(xmlDoc, node2, "_Emphasize_1_Z", Emphasize_1_Z.ToString());

            CreateNode(xmlDoc, node2, "_MODEL_Threshold_L_", MODEL_Threshold_L.ToString());
            CreateNode(xmlDoc, node2, "_MODEL_AREA_L_", MODEL_AREA_L.ToString());
            CreateNode(xmlDoc, node2, "_MODEL_RegionDilation_", MODEL_RegionDilation.ToString());
            //CreateNode(xmlDoc, node2, "_MODEL_RegionToBin_L_", MODEL_RegionToBin_L.ToString());
            //CreateNode(xmlDoc, node2, "_MODEL_RegionToBin_H_", MODEL_RegionToBin_H.ToString());

            CreateNode(xmlDoc, node2, "_Model_XSD", _模板匹配相似度.ToString());
            CreateNode(xmlDoc, node2, "_Model_XMJD_", _模板扫描精度.ToString());
            CreateNode(xmlDoc, node2, "__SelectShapeXld_Min__", SelectShapeXld_Min.ToString());
            //CreateNode(xmlDoc, node2, "__SelectShapeXld_Max__", SelectShapeXld_Max.ToString());
            CreateNode(xmlDoc, node2, "__UnionAdjacentContoursXld_X__", UnionAdjacentContoursXld_X.ToString());
            CreateNode(xmlDoc, node2, "__UnionAdjacentContoursXld_Y__", UnionAdjacentContoursXld_Y.ToString());

            root.AppendChild(node2);
            #endregion
            #region 工件定位参数
            //XmlNode node3 = xmlDoc.CreateNode(XmlNodeType.Element, "workpiece_localization", null);
            ////CreateNode(xmlDoc, node3, "_Threshold_2_L", Threshold_2_L.ToString());
            ////CreateNode(xmlDoc, node3, "_ClosingCircle_1", ClosingCircle_1.ToString());
            ////CreateNode(xmlDoc, node3, "_SelectShape_2_area_L", SelectShape_2_area_L.ToString());
            ////CreateNode(xmlDoc, node3, "_SelectShape_2_area_H", SelectShape_2_area_H.ToString());
            ////CreateNode(xmlDoc, node3, "_SelectShape_2_width_L", SelectShape_2_width_L.ToString());
            ////CreateNode(xmlDoc, node3, "_SelectShape_2_width_H", SelectShape_2_width_H.ToString());
            ////CreateNode(xmlDoc, node3, "_ClosingCircle_2", ClosingCircle_2.ToString());
            //root.AppendChild(node3);
            #endregion
            #region 重新定位矩形坐标参数
            XmlNode node4 = xmlDoc.CreateNode(XmlNodeType.Element, "Rectangular_coordinate", null);
            CreateNode(xmlDoc, node4, "_Emphasize_2_X", Emphasize_2_X.ToString());
            CreateNode(xmlDoc, node4, "_Emphasize_2_Y", Emphasize_2_Y.ToString());
            CreateNode(xmlDoc, node4, "_Emphasize_2_Z", Emphasize_2_Z.ToString());
            CreateNode(xmlDoc, node4, "_Image_Analyse_rectangle2_L1", Image_Analyse_rectangle2_L1.ToString());
            CreateNode(xmlDoc, node4, "_Image_Analyse_rectangle2_L2", Image_Analyse_rectangle2_L2.ToString());
            CreateNode(xmlDoc, node4, "_Image_Analyse_rectangle3_LL1", Image_Analyse_rectangle3_LL1.ToString());
            CreateNode(xmlDoc, node4, "_Image_Analyse_rectangle3_LL2", Image_Analyse_rectangle3_LL2.ToString());
            CreateNode(xmlDoc, node4, "_Text_display_position_", 文字显示位置.ToString());
            root.AppendChild(node4);
            #endregion
            #region 焊接段子识别、容错率参数、设定
            XmlNode node5 = xmlDoc.CreateNode(XmlNodeType.Element, "Part_recognition", null);
            CreateNode(xmlDoc, node5, "_Image_Analyse_Threshold_1_H", Image_Analyse_Threshold_1_H.ToString());
            CreateNode(xmlDoc, node5, "_ClosingCircle_1_", ClosingCircle_1.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR1_AREA_L", Image_Analyse_ss_SR1_AREA_L.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR1_HEIGHT_L", Image_Analyse_ss_SR1_HEIGHT_L.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_Threshold_2_L", Image_Analyse_Threshold_2_L.ToString());
            //
            CreateNode(xmlDoc, node5, "_ER_DR_XX_", ER_DR_XX.ToString());
            //
            CreateNode(xmlDoc, node5, "_ClosingCircle_2_", ClosingCircle_2.ToString());//ClosingCircle_2 - ClosingCircle_3
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR2_AREA_L_I", Image_Analyse_ss_SR2_AREA_L_I.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR2_AREA_L_II", Image_Analyse_ss_SR2_AREA_L_II.ToString());
            CreateNode(xmlDoc, node5, "_The_original_point_line_coordinate", _原始点行坐标.ToString());
            CreateNode(xmlDoc, node5, "_The_original_points_coordinates", _原来点列坐标.ToString());

            //==================不插孔接线盒===================上插孔参数
            CreateNode(xmlDoc, node5, "_The_original_point_line_coordinate_II", _原始点行坐标_II.ToString());
            CreateNode(xmlDoc, node5, "_The_original_points_coordinates_II", _原来点列坐标_II.ToString());

            //=================================================================
            CreateNode(xmlDoc, node5, "_Image_Analyse_Threshold_3_L_", Image_Analyse_Threshold_3_L.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR3_AREA_L_", Image_Analyse_ss_SR3_AREA_L.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR3_ss_SR3_WIDTH_L_", Image_Analyse_ss_SR3_ss_SR3_WIDTH_L.ToString());

            //==================不插孔接线盒===================下参数
            CreateNode(xmlDoc, node5, "_X_TINIT_", X_TINIT.ToString());
            CreateNode(xmlDoc, node5, "_HV_threshold_3_L_", HV_threshold_3_L.ToString());
            CreateNode(xmlDoc, node5, "_HV_area_3_", HV_area_3.ToString());
            CreateNode(xmlDoc, node5, "_HV_convexity_", HV_convexity.ToString());

            //==================不插孔接线盒===================上插孔参数
            CreateNode(xmlDoc, node5, "_Image_Analyse_Threshold_4_H_", HV_threshold_4_H.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR4_AREA_L_", HV_ss_SR4_AREA_L.ToString());
            CreateNode(xmlDoc, node5, "_Image_Analyse_ss_SR4_WIDTH_L_", HV_ss_SR4_WIDTH_L.ToString());

            root.AppendChild(node5);
            #endregion
            #region 模板目录
            XmlNode node6 = xmlDoc.CreateNode(XmlNodeType.Element, "Template_directory", null);
            CreateNode(xmlDoc, node6, "_Template_directory", _模板目录.ToString());
            root.AppendChild(node6);
            #endregion
            #region 图像保存，相机SDK驱动与DirectShow接口驱动模式转换
            XmlNode node8 = xmlDoc.CreateNode(XmlNodeType.Element, "Save_image", null);
            CreateNode(xmlDoc, node8, "Qualified_image_preservation", _合格图像保存_.ToString());
            CreateNode(xmlDoc, node8, "Unqualified_image_preservation", _不合格图像保存_.ToString());
            CreateNode(xmlDoc, node8, "__信息图保存控制__", _信息图保存控制_.ToString());

            CreateNode(xmlDoc, node8, "SDK_OR_DirectShow_Mode_conversion", _IMAGE_DirectShow_.ToString());

            root.AppendChild(node8);
            #endregion
            #region 识别等级设定
            XmlNode node9 = xmlDoc.CreateNode(XmlNodeType.Element, "opt_str_out", null);
            CreateNode(xmlDoc, node9, "_str_out_", _str_out.ToString());
            root.AppendChild(node9);
            #endregion
            #region 侧拍检测调整
            XmlNode node10 = xmlDoc.CreateNode(XmlNodeType.Element, "Check_Param", null);
            CreateNode(xmlDoc, node10, "holeGray1", ImgProcess.holeGrayLeft1.ToString());
            CreateNode(xmlDoc, node10, "holeGray2", ImgProcess.holeGrayLeft2.ToString());
            CreateNode(xmlDoc, node10, "holeGray3", ImgProcess.holeGrayLeft3.ToString());
            CreateNode(xmlDoc, node10, "holeGray4", ImgProcess.holeGrayLeft4.ToString());
            CreateNode(xmlDoc, node10, "holeHeight", ImgProcess.holeHeightLeft.ToString());
            CreateNode(xmlDoc, node10, "holeWidth", ImgProcess.holeWidthLeft.ToString());
            CreateNode(xmlDoc, node10, "weldGray", ImgProcess.weldGrayRight1.ToString());
            CreateNode(xmlDoc, node10, "weldGray2", ImgProcess.weldGrayRight2.ToString());
            CreateNode(xmlDoc, node10, "weldGray3", ImgProcess.weldGrayRight3.ToString());
            CreateNode(xmlDoc, node10, "weldGray4", ImgProcess.weldGrayRight4.ToString());




            CreateNode(xmlDoc, node10, "weldHeight", ImgProcess.weldHeightRight.ToString());
            CreateNode(xmlDoc, node10, "weldWidth", ImgProcess.weldWidthRight.ToString());
            CreateNode(xmlDoc, node10, "holeAreaSel", ImgProcess.holeAreaSel.ToString());
            CreateNode(xmlDoc, node10, "holeWidthSel", ImgProcess.holeWidthSel.ToString());
            CreateNode(xmlDoc, node10, "weldSmallSelLength", ImgProcess.weldSmallSelLength.ToString("0.00"));
            CreateNode(xmlDoc, node10, "weldSmallSelWidth", ImgProcess.weldSmallSelWidth.ToString("0.00"));
            CreateNode(xmlDoc, node10, "weldLengthPercent", ImgProcess.weldLengthPercent.ToString("0.00"));

            CreateNode(xmlDoc, node10, "weldLimitPercent", ImgProcess.weldLimitPercent.ToString());
            CreateNode(xmlDoc, node10, "HoleAreaPercent", ImgProcess.HoleAreaPercent.ToString());

            CreateNode(xmlDoc, node10, "weldBackGray", ImgProcess.weldBackGray.ToString());
            CreateNode(xmlDoc, node10, "weldBackPercent", ImgProcess.weldBackPercent.ToString());

            CreateNode(xmlDoc, node10, "TPLAngleLimit", ImgProcess.TPLAngleLimit.ToString("0.000"));
            CreateNode(xmlDoc, node10, "TPLXLimit", ImgProcess.TPLXLimit.ToString());
            CreateNode(xmlDoc, node10, "TPLYLimit", ImgProcess.TPLYLimit.ToString());

            root.AppendChild(node10);
            #endregion
            #region 定位位置
            XmlNode node11 = xmlDoc.CreateNode(XmlNodeType.Element, "Position_Param", null);
            CreateNode(xmlDoc, node11, "XErrorL1", TableCtl.XErrorL1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YErrorL1", TableCtl.YErrorL1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RErrorL1", TableCtl.RErrorL1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "XErrorL2", TableCtl.XErrorL2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YErrorL2", TableCtl.YErrorL2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RErrorL2", TableCtl.RErrorL2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "XErrorR1", TableCtl.XErrorR1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YErrorR1", TableCtl.YErrorR1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RErrorR1", TableCtl.RErrorR1.ToString("0.00"));
            CreateNode(xmlDoc, node11, "XErrorR2", TableCtl.XErrorR2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YErrorR2", TableCtl.YErrorR2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RErrorR2", TableCtl.RErrorR2.ToString("0.00"));
            CreateNode(xmlDoc, node11, "XErrorSN", TableCtl.XErrorSN.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YErrorSN", TableCtl.YErrorSN.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RErrorSN", TableCtl.RErrorSN.ToString("0.00"));

            CreateNode(xmlDoc, node11, "XDefPos", TableCtl.XDefPos.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YDefPos", TableCtl.YDefPos.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RDefPos", TableCtl.RDefPos.ToString("0.00"));

            CreateNode(xmlDoc, node11, "XScale", TableCtl.XScale.ToString());
            CreateNode(xmlDoc, node11, "YScale", TableCtl.YScale.ToString());
            CreateNode(xmlDoc, node11, "RScale", TableCtl.RScale.ToString());
            CreateNode(xmlDoc, node11, "TScale", TableCtl.TScale.ToString());
            CreateNode(xmlDoc, node11, "TScale2", TableCtl.TScale2.ToString());
            CreateNode(xmlDoc, node11, "TScale3", TableCtl.TScale3.ToString());
            CreateNode(xmlDoc, node11, "TScale4", TableCtl.TScale4.ToString());

            CreateNode(xmlDoc, node11, "XVel", TableCtl.XVel.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YVel", TableCtl.YVel.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RVel", TableCtl.RVel.ToString("0.00"));
            CreateNode(xmlDoc, node11, "TVel", TableCtl.TVel.ToString("0.00"));

            CreateNode(xmlDoc, node11, "XAcc", TableCtl.XAcc.ToString("0.00"));
            CreateNode(xmlDoc, node11, "YAcc", TableCtl.YAcc.ToString("0.00"));
            CreateNode(xmlDoc, node11, "RAcc", TableCtl.RAcc.ToString("0.00"));
            CreateNode(xmlDoc, node11, "TAcc", TableCtl.TAcc.ToString("0.00"));

            CreateNode(xmlDoc, node11, "PixelScale", ImgProcess.PixelScale.ToString("0.000"));
            CreateNode(xmlDoc, node11, "TDistance", TableCtl.TDistance.ToString("0.00"));

            //CreateNode(xmlDoc, node11, "AutoOverStation", AutoOverStation.ToString());

            root.AppendChild(node11);
            #endregion

            #region 影像检测
            XmlNode node12 = xmlDoc.CreateNode(XmlNodeType.Element, "Vision_Param", null);
            CreateNode(xmlDoc, node12, "grayVal_weldRegions", VisionParam_JKObject.grayVal_weldRegions.ToString());
            CreateNode(xmlDoc, node12, "grayVal_realShadow", VisionParam_JKObject.grayVal_realShadow.ToString());
            CreateNode(xmlDoc, node12, "grayVal_weldShadow", VisionParam_JKObject.grayVal_weldShadow.ToString());
            CreateNode(xmlDoc, node12, "grayVal_whiteCore", VisionParam_JKObject.grayVal_whiteCore.ToString());
            CreateNode(xmlDoc, node12, "sideGrayVal_WeldRegions", VisionParam_JKObject.sideGrayVal_WeldRegions.ToString());
            CreateNode(xmlDoc, node12, "region_Valid_Height", VisionParam_JKObject.region_Valid_Height.ToString());
            CreateNode(xmlDoc, node12, "whiteCore_valid_width_min", VisionParam_JKObject.whiteCore_valid_width_min.ToString());
            CreateNode(xmlDoc, node12, "focusGrayVal_WeldRegions", VisionParam_JKObject.focusGrayVal_WeldRegions.ToString());
            CreateNode(xmlDoc, node12, "foucsWhite_Valid_Width", VisionParam_JKObject.foucsWhite_Valid_Width.ToString());
            CreateNode(xmlDoc, node12, "Shadow_Valid_Height_Min", VisionParam_JKObject.Shadow_Valid_Height_Min.ToString());
            CreateNode(xmlDoc, node12, "Shadow_Valid_Thick_Min", VisionParam_JKObject.Shadow_Valid_Thick_Min.ToString());
            CreateNode(xmlDoc, node12, "Shadow_Valid_Width_Min", VisionParam_JKObject.Shadow_Valid_Width_Min.ToString());





            //CreateNode(xmlDoc, node11, "AutoOverStation", AutoOverStation.ToString());

            root.AppendChild(node12);
            #endregion
            try
            {
                xmlDoc.Save(filename);
                保存本地配置("user_save_read_", filename);
                MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("保存设置文件错误！\n" + e.Message);
                //maskedTextBox2.Text = "default_setting.config";
                //保存本地配置("user_save_read_", "default_setting.config");
            }
        }
        #endregion
        #region 读取xml中的指定节点的值
        //读取xml中的指定节点的值  
        public void ReadXmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);
                #region 模板定位参数
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root1 = xmlDoc.SelectSingleNode("//star_models");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root1 != null)
                {
                    _ROI_0_R1_X.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_0_C1").InnerText);
                    _ROI_0_R1_Y.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_0_R1").InnerText);
                    _ROI_0_R2_X.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_0_C2").InnerText);
                    _ROI_0_R2_Y.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_0_R2").InnerText);
                    _ROI_1_R1_X.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_1_C1").InnerText);
                    _ROI_1_R1_Y.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_1_R1").InnerText);
                    _ROI_1_R2_X.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_1_C2").InnerText);
                    _ROI_1_R2_Y.Value = Convert.ToInt32(root1.SelectSingleNode("_ROI_1_R2").InnerText);
                    _Model_T_1_L.Value = Convert.ToInt32(root1.SelectSingleNode("_Model_Threshold_1_L").InnerText);
                    _Model_T_1_H.Value = Convert.ToInt32(root1.SelectSingleNode("_Model_Threshold_1_H").InnerText);

                    //1
                    _hv_Row_X1 = Convert.ToDouble(root1.SelectSingleNode("_hv_Row_X1_").InnerText);
                    _hv_Column_X1 = Convert.ToDouble(root1.SelectSingleNode("_hv_Column_X1_").InnerText);
                    //_hv_Phi_X1 = Convert.ToDouble(root1.SelectSingleNode("_hv_Phi_X1_").InnerText);
                    _hv_Length1_X1 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length1_X1_").InnerText);
                    _hv_Length2_X1 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length2_X1_").InnerText);
                    //2
                    _hv_Row_X2 = Convert.ToDouble(root1.SelectSingleNode("_hv_Row_X2_").InnerText);
                    _hv_Column_X2 = Convert.ToDouble(root1.SelectSingleNode("_hv_Column_X2_").InnerText);
                    //_hv_Phi_X2 = Convert.ToDouble(root1.SelectSingleNode("_hv_Phi_X1_").InnerText);
                    _hv_Length1_X2 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length1_X2_").InnerText);
                    _hv_Length2_X2 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length2_X2_").InnerText);
                    //3
                    _hv_Row_X3 = Convert.ToDouble(root1.SelectSingleNode("_hv_Row_X3_").InnerText);
                    _hv_Column_X3 = Convert.ToDouble(root1.SelectSingleNode("_hv_Column_X3_").InnerText);
                    //_hv_Phi_X3 = Convert.ToDouble(root1.SelectSingleNode("_hv_Phi_X3_").InnerText);
                    _hv_Length1_X3 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length1_X3_").InnerText);
                    _hv_Length2_X3 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length2_X3_").InnerText);
                    //4
                    _hv_Row_X4 = Convert.ToDouble(root1.SelectSingleNode("_hv_Row_X4_").InnerText);
                    _hv_Column_X4 = Convert.ToDouble(root1.SelectSingleNode("_hv_Column_X4_").InnerText);
                    //_hv_Phi_X4 = Convert.ToDouble(root1.SelectSingleNode("_hv_Phi_X4_").InnerText);
                    _hv_Length1_X4 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length1_X4_").InnerText);
                    _hv_Length2_X4 = Convert.ToDouble(root1.SelectSingleNode("_hv_Length2_X4_").InnerText);

                    _更新焊接点标定绘制数据();
                    //保存临时坐标参数
                    _RCPLL_();

                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 模板与特征参数
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root2 = xmlDoc.SelectSingleNode("//shape_models_MaxOverlap");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root2 != null)
                {
                    _Emphasize_1_X_.Value = Convert.ToInt32(root2.SelectSingleNode("_Emphasize_1_X").InnerText);
                    _Emphasize_1_Y_.Value = Convert.ToInt32(root2.SelectSingleNode("_Emphasize_1_Y").InnerText);
                    _Emphasize_1_Z_.Value = Convert.ToDecimal(root2.SelectSingleNode("_Emphasize_1_Z").InnerText);

                    _MODEL_Threshold_L.Value = Convert.ToInt32(root2.SelectSingleNode("_MODEL_Threshold_L_").InnerText);
                    _MODEL_AREA_L.Value = Convert.ToInt32(root2.SelectSingleNode("_MODEL_AREA_L_").InnerText);
                    _MODEL_RegionDilation.Value = Convert.ToInt32(root2.SelectSingleNode("_MODEL_RegionDilation_").InnerText);
                    //_MODEL_RegionToBin_L.Value = Convert.ToInt32(root2.SelectSingleNode("_MODEL_RegionToBin_L_").InnerText);
                    //_MODEL_RegionToBin_H.Value = Convert.ToInt32(root2.SelectSingleNode("_MODEL_RegionToBin_H_").InnerText);

                    _模板匹配相似度_.Value = Convert.ToDecimal(root2.SelectSingleNode("_Model_XSD").InnerText);
                    _模板扫描精度_.Value = Convert.ToDecimal(root2.SelectSingleNode("_Model_XMJD_").InnerText);
                    _SelectShapeXld_Min_.Value = Convert.ToDecimal(root2.SelectSingleNode("__SelectShapeXld_Min__").InnerText);
                    //_SelectShapeXld_Max_.Value = Convert.ToDecimal(root2.SelectSingleNode("__SelectShapeXld_Max__").InnerText);
                    _UnionAdjacentContoursXld_X_.Value = Convert.ToDecimal(root2.SelectSingleNode("__UnionAdjacentContoursXld_X__").InnerText);
                    _UnionAdjacentContoursXld_Y_.Value = Convert.ToDecimal(root2.SelectSingleNode("__UnionAdjacentContoursXld_Y__").InnerText);

                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 工件定位参数
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                //XmlNode root3 = xmlDoc.SelectSingleNode("//workpiece_localization");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                //if (root3 != null)
                //{
                //    //_Threshold_2_L_.Value = Convert.ToInt32(root3.SelectSingleNode("_Threshold_2_L").InnerText);
                //    //_ClosingCircle_1_.Value = Convert.ToDecimal(root3.SelectSingleNode("_ClosingCircle_1").InnerText);
                //    //_SelectShape_2_area_L_.Value = Convert.ToInt32(root3.SelectSingleNode("_SelectShape_2_area_L").InnerText);
                //    //_SelectShape_2_area_H_.Value = Convert.ToInt32(root3.SelectSingleNode("_SelectShape_2_area_H").InnerText);
                //    //_SelectShape_2_width_L_.Value = Convert.ToDecimal(root3.SelectSingleNode("_SelectShape_2_width_L").InnerText);
                //    //_SelectShape_2_width_H_.Value = Convert.ToDecimal(root3.SelectSingleNode("_SelectShape_2_width_H").InnerText);
                //    //_ClosingCircle_2_.Value = Convert.ToDecimal(root3.SelectSingleNode("_ClosingCircle_2").InnerText);
                //}
                //else
                //{ //不存在的节点！
                //    MessageBox.Show("读取设置错误：已使用默认配置文件");
                //    maskedTextBox2.Text = _配置文件_;
                //    保存本地配置("user_save_read_", _配置文件_);
                //    CreateXmlFile(_配置文件_);
                //}
                #endregion
                #region 重新定位矩形坐标参数
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root4 = xmlDoc.SelectSingleNode("//Rectangular_coordinate");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root4 != null)
                {
                    _Emphasize_2_X_.Value = Convert.ToInt32(root4.SelectSingleNode("_Emphasize_2_X").InnerText);
                    _Emphasize_2_Y_.Value = Convert.ToInt32(root4.SelectSingleNode("_Emphasize_2_Y").InnerText);
                    _Emphasize_2_Z_.Value = Convert.ToDecimal(root4.SelectSingleNode("_Emphasize_2_Z").InnerText);
                    _Image_Analyse_rectangle2_L1_.Value = Convert.ToInt32(root4.SelectSingleNode("_Image_Analyse_rectangle2_L1").InnerText);
                    _Image_Analyse_rectangle2_L2_.Value = Convert.ToInt32(root4.SelectSingleNode("_Image_Analyse_rectangle2_L2").InnerText);
                    _Image_Analyse_rectangle3_LL1_.Value = Convert.ToInt32(root4.SelectSingleNode("_Image_Analyse_rectangle3_LL1").InnerText);
                    _Image_Analyse_rectangle3_LL2_.Value = Convert.ToInt32(root4.SelectSingleNode("_Image_Analyse_rectangle3_LL2").InnerText);
                    _文字显示位置.Value = Convert.ToInt32(root4.SelectSingleNode("_Text_display_position_").InnerText);
                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 焊接段子识别、容错率参数、设定
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root5 = xmlDoc.SelectSingleNode("//Part_recognition");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root5 != null)
                {
                    _Image_Analyse_Threshold_1_H_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_Threshold_1_H").InnerText);
                    焊接点插口处修补.Value = Convert.ToDecimal(root5.SelectSingleNode("_ClosingCircle_1_").InnerText);
                    _NEW_Image_Analyse_ss_SR1_AREA_L_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_ss_SR1_AREA_L").InnerText);
                    _Image_Analyse_ss_SR1_HEIGHT_L_.Value = Convert.ToDecimal(root5.SelectSingleNode("_Image_Analyse_ss_SR1_HEIGHT_L").InnerText);
                    _Image_Analyse_Threshold_2_L_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_Threshold_2_L").InnerText);
                    //
                    漏铜干扰修补.Value = Convert.ToDecimal(root5.SelectSingleNode("_ER_DR_XX_").InnerText);
                    //
                    漏铜面积修补.Value = Convert.ToDecimal(root5.SelectSingleNode("_ClosingCircle_2_").InnerText);//ClosingCircle_2 - ClosingCircle_3
                    _Image_Analyse_ss_SR2_AREA_L_I_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_ss_SR2_AREA_L_I").InnerText);
                    _Image_Analyse_ss_SR2_AREA_L_II_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_ss_SR2_AREA_L_II").InnerText);
                    原始点行坐标.Value = Convert.ToDecimal(root5.SelectSingleNode("_The_original_point_line_coordinate").InnerText);
                    原来点列坐标.Value = Convert.ToDecimal(root5.SelectSingleNode("_The_original_points_coordinates").InnerText);

                    //=====================不插孔接线盒=========================上插孔参数
                    原始点行坐标_II.Value = Convert.ToDecimal(root5.SelectSingleNode("_The_original_point_line_coordinate_II").InnerText);
                    原来点列坐标_II.Value = Convert.ToDecimal(root5.SelectSingleNode("_The_original_points_coordinates_II").InnerText);

                    //=================================================================
                    _Image_Analyse_Threshold_3_L_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_Threshold_3_L_").InnerText);
                    _Image_Analyse_ss_SR3_AREA_L_.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_ss_SR3_AREA_L_").InnerText);
                    _Image_Analyse_ss_SR3_ss_SR3_WIDTH_L.Value = Convert.ToDecimal(root5.SelectSingleNode("_Image_Analyse_ss_SR3_ss_SR3_WIDTH_L_").InnerText);

                    string _FX1_ = root5.SelectSingleNode("_X_TINIT_").InnerText;
                    if (_FX1_ == "1")
                    {
                        checkBox_接线盒是否插孔.Checked = true;
                    }
                    else
                    {
                        checkBox_接线盒是否插孔.Checked = false;
                    }

                    //=====================不插孔接线盒=========================下参数
                    _HV_threshold_3_L.Value = Convert.ToInt32(root5.SelectSingleNode("_HV_threshold_3_L_").InnerText);
                    _HV_area_3.Value = Convert.ToInt32(root5.SelectSingleNode("_HV_area_3_").InnerText);
                    _HV_convexity.Value = Convert.ToDecimal(root5.SelectSingleNode("_HV_convexity_").InnerText);

                    //=====================不插孔接线盒=========================上插孔参数
                    _HV_threshold_4_H.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_Threshold_4_H_").InnerText);
                    _HV_ss_SR4_AREA_L.Value = Convert.ToInt32(root5.SelectSingleNode("_Image_Analyse_ss_SR4_AREA_L_").InnerText);
                    _HV_ss_SR4_WIDTH_L.Value = Convert.ToDecimal(root5.SelectSingleNode("_Image_Analyse_ss_SR4_WIDTH_L_").InnerText);
                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 模板目录
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root6 = xmlDoc.SelectSingleNode("//Template_directory");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root6 != null)
                {
                    maskedTextBox3.Text = root6.SelectSingleNode("_Template_directory").InnerText;
                    _初始化匹配目录();
                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 图像保存，相机SDK驱动与DirectShow接口驱动模式转换
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root8 = xmlDoc.SelectSingleNode("//Save_image");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root8 != null)
                {
                    string _F1_ = root8.SelectSingleNode("Qualified_image_preservation").InnerText;
                    string _F2_ = root8.SelectSingleNode("Unqualified_image_preservation").InnerText;
                    if (_F1_ == "True")
                    { checkBox_保存合格图像.Checked = true; }
                    else
                    { checkBox_保存合格图像.Checked = false; }
                    if (_F2_ == "True")
                    { checkBox_保存不合格图像.Checked = true; }
                    else
                    { checkBox_保存不合格图像.Checked = false; }

                    信息图保存控制.Value = Convert.ToInt32(root8.SelectSingleNode("__信息图保存控制__").InnerText);

                    string _F3_ = root8.SelectSingleNode("SDK_OR_DirectShow_Mode_conversion").InnerText;
                    if (_F3_ == "True")
                    { IMAGE_directshow.Checked = true; }
                    else
                    { IMAGE_directshow.Checked = false; }

                }
                else
                {//不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion
                #region 识别等级设定
                _str_out = 1;
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root9 = xmlDoc.SelectSingleNode("//opt_str_out");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root9 != null)
                {
                    Int32 temp1 = 0;
                    temp1 = Convert.ToInt32(root9.SelectSingleNode("_str_out_").InnerText);
                    if (temp1 == 2)
                    {
                        _合格等级高.Checked = true;
                    }
                    else if (temp1 == 1)
                    {
                        _合格等级中.Checked = true;
                    }
                    else
                    {
                        _合格等级低.Checked = true;
                    }
                }
                else
                { //不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion


                #region 图像保存，相机SDK驱动与DirectShow接口驱动模式转换
                //读取Activity节点下的数据。SelectSingleNode匹配第一个Activity节点  
                XmlNode root10 = xmlDoc.SelectSingleNode("//Check_Param");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root10 != null)
                {


                    ImgProcess.holeGrayLeft1 = Convert.ToInt32(root10.SelectSingleNode("holeGray1").InnerText);
                    ImgProcess.holeGrayLeft2 = Convert.ToInt32(root10.SelectSingleNode("holeGray2").InnerText);
                    ImgProcess.holeGrayLeft3 = Convert.ToInt32(root10.SelectSingleNode("holeGray3").InnerText);
                    ImgProcess.holeGrayLeft4 = Convert.ToInt32(root10.SelectSingleNode("holeGray4").InnerText);

                    ImgProcess.holeHeightLeft = Convert.ToInt32(root10.SelectSingleNode("holeHeight").InnerText);
                    ImgProcess.holeWidthLeft = Convert.ToInt32(root10.SelectSingleNode("holeWidth").InnerText);
                    ImgProcess.weldGrayRight1 = Convert.ToDouble(root10.SelectSingleNode("weldGray").InnerText);
                    ImgProcess.weldGrayRight2 = Convert.ToInt32(root10.SelectSingleNode("weldGray2").InnerText);
                    ImgProcess.weldGrayRight3 = Convert.ToInt32(root10.SelectSingleNode("weldGray3").InnerText);
                    ImgProcess.weldGrayRight4 = Convert.ToInt32(root10.SelectSingleNode("weldGray4").InnerText);



                    ImgProcess.weldWidthRight = Convert.ToInt32(root10.SelectSingleNode("weldWidth").InnerText);
                    ImgProcess.weldHeightRight = Convert.ToInt32(root10.SelectSingleNode("weldHeight").InnerText);
                    ImgProcess.weldLimitPercent = Convert.ToInt32(root10.SelectSingleNode("weldLimitPercent").InnerText);
                    ImgProcess.HoleAreaPercent = Convert.ToInt32(root10.SelectSingleNode("HoleAreaPercent").InnerText);
                    ImgProcess.holeAreaSel = Convert.ToInt32(root10.SelectSingleNode("holeAreaSel").InnerText);
                    ImgProcess.holeWidthSel = Convert.ToInt32(root10.SelectSingleNode("holeWidthSel").InnerText);



                    textBox_WiedLimit.Text = ImgProcess.weldLimitPercent.ToString();
                    textBox_HoleArea.Text = ImgProcess.HoleAreaPercent.ToString();
                    textBox_HoleGAdjust.Text = ImgProcess.holeGrayLeft1.ToString();
                    textBox_HoleGAdjust2.Text = ImgProcess.holeGrayLeft2.ToString();
                    textBox_HoleGAdjust3.Text = ImgProcess.holeGrayLeft3.ToString();
                    textBox_HoleGAdjust4.Text = ImgProcess.holeGrayLeft4.ToString();
                    textBox_HoleHAdjust.Text = ImgProcess.holeHeightLeft.ToString();
                    textBox_HoleWAdjust.Text = ImgProcess.holeWidthLeft.ToString();
                    textBox_weldGAdjust.Text = ImgProcess.weldGrayRight1.ToString();
                    textBox_weldGAdjust2.Text = ImgProcess.weldGrayRight2.ToString();
                    textBox_weldGAdjust3.Text = ImgProcess.weldGrayRight3.ToString();
                    textBox_weldGAdjust4.Text = ImgProcess.weldGrayRight4.ToString();
                    textBox_weldHAdjust.Text = ImgProcess.weldHeightRight.ToString();
                    textBox_weldWAdjust.Text = ImgProcess.weldWidthRight.ToString();
                    textBox_HoleSelArea.Text = ImgProcess.holeAreaSel.ToString();
                    textBox_HoleSelW.Text = ImgProcess.holeWidthSel.ToString();

                    ImgProcess.weldSmallSelLength = Convert.ToDouble(root10.SelectSingleNode("weldSmallSelLength").InnerText);
                    ImgProcess.weldLengthPercent = Convert.ToDouble(root10.SelectSingleNode("weldLengthPercent").InnerText);
                    textBox_weldSelLength.Text = ImgProcess.weldSmallSelLength.ToString("0.00");
                    textBox_weldLengthPercent.Text = ImgProcess.weldLengthPercent.ToString("0.00");

                    ImgProcess.weldSmallSelWidth = Convert.ToDouble(root10.SelectSingleNode("weldSmallSelWidth").InnerText);
                    textBox_weldSelWidth.Text = ImgProcess.weldSmallSelWidth.ToString("0.00");

                    ImgProcess.weldBackGray = Convert.ToInt32(root10.SelectSingleNode("weldBackGray").InnerText);
                    textBox_weldBackGray.Text = ImgProcess.weldBackGray.ToString();

                    ImgProcess.weldBackPercent = Convert.ToInt32(root10.SelectSingleNode("weldBackPercent").InnerText);
                    textBox_weldBackPercent.Text = ImgProcess.weldBackPercent.ToString();

                    XmlNode tmp = root10.SelectSingleNode("TPLAngleLimit");
                    if (tmp == null)
                    {
                        ImgProcess.TPLAngleLimit = 1;
                    }
                    else
                        ImgProcess.TPLAngleLimit = Convert.ToDouble(root10.SelectSingleNode("TPLAngleLimit").InnerText);
                    textBox_TPLAngleLimit.Text = ImgProcess.TPLAngleLimit.ToString();

                    tmp = root10.SelectSingleNode("TPLXLimit");
                    if (tmp == null)
                    {
                        ImgProcess.TPLXLimit = 5;
                    }
                    else
                        ImgProcess.TPLXLimit = Convert.ToInt32(root10.SelectSingleNode("TPLXLimit").InnerText);
                    textBox_TPLXLimit.Text = ImgProcess.TPLXLimit.ToString();

                    tmp = root10.SelectSingleNode("TPLYLimit");
                    if (tmp == null)
                    {
                        ImgProcess.TPLYLimit = 5;
                    }
                    else
                        ImgProcess.TPLYLimit = Convert.ToInt32(root10.SelectSingleNode("TPLYLimit").InnerText);
                    textBox_TPLYLimit.Text = ImgProcess.TPLYLimit.ToString();
                }
                else
                {//不存在的节点！
                    MessageBox.Show("读取设置错误：已使用默认配置文件");
                    maskedTextBox2.Text = _配置文件_;
                    保存本地配置("user_save_read_", _配置文件_);
                    CreateXmlFile(_配置文件_);
                }
                #endregion

                #region 定位位置

                XmlNode root11 = xmlDoc.SelectSingleNode("//Position_Param");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root11 != null)
                {
                    TableCtl.XErrorL1 = Convert.ToDouble(root11.SelectSingleNode("XErrorL1").InnerText);
                    TableCtl.YErrorL1 = Convert.ToDouble(root11.SelectSingleNode("YErrorL1").InnerText);
                    TableCtl.RErrorL1 = Convert.ToDouble(root11.SelectSingleNode("RErrorL1").InnerText);
                    TableCtl.XErrorL2 = Convert.ToDouble(root11.SelectSingleNode("XErrorL2").InnerText);
                    TableCtl.YErrorL2 = Convert.ToDouble(root11.SelectSingleNode("YErrorL2").InnerText);
                    TableCtl.RErrorL2 = Convert.ToDouble(root11.SelectSingleNode("RErrorL2").InnerText);
                    TableCtl.XErrorR1 = Convert.ToDouble(root11.SelectSingleNode("XErrorR1").InnerText);
                    TableCtl.YErrorR1 = Convert.ToDouble(root11.SelectSingleNode("YErrorR1").InnerText);
                    TableCtl.RErrorR1 = Convert.ToDouble(root11.SelectSingleNode("RErrorR1").InnerText);
                    TableCtl.XErrorR2 = Convert.ToDouble(root11.SelectSingleNode("XErrorR2").InnerText);
                    TableCtl.YErrorR2 = Convert.ToDouble(root11.SelectSingleNode("YErrorR2").InnerText);
                    TableCtl.RErrorR2 = Convert.ToDouble(root11.SelectSingleNode("RErrorR2").InnerText);
                    TableCtl.XErrorSN = Convert.ToDouble(root11.SelectSingleNode("XErrorSN").InnerText);
                    TableCtl.YErrorSN = Convert.ToDouble(root11.SelectSingleNode("YErrorSN").InnerText);
                    TableCtl.RErrorSN = Convert.ToDouble(root11.SelectSingleNode("RErrorSN").InnerText);
                    TableCtl.XDefPos = Convert.ToDouble(root11.SelectSingleNode("XDefPos").InnerText);
                    TableCtl.YDefPos = Convert.ToDouble(root11.SelectSingleNode("YDefPos").InnerText);
                    TableCtl.RDefPos = Convert.ToDouble(root11.SelectSingleNode("RDefPos").InnerText);
                    TableCtl.XScale = Convert.ToInt32(root11.SelectSingleNode("XScale").InnerText);
                    TableCtl.YScale = Convert.ToInt32(root11.SelectSingleNode("YScale").InnerText);
                    TableCtl.RScale = Convert.ToInt32(root11.SelectSingleNode("RScale").InnerText);


                    TableCtl.XVel = Convert.ToDouble(root11.SelectSingleNode("XVel").InnerText);
                    TableCtl.YVel = Convert.ToDouble(root11.SelectSingleNode("YVel").InnerText);
                    TableCtl.RVel = Convert.ToDouble(root11.SelectSingleNode("RVel").InnerText);
                    TableCtl.XAcc = Convert.ToDouble(root11.SelectSingleNode("XAcc").InnerText);
                    TableCtl.YAcc = Convert.ToDouble(root11.SelectSingleNode("YAcc").InnerText);
                    TableCtl.RAcc = Convert.ToDouble(root11.SelectSingleNode("RAcc").InnerText);

                    ImgProcess.PixelScale = Convert.ToDouble(root11.SelectSingleNode("PixelScale").InnerText);


                    textBox_XSCALE.Text = TableCtl.XScale.ToString();
                    textBox_XYACC.Text = TableCtl.XAcc.ToString("0.00");
                    textBox_XYVEL.Text = TableCtl.XVel.ToString("0.00");
                    textBox_XERROR.Text = TableCtl.XErrorL1.ToString("0.00");
                    textBox_XERROR2.Text = TableCtl.XErrorR1.ToString("0.00");
                    textBox_XERRORL.Text = TableCtl.XErrorL2.ToString("0.00");
                    textBox_XERROR2R.Text = TableCtl.XErrorR2.ToString("0.00");
                    textBox_XERRORSN.Text = TableCtl.XErrorSN.ToString("0.00");
                    textBox_XDEFPOS.Text = TableCtl.XDefPos.ToString("0.00");

                    textBox_YSCALE.Text = TableCtl.YScale.ToString();

                    textBox_YERROR.Text = TableCtl.YErrorL1.ToString("0.00");
                    textBox_YERROR2.Text = TableCtl.YErrorR1.ToString("0.00");
                    textBox_YERRORL.Text = TableCtl.YErrorL2.ToString("0.00");
                    textBox_YERROR2R.Text = TableCtl.YErrorR2.ToString("0.00");
                    textBox_YERRORSN.Text = TableCtl.YErrorSN.ToString("0.00");
                    textBox_YDELPOS.Text = TableCtl.YDefPos.ToString("0.00");

                    textBox_RSCALE.Text = TableCtl.RScale.ToString();
                    textBox_RACC.Text = TableCtl.RAcc.ToString("0.00");
                    textBox_RVEL.Text = TableCtl.RVel.ToString("0.00");
                    textBox_RERROR.Text = TableCtl.RErrorL1.ToString("0.00");
                    textBox_RERROR2.Text = TableCtl.RErrorR1.ToString("0.00");
                    textBox_RERRORL.Text = TableCtl.RErrorL2.ToString("0.00");
                    textBox_RERROR2R.Text = TableCtl.RErrorR2.ToString("0.00");
                    textBox_RERRORSN.Text = TableCtl.RErrorSN.ToString("0.00");
                    textBox_RDELPOS.Text = TableCtl.RDefPos.ToString("0.00");
                    textBox_PixlScale.Text = ImgProcess.PixelScale.ToString("0.000");


                    TableCtl.TScale = Convert.ToInt32(root11.SelectSingleNode("TScale").InnerText);
                    TableCtl.TScale2 = Convert.ToInt32(root11.SelectSingleNode("TScale2").InnerText);
                    TableCtl.TScale3 = Convert.ToInt32(root11.SelectSingleNode("TScale3").InnerText);
                    TableCtl.TScale4 = Convert.ToInt32(root11.SelectSingleNode("TScale4").InnerText);
                    TableCtl.TVel = Convert.ToDouble(root11.SelectSingleNode("TVel").InnerText);
                    TableCtl.TAcc = Convert.ToDouble(root11.SelectSingleNode("TAcc").InnerText);
                    TableCtl.TDistance = Convert.ToDouble(root11.SelectSingleNode("TDistance").InnerText);

                    textBox_Tin_Scale.Text = TableCtl.TScale.ToString();
                    textBox_Tin_Scale2.Text = TableCtl.TScale2.ToString();
                    textBox_Tin_Scale3.Text = TableCtl.TScale3.ToString();
                    textBox_Tin_Scale4.Text = TableCtl.TScale4.ToString();
                    textBox_Tin_Acc.Text = TableCtl.TAcc.ToString("0.00");
                    textBox_Tin_Vel.Text = TableCtl.TVel.ToString("0.00");
                    textBox_Tin_Len.Text = TableCtl.TDistance.ToString("0.00");
                }

                #endregion

                #region 影像检测

                XmlNode root12 = xmlDoc.SelectSingleNode("//Vision_Param");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (root12 != null)
                {
                    VisionParam_JKObject.grayVal_weldRegions = Convert.ToInt32(root12.SelectSingleNode("grayVal_weldRegions").InnerText);
                    VisionParam_JKObject.grayVal_realShadow = Convert.ToInt32(root12.SelectSingleNode("grayVal_realShadow").InnerText);
                    VisionParam_JKObject.grayVal_weldShadow = Convert.ToInt32(root12.SelectSingleNode("grayVal_weldShadow").InnerText);
                    VisionParam_JKObject.grayVal_whiteCore = Convert.ToInt32(root12.SelectSingleNode("grayVal_whiteCore").InnerText);

                    VisionParam_JKObject.Shadow_Valid_Height_Min = Convert.ToInt32(root12.SelectSingleNode("Shadow_Valid_Height_Min").InnerText);
                    VisionParam_JKObject.Shadow_Valid_Thick_Min = Convert.ToInt32(root12.SelectSingleNode("Shadow_Valid_Thick_Min").InnerText);
                    VisionParam_JKObject.Shadow_Valid_Width_Min = Convert.ToInt32(root12.SelectSingleNode("Shadow_Valid_Width_Min").InnerText);
                    VisionParam_JKObject.sideGrayVal_WeldRegions = Convert.ToInt32(root12.SelectSingleNode("sideGrayVal_WeldRegions").InnerText);

                    VisionParam_JKObject.focusGrayVal_WeldRegions = Convert.ToInt32(root12.SelectSingleNode("focusGrayVal_WeldRegions").InnerText);
                    VisionParam_JKObject.foucsWhite_Valid_Width = Convert.ToInt32(root12.SelectSingleNode("foucsWhite_Valid_Width").InnerText);
                    VisionParam_JKObject.region_Valid_Height = Convert.ToInt32(root12.SelectSingleNode("region_Valid_Height").InnerText);
                    VisionParam_JKObject.whiteCore_valid_width_min = Convert.ToInt32(root12.SelectSingleNode("whiteCore_valid_width_min").InnerText);

                }

                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取用户配置文件失败！\n" + e.Message);
                //maskedTextBox2.Text = _配置文件_;
                //保存本地配置("user_save_read_", _配置文件_);
                //CreateXmlFile(_配置文件_);
            }
        }
        #endregion
        #region 保存图片、保存配置文件，读取配置文件
        private void 保存图片目录_Click_1(object sender, EventArgs e)
        {
            // 设置根在桌面
            //folderBrowserDialog1.RootFolder = SpecialFolder.Desktop;
            // 设置当前选择的路径
            folderBrowserDialog1.SelectedPath = "D:";
            // 允许在对话框中包括一个新建目录的按钮
            folderBrowserDialog1.ShowNewFolderButton = true;
            // 设置对话框的说明信息
            folderBrowserDialog1.Description = "请选择保存目录";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                maskedTextBox1.Text = folderBrowserDialog1.SelectedPath;//得到选择的路径
                保存本地配置("Image_save_", maskedTextBox1.Text);
            }
        }
        private void 保存配置路径()
        {
            saveFileDialog1.Filter = "|*.config";
            //saveFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                maskedTextBox2.Text = saveFileDialog1.FileName;//得到选择的路径
                CreateXmlFile(maskedTextBox2.Text);
                保存本地配置("user_save_read_", maskedTextBox2.Text);
            }
        }
        private void 读取配置路径()
        {
            openFileDialog1.Filter = "|*.config";
            //openFileDialog1.RestoreDirectory = true;
            //openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                maskedTextBox2.Text = openFileDialog1.FileName;//得到选择的路径
                ReadXmlNode(maskedTextBox2.Text);
                //保存本地配置("user_save_read_", maskedTextBox2.Text);
            }
        }


        private void 保存设置文件_Click_1(object sender, EventArgs e)
        {
            保存本地配置("Camera_port_", 相机端口号.Value.ToString());
            保存本地配置("Image_save_", maskedTextBox1.Text);
            保存本地配置("user_save_read_", maskedTextBox2.Text);

            保存配置路径();
            //CreateXmlFile(maskedTextBox2.Text);
        }

        private void 读取设置文件_Click_1(object sender, EventArgs e)
        {
            读取配置路径();
        }

        #endregion

        bool _合格图像保存_ = true;
        bool _不合格图像保存_ = true;
        int _信息图保存控制_ = 0;
        private void 信息图保存控制_ValueChanged(object sender, EventArgs e)
        {
            _信息图保存控制_ = (int)信息图保存控制.Value;
        }
        private void checkBox_保存合格图像_CheckedChanged(object sender, EventArgs e)
        {
            _合格图像保存_ = checkBox_保存合格图像.Checked;
        }
        private void checkBox_保存不合格图像_CheckedChanged(object sender, EventArgs e)
        {
            _不合格图像保存_ = checkBox_保存不合格图像.Checked;
        }
        #region HW_SAVER_IMAGE
        private void HW_SAVER_IMAGE(HObject IMAGE, string D)
        {
            // Error variable 'hv_ErrorNum' activated
            hv_ErrorNum = 2;
            // (dev_)set_check ("~give_error")
            //open first camera device with current video settings:
            try
            {
                hv_ErrorNum = 2;
                if (_信息图保存控制_ == 1)
                {
                    //窗口截图保存,将显示的文字和选区一起保存
                    HOperatorSet.DumpWindow(hWindowControl1.HalconWindow, "jpeg", D + "S");
                }
                else if (_信息图保存控制_ == 2)
                {
                    HOperatorSet.WriteImage(IMAGE, "jpeg", 0, D);
                    //窗口截图保存,将显示的文字和选区一起保存
                    HOperatorSet.DumpWindow(hWindowControl1.HalconWindow, "jpeg", D + "S");
                }
                else
                {
                    HOperatorSet.WriteImage(IMAGE, "jpeg", 0, D);
                }
            }
            catch (HalconException e)
            {
                hv_ErrorNum = e.GetErrorNumber();
                if ((int)hv_ErrorNum < 0) throw e;
                通讯打开标志位 = false;
                MessageBox.Show("保存图像错误" + e.Message);
            }

        }
        #endregion
        #region 保存图像
        private void _保存图像_()
        {
            //if (通讯打开标志位)
            //{
            if (ADT_text_flag == "NG" || ADT_text_flag == "OK")
            {
                if (_合格图像保存_ && _不合格图像保存_)
                {
                    //获取时间
                    DateTime _DATATIME_ = DateTime.Now;//20:16:16
                    string _DATATIME_M_ = _DATATIME_.ToString("HHmmss");
                    HW_SAVER_IMAGE(ho_ImageZoom1, 目录 + "\\" + ADT_text_flag + "\\" + _DATATIME_M_);
                }
                else if (_合格图像保存_)
                {
                    DateTime _DATATIME_ = DateTime.Now;
                    string _DATATIME_M_ = _DATATIME_.ToString("HHmmss");
                    HW_SAVER_IMAGE(ho_ImageZoom1, 目录 + "\\" + ADT_text_flag + "\\" + _DATATIME_M_);

                }
                else if (_不合格图像保存_)
                {
                    DateTime _DATATIME_ = DateTime.Now;
                    string _DATATIME_M_ = _DATATIME_.ToString("HHmmss");
                    HW_SAVER_IMAGE(ho_ImageZoom1, 目录 + "\\" + ADT_text_flag + "\\" + _DATATIME_M_);
                }
            }
            else if (ADT_text_flag == "ERROR")
            {
                DateTime _DATATIME_ = DateTime.Now;
                string _DATATIME_M_ = _DATATIME_.ToString("HHmmss");
                HW_SAVER_IMAGE(ho_ImageZoom1, 目录 + "\\" + ADT_text_flag + "\\" + _DATATIME_M_ + ADT_text_ERROR);
            }
            //}
        }
        #endregion

        //
        private void colour_difference_default_Click(object sender, EventArgs e)
        {
            相机端口号.Value = 0;
            maskedTextBox1.Text = "D:\\Image";
            maskedTextBox2.Text = "default_setting.config";
            ReadXmlNode(maskedTextBox2.Text);
        }
        //
        private void colour_difference_save_Click(object sender, EventArgs e)
        {
            保存本地配置("Camera_port_", 相机端口号.Value.ToString());
            保存本地配置("Image_save_", maskedTextBox1.Text);
            保存本地配置("user_save_read_", maskedTextBox2.Text);

            CreateXmlFile(maskedTextBox2.Text);//
            Saver_hq_zm1_recognition_XmlFile("hqzm1_recognition.config");

        }

        //'white', 'black', 'gray', 'red', 'green', 'blue'
        //
        #region 模板定位参数
        //2
        Int32 ROI_0_C1 = 156;
        Int32 ROI_0_R1 = 511;
        Int32 ROI_0_C2 = 1125;
        Int32 ROI_0_R2 = 722;
        //1
        Int32 ROI_1_C1 = 228;
        Int32 ROI_1_R1 = 360;
        Int32 ROI_1_C2 = 1062;
        Int32 ROI_1_R2 = 657;
        //
        bool _显示定标矩形 = false;
        bool _显示相减矩形 = false;
        //模板定位阈值设定 L - 低 H - 高
        Int32 Model_Threshold_1_L = 45;
        Int32 Model_Threshold_1_H = 150;
        //测试显示当前模板图形
        bool _XT1 = false;
        //
        double _hv_Row_X1 = 464.44, _hv_Column_X1 = 359.14, _hv_Phi_X1 = 0, _hv_Length1_X1 = 63.18, _hv_Length2_X1 = 35.64;
        double _hv_Row_X2 = 466.06, _hv_Column_X2 = 582.70, _hv_Phi_X2 = 0, _hv_Length1_X2 = 69.66, _hv_Length2_X2 = 37.26;
        double _hv_Row_X3 = 467.68, _hv_Column_X3 = 765.76, _hv_Phi_X3 = 0, _hv_Length1_X3 = 66.42, _hv_Length2_X3 = 37.26;
        double _hv_Row_X4 = 469.23, _hv_Column_X4 = 986.08, _hv_Phi_X4 = 0, _hv_Length1_X4 = 58.32, _hv_Length2_X4 = 35.64;
        double HVROW_X1 = 0, HVCOLUMN_X1 = 0, HVPHI_X1 = 0, HVLENGTH1_X1 = 0, HVLENGTH2_X1 = 0;
        double HVROW_X2 = 0, HVCOLUMN_X2 = 0, HVPHI_X2 = 0, HVLENGTH1_X2 = 0, HVLENGTH2_X2 = 0;
        double HVROW_X3 = 0, HVCOLUMN_X3 = 0, HVPHI_X3 = 0, HVLENGTH1_X3 = 0, HVLENGTH2_X3 = 0;
        double HVROW_X4 = 0, HVCOLUMN_X4 = 0, HVPHI_X4 = 0, HVLENGTH1_X4 = 0, HVLENGTH2_X4 = 0;
        //保存临时坐标参数
        private void _RCPLL_()
        {
            HVROW_X1 = _hv_Row_X1; HVCOLUMN_X1 = _hv_Column_X1; HVPHI_X1 = _hv_Phi_X1; HVLENGTH1_X1 = _hv_Length1_X1; HVLENGTH2_X1 = _hv_Length2_X1;
            HVROW_X2 = _hv_Row_X2; HVCOLUMN_X2 = _hv_Column_X2; HVPHI_X2 = _hv_Phi_X2; HVLENGTH1_X2 = _hv_Length1_X2; HVLENGTH2_X2 = _hv_Length2_X2;
            HVROW_X3 = _hv_Row_X3; HVCOLUMN_X3 = _hv_Column_X3; HVPHI_X3 = _hv_Phi_X3; HVLENGTH1_X3 = _hv_Length1_X3; HVLENGTH2_X3 = _hv_Length2_X3;
            HVROW_X4 = _hv_Row_X4; HVCOLUMN_X4 = _hv_Column_X4; HVPHI_X4 = _hv_Phi_X4; HVLENGTH1_X4 = _hv_Length1_X4; HVLENGTH2_X4 = _hv_Length2_X4;
        }
        //
        int _端子标记 = 0;
        //
        //bool _端子一鼠标设置 = false;
        //bool _端子二鼠标设置 = false;
        //bool _端子三鼠标设置 = false;
        //bool _端子四鼠标设置 = false;
        //
        bool _标注位置V1 = true;
        bool _标注位置V2 = false;
        bool _标注位置V3 = false;
        bool _标注位置V4 = false;
        //2
        private void _ROI_0_R1_X_ValueChanged(object sender, EventArgs e)
        { ROI_0_C1 = (Int32)_ROI_0_R1_X.Value; }
        private void _ROI_0_R1_Y_ValueChanged(object sender, EventArgs e)
        { ROI_0_R1 = (Int32)_ROI_0_R1_Y.Value; }
        private void _ROI_0_R2_X_ValueChanged(object sender, EventArgs e)
        { ROI_0_C2 = (Int32)_ROI_0_R2_X.Value; }
        private void _ROI_0_R2_Y_ValueChanged(object sender, EventArgs e)
        { ROI_0_R2 = (Int32)_ROI_0_R2_Y.Value; }
        //1
        private void _ROI_1_R1_X_ValueChanged(object sender, EventArgs e)
        { ROI_1_C1 = (Int32)_ROI_1_R1_X.Value; }
        private void _ROI_1_R1_Y_ValueChanged(object sender, EventArgs e)
        { ROI_1_R1 = (Int32)_ROI_1_R1_Y.Value; }
        private void _ROI_1_R2_X_ValueChanged(object sender, EventArgs e)
        { ROI_1_C2 = (Int32)_ROI_1_R2_X.Value; }
        private void _ROI_1_R2_Y_ValueChanged(object sender, EventArgs e)
        { ROI_1_R2 = (Int32)_ROI_1_R2_Y.Value; }
        private void _显示定标矩形__CheckedChanged(object sender, EventArgs e)
        { _显示定标矩形 = _显示定标矩形_.Checked; }
        private void _显示相减矩形__CheckedChanged(object sender, EventArgs e)
        { _显示相减矩形 = _显示相减矩形_.Checked; }
        private void _Model_T_1_L_ValueChanged(object sender, EventArgs e)
        {
            if (_Model_T_1_L.Value >= _Model_T_1_H.Value - 9)
            {
                _Model_T_1_L.Value = _Model_T_1_H.Value - 10;
                Model_Threshold_1_L = (Int32)_Model_T_1_L.Value;
            }
            else
            { Model_Threshold_1_L = (Int32)_Model_T_1_L.Value; }
        }
        private void _Model_T_1_H_ValueChanged(object sender, EventArgs e)
        {
            if (_Model_T_1_H.Value <= _Model_T_1_L.Value + 9)
            {
                _Model_T_1_H.Value = _Model_T_1_L.Value + 10;
                Model_Threshold_1_L = (Int32)_Model_T_1_L.Value;
            }
            else
            { Model_Threshold_1_H = (Int32)_Model_T_1_H.Value; }
        }
        private void _显示匹配轮廓__CheckedChanged(object sender, EventArgs e)
        { _XT1 = _显示匹配轮廓_.Checked; }
        private void radioButton_V1_CheckedChanged(object sender, EventArgs e)
        {
            _端子标记 = 0;
            _标注位置V1 = true;
            _标注位置V2 = false;
            _标注位置V3 = false;
            _标注位置V4 = false;
            _更新焊接点标定绘制数据();
        }
        private void radioButton_V2_CheckedChanged(object sender, EventArgs e)
        {
            _端子标记 = 1;
            _标注位置V1 = false;
            _标注位置V2 = true;
            _标注位置V3 = false;
            _标注位置V4 = false;
            _更新焊接点标定绘制数据();
        }
        private void radioButton_V3_CheckedChanged(object sender, EventArgs e)
        {
            _端子标记 = 2;
            _标注位置V1 = false;
            _标注位置V2 = false;
            _标注位置V3 = true;
            _标注位置V4 = false;
            _更新焊接点标定绘制数据();
        }
        private void radioButton_V4_CheckedChanged(object sender, EventArgs e)
        {
            _端子标记 = 3;
            _标注位置V1 = false;
            _标注位置V2 = false;
            _标注位置V3 = false;
            _标注位置V4 = true;
            _更新焊接点标定绘制数据();
        }
        private void _更新焊接点标定绘制数据()
        {
            if (_端子标记 > 2)
            {
                焊接点位置坐标Y.Value = (decimal)_hv_Row_X4;
                焊接点位置坐标X.Value = (decimal)_hv_Column_X4;
                焊接点绘制长度.Value = (decimal)_hv_Length1_X4;
                焊接点绘制宽度.Value = (decimal)_hv_Length2_X4;
            }
            else if (_端子标记 == 2)
            {
                焊接点位置坐标Y.Value = (decimal)_hv_Row_X3;
                焊接点位置坐标X.Value = (decimal)_hv_Column_X3;
                焊接点绘制长度.Value = (decimal)_hv_Length1_X3;
                焊接点绘制宽度.Value = (decimal)_hv_Length2_X3;
            }
            else if (_端子标记 == 1)
            {
                焊接点位置坐标Y.Value = (decimal)_hv_Row_X2;
                焊接点位置坐标X.Value = (decimal)_hv_Column_X2;
                焊接点绘制长度.Value = (decimal)_hv_Length1_X2;
                焊接点绘制宽度.Value = (decimal)_hv_Length2_X2;
            }
            else
            {
                焊接点位置坐标Y.Value = (decimal)_hv_Row_X1;
                焊接点位置坐标X.Value = (decimal)_hv_Column_X1;
                焊接点绘制长度.Value = (decimal)_hv_Length1_X1;
                焊接点绘制宽度.Value = (decimal)_hv_Length2_X1;
            }
        }
        //private void 绘制焊接区域_Click(object sender, EventArgs e)
        //{
        //    if (_端子标记 > 2)
        //    {
        //        _端子四鼠标设置 = true;
        //    }
        //    else if (_端子标记 == 2)
        //    {
        //        _端子三鼠标设置 = true;
        //    }
        //    else if (_端子标记 == 1)
        //    {
        //        _端子二鼠标设置 = true;
        //    }
        //    else
        //    {
        //        _端子一鼠标设置 = true;
        //    }
        //}
        private void 焊接点位置坐标Y_ValueChanged(object sender, EventArgs e)
        {
            if (_端子标记 > 2)
            {
                _hv_Row_X4 = (double)焊接点位置坐标Y.Value;
            }
            else if (_端子标记 == 2)
            {
                _hv_Row_X3 = (double)焊接点位置坐标Y.Value;
            }
            else if (_端子标记 == 1)
            {
                _hv_Row_X2 = (double)焊接点位置坐标Y.Value;
            }
            else
            {
                _hv_Row_X1 = (double)焊接点位置坐标Y.Value;
            }
        }
        private void 焊接点位置坐标X_ValueChanged(object sender, EventArgs e)
        {
            if (_端子标记 > 2)
            {
                _hv_Column_X4 = (double)焊接点位置坐标X.Value;
            }
            else if (_端子标记 == 2)
            {
                _hv_Column_X3 = (double)焊接点位置坐标X.Value;
            }
            else if (_端子标记 == 1)
            {
                _hv_Column_X2 = (double)焊接点位置坐标X.Value;
            }
            else
            {
                _hv_Column_X1 = (double)焊接点位置坐标X.Value;
            }
        }
        private void 焊接点绘制长度_ValueChanged(object sender, EventArgs e)
        {
            if (_端子标记 > 2)
            {
                _hv_Length1_X4 = (double)焊接点绘制长度.Value;
            }
            else if (_端子标记 == 2)
            {
                _hv_Length1_X3 = (double)焊接点绘制长度.Value;
            }
            else if (_端子标记 == 1)
            {
                _hv_Length1_X2 = (double)焊接点绘制长度.Value;
            }
            else
            {
                _hv_Length1_X1 = (double)焊接点绘制长度.Value;
            }
        }
        private void 焊接点绘制宽度_ValueChanged(object sender, EventArgs e)
        {
            if (_端子标记 > 2)
            {
                _hv_Length2_X4 = (double)焊接点绘制宽度.Value;
            }
            else if (_端子标记 == 2)
            {
                _hv_Length2_X3 = (double)焊接点绘制宽度.Value;
            }
            else if (_端子标记 == 1)
            {
                _hv_Length2_X2 = (double)焊接点绘制宽度.Value;
            }
            else
            {
                _hv_Length2_X1 = (double)焊接点绘制宽度.Value;
            }
        }
        #endregion
        //
        #region 模板与特征参数
        //模板与特征、图像增强参数1
        Int32 Emphasize_1_X = 10;
        Int32 Emphasize_1_Y = 5;
        double Emphasize_1_Z = 3.0;

        //模板目录
        string _模板目录 = "";
        //========================================
        //区域生成显示图像
        bool _显示生成图像 = false;
        bool _显示二值化选区 = false;
        //模板图像二值化
        Int32 MODEL_Threshold_L = 80;
        //模板最小面积
        Int32 MODEL_AREA_L = 400000;
        //模板区域填充大小
        Int32 MODEL_RegionDilation = 5;

        //模板匹配精度
        double _模板匹配相似度 = 0.8;
        double _模板扫描精度 = 0.9;
        bool _显示轮廓线数量 = false;
        //选择大于一定长度的轮廓线
        Int32 SelectShapeXld_Min = 2500;
        Int32 SelectShapeXld_Max = 999999;
        //合并轮廓线 轮廓终点的最大距离。轮廓“端点相对于较长轮廓的长度最大的距离。
        Int32 UnionAdjacentContoursXld_X = 10;
        Int32 UnionAdjacentContoursXld_Y = 1;
        private void checkBox_显示生成图像_CheckedChanged(object sender, EventArgs e)
        {
            _显示生成图像 = checkBox_显示生成图像.Checked;
        }
        private void checkBox_显示二值化选区_CheckedChanged(object sender, EventArgs e)
        {
            _显示二值化选区 = checkBox_显示二值化选区.Checked;
        }
        private void _Emphasize_1_X__ValueChanged(object sender, EventArgs e)
        { Emphasize_1_X = (Int32)_Emphasize_1_X_.Value; }
        private void _Emphasize_1_Y__ValueChanged(object sender, EventArgs e)
        { Emphasize_1_Y = (Int32)_Emphasize_1_Y_.Value; }
        private void _Emphasize_1_Z__ValueChanged(object sender, EventArgs e)
        { Emphasize_1_Z = (double)_Emphasize_1_Z_.Value; }

        private void _MODEL_Threshold_L_ValueChanged(object sender, EventArgs e)
        { MODEL_Threshold_L = (Int32)_MODEL_Threshold_L.Value; }
        private void _MODEL_AREA_L_ValueChanged(object sender, EventArgs e)
        { MODEL_AREA_L = (Int32)_MODEL_AREA_L.Value; }

        private void _MODEL_RegionDilation_ValueChanged(object sender, EventArgs e)
        { MODEL_RegionDilation = (Int32)_MODEL_RegionDilation.Value; }

        //private void _MODEL_RegionToBin_L_ValueChanged(object sender, EventArgs e)
        //{ MODEL_RegionToBin_L = (Int32)_MODEL_RegionToBin_L.Value; }
        //private void _MODEL_RegionToBin_H_ValueChanged(object sender, EventArgs e)
        //{ MODEL_RegionToBin_H = (Int32)_MODEL_RegionToBin_H.Value; }

        private void _模板匹配相似度__ValueChanged(object sender, EventArgs e)
        { _模板匹配相似度 = (double)_模板匹配相似度_.Value; }
        private void _模板扫描精度__ValueChanged(object sender, EventArgs e)
        { _模板扫描精度 = (double)_模板扫描精度_.Value; }
        private void _显示轮廓线数量__CheckedChanged(object sender, EventArgs e)
        { _显示轮廓线数量 = _显示轮廓线数量_.Checked; }
        private void _SelectShapeXld_Min__ValueChanged(object sender, EventArgs e)
        { SelectShapeXld_Min = (Int32)_SelectShapeXld_Min_.Value; }
        //private void _SelectShapeXld_Max__ValueChanged(object sender, EventArgs e)
        //{ SelectShapeXld_Max = (Int32)_SelectShapeXld_Max_.Value; }
        private void _UnionAdjacentContoursXld_X__ValueChanged(object sender, EventArgs e)
        { UnionAdjacentContoursXld_X = (Int32)_UnionAdjacentContoursXld_X_.Value; }
        private void _UnionAdjacentContoursXld_Y__ValueChanged(object sender, EventArgs e)
        { UnionAdjacentContoursXld_Y = (Int32)_UnionAdjacentContoursXld_Y_.Value; }
        #endregion
        //
        #region 工件定位参数
        //工件定位参数
        bool _显示选中特征 = false;
        bool _显示选中焊接端子I = false;
        private void _显示选中特征__CheckedChanged(object sender, EventArgs e)
        { _显示选中特征 = _显示选中特征_.Checked; }
        private void _显示选中焊接端子__CheckedChanged(object sender, EventArgs e)
        { _显示选中焊接端子I = _显示选中焊接端子_.Checked; }
        #endregion
        //
        #region 重新定位矩形坐标参数
        //重新增强图像
        Int32 Emphasize_2_X = 5;
        Int32 Emphasize_2_Y = 3;
        double Emphasize_2_Z = 1.0;
        //焊接点定位
        Int32 Image_Analyse_rectangle2_L1 = 28;
        Int32 Image_Analyse_rectangle2_L2 = 100;
        Int32 Image_Analyse_rectangle3_LL1 = -9;
        Int32 Image_Analyse_rectangle3_LL2 = -9;
        //显示矩形绘制状态
        bool _显示矩形绘制状态 = false;
        Int32 文字显示位置 = 100;
        private void _Emphasize_2_X__ValueChanged(object sender, EventArgs e)
        { Emphasize_2_X = (Int32)_Emphasize_2_X_.Value; }
        private void _Emphasize_2_Y__ValueChanged(object sender, EventArgs e)
        { Emphasize_2_Y = (Int32)_Emphasize_2_Y_.Value; }
        private void _Emphasize_2_Z__ValueChanged(object sender, EventArgs e)
        { Emphasize_2_Z = (double)_Emphasize_2_Z_.Value; }
        private void _Image_Analyse_rectangle2_L1__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_rectangle2_L1 = (Int32)_Image_Analyse_rectangle2_L1_.Value; }
        private void _Image_Analyse_rectangle2_L2__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_rectangle2_L2 = (Int32)_Image_Analyse_rectangle2_L2_.Value; }
        private void _Image_Analyse_rectangle3_LL1__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_rectangle3_LL1 = (Int32)_Image_Analyse_rectangle3_LL1_.Value; }
        private void _Image_Analyse_rectangle3_LL2__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_rectangle3_LL2 = (Int32)_Image_Analyse_rectangle3_LL2_.Value; }
        private void _显示焊接标定矩形__CheckedChanged(object sender, EventArgs e)
        { _显示矩形绘制状态 = _显示焊接标定矩形_.Checked; }
        private void _文字显示位置_ValueChanged(object sender, EventArgs e)
        { 文字显示位置 = (Int32)_文字显示位置.Value; }
        #endregion
        //
        #region 焊接段子识别、容错率参数、设定
        Int32 Image_Analyse_Threshold_1_H = 52;
        double ClosingCircle_1 = 1.0;
        Int32 Image_Analyse_ss_SR1_AREA_L = 260;
        double Image_Analyse_ss_SR1_HEIGHT_L = 11.8;
        //漏铜干扰修补
        double ER_DR_XX = 2.0;
        //漏铜面积修补
        double ClosingCircle_2 = 1.0;//ClosingCircle_2 - ClosingCircle_3
        Int32 Image_Analyse_Threshold_2_L = 42;
        //
        double _原始点行坐标 = 550.00;
        double _原来点列坐标 = 623.00;

        //=====================不插孔接线盒=========================上插孔参数
        double _原始点行坐标_II = 570.00;
        double _原来点列坐标_II = 623.00;

        //容错率
        Int32 Image_Analyse_ss_SR2_AREA_L_I = 190;
        Int32 Image_Analyse_ss_SR2_AREA_L_II = 210;
        //
        Int32 Image_Analyse_Threshold_3_L = 32;
        Int32 Image_Analyse_ss_SR3_AREA_L = 280;
        double Image_Analyse_ss_SR3_ss_SR3_WIDTH_L = 43.8;

        //======================不插孔接线盒========================上插孔参数
        Int32 HV_threshold_4_H = 42;
        Int32 HV_ss_SR4_AREA_L = 280;
        double HV_ss_SR4_WIDTH_L = 15.8;

        //=============不插孔接线盒===========下参数
        //接线盒类型
        int X_TINIT = 0;
        //临时控制参数,二值化强度
        Int32 HV_threshold_3_L = 240;
        //区域面积
        Int32 HV_area_3 = 200;
        //区域圆度
        double HV_convexity = 0.81;
        //接线盒类型控制
        private void checkBox_接线盒是否插孔_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_接线盒是否插孔.Checked)
            {
                X_TINIT = 1;
            }
            else
            {
                X_TINIT = 0;
            }
        }

        //=============不插孔接线盒===========下参数
        private void _HV_threshold_3_L_ValueChanged(object sender, EventArgs e)
        { HV_threshold_3_L = (Int32)_HV_threshold_3_L.Value; }
        private void _HV_area_3_ValueChanged(object sender, EventArgs e)
        { HV_area_3 = (Int32)_HV_area_3.Value; }
        private void _HV_convexity_ValueChanged(object sender, EventArgs e)
        { HV_convexity = (double)_HV_convexity.Value; }

        private void _Image_Analyse_Threshold_1_H__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_Threshold_1_H = (Int32)_Image_Analyse_Threshold_1_H_.Value; }
        private void 漏铜干扰修补_ValueChanged(object sender, EventArgs e)
        {
            ER_DR_XX = (double)漏铜干扰修补.Value;
        }
        private void 焊接点插口处修补_ValueChanged(object sender, EventArgs e)
        {
            ClosingCircle_1 = (double)焊接点插口处修补.Value;
        }
        private void _NEW_Image_Analyse_ss_SR1_AREA_L__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR1_AREA_L = (Int32)_NEW_Image_Analyse_ss_SR1_AREA_L_.Value; }
        private void _Image_Analyse_ss_SR1_HEIGHT_L__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR1_HEIGHT_L = (double)_Image_Analyse_ss_SR1_HEIGHT_L_.Value; }
        private void _Image_Analyse_Threshold_2_L__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_Threshold_2_L = (Int32)_Image_Analyse_Threshold_2_L_.Value; }
        private void 漏铜面积修补_ValueChanged(object sender, EventArgs e)
        {
            ClosingCircle_2 = (double)漏铜面积修补.Value;
        }
        private void _Image_Analyse_ss_SR2_AREA_L_I__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR2_AREA_L_I = (Int32)_Image_Analyse_ss_SR2_AREA_L_I_.Value; }
        private void _Image_Analyse_ss_SR2_AREA_L_II__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR2_AREA_L_II = (Int32)_Image_Analyse_ss_SR2_AREA_L_II_.Value; }
        private void _Image_Analyse_Threshold_3_L__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_Threshold_3_L = (Int32)_Image_Analyse_Threshold_3_L_.Value; }
        private void _Image_Analyse_ss_SR3_ss_SR3_WIDTH_L_ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR3_ss_SR3_WIDTH_L = (double)_Image_Analyse_ss_SR3_ss_SR3_WIDTH_L.Value; }
        private void _Image_Analyse_ss_SR3_AREA_L__ValueChanged(object sender, EventArgs e)
        { Image_Analyse_ss_SR3_AREA_L = (Int32)_Image_Analyse_ss_SR3_AREA_L_.Value; }

        //=============================不插孔接线盒============================================上插孔参数
        private void _HV_threshold_4_H_ValueChanged(object sender, EventArgs e)
        {
            HV_threshold_4_H = (Int32)_HV_threshold_4_H.Value;
        }
        private void _HV_ss_SR4_AREA_L_ValueChanged(object sender, EventArgs e)
        {
            HV_ss_SR4_AREA_L = (Int32)_HV_ss_SR4_AREA_L.Value;
        }
        private void _HV_ss_SR4_WIDTH_L_ValueChanged(object sender, EventArgs e)
        {
            HV_ss_SR4_WIDTH_L = (Int32)_HV_ss_SR4_WIDTH_L.Value;
        }

        private void 原始点行坐标_ValueChanged(object sender, EventArgs e)
        {
            _原始点行坐标 = (double)原始点行坐标.Value;
        }
        private void 原来点列坐标_ValueChanged(object sender, EventArgs e)
        {
            _原来点列坐标 = (double)原来点列坐标.Value;
        }

        //================================不插孔接线盒=========================================上插孔参数
        private void 原始点行坐标_II_ValueChanged(object sender, EventArgs e)
        {
            _原始点行坐标_II = (double)原始点行坐标_II.Value;
        }
        private void 原来点列坐标_II_ValueChanged(object sender, EventArgs e)
        {
            _原来点列坐标_II = (double)原来点列坐标_II.Value;
        }
        #endregion
        //
        #region 识别等级设定
        Int32 _str_out = 1;
        private void _合格等级高_CheckedChanged(object sender, EventArgs e)
        { _str_out = 2; }
        private void _合格等级中_CheckedChanged(object sender, EventArgs e)
        { _str_out = 1; }
        private void _合格等级低_CheckedChanged(object sender, EventArgs e)
        { _str_out = 0; }
        #endregion
        //
        #region 模板定位调试
        private bool _INIT_MODELS_IMAGE()
        {
            hv_ErrorNum = 2;
            // Error variable 'hv_ErrorNum' activated
            // (dev_)set_check ("~give_error")
            try
            {
                //读取目录前一段字符串
                hv_ImageName = _模板目录;
            }
            catch (HalconException e)
            {
                hv_ErrorNum = e.GetErrorNumber();
                //if ((int)hv_ErrorNum < 0)
                //    throw e;
            }
            // Error variable 'hv_ErrorNum' deactivated
            // (dev_)set_check ("give_error")

            if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
            {
                //读取图像
                ho_MODELS_Image.Dispose();
                try
                {
                    HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                }
                catch (HalconException e)
                {
                    hv_ErrorNum = e.GetErrorNumber();
                }
                if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                {
                    return true;
                }
                else
                {
                    //disp_message(hWindowControl1.HalconWindow, "读取模板路径错误！", "window", 20, 100, "red", "false");
                    return false;
                }
            }
            else
            {
                // disp_message(hWindowControl1.HalconWindow, "读取模板路径错误！", "window", 20, 100, "red", "false");
                return false;
            }
        }
        #endregion
        //
        #region 模板调试
        private bool _STAR_MODELS()
        {
            //int IO = 0;

            string star_c1 = "OK";
            string star_c2 = "OK";
            string star_c3 = "OK";
            string star_c4 = "OK";

            ho_ImageZoom1.Dispose();
            HOperatorSet.ZoomImageSize(ho_MODELS_Image, out ho_ImageZoom1, R_width / 2, R_height / 2, "constant");

            if (_启用匹配调试_)
            {
                //显示1/2图像
                HOperatorSet.DispObj(ho_ImageZoom1, hWindowControl1.HalconWindow);
                disp_message(hWindowControl1.HalconWindow, "调试......", "window", 20, 100, "green", "false");
            }

            ho_ImageEmphasize.Dispose();
            HOperatorSet.Emphasize(ho_ImageZoom1, out ho_ImageEmphasize, Emphasize_1_X, Emphasize_1_Y, Emphasize_1_Z);
            ho_Red.Dispose();
            ho_Green.Dispose();
            ho_Blue.Dispose();
            HOperatorSet.Decompose3(ho_ImageEmphasize, out ho_Red, out ho_Green, out ho_Blue);

            //2
            ho_ROI_0.Dispose();
            HOperatorSet.GenRectangle1(out ho_ROI_0, ROI_0_R1, ROI_0_C1, ROI_0_R2, ROI_0_C2);
            //1
            ho_ROI_1.Dispose();
            HOperatorSet.GenRectangle1(out ho_ROI_1, ROI_1_R1, ROI_1_C1, ROI_1_R2, ROI_1_C2);

            ho_RegionDifference.Dispose();
            HOperatorSet.Difference(ho_ROI_0, ho_ROI_1, out ho_RegionDifference);
            ho_ImageROIRing.Dispose();
            HOperatorSet.ReduceDomain(ho_Green, ho_RegionDifference, out ho_ImageROIRing);

            if (_显示定标矩形)
            {
                //2
                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                HOperatorSet.DispObj(ho_ROI_0, hWindowControl1.HalconWindow);
                //1
                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                HOperatorSet.DispObj(ho_ROI_1, hWindowControl1.HalconWindow);
            }

            if (_显示相减矩形)
            {
                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "white");
                HOperatorSet.DispObj(ho_RegionDifference, hWindowControl1.HalconWindow);
            }

            HOperatorSet.ClearAllShapeModels();

            hv_ErrorNum = 2;
            // Error variable 'hv_ErrorNum' activated
            // (dev_)set_check ("~give_error")
            try
            {
                //hv_ErrorNum = 2;
                HOperatorSet.CreateShapeModel(ho_ImageROIRing, "auto", (new HTuple(0)).TupleRad(), (new HTuple(360)).TupleRad(), "auto",
                    "auto", "use_polarity", (new HTuple(Model_Threshold_1_L)).TupleConcat(Model_Threshold_1_H), "auto", out hv_ModelIDRing);
            }
            catch (HalconException e)
            {
                hv_ErrorNum = e.GetErrorNumber();
                //if ((int)hv_ErrorNum < 0)
                //    throw e;
            }
            // Error variable 'hv_ErrorNum' deactivated
            // (dev_)set_check ("give_error")

            if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
            {
                //HOperatorSet.DispObj(ho_ImageZoom1, hWindowControl1.HalconWindow);
                ho_PyramidImage.Dispose();
                ho_ModelRegionRing.Dispose();
                HOperatorSet.InspectShapeModel(ho_ImageROIRing, out ho_PyramidImage, out ho_ModelRegionRing, 1, 30);
                //HOperatorSet.DispObj(ho_ImageZoom1, hWindowControl1.HalconWindow);
                ho_ShapeModelRing.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ShapeModelRing, hv_ModelIDRing, 1);

                if (_显示轮廓线数量)
                {
                    HOperatorSet.CountObj(ho_ShapeModelRing, out hv_NumContoursRing);
                    disp_message(hWindowControl1.HalconWindow, "前段轮廓线数量：" + hv_NumContoursRing, "window", 50, 100, "green", "false");
                }

                //选择XLD特征
                ho_SelectedXLD.Dispose();
                HOperatorSet.SelectShapeXld(ho_ShapeModelRing, out ho_SelectedXLD, "area", "and", SelectShapeXld_Min, SelectShapeXld_Max);

                //合并XLD
                ho_UnionContours.Dispose();
                HOperatorSet.UnionAdjacentContoursXld(ho_SelectedXLD, out ho_UnionContours, UnionAdjacentContoursXld_X, UnionAdjacentContoursXld_Y, "attr_keep");
                HOperatorSet.CountObj(ho_UnionContours, out hv_NumContoursRing);


                if (_XT1)
                {
                    //********************************
                    //模板匹配焊接盘部分特征、转换到矩形
                    HOperatorSet.FindShapeModels(ho_Green, hv_ModelIDRing, (new HTuple(0)).TupleRad()
                        , (new HTuple(360)).TupleRad(), _模板匹配相似度, 1, 0.5, "least_squares", 0, _模板扫描精度, out hv_RowCheck,
                        out hv_ColumnCheck, out hv_AngleCheck, out hv_Score1, out hv_Model);
                    // Error variable 'hv_ErrorNum' activated
                    hv_ErrorNum = 2;
                    // (dev_)set_check ("~give_error")
                    try
                    {
                        hv_ErrorNum = 2;
                        HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject0);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum = e.GetErrorNumber();
                        if ((int)hv_ErrorNum < 0)
                            throw e;
                    }
                    // Error variable 'hv_ErrorNum' deactivated
                    // (dev_)set_check ("give_error")
                    if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                    {
                        //创建XLD轮廓
                        ho_ContoursAffinTrans.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ShapeModelRing, out ho_ContoursAffinTrans, hv_MovementOfObject0);
                        HOperatorSet.DispObj(ho_ContoursAffinTrans, hWindowControl1.HalconWindow);
                    }
                    //********************************
                }

                if ((int)((new HTuple(hv_ErrorNum.TupleEqual(2))).TupleAnd(new HTuple(hv_NumContoursRing.TupleEqual(1)))) != 0)
                {
                    hv_ErrorNum1 = 2;
                    try
                    {
                        ho_Rectangle_X1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle_X1, _hv_Row_X1, _hv_Column_X1, _hv_Phi_X1, _hv_Length1_X1, _hv_Length2_X1);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum1 = e.GetErrorNumber();
                        star_c1 = "ERROR";

                        _hv_Row_X1 = HVROW_X1;
                        _hv_Column_X1 = HVCOLUMN_X1;
                        _hv_Phi_X1 = HVPHI_X1;
                        _hv_Length1_X1 = HVLENGTH1_X1;
                        _hv_Length2_X1 = HVLENGTH2_X1;

                    }

                    hv_ErrorNum2 = 2;
                    try
                    {
                        ho_Rectangle_X2.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle_X2, _hv_Row_X2, _hv_Column_X2, _hv_Phi_X2, _hv_Length1_X2, _hv_Length2_X2);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum2 = e.GetErrorNumber();
                        star_c2 = "ERROR";

                        _hv_Row_X2 = HVROW_X2;
                        _hv_Column_X2 = HVCOLUMN_X2;
                        _hv_Phi_X2 = HVPHI_X2;
                        _hv_Length1_X2 = HVLENGTH1_X2;
                        _hv_Length2_X2 = HVLENGTH2_X2;

                    }

                    hv_ErrorNum3 = 2;
                    try
                    {
                        ho_Rectangle_X3.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle_X3, _hv_Row_X3, _hv_Column_X3, _hv_Phi_X3, _hv_Length1_X3, _hv_Length2_X3);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum3 = e.GetErrorNumber();
                        star_c3 = "ERROR";

                        _hv_Row_X3 = HVROW_X3;
                        _hv_Column_X3 = HVCOLUMN_X3;
                        _hv_Phi_X3 = HVPHI_X3;
                        _hv_Length1_X3 = HVLENGTH1_X3;
                        _hv_Length2_X3 = HVLENGTH2_X3;

                    }

                    hv_ErrorNum4 = 2;
                    try
                    {
                        ho_Rectangle_X4.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle_X4, _hv_Row_X4, _hv_Column_X4, _hv_Phi_X4, _hv_Length1_X4, _hv_Length2_X4);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum4 = e.GetErrorNumber();
                        star_c4 = "ERROR";

                        _hv_Row_X4 = HVROW_X4;
                        _hv_Column_X4 = HVCOLUMN_X4;
                        _hv_Phi_X4 = HVPHI_X4;
                        _hv_Length1_X4 = HVLENGTH1_X4;
                        _hv_Length2_X4 = HVLENGTH2_X4;

                    }

                    if (((int)(new HTuple(hv_ErrorNum1.TupleEqual(2))) != 0) && ((int)(new HTuple(hv_ErrorNum2.TupleEqual(2))) != 0)
                         && ((int)(new HTuple(hv_ErrorNum3.TupleEqual(2))) != 0) && ((int)(new HTuple(hv_ErrorNum4.TupleEqual(2))) != 0))
                    {
                        ho_RegionUnion1.Dispose();
                        HOperatorSet.Union2(ho_Rectangle_X1, ho_Rectangle_X2, out ho_RegionUnion1);
                        ho_RegionUnion2.Dispose();
                        HOperatorSet.Union2(ho_Rectangle_X3, ho_Rectangle_X4, out ho_RegionUnion2);
                        ho_RegionUnion3.Dispose();
                        HOperatorSet.Union2(ho_RegionUnion1, ho_RegionUnion2, out ho_RegionUnion3);
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_RegionUnion3, out ho_Contours, "border");

                        //加入显示部分
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                        HOperatorSet.DispObj(ho_Contours, hWindowControl1.HalconWindow);

                        if (_标注位置V1)
                        {
                            disp_message(hWindowControl1.HalconWindow, "V1", "window", _hv_Row_X1 + 文字显示位置 - 20,
                                _hv_Column_X1 + 文字显示位置 - 20, "green", "false");
                        }
                        else if (_标注位置V2)
                        {
                            disp_message(hWindowControl1.HalconWindow, "V2", "window", _hv_Row_X2 + 文字显示位置 - 20,
                                _hv_Column_X2 + 文字显示位置 - 20, "green", "false");
                        }
                        else if (_标注位置V3)
                        {
                            disp_message(hWindowControl1.HalconWindow, "V3", "window", _hv_Row_X3 + 文字显示位置 - 20,
                                _hv_Column_X3 + 文字显示位置 - 20, "green", "false");
                        }
                        else if (_标注位置V4)
                        {
                            disp_message(hWindowControl1.HalconWindow, "V4", "window", _hv_Row_X4 + 文字显示位置 - 20,
                                _hv_Column_X4 + 文字显示位置 - 20, "green", "false");
                        }

                        return true;
                    }
                    else
                    {
                        disp_message(hWindowControl1.HalconWindow, "调试、绘制模板标定错误，请重新绘制！\n" +
                            "V1：" + star_c1 + "    " + "V2：" + star_c2 + "    " + "V3：" + star_c3 + "    " +
                            "V4：" + star_c4, "window", 80, 100, "red", "false");
                        if (_启用匹配调试_)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    disp_message(hWindowControl1.HalconWindow, "调试、制作模板标定错误-2！", "window", 80, 100, "red", "false");
                    if (_启用匹配调试_)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                disp_message(hWindowControl1.HalconWindow, "调试、制作模板标定错误-1！", "window", 80, 100, "red", "false");
                if (_启用匹配调试_)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


        }
        #endregion

        //错误标记
        Int32 hv_out_ERROR;
        #region 焊接端子识别
        //ho_in_image2
        public void Image_Analyse(HObject ho_in_saturation, HObject ho_in_image1, HObject ho_in_green, HObject ho_ID_REGIONS2,
            HTuple hv_ROW, HTuple hv_COLUMN, HTuple hv_PHI, HTuple hv_LENGTH1, HTuple hv_LENGTH2, HTuple hv_rectangle2_L1,
            HTuple hv_rectangle2_L2, HTuple hv_rectangle3_LL1, HTuple hv_rectangle3_LL2,
            HTuple hv_threshold_1_H, HTuple hv_r_closing_1, HTuple hv_ss_SR1_AREA_L, HTuple hv_ss_SR1_HEIGHT_L,
            HTuple hv_threshold_2_L, HTuple hv_r_closing_2, HTuple hv_ss_SR2_AREA_L, HTuple hv_threshold_3_L,
            HTuple hv_r_closing_3, HTuple hv_ss_SR3_AREA_L, HTuple hv_ss_SR3_WIDTH_L, HTuple hv_X_INIT,
            HTuple hv_threshold_4_H, HTuple hv_ss_SR4_AREA_L, HTuple hv_ss_SR4_WIDTH_L, HTuple hv_ER_DR_XX/*, HTuple hv_in_WindowID, */)
        {
            if (hv_X_INIT > 0)
            {

                // Local iconic variables 
                HObject ho_Rectangle1;
                HObject ho_Rectangle2;

                HObject ho_ImageIS7 = null;
                HObject ho_ImageIS8 = null;

                HObject ho_Regions = null;
                HObject ho_ConnectedRegions = null;

                HObject ho_out_sr1 = null;
                HObject ho_out_sr2 = null;
                HObject ho_out_sr3 = null;
                HObject ho_OUT_STAR = null;

                HObject ho_RegionDilation = null, ho_RD = null;
                HObject ho_RegionDifference = null, ho_RegionFillUp = null;
                HObject ho_RegionUnion = null;

                // Stack for temporary objects 
                HObject[] OTemp = new HObject[20];
                long SP_O = 0;

                HTuple hv_Number_D1;
                //HTuple hv_Number_D2;

                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_out_sr1);
                HOperatorSet.GenEmptyObj(out ho_out_sr2);
                HOperatorSet.GenEmptyObj(out ho_out_sr3);

                HOperatorSet.GenEmptyObj(out ho_OUT_STAR);

                HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                HOperatorSet.GenEmptyObj(out ho_Rectangle2);

                HOperatorSet.GenEmptyObj(out ho_ImageIS7);
                HOperatorSet.GenEmptyObj(out ho_ImageIS8);

                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference);

                HOperatorSet.GenEmptyObj(out ho_RegionDilation);
                HOperatorSet.GenEmptyObj(out ho_RD);

                HOperatorSet.GenEmptyObj(out ho_RegionUnion);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);

                //绘制特定矩形
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                hv_ErrorNum = 2;
                // Error variable 'hv_ErrorNum' activated
                // (dev_)set_check ("~give_error")
                try
                {
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_ROW, hv_COLUMN, hv_PHI, hv_LENGTH1, hv_LENGTH2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_ROW, hv_COLUMN, hv_PHI, hv_rectangle2_L1, hv_rectangle2_L2);
                }
                catch (HalconException e)
                {
                    hv_ErrorNum = e.GetErrorNumber();
                }

                //显示矩形绘制状态
                if (_显示矩形绘制状态)
                {
                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                    HOperatorSet.DispObj(ho_Rectangle1, hWindowControl1.HalconWindow);
                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_Rectangle2, hWindowControl1.HalconWindow);
                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "blue");
                    //disp_message(hWindowControl1.HalconWindow, "调试......", "window", 20, 100, "green", "false");
                }

                #region 不插孔接线盒（目前不准确）

                if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                {
                    //判断未焊接或翘起（曝光处）
                    //全部
                    ho_ImageIS7.Dispose();
                    HOperatorSet.ReduceDomain(ho_in_image1, ho_Rectangle1, out ho_ImageIS7);
                    //大或小圆形
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageIS7, out ho_Regions, HV_threshold_3_L, 255);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
                    ho_out_sr1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr1, "area", "and", HV_area_3, 9999999);
                    ho_out_sr2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr2, (new HTuple("area")).TupleConcat(
                        "convexity"), "and", (new HTuple(HV_area_3 / 2)).TupleConcat(HV_convexity), (new HTuple(9999999)).TupleConcat(1));

                    //上面，汇流条与焊接点插口处
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_Rectangle2, out ho_RegionDilation, 100, 1);
                    ho_RD.Dispose();
                    HOperatorSet.Intersection(ho_RegionDilation, ho_ID_REGIONS2, out ho_RD);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Intersection(ho_Rectangle2, ho_RD, out ho_RegionDifference);
                    ho_ImageIS8.Dispose();
                    HOperatorSet.ReduceDomain(ho_in_image1, ho_RegionDifference, out ho_ImageIS8);
                    //大块区域
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageIS8, out ho_Regions, 0, hv_threshold_4_H);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_Regions, out ho_RegionFillUp);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
                    ho_out_sr3.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr3, (new HTuple("area")).TupleConcat(
                        "width"), "and", hv_ss_SR4_AREA_L.TupleConcat(hv_ss_SR4_WIDTH_L), (new HTuple(9999)).TupleConcat(9999));

                    ho_OUT_STAR.Dispose();
                    HOperatorSet.Union2(ho_out_sr1, ho_out_sr2, out ho_OUT_STAR);
                    OTemp[SP_O] = ho_OUT_STAR.CopyObj(1, -1);
                    SP_O++;
                    ho_OUT_STAR.Dispose();
                    HOperatorSet.Union2(ho_out_sr3, OTemp[SP_O - 1], out ho_OUT_STAR);
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                    HOperatorSet.CountObj(ho_OUT_STAR, out hv_Number_D1);
                    if ((int)(new HTuple(hv_Number_D1.TupleGreater(0))) != 0)
                    {
                        //dev_display (in_image1)
                        ho_RegionUnion.Dispose();
                        HOperatorSet.Union2(ho_RD, ho_Rectangle1, out ho_RegionUnion);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "blue");
                        HOperatorSet.DispObj(ho_RegionUnion, hWindowControl1.HalconWindow);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_OUT_STAR, hWindowControl1.HalconWindow);
                        disp_message(hWindowControl1.HalconWindow, "NG", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "red", "false");
                        hv_out_ERROR = hv_out_ERROR + 1;
                    }
                    else
                    {
                        disp_message(hWindowControl1.HalconWindow, "OK", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "green", "false");
                    }

                    ho_out_sr1.Dispose();
                    ho_out_sr2.Dispose();
                    ho_out_sr3.Dispose();

                    ho_OUT_STAR.Dispose();

                    ho_Rectangle1.Dispose();
                    ho_Rectangle2.Dispose();

                    ho_ImageIS7.Dispose();
                    ho_ImageIS8.Dispose();

                    ho_Regions.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_RegionDifference.Dispose();

                    ho_RegionDilation.Dispose();
                    ho_RD.Dispose();

                    ho_RegionUnion.Dispose();
                    ho_RegionFillUp.Dispose();
                    return;
                }
                else
                {
                    disp_message(hWindowControl1.HalconWindow, "焊接端子重定位错误！", "window", 20, 100, "red", "false");
                    return;
                }

                #endregion

            }
            else
            {
                // Local iconic variables 

                HObject ho_Rectangle1;
                HObject ho_Rectangle2;
                HObject ho_Rectangle3;
                HObject ho_RegionDifference1;
                HObject ho_RegionDifference2;
                HObject ho_ImageIS3;
                HObject ho_ImageIS2;
                HObject ho_ImageIS1;
                HObject ho_Regions;

                HObject ho_RegionEEX;
                HObject ho_RegionDilationXX;

                HObject ho_RegionFillUp;
                HObject ho_RegionClosing;
                HObject ho_ConnectedRegions;

                HObject ho_out_sr1;
                HObject ho_out_sr2;
                HObject ho_out_sr3;

                // Local control variables 

                HTuple hv_Number_D1;
                HTuple hv_Number_D2;
                HTuple hv_Number_D3;

                //HTuple hv_out_ERROR;

                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_out_sr1);
                HOperatorSet.GenEmptyObj(out ho_out_sr2);
                HOperatorSet.GenEmptyObj(out ho_out_sr3);

                HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_Rectangle3);

                HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
                HOperatorSet.GenEmptyObj(out ho_RegionDifference2);

                HOperatorSet.GenEmptyObj(out ho_ImageIS3);
                HOperatorSet.GenEmptyObj(out ho_ImageIS2);
                HOperatorSet.GenEmptyObj(out ho_ImageIS1);

                HOperatorSet.GenEmptyObj(out ho_Regions);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);

                HOperatorSet.GenEmptyObj(out ho_RegionEEX);
                HOperatorSet.GenEmptyObj(out ho_RegionDilationXX);
                HOperatorSet.GenEmptyObj(out ho_RegionClosing);
                //绘制特定矩形
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                ho_Rectangle3.Dispose();

                hv_ErrorNum = 2;
                // Error variable 'hv_ErrorNum' activated
                // (dev_)set_check ("~give_error")
                try
                {
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_ROW, hv_COLUMN, hv_PHI, hv_LENGTH1, hv_LENGTH2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_ROW, hv_COLUMN, hv_PHI, hv_rectangle2_L1, hv_rectangle2_L2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_ROW, hv_COLUMN, hv_PHI, hv_LENGTH1 + hv_rectangle3_LL1, hv_LENGTH2 + hv_rectangle3_LL2);
                }
                catch (HalconException e)
                {
                    hv_ErrorNum = e.GetErrorNumber();
                }

                if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                {

                    //载取图像
                    ho_RegionDifference1.Dispose();
                    HOperatorSet.Difference(ho_Rectangle1, ho_Rectangle2, out ho_RegionDifference1);
                    ho_ImageIS2.Dispose();
                    HOperatorSet.ReduceDomain(ho_in_saturation, ho_RegionDifference1, out ho_ImageIS2);
                    ho_ImageIS1.Dispose();
                    HOperatorSet.ReduceDomain(ho_in_green, ho_Rectangle3, out ho_ImageIS1);
                    ho_RegionDifference2.Dispose();
                    HOperatorSet.Difference(ho_Rectangle1, ho_RegionDifference1, out ho_RegionDifference2);
                    ho_ImageIS3.Dispose();
                    HOperatorSet.ReduceDomain(ho_in_saturation, ho_RegionDifference2, out ho_ImageIS3);

                    //显示矩形绘制状态
                    if (_显示矩形绘制状态)
                    {
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "white");
                        HOperatorSet.DispObj(ho_Rectangle1, hWindowControl1.HalconWindow);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_Rectangle2, hWindowControl1.HalconWindow);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "blue");
                        HOperatorSet.DispObj(ho_Rectangle3, hWindowControl1.HalconWindow);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "black");
                        HOperatorSet.DispObj(ho_RegionDifference2, hWindowControl1.HalconWindow);
                        //disp_message(hWindowControl1.HalconWindow, "调试......", "window", 20, 100, "green", "false");
                    }

                    //汇流条与焊接点插口处
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageIS1, out ho_Regions, 0, hv_threshold_1_H);
                    //拉扯图像
                    ho_RegionEEX.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Regions, out ho_RegionEEX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionDilationXX.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionEEX, out ho_RegionDilationXX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionDilationXX, out ho_RegionFillUp);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, hv_r_closing_1);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_out_sr1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr1, (new HTuple("area")).TupleConcat(
                        "height"), "and", hv_ss_SR1_AREA_L.TupleConcat(hv_ss_SR1_HEIGHT_L), (new HTuple(9999)).TupleConcat(9999));

                    //两边漏铜面积
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageIS2, out ho_Regions, hv_threshold_2_L, 255);
                    //拉扯图像
                    ho_RegionEEX.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Regions, out ho_RegionEEX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionDilationXX.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionEEX, out ho_RegionDilationXX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionDilationXX, out ho_RegionFillUp);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, hv_r_closing_2);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_out_sr2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr2, "area", "and", hv_ss_SR2_AREA_L, 9999);

                    //上边漏铜面积
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageIS3, out ho_Regions, hv_threshold_3_L, 255);
                    //拉扯图像
                    ho_RegionEEX.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Regions, out ho_RegionEEX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionDilationXX.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionEEX, out ho_RegionDilationXX, hv_ER_DR_XX, hv_ER_DR_XX);
                    //
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionDilationXX, out ho_RegionFillUp);
                    ho_RegionClosing.Dispose();
                    //                                                                hv_r_closing_3
                    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, hv_r_closing_2);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_out_sr3.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_out_sr3, (new HTuple("area")).TupleConcat(
                        "width"), "and", hv_ss_SR3_AREA_L.TupleConcat(hv_ss_SR3_WIDTH_L), (new HTuple(9999)).TupleConcat(9999));

                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_out_sr1, hWindowControl1.HalconWindow);

                    HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                    HOperatorSet.DispObj(ho_out_sr2, hWindowControl1.HalconWindow);
                    HOperatorSet.DispObj(ho_out_sr3, hWindowControl1.HalconWindow);

                    HOperatorSet.CountObj(ho_out_sr1, out hv_Number_D1);
                    HOperatorSet.CountObj(ho_out_sr2, out hv_Number_D2);
                    HOperatorSet.CountObj(ho_out_sr3, out hv_Number_D3);

                    hv_opt_str = _str_out;

                    if ((int)(new HTuple(hv_opt_str.TupleEqual(2))) != 0)
                    {
                        //任何一个没焊接 = 不合格
                        if ((int)((new HTuple((new HTuple(hv_Number_D1.TupleGreater(0))).TupleOr(new HTuple(hv_Number_D2.TupleGreater(
                             0))))).TupleOr(new HTuple(hv_Number_D3.TupleGreater(0)))) != 0)
                        {
                            disp_message(hWindowControl1.HalconWindow, "NG", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "red", "false");
                            hv_out_ERROR = hv_out_ERROR + 1;
                        }
                        else
                        {
                            disp_message(hWindowControl1.HalconWindow, "OK", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "green", "false");
                        }

                    }
                    else if ((int)(new HTuple(hv_opt_str.TupleEqual(1))) != 0)
                    {
                        //中、上或者左右两边都没焊接 = 不合格。
                        if ((int)((new HTuple((new HTuple(hv_Number_D1.TupleGreater(0))).TupleOr(new HTuple(hv_Number_D2.TupleGreater(
                                1))))).TupleOr(new HTuple(hv_Number_D3.TupleGreater(0)))) != 0)
                        {
                            disp_message(hWindowControl1.HalconWindow, "NG", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "red", "false");
                            hv_out_ERROR = hv_out_ERROR + 1;
                        }
                        else
                        {
                            disp_message(hWindowControl1.HalconWindow, "OK", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "green", "false");
                        }

                    }
                    else
                    {

                        //修改了抵挡的判断，在中间没有焊接的情况下、左右和上必须焊接。
                        if ((int)(new HTuple(hv_Number_D1.TupleGreater(0))) != 0)
                        {
                            if ((int)((new HTuple(hv_Number_D2.TupleGreater(0))).TupleOr(new HTuple(hv_Number_D3.TupleGreater(0)))) != 0)
                            {
                                disp_message(hWindowControl1.HalconWindow, "NG", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "red", "false");
                                hv_out_ERROR = hv_out_ERROR + 1;
                            }
                            else
                            {
                                disp_message(hWindowControl1.HalconWindow, "OK", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "green", "false");
                            }
                        }
                        else if ((int)((new HTuple(hv_Number_D2.TupleGreater(1))).TupleOr(new HTuple(hv_Number_D3.TupleGreater(0)))) != 0)
                        {//中间焊接时也必须把上面焊接好，同时左右允许缺一边。
                            disp_message(hWindowControl1.HalconWindow, "NG", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "red", "false");
                            hv_out_ERROR = hv_out_ERROR + 1;
                        }
                        else
                        {
                            disp_message(hWindowControl1.HalconWindow, "OK", "window", hv_ROW - 文字显示位置, hv_COLUMN - 文字显示位置, "green", "false");
                        }

                    }

                    ho_Rectangle1.Dispose();
                    ho_Rectangle2.Dispose();
                    ho_Rectangle3.Dispose();

                    ho_RegionDifference1.Dispose();
                    ho_RegionDifference2.Dispose();

                    ho_ImageIS3.Dispose();
                    ho_ImageIS2.Dispose();
                    ho_ImageIS1.Dispose();

                    ho_Regions.Dispose();
                    ho_RegionFillUp.Dispose();
                    ho_ConnectedRegions.Dispose();

                    ho_out_sr1.Dispose();
                    ho_out_sr2.Dispose();
                    ho_out_sr3.Dispose();

                    return;
                }
                else
                {
                    disp_message(hWindowControl1.HalconWindow, "焊接端子重定位错误！", "window", 20, 100, "red", "false");
                    return;
                }
            }



        }
        #endregion

        #region 启动识别
        private void _identification()
        {
            //创建一个通过时间的变量开始。
            HOperatorSet.CountSeconds(out hv_S1);

            ho_Image.Dispose();

            bool _GEN_IMAGE_ERROR_ = true;
            hv_ErrorNum = 2;

            if (_IMAGE_DirectShow_)
            {
                try
                {
                    //hv_ErrorNum = 2;
                    //    HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);
                }
                catch (HalconException e)
                {
                    hv_ErrorNum = e.GetErrorNumber();
                    //if ((int)hv_ErrorNum < 0)
                    //    throw e;
                }
            }
            else
            {
                byte[] image_date = new byte[len];
                /*if (JHCap.CameraQueryImage(g_index, image_date, ref len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                {
                    //_GEN_IMAGE_ERROR_ = true;
                    Bitmap img = new Bitmap(R_width, R_height, PixelFormat.Format24bppRgb);
                    BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, len);
                    HOperatorSet.GenImageInterleaved(out ho_Image, (long)data.Scan0, "rgb", R_width, R_height, 0, "byte", R_width, R_height, 0, 0, -1, 0);
                    //HOperatorSet.DispObj(ho_Image, hWindowControl1.HalconWindow);
                }
                else
                {
                    _GEN_IMAGE_ERROR_ = false;
                }*/
            }

            #region IMAGE_STR
            if (((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0) && _GEN_IMAGE_ERROR_)
            {
                if ((int)(new HTuple(hv_NumContoursRing.TupleEqual(1))) != 0)
                {
                    ho_ImageZoom1.Dispose();
                    HOperatorSet.ZoomImageSize(ho_Image, out ho_ImageZoom1, R_width / 2, R_height / 2, "constant");
                    //显示1/2图像
                    HOperatorSet.DispObj(ho_ImageZoom1, hWindowControl1.HalconWindow);

                    //选中特征
                    ho_ImageEmphasize.Dispose();
                    HOperatorSet.Emphasize(ho_ImageZoom1, out ho_ImageEmphasize, Emphasize_1_X, Emphasize_1_Y, Emphasize_1_Z);
                    ho_Red.Dispose();
                    ho_Green.Dispose();
                    ho_Blue.Dispose();
                    HOperatorSet.Decompose3(ho_ImageEmphasize, out ho_Red, out ho_Green, out ho_Blue);
                    ho_Hue.Dispose();
                    ho_Saturation.Dispose();
                    ho_Intensity.Dispose();
                    HOperatorSet.TransFromRgb(ho_Red, ho_Green, ho_Blue, out ho_Hue, out ho_Saturation, out ho_Intensity, "hsi");

                    //===================增加选区，用于提高匹配轮廓的几率=====================
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageEmphasize, out ho_Regions, 0, MODEL_Threshold_L);
                    ho_RegionFillUp1.Dispose();
                    HOperatorSet.FillUp(ho_Regions, out ho_RegionFillUp1);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionFillUp1, out ho_ConnectedRegions1);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions, "area", "and", MODEL_AREA_L, 9999999);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationCircle(ho_SelectedRegions, out ho_RegionDilation, MODEL_RegionDilation);
                    ho_ImageROIRing.Dispose();
                    HOperatorSet.ReduceDomain(ho_Green, ho_RegionDilation, out ho_ImageROIRing);


                    //***********************************************************************
                    if (_显示二值化选区)
                    {
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_RegionDilation, hWindowControl1.HalconWindow);
                    }

                    if (_显示生成图像)
                    {
                        //ho_ImageROIRing
                        HOperatorSet.DispObj(ho_ImageROIRing, hWindowControl1.HalconWindow);
                    }

                    //hv_ErrorNum2 = 2;
                    try
                    {
                        //模板匹配焊接盘部分特征     ho_ImageROIRing
                        HOperatorSet.FindShapeModels(ho_ImageROIRing, hv_ModelIDRing, (new HTuple(0)).TupleRad(),
                            (new HTuple(360)).TupleRad(), _模板匹配相似度, 1, 0.5, "least_squares", 0, _模板扫描精度, out hv_RowCheck,
                            out hv_ColumnCheck, out hv_AngleCheck, out hv_Score1, out hv_Model);
                    }
                    catch (HalconException e)
                    {
                        hv_ErrorNum = e.GetErrorNumber();
                    }

                    if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                    {
                        //加入显示部分
                        if (_显示选中特征)
                        {
                            try
                            {
                                //匹配特征坐标
                                HOperatorSet.VectorAngleToRigid(0, 0, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject1);
                            }
                            catch (HalconException e)
                            {
                                hv_ErrorNum = e.GetErrorNumber();
                            }

                            if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                            {
                                //创建XLD轮廓
                                ho_ContoursAffinTrans.Dispose();
                                HOperatorSet.AffineTransContourXld(ho_UnionContours, out ho_XLDTrans, hv_MovementOfObject1);

                                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                                HOperatorSet.DispObj(ho_XLDTrans, hWindowControl1.HalconWindow);
                            }
                            else
                            {
                                disp_message(hWindowControl1.HalconWindow, "定位轮廓错误、识别物体不在范围内！", "window", 50, 100, "red", "false");

                            }
                        }

                        try
                        {
                            HOperatorSet.VectorAngleToRigid(_原始点行坐标, _原来点列坐标, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject2);
                        }
                        catch (HalconException e)
                        {
                            hv_ErrorNum = e.GetErrorNumber();
                        }
                        try
                        {
                            hv_ErrorNum = 2;
                            HOperatorSet.VectorAngleToRigid(_原始点行坐标_II, _原来点列坐标_II, 0, hv_RowCheck, hv_ColumnCheck, hv_AngleCheck, out hv_MovementOfObject3);
                        }
                        catch (HalconException e)
                        {
                            hv_ErrorNum = e.GetErrorNumber();
                        }
                        if ((int)(new HTuple(hv_ErrorNum.TupleEqual(2))) != 0)
                        {
                            //创建XLD轮廓、转换到特定矩形，定位焊接点
                            ho_ContoursAffinTrans1.Dispose();
                            ho_ContoursAffinTrans2.Dispose();
                            HOperatorSet.AffineTransContourXld(ho_Contours, out ho_ContoursAffinTrans1, hv_MovementOfObject2);
                            HOperatorSet.AffineTransContourXld(ho_Contours, out ho_ContoursAffinTrans2, hv_MovementOfObject3);
                            //轮廓转地区。
                            ho_ID_Region1.Dispose();
                            ho_ID_Region2.Dispose();
                            HOperatorSet.GenRegionContourXld(ho_ContoursAffinTrans1, out ho_ID_Region1, "filled");
                            HOperatorSet.GenRegionContourXld(ho_ContoursAffinTrans2, out ho_ID_Region2, "filled");

                            //获取坐标与角度
                            HOperatorSet.SmallestRectangle2(ho_ID_Region1, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                            HOperatorSet.CountObj(ho_ID_Region1, out hv_Number);
                            //加入显示部分
                            if (_显示选中焊接端子I)
                            {
                                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
                                HOperatorSet.DispObj(ho_ID_Region1, hWindowControl1.HalconWindow);
                                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                                HOperatorSet.DispObj(ho_ID_Region2, hWindowControl1.HalconWindow);
                            }

                            if (!_显示选中焊接端子I)
                            {
                                if ((int)(new HTuple(hv_Number.TupleEqual(4))) != 0)
                                {

                                    //从左到右排序
                                    HOperatorSet.TupleMin(hv_Column, out hv_Min_1);
                                    HOperatorSet.TupleMax(hv_Column, out hv_Max_1);
                                    hv_TEMP_X = new HTuple();
                                    hv_TEMP_X[0] = 0;
                                    hv_TEMP_X[1] = 0;
                                    hv_IDX = 0;
                                    for (hv_Index2 = 0; (int)hv_Index2 <= 3; hv_Index2 = (int)hv_Index2 + 1)
                                    {
                                        if ((int)((new HTuple(((hv_Column.TupleSelect(hv_Index2))).TupleGreater(hv_Min_1))
                                            ).TupleAnd(new HTuple(((hv_Column.TupleSelect(hv_Index2))).TupleLess(hv_Max_1)))) != 0)
                                        {
                                            hv_TEMP_X[hv_IDX] = hv_Column.TupleSelect(hv_Index2);
                                            hv_IDX = hv_IDX + 1;
                                        }
                                    }
                                    HOperatorSet.TupleMin(hv_TEMP_X, out hv_Min_2);
                                    HOperatorSet.TupleMax(hv_TEMP_X, out hv_Max_2);

                                    //重新增强图像
                                    ho_ImageEmphasize.Dispose();
                                    HOperatorSet.Emphasize(ho_ImageZoom1, out ho_ImageEmphasize, Emphasize_2_X, Emphasize_2_Y, Emphasize_2_Z);

                                    ho_Red.Dispose();
                                    ho_Green.Dispose();
                                    ho_Blue.Dispose();
                                    HOperatorSet.Decompose3(ho_ImageEmphasize, out ho_Red, out ho_Green, out ho_Blue);

                                    ho_Hue.Dispose();
                                    ho_Saturation.Dispose();
                                    ho_Intensity.Dispose();
                                    HOperatorSet.TransFromRgb(ho_Red, ho_Green, ho_Blue, out ho_Hue, out ho_Saturation, out ho_Intensity, "hsi");

                                    for (hv_Index1 = 0; (int)hv_Index1 <= 3; hv_Index1 = (int)hv_Index1 + 1)
                                    {
                                        if ((int)(new HTuple(((hv_Column.TupleSelect(hv_Index1))).TupleEqual(hv_Min_1))) != 0)
                                        {
                                            //ho_out_sr_A1.Dispose();
                                            //ho_out_sr_A2.Dispose();
                                            Image_Analyse(ho_Saturation, ho_ImageZoom1, ho_Green, ho_ID_Region2,
                                                hv_Row.TupleSelect(hv_Index1), hv_Column.TupleSelect(hv_Index1),
                                                hv_Phi.TupleSelect(hv_Index1), hv_Length1.TupleSelect(hv_Index1),
                                                hv_Length2.TupleSelect(hv_Index1), Image_Analyse_rectangle2_L1, Image_Analyse_rectangle2_L2,
                                                Image_Analyse_rectangle3_LL1, Image_Analyse_rectangle3_LL2, Image_Analyse_Threshold_1_H, ClosingCircle_1,
                                                Image_Analyse_ss_SR1_AREA_L, Image_Analyse_ss_SR1_HEIGHT_L, Image_Analyse_Threshold_2_L, ClosingCircle_2,
                                                Image_Analyse_ss_SR2_AREA_L_I, Image_Analyse_Threshold_3_L,
                                                ClosingCircle_2, Image_Analyse_ss_SR3_AREA_L, Image_Analyse_ss_SR3_ss_SR3_WIDTH_L, X_TINIT,
                                                HV_threshold_4_H, HV_ss_SR4_AREA_L, HV_ss_SR4_WIDTH_L, ER_DR_XX);
                                            //  ClosingCircle_3

                                        }
                                        else if ((int)(new HTuple(((hv_Column.TupleSelect(hv_Index1))).TupleEqual(hv_Max_1))) != 0)
                                        {
                                            //ho_out_sr_B1.Dispose();
                                            //ho_out_sr_B2.Dispose();
                                            Image_Analyse(ho_Saturation, ho_ImageZoom1, ho_Green, ho_ID_Region2,
                                                hv_Row.TupleSelect(hv_Index1), hv_Column.TupleSelect(hv_Index1),
                                                hv_Phi.TupleSelect(hv_Index1), hv_Length1.TupleSelect(hv_Index1),
                                                hv_Length2.TupleSelect(hv_Index1), Image_Analyse_rectangle2_L1, Image_Analyse_rectangle2_L2,
                                                Image_Analyse_rectangle3_LL1, Image_Analyse_rectangle3_LL2, Image_Analyse_Threshold_1_H, ClosingCircle_1,
                                                Image_Analyse_ss_SR1_AREA_L, Image_Analyse_ss_SR1_HEIGHT_L, Image_Analyse_Threshold_2_L, ClosingCircle_2,
                                                Image_Analyse_ss_SR2_AREA_L_I, Image_Analyse_Threshold_3_L,
                                                ClosingCircle_2, Image_Analyse_ss_SR3_AREA_L, Image_Analyse_ss_SR3_ss_SR3_WIDTH_L, X_TINIT,
                                                HV_threshold_4_H, HV_ss_SR4_AREA_L, HV_ss_SR4_WIDTH_L, ER_DR_XX);
                                            //  ClosingCircle_3

                                        }
                                        else if ((int)(new HTuple(((hv_Column.TupleSelect(hv_Index1))).TupleEqual(hv_Min_2))) != 0)
                                        {
                                            //ho_out_sr_C1.Dispose();
                                            //ho_out_sr_C2.Dispose();
                                            Image_Analyse(ho_Saturation, ho_ImageZoom1, ho_Green, ho_ID_Region2,
                                                hv_Row.TupleSelect(hv_Index1), hv_Column.TupleSelect(hv_Index1),
                                                hv_Phi.TupleSelect(hv_Index1), hv_Length1.TupleSelect(hv_Index1),
                                                hv_Length2.TupleSelect(hv_Index1), Image_Analyse_rectangle2_L1, Image_Analyse_rectangle2_L2,
                                                Image_Analyse_rectangle3_LL1, Image_Analyse_rectangle3_LL2, Image_Analyse_Threshold_1_H, ClosingCircle_1,
                                                Image_Analyse_ss_SR1_AREA_L, Image_Analyse_ss_SR1_HEIGHT_L, Image_Analyse_Threshold_2_L, ClosingCircle_2,
                                                Image_Analyse_ss_SR2_AREA_L_II, Image_Analyse_Threshold_3_L,
                                                ClosingCircle_2, Image_Analyse_ss_SR3_AREA_L, Image_Analyse_ss_SR3_ss_SR3_WIDTH_L, X_TINIT,
                                                HV_threshold_4_H, HV_ss_SR4_AREA_L, HV_ss_SR4_WIDTH_L, ER_DR_XX);
                                            //  ClosingCircle_3

                                        }
                                        else
                                        {
                                            //ho_out_sr_D1.Dispose();
                                            //ho_out_sr_D2.Dispose();
                                            Image_Analyse(ho_Saturation, ho_ImageZoom1, ho_Green, ho_ID_Region2,/*out ho_out_sr_D1, out ho_out_sr_D2,*/
                                                hv_Row.TupleSelect(hv_Index1), hv_Column.TupleSelect(hv_Index1),
                                                hv_Phi.TupleSelect(hv_Index1), hv_Length1.TupleSelect(hv_Index1),
                                                hv_Length2.TupleSelect(hv_Index1), Image_Analyse_rectangle2_L1, Image_Analyse_rectangle2_L2,
                                                Image_Analyse_rectangle3_LL1, Image_Analyse_rectangle3_LL2, Image_Analyse_Threshold_1_H, ClosingCircle_1,
                                                Image_Analyse_ss_SR1_AREA_L, Image_Analyse_ss_SR1_HEIGHT_L, Image_Analyse_Threshold_2_L, ClosingCircle_2,
                                                Image_Analyse_ss_SR2_AREA_L_II, Image_Analyse_Threshold_3_L,
                                                ClosingCircle_2, Image_Analyse_ss_SR3_AREA_L, Image_Analyse_ss_SR3_ss_SR3_WIDTH_L, X_TINIT,
                                                HV_threshold_4_H, HV_ss_SR4_AREA_L, HV_ss_SR4_WIDTH_L, ER_DR_XX);
                                            //  ClosingCircle_3

                                        }

                                    }

                                    //显示当前设定等级
                                    if (_str_out == 2)
                                    {
                                        disp_message(hWindowControl1.HalconWindow, "高", "window", 50, 50, "green", "false");
                                    }
                                    else if (_str_out == 1)
                                    {
                                        disp_message(hWindowControl1.HalconWindow, "中", "window", 50, 50, "green", "false");
                                    }
                                    else
                                    {
                                        disp_message(hWindowControl1.HalconWindow, "低", "window", 50, 50, "green", "false");
                                    }

                                    if (hv_out_ERROR > 0)
                                    {
                                        plc_send_signal(_SEND_NG_, _M_Signal_type, 2);
                                        ADT_text_flag = "NG";
                                    }
                                    else
                                    {
                                        plc_send_signal(_SEND_OK_, _M_Signal_type, 2);
                                        ADT_text_flag = "OK";
                                    }
                                    hv_out_ERROR = 0;
                                }
                                else
                                {
                                    disp_message(hWindowControl1.HalconWindow, "焊接点标定错误、识别物体不在范围内！", "window", 20, 100, "red", "false");
                                    plc_send_signal(_SEND_NG_, _M_Signal_type, 2);
                                    ADT_text_flag = "ERROR";
                                    ADT_text_ERROR = "焊接点标定错误";
                                    hv_out_ERROR = 0;
                                }
                            }
                            else
                            {
                                disp_message(hWindowControl1.HalconWindow, "调试......", "window", 20, 100, "green", "false");
                            }

                        }
                        else
                        {
                            disp_message(hWindowControl1.HalconWindow, "轮廓标定错误、识别物体不在范围内！", "window", 20, 100, "red", "false");
                            plc_send_signal(_SEND_NG_, _M_Signal_type, 2);
                            ADT_text_flag = "ERROR";
                            ADT_text_ERROR = "轮廓标定错误";
                            hv_out_ERROR = 0;
                        }

                    }
                    else
                    {
                        disp_message(hWindowControl1.HalconWindow, "特征匹配错误、识别物体不在范围内！", "window", 20, 100, "red", "false");
                        plc_send_signal(_SEND_NG_, _M_Signal_type, 2);
                        ADT_text_flag = "ERROR";
                        ADT_text_ERROR = "特征匹配错误";
                        hv_out_ERROR = 0;
                    }

                }
                else
                {
                    disp_message(hWindowControl1.HalconWindow, "模板初始化错误或不存在！", "window", 20, 100, "red", "false");
                    //plc_send_signal(_SEND_NG_, _M_Signal_type, 2);
                }
                //创建一个通过时间的变量结束。
                HOperatorSet.CountSeconds(out hv_S2);
                hv_Time = 1000.0 * (hv_S2 - hv_S1);
                disp_message(hWindowControl1.HalconWindow, hv_Time + "ms", "window", 20, 1000, "green", "false");

                if (ADT_text_flag == "OK" || ADT_text_flag == "NG" || ADT_text_flag == "ERROR")
                {

                    SetLabel();
                }
                else
                {
                    SetLabel();
                }
            }
            else
            {
                if (_IMAGE_DirectShow_)
                {
                    OK_ERROR_Camera = false;
                    MessageBox.Show("读取图像错误");
                }
                else
                {
                    if (_IMAGE_ERROR_USB_RST_ > 2)
                    {
                        OK_ERROR_Camera = false;
                        MessageBox.Show("读取图像错误" + _IMAGE_ERROR_USB_RST_.ToString());
                    }
                    else
                    {
                        /* if (JHCap.CameraReconnect(g_index) == 0)
                         {
                             if (JHCap.CameraInit(g_index) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                             {
                                 JHCap.CameraSetSnapMode(g_index, JHCap.CAMERA_SNAP_TRIGGER);
                                 //获取分辨率
                                 JHCap.CameraGetResolution(g_index, 0, ref R_width, ref R_height);
                                 JHCap.CameraGetImageBufferSize(g_index, ref len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                                 OK_ERROR_Camera = true;
                                 _IMAGE_ERROR_USB_RST_++;
                                 MessageBox.Show("连接错误、已重启相机！");
                             }
                             else
                             {
                                 OK_ERROR_Camera = false;
                                 MessageBox.Show("重启相机失败");
                             }
                         }
                         else
                         {
                             OK_ERROR_Camera = false;
                             MessageBox.Show("重启相机失败");
                         */
                    }
                }
            }
            #endregion

        }
        #endregion

        #region 启动停止界面图标显示和部分控制
        private void _boot1_()
        {
            STAR_手动检测 = false;

            //timer1.Enabled = false;
            timer2.Enabled = false;

            _启用匹配调试.Enabled = true;

            Colours_Thread = false;//启动线程或允许检测标志
            //通讯打开标志位 = false;

            连续测试.Enabled = true;

            if (OK_ERROR_Camera)
            {
                启动.Enabled = true;
                启动.BackgroundImage = opt_Weld_identification.Properties.Resources.启动1;
                相机参数.Enabled = true;
                相机参数.BackgroundImage = opt_Weld_identification.Properties.Resources.相机1;
            }

            //退出.Enabled = true;
            //退出.BackgroundImage = opt_Weld_identification.Properties.Resources.退出1;

            PLC通讯信号灯1.BackColor = Color.White;

            IMAGE_directshow.Enabled = true;
        }
        private void _boot2_()
        {
            _启用匹配调试.Enabled = false;

            启动.Enabled = false;
            启动.BackgroundImage = opt_Weld_identification.Properties.Resources.启动2;
            相机参数.Enabled = false;
            相机参数.BackgroundImage = opt_Weld_identification.Properties.Resources.相机2;
            退出.Enabled = false;
            退出.BackgroundImage = opt_Weld_identification.Properties.Resources.退出2;
            连续测试.Enabled = false;
            IMAGE_directshow.Enabled = false;
        }

        #endregion

        #region 启动主线程
        private void 启动主线程()
        {
            /* if (_启用匹配调试_)
             {
                 Thread myThread_1 = new Thread(new ThreadStart(RefreshProcess));
                 myThread_1.Start();
             }
             else
             {
                 //创建线程，把函数放入线程中
                 Thread myThread_1 = new Thread(new ThreadStart(RefreshProcess));
                 //设置线程为最高级别
                 //myThread_1.Priority = ThreadPriority.Highest;
                 //后台执行线程
                 //myThread_1.IsBackground = true; 
                 //启动线程
                 myThread_1.Start();
             }*/


        }
        #endregion
        #region 模板调试线程
        //public delegate bool MethodCaller();//定义委托代理
        private void 模板调试线程()
        {
            //通过这种方式生成新线程是运行在后台的（background）,优先级为normal
            //MethodCaller mc = new MethodCaller(_STAR_MODELS);
            //string name = "my name";//输入参数 
            //IAsyncResult result = mc.BeginInvoke(null,null);
            //bool myname = mc.EndInvoke(result);//用于接收返回值
            if (_STAR_MODELS())
            {
                Colours_Thread = true;//启动线程或允许检测标志
            }
            //else
            //{
            //    Colours_Thread = false;//启动线程或允许检测标志
            //    timer2.Enabled = false;
            //}
            //return myname;
        }
        #endregion
        #region PLC信号
        //****************PLC发送信号****************
        private static string _SEND_OK_ = "";
        private static string _SEND_NG_ = "";
        private static int _M_Signal_type = 1;//信号类型
        private static string _M_Photo_signal = "M72";//拍照信号
        private static int _M_Delay_cycle = 3;//延时周期
        #endregion

        private bool 通讯打开标志位 = false;
        private bool 通讯信号灯 = false;
        private int PLC_PASS_M = 0;//通讯返回值
        private bool PLC_PASS_FLAG = true;//启动主线程时不访问PLC
        private int PLC延时启动 = 4;//图像分析完成、发送完成信号、延时一小会后访问PLC
        private int 错误记录 = 0;
        private bool 等待PLC信号提示标记 = true;

        #region plc_com_init
        private void plc_com_init()
        {
            _M_Photo_signal = 拍照信号.Text;

            _SEND_OK_ = 合格信号.Text;
            _SEND_NG_ = 不合格信号.Text;
            //
            _M_Delay_cycle = (int)延时次数.Value;
            //
            _M_Signal_type = (int)信号类型.Value;
        }
        #endregion
        #region plc_send_signal
        private void plc_send_signal(string _M_, int _t, int J)
        {
            /*if (timer1.Enabled)
            {
                if (通讯打开标志位)
                {
                    int i = 0, j = J;
                    for (i = 0; i < j; i++)
                    {
                        if (ACTMFX.SetDevice(_M_, _t) == 0)
                        {
                            break;
                        }
                        else
                        {
                            if (i >= j)
                            {
                                通讯打开标志位 = false;
                                MessageBox.Show("PLC通讯无应答、已关闭检测");
                            }
                        }

                    }
                }
            }*/
        }
        #endregion
        //****************PLC通讯*****************
        #region 端口初始化
        //***************PLC初始化****************
        private void PLC_Init()
        {
            /*if (端口号.Text == "COM1") ACTMFX.ActPortNumber = 0X01;
            if (端口号.Text == "COM2") ACTMFX.ActPortNumber = 0X02;
            if (端口号.Text == "COM3") ACTMFX.ActPortNumber = 0X03;
            if (端口号.Text == "COM4") ACTMFX.ActPortNumber = 0X04;
            if (端口号.Text == "COM5") ACTMFX.ActPortNumber = 0X05;
            if (端口号.Text == "COM6") ACTMFX.ActPortNumber = 0X06;
            if (端口号.Text == "COM7") ACTMFX.ActPortNumber = 0X07;
            if (端口号.Text == "COM8") ACTMFX.ActPortNumber = 0X08;
            if (端口号.Text == "COM9") ACTMFX.ActPortNumber = 0X09;
            if (端口号.Text == "COM10") ACTMFX.ActPortNumber = 0X0A;
            if (端口号.Text == "COM11") ACTMFX.ActPortNumber = 0X0B;
            if (端口号.Text == "COM12") ACTMFX.ActPortNumber = 0X0C;
            if (端口号.Text == "COM13") ACTMFX.ActPortNumber = 0X0D;
            if (端口号.Text == "COM14") ACTMFX.ActPortNumber = 0X0E;
            if (端口号.Text == "COM15") ACTMFX.ActPortNumber = 0X0F;
            if (波特率.Text == "9600") ACTMFX.ActBaudRate = 9600;
            if (波特率.Text == "19200") ACTMFX.ActBaudRate = 19200;
            ACTMFX.ActTimeOut = 200;
            ACTMFX.ActControl = 8;*/
        }
        #endregion
        #region 打开PLC通讯口
        private void PLC_Open()
        {
            PLC_Init();
            /*if (选择PLC类型.Text == "FX0")
            {
                ACTMFX.ActCpuType = 0X201;//初始化FX0PLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX0"; PLC连接状态2.Text = "PLC:FX0";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }

            }
            else if (选择PLC类型.Text == "FX0N")
            {
                ACTMFX.ActCpuType = 0X202;//初始化FX0NPLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX0N"; PLC连接状态2.Text = "PLC:FX0N";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX1")
            {
                ACTMFX.ActCpuType = 0X203;//初始化FX1PLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX1"; PLC连接状态2.Text = "PLC:FX1";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX2")
            {
                ACTMFX.ActCpuType = 0X204;//初始化FX2PLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX2"; PLC连接状态2.Text = "PLC:FX2";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX2N")
            {
                ACTMFX.ActCpuType = 0X205;//初始化FX2NPLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX2N"; PLC连接状态2.Text = "PLC:FX2N";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX1S")
            {
                ACTMFX.ActCpuType = 0X206;//初始化FX2NPLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX1S"; PLC连接状态2.Text = "PLC:FX1S";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX1N")
            {
                ACTMFX.ActCpuType = 0X207;//初始化FX2NPLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX1N"; PLC连接状态2.Text = "PLC:FX1N";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else if (选择PLC类型.Text == "FX3U")
            {
                ACTMFX.ActCpuType = 0X208;//初始化FX2NPLC
                if (ACTMFX.Open() == 0)
                {
                    PLC连接状态1.Text = "PLC:FX3U"; PLC连接状态2.Text = "PLC:FX3U";
                    //开启测试.Enabled = true;
                    通讯打开标志位 = true;
                }
                else
                {
                    通讯打开标志位 = false;
                    MessageBox.Show("PLC连接失败");
                }
            }
            else
            {
                通讯打开标志位 = false;
                MessageBox.Show("请选择PLC类型");
            }*/

        }
        #endregion
        #region 关闭PLC通讯口
        private void PLC_Close()
        {
            通讯打开标志位 = false;
            //timer1.Enabled = false;
            通讯打开标志位 = false;
            PLC通讯信号灯1.BackColor = Color.White;
            if (PLC连接状态1.Text != "未连接")
            {
                /*if (ACTMFX.Close() != 0)
                {
                    //开启测试.Enabled = false; 关闭测试.Enabled = false;
                    PLC连接状态1.Text = "PLC:未连接"; PLC连接状态2.Text = "PLC:未连接";
                }
                else
                {
                    PLC连接状态1.Text = "PLC:未连接"; PLC连接状态2.Text = "PLC:未连接";
                    //开启测试.Enabled = false; 关闭测试.Enabled = false;
                }*/
            }

        }
        #endregion
        #region timer1
        private void timer1_Tick(object sender, EventArgs e)
        {
            ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
            //创建目录(目录);
        }
        #endregion
        #region timer2
        //bool _绘制焊接端子完成标志位 = false;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (OK_ERROR_Camera != true)
            {
                _boot1_();
                Cloce_Camera();
            }
            else
            {
                #region
                //if (_启用匹配调试.Checked)
                //{
                //    if (Colours_Thread)
                //    {
                //        Colours_Thread = false;
                //        if (!模板调试线程())
                //        {
                //            timer2.Enabled = false;
                //        }
                //    }

                //}
                //else
                //{
                #endregion
                if (Colours_Thread)//启动线程或允许检测标志
                {
                    Colours_Thread = false;//启动线程或允许检测标志
                    启动主线程();
                }

                //}
            }
        }
        #endregion
        #region 启动
        //手动拍照标记
        bool STAR_手动检测 = false;
        private void 启动_Click(object sender, EventArgs e)
        {
            using (CheckTime t = new CheckTime("开始软件！"))
            {
                //设置按钮上的图像
                启动.Image = global::opt_Weld_identification.Properties.Resources.启动2;
                停止.Image = global::opt_Weld_identification.Properties.Resources.停止1;
                退出.Image = global::opt_Weld_identification.Properties.Resources.退出2;
                //按钮功能
                启动.Enabled = false;
                停止.Enabled = true;
                退出.Enabled = false;

                ModuleInPosTrigger = false;//清楚拍照标记上升沿M72
                //11111111111111
                Byte[] recvDate_X16 = new Byte[50];
                Pause_mutex.WaitOne();
                res = FXPlc2IO.GetDO2("D125", ref recvDate_X16);
                Pause_mutex.ReleaseMutex();
                if (res == FXPlcComm.FXPlcStatus.FX_OK)
                {
                    bool RST_ER = FXPlc2IO._analysisddata_(recvDate_X16, ref FXPlc2IO.DI, 1);
                    if (RST_ER)
                    {
                        String strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 0, 1);
                        read_plc_[8] = int.Parse(strDI);//下压接线盒气缸下压到位
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 1, 1);
                        read_plc_[9] = int.Parse(strDI);//设备急停

                    }

                }
                if (read_plc_[9] < 1)
                {
                    退出.Enabled = false;
                    退出.BackgroundImage = opt_Weld_identification.Properties.Resources.退出2;

                    //    star_pause_handle = false;//启动忽视检测设备急停 = false
                    button_TABLE_PARAM_SET_Click(sender, e);//将平台参数应用
                    TableCtl.WeldMotorUp();
                    TableCtl.MoveAbsXYR(TableCtl.XDefPos, TableCtl.YDefPos, TableCtl.RDefPos);
                    bNewMainStop = false;//启动焊接
                    _star_read_plc = false;//开始读取PLC
                    #region 按钮初始化部分

                    //bool temp_alert = true;//急停时不初始化
                    //while (TableCtl.InPosXYR() == 0)
                    //{
                    //    //如果设备急停就关闭软件启动标记，同时跳转到线程起始位置等待
                    //    if (FXPlc2IO.GetDI(UnitDefine.DI_Emergency_stop) == 1)
                    //    { star_Pause_software(); //goto CC; 
                    //    temp_alert = false;//急停时不初始化
                    //    }
                    //    ////
                    //    //if (bNewMainStop)
                    //    //    break;
                    //    Thread.Sleep(50);
                    //    continue;
                    //}
                    //if (temp_alert)//急停时不处理下面的代码
                    //{
                    bool star_OK = false;
                    _INIT_GenEmptyObj();
                    if (通讯打开标志位)
                    {
                        _boot2_();

                        _初始化匹配目录();

                        if (_INIT_MODELS_IMAGE())
                        {
                            if (_STAR_MODELS())
                            {
                                star_OK = true;
                            }
                        }

                        if (star_OK && !_启用匹配调试_)
                        {
                            if (IF_init())
                            {
                                Colours_Thread = true;//启动线程或允许检测标志
                                timer1.Enabled = true;//启动定时器1
                            }
                            else
                            {
                                //Colours_Thread = false;
                                _boot1_();
                            }
                        }
                        else
                        {
                            _boot1_();
                        }

                    }
                    else if (_手动检测.Checked)//////////////////////////
                    {
                        _boot2_();

                        _初始化匹配目录();

                        if (_INIT_MODELS_IMAGE())
                        {
                            if (_STAR_MODELS())
                            {
                                star_OK = true;
                            }
                        }

                        if (star_OK)
                        {
                            if (IF_init())
                            {
                                //选择图像显示控件
                                //hWindowControl1.Select(true);// = 42;


                                Colours_Thread = true;//启动线程或允许检测标志
                                STAR_手动检测 = true;
                                textBox_hw.AppendText("请使用键盘<空格键>控制检测！\n");
                                disp_message(hWindowControl1.HalconWindow, "请使用键盘<空格键>控制检测！", "window", 20, 100, "green", "false");
                            }
                            else
                            {
                                //Colours_Thread = false;
                                _boot1_();
                            }
                        }
                        else
                        {
                            _boot1_();
                        }

                    }

                    // }
                    #endregion
                }
            }
        }
        #endregion
        #region 停止
        private void 停止_Click(object sender, EventArgs e)
        {
            //设置按钮上的图像
            启动.Image = global::opt_Weld_identification.Properties.Resources.启动1;
            停止.Image = global::opt_Weld_identification.Properties.Resources.停止2;
            退出.Image = global::opt_Weld_identification.Properties.Resources.退出1;
            //按钮功能
            启动.Enabled = true;
            停止.Enabled = false;
            退出.Enabled = true;
            DialogResult dr = MessageBox.Show("确认停止检测？", "检测中！", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            if (dr == DialogResult.OK)
            {
                using (CheckTime t = new CheckTime("停止软件！"))
                {
                    //    star_pause_handle = true;//启动忽视检测设备急停 = false
                    _star_read_plc = true;//开始读取PLC
                    bNewMainStop = true;//
                    退出.Enabled = true;
                    启动.Image = global::opt_Weld_identification.Properties.Resources.启动1;
                    停止.Image = global::opt_Weld_identification.Properties.Resources.停止2;
                    //退出.BackgroundImage = opt_Weld_identification.Properties.Resources.退出1;
                    退出.Image = global::opt_Weld_identification.Properties.Resources.退出1;

                    停止.Enabled = false;
                    _boot1_();
                    启动.Enabled = true;
                    //
                }
            }
            else
            {
                //设置按钮上的图像
                启动.Image = global::opt_Weld_identification.Properties.Resources.启动2;
                停止.Image = global::opt_Weld_identification.Properties.Resources.停止1;
                退出.Image = global::opt_Weld_identification.Properties.Resources.退出2;
                //按钮功能
                启动.Enabled = false;
                停止.Enabled = true;
                退出.Enabled = false;
            }



        }
        #endregion
        #region 移出组件
        private void 移出组件_Click(object sender, EventArgs e)
        {
            // ACTMFX.SetDevice(_SEND_OK_, _M_Signal_type);
            res = FXPlc2IO.setM("M38", true);
        }
        #endregion
        #region 连续测试
        private void 连续测试_Click(object sender, EventArgs e)
        {
            bool star_OK = false;
            textBox_hw.AppendText("调试......\n");
            _INIT_GenEmptyObj();
            _boot2_();

            _初始化匹配目录();

            //if (_启用匹配调试.Checked)
            //{
            //if (_INIT_MODELS_IMAGE())
            //{
            //    star_OK = true;
            //}
            //}
            //else
            //{
            if (_INIT_MODELS_IMAGE())
            {
                if (_STAR_MODELS())
                {
                    star_OK = true;
                }
            }
            //}

            if (star_OK)
            {
                if (IF_init())
                {

                    Colours_Thread = true;//启动线程或允许检测标志
                    timer2.Enabled = true;//启动定时器1
                }
                else
                {
                    //Colours_Thread = false;
                    _boot1_();
                }
            }
            else
            {
                _boot1_();
            }

        }
        #endregion
        #region 停止相机
        private void Cloce_Camera()
        {
            HOperatorSet.CloseAllFramegrabbers();

            //暂停
            //JHCap.CameraStop(g_index);
            //释放相机
            //JHCap.CameraFree(g_index);

            OK_ERROR_Camera = false;
            toolStripStatusLabel6.Text = "相机:未连接";

        }
        #endregion

        #region 相机参数（可能没有用）
        private void 相机参数_Click(object sender, EventArgs e)
        {
            Cloce_Camera();

            if (Weld_Form1 == null)
            {
                Weld_Form1 = new Weld_Form();
                Weld_Form1.Show();
            }
            else
            {
                Weld_Form1.Activate();
            }
        }
        #endregion

        //计数
        Int32 _OK_ = 0;
        Int32 _NG_ = 0;
        Int32 _ERROR_ = 0;
        #region 窗口文本更新
        private void SetLabel()
        {
            CrossDelegate dl = new CrossDelegate(文本更新);
            //double i = hv_Time;
            string _HW_TIME = "0";
            string _TEXT_FLAGE = ADT_text_flag;
            string _TEXT_ERROR = ADT_text_ERROR;
            string _FindSN_Module = "";
            string _FindSN_Box = "";
            BeginInvoke(dl, _HW_TIME, _TEXT_FLAGE, _TEXT_ERROR, _FindSN_Module, _FindSN_Box); // 异步调用委托,调用后立即返回并立即执行下面的语句
            //this.Invoke(dl, _hv_time_, text_flag); // 等待工作线程完成后, 才接着执行下面的语句.
            //MessageBox.Show("委托调用返回);
        }
        //委托
        public delegate void CrossDelegate(string _hv_time_, string text_flag, string text_ERROR, string SN_Module, string SN_Box);
        private void 文本更新(string _HW_TIME, string _TEXT_FLAGE, string _TEXT_ERROR, string SN_Module, string SN_Box)
        {
            textBox_hw.AppendText(_HW_TIME + "ms\n");
            textBox_SN.Text = SN_Module;

            if (_TEXT_FLAGE != "")
            {
                textBox_hw.AppendText(_TEXT_FLAGE + "\n");
                if (_TEXT_FLAGE == "OK")
                {
                    _OK_++;
                    label_OK.Text = "OK:" + _OK_.ToString();
                }
                else if (_TEXT_FLAGE == "ERROR")
                {
                    _ERROR_++;
                    textBox_hw.ForeColor = Color.Red;//设置字体颜色
                    if (_TEXT_ERROR != null)
                    {
                        textBox_hw.AppendText(_ERROR_ + ":" + _TEXT_ERROR + "\n");
                    }
                    else
                    {
                        textBox_hw.AppendText(_ERROR_ + ":" + "匹配错误" + "\n");
                    }
                    textBox_hw.ForeColor = Color.Black;//设置字体颜色
                }
                else
                {
                    _NG_++;
                    label_NG.Text = "NG:" + _NG_.ToString();
                }

            }

            //MEAN_FLAG = false;//清楚标记
            //_TEXT_FLAGE = null;
            ADT_text_flag = "";
            ADT_text_ERROR = null;
            Colours_Thread = true;//启动线程或允许检测标志
        }
        private void ClearLabel()
        {
            CrossDelegate dl = new CrossDelegate(文本更新);
            //double i = hv_Time;
            string _HW_TIME = "0";
            string _TEXT_FLAGE = "";
            string _TEXT_ERROR = ADT_text_ERROR;
            string _FindSN_Module = "";
            string _FindSN_Box = "";
            BeginInvoke(dl, _HW_TIME, _TEXT_FLAGE, _TEXT_ERROR, _FindSN_Module, _FindSN_Box); ; // 异步调用委托,调用后立即返回并立即执行下面的语句
            //this.Invoke(dl, _hv_time_, text_flag); // 等待工作线程完成后, 才接着执行下面的语句.
            //MessageBox.Show("委托调用返回);
        }
        //委托

        #endregion
        #region 链接PLC
        private void action()
        {

            // Local iconic variables 

            HObject ho_Image, ho_ImageRotate, ho_Red, ho_Green;
            HObject ho_Blue, ho_ImageScaleMax, ho_SymbolRegions;


            // Local control variables 

            HTuple hv_WindowID, hv_BarCodeHandle, hv_DecodedDataStrings;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);

            try
            {

                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, "H:/1217/sn/SN2016年3月17日1505.JPG");
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_Image, out ho_ImageRotate, 180, "constant");
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                HOperatorSet.Decompose3(ho_ImageRotate, out ho_Red, out ho_Green, out ho_Blue
                    );

                ho_ImageScaleMax.Dispose();
                HOperatorSet.ScaleImageMax(ho_Green, out ho_ImageScaleMax);


                HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
                HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "check_char", "absent");
                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_ImageScaleMax, out ho_SymbolRegions, hv_BarCodeHandle,
                    "auto", out hv_DecodedDataStrings);

                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_ImageRotate.Dispose();
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_SymbolRegions.Dispose();

                throw HDevExpDefaultException;
            }
            ho_Image.Dispose();
            ho_ImageRotate.Dispose();
            ho_Red.Dispose();
            ho_Green.Dispose();
            ho_Blue.Dispose();
            ho_ImageScaleMax.Dispose();
            ho_SymbolRegions.Dispose();

        }
        private void 连接PLC_Click(object sender, EventArgs e)
        {
            //PLC_Open();
            action();
        }
        #endregion
        #region 保存设置
        private void 保存设置_Click(object sender, EventArgs e)
        {
            保存本地配置("PortNumber_", 端口号.Text);
            保存本地配置("BaudRate_", 波特率.Text);
            保存本地配置("PLC_CpuType_", 选择PLC类型.Text);
            MessageBox.Show("保存设置文件成功");
        }
        #endregion
        #region 断开PLC连接
        private void 断开PLC连接_Click(object sender, EventArgs e)
        {
            PLC_Close();
        }
        #endregion
        #region 复位默认设置
        private void 复位默认设置_Click(object sender, EventArgs e)
        {
            端口号.Text = "COM1";
            波特率.Text = "9600";
            选择PLC类型.Text = "FX1N";
        }
        #endregion

        #region 软件菜单设置等等 - 1

        private void _初始化匹配目录()
        {
            _模板目录 = maskedTextBox3.Text;

        }


        private void _读取匹配图像__Click(object sender, EventArgs e)
        {
            // 设置根在桌面
            //folderBrowserDialog1.RootFolder = SpecialFolder.Desktop;
            // 设置当前选择的路径
            folderBrowserDialog1.SelectedPath = "D:";
            // 允许在对话框中包括一个新建目录的按钮
            folderBrowserDialog1.ShowNewFolderButton = true;
            // 设置对话框的说明信息
            folderBrowserDialog1.Description = "请选择模版目录";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                maskedTextBox3.Text = folderBrowserDialog1.SelectedPath;//得到选择的路径
                _初始化匹配目录(); ;
            }



        }
        bool _启用匹配调试_ = false;
        private void _启用匹配调试_CheckedChanged(object sender, EventArgs e)
        {
            _启用匹配调试_ = _启用匹配调试.Checked;
        }

        //手动测试
        private void hWindowControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (STAR_手动检测)
            {
                if (Colours_Thread)
                {
                    if (e.KeyCode == Keys.Space /*&& e.Control*/)
                    {
                        e.Handled = true;
                        //将Handled设置为true，指示已经处理过KeyPress事件
                        Colours_Thread = false;//启动线程或允许检测标志
                        启动主线程();
                    }
                }

            }
        }
        #endregion
        private CImageProcess ImgProcess = new CImageProcess();//引用算法
        //private CAdvIOCtrl IOCtl = new CAdvIOCtrl();
        private C_3AxisTable TableCtl = new C_3AxisTable();//运动控制卡
        private bool bNewMainTerminate = false;//线程控制
        private bool bNewMainStop = true;//线程等待（控制软件和设备启动）
        //  private bool bLastManualRedoStatus = false;//
        //  private bool ManualRedoTrigger = false;//
        private bool bLastModuleInPosStatus = false;//拍照信号置位标记
        private bool ModuleInPosTrigger = false;//上升沿标记
        //
        void ShowInfo(String Info)
        {
            HOperatorSet.SetColor(hWindowControl1.HalconWindow, "red");
            HOperatorSet.SetTposition(hWindowControl1.HalconWindow, 100, 100);
            HOperatorSet.WriteString(hWindowControl1.HalconWindow, Info);

            //新增 细节窗口显示缺锡提示20180428
            HOperatorSet.SetColor(ZM1_hWindow.HalconWindow, "red");
            HOperatorSet.SetTposition(ZM1_hWindow.HalconWindow, 100, 100);
            HOperatorSet.WriteString(ZM1_hWindow.HalconWindow, Info);



        }
        //
        #region 调用：暂停软件（未使用）
        //private void star_Pause_software()
        //{
        //    Pause_software_CrossDelegate _Pause_software = new Pause_software_CrossDelegate(Pause_software);
        //    BeginInvoke(_Pause_software); // 异步调用委托,调用后立即返回并立即执行下面的语句
        //}
        ////委托
        //public delegate void Pause_software_CrossDelegate();
        //private void Pause_software()
        //{
        ////    star_pause_handle = true;//启动忽视检测设备急停 = false
        //    bNewMainStop = true;//
        //    退出.Enabled = true;
        //    退出.BackgroundImage = opt_Weld_identification.Properties.Resources.退出1;
        //    _boot1_();
        //    启动.Enabled = true;
        //    FXPlc2IO.SetDO(UnitDefine.DO_str_alert, true);//报警
        //    MessageBox.Show("设备急停！");
        //    //
        //}
        #endregion
        #region 三菱PLC通讯线程部分
        ////每秒巡查一次PLC急停信号，如果急停被按下，则直接关闭软件，这是临时措施
        //private bool star_pause_handle = true;//启动忽视检测设备急停 = false
        bool _star_PLC = false;//启动PLC通讯线程
        bool _star_read_plc = true;//开始读取PLC
        //GET、拍照信号                 D123               0x01           M72	0
        //GET、X轴电钢报[警              D123               0x02           M73	1
        //GET、Y轴电钢报警              D123               0x04           M74	2
        //GET、门                       D123               0x08           M75	3
        //GET、PLC出错报警              D123               0x10           M76	4
        //GET、缺锡                     D123               0x20           M77	5
        //GET、未使用                   D123（未使用）     0x40           M78	6
        //GET、下压接线盒气缸原始位置   D123               0x80           M79	7
        //GET、下压接线盒气缸下压到位   D125               0x01           M80	8
        //GET、设备急停                 D125               0x02           M81	9
        //GET、给上位机电机转动信号     D125（未使用）     0x04           M82	10
        //GET、                         D125（未使用）     0x08           M83	11
        int[] read_plc_ = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //SET、高频焊接信号             M32（未使用）		0
        //SET、给下压接线盒气缸信号     M33			        1
        //SET、给接线盒压针信号         M34			        2
        //SET、给焊接完成信号           M35			        3
        //SET、吹气                     M36（暂时未使用）	4
        //SET、视觉NG报警信号           M37			        5
        //SET、出料OK                   M38			        6
        //SET、出料NG                   M39			        7
        //int[] setM_plc_ = { 0, 0, 0, 0, 0, 0, 0, 0 };
        //
        FXPlcComm.FXPlcStatus res;//读取PLC信号返回的状态
        private Mutex Pause_mutex = new Mutex();//与主线程发送信号忽视
        private void Star_IO_PLC()
        {
            while (!_star_PLC)//启动线程
            {
                if (_star_read_plc)//开始读取PLC
                {
                    Thread.Sleep(20);
                    continue;
                }
                //GET、拍照信号                 D123               0x01           M72	0
                //GET、X轴电钢报警              D123               0x02           M73	1
                //GET、Y轴电钢报警              D123               0x04           M74	2
                //GET、门                       D123               0x08           M75	3
                //GET、PLC出错报警              D123               0x10           M76	4
                //GET、缺锡                     D123               0x20           M77	5
                //GET、未使用                   D123（未使用）     0x40           M78	6
                //GET、下压接线盒气缸原始位置   D123               0x80           M79	7
                Byte[] recvDate = new Byte[50];
                Pause_mutex.WaitOne();
                res = FXPlc2IO.GetDO2("D123", ref recvDate);
                Pause_mutex.ReleaseMutex();
                if (res == FXPlcComm.FXPlcStatus.FX_OK)
                {
                    bool RST_ER = FXPlc2IO._analysisddata_(recvDate, ref FXPlc2IO.DI, 1);
                    if (RST_ER)
                    {
                        String strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 0, 1);
                        read_plc_[0] = int.Parse(strDI);//拍照信号

                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 1, 1);
                        // read_plc_[1] = int.Parse(strDI);//X轴电钢报警
                        read_plc_[4] = int.Parse(strDI);//PLC出错报警 20180404 金帅修改

                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 2, 1);
                        // read_plc_[2] = int.Parse(strDI);//Y轴电钢报警
                        read_plc_[4] = int.Parse(strDI);//PLC出错报警  20180404 金帅修改

                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 3, 1);
                        read_plc_[3] = int.Parse(strDI);//门
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 4, 1);
                        read_plc_[4] = int.Parse(strDI);//PLC出错报警
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 5, 1);
                        read_plc_[5] = int.Parse(strDI);//缺锡 
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 6, 1);
                        //read_plc_[6] = int.Parse(strDI);//未使用
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 7, 1);
                        read_plc_[7] = int.Parse(strDI);//下压接线盒气缸原始位置
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 8, 1);

                    }

                }

                //GET、下压接线盒气缸下压到位   D125               0x01           M80	8
                //GET、设备急停                 D125               0x02           M81	9
                //GET、给上位机电机转动信号     D125（未使用）     0x04           M82	10
                //GET、                         D125（未使用）     0x08           M83	11
                Byte[] recvDate_X16 = new Byte[50];
                Pause_mutex.WaitOne();
                res = FXPlc2IO.GetDO2("D125", ref recvDate_X16);
                Pause_mutex.ReleaseMutex();
                if (res == FXPlcComm.FXPlcStatus.FX_OK)
                {
                    bool RST_ER = FXPlc2IO._analysisddata_(recvDate_X16, ref FXPlc2IO.DI, 1);
                    if (RST_ER)
                    {
                        String strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 0, 1);
                        read_plc_[8] = int.Parse(strDI);//下压接线盒气缸下压到位
                        strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 1, 1);
                        read_plc_[9] = int.Parse(strDI);//设备急停
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 2, 1);
                        //read_plc_[10] = int.Parse(strDI);//给上位机电机转动信号
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 3, 1);
                        //read_plc_[11] = int.Parse(strDI);//（未使用）
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 4, 1);
                        //read_plc_[12] = int.Parse(strDI);//（未使用）
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 5, 1);
                        //read_plc_[13] = int.Parse(strDI);//（未使用）
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 6, 1);
                        //read_plc_[14] = int.Parse(strDI);//（未使用）
                        //strDI = System.Text.Encoding.Default.GetString(FXPlc2IO.DI, 7, 1);
                        //read_plc_[15] = int.Parse(strDI);//（未使用）

                    }

                }
                //手动触发
                if (read_plc_[0] > 0)//读取组件到位信号
                {
                    if (!bLastModuleInPosStatus)//上升沿
                    {
                        ModuleInPosTrigger = true;
                    }
                    bLastModuleInPosStatus = true;
                }
                else
                {
                    bLastModuleInPosStatus = false;
                }
                ////
                ////SET、高频焊接信号             M32（未使用）		0
                ////SET、给下压接线盒气缸信号     M33			1
                ////SET、给接线盒压针信号         M34			2
                ////SET、给焊接完成信号           M35			3
                ////SET、吹气                     M36（暂时未使用）	4
                ////SET、视觉NG报警信号           M37			5
                ////SET、出料OK                   M38			6
                ////SET、出料NG                   M39			7
                //if (setM_plc_[0] > 0)//（未使用）
                //{ setM_plc_[0] = 0; res = FXPlc2IO.setM("M32", true); }
                //else if (setM_plc_[1] > 0)//给下压接线盒气缸信号
                //{ setM_plc_[1] = 0; res = FXPlc2IO.setM("M33", true); }
                //else if (setM_plc_[2] > 0)//给接线盒压针信号
                //{ setM_plc_[2] = 0; res = FXPlc2IO.setM("M34", true); }
                //else if (setM_plc_[4] > 0)//给焊接完成信号
                //{ setM_plc_[3] = 0; res = FXPlc2IO.setM("M35", true); }
                //else if (setM_plc_[4] > 0)//（暂时未使用）
                //{ setM_plc_[4] = 0; res = FXPlc2IO.setM("M36", true); }
                //else if (setM_plc_[5] > 0)//视觉NG报警信号 
                //{ setM_plc_[5] = 0; res = FXPlc2IO.setM("M37", true); }
                //else if (setM_plc_[6] > 0)//出料OK
                //{ setM_plc_[6] = 0; res = FXPlc2IO.setM("M38", true); }
                //else if (setM_plc_[7] > 0)//出料NG
                //{ setM_plc_[7] = 0; res = FXPlc2IO.setM("M39", true); }
                ////
                //如果设备急停就关闭软件启动标记，同时跳转到线程起始位置等待
                if (read_plc_[9] > 0)
                {
                    //star_pause_handle = true;//启动忽视检测设备急停 = false
                    _star_PLC = true;//关闭通讯线程
                    bNewMainStop = true;//停止流程
                    bNewMainTerminate = true;//关闭主线程
                    //
                    System.Diagnostics.Process _Process = new System.Diagnostics.Process();
                    _Process.StartInfo.FileName = "Alert_handle_from.exe";
                    _Process.StartInfo.WorkingDirectory = "Alert_handle_from.exe";
                    _Process.StartInfo.CreateNoWindow = true;
                    _Process.Start();

                    HOperatorSet.CloseAllFramegrabbers();
                    //暂停
                    JHCap.CameraStop(g_index);
                    //释放相机
                    JHCap.CameraFree(g_index);
                    //释放资源关闭软件
                    Environment.Exit(1);
                }
                Thread.Sleep(10);
            }
        }
        #endregion
        //   int time_Millling_coutter_head = 300;//用户设定：开始清晰的时间 ImgProcess.weldGrayRight4
        int time_delay = 0; //清晰等待计数
        #region 焊接主线程,上饶晶科
        //焊接主线程,上饶晶科 此处清洗只是长时间等待时清洗 20180705
        private void RefreshProcess()
        {
            int WeldTimes = 0;

            while (!bNewMainTerminate)
            {
                if (bNewMainStop)//启动和停止
                {
                    Thread.Sleep(20);
                    if (time_delay <= ImgProcess.weldGrayRight4)
                    {
                        time_delay++;//增加等待时间
                    }
                    continue;
                }


                //检测组件到位信号 （长时间等待时，自动清洗）
                if (!ModuleInPosTrigger)
                {
                    Thread.Sleep(20);
                    #region 清晰流程
                    time_delay++;//增加等待时间
                    if (ImgProcess.weldGrayRight4 > 0 )//小于0时不清洗
                    {
                        if (time_delay >= ImgProcess.weldGrayRight4)//大于用户设定的时间，则开始清晰动作
                        {
                            time_delay = 0;//清楚等待时间

                            //开始清洗动作
                            if (!bWeldDebug)//非调试模式
                            {
                                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                                {
                                    //          ShowInfo("门打开，设备暂停动作");
                                    Thread.Sleep(20);
                                    continue;
                                }
                                #endregion
                                //移动到清洗位置上方
                                TableCtl.MoveAbsXYR(TableCtl.XErrorSN, TableCtl.YErrorSN, TableCtl.RErrorSN);
                                Thread.Sleep(1000);
                                //等待到位
                                while (TableCtl.InPosXYR() == 0)//或软件暂停
                                {
                                    if (bNewMainStop)
                                        break;
                                    Thread.Sleep(20);
                                    continue;
                                }
                                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                                {
                                    //          ShowInfo("门打开，设备暂停动作");
                                    Thread.Sleep(20);
                                    continue;
                                }
                                #endregion
                                //修改20180420
                                TableCtl.MoveRefR(TableCtl.RErrorSN);//清洗的下降距离（位置）
                                //等待下降到位 20180515金帅
                                while (TableCtl.InPosXYR() == 0)//或软件暂停
                                {
                                    if (bNewMainStop)
                                        break;
                                    Thread.Sleep(20);
                                    continue;
                                }

                                if (ImgProcess.RunClearing_JK == 1)
                                {
                                    #region 应唐永飞要求，增加清洗动作分列式 20180515
                                    //下压到位后等待时间
                                    Thread.Sleep(ImgProcess.CleaningTime_JK);
                                    //向后靠拢
                                    TableCtl.MoveRefXYR(0, ImgProcess.CleaningSwingDist_JK, 0);
                                    //等待到位
                                    while (TableCtl.InPosXYR() == 0)
                                    {
                                        if (bNewMainStop)
                                            break;
                                        Thread.Sleep(20);
                                        continue;
                                    }
                                    Thread.Sleep(ImgProcess.CleaningTime_JK);
                                    //向前靠拢
                                    TableCtl.MoveRefXYR(0, -ImgProcess.CleaningSwingDist_JK, 0);
                                    //等待到位
                                    while (TableCtl.InPosXYR() == 0)
                                    {
                                        if (bNewMainStop)
                                            break;
                                        Thread.Sleep(20);
                                        continue;
                                    }
                                    Thread.Sleep(ImgProcess.CleaningTime_JK);
                                    #endregion
                                }
                                else
                                {
                                    Thread.Sleep(1000);
                                }
                                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                                {
                                    //if (bNewMainStop)
                                    //    continue;
                                    //          ShowInfo("门打开，设备暂停动作");
                                    Thread.Sleep(20);
                                    continue;
                                }
                                #endregion
                                //修改20180415
                                TableCtl.MoveRefR(-TableCtl.RErrorSN);//返回到清洗前的位置
                                Thread.Sleep(1000);
                                while (TableCtl.InPosXYR() == 0)//或软件暂停
                                {
                                    if (bNewMainStop)
                                        break;
                                    Thread.Sleep(20);
                                    continue;
                                }
                            }
                            else
                            {
                                //移动到清晰位置上方
                                TableCtl.MoveAbsXYR(TableCtl.XErrorSN, TableCtl.YErrorSN, TableCtl.RErrorSN);
                                Thread.Sleep(1000);
                                while (TableCtl.InPosXYR() == 0)//或软件暂停
                                {
                                    if (bNewMainStop)
                                        break;
                                    Thread.Sleep(20);
                                    continue;
                                }
                                Thread.Sleep(5000);
                            }
                            //回到待机位置
                            TableCtl.MoveAbsXYR(TableCtl.XDefPos, TableCtl.YDefPos, TableCtl.RDefPos);
                            if (!bWeldDebug)//非调试模式
                                TableCtl.MoveRefT(ImgProcess.holeAreaSel);//清晰结束加一点锡
                        }
                    }
                    #endregion

                    continue;
                }
                time_delay = 0;//清除等待时间
                #region 左右缺锡判断
                ModuleInPosTrigger = false;
                //if (FXPlc2IO.GetDI(UnitDefine.DI_Left_SolderingTin) == 1)
                //解决缺锡不停机的问题20180323
                if (read_plc_[5] > 0)
                {
                    ShowInfo("缺锡");
                    if (_checkbox_alert)
                    { Pause_mutex.WaitOne(); res = FXPlc2IO.setM("M37", true); Pause_mutex.ReleaseMutex(); }//报警（借用视觉NG信号）
                    continue;
                }
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion
                #endregion

                #region 开始拍照定位判断
                //Thread.Sleep(2000);
                //接线盒固定下压气缸下压  FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, true);

                #region 接线盒气缸下压
                using (CheckTime t = new CheckTime("接线盒气缸下压！"))
                {
                    Pause_mutex.WaitOne();
                    res = FXPlc2IO.setM("M33", true);//接线盒固定下压气缸下压
                    Pause_mutex.ReleaseMutex();

                    TableCtl.Log("等待气缸下压到位。");
                    //启动定位检测
                    //      TableCtl.Log("启动检测到位");
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (TableCtl.InPosXYR() == 0)//判断在原点位置
                    {
                        if (bNewMainStop)
                            break;
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                    TableCtl.Log("等待原点位置。");
                    //if (bNewMainStop)
                    //    continue;
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[8] == 0 || bNewMainStop)//或软件暂停
                    {
                        //           ShowInfo("等待接线盒固定气缸下压到位");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                    TableCtl.Log("气缸下压到位。");
                }
                #endregion

                #region 焊前正面图像定位和焊前侧面图像识别
                using (CheckTime t = new CheckTime("焊前正面图像定位与识别和焊前侧面图像识别！"))
                {




                    ImgProcess.resPos = -1;
                    ImgProcess.PosStop = false;//线程标记位启动
                    while (ImgProcess.resPos < 0)
                    {
                        Thread.Sleep(20);
                        continue;
                    }

                    if (ImgProcess.CamSetNum == 1)
                    {
                        //焊前侧面检测
                        //先检测汇流条有没有下压
                        ImgProcess.Q_resTest = -1;
                        ImgProcess.Q_star_Template = false;//
                        while (ImgProcess.Q_resTest < 0)
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                    }
                    else if (ImgProcess.CamSetNum == 2)
                    {

                    }




                    star_Temp_OKNG = true;//初始化时，OK数量加一
                    //     star_Number_OKNG = true;//OKNG记录打开
                    //汇流条未按压
                    if (ImgProcess.Q_error && (ImgProcess.Q_star_Q_lr_checkbox == 1))
                    {
                        ShowInfo("汇流条未压");
                        //FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, false);//抬起接线盒固定按压气缸
                        Pause_mutex.WaitOne();
                        if (_checkbox_alert)
                        { res = FXPlc2IO.setM("M37", true); }//报警（借用视觉NG信号）
                        //20180308
                        //FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, false);//抬起接线盒固定按压气缸
                        res = FXPlc2IO.setM("M33", false);//抬起接线盒固定按压气缸
                        Pause_mutex.ReleaseMutex();
                        ImgProcess.HQ_CM1_ERnumber++;//侧面缺陷加1
                        //     star_Temp_OKNG = false;//NG数量加一
                        updata_Total_ERnumber();//显示识别数据
                        continue;
                    }
                    //如果定位失败则返回初始
                    if (ImgProcess._pos_error)
                    {
                        ShowInfo("影像定位失败");
                        //    TableCtl.Log("影像定位失败");
                        //    Thread.Sleep(50);
                        Pause_mutex.WaitOne();
                        if (_checkbox_alert)
                        { res = FXPlc2IO.setM("M37", true); }//报警（借用视觉NG信号）
                        //FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, false);//抬起接线盒固定按压气缸
                        res = FXPlc2IO.setM("M33", false);//抬起接线盒固定按压气缸
                        Pause_mutex.ReleaseMutex();

                        ImgProcess.EROOR_Location_Number++;//定位失败加1
                        //      star_Temp_OKNG = false;//NG数量加一（有记录标记时不增加数量）
                        updata_Total_ERnumber();//显示识别数据
                        continue;
                    }

                    //焊前正面识别汇流条长度
                    if (ImgProcess._HQ_error && (ImgProcess.Z2_star_zm1_soldering == 1))
                    {
                        Pause_mutex.WaitOne();
                        if (_checkbox_alert)
                        { res = FXPlc2IO.setM("M37", true); }//报警（借用视觉NG信号）
                        //FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, false);//抬起接线盒固定按压气缸
                        res = FXPlc2IO.setM("M33", false);//抬起接线盒固定按压气缸
                        Pause_mutex.ReleaseMutex();

                        ImgProcess.HQ_ZM1_ERnumber++;//焊前汇流条长度识别缺陷加1
                        //      star_Temp_OKNG = false;//NG数量加一（有记录标记时不增加数量）
                        updata_Total_ERnumber();//显示识别数据
                        continue;
                    }

                    ////固定气缸下压
                    //if (bNewMainStop)
                    //    continue;
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                    //=1是定位成功
                    if ((ImgProcess.Pos_Y + TableCtl.YErrorL1 < 0) || Math.Abs(ImgProcess.Pos_R) > 15)
                    {
                        TableCtl.Log("定位偏差过大");
                        TableCtl.MoveAbsXYR(TableCtl.XDefPos, TableCtl.YDefPos, TableCtl.RDefPos);
                        Thread.Sleep(50);
                        Pause_mutex.WaitOne();
                        if (_checkbox_alert)
                        { res = FXPlc2IO.setM("M37", true); }//报警（借用视觉NG信号）
                        //FXPlc2IO.SetDO(UnitDefine.DO_FixedAir, false);//抬起接线盒固定按压气缸
                        res = FXPlc2IO.setM("M33", false);//抬起接线盒固定按压气缸
                        Pause_mutex.ReleaseMutex();
                        continue;
                    }
                    //
                    TableCtl.Log("影像定位成功，开始焊接流程。");
                    //移动左侧位置1
                    TableCtl.MoveAbsXYR((ImgProcess.Pos_X + TableCtl.XErrorL1), (ImgProcess.Pos_Y + TableCtl.YErrorL1), TableCtl.RDefPos);
                    //
                }
                #endregion
                #endregion

                using (CheckTime t = new CheckTime("焊接动作用时！"))
                {
                    if (ImgProcess.star_soldering_mode == 1)
                    {
                        #region 晶科方案1，带涂膜动作
                        //晶科方案1，带涂膜动作
                        //移动等待一段时间后开始加锡
                        Thread.Sleep(ImgProcess.weldBackGray);//送锡延时
                        if (!bWeldDebug)//非调试模式
                            TableCtl.MoveRefT(ImgProcess.holeHeightLeft);//加锡

                        //判断焊头到达指定位置？
                        while (TableCtl.InPosXYR() == 0)//
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(20);
                            continue;
                        }
                        //if (bNewMainStop)
                        //    continue;
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        if (checkBox_WeldType.Checked)//
                        {
                            //？？？
                            TableCtl.WeldAir(1);
                        }
                        else
                        {
                            Thread.Sleep(ImgProcess.weldBackPercent);//下压等待时间
                            //焊头开始下压？
                            TableCtl.WeldMotorDown();
                            Thread.Sleep(100);
                            while (TableCtl.WeldMotorCpt() == 0)//
                            {
                                if (bNewMainStop)
                                    break;
                                Thread.Sleep(20);
                                continue;
                            }
                        }
                        //if (bNewMainStop)
                        //    continue;
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        //到位开始焊接
                        bool WeldOk = Weld_ActionJK();//默认：晶科方案1（涂膜焊锡动作）
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion


                        //
                        #endregion
                    }
                    else if (ImgProcess.star_soldering_mode == 2)
                    {
                        #region 晶科方案2，无涂膜动作，压针朝外

                        //晶科方案2，无涂膜动作，压针朝外
                        //移动等待一段时间后开始加锡
                        Thread.Sleep(ImgProcess.time_add_soldering);//送锡前延时
                        if (!bWeldDebug)//非调试模式
                            TableCtl.MoveRefT(ImgProcess.star_add_soldering_L);//加锡


                        using (CheckTime m = new CheckTime("焊头下压用时(不计入总时间)！"))
                        {
                            //判断焊头到达指定位置？
                            while (TableCtl.InPosXYR() == 0)
                            {
                                if (bNewMainStop)
                                    break;
                                Thread.Sleep(20);
                                continue;
                            }
                            //if (bNewMainStop)
                            //    continue;
                            #region //注销20180310 原因：接线盒下压组件下压，不焊接
                            while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                            {
                                //if (bNewMainStop)
                                //    continue;
                                //          ShowInfo("门打开，设备暂停动作");
                                Thread.Sleep(20);
                                continue;
                            }
                            #endregion

                            //
                            if (checkBox_WeldType.Checked)//
                            {
                                //？？？
                                TableCtl.WeldAir(1);
                            }
                            else
                            {
                                Thread.Sleep(ImgProcess.time_fornt_pushing);//下压等待时间
                                //焊头开始下压？
                                TableCtl.WeldMotorDown();
                                Thread.Sleep(100);

                                while (TableCtl.WeldMotorCpt() == 0)//判断是否下压到位？
                                {
                                    if (bNewMainStop)//
                                        break;
                                    Thread.Sleep(20);
                                    continue;
                                }
                                //根据客服唐勇飞要求添加动作    20180424
                                //焊头下压到位后等待一定时间，微抬起等待一定时间，再下压
                                #region  新增动作20180424  20180627注销
                                //Thread.Sleep(ImgProcess.time_pushing_soldering_jk1);//Z轴下压后到位后等待时间
                                //TableCtl.MoveRefXYR(0, 0, ImgProcess.before_compensation_soldering_jk);//Z轴微抬起距离
                                //Thread.Sleep(ImgProcess.time_pushing_soldering_jk2);//Z轴抬起后等待时间
                                ////焊头开始下压
                                //TableCtl.WeldMotorDown();
                                //Thread.Sleep(100);

                                //while (TableCtl.WeldMotorCpt() == 0)//判断是否下压到位？
                                //{
                                //    if (bNewMainStop)
                                //        break;
                                //    Thread.Sleep(20);
                                //    continue;
                                //}
                                #endregion
                            }


                        }
                        if (bNewMainStop)
                            continue;
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        bool WeldOk = Weld_ActionJK2();//默认：晶科方案2
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        #endregion
                    }
                    else
                    {
                        #region 方案3，增加填孔摆动动作
                        //移动等待一段时间后开始加锡
                        Thread.Sleep(ImgProcess.time_add_soldering_YL);//送锡前延时
                        if (!bWeldDebug)//非调试模式
                            TableCtl.MoveRefT(ImgProcess.star_add_soldering_L_YL);//加锡

                        //判断焊头到达指定位置？
                        while (TableCtl.InPosXYR() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(20);
                            continue;
                        }
                        //if (bNewMainStop)
                        //    continue;
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        if (checkBox_WeldType.Checked)//
                        {
                            //？？？
                            TableCtl.WeldAir(1);
                        }
                        else
                        {
                            Thread.Sleep(ImgProcess.time_fornt_pushing_YL);//下压等待时间
                            //焊头开始下压？
                            TableCtl.WeldMotorDown();
                            Thread.Sleep(100);

                            while (TableCtl.WeldMotorCpt() == 0)//判断是否下压到位？
                            {
                                if (bNewMainStop)//
                                    break;
                                Thread.Sleep(20);
                                continue;
                            }

                        }
                        if (bNewMainStop)
                            continue;
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        bool WeldOk = Weld_ActionJK3();//默认：方案3，填孔
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //
                        #endregion
                    }
                }

                if (checkBox_WeldType.Checked)//
                {
                    Thread.Sleep(200);
                }
                else
                {
                    #region 焊接完成抬起同时开始拍条码
                    using (CheckTime t = new CheckTime("条码拍照和焊头抬起！"))
                    {
                        TableCtl.Log("焊接完成，开始条码拍摄");
                        if (bNewMainStop)
                            continue;
                        while (read_plc_[3] > 0)
                        {
                            if (bNewMainStop)
                                continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }

                        //
                        TableCtl.WeldMotorUp(); //抬起焊头
                        //         TableCtl.Log("开始条码");
                        ImgProcess.SN_resTest = -1;
                        ImgProcess.TestStopSN = false;
                        //等待焊头到达顶部
                        while (TableCtl.WeldMotorCpt() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(10);
                            continue;
                        }
                    }
                    #endregion
                }

                #region 返回到初始位置完成条码识别
                using (CheckTime t = new CheckTime("焊接完成后返回初始位置！"))
                {
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                    TableCtl.Log("返回默认位置。");
                    //移动到默认位置  开始影像检TableCtl.XErrorL2
                    TableCtl.MoveAbsXYR((ImgProcess.Pos_X + TableCtl.XErrorL2), (ImgProcess.Pos_Y + TableCtl.YErrorL2), TableCtl.RDefPos);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        if (bNewMainStop)
                            break;
                        Thread.Sleep(10);
                        continue;
                    }
                }


                //等待条码识别完成
                while (ImgProcess.SN_resTest < 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                if (ImgProcess.SN_error && (ImgProcess.SN_star_SN_checkbox == 1))//判断条码识别状态
                {
                    ImgProcess.ERROR_SNnumber++;//条码失败加1
                    //   star_Temp_OKNG = false;//NG数量加一（有记录标记时不增加数量）
                    updata_Total_ERnumber();//显示识别数据
                    //continue;
                }
                #endregion

                //此处应该拍照检测20180622
                #region 20180622 新增添加 组件下压机构
                //下压气缸抬升
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M33", false);//下压气缸抬升
                Pause_mutex.ReleaseMutex();
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    Thread.Sleep(20);
                    continue;
                }
                #endregion
                //松开限位20180627
                //Pause_mutex.WaitOne();
                //res = FXPlc2IO.setM("M35", true);//焊接完成
                //Pause_mutex.ReleaseMutex();
                //while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                //{
                //    Thread.Sleep(20);
                //    continue;
                //}
                



                #region A03焊后正面识别，更新显示判断数据
                using (CheckTime t = new CheckTime("开始正面识别！"))
                {
                    TableCtl.Log("开始正面识别。");
                    ////焊接后正面识别
                    ImgProcess.Z2_resTest = -1;
                    ImgProcess.Z2_star_Positive = false;
                    while (ImgProcess.Z2_resTest < 0)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    //       TableCtl.Log("检测拍照完成");
                    if (ImgProcess.Z2_error)
                    {
                        if (_checkbox_alert)
                        { Pause_mutex.WaitOne(); FXPlc2IO.SetDO(UnitDefine.DO_str_alert, true); Pause_mutex.ReleaseMutex(); }//报警
                        ImgProcess.HH_ZM2_ERnumber++;//
                        star_Temp_OKNG = false;//NG数量加一（有记录标记时不增加数量）
                    }

                }
                #endregion

                #region A02焊接完成开始清晰动作
                #endregion













                using (CheckTime t = new CheckTime("清洗动作！"))
                {
                    TableCtl.Log("开始清洗动作。");
                    bool VISOINRESULT = false;
                    VISOINRESULT = star_Temp_OKNG;
                    //
                    updata_Total_ERnumber();//显示识别数据
                    //


                    if (!bWeldDebug)//非调试模式
                    {
                        #region 20180627 注销 限位松开
                        Pause_mutex.WaitOne();
                        res = FXPlc2IO.setM("M35", true);//焊接完成
                        Pause_mutex.ReleaseMutex();
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //移动到清晰位置上方 2080628注销
                        //TableCtl.MoveAbsXYR(TableCtl.XErrorSN, TableCtl.YErrorSN, TableCtl.RErrorSN);
                        //Thread.Sleep(1000);

                        #region 20180622 注销 组件下压机构
                        ////下压气缸抬升
                        //Pause_mutex.WaitOne();
                        //res = FXPlc2IO.setM("M33", false);//下压气缸抬升
                        //Pause_mutex.ReleaseMutex();
                        //#region //注销20180310 原因：接线盒下压组件下压，不焊接
                        //while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        //{
                        //    Thread.Sleep(20);
                        //    continue;
                        //}
                        #endregion
                        //
                        Pause_mutex.WaitOne();
                        if (VISOINRESULT)//发送识别OK或NG信号
                            res = FXPlc2IO.setM("M38", true);
                        else
                            res = FXPlc2IO.setM("M39", true);
                        Pause_mutex.ReleaseMutex();

                        //移动到清晰位置上方 2080628注销
                        //while (TableCtl.InPosXYR() == 0)
                        //{
                        //    if (bNewMainStop)
                        //        break;
                        //    Thread.Sleep(50);
                        //    continue;
                        //}
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            //if (bNewMainStop)
                            //    continue;
                            //          ShowInfo("门打开，设备暂停动作");
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //新增20180705
                        TotelNum++;
                        if (TotelNum == ImgProcess.time_pushing_soldering_jk1)
                        {
                            ImgProcess.ClearNum = 1;
                            TotelNum = 0;
                        }
                        else
                        {
                            ImgProcess.ClearNum = 0;
                        }
                        if (TotelNum > ImgProcess.time_pushing_soldering_jk1)
                        {
                            TotelNum = 0;
                        }


                        if (ImgProcess.ClearNum==1)
                        {
                        //移动到清洗位置上方 2080628新增
                        TableCtl.MoveAbsXYR(TableCtl.XErrorSN, TableCtl.YErrorSN, TableCtl.RErrorSN);
                        Thread.Sleep(1000);
                        while (TableCtl.InPosXYR() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(50);
                            continue;
                        }

                        TableCtl.MoveRefR(TableCtl.RErrorSN);//清洗的下降距离
                        while (TableCtl.InPosXYR() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(50);
                            continue;
                        }

                        if (ImgProcess.RunClearing_JK == 1)
                        {
                            #region 应唐永飞要求，增加清洗动作分列式 20180517
                            //下压到位后等待时间
                            Thread.Sleep(ImgProcess.CleaningTime_JK);
                            //向后靠拢
                            TableCtl.MoveRefXYR(0, ImgProcess.CleaningSwingDist_JK, 0);
                            //等待到位
                            while (TableCtl.InPosXYR() == 0)
                            {
                                if (bNewMainStop)
                                    break;
                                Thread.Sleep(20);
                                continue;
                            }
                            Thread.Sleep(ImgProcess.CleaningTime_JK);
                            //向前靠拢
                            TableCtl.MoveRefXYR(0, -ImgProcess.CleaningSwingDist_JK, 0);
                            //等待到位
                            while (TableCtl.InPosXYR() == 0)
                            {
                                if (bNewMainStop)
                                    break;
                                Thread.Sleep(20);
                                continue;
                            }
                            Thread.Sleep(ImgProcess.CleaningTime_JK);
                            #endregion
                        }
                        else
                        {
                            Thread.Sleep(1500);
                        }

                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion
                        //
                        TableCtl.MoveRefR(-TableCtl.RErrorSN);//清洗后抬起的距离
                        Thread.Sleep(1000);
                        while (TableCtl.InPosXYR() == 0)//判断轴位置
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(10);
                            continue;
                        }
                        //新增20180628
                        TableCtl.MoveRefT(ImgProcess.holeAreaSel);//清洗结束加一点锡
                   
                        }
                    }
                    else
                    {
                        Pause_mutex.WaitOne();
                        res = FXPlc2IO.setM("M35", true);//焊接完成
                        Pause_mutex.ReleaseMutex();
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion

                        //移动到清晰位置上方
                        TableCtl.MoveAbsXYR(TableCtl.XErrorSN, TableCtl.YErrorSN, TableCtl.RErrorSN);
                        //
                        Thread.Sleep(1000);
                        Pause_mutex.WaitOne();
                        res = FXPlc2IO.setM("M33", false);//下压气缸抬升
                        Pause_mutex.ReleaseMutex();
                        while (read_plc_[8] > 0)//判断下压气缸到达原始位置
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        //
                        Pause_mutex.WaitOne();
                        if (VISOINRESULT)//发送识别OK或NG信号
                            res = FXPlc2IO.setM("M38", true);//
                        else
                            res = FXPlc2IO.setM("M39", true);//
                        Pause_mutex.ReleaseMutex();
                        //
                        while (TableCtl.InPosXYR() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(50);
                            continue;
                        }
                        //调试模式不做补锡动作
                        Thread.Sleep(5000);
                    }
                    time_delay = 0;//清楚等待时间

                    //if (!bWeldDebug)//非调试模式 注销20180628
                    //    TableCtl.MoveRefT(ImgProcess.holeAreaSel);//清洗结束加一点锡
                }


                #endregion

                #region A01设备返回待机位置
                using (CheckTime t = new CheckTime("设备返回待机位置！"))
                {
                    //检测结果
                    #region 设备回零动作
                    WeldTimes++;
                    //30次回零
                    if (WeldTimes > TableCtl.XErrorR2)
                    {
                        #region //注销20180310 原因：接线盒下压组件下压，不焊接
                        while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        #endregion
                        //
                        TableCtl.MoveAbsXYR(100, 100, 0);
                        WeldTimes = 0;
                        while (TableCtl.InPosXYR() == 0)
                        {
                            if (bNewMainStop)
                                break;
                            Thread.Sleep(50);
                            continue;
                        }

                        HoldOnHoming();
                        //          ShowInfo("回零完成");
                    }
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                    #endregion

                    //回到待机位置
                    TableCtl.MoveAbsXYR(TableCtl.XDefPos, TableCtl.YDefPos, TableCtl.RDefPos);
                    //
                    Thread.Sleep(1000);
                    TableCtl.Log("清洗完成回到待机位置。");
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0)
                    {
                        if (bNewMainStop)
                            continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion
                }
                #endregion



            }

        }































        public int HoldOnHoming()
        {
            TableCtl.Home();
            while (TableCtl.HomeCpt() == 0)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }
            TableCtl.HomeClear();
            return 0;
        }
        #endregion
        #region 默认：晶科方案1
        //默认：晶科方案1
        public bool Weld_ActionJK()
        {
            Thread.Sleep(ImgProcess.holeWidthSel);//下压焊接时间
            #region //注销20180310 原因：接线盒下压组件下压，不焊接
            while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
            {
                //if (bNewMainStop)
                //    continue;
                //          ShowInfo("门打开，设备暂停动作");
                Thread.Sleep(20);
                continue;
            }
            #endregion

            //
            TableCtl.MoveRefXYR(0, 0, ImgProcess.weldLengthPercent);//Z轴抬起距离
            while (TableCtl.InPosXYR() == 0)//等待抬起完成
            {
                //if (bNewMainStop)
                //    continue;
                Thread.Sleep(20);
                continue;
            }
            double OldXVel = TableCtl.XVel;//存放之前的速度
            double OldYVel = TableCtl.YVel;
            #region //注销20180310 原因：接线盒下压组件下压，不焊接
            while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
            {
                //if (bNewMainStop)
                //    continue;
                //          ShowInfo("门打开，设备暂停动作");
                Thread.Sleep(20);
                continue;
            }
            #endregion

            //
            if (!bWeldDebug)//非调试模式
            {
                TableCtl.XVel = TableCtl.YVel = ImgProcess.weldGrayRight3;//设置移动速度？
                TableCtl.SetParam();                        //写入速度？
                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);//补锡长度
                Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.weldGrayRight2);
                int Times = ImgProcess.HoleAreaPercent;//填空移动次数 
                for (int i = 0; i < Times; i++)
                {
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion

                    //
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.weldGrayRight2);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.weldGrayRight2);

                }
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //
                TableCtl.MoveRefXYR(0, 0, ImgProcess.weldGrayRight1);//焊头抬起一定距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //Y向前移动
                TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                ////开始吹气压针动作
                //FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);//吹气（冷却）打开
                //FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);//压针气缸下压
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M36", true);
                res = FXPlc2IO.setM("M34", true);
                Pause_mutex.ReleaseMutex();
                Thread.Sleep(ImgProcess.weldWidthRight);//压针与吹气延时时间（冷却）
                //FXPlc2IO.SetDO(UnitDefine.DO_BusPress, false);//抬起压针气缸
                //FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, false);//吹气（冷却）停止
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M36", false);
                res = FXPlc2IO.setM("M34", false);
                Pause_mutex.ReleaseMutex();

                TableCtl.XVel = OldXVel;//返回之前的速度
                TableCtl.YVel = OldYVel;
                TableCtl.SetParam();    //初始化移动速度？

            }
            else
            {
                Thread.Sleep(3200);
            }
            return true;
        }
        #endregion

        #region 晶科方案2
        //晶科方案2
        public bool Weld_ActionJK2()
        {
            double OldXVel = TableCtl.XVel;//存放之前的速度
            double OldYVel = TableCtl.YVel;
            TableCtl.XVel = TableCtl.YVel = ImgProcess.paint_speed;//设置移动速度？
            TableCtl.SetParam();                        //写入速度？
            //调试时不做补锡等动作
            if (!bWeldDebug)//非调试模式
            {
                Thread.Sleep(ImgProcess.time_pushing_soldering);//下压焊接时间
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                TableCtl.MoveRefXYR(0, 0, ImgProcess.before_compensation_soldering);//补锡前Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                TableCtl.MoveRefT(ImgProcess.compensation_soldering_L);//补锡长度
                Thread.Sleep(ImgProcess.before_time_paint);//涂抹前等待时间
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time);
                int Times = ImgProcess.paint_number;//涂抹次数 
                for (int i = 0; i < Times; i++)
                {
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion

                    //
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }

                //涂抹后焊头抬起一定距离 再向前或向左右移动 20180529
                TableCtl.MoveRefXYR(0, 0, ImgProcess.accomplish_lift_dist);//Z轴抬起距离
                //判断是否抬起到位
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(120);
                    continue;
                }

                //Z:20180418
                //改为X轴移动距离
                if (ImgProcess._cB_paint_X_displacement_distance_)
                {
                    //X向左或右移动
                    TableCtl.MoveRefXYR(ImgProcess.paint_Y_displacement_distance, 0, 0);
                }
                else
                {
                    //Y向前移动
                    TableCtl.MoveRefXYR(0, ImgProcess.paint_Y_displacement_distance, 0);
                }
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(120);
                    continue;
                }
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                ////
                ////开始吹气压针动作
                //FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);//压针气缸下压
                //FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);//吹气（冷却）打开
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M36", true);
                res = FXPlc2IO.setM("M34", true);
                Pause_mutex.ReleaseMutex();
                TableCtl.MoveRefXYR(0, 0, ImgProcess.accomplish_lift_distance);//补锡完成Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.time_accomplish_soldering);//压针与吹气延时时间（冷却）
                //FXPlc2IO.SetDO(UnitDefine.DO_BusPress, false);//抬起压针气缸
                //FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, false);//吹气（冷却）停止
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M36", false);
                res = FXPlc2IO.setM("M34", false);
                Pause_mutex.ReleaseMutex();

            }
            else
            {
                //Thread.Sleep(ImgProcess.time_pushing_soldering);//下压焊接时间
                //while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                //{
                //    if (bNewMainStop)
                //        continue;
                //    //      ShowInfo("门打开，设备暂停动作");
                //    Thread.Sleep(20);
                //    continue;
                //}
                TableCtl.MoveRefXYR(0, 0, ImgProcess.before_compensation_soldering);//补锡前Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //TableCtl.MoveRefT(ImgProcess.compensation_soldering_L);//补锡长度
                Thread.Sleep(ImgProcess.before_time_paint);//涂抹前等待时间
                //while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                //{
                //    if (bNewMainStop)
                //        continue;
                //    //    ShowInfo("门打开，设备暂停动作");
                //    Thread.Sleep(20);
                //    continue;
                //}
                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time);
                int Times = ImgProcess.paint_number;//涂抹次数 
                for (int i = 0; i < Times; i++)
                {
                    //while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                    //{
                    //    if (bNewMainStop)
                    //        continue;
                    //    //         ShowInfo("门打开，设备暂停动作");
                    //    Thread.Sleep(20);
                    //    continue;
                    //}
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //Z:20180418
                //改为X轴移动距离
                if (ImgProcess._cB_paint_X_displacement_distance_)
                {
                    //X向左或右移动
                    TableCtl.MoveRefXYR(ImgProcess.paint_Y_displacement_distance, 0, 0);
                }
                else
                {
                    //Y向前移动
                    TableCtl.MoveRefXYR(0, ImgProcess.paint_Y_displacement_distance, 0);
                }
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                //{
                //    if (bNewMainStop)
                //        continue;
                //    //         ShowInfo("门打开，设备暂停动作");
                //    Thread.Sleep(20);
                //    continue;
                //}
                //       //开始吹气压针动作
                //       FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);//压针气缸下压
                //       FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);//吹气（冷却）打开
                TableCtl.MoveRefXYR(0, 0, ImgProcess.accomplish_lift_distance);//补锡完成Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.time_accomplish_soldering);//压针与吹气延时时间（冷却）
                //     FXPlc2IO.SetDO(UnitDefine.DO_BusPress, false);//抬起压针气缸
                //     FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, false);//吹气（冷却）停止

            }
            TableCtl.XVel = OldXVel;//返回之前的速度
            TableCtl.YVel = OldYVel;
            TableCtl.SetParam();    //初始化移动速度？

            return true;
        }
        #endregion

        #region 镇江英利方案3
        //方案3
        public bool Weld_ActionJK3()
        {
            double OldXVel = TableCtl.XVel;//存放之前的速度
            double OldYVel = TableCtl.YVel;
            TableCtl.XVel = TableCtl.YVel = ImgProcess.paint_speed_YL;//设置移动速度？
            TableCtl.SetParam();                        //写入速度？
            //调试时不做补锡等动作
            if (!bWeldDebug)//非调试模式
            {
                Thread.Sleep(ImgProcess.time_pushing_soldering_YL);//下压焊接时间
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion
                //
                TableCtl.MoveRefXYR(0, 0, ImgProcess.before_compensation_soldering_YL);//补锡前Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                TableCtl.MoveRefT(ImgProcess.compensation_soldering_L_YL);//补锡长度
                Thread.Sleep(ImgProcess.before_time_paint_YL);//涂抹前等待时间
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance_YL, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time_YL);
                int Times = ImgProcess.paint_number_YL;//涂抹次数 
                for (int i = 0; i < Times; i++)
                {
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion

                    //
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance_YL * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time_YL);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance_YL * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time_YL);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance_YL, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //
                //Y向前移动
                TableCtl.MoveRefXYR(0, ImgProcess.paint_Y_displacement_distance_YL, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(120);
                    continue;
                }
                //
                //
                //二次涂抹填孔开始
                TableCtl.XVel = TableCtl.YVel = ImgProcess.paint_speed_YL2;//设置移动速度？
                TableCtl.SetParam();                        //写入速度？
                TableCtl.MoveRefT(ImgProcess.compensation_soldering_L_YL2);//补锡长度
                Thread.Sleep(ImgProcess.before_time_paint_YL2);//涂抹前等待时间
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                //
                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance_YL2, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time_YL2);
                int Times2 = ImgProcess.paint_number_YL2;//涂抹次数 
                for (int i = 0; i < Times2; i++)
                {
                    #region //注销20180310 原因：接线盒下压组件下压，不焊接
                    while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                    {
                        //if (bNewMainStop)
                        //    continue;
                        //          ShowInfo("门打开，设备暂停动作");
                        Thread.Sleep(20);
                        continue;
                    }
                    #endregion

                    //
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance_YL2 * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time_YL2);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance_YL2 * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time_YL2);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance_YL2, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                #region //注销20180310 原因：接线盒下压组件下压，不焊接
                while (read_plc_[3] > 0 || bNewMainStop)//或软件暂停
                {
                    //if (bNewMainStop)
                    //    continue;
                    //          ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(20);
                    continue;
                }
                #endregion

                #region 临时添，可能需要注销 加20180317 金帅
                TableCtl.MoveRefXYR(0, 0, ImgProcess.accomplish_lift_distance_YL);//补锡完成Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {

                    Thread.Sleep(20);
                    continue;
                }

                #endregion
                //
                //
                ////
                ////开始吹气压针动作
                //FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);//压针气缸下压
                //FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);//吹气（冷却）打开
                Pause_mutex.WaitOne();
                res = FXPlc2IO.setM("M36", true);
                Thread.Sleep(ImgProcess.time_accomplish_soldering_YL);//压针与吹气延时时间（冷却）
                res = FXPlc2IO.setM("M36", false);
                Pause_mutex.ReleaseMutex();

            }
            else
            {
                TableCtl.MoveRefXYR(0, 0, ImgProcess.before_compensation_soldering_YL);//补锡前Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.before_time_paint);//涂抹前等待时间
                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time);
                int Times = ImgProcess.paint_number;//涂抹次数 
                for (int i = 0; i < Times; i++)
                {
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //
                //Y向前移动
                TableCtl.MoveRefXYR(0, ImgProcess.paint_Y_displacement_distance, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }

                Thread.Sleep(ImgProcess.before_time_paint);//涂抹前等待时间

                TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance, 0, 0);//涂膜焊锡半径X，先往一侧移动
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.paint_time);
                int Times3 = ImgProcess.paint_number;//涂抹次数 
                for (int i = 0; i < Times; i++)
                {
                    //while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                    //{
                    //    if (bNewMainStop)
                    //        continue;
                    //    //         ShowInfo("门打开，设备暂停动作");
                    //    Thread.Sleep(20);
                    //    continue;
                    //}
                    //再往反方向移动两倍的距离，正好到达另外一侧
                    TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);
                    //再返回到另外一侧
                    TableCtl.MoveRefXYR(ImgProcess.paint_X_displacement_distance * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        //if (bNewMainStop)
                        //    continue;
                        Thread.Sleep(20);
                        continue;
                    }
                    Thread.Sleep(ImgProcess.paint_time);

                }
                //返回到初始位置
                TableCtl.MoveRefXYR(-ImgProcess.paint_X_displacement_distance, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                //
                //
                //       //开始吹气压针动作
                //       FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);//压针气缸下压
                //       FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);//吹气（冷却）打开
                TableCtl.MoveRefXYR(0, 0, ImgProcess.accomplish_lift_distance);//补锡完成Z轴抬起距离
                while (TableCtl.InPosXYR() == 0)//等待抬起完成
                {
                    //if (bNewMainStop)
                    //    continue;
                    Thread.Sleep(20);
                    continue;
                }
                Thread.Sleep(ImgProcess.time_accomplish_soldering);//压针与吹气延时时间（冷却）
                //     FXPlc2IO.SetDO(UnitDefine.DO_BusPress, false);//抬起压针气缸
                //     FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, false);//吹气（冷却）停止

            }
            TableCtl.XVel = OldXVel;//返回之前的速度
            TableCtl.YVel = OldYVel;
            TableCtl.SetParam();    //初始化移动速度？

            return true;
        }
        #endregion
        //
        #region 其他厂家焊接涂膜动作
        public bool Weld_ActionXX()
        {

            //int Dis = 4;


            Thread.Sleep(ImgProcess.holeWidthSel);//下压等待延时

            while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
            {
                if (bNewMainStop)
                    return false;
                ShowInfo("门打开，设备暂停动作");
                Thread.Sleep(50);
                continue;
            }


            TableCtl.MoveRefR(ImgProcess.weldLengthPercent);
            while (TableCtl.InPosXYR() == 0)
            {
                if (bNewMainStop)
                    return false;
                Thread.Sleep(50);
                continue;
            }
            double OldXVel = TableCtl.XVel;
            double OldYVel = TableCtl.YVel;
            while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
            {
                if (bNewMainStop)
                    return false;
                ShowInfo("门打开，设备暂停动作");
                Thread.Sleep(50);
                continue;
            }
            if (!bWeldDebug)//非调试模式
            {
                TableCtl.XVel = TableCtl.YVel = 100;
                TableCtl.SetParam();
                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);
                Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时

                while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                {
                    if (bNewMainStop)
                        return false;
                    ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }

                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }




                while (FXPlc2IO.GetDI(UnitDefine.DI_SafeSensor) == 1)
                {
                    if (bNewMainStop)
                        return false;
                    ShowInfo("门打开，设备暂停动作");
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }

                FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, true);

                TableCtl.XVel = OldXVel;
                TableCtl.YVel = OldYVel;
                TableCtl.SetParam();
                Thread.Sleep(ImgProcess.weldWidthRight);
                TableCtl.MoveRefXYR(0, 0, TableCtl.RErrorR1);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                FXPlc2IO.SetDO(UnitDefine.DO_Blowing_Cooling, false);
            }
            else
            {
                Thread.Sleep(3200);
            }
            return true;
        }
        public bool Weld_ActionSD()//尚德
        {

            //int Dis = 4;

            Thread.Sleep(ImgProcess.holeWidthSel);//下压等待延时
            TableCtl.MoveRefR(ImgProcess.weldLengthPercent);
            while (TableCtl.InPosXYR() == 0)
            {
                if (bNewMainStop)
                    return false;
                Thread.Sleep(50);
                continue;
            }
            double OldXVel = TableCtl.XVel;
            double OldYVel = TableCtl.YVel;
            if (!bWeldDebug)//非调试模式
            {
                TableCtl.XVel = TableCtl.YVel = 100;
                TableCtl.SetParam();


                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);
                Thread.Sleep(500);//送锡延时
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }

                TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                int Times = 2;//填空移动次数 
                for (int i = 0; i < Times; i++)
                {

                    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        if (bNewMainStop)
                            return false;
                        Thread.Sleep(50);
                        continue;
                    }
                    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        if (bNewMainStop)
                            return false;
                        Thread.Sleep(50);
                        continue;
                    }


                }
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }


                TableCtl.XVel = OldXVel;
                TableCtl.YVel = OldYVel;

                TableCtl.MoveRefXYR(0, 0, TableCtl.RErrorR1);
                TableCtl.SetParam();
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(TableCtl.XErrorR1, TableCtl.YErrorR1 , 0);
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(0, 0, -TableCtl.RErrorR1);
                //TableCtl.SetParam();
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //Thread.Sleep(1500);

            }
            else
            {
                Thread.Sleep(3200);
            }
            return true;
        }
        public bool Weld_ActionYL()//英利
        {

            //int Dis = 4;

            Thread.Sleep(ImgProcess.holeWidthSel);//下压等待延时
            TableCtl.MoveRefR(ImgProcess.weldLengthPercent);
            while (TableCtl.InPosXYR() == 0)
            {
                if (bNewMainStop)
                    return false;
                Thread.Sleep(50);
                continue;
            }
            double OldXVel = TableCtl.XVel;
            double OldYVel = TableCtl.YVel;
            if (!bWeldDebug)//非调试模式
            {
                TableCtl.XVel = TableCtl.YVel = 100;
                TableCtl.SetParam();


                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);
                Thread.Sleep(500);//送锡延时
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }

                TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);
                int Times = 3;//填空移动次数 
                for (int i = 0; i < Times; i++)
                {

                    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        if (bNewMainStop)
                            return false;
                        Thread.Sleep(50);
                        continue;
                    }
                    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        if (bNewMainStop)
                            return false;
                        Thread.Sleep(50);
                        continue;
                    }


                }
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    if (bNewMainStop)
                        return false;
                    Thread.Sleep(50);
                    continue;
                }


                TableCtl.XVel = OldXVel;
                TableCtl.YVel = OldYVel;

                TableCtl.MoveRefXYR(0, 0, TableCtl.RErrorR1);
                TableCtl.SetParam();
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(TableCtl.XErrorR1, TableCtl.YErrorR1 , 0);
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(0, 0, -TableCtl.RErrorR1);
                //TableCtl.SetParam();
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //Thread.Sleep(1500);

            }
            else
            {
                Thread.Sleep(3200);
            }
            return true;
        }
        public bool Weld_Action()
        {

            //int Dis = 4;

            Thread.Sleep(ImgProcess.holeWidthSel);//下压等待延时

            TableCtl.MoveRefR(ImgProcess.weldLengthPercent);
            while (TableCtl.InPosXYR() == 0)
            {
                if (bNewMainStop)
                    return false;
                Thread.Sleep(50);
                continue;
            }
            double OldXVel = TableCtl.XVel;
            double OldYVel = TableCtl.YVel;
            if (!bWeldDebug)//非调试模式
            {
                TableCtl.XVel = TableCtl.YVel = 100;
                TableCtl.SetParam();
                TableCtl.MoveRefT(ImgProcess.holeWidthLeft);
                Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时

                //for (int i = 0; i < 3; i++)
                //{
                //    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                //    while (TableCtl.InPosXYR() == 0)
                //    {
                //        Thread.Sleep(50);
                //        continue;
                //    }
                //    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                //    while (TableCtl.InPosXYR() == 0)
                //    {
                //        Thread.Sleep(50);
                //        continue;
                //    }
                //    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                //    while (TableCtl.InPosXYR() == 0)
                //    {
                //        Thread.Sleep(50);
                //        continue;
                //    }
                //    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                //    while (TableCtl.InPosXYR() == 0)
                //    {
                //        Thread.Sleep(50);
                //        continue;
                //    }

                //}
                //1-20
                TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                int Times = 2;//填空移动次数 
                for (int i = 0; i < Times; i++)
                {

                    TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    TableCtl.MoveRefXYR(ImgProcess.weldSmallSelLength * 2, 0, 0);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }


                }
                TableCtl.MoveRefXYR(-ImgProcess.weldSmallSelLength, 0, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }

                ////二次
                //TableCtl.MoveRefXYR(0, 0, 3);
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(0, ImgProcess.weldSmallSelWidth, 0);
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                //TableCtl.MoveRefXYR(0, 0, -2.7);
                //while (TableCtl.InPosXYR() == 0)
                //{
                //    Thread.Sleep(50);
                //    continue;
                //}
                ////TableCtl.MoveRefT(ImgProcess.holeWidthLeft * 0.5);
                //Thread.Sleep(ImgProcess.weldHeightRight);//送锡延时



                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.XVel = OldXVel;
                TableCtl.YVel = OldYVel;

                //Thread.Sleep(1000);
                TableCtl.MoveRefXYR(0, 0, TableCtl.RErrorR1);
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                TableCtl.MoveRefXYR(TableCtl.XErrorR1, TableCtl.YErrorR1, 0);
                while (TableCtl.InPosXYR() == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                FXPlc2IO.SetDO(UnitDefine.DO_BusPress, true);
                //TableCtl.WeldAir(1);
                TableCtl.SetParam();
                Thread.Sleep(1500);
                //TableCtl.WeldAir(0);
                FXPlc2IO.SetDO(UnitDefine.DO_BusPress, false);
            }
            else
            {
                Thread.Sleep(3200);
            }
            return true;
        }
        #endregion

        #region 定位和焊接参数设置部分
        private void button_TABLE_PARAM_SET_Click(object sender, EventArgs e)
        {
            try
            {
                TableCtl.XScale = int.Parse(textBox_XSCALE.Text);
                TableCtl.XAcc = double.Parse(textBox_XYACC.Text);
                TableCtl.XVel = double.Parse(textBox_XYVEL.Text);
                TableCtl.XErrorL1 = double.Parse(textBox_XERROR.Text);
                TableCtl.XErrorR1 = double.Parse(textBox_XERROR2.Text);
                TableCtl.XErrorL2 = double.Parse(textBox_XERRORL.Text);
                TableCtl.XErrorR2 = double.Parse(textBox_XERROR2R.Text);
                TableCtl.XErrorSN = double.Parse(textBox_XERRORSN.Text);
                TableCtl.XDefPos = double.Parse(textBox_XDEFPOS.Text);

                TableCtl.YScale = int.Parse(textBox_YSCALE.Text);
                TableCtl.YAcc = double.Parse(textBox_XYACC.Text);
                TableCtl.YVel = double.Parse(textBox_XYVEL.Text);
                TableCtl.YErrorL1 = double.Parse(textBox_YERROR.Text);
                TableCtl.YErrorR1 = double.Parse(textBox_YERROR2.Text);
                TableCtl.YErrorL2 = double.Parse(textBox_YERRORL.Text);
                TableCtl.YErrorR2 = double.Parse(textBox_YERROR2R.Text);
                TableCtl.YErrorSN = double.Parse(textBox_YERRORSN.Text);
                TableCtl.YDefPos = double.Parse(textBox_YDELPOS.Text);

                TableCtl.RScale = int.Parse(textBox_RSCALE.Text);
                TableCtl.RAcc = double.Parse(textBox_RACC.Text);
                TableCtl.RVel = double.Parse(textBox_RVEL.Text);
                TableCtl.RErrorL1 = double.Parse(textBox_RERROR.Text);
                TableCtl.RErrorR1 = double.Parse(textBox_RERROR2.Text); //
                TableCtl.RErrorL2 = double.Parse(textBox_RERRORL.Text);
                TableCtl.RErrorR2 = double.Parse(textBox_RERROR2R.Text);
                //修改20180415
                //TableCtl.RErrorSN = double.Parse(textBox_RERRORSN.Text)/2;
                TableCtl.RErrorSN = double.Parse(textBox_RERRORSN.Text);
                TableCtl.RDefPos = double.Parse(textBox_RDELPOS.Text);
                ImgProcess.PixelScale = double.Parse(textBox_PixlScale.Text);

                TableCtl.TScale = int.Parse(textBox_Tin_Scale.Text);
                TableCtl.TScale2 = int.Parse(textBox_Tin_Scale2.Text);
                TableCtl.TScale3 = int.Parse(textBox_Tin_Scale3.Text);
                TableCtl.TScale4 = int.Parse(textBox_Tin_Scale4.Text);
                TableCtl.TAcc = double.Parse(textBox_Tin_Acc.Text);
                TableCtl.TVel = double.Parse(textBox_Tin_Vel.Text);
                TableCtl.TDistance = double.Parse(textBox_Tin_Len.Text);

                ImgProcess.CleaningSwingDist_JK = double.Parse(tbCleaningSwingDist_JK.Text);
                ImgProcess.CleaningTime_JK = int.Parse(tbCleaningTime_JK.Text);

                TableCtl.SetParam();

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("运动控制卡初始化失败！");
            }

        }

        private void button_TABLE_YD_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefY(double.Parse(textBox_XY_REFMOVE.Text));
        }

        private void button_TABLE_YN_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefY(-double.Parse(textBox_XY_REFMOVE.Text));
        }

        private void button_TABLE_XD_Click(object sender, EventArgs e)
        {

            TableCtl.MoveRefX(double.Parse(textBox_XY_REFMOVE.Text));
        }

        private void button_TABLE_XN_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefX(-double.Parse(textBox_XY_REFMOVE.Text));
        }

        private void button_TABLE_RD_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefR(double.Parse(textBox_R_REFMOVE.Text));
        }

        private void button_TABLE_RN_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefR(-double.Parse(textBox_R_REFMOVE.Text));
        }

        private void checkBox_Air_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_WeldType.Checked)
            {

                if (checkBox_Air.Checked)
                {
                    TableCtl.WeldAir(1);
                    checkBox_Air.BackColor = Color.LightCoral;
                }
                else
                {
                    TableCtl.WeldAir(0);
                    checkBox_Air.BackColor = Color.LightGreen;
                }
            }
            else
            {
                if (checkBox_Air.Checked)
                {
                    TableCtl.WeldMotorDown();
                    checkBox_Air.BackColor = Color.LightCoral;
                }
                else
                {
                    TableCtl.MoveRefR(10);
                    while (TableCtl.InPosXYR() == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    TableCtl.WeldMotorUp();
                    checkBox_Air.BackColor = Color.LightGreen;
                }
            }
        }

        private void checkBox_Home_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Home.Checked)
            {
                TableCtl.Home();
            }
        }

        private void button_SET_DEFAULTPOS_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (TableCtl.GetPosX(ref value) == 0)
            {
                textBox_XDEFPOS.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosY(ref value) == 0)
            {
                textBox_YDELPOS.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosR(ref value) == 0)
            {
                //textBox_RDELPOS.Text = value.ToString("0.000");
            }
        }

        private void button_siml_Click(object sender, EventArgs e)
        {
            ModuleInPosTrigger = true;
        }

        private void button_WeldCheck_Click(object sender, EventArgs e)
        {
            ImgProcess.holeGrayLeft1 = Convert.ToInt32(textBox_HoleGAdjust.Text);
            ImgProcess.holeGrayLeft2 = Convert.ToInt32(textBox_HoleGAdjust2.Text);
            ImgProcess.holeGrayLeft3 = Convert.ToInt32(textBox_HoleGAdjust3.Text);
            ImgProcess.holeGrayLeft4 = Convert.ToInt32(textBox_HoleGAdjust4.Text);

            ImgProcess.holeHeightLeft = Convert.ToInt32(textBox_HoleHAdjust.Text);  //加锡长度
            ImgProcess.holeWidthLeft = Convert.ToInt32(textBox_HoleWAdjust.Text);   //补锡长度
            ImgProcess.weldGrayRight1 = Convert.ToDouble(textBox_weldGAdjust.Text);
            ImgProcess.weldGrayRight2 = Convert.ToInt32(textBox_weldGAdjust2.Text);
            ImgProcess.weldGrayRight3 = Convert.ToInt32(textBox_weldGAdjust3.Text);
            ImgProcess.weldGrayRight4 = Convert.ToInt32(textBox_weldGAdjust4.Text);
            ImgProcess.weldWidthRight = Convert.ToInt32(textBox_weldWAdjust.Text);  //焊接完成吹气延时时间（冷却）
            ImgProcess.weldHeightRight = Convert.ToInt32(textBox_weldHAdjust.Text); //
            ImgProcess.weldLimitPercent = Convert.ToInt32(textBox_WiedLimit.Text);
            ImgProcess.HoleAreaPercent = Convert.ToInt32(textBox_HoleArea.Text);    //
            //    ImgProcess.HoleAreaPercent = Convert.ToInt32(textBox_HoleArea.Text);
            ImgProcess.holeAreaSel = Convert.ToInt32(textBox_HoleSelArea.Text);     //初始送锡长度
            ImgProcess.holeWidthSel = Convert.ToInt32(textBox_HoleSelW.Text);       //下压焊接时间
            ImgProcess.weldSmallSelLength = Convert.ToDouble(textBox_weldSelLength.Text);   //涂膜焊锡半径X
            ImgProcess.weldSmallSelWidth = Convert.ToDouble(textBox_weldSelWidth.Text);     //涂膜焊锡半径Y
            ImgProcess.weldLengthPercent = Convert.ToDouble(textBox_weldLengthPercent.Text);//涂膜焊锡前、抬起的距离
            ImgProcess.weldBackGray = Convert.ToInt32(textBox_weldBackGray.Text);
            ImgProcess.weldBackPercent = Convert.ToInt32(textBox_weldBackPercent.Text);
            //
            ImgProcess.TPLAngleLimit = Convert.ToDouble(textBox_TPLAngleLimit.Text);
            ImgProcess.TPLXLimit = Convert.ToInt32(textBox_TPLXLimit.Text);
            ImgProcess.TPLYLimit = Convert.ToInt32(textBox_TPLYLimit.Text);

            //添加第三种焊接模式（英利填孔）20180316 
            //模式选择
            if (JK_MOD1.Checked)
            {
                ImgProcess.star_soldering_mode = 1;
            }
            else if (JK_MOD2.Checked)
            {
                ImgProcess.star_soldering_mode = 2;
            }
            else
            {
                ImgProcess.star_soldering_mode = 3;
            }

            //晶科模式2参数
            ImgProcess.time_add_soldering = Convert.ToInt32(textBox_time_add_soldering.Text);
            ImgProcess.star_add_soldering_L = Convert.ToDouble(textBox_star_add_soldering_L.Text);
            ImgProcess.time_fornt_pushing = Convert.ToInt32(textBox_time_fornt_pushing.Text);
            //新增20180424
            ImgProcess.time_pushing_soldering_jk1 = Convert.ToInt32(tb_time_pushing_soldering_jk1.Text);
            ImgProcess.before_compensation_soldering_jk = Convert.ToInt32(tb_before_compensation_soldering_jk.Text);
            ImgProcess.time_pushing_soldering_jk2 = Convert.ToInt32(tb_time_pushing_soldering_jk2.Text);

            ImgProcess.time_pushing_soldering = Convert.ToInt32(textBox_time_pushing_soldering.Text);
            ImgProcess.before_compensation_soldering = Convert.ToDouble(textBox_before_compensation_soldering.Text);
            ImgProcess.compensation_soldering_L = Convert.ToDouble(textBox_compensation_soldering_L.Text);
            ImgProcess.before_time_paint = Convert.ToInt32(textBox_before_time_paint.Text);
            ImgProcess.paint_X_displacement_distance = Convert.ToDouble(textBox_paint_X_displacement_distance.Text);
            ImgProcess.paint_speed = Convert.ToInt32(textBox_paint_speed.Text);
            ImgProcess.paint_time = Convert.ToInt32(textBox_paint_time.Text);
            ImgProcess.paint_number = Convert.ToInt32(textBox_paint_number.Text);
            ImgProcess.paint_Y_displacement_distance = Convert.ToDouble(textBox_paint_Y_displacement_distance.Text);
            ImgProcess.accomplish_lift_distance = Convert.ToDouble(textBox_accomplish_lift_distance.Text);
            ImgProcess.accomplish_lift_dist = Convert.ToDouble(tb_accomplish_lift_distance.Text);
            ImgProcess.time_accomplish_soldering = Convert.ToInt32(textBox_time_accomplish_soldering.Text);
            //
            //应用英利焊接方案3动作设置
            #region //应用英利焊接方案3动作设置 金帅20180317
            ImgProcess.time_add_soldering_YL = Convert.ToInt32(textBox_time_add_soldering_YL.Text);
            ImgProcess.star_add_soldering_L_YL = Convert.ToDouble(textBox_star_add_soldering_L_YL.Text);

            ImgProcess.time_fornt_pushing_YL = Convert.ToInt32(textBox_time_fornt_pushing_YL.Text);
            ImgProcess.time_pushing_soldering_YL = Convert.ToInt32(textBox_time_pushing_soldering_YL.Text);
            ImgProcess.before_compensation_soldering_YL = Convert.ToDouble(textBox_before_compensation_soldering_YL.Text);
            ImgProcess.compensation_soldering_L_YL = Convert.ToDouble(textBox_compensation_soldering_L_YL.Text);
            ImgProcess.before_time_paint_YL = Convert.ToInt32(textBox_before_time_paint_YL.Text);
            ImgProcess.paint_X_displacement_distance_YL = Convert.ToDouble(textBox_paint_X_displacement_distance_YL.Text);
            ImgProcess.paint_speed_YL = Convert.ToInt32(textBox_paint_speed_YL.Text);
            ImgProcess.paint_time_YL = Convert.ToInt32(textBox_paint_time_YL.Text);
            ImgProcess.paint_number_YL = Convert.ToInt32(textBox_paint_number_YL.Text);
            ImgProcess.paint_Y_displacement_distance_YL = Convert.ToDouble(textBox_paint_Y_displacement_distance_YL.Text);
            ////////////////////////////////////////////////////////////
            ImgProcess.compensation_soldering_L_YL2 = Convert.ToDouble(textBox_compensation_soldering_L_YL2.Text);
            ImgProcess.before_time_paint_YL2 = Convert.ToInt32(textBox_before_time_paint_YL2.Text);
            ImgProcess.paint_X_displacement_distance_YL2 = Convert.ToDouble(textBox_paint_X_displacement_distance_YL2.Text);
            ImgProcess.paint_speed_YL2 = Convert.ToInt32(textBox_paint_speed_YL2.Text);
            ImgProcess.paint_time_YL2 = Convert.ToInt32(textBox_paint_time_YL2.Text);
            ImgProcess.paint_number_YL2 = Convert.ToInt32(textBox_paint_number_YL2.Text);


            ////////////////////////////////////////////////////////////
            ImgProcess.accomplish_lift_distance_YL = Convert.ToDouble(textBox_accomplish_lift_distance_YL.Text);
            ImgProcess.time_accomplish_soldering_YL = Convert.ToInt32(textBox_time_accomplish_soldering_YL.Text);

            #endregion
            //
            //晶科模式2：增加往左或往右压针模式 Z
            ImgProcess._cB_paint_X_displacement_distance_ = cB_paint_X_displacement_distance.Checked;
            //

        }

        private void label_NG_Click(object sender, EventArgs e)
        {

            //IOCtl.WriteDOPort(0, UnitDefine.DO_WeldDown, false);
            //IOCtl.WriteDOPort(0, UnitDefine.DO_Red_Light, false);

            FXPlc2IO.SetDO(UnitDefine.DO_str_alert, false);

        }

        private void hWindowControl1_HMouseMove(object sender, HMouseEventArgs e)
        {
            try
            {
                int x, y, b;
                hWindowControl1.HalconWindow.GetMposition(out y, out x, out b);
                toolStripStatusLabel_HWMouse.Text = "X:" + x.ToString() + " Y:" + y.ToString();
            }
            catch (Exception ee)
            { }

        }

        private void button_test_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {



                HObject Left_Image = null;
                HOperatorSet.ReadImage(out Left_Image, openFileDialog1.FileName);

                HOperatorSet.DispImage(Left_Image, hWindowControl1.HalconWindow);

                ImgProcess.LeftAction2(ImgProcess.IDLeft1, 1, Left_Image);
                //ImgProcess.Find_ModuleSN(Left_Image);

                创建目录(目录);

                if (ImgProcess.ShowResult(目录, true))
                {

                    ADT_text_flag = "OK";
                    TableCtl.Log("OK");
                    SetLabel();
                }
                else
                {

                    ADT_text_flag = "NG";
                    TableCtl.Log("NG");
                    SetLabel();
                }
            }

        }
        TplCreate hTplCrt = new TplCreate();
        private void button_OpenTplImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HObject Left_Image = null;
                HOperatorSet.ReadImage(out Left_Image, openFileDialog1.FileName);
                HOperatorSet.DispObj(Left_Image, hWindowControl1.HalconWindow);
                hTplCrt.SrcImage = Left_Image;
                //拷贝图像用于识别
                HOperatorSet.CopyObj(Left_Image, out ImgProcess.zm1_Positive_Image, 1, 1);
                HOperatorSet.DispObj(ImgProcess.zm1_Positive_Image, hWindowControl6.HalconWindow);
                //     hTplCrt.HTplWindow = hWindowControl1;
            }
        }

        private void button_CreateTpl_Click(object sender, EventArgs e)
        {
            hTplCrt.bUse = true;
            hTplCrt.CreateRect(500, 500, (int)numericUpDown_Width.Value, (int)numericUpDown_Height.Value);

        }

        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            hTplCrt.CreateRect((int)e.X, (int)e.Y, (int)numericUpDown_Width.Value, (int)numericUpDown_Height.Value);
        }

        private void button_SaveTpl_Click(object sender, EventArgs e)
        {
            hTplCrt.SaveTplImage(_模板目录);
        }
        //正面定位与识别拍照
        private void button_SnapTpl_Click(object sender, EventArgs e)
        {
            ImgProcess.SnapPosImage();
            HOperatorSet.DispImage(ImgProcess.Pos_Image, hWindowControl1.HalconWindow);//定位窗口
            HOperatorSet.DispImage(ImgProcess.Pos_Image, hWindowControl6.HalconWindow);//识别窗口
            hTplCrt.SrcImage = ImgProcess.Pos_Image;            //赋值定位图像
            ImgProcess.zm1_Positive_Image = ImgProcess.Pos_Image;//赋值识别图像
            hTplCrt.HTplWindow = hWindowControl1;
        }
        //
        private void button_TplTest_Click(object sender, EventArgs e)
        {
            try
            {
                HObject Tpl_Image = null;
                HOperatorSet.ReadImage(out Tpl_Image, _模板目录 + "\\Tpl.jpg");
                HTuple hv_TPLID = new HTuple();
                ImgProcess.TPLXDef = hTplCrt.CurX;
                ImgProcess.TPLYDef = hTplCrt.CurY;
                ImgProcess.PosCreateTPL(Tpl_Image, ref hv_TPLID);
                ImgProcess.PosActionTPL(hv_TPLID, hTplCrt.SrcImage);
            }
            catch (System.Exception ex)
            {

            }
        }
        bool bDebug_SimWildCpt = false;
        private void button_SimWildCpt_Click(object sender, EventArgs e)
        {
            bDebug_SimWildCpt = true;
        }
        bool bWeldDebug = false;
        private void checkBox_WeldDebug_CheckedChanged(object sender, EventArgs e)
        {
            bWeldDebug = checkBox_WeldDebug.Checked;
        }

        private void button_Set_Word1_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (TableCtl.GetPosX(ref value) == 0)
            {
                textBox_XERROR.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosY(ref value) == 0)
            {
                textBox_YERROR.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosR(ref value) == 0)
            {
                //textBox_RDELPOS.Text = value.ToString("0.000");
            }
        }

        private void button_Set_Word2_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (TableCtl.GetPosX(ref value) == 0)
            {
                textBox_XERRORL.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosY(ref value) == 0)
            {
                textBox_YERRORL.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosR(ref value) == 0)
            {
                //textBox_RDELPOS.Text = value.ToString("0.000");
            }
        }

        private void TabP_Table_Click(object sender, EventArgs e)
        {

        }

        private void button_TinTrg_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefT(TableCtl.TDistance);
        }

        private void button_WeldClear_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (TableCtl.GetPosX(ref value) == 0)
            {
                textBox_XERRORSN.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosY(ref value) == 0)
            {
                textBox_YERRORSN.Text = value.ToString("0.000");
            }
            if (TableCtl.GetPosR(ref value) == 0)
            {
                //textBox_RDELPOS.Text = value.ToString("0.000");
            }
        }

        private void button_TinInit_Click(object sender, EventArgs e)
        {
            TableCtl.MoveRefT(ImgProcess.holeAreaSel);
        }
        FXPlcComm.Plc2IO FXPlc2IO = new FXPlcComm.Plc2IO();
        private void button_IO_Click(object sender, EventArgs e)
        {

        }

        private void button_Plc_Force_Click(object sender, EventArgs e)
        {
            bool val = false;
            if (comboBox_PlcDO_Val.SelectedIndex == 1)
            {
                val = true;
            }
            FXPlc2IO.SetDO(comboBox_PlcDO_Addr.SelectedIndex, val);
        }
        private void RefreshIOLight()
        {
            for (int i = 0; i < 11; i++)
            {
                // listView_IO.Items[i].ImageIndex = FXPlc2IO.GetDI(i);
                if (read_plc_[i] > 0)
                {
                    listView_IO.Items[i].ImageIndex = 1;
                }
                else
                {
                    listView_IO.Items[i].ImageIndex = 0;
                }

            }

        }
        //监控IO状态
        private void timer_IO_Tick(object sender, EventArgs e)
        {
            RefreshIOLight();
        }

        private void button_Home_Click(object sender, EventArgs e)
        {

        }
        #endregion
        //
        #region 保存焊接前识别设置
        //保存焊接前识别设置
        public void Saver_Location_recognition_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 焊接前识别
            XmlNode Qnode13 = xmlDoc.CreateNode(XmlNodeType.Element, "Q_Location_recognition", null);

            CreateNode(xmlDoc, Qnode13, "_Q_row", ImgProcess.Q_row.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_column", ImgProcess.Q_column.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_HJ_spacing", ImgProcess.Q_HJ_spacing.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_GR_W", ImgProcess.Q_GR_W.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_GR_H", ImgProcess.Q_GR_H.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_threshold", ImgProcess.Q_threshold.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_area", ImgProcess.Q_area.ToString());
            CreateNode(xmlDoc, Qnode13, "_Q_high", ImgProcess.Q_high.ToString());

            CreateNode(xmlDoc, Qnode13, "_ZM1_saver_image", ImgProcess._ZM1_saver_image.ToString());
            CreateNode(xmlDoc, Qnode13, "_CM1_saver_image", ImgProcess._CM1_saver_image.ToString());
            CreateNode(xmlDoc, Qnode13, "_ZM2_saver_image", ImgProcess._ZM2_saver_image.ToString());
            CreateNode(xmlDoc, Qnode13, "_CBZM2_saver_image", ImgProcess._CBZM2_saver_image.ToString());
            CreateNode(xmlDoc, Qnode13, "_CM2_saver_image", ImgProcess._CM2_saver_image.ToString());
            CreateNode(xmlDoc, Qnode13, "_SN_saver_image", ImgProcess._SN_saver_image.ToString());

            CreateNode(xmlDoc, Qnode13, "_Q_star_Q_lr_checkbox", ImgProcess.Q_star_Q_lr_checkbox.ToString());


            root.AppendChild(Qnode13);

            #endregion


            try
            {
                xmlDoc.Save(filename);
                MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("保存设置文件错误！\n" + e.Message);
            }
        }
        #endregion

        #region 读取焊接前识别设置
        //读取焊接前识别设置
        public void Read_Location_recognition_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                #region 焊接前识别

                XmlNode Qroot13 = xmlDoc.SelectSingleNode("//Q_Location_recognition");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (Qroot13 != null)
                {
                    //CreateNode(xmlDoc, Qnode13, "_Q_row", ImgProcess.Q_row.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_column", ImgProcess.Q_column.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_HJ_spacing", ImgProcess.Q_HJ_spacing.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_GR_W", ImgProcess.Q_GR_W.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_GR_H", ImgProcess.Q_GR_H.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_threshold", ImgProcess.Q_threshold.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_area", ImgProcess.Q_area.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_Q_high", ImgProcess.Q_high.ToString());

                    num_Q_row.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_row").InnerText);
                    num_Q_column.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_column").InnerText);
                    num_Q_HJ_spacing.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_HJ_spacing").InnerText);
                    num_Q_GR_W.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_GR_W").InnerText);
                    num_Q_GR_H.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_GR_H").InnerText);
                    num_Q_threshold.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_threshold").InnerText);
                    num_Q_area.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_area").InnerText);
                    num_Q_high.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_high").InnerText);

                    //if (ZM1_saver_image.Checked) ImgProcess._ZM1_saver_image = 1;
                    ////else ImgProcess._ZM1_saver_image = -1;
                    ////if (CM1_saver_image.Checked) ImgProcess._CM1_saver_image = 1;
                    ////else ImgProcess._CM1_saver_image = -1;
                    ////if (ZM2_saver_image.Checked) ImgProcess._ZM2_saver_image = 1;
                    ////else ImgProcess._ZM2_saver_image = -1;
                    ////if (CM2_saver_image.Checked) ImgProcess._CM2_saver_image = 1;
                    ////else ImgProcess._CM2_saver_image = -1;
                    ////if (SN_saver_Image.Checked) ImgProcess._SN_saver_image = 1;
                    ////else ImgProcess._SN_saver_image = -1;
                    //CreateNode(xmlDoc, Qnode13, "_ZM1_saver_image", ImgProcess._ZM1_saver_image.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_CM1_saver_image", ImgProcess._CM1_saver_image.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_ZM2_saver_image", ImgProcess._ZM2_saver_image.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_CM2_saver_image", ImgProcess._CM2_saver_image.ToString());
                    //CreateNode(xmlDoc, Qnode13, "_SN_saver_image", ImgProcess._SN_saver_image.ToString());
                    ImgProcess._ZM1_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_ZM1_saver_image").InnerText);
                    ImgProcess._CM1_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_CM1_saver_image").InnerText);
                    ImgProcess._ZM2_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_ZM2_saver_image").InnerText);
                    ImgProcess._CBZM2_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_CBZM2_saver_image").InnerText);
                    ImgProcess._CM2_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_CM2_saver_image").InnerText);
                    ImgProcess._SN_saver_image = Convert.ToInt32(Qroot13.SelectSingleNode("_SN_saver_image").InnerText);
                    if (ImgProcess._ZM1_saver_image > 0) ZM1_saver_image.Checked = true;
                    else ZM1_saver_image.Checked = false;
                    if (ImgProcess._CM1_saver_image > 0) CM1_saver_image.Checked = true;
                    else CM1_saver_image.Checked = false;
                    if (ImgProcess._ZM2_saver_image > 0) ZM2_saver_image.Checked = true;
                    else ZM2_saver_image.Checked = false;
                    if (ImgProcess._CBZM2_saver_image > 0) cbZM2_saver_image.Checked = true;
                    else cbZM2_saver_image.Checked = false;

                    if (ImgProcess._CM2_saver_image > 0) CM2_saver_image.Checked = true;
                    else CM2_saver_image.Checked = false;
                    if (ImgProcess._SN_saver_image > 0) SN_saver_Image.Checked = true;
                    else SN_saver_Image.Checked = false;
                    //
                    //CreateNode(xmlDoc, Qnode13, "_Q_star_Q_lr_checkbox", ImgProcess.Q_star_Q_lr_checkbox.ToString());
                    ImgProcess.Q_star_Q_lr_checkbox = Convert.ToInt32(Qroot13.SelectSingleNode("_Q_star_Q_lr_checkbox").InnerText);
                    if (ImgProcess.Q_star_Q_lr_checkbox > 0) star_Q_lr_checkbox.Checked = true;
                    else star_Q_lr_checkbox.Checked = false;


                }
                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取焊接前识别配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        //
        #region 保存焊接前正面识别设置
        //保存焊接前识别设置
        public void Saver_zm1_Location_recognition_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 焊接前识别
            XmlNode Qnode13 = xmlDoc.CreateNode(XmlNodeType.Element, "zm1_Location_recognition", null);

            CreateNode(xmlDoc, Qnode13, "_zm1_row", ImgProcess.zm1_row.ToString());
            CreateNode(xmlDoc, Qnode13, "_zm1_column", ImgProcess.zm1_column.ToString());
            CreateNode(xmlDoc, Qnode13, "_zm1_HJ_spacing", ImgProcess.zm1_HJ_spacing.ToString());
            CreateNode(xmlDoc, Qnode13, "_zm1_GR_W", ImgProcess.zm1_GR_W.ToString());
            CreateNode(xmlDoc, Qnode13, "_zm1_GR_H", ImgProcess.zm1_GR_H.ToString());

            CreateNode(xmlDoc, Qnode13, "_WeldWhiteH", ImgProcess.hv_WeldWhiteH.ToString());
            CreateNode(xmlDoc, Qnode13, "_WeldBalckH", ImgProcess.hv_WeldBalckH.ToString());
            CreateNode(xmlDoc, Qnode13, "_WeldWhiteL", ImgProcess.hv_WeldWhiteL.ToString());
            //
            CreateNode(xmlDoc, Qnode13, "_Z2_star_zm1_soldering", ImgProcess.Z2_star_zm1_soldering.ToString());
            CreateNode(xmlDoc, Qnode13, "_Z2_star_zm1_solderingL", ImgProcess.Z2_star_zm1_solderingL.ToString());

            root.AppendChild(Qnode13);
            #endregion
            try
            {
                xmlDoc.Save(filename);
                MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("保存设置文件错误！\n" + e.Message);
            }
        }
        #endregion
        #region 读取焊接前正面识别设置
        //读取焊接前识别设置
        public void Read_zm1_Location_recognition_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                #region 焊接前识别

                XmlNode Qroot13 = xmlDoc.SelectSingleNode("//zm1_Location_recognition");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (Qroot13 != null)
                {
                    num_zm1_row.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_zm1_row").InnerText);
                    num_zm1_column.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_zm1_column").InnerText);
                    num_zm1_HJ_spacing.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_zm1_HJ_spacing").InnerText);
                    num_zm1_GR_W.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_zm1_GR_W").InnerText);
                    num_zm1_GR_H.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_zm1_GR_H").InnerText);
                    num_zm1_BackH.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_WeldBalckH").InnerText);
                    num_zm1_WhiteH.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_WeldWhiteH").InnerText);
                    num_zm1_WhiteL.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_WeldWhiteL").InnerText);
                    ImgProcess.Z2_star_zm1_soldering = Convert.ToInt32(Qroot13.SelectSingleNode("_Z2_star_zm1_soldering").InnerText);
                    if (ImgProcess.Z2_star_zm1_soldering == 1)
                    {
                        star_zm1_soldering.Checked = true;
                    }
                    else
                    {
                        star_zm1_soldering.Checked = false;
                    }
                    ImgProcess.Z2_star_zm1_solderingL = Convert.ToInt32(Qroot13.SelectSingleNode("_Z2_star_zm1_solderingL").InnerText);
                    if (ImgProcess.Z2_star_zm1_solderingL == 1)
                    {
                        star_zm1_solderingL.Checked = true;
                    }
                    else
                    {
                        star_zm1_solderingL.Checked = false;
                    }



                }
                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取焊接前识别配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        //    HObject Q_LD_Template_image = new HObject();//模板调试图像
        #region 焊前侧面和焊后条码识别

        //设置角度
        int _num_Template_phi_ = 0;
        private void num_Template_phi_ValueChanged(object sender, EventArgs e)
        {
            _num_Template_phi_ = (int)num_Template_phi.Value;
        }
        HObject Q_temp_image = null;
        //调整图像角度
        private void Q_Angle_Template_Image_Click(object sender, EventArgs e)
        {
            try
            {
                //读取图像先做角度矫正，然后拷贝到模板图像用于模板调试
                HObject Q_rotate_image = null;
                HOperatorSet.GenEmptyObj(out Q_rotate_image);
                HOperatorSet.RotateImage(Q_temp_image, out Q_rotate_image, (new HTuple(_num_Template_phi_)).TupleRad(), "constant");
                HOperatorSet.CopyObj(Q_rotate_image, out ImgProcess.Q_side_Image, 1, 1);
                HOperatorSet.DispObj(ImgProcess.Q_side_Image, hWindowControl7.HalconWindow);
                Q_rotate_image.Dispose();

            }
            catch (System.Exception ex)
            {

            }

        }
        //读取侧面模板图像
        private void Q_open_Template_Image_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    HOperatorSet.GenEmptyObj(out Q_temp_image);
                    Q_temp_image.Dispose();
                    HOperatorSet.ReadImage(out Q_temp_image, openFileDialog1.FileName);
                    //读取图像先做角度矫正，然后拷贝到模板图像用于模板调试
                    HObject Q_rotate_image = null;
                    HOperatorSet.GenEmptyObj(out Q_rotate_image);
                    HOperatorSet.RotateImage(Q_temp_image, out Q_rotate_image, (new HTuple(_num_Template_phi_)).TupleRad(), "constant");
                    HOperatorSet.CopyObj(Q_rotate_image, out ImgProcess.Q_side_Image, 1, 1);
                    HOperatorSet.DispObj(ImgProcess.Q_side_Image, hWindowControl7.HalconWindow);
                    Q_rotate_image.Dispose();
                }
                catch (System.Exception ex)
                {

                }
            }
        }
        //创建模板图像
        private void Q_start_Template_region_Click(object sender, EventArgs e)
        {
            ImgProcess.Q_tlp_region(500, 500, 300, 130);
        }
        //鼠标设置模板区域
        private void hWindowControl7_HMouseDown(object sender, HMouseEventArgs e)
        {
            ImgProcess.Q_tlp_region((int)e.Y, (int)e.X, (int)num_Template_w.Value, (int)num_Template_h.Value);
        }
        //保存模板图像
        private void saver_Template_image_Click(object sender, EventArgs e)
        {
            ImgProcess.Q_save_tlp_image();
        }
        //保存焊接前识别设置
        private void save_Q_lr_Click(object sender, EventArgs e)
        {
            Saver_Location_recognition_XmlFile("Q_Location_recognition.config");
        }
        //读取图像焊接前识别
        private void ld_Q_lr_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HObject Left_Image = null;
                HOperatorSet.ReadImage(out Left_Image, openFileDialog1.FileName);
                ImgProcess.Q_side_Image = Left_Image;
                star_Q_lr_Click(null, null);//测试焊接前识别

            }
        }
        //测试焊接前侧面识别
        private void star_Q_lr_Click(object sender, EventArgs e)
        {
            try
            {
                ImgProcess.Q_show_lr = true;    //调试显示标记位
                ImgProcess.Q_star_PWR();
                ImgProcess.Q_show_lr = false;
            }
            catch (System.Exception ex)
            {

            }
        }
        //起始位置R
        private void num_Q_row_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_row = (int)num_Q_row.Value;
        }
        //起始位置W
        private void num_Q_column_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_column = (int)num_Q_column.Value;
        }
        //每个焊接位置的间距
        private void num_Q_HJ_spacing_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_HJ_spacing = (int)num_Q_HJ_spacing.Value;
        }
        //识别大小W
        private void num_Q_GR_W_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_GR_W = (int)num_Q_GR_W.Value;
        }
        //识别大小H
        private void num_Q_GR_H_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_GR_H = (int)num_Q_GR_H.Value;
        }
        //识别阈值强度
        private void num_Q_threshold_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_threshold = (int)num_Q_threshold.Value;
        }
        //大于设定面积NG
        private void num_Q_area_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_area = (int)num_Q_area.Value;
        }
        //大于设定高度NG
        private void num_Q_high_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Q_high = (int)num_Q_high.Value;
        }
        //侧面相机拍照
        private void Q_Cam_Image_Click(object sender, EventArgs e)
        {
            ImgProcess.Q_SnapPosImage();
        }
        //打开或关闭侧面识别
        private void star_Q_lr_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Q_lr_checkbox.Checked)
                ImgProcess.Q_star_Q_lr_checkbox = 1;
            else
                ImgProcess.Q_star_Q_lr_checkbox = 0;

        }
        //图像保存设置
        private void p_Save_Image_Click(object sender, EventArgs e)
        {
            if (ZM1_saver_image.Checked) ImgProcess._ZM1_saver_image = 1;
            else ImgProcess._ZM1_saver_image = -1;
            if (CM1_saver_image.Checked) ImgProcess._CM1_saver_image = 1;
            else ImgProcess._CM1_saver_image = -1;
            if (ZM2_saver_image.Checked) ImgProcess._ZM2_saver_image = 1;
            else ImgProcess._ZM2_saver_image = -1;
            if (CM2_saver_image.Checked) ImgProcess._CM2_saver_image = 1;
            else ImgProcess._CM2_saver_image = -1;
            if (SN_saver_Image.Checked) ImgProcess._SN_saver_image = 1;
            else ImgProcess._SN_saver_image = -1;
            save_Q_lr_Click(null, null);
            if (cbZM2_saver_image.Checked) ImgProcess._CBZM2_saver_image = 1;
            else ImgProcess._CBZM2_saver_image = -1;
        }
        //
        //条码相机拍照
        private void SN_Cam_Image_Click(object sender, EventArgs e)
        {
            try
            {
                ImgProcess.SN_SnapPosImage();
            }
            catch (System.Exception ex)
            {

            }
        }
        //读取条码图像
        private void SN_open_Image_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                HObject Left_Image = null;
                HOperatorSet.ReadImage(out Left_Image, openFileDialog1.FileName);
                ImgProcess.SN_image = Left_Image;
                star_SN_Recognize_Click(null, null);
            }

        }
        //测试条码图像
        private void star_SN_Recognize_Click(object sender, EventArgs e)
        {
            ImgProcess.str_SN();
        }
        //侧面焊接后拍照
        private void CM2_Cam_Image_Click(object sender, EventArgs e)
        {
            ImgProcess.C2_SnapPos_image();
        }
        #endregion
        //
        #region 创建正面识别模板创建jin
        //打开模板图片
        private void Zbtn_OPenTemplate_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    HObject Z2_temp_image = null;
                    HOperatorSet.GenEmptyObj(out Z2_temp_image);
                    Z2_temp_image.Dispose();
                    HOperatorSet.ReadImage(out Z2_temp_image, openFileDialog1.FileName);
                    //读取图像先做角度矫正，然后拷贝到模板图像用于模板调试
                    HObject Z2_rotate_image = null;
                    HOperatorSet.GenEmptyObj(out Z2_rotate_image);
                    //
                    HTuple hv_Height = new HTuple(), hv_Width = new HTuple();
                    HOperatorSet.GetImageSize(Z2_temp_image, out hv_Width, out hv_Height);
                    ImgProcess.Star_AffineTransImage(Z2_temp_image, out Z2_rotate_image, hv_Height, hv_Width);
                    //
                    HOperatorSet.CopyObj(Z2_rotate_image, out ImgProcess.Z2_Positive_Image, 1, 1);
                    HOperatorSet.DispObj(ImgProcess.Z2_Positive_Image, hWindowControl8.HalconWindow);
                    Z2_temp_image.Dispose();
                    Z2_rotate_image.Dispose();

                }
                catch (System.Exception ex)
                {
                }
            }
        }
        //创建模板区域
        private void Zbtn_CreateTemplate_Click(object sender, EventArgs e)
        {
            ImgProcess.Z2_tlp_region(500, 500, 300, 130);
        }
        //鼠标设置模板区域
        private void hWindowControl8_HMouseDown(object sender, HMouseEventArgs e)
        {
            ImgProcess.Z2_tlp_region((int)e.Y, (int)e.X, (int)numTemplate_W.Value, (int)numTemplate_H.Value);
        }
        //正面焊接后拍照(图像采集)
        private void Zbtn_ImgAcquisition_Click(object sender, EventArgs e)
        {
            ImgProcess.Z2_SnapPos_image();
        }
        //保存模板区域
        private void Zbtn_SaveTemplate_Click(object sender, EventArgs e)
        {
            ImgProcess.Z2_save_Template_image_z();
            Zbtn_SaveSetting_Click(null, null);//保存模板坐标参数
        }
        //正面焊后检测参数保存(保存参数)
        private void Zbtn_SaveSetting_Click(object sender, EventArgs e)
        {
            Saver_Z2_Positive_XmlFile("Z2_Positive_Config.config");
        }
        //读取图像
        private void btn_ld_Z_lr_Click(object sender, EventArgs e)
        {
            try
            {
                //需修改
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    HObject Left_Image = null;
                    HOperatorSet.ReadImage(out Left_Image, openFileDialog1.FileName);
                    ImgProcess.Z2_Positive_Image = Left_Image;
                    Zbtn_Test_Click(null, null);//测试焊接后识别
                }

            }
            catch (Exception)
            {
            }
        }
        //测试
        private void Zbtn_Test_Click(object sender, EventArgs e)
        {
            try
            {
                ImgProcess.Z2_show_lr = true;    //调试显示标记位
                ImgProcess.Z2_star_PR_z(ImgProcess.Z2_Positive_Image, out ImgProcess.ho_ResultOOK, out ImgProcess.ho_ResultONG);
                ImgProcess.Z2_show_lr = false;
            }
            catch (System.Exception ex)
            {
            }
        }

        //厂家焊后正面识别模式选择
        private void cbRecognize_TextChanged(object sender, EventArgs e)
        {
            if (cbRecognize.Text == "晶科识别")
            {
                ImgProcess.Z2_CcbRecognize = 0; //厂家识别模式0
                //隐藏晶科焊后正面识别调试界面
                //晶科
                this.tabPage5.Parent = this.tabControl2;//显示
                //英利
                tabPage16.Parent = null;
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                tabPage22.Parent = null;
                cbRecognize.Text = "晶科识别";
            }
            else if (cbRecognize.Text == "协鑫识别")
            {
                ImgProcess.Z2_CcbRecognize = 1; //厂家识别模式1
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                tabPage16.Parent = null;
                //协鑫
                this.tabPage4.Parent = this.tabControl2;//显示
                //JinKO识别 金帅
                tabPage22.Parent = null;
                cbRecognize.Text = "协鑫识别";
            }
            else if (cbRecognize.Text == "英利识别")
            {
                ImgProcess.Z2_CcbRecognize = 2; //厂家识别模式2
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                this.tabPage16.Parent = this.tabControl2;//显示
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                tabPage22.Parent = null;

                cbRecognize.Text = "英利识别";
            }
            else if (cbRecognize.Text == "JinKO识别")
            {
                ImgProcess.Z2_CcbRecognize = 3; //厂家识别模式3
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                tabPage16.Parent = null;
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                this.tabPage22.Parent = this.tabControl2;//显示
                cbRecognize.Text = "JinKO识别";
            }
        }
        private void cbRecognize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRecognize.Text == "晶科识别")
            {
                ImgProcess.Z2_CcbRecognize = 0; //厂家识别模式0
                //隐藏晶科焊后正面识别调试界面
                //晶科
                this.tabPage5.Parent = this.tabControl2;//显示
                //英利
                tabPage16.Parent = null;
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                tabPage22.Parent = null;
                cbRecognize.Text = "晶科识别";
            }
            else if (cbRecognize.Text == "协鑫识别")
            {
                ImgProcess.Z2_CcbRecognize = 1; //厂家识别模式1
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                tabPage16.Parent = null;
                //协鑫
                this.tabPage4.Parent = this.tabControl2;//显示
                //JinKO识别 金帅
                tabPage22.Parent = null;
                cbRecognize.Text = "协鑫识别";
            }
            else if (cbRecognize.Text == "英利识别")
            {
                ImgProcess.Z2_CcbRecognize = 2; //厂家识别模式2
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                this.tabPage16.Parent = this.tabControl2;//显示
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                tabPage22.Parent = null;

                cbRecognize.Text = "英利识别";
            }
            else if (cbRecognize.Text == "JinKO识别")
            {
                ImgProcess.Z2_CcbRecognize = 3; //厂家识别模式3
                //隐藏晶科焊后正面识别调试界面
                //晶科
                tabPage5.Parent = null;
                //英利
                tabPage16.Parent = null;
                //协鑫
                tabPage4.Parent = null;
                //JinKO识别 金帅
                this.tabPage22.Parent = this.tabControl2;//显示
                cbRecognize.Text = "JinKO识别";
            }
        }




        #endregion
        #region 保存焊后正面识别设置
        //保存焊接后识别设置
        public void Saver_Z2_Positive_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 焊接后正面识别
            XmlNode ZM2node1 = xmlDoc.CreateNode(XmlNodeType.Element, "Z2_Positive_Config", null);
            CreateNode(xmlDoc, ZM2node1, "_Z2_CenterRow", ImgProcess.Z2_CenterRow.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_CenterCol", ImgProcess.Z2_CenterCol.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_WeldDis", ImgProcess.Z2_WeldDis.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_WeldW", ImgProcess.Z2_WeldW.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_WeldH", ImgProcess.Z2_WeldH.ToString());
            //
            //保存模板区域坐标
            CreateNode(xmlDoc, ZM2node1, "_Z2P_X", ImgProcess.Z2P_X.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2P_Y", ImgProcess.Z2P_Y.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2P_W", ImgProcess.Z2P_W.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2P_H", ImgProcess.Z2P_H.ToString());
            //
            //            //焊后正面识别参数
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_", ImgProcess.star_Z2_L_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_dyn_threshold_", ImgProcess.Z2_L_dyn_threshold_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_height_max_", ImgProcess.Z2_L_height_max_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_height_min_", ImgProcess.Z2_L_height_min_.ToString());
            ////
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L2_", ImgProcess.star_Z2_L2_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_perforation_length_", ImgProcess.Z2_L_perforation_length_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_AfewLines_dyn_threshold_", ImgProcess.Z2_L_AfewLines_dyn_threshold_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_AfewLines_height_", ImgProcess.Z2_L_AfewLines_height_.ToString());
            //show
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_AfewLines_length_", ImgProcess.Z2_L_AfewLines_length_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_height_90_", ImgProcess.Z2_L_height_90_.ToString());
            ////
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_r1area_", ImgProcess.star_Z2_r1area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r1_hrow_", ImgProcess.Z2_r1_hrow_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r1_crw_", ImgProcess.Z2_r1_crw_.ToString());
            //show
            CreateNode(xmlDoc, ZM2node1, "_Z2_r1area_", ImgProcess.Z2_r1area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r1_number_", ImgProcess.Z2_r1_number_.ToString());
            ////
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_r2area_", ImgProcess.star_Z2_r2area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r2area_", ImgProcess.Z2_r2area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r2_aHeight_", ImgProcess.Z2_r2_aHeight_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r2_awidth_", ImgProcess.Z2_r2_awidth_.ToString());
            ////
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_r3area_", ImgProcess.star_Z2_r3area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r3area_", ImgProcess.Z2_r3area_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r3_crw_min_", ImgProcess.Z2_r3_crw_min_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r3_crw_max_", ImgProcess.Z2_r3_crw_max_.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_r3_hrow_", ImgProcess.Z2_r3_hrow_.ToString());

            ////附加参数
            //public int star_Z2_L_rw_ = 1;//打开左右两条边界识别
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_rw_", ImgProcess.star_Z2_L_rw_.ToString());


            //厂家识别模式切换20180208 
            CreateNode(xmlDoc, ZM2node1, "_Z2_CcbRecognize", ImgProcess.Z2_CcbRecognize.ToString());
            //协鑫焊接正面参数
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_dyn_threshold_Xx", ImgProcess.Z2_L_dyn_threshold_Xx.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_height_max_Xx", ImgProcess.Z2_L_height_max_Xx.ToString());
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_rw_Xx", ImgProcess.star_Z2_L_rw_Xx.ToString());

            CreateNode(xmlDoc, ZM2node1, "_Z2_L_threshold_Xx", ImgProcess.Z2_L_threshold_Xx.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_Back_Area_Xx", ImgProcess.Z2_L_Back_Area_Xx.ToString());
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_rc_Xx", ImgProcess.star_Z2_L_rc_Xx.ToString());
            //CreateNode(xmlDoc, ZM2node1, "star_Z2_L_rc_Xx", ImgProcess.star_Z2_L_rw_YL.ToString());
            //英利焊点专用余量
            CreateNode(xmlDoc, ZM2node1, "_Z2_WeldDisY", ImgProcess.Z2_WeldDisY.ToString());
            CreateNode(xmlDoc, ZM2node1, "_Z2_L_dyn_threshold_YL", ImgProcess.Z2_L_dyn_threshold_YL.ToString());

            CreateNode(xmlDoc, ZM2node1, "_nudHoleMinHeight", ImgProcess.nudHoleMinHeight.ToString());
            CreateNode(xmlDoc, ZM2node1, "_nudHoleMaxHeight", ImgProcess.nudHoleMaxHeight.ToString());
            CreateNode(xmlDoc, ZM2node1, "_nudHoleMinWidth", ImgProcess.nudHoleMinWidth.ToString());
            CreateNode(xmlDoc, ZM2node1, "_nudBacknum", ImgProcess.nudBacknum.ToString());
            CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_rw_YL", ImgProcess.star_Z2_L_rw_YL.ToString());
            CreateNode(xmlDoc, ZM2node1, "_nudHoleMaxArea", ImgProcess.nudHoleMaxArea.ToString());
            //ImgProcess.WeldDate = (string)tbChangData.Text;
            CreateNode(xmlDoc, ZM2node1, "_WeldDate", ImgProcess.WeldDate.ToString());
            CreateNode(xmlDoc, ZM2node1, "_LifeTime", ImgProcess.LifeTime.ToString());




            root.AppendChild(ZM2node1);

            #endregion
            try
            {
                xmlDoc.Save(filename);
                MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("保存设置文件错误！\n" + e.Message);
            }
        }
        #endregion
        #region 读取焊后正面识别参数
        public void Read_Z2_Positive_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);
                #region 焊接后识别参数设置
                XmlNode ZM2node1 = xmlDoc.SelectSingleNode("//Z2_Positive_Config");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (ZM2node1 != null)
                {
                    nUD_CenterRow.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_CenterRow").InnerText);
                    nUD_CenterCol.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_CenterCol").InnerText);
                    nUD_WeldDis.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_WeldDis").InnerText);
                    nUD_WeldW.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_WeldW").InnerText);
                    nUD_WeldH.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_WeldH").InnerText);

                    ////保存模板区域坐标

                    ImgProcess.Z2P_X = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2P_X").InnerText);
                    ImgProcess.Z2P_Y = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2P_Y").InnerText);
                    ImgProcess.Z2P_W = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2P_W").InnerText);
                    ImgProcess.Z2P_H = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2P_H").InnerText);
                    ////            //焊后正面识别参数

                    ImgProcess.star_Z2_L_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L_").InnerText);
                    if (ImgProcess.star_Z2_L_ == 1) star_Z2_L.Checked = true;
                    else star_Z2_L.Checked = false;
                    Z2_L_dyn_threshold.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_L_dyn_threshold_").InnerText);
                    Z2_L_height_max.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_max_").InnerText);
                    Z2_L_height_min.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_min_").InnerText);                    //////

                    ImgProcess.star_Z2_L2_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L2_").InnerText);
                    if (ImgProcess.star_Z2_L2_ == 1) star_Z2_L2.Checked = true;
                    else star_Z2_L2.Checked = false;
                    Z2_L_perforation_length.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_perforation_length_").InnerText);
                    Z2_L_AfewLines_dyn_threshold.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_L_AfewLines_dyn_threshold_").InnerText);
                    Z2_L_AfewLines_height.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_AfewLines_height_").InnerText);
                    //show
                    Z2_L_AfewLines_length.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_AfewLines_length_").InnerText);
                    Z2_L_height_90.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_90_").InnerText);
                    //////

                    ImgProcess.star_Z2_r1area_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_r1area_").InnerText);
                    if (ImgProcess.star_Z2_r1area_ == 1) star_Z2_r1area.Checked = true;
                    else star_Z2_r1area.Checked = false;
                    Z2_r1_hrow.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1_hrow_").InnerText);
                    Z2_r1_crw.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1_crw_").InnerText);
                    //show
                    Z2_r1area.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1area_").InnerText);
                    Z2_r1_number.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_r1_number_").InnerText);
                    //////

                    ImgProcess.star_Z2_r2area_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_r2area_").InnerText);
                    if (ImgProcess.star_Z2_r2area_ == 1) star_Z2_r2area.Checked = true;
                    else star_Z2_r2area.Checked = false;
                    Z2_r2area.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r2area_").InnerText);
                    Z2_r2_aHeight.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r2_aHeight_").InnerText);
                    Z2_r2_awidth.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r2_awidth_").InnerText);
                    //////

                    ImgProcess.star_Z2_r3area_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_r3area_").InnerText);
                    if (ImgProcess.star_Z2_r3area_ == 1) star_Z2_r3area.Checked = true;
                    else star_Z2_r3area.Checked = false;
                    Z2_r3area.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3area_").InnerText);
                    Z2_r3_crw_min.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_crw_min_").InnerText);
                    Z2_r3_crw_max.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_crw_max_").InnerText);
                    Z2_r3_hrow.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_hrow_").InnerText);

                    //////附加参数
                    ////public int star_Z2_L_rw_ = 1;//打开左右两条边界识别
                    //CreateNode(xmlDoc, ZM2node1, "_star_Z2_L_rw_", ImgProcess.star_Z2_L_rw_.ToString());
                    ImgProcess.star_Z2_L_rw_ = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L_rw_").InnerText);
                    if (ImgProcess.star_Z2_L_rw_ == 1) star_Z2_L_rw.Checked = true;
                    else star_Z2_L_rw.Checked = false;
                    //模式切换
                    ImgProcess.Z2_CcbRecognize = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_CcbRecognize").InnerText);
                    //协鑫焊接正面参数
                    Z2_L_dyn_threshold_Xx.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_dyn_threshold_Xx").InnerText);
                    Z2_L_height_max_Xx.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_max_Xx").InnerText);
                    ImgProcess.star_Z2_L_rw_Xx = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L_rw_Xx").InnerText);
                    if (ImgProcess.star_Z2_L_rw_Xx == 1) star_Z2_L_rw_Xx.Checked = true;
                    else star_Z2_L_rw_Xx.Checked = false;
                    Z2_L_threshold_Xx.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_threshold_Xx").InnerText);
                    Z2_L_Back_Area_Xx.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_Back_Area_Xx").InnerText);
                    ImgProcess.star_Z2_L_rc_Xx = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L_rc_Xx").InnerText);
                    if (ImgProcess.star_Z2_L_rc_Xx == 1) star_Z2_L_rc_Xx.Checked = true;
                    else star_Z2_L_rc_Xx.Checked = false;
                    //英利参数
                    nUD_WeldDisY.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_WeldDisY").InnerText);
                    Z2_L_dyn_threshold_YL.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_L_dyn_threshold_YL").InnerText);
                    nudHoleMinHeight.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_nudHoleMinHeight").InnerText);
                    nudHoleMaxHeight.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_nudHoleMaxHeight").InnerText);
                    nudHoleMinWidth.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_nudHoleMinWidth").InnerText);
                    nudBacknum.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_nudBacknum").InnerText);
                    //ImgProcess.star_Z2_L_rw_YL = Convert.ToInt32(ZM2node1.SelectSingleNode("_star_Z2_L_rw_YL").InnerText);
                    //if (ImgProcess.star_Z2_L_rw_YL == 1) star_Z2_L_rw_YL.Checked = true;
                    //else star_Z2_L_rw_YL.Checked = false;
                    nudHoleMaxArea.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_nudHoleMaxArea").InnerText);
                    tbChangData.Text = Convert.ToString(ZM2node1.SelectSingleNode("_WeldDate").InnerText);
                    tbLifeTime.Text = Convert.ToString(ZM2node1.SelectSingleNode("_LifeTime").InnerText);
                    //////////////////////////////////////JinKO 金帅 20180420
                    if (ImgProcess.star_Z2_L_ == 1) star_Z2_L_JK.Checked = true;
                    else star_Z2_L_JK.Checked = false;
                    if (ImgProcess.star_Z2_L_rw_ == 1) star_Z2_L_rw_JK.Checked = true;
                    else star_Z2_L_rw_JK.Checked = false;
                    Z2_L_dyn_threshold_JK.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_L_dyn_threshold_").InnerText);
                    Z2_L_height_max_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_max_").InnerText);
                    Z2_L_height_min_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_L_height_min_").InnerText);                    //////

                    if (ImgProcess.star_Z2_r1area_ == 1) star_Z2_r1area_JK.Checked = true;
                    else star_Z2_r1area_JK.Checked = false;
                    Z2_r1_hrow_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1_hrow_").InnerText);
                    Z2_r1_crw_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1_crw_").InnerText);
                    Z2_r1area_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r1area_").InnerText);
                    Z2_r1_number_JK.Value = Convert.ToInt32(ZM2node1.SelectSingleNode("_Z2_r1_number_").InnerText);

                    if (ImgProcess.star_Z2_r3area_ == 1) star_Z2_r3area_JK.Checked = true;
                    else star_Z2_r3area_JK.Checked = false;
                    Z2_r3area_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3area_").InnerText);
                    Z2_r3_crw_min_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_crw_min_").InnerText);
                    Z2_r3_crw_max_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_crw_max_").InnerText);
                    Z2_r3_hrow_JK.Value = Convert.ToDecimal(ZM2node1.SelectSingleNode("_Z2_r3_hrow_").InnerText);



                }
                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取焊后正面识别配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        //
        private void nUD_CenterRow_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_CenterRow = (int)nUD_CenterRow.Value;
        }
        //
        private void nUD_CenterCol_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_CenterCol = (int)nUD_CenterCol.Value;
        }
        //
        private void nUD_WeldDis_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_WeldDis = (int)nUD_WeldDis.Value;
        }
        //
        private void nUD_WeldW_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_WeldW = (int)nUD_WeldW.Value;
        }
        //
        private void nUD_WeldH_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_WeldH = (int)nUD_WeldH.Value;
        }
        //
        //英利焊点间距余量
        private void nUD_WeldDisY_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_WeldDisY = (int)nUD_WeldDisY.Value;
        }
        //
        private void Form2_Resize(object sender, EventArgs e)
        {
            //刷新下窗体
            tabControl1.SelectedTab = tabControl1.TabPages[5];
            tabControl1.SelectedTab = tabControl1.TabPages[4];
            tabControl1.SelectedTab = tabControl1.TabPages[3];
            tabControl1.SelectedTab = tabControl1.TabPages[2];
            tabControl1.SelectedTab = tabControl1.TabPages[1];
            tabControl1.SelectedTab = tabControl1.TabPages[0];

        }
        //显示正面第二次识别调试区域
        private void nUD_show_rectangle_CheckedChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_show_rectangle_z = nUD_show_rectangle.Checked;
        }
        //-------------------------------------
        #region 晶科：保存焊接方案2动作设置
        //保存焊接前识别设置
        public void Saver_hq_zm1_recognition_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 焊接前识别
            XmlNode hqzm1_node = xmlDoc.CreateNode(XmlNodeType.Element, "hqzm1_recognition", null);

            CreateNode(xmlDoc, hqzm1_node, "_star_soldering_mode", ImgProcess.star_soldering_mode.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_add_soldering", ImgProcess.time_add_soldering.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_star_add_soldering_L", ImgProcess.star_add_soldering_L.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_fornt_pushing", ImgProcess.time_fornt_pushing.ToString());


            CreateNode(xmlDoc, hqzm1_node, "_time_pushing_soldering", ImgProcess.time_pushing_soldering.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_compensation_soldering", ImgProcess.before_compensation_soldering.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_compensation_soldering_L", ImgProcess.compensation_soldering_L.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_time_paint", ImgProcess.before_time_paint.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_X_displacement_distance", ImgProcess.paint_X_displacement_distance.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_speed", ImgProcess.paint_speed.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_time", ImgProcess.paint_time.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_number", ImgProcess.paint_number.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_Y_displacement_distance", ImgProcess.paint_Y_displacement_distance.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_accomplish_lift_distance", ImgProcess.accomplish_lift_distance.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_accomplish_lift_dist", ImgProcess.accomplish_lift_dist.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_accomplish_soldering", ImgProcess.time_accomplish_soldering.ToString());
            //保存英利焊接方案3动作设置
            #region //保存英利焊接方案3动作设置 金帅20180317
            CreateNode(xmlDoc, hqzm1_node, "_time_add_soldering_YL", ImgProcess.time_add_soldering_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_star_add_soldering_L_YL", ImgProcess.star_add_soldering_L_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_fornt_pushing_YL", ImgProcess.time_fornt_pushing_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_pushing_soldering_YL", ImgProcess.time_pushing_soldering_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_compensation_soldering_YL", ImgProcess.before_compensation_soldering_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_compensation_soldering_L_YL", ImgProcess.compensation_soldering_L_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_time_paint_YL", ImgProcess.before_time_paint_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_X_displacement_distance_YL", ImgProcess.paint_X_displacement_distance_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_speed_YL", ImgProcess.paint_speed_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_time_YL", ImgProcess.paint_time_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_number_YL", ImgProcess.paint_number_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_Y_displacement_distance_YL", ImgProcess.paint_Y_displacement_distance_YL.ToString());
            //////////////////////////////////////////////////////
            CreateNode(xmlDoc, hqzm1_node, "_compensation_soldering_L_YL2", ImgProcess.compensation_soldering_L_YL2.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_time_paint_YL2", ImgProcess.before_time_paint_YL2.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_X_displacement_distance_YL2", ImgProcess.paint_X_displacement_distance_YL2.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_speed_YL2", ImgProcess.paint_speed_YL2.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_time_YL2", ImgProcess.paint_time_YL2.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_paint_number_YL2", ImgProcess.paint_number_YL2.ToString());
            //////////////////////////////////////////////////////////////
            CreateNode(xmlDoc, hqzm1_node, "_accomplish_lift_distance_YL", ImgProcess.accomplish_lift_distance_YL.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_accomplish_soldering_YL", ImgProcess.time_accomplish_soldering_YL.ToString());
            #endregion
            //
            //晶科模式2：增加往左或往右压针模式
            //public bool _cB_paint_X_displacement_distance_ = false;//改为X轴移动距离  //Z:20180418
            CreateNode(xmlDoc, hqzm1_node, "__cB_paint_X_displacement_distance_", ImgProcess._cB_paint_X_displacement_distance_.ToString());
            //
            //新增20180424
            CreateNode(xmlDoc, hqzm1_node, "_time_pushing_soldering_jk1", ImgProcess.time_pushing_soldering_jk1.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_before_compensation_soldering_jk", ImgProcess.before_compensation_soldering_jk.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_time_pushing_soldering_jk2", ImgProcess.time_pushing_soldering_jk2.ToString());
            //应唐勇飞要求新增清洗动作 20180515
            CreateNode(xmlDoc, hqzm1_node, "_CleaningSwingDist_JK", ImgProcess.CleaningSwingDist_JK.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_CleaningTime_JK", ImgProcess.CleaningTime_JK.ToString());
            CreateNode(xmlDoc, hqzm1_node, "_RunClearing_JK", ImgProcess.RunClearing_JK.ToString());


            root.AppendChild(hqzm1_node);

            #endregion


            try
            {
                xmlDoc.Save(filename);
                //         MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("晶科模式2保存设置失败！\n" + e.Message);
            }
        }
        #endregion
        #region 晶科：读取焊接方案2动作设置
        //读取焊接前识别设置
        public void Read_hq_zm1_recognition_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                #region 焊接前识别

                XmlNode hqzm1_root = xmlDoc.SelectSingleNode("//hqzm1_recognition");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (hqzm1_root != null)
                {

                    ImgProcess.star_soldering_mode = Convert.ToInt32(hqzm1_root.SelectSingleNode("_star_soldering_mode").InnerText);
                    ImgProcess.time_add_soldering = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_add_soldering").InnerText);
                    ImgProcess.star_add_soldering_L = Convert.ToDouble(hqzm1_root.SelectSingleNode("_star_add_soldering_L").InnerText);
                    ImgProcess.time_fornt_pushing = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_fornt_pushing").InnerText);

                    ImgProcess.time_pushing_soldering = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_pushing_soldering").InnerText);
                    ImgProcess.before_compensation_soldering = Convert.ToDouble(hqzm1_root.SelectSingleNode("_before_compensation_soldering").InnerText);
                    ImgProcess.compensation_soldering_L = Convert.ToDouble(hqzm1_root.SelectSingleNode("_compensation_soldering_L").InnerText);
                    ImgProcess.before_time_paint = Convert.ToInt32(hqzm1_root.SelectSingleNode("_before_time_paint").InnerText);
                    ImgProcess.paint_X_displacement_distance = Convert.ToDouble(hqzm1_root.SelectSingleNode("_paint_X_displacement_distance").InnerText);
                    ImgProcess.paint_speed = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_speed").InnerText);
                    ImgProcess.paint_time = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_time").InnerText);
                    ImgProcess.paint_number = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_number").InnerText);
                    ImgProcess.paint_Y_displacement_distance = Convert.ToDouble(hqzm1_root.SelectSingleNode("_paint_Y_displacement_distance").InnerText);
                    ImgProcess.accomplish_lift_distance = Convert.ToDouble(hqzm1_root.SelectSingleNode("_accomplish_lift_distance").InnerText);
                    ImgProcess.accomplish_lift_dist = Convert.ToDouble(hqzm1_root.SelectSingleNode("_accomplish_lift_dist").InnerText);
                    ImgProcess.time_accomplish_soldering = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_accomplish_soldering").InnerText);

                    //增加英利填孔模式 20180316
                    //模式选择
                    if (ImgProcess.star_soldering_mode == 1)
                    {
                        JK_MOD1.Checked = true;
                        JK_MOD2.Checked = false;
                        JK_MOD3.Checked = false;
                    }
                    else if (ImgProcess.star_soldering_mode == 2)
                    {
                        JK_MOD1.Checked = false;
                        JK_MOD2.Checked = true;
                        JK_MOD3.Checked = false;
                    }
                    else if (ImgProcess.star_soldering_mode == 3)
                    {
                        JK_MOD1.Checked = false;
                        JK_MOD2.Checked = false;
                        JK_MOD3.Checked = true;
                    }
                    textBox_time_add_soldering.Text = ImgProcess.time_add_soldering.ToString();
                    textBox_star_add_soldering_L.Text = ImgProcess.star_add_soldering_L.ToString();
                    textBox_time_fornt_pushing.Text = ImgProcess.time_fornt_pushing.ToString();


                    textBox_time_pushing_soldering.Text = ImgProcess.time_pushing_soldering.ToString();
                    textBox_before_compensation_soldering.Text = ImgProcess.before_compensation_soldering.ToString();
                    textBox_compensation_soldering_L.Text = ImgProcess.compensation_soldering_L.ToString();
                    textBox_before_time_paint.Text = ImgProcess.before_time_paint.ToString();
                    textBox_paint_X_displacement_distance.Text = ImgProcess.paint_X_displacement_distance.ToString();
                    textBox_paint_speed.Text = ImgProcess.paint_speed.ToString();
                    textBox_paint_time.Text = ImgProcess.paint_time.ToString();
                    textBox_paint_number.Text = ImgProcess.paint_number.ToString();
                    textBox_paint_Y_displacement_distance.Text = ImgProcess.paint_Y_displacement_distance.ToString();
                    textBox_accomplish_lift_distance.Text = ImgProcess.accomplish_lift_distance.ToString();
                    tb_accomplish_lift_distance.Text = ImgProcess.accomplish_lift_dist.ToString();
                    textBox_time_accomplish_soldering.Text = ImgProcess.time_accomplish_soldering.ToString();
                    //
                    //读取英利焊接方案3动作设置
                    #region //读取英利焊接方案3动作设置 金帅20180317
                    ImgProcess.time_add_soldering_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_add_soldering_YL").InnerText);
                    ImgProcess.star_add_soldering_L_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_star_add_soldering_L_YL").InnerText);
                    ImgProcess.time_fornt_pushing_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_fornt_pushing_YL").InnerText);
                    ImgProcess.time_pushing_soldering_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_pushing_soldering_YL").InnerText);
                    ImgProcess.before_compensation_soldering_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_before_compensation_soldering_YL").InnerText);
                    ImgProcess.compensation_soldering_L_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_compensation_soldering_L_YL").InnerText);
                    ImgProcess.before_time_paint_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_before_time_paint_YL").InnerText);
                    ImgProcess.paint_X_displacement_distance_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_paint_X_displacement_distance_YL").InnerText);
                    ImgProcess.paint_speed_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_speed_YL").InnerText);
                    ImgProcess.paint_time_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_time_YL").InnerText);
                    ImgProcess.paint_number_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_number_YL").InnerText);
                    ImgProcess.paint_Y_displacement_distance_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_paint_Y_displacement_distance_YL").InnerText);
                    ImgProcess.accomplish_lift_distance_YL = Convert.ToDouble(hqzm1_root.SelectSingleNode("_accomplish_lift_distance_YL").InnerText);
                    ImgProcess.time_accomplish_soldering_YL = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_accomplish_soldering_YL").InnerText);
                    //////////////////////////////////////////////////////////////////////////
                    ImgProcess.compensation_soldering_L_YL2 = Convert.ToDouble(hqzm1_root.SelectSingleNode("_compensation_soldering_L_YL2").InnerText);
                    ImgProcess.before_time_paint_YL2 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_before_time_paint_YL2").InnerText);
                    ImgProcess.paint_X_displacement_distance_YL2 = Convert.ToDouble(hqzm1_root.SelectSingleNode("_paint_X_displacement_distance_YL2").InnerText);
                    ImgProcess.paint_speed_YL2 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_speed_YL2").InnerText);
                    ImgProcess.paint_time_YL2 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_time_YL2").InnerText);
                    ImgProcess.paint_number_YL2 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_paint_number_YL2").InnerText);
                    //////////////////////////////////////////////////////////////////////////
                    textBox_time_add_soldering_YL.Text = ImgProcess.time_add_soldering_YL.ToString();
                    textBox_star_add_soldering_L_YL.Text = ImgProcess.star_add_soldering_L_YL.ToString();
                    textBox_time_fornt_pushing_YL.Text = ImgProcess.time_fornt_pushing_YL.ToString();
                    textBox_time_pushing_soldering_YL.Text = ImgProcess.time_pushing_soldering_YL.ToString();
                    textBox_before_compensation_soldering_YL.Text = ImgProcess.before_compensation_soldering_YL.ToString();
                    textBox_compensation_soldering_L_YL.Text = ImgProcess.compensation_soldering_L_YL.ToString();
                    textBox_before_time_paint_YL.Text = ImgProcess.before_time_paint_YL.ToString();
                    textBox_paint_X_displacement_distance_YL.Text = ImgProcess.paint_X_displacement_distance_YL.ToString();
                    textBox_paint_speed_YL.Text = ImgProcess.paint_speed_YL.ToString();
                    textBox_paint_time_YL.Text = ImgProcess.paint_time_YL.ToString();
                    textBox_paint_number_YL.Text = ImgProcess.paint_number_YL.ToString();
                    textBox_paint_Y_displacement_distance_YL.Text = ImgProcess.paint_Y_displacement_distance_YL.ToString();
                    ////////////////////////////////////////////
                    textBox_compensation_soldering_L_YL2.Text = ImgProcess.compensation_soldering_L_YL2.ToString();
                    textBox_before_time_paint_YL2.Text = ImgProcess.before_time_paint_YL2.ToString();
                    textBox_paint_X_displacement_distance_YL2.Text = ImgProcess.paint_X_displacement_distance_YL2.ToString();
                    textBox_paint_speed_YL2.Text = ImgProcess.paint_speed_YL2.ToString();
                    textBox_paint_time_YL2.Text = ImgProcess.paint_time_YL2.ToString();
                    textBox_paint_number_YL2.Text = ImgProcess.paint_number_YL2.ToString();
                    ////////////////////////////////////////////////
                    textBox_accomplish_lift_distance_YL.Text = ImgProcess.accomplish_lift_distance_YL.ToString();
                    textBox_time_accomplish_soldering_YL.Text = ImgProcess.time_accomplish_soldering_YL.ToString();
                    #endregion
                    ////晶科模式2：增加往左或往右压针模式
                    ////public bool _cB_paint_X_displacement_distance_ = false;//改为X轴移动距离  //Z:20180418
                    //CreateNode(xmlDoc, hqzm1_node, "__cB_paint_X_displacement_distance_", ImgProcess._cB_paint_X_displacement_distance_.ToString());
                    ////
                    string _str_paint_x_dd = hqzm1_root.SelectSingleNode("__cB_paint_X_displacement_distance_").InnerText;
                    if (_str_paint_x_dd == "True")
                    {
                        ImgProcess._cB_paint_X_displacement_distance_ = true;
                        cB_paint_X_displacement_distance.Checked = ImgProcess._cB_paint_X_displacement_distance_;
                    }
                    else
                    {
                        ImgProcess._cB_paint_X_displacement_distance_ = false;
                        cB_paint_X_displacement_distance.Checked = ImgProcess._cB_paint_X_displacement_distance_;
                    }
                    //新增20180424
                    ImgProcess.time_pushing_soldering_jk1 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_pushing_soldering_jk1").InnerText);
                    ImgProcess.before_compensation_soldering_jk = Convert.ToInt32(hqzm1_root.SelectSingleNode("_before_compensation_soldering_jk").InnerText);
                    ImgProcess.time_pushing_soldering_jk2 = Convert.ToInt32(hqzm1_root.SelectSingleNode("_time_pushing_soldering_jk2").InnerText);
                    //新增20180424
                    tb_time_pushing_soldering_jk1.Text = ImgProcess.time_pushing_soldering_jk1.ToString();
                    tb_before_compensation_soldering_jk.Text = ImgProcess.before_compensation_soldering_jk.ToString();
                    tb_time_pushing_soldering_jk2.Text = ImgProcess.time_pushing_soldering_jk2.ToString();
                    //应唐勇飞要求新增清洗动作 20180515
                    ImgProcess.CleaningSwingDist_JK = Convert.ToInt32(hqzm1_root.SelectSingleNode("_CleaningSwingDist_JK").InnerText);
                    ImgProcess.CleaningTime_JK = Convert.ToInt32(hqzm1_root.SelectSingleNode("_CleaningTime_JK").InnerText);
                    tbCleaningSwingDist_JK.Text = ImgProcess.CleaningSwingDist_JK.ToString();
                    tbCleaningTime_JK.Text = ImgProcess.CleaningTime_JK.ToString();
                    ImgProcess.RunClearing_JK = Convert.ToInt32(hqzm1_root.SelectSingleNode("_RunClearing_JK").InnerText);
                    if (ImgProcess.RunClearing_JK == 1)
                    {
                        cbRunClear.Checked = true;
                    }
                    else
                    {
                        cbRunClear.Checked = false;
                    }


                }
                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取晶科模式2配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        //-------------------------------------
        //模式2参数应用（直接调用模式1参数应用）
        private void STAR_JK_MOD2_Click(object sender, EventArgs e)
        {
            button_WeldCheck_Click(null, null);
        }
        //模式3参数应用（直接调用模式2参数应用）
        private void STAR_JK_MOD2_YL_Click(object sender, EventArgs e)
        {
            STAR_JK_MOD2_Click(null, null);
            button_WeldCheck_Click(null, null);
            colour_difference_save_Click(null, null);
        }

        //清晰参数应用（直接调用模式1参数应用）
        private void button_STAR_timeQX_Click(object sender, EventArgs e)
        {
            button_WeldCheck_Click(null, null);
        }
        //调用总保存设置
        private void Save_WeldCheck_Click(object sender, EventArgs e)
        {
            //20180508 注销 目的：减少一个设置保存对话框（可能有问题）
            //colour_difference_save_Click(null, null);
            button_WeldCheck_Click(null, null);
            STAR_JK_MOD2_YL_Click(null, null);
            STAR_JK_MOD2_Click(null, null);
        }
        //-----------------------焊前正面识别---------------------
        #region 焊前正面识别
        //正面焊前识别拍照
        private void zm1_button_soldering_before_cam_Click(object sender, EventArgs e)
        {
            button_SnapTpl_Click(null, null);//正面拍照
        }
        //打开模板图片
        private void zm1_open_soldering_before_Click(object sender, EventArgs e)
        {
            button_OpenTplImage_Click(null, null);
        }
        //保存模板图像
        private void zm1_saver_mod_soldering_before_Click(object sender, EventArgs e)
        {
            ImgProcess.zm1_save_Template_img_j();
        }
        //识别测试
        private void zm1_test_soldering_before_Click(object sender, EventArgs e)
        {
            try
            {
                ImgProcess.zm1_soldering_Recognize(ImgProcess.zm1_Positive_Image);
            }
            catch (System.Exception ex)
            {
            }
        }
        //保存设置
        private void zm1_saver_soldering_config_Click(object sender, EventArgs e)
        {
            //保存正面焊接前设置参数
            Saver_zm1_Location_recognition_XmlFile("zm1_Location_recognition.config");
        }
        //创建模板区域
        private void zm1_mod_soldering_before_Click(object sender, EventArgs e)
        {
            ImgProcess.zm1_tlp_region(500, 500, 300, 130);
        }
        //获取窗口鼠标坐标
        private void hWindowControl6_HMouseDown(object sender, HMouseEventArgs e)
        {
            ImgProcess.zm1_tlp_region((int)e.Y, (int)e.X, (int)numTemplate_FW.Value, (int)numTemplate_FH.Value);
        }
        //打开或关闭焊前正面识别
        private void star_zm1_soldering_CheckedChanged(object sender, EventArgs e)
        {
            if (star_zm1_soldering.Checked)
                ImgProcess.Z2_star_zm1_soldering = 1;
            else
                ImgProcess.Z2_star_zm1_soldering = 0;
        }
        #endregion
        #region //正面焊前参数设置20180108
        //设置焊点起始位置坐标x
        private void num_zm1_column_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.zm1_column = (int)num_zm1_column.Value;
        }
        //设置焊点起始位置坐标y
        private void num_zm1_row_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.zm1_row = (int)num_zm1_row.Value;
        }
        //设置焊点间距
        private void num_zm1_HJ_spacing_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.zm1_HJ_spacing = (int)num_zm1_HJ_spacing.Value;
        }
        //设置焊点的宽度
        private void num_zm1_GR_W_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.zm1_GR_W = (int)num_zm1_GR_W.Value;
        }
        //设置焊点的高度
        private void num_zm1_GR_H_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.zm1_GR_H = (int)num_zm1_GR_H.Value;
        }
        //黑色汇流条最高点距离矩形中心的距离
        private void num_zm1_BackH_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.hv_WeldBalckH = (int)num_zm1_BackH.Value;
        }
        //白色汇流条最高点距离矩形中心的距离
        private void num_zm1_WhiteH_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.hv_WeldWhiteH = (int)num_zm1_WhiteH.Value;
        }

        //最大长度检测
        private void num_zm1_WhiteL_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.hv_WeldWhiteL = (int)num_zm1_WhiteL.Value;

        }
        #endregion
        //
        #region 界面文本刷新
        #region 保存检测数据
        //保存焊接前识别设置
        public void Saver_ER_number_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 焊接前识别
            XmlNode ER_number_node = xmlDoc.CreateNode(XmlNodeType.Element, "ER_number", null);

            //public UInt32 Total_OK_number = 0;          //OK    组件
            //public UInt32 Total_NG_number = 0;          //NG    组件
            //public UInt32 HQ_CM1_ERnumber = 0;          //焊前侧面NG数量
            //public UInt32 HQ_ZM1_ERnumber = 0;          //焊前正面NG数量
            //public UInt32 HH_ZM2_ERnumber = 0;          //焊后正面NG数量
            //public UInt32 EROOR_Location_Number = 0;    //定位失败数量
            //public UInt32 ERROR_SNnumber = 0;           //条码识别失败数量
            CreateNode(xmlDoc, ER_number_node, "_Total_OK_number", ImgProcess.Total_OK_number.ToString());
            CreateNode(xmlDoc, ER_number_node, "_Total_NG_number", ImgProcess.Total_NG_number.ToString());
            CreateNode(xmlDoc, ER_number_node, "_HQ_CM1_ERnumber", ImgProcess.HQ_CM1_ERnumber.ToString());
            CreateNode(xmlDoc, ER_number_node, "_HQ_ZM1_ERnumber", ImgProcess.HQ_ZM1_ERnumber.ToString());
            CreateNode(xmlDoc, ER_number_node, "_HH_ZM2_ERnumber", ImgProcess.HH_ZM2_ERnumber.ToString());
            CreateNode(xmlDoc, ER_number_node, "_EROOR_Location_Number", ImgProcess.EROOR_Location_Number.ToString());
            CreateNode(xmlDoc, ER_number_node, "_ERROR_SNnumber", ImgProcess.ERROR_SNnumber.ToString());

            root.AppendChild(ER_number_node);

            #endregion

            try
            {
                xmlDoc.Save(filename);
                //         MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //      //显示错误信息  
                //      MessageBox.Show("晶科模式2保存设置失败！\n" + e.Message);
            }
        }
        #endregion
        #region 读取检测数据
        //读取焊接前识别设置
        public void Read_ER_number_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                #region 焊接前识别

                XmlNode ER_number_root = xmlDoc.SelectSingleNode("//ER_number");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (ER_number_root != null)
                {
                    ////public UInt32 Total_OK_number = 0;          //OK    组件
                    ////public UInt32 Total_NG_number = 0;          //NG    组件
                    ////public UInt32 HQ_CM1_ERnumber = 0;          //焊前侧面NG数量
                    ////public UInt32 HQ_ZM1_ERnumber = 0;          //焊前正面NG数量
                    ////public UInt32 HH_ZM2_ERnumber = 0;          //焊后正面NG数量
                    ////public UInt32 EROOR_Location_Number = 0;    //定位失败数量
                    ////public UInt32 ERROR_SNnumber = 0;           //条码识别失败数量
                    //CreateNode(xmlDoc, ER_number_node, "_Total_OK_number", ImgProcess.Total_OK_number.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_Total_NG_number", ImgProcess.Total_NG_number.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_HQ_CM1_ERnumber", ImgProcess.HQ_CM1_ERnumber.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_HQ_ZM1_ERnumber", ImgProcess.HQ_ZM1_ERnumber.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_HH_ZM2_ERnumber", ImgProcess.HH_ZM2_ERnumber.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_EROOR_Location_Number", ImgProcess.EROOR_Location_Number.ToString());
                    //CreateNode(xmlDoc, ER_number_node, "_ERROR_SNnumber", ImgProcess.ERROR_SNnumber.ToString());
                    ImgProcess.Total_OK_number = Convert.ToUInt32(ER_number_root.SelectSingleNode("_Total_OK_number").InnerText);
                    ImgProcess.Total_NG_number = Convert.ToUInt32(ER_number_root.SelectSingleNode("_Total_NG_number").InnerText);
                    ImgProcess.HQ_CM1_ERnumber = Convert.ToUInt32(ER_number_root.SelectSingleNode("_HQ_CM1_ERnumber").InnerText);
                    ImgProcess.HQ_ZM1_ERnumber = Convert.ToUInt32(ER_number_root.SelectSingleNode("_HQ_ZM1_ERnumber").InnerText);
                    ImgProcess.HH_ZM2_ERnumber = Convert.ToUInt32(ER_number_root.SelectSingleNode("_HH_ZM2_ERnumber").InnerText);
                    ImgProcess.EROOR_Location_Number = Convert.ToUInt32(ER_number_root.SelectSingleNode("_EROOR_Location_Number").InnerText);
                    ImgProcess.ERROR_SNnumber = Convert.ToUInt32(ER_number_root.SelectSingleNode("_ERROR_SNnumber").InnerText);
                    updata_Total_ERnumber();//刷新界面数据

                }
                #endregion
            }
            catch (Exception e)
            {
                Saver_ER_number_XmlFile("data_record.config");
                //      //显示错误信息  
                //      MessageBox.Show("读取晶科模式2配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        private void updata_Total_ERnumber()
        {
            updata_CrossDelegate updata_Total_ERnumber = new updata_CrossDelegate(UPdata_Total_number);
            BeginInvoke(updata_Total_ERnumber); // 异步调用委托,调用后立即返回并立即执行下面的语句
        }
        //委托
        public delegate void updata_CrossDelegate();
        //    bool star_Number_OKNG = false;//允许刷新：整个焊接流程中有一个检测NG时，总数OK和NG只刷新一次
        bool star_Temp_OKNG = true;//OKNG标记
        
        public int TotelNum=0;
        private void UPdata_Total_number()
        {
            //try
            //{
                if (star_Temp_OKNG)//
                {
                    ImgProcess.Total_OK_number++;
                }
                else
                {
                    ImgProcess.Total_NG_number++;
                }
            //}
            //catch (Exception)
            //{ }
            //finally
            //{
                ////新增20180705
                //TotelNum++;
                //if (TotelNum==ImgProcess.time_pushing_soldering_jk1)
                //{
                //    ImgProcess.ClearNum = 1;
                //    TotelNum = 0;
                //}
                //else
                //{
                //    ImgProcess.ClearNum = 0;
                //}
                //if (TotelNum>ImgProcess.time_pushing_soldering_jk1)
                //{
                //    TotelNum = 0;
                //}
            //}


           


            double m_OK_Percentage = Math.Round((ImgProcess.Total_OK_number * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);
            double m_NG_Percentage = Math.Round((ImgProcess.Total_NG_number * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);
            //焊前侧面
            double m_NG_Percentage_QC = Math.Round((ImgProcess.HQ_CM1_ERnumber * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);
            //焊前正面
            double m_NG_Percentage_QZ = Math.Round((ImgProcess.HQ_ZM1_ERnumber * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);
            //定位失败
            double m_NG_Percentage_D = Math.Round((ImgProcess.EROOR_Location_Number * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);
            //条码识别失败
            double m_NG_Percentage_SN = Math.Round((ImgProcess.ERROR_SNnumber * 1.0)
                        / ((ImgProcess.Total_OK_number + ImgProcess.Total_NG_number) * 1.0) * 100.0, 2);

            if (ImgProcess.Total_OK_number == 0)
            {
                m_OK_Percentage = 0;
            }
            if (ImgProcess.Total_NG_number == 0)
            {
                m_NG_Percentage = 0;
            }
            label_OK.Text = "焊接质量 OK：" + ImgProcess.Total_OK_number + "   " + m_OK_Percentage.ToString() + "%";
            label_NG.Text = "焊接质量 NG：" + ImgProcess.Total_NG_number + "   " + m_NG_Percentage.ToString() + "%";
            //焊头使用寿命计数
            tbLifeLength.Text = (ImgProcess.Total_OK_number + ImgProcess.Total_NG_number).ToString();

            label_Display_data.Text = "焊前侧面NG：" + ImgProcess.HQ_CM1_ERnumber + "   " + m_NG_Percentage_QC.ToString() + "%" + "\n"
                                    + "焊前正面NG：" + ImgProcess.HQ_ZM1_ERnumber + "   " + m_NG_Percentage_QZ.ToString() + "%" + "\n"
                                    + "焊后正面NG：" + ImgProcess.HH_ZM2_ERnumber + "   " + m_NG_Percentage.ToString() + "%" + "\n"
                                    + "定位失败：" + ImgProcess.EROOR_Location_Number + "   " + m_NG_Percentage_D.ToString() + "%" + "\n"
                                    + "条码识别失败：" + ImgProcess.ERROR_SNnumber + "   " + m_NG_Percentage_SN.ToString() + "%" + "\n";
       
            Saver_ER_number_XmlFile("data_record.config");//保存数据
        }
        #endregion

        private void button_Scavenging_data_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认清除检测数据？", "！", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            if (dr == DialogResult.OK)
            {
                ImgProcess.Total_OK_number = 0;         //OK    组件
                ImgProcess.Total_NG_number = 0;         //NG    组件
                ImgProcess.HQ_CM1_ERnumber = 0;         //焊前侧面NG数量
                ImgProcess.HQ_ZM1_ERnumber = 0;         //焊前正面NG数量
                ImgProcess.HH_ZM2_ERnumber = 0;         //焊后正面NG数量
                ImgProcess.EROOR_Location_Number = 0;   //定位失败数量
                ImgProcess.ERROR_SNnumber = 0;          //条码识别失败数量
                UPdata_Total_number();//刷新数据
            }
        }
        #region 保存条码识别设置
        //保存焊接前识别设置
        public void Saver_SN_XmlFile(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Users");
            xmlDoc.AppendChild(root);
            #region 条码识别
            XmlNode SNnode13 = xmlDoc.CreateNode(XmlNodeType.Element, "SNxml", null);
            CreateNode(xmlDoc, SNnode13, "_SN_star_SN_checkbox", ImgProcess.SN_star_SN_checkbox.ToString());

            CreateNode(xmlDoc, SNnode13, "_CamSetNum", ImgProcess.CamSetNum.ToString());
            if (ImgProcess.CamSetNum == 1)
            {
                rbCamNum1.Checked = true;
                rbCamNum2.Checked = false;
                rbCamNum3.Checked = false;
                rbCamNum4.Checked = false;
            }
            else if (ImgProcess.CamSetNum == 2)
            {
                rbCamNum1.Checked = false;
                rbCamNum2.Checked = true;
                rbCamNum3.Checked = false;
                rbCamNum4.Checked = false;
            }
            else if (ImgProcess.CamSetNum == 3)
            {
                rbCamNum1.Checked = false;
                rbCamNum2.Checked = false;
                rbCamNum3.Checked = true;
                rbCamNum4.Checked = false;
            }
            else if (ImgProcess.CamSetNum == 4)
            {
                rbCamNum1.Checked = false;
                rbCamNum2.Checked = false;
                rbCamNum3.Checked = false;
                rbCamNum4.Checked = true;
            }
            else
            {
                rbCamNum1.Checked = true;
                rbCamNum2.Checked = false;
                rbCamNum3.Checked = false;
                rbCamNum4.Checked = false;
            }
            CreateNode(xmlDoc, SNnode13, "_SNCamIdx", ImgProcess.SNCamIdx.ToString());
            CreateNode(xmlDoc, SNnode13, "_LF2CamIdx", ImgProcess.LF2CamIdx.ToString());






            root.AppendChild(SNnode13);
            #endregion
            try
            {
                xmlDoc.Save(filename);
                MessageBox.Show("保存设置文件成功");
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("保存设置文件错误！\n" + e.Message);
            }
        }
        #endregion
        #region 读取条码识别设置
        //读取焊接前识别设置
        public void Read_SN_XmlNode(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filename);

                #region 焊接前识别

                XmlNode Qroot13 = xmlDoc.SelectSingleNode("//SNxml");//当节点Workflow带有属性时，使用SelectSingleNode无法读取          
                if (Qroot13 != null)
                {
                    ImgProcess.SN_star_SN_checkbox = Convert.ToInt32(Qroot13.SelectSingleNode("_SN_star_SN_checkbox").InnerText);
                    if (ImgProcess.SN_star_SN_checkbox == 1)
                    {
                        star_SN_checkbox.Checked = true;
                    }
                    else
                    {
                        star_SN_checkbox.Checked = false;
                    }

                    ImgProcess.CamSetNum = Convert.ToInt32(Qroot13.SelectSingleNode("_CamSetNum").InnerText);
                    if (ImgProcess.CamSetNum == 1)
                    {
                        rbCamNum1.Checked = true;
                        rbCamNum2.Checked = false;
                        rbCamNum3.Checked = false;
                        rbCamNum4.Checked = false;
                    }
                    else if (ImgProcess.CamSetNum == 2)
                    {
                        rbCamNum1.Checked = false;
                        rbCamNum2.Checked = true;
                        rbCamNum3.Checked = false;
                        rbCamNum4.Checked = false;
                    }
                    else if (ImgProcess.CamSetNum == 3)
                    {
                        rbCamNum1.Checked = false;
                        rbCamNum2.Checked = false;
                        rbCamNum3.Checked = true;
                        rbCamNum4.Checked = false;
                    }
                    else if (ImgProcess.CamSetNum == 4)
                    {
                        rbCamNum1.Checked = false;
                        rbCamNum2.Checked = false;
                        rbCamNum3.Checked = false;
                        rbCamNum4.Checked = true;
                    }
                    else
                    {
                        rbCamNum1.Checked = true;
                        rbCamNum2.Checked = false;
                        rbCamNum3.Checked = false;
                        rbCamNum4.Checked = false;
                    }
                    ImgProcess.SNCamIdx = Convert.ToInt32(Qroot13.SelectSingleNode("_SNCamIdx").InnerText);
                    ImgProcess.LF2CamIdx = Convert.ToInt32(Qroot13.SelectSingleNode("_LF2CamIdx").InnerText);

                    nudCamIndexC.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_LF2CamIdx").InnerText);
                    nudCamIndexSN.Value = Convert.ToInt32(Qroot13.SelectSingleNode("_SNCamIdx").InnerText);

                }
                #endregion
            }
            catch (Exception e)
            {
                //显示错误信息  
                MessageBox.Show("读取条码识别配置文件失败！\n" + e.Message);
            }
        }
        #endregion
        //保存条码识别设置
        private void saver_star_SN_Click(object sender, EventArgs e)
        {
            Saver_SN_XmlFile("sn_config.config");
        }
        //打开或关闭条码识别
        private void star_SN_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (star_SN_checkbox.Checked)
                ImgProcess.SN_star_SN_checkbox = 1;
            else
                ImgProcess.SN_star_SN_checkbox = 0;

        }
        //
        //----------------焊后识别参数设置----------------
        //打开或关闭最大边界识别
        private void star_Z2_L_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L.Checked) ImgProcess.star_Z2_L_ = 1;
            else ImgProcess.star_Z2_L_ = 0;
        }
        #region 焊后正面识别控制

        //边界识别阈值
        private void Z2_L_dyn_threshold_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_dyn_threshold_ = (int)Z2_L_dyn_threshold.Value;
        }
        //边界最大允许长度
        private void Z2_L_height_max_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_max_ = (double)Z2_L_height_max.Value;
        }
        //两条边界允许长度
        private void Z2_L_height_min_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_min_ = (double)Z2_L_height_min.Value;
        }
        //打开或关闭90度边界识别
        private void star_Z2_L2_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L2.Checked) ImgProcess.star_Z2_L2_ = 1;
            else ImgProcess.star_Z2_L2_ = 0;
        }
        //90度上边界阈值
        private void Z2_L_AfewLines_dyn_threshold_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_AfewLines_dyn_threshold_ = (int)Z2_L_AfewLines_dyn_threshold.Value;
        }
        //90度上边界区域获取位置
        private void Z2_L_AfewLines_height_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_AfewLines_height_ = (double)Z2_L_AfewLines_height.Value;
        }
        //调试显示90度上边界区域获取位置
        private void show_Z2_L_AfewLines_height_CheckedChanged(object sender, EventArgs e)
        {
            ImgProcess.show_Z2_L_AfewLines_height_ = show_Z2_L_AfewLines_height.Checked;
        }
        //链接穿孔处边界的长度
        private void Z2_L_perforation_length_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_perforation_length_ = (double)Z2_L_perforation_length.Value;
        }
        //行区域的获取长度
        private void Z2_L_AfewLines_length_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_AfewLines_length_ = (double)Z2_L_AfewLines_length.Value;
        }
        //90度边界缺陷高度
        private void Z2_L_height_90_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_90_ = (double)Z2_L_height_90.Value;
        }
        //打开或关闭左右焊接区域识别
        private void star_Z2_r1area_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_r1area.Checked) ImgProcess.star_Z2_r1area_ = 1;
            else ImgProcess.star_Z2_r1area_ = 0;
        }
        //左右识别高度
        private void Z2_r1_hrow_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_hrow_ = (double)Z2_r1_hrow.Value;
        }
        //调试显示左右识别区域位置
        private void show_Z2_r1_hrow_crw_CheckedChanged(object sender, EventArgs e)
        {
            ImgProcess.show_Z2_r1_hrow_crw_ = show_Z2_r1_hrow_crw.Checked;
        }
        //左右识别宽度
        private void Z2_r1_crw_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_crw_ = (double)Z2_r1_crw.Value;
        }
        //左右区域的最小占比面积
        private void Z2_r1area_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1area_ = (double)Z2_r1area.Value;
        }
        //左右区域的缺陷数量
        private void Z2_r1_number_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_number_ = (int)Z2_r1_number.Value;
        }
        //打开或关闭焊接均匀度识别
        private void star_Z2_r2area_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_r2area.Checked) ImgProcess.star_Z2_r2area_ = 1;
            else ImgProcess.star_Z2_r2area_ = 0;
        }
        //两个区域面积大小相似度
        private void Z2_r2area_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r2area_ = (double)Z2_r2area.Value;
        }
        //均匀度、高
        private void Z2_r2_aHeight_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r2_aHeight_ = (double)Z2_r2_aHeight.Value;
        }
        //均匀度、宽
        private void Z2_r2_awidth_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r2_awidth_ = (double)Z2_r2_awidth.Value;
        }
        //打开或关闭包裹度识别
        private void star_Z2_r3area_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_r3area.Checked) ImgProcess.star_Z2_r3area_ = 1;
            else ImgProcess.star_Z2_r3area_ = 0;
        }
        //包裹度：面积
        private void Z2_r3area_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3area_ = (double)Z2_r3area.Value;
        }
        //包裹度：宽度范围min
        private void Z2_r3_crw_min_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_crw_min_ = (double)Z2_r3_crw_min.Value;
        }
        //包裹度：宽度范围max
        private void Z2_r3_crw_max_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_crw_max_ = (double)Z2_r3_crw_max.Value;
        }
        //包裹度：高度
        private void Z2_r3_hrow_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_hrow_ = (double)Z2_r3_hrow.Value;
        }
        #endregion
        //关闭报警
        bool _checkbox_alert = true;
        private void checkbox_alert_CheckedChanged(object sender, EventArgs e)
        {
            _checkbox_alert = checkbox_alert.Checked;
        }
        //报警测试
        private void button_DO_str_alert_Click(object sender, EventArgs e)
        {
            if (_checkbox_alert)
            {
                //FXPlc2IO.SetDO(UnitDefine.DO_str_alert, true);//报警
                res = FXPlc2IO.setM("M37", true);
            }
        }
        //急停模拟测试
        private void button_alert_Handle_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process _Process = new System.Diagnostics.Process();
            _Process.StartInfo.FileName = "Alert_handle_from.exe";
            _Process.StartInfo.WorkingDirectory = "Alert_handle_from.exe";
            _Process.StartInfo.CreateNoWindow = true;
            _Process.Start();

            //暂停
            JHCap.CameraStop(g_index);
            //释放相机
            JHCap.CameraFree(g_index);
            //释放资源关闭软件
            Environment.Exit(1);
        }
        //登陆设置
        private void 用户登陆_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox_JK_MOD1.Enabled = false;
            groupBox5.Enabled = false;
            if (comboBox1.Text == "调试")
            {
                if (textBox1.Text == "112")
                {
                    groupBox_图像保存与分选调试.Enabled = true;
                    groupBox_用户设置.Enabled = true;

                    groupBox_焊接容错率.Enabled = true;
                    tabControl1_主设置.Enabled = true;
                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                    groupBox_JK_MOD1.Enabled = true;
                    groupBox5.Enabled = true;
                    toolStripStatusLabel4.Text = "调试";
                    button_DO_str_alert.Visible = false;//显示报警测试按钮
                    button_alert_Handle.Visible = false;//显示急停模拟测试按钮

                    //用户登录权限设置 20180604
                    #region //调试界面
                    //显示信息
                    this.tabPage1.Parent = this.tabControl1_主设置;
                    //显示参数设置
                    this.tabPage7.Parent = this.tabControl1_主设置;
                    //隐藏相机设置
                    tabPage23.Parent = null;
                    //显示焊接参数
                    tabPage_rightcam.Parent = this.tabControl1_主设置;
                    //显示定位平台
                    TabP_Table.Parent = this.tabControl1_主设置;
                    //隐藏通讯设置
                    tabPage2.Parent = null;
                    //显示定位模板
                    tabPage_IO.Parent = this.tabControl1_主设置;
                    //显示焊前正面
                    tabPage17.Parent = null;
                    //隐藏焊前侧面
                    tabPage6.Parent = null;
                    //隐藏焊后正面
                    tabPage9.Parent = null;
                    //隐藏焊后侧面
                    tabPage10.Parent = null;
                    //隐藏条码设置
                    tabPage14.Parent = null;
                    #endregion

                    #region //显示窗口
                    ////显示细节窗口
                    //this.tabPage_Left.Parent = this.tabControl1;
                    ////显示焊后正面窗口
                    //this.tabPage11.Parent = this.tabControl1;
                    ////显示正面图像定位窗口
                    //tabPage3.Parent = this.tabControl1;
                    ////隐藏正面焊前识别窗口
                    //tabPage18.Parent = null;
                    ////隐藏侧面焊前识别窗口
                    //tabPage8.Parent = null;
                    ////隐藏条码识别窗口
                    //tabPage13.Parent = null;
                    ////隐藏未使用窗口
                    //tabPage12.Parent = null;
                    #endregion
                }
                else
                {
                    MessageBox.Show("密码错误");
                }
            }
            else if (comboBox1.Text == "admin")
            {
                if (textBox1.Text == "111")
                {
                    toolStripStatusLabel4.Text = "admin";
                    //相机参数.Enabled = true;
                    //相机参数.BackgroundImage = opt_Weld_identification.Properties.Resources.相机1;

                    IMAGE_directshow.Visible = true;

                    相机端口号.Visible = true;
                    label_相机端口号.Visible = true;

                    groupBox_图像保存与分选调试.Enabled = true;
                    groupBox_用户设置.Enabled = true;
                    groupBox_匹配坐标.Visible = true;
                    groupBox_特征定位.Visible = true;
                    groupBox_焊接端子定位.Visible = true;

                    groupBox_焊接容错率.Enabled = true;

                    groupBox_PLC类型.Enabled = true;
                    tabControl1_主设置.Enabled = true;
                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                    groupBox_JK_MOD1.Enabled = true;
                    groupBox5.Enabled = true;

                    button_DO_str_alert.Visible = true;//显示报警测试按钮
                    button_alert_Handle.Visible = true;//显示急停模拟测试按钮

                    //用户登录权限设置 20180604
                    #region //调试界面
                    //显示信息
                    this.tabPage1.Parent = this.tabControl1_主设置;
                    //显示参数设置
                    this.tabPage7.Parent = this.tabControl1_主设置;
                    //显示相机设置
                    tabPage23.Parent = this.tabControl1_主设置;
                    //显示焊接参数
                    tabPage_rightcam.Parent = this.tabControl1_主设置;
                    //显示定位平台
                    TabP_Table.Parent = this.tabControl1_主设置;
                    //显示通讯设置
                    tabPage2.Parent = this.tabControl1_主设置;
                    //显示定位模板
                    tabPage_IO.Parent = this.tabControl1_主设置;
                    //显示焊前正面
                    tabPage17.Parent = this.tabControl1_主设置;
                    //显示焊前侧面
                    tabPage6.Parent = this.tabControl1_主设置;
                    //显示焊后正面
                    tabPage9.Parent = this.tabControl1_主设置;
                    //显示焊后侧面
                    tabPage10.Parent = this.tabControl1_主设置;
                    //显示条码设置
                    tabPage14.Parent = this.tabControl1_主设置;
                    #endregion

                    #region //显示窗口
                    ////显示细节窗口
                    //this.tabPage_Left.Parent = this.tabControl1;
                    ////显示焊后正面窗口
                    //this.tabPage11.Parent = this.tabControl1;
                    ////显示正面图像定位窗口
                    //tabPage3.Parent = this.tabControl1;
                    ////显示正面焊前识别窗口
                    //tabPage18.Parent = this.tabControl1;
                    ////显示侧面焊前识别窗口
                    //tabPage8.Parent = this.tabControl1;
                    ////显示条码识别窗口
                    //tabPage13.Parent = this.tabControl1;
                    ////显示未使用窗口
                    //tabPage12.Parent = this.tabControl1;
                    #endregion
                }
                else
                {
                    MessageBox.Show("密码错误");

                }
            }
            else
            {
                toolStripStatusLabel4.Text = "用户";
                button_DO_str_alert.Visible = false;//显示报警测试按钮
                button_alert_Handle.Visible = false;//显示急停模拟测试按钮
            }

        }
        //打开左右两条边界识别
        private void star_Z2_L_rw_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L_rw.Checked)
                ImgProcess.star_Z2_L_rw_ = 1;
            else
                ImgProcess.star_Z2_L_rw_ = 0;
        }

        //协鑫正面检测算法
        //左右双边最大高度
        private void Z2_L_height_max_Xx_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_max_Xx = (int)Z2_L_height_max_Xx.Value;

        }
        //协鑫正面检测算法
        //左右边界检测阈值
        private void Z2_L_dyn_threshold_Xx_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_dyn_threshold_Xx = (int)Z2_L_dyn_threshold_Xx.Value;
        }
        //协鑫正面检测算法
        //是否检测左右边x
        private void star_Z2_L_rw_Xx_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L_rw_Xx.Checked)
                ImgProcess.star_Z2_L_rw_Xx = 1;
            else
                ImgProcess.star_Z2_L_rw_Xx = 0;
        }
        //协鑫正面检测算法
        //手动阈值范围
        private void Z2_L_threshold_Xx_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_threshold_Xx = (int)Z2_L_threshold_Xx.Value;
        }
        //协鑫正面检测算法
        //黑车区域面积大小
        private void Z2_L_Back_Area_Xx_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_Back_Area_Xx = (int)Z2_L_Back_Area_Xx.Value;
        }
        //协鑫正面检测算法
        //是否检测黑色区域（可能虚焊）
        private void star_Z2_L_rc_Xx_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L_rc_Xx.Checked)
                ImgProcess.star_Z2_L_rc_Xx = 1;
            else
                ImgProcess.star_Z2_L_rc_Xx = 0;
        }

        //创建文件夹定时器
        private void tmrPath_Tick(object sender, EventArgs e)
        {
            int intHour = DateTime.Now.Hour;
            int intMinute = DateTime.Now.Minute;
            int intSecond = DateTime.Now.Second;

            //白班
            int iWHour = 08;
            int iWMinute = 00;
            int iWSecond = 00;
            //夜班
            int iBHour = 20;
            int iBMinute = 00;
            int iBSecond = 00;

            // 设置　每天的08:00:00创建一个白班文件夹  
            if (intHour == iWHour && intMinute == iWMinute && intSecond == iWSecond)
            {
                //对当前日期进行格式化
                string strName = DateTime.Now.ToString("yyyy年MM月dd日 白班");
                //创建DirectoryInfo对象
                //DirectoryInfo DInfo = new DirectoryInfo(maskedTextBox1.Text + strName);
                ImgProcess._Save_image_directory_ = maskedTextBox1.Text + "\\" + strName;
                //创建文件夹
                //DInfo.Create();
                ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
            }

            // 设置　每天的20:00:00创建一个夜班文件夹 
            if (intHour == iBHour && intMinute == iBMinute && intSecond == iBSecond)
            {
                //对当前日期进行格式化
                string strName = DateTime.Now.ToString("yyyy年MM月dd日 夜班");
                //创建DirectoryInfo对象
                //DirectoryInfo DInfo = new DirectoryInfo(maskedTextBox1.Text + strName);
                ImgProcess._Save_image_directory_ = maskedTextBox1.Text + "\\" + strName;
                //创建文件夹
                //DInfo.Create();
                ImgProcess.A_save_image(ImgProcess._Save_image_directory_);
            }
        }

        //英利正面算法20180326
        private void Z2_L_dyn_threshold_YL_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_dyn_threshold_YL = (int)Z2_L_dyn_threshold_YL.Value;
        }

        //孔的最小高度
        private void nudHoleMinHeight_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.nudHoleMinHeight = (int)nudHoleMinHeight.Value;
        }

        //孔的最大高度
        private void nudHoleMaxHeight_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.nudHoleMaxHeight = (int)nudHoleMaxHeight.Value;
        }

        //孔的最小宽度
        private void nudHoleMinWidth_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.nudHoleMinWidth = (int)nudHoleMinWidth.Value;
        }

        //黑色虚焊系数
        private void nudBacknum_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.nudBacknum = (double)nudBacknum.Value;
        }

        private void star_Z2_L_rw_YL_CheckedChanged(object sender, EventArgs e)
        {
            //if (star_Z2_L_rw_Xx.Checked)
            //    ImgProcess.star_Z2_L_rw_YL = 1;
            //else
            //    ImgProcess.star_Z2_L_rw_YL = 0;
            //ImgProcess.show_Z2_r1_hrow_crw_ = show_Z2_r1_hrow_crw_JK.Checked;
            ImgProcess.star_Z2_L_rw_YL = star_Z2_L_rw_YL.Checked;
        }

        //孔的最小面积
        private void nudHoleMaxArea_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.nudHoleMaxArea = (int)nudHoleMaxArea.Value;
        }

        //汇流条 最长长度功能启用选项
        private void star_zm1_solderingL_CheckedChanged(object sender, EventArgs e)
        {
            if (star_zm1_solderingL.Checked)
                ImgProcess.Z2_star_zm1_solderingL = 1;
            else
                ImgProcess.Z2_star_zm1_solderingL = 0;
        }

        //汇流条特征提取 在定位图片上是否显示的功能
        private void cbWeldShow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeldShow.Checked)
                ImgProcess.WeldShow = 1;
            else
                ImgProcess.WeldShow = 0;
        }


        //焊头寿命计数开始
        private void btnLifeCount_Click(object sender, EventArgs e)
        {
            tbLifeLength.Text = "0";
            tbChangData.Text = DateTime.Now.ToString("yyyy-MM-dd HH点");
            btnLifeCount.Enabled = false;
        }

        //焊头寿命解除报警
        private void btnDisAlarm_Click(object sender, EventArgs e)
        {
            btnLifeCount.Enabled = true;
            //MessageBox.Show("焊头寿命已尽，请及时更换！");

        }

        #region //漏焊检测
        private void star_Z2_L_JK_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L_JK.Checked) ImgProcess.star_Z2_L_ = 1;
            else ImgProcess.star_Z2_L_ = 0;
        }

        private void Z2_L_height_max_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_max_ = (double)Z2_L_height_max_JK.Value;
        }

        private void star_Z2_L_rw_JK_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_L_rw_JK.Checked)
                ImgProcess.star_Z2_L_rw_ = 1;
            else
                ImgProcess.star_Z2_L_rw_ = 0;
        }

        private void Z2_L_height_min_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_height_min_ = (double)Z2_L_height_min_JK.Value;
        }

        private void Z2_L_dyn_threshold_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_L_dyn_threshold_ = (int)Z2_L_dyn_threshold_JK.Value;
        }

        private void star_Z2_r1area_JK_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_r1area_JK.Checked) ImgProcess.star_Z2_r1area_ = 1;
            else ImgProcess.star_Z2_r1area_ = 0;
        }

        private void Z2_r1area_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1area_ = (double)Z2_r1area_JK.Value;
        }

        private void Z2_r1_number_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_number_ = (int)Z2_r1_number_JK.Value;
        }

        private void show_Z2_r1_hrow_crw_JK_CheckedChanged(object sender, EventArgs e)
        {
            ImgProcess.show_Z2_r1_hrow_crw_ = show_Z2_r1_hrow_crw_JK.Checked;
        }

        private void Z2_r1_hrow_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_hrow_ = (double)Z2_r1_hrow_JK.Value;
        }

        private void Z2_r1_crw_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r1_crw_ = (double)Z2_r1_crw_JK.Value;
        }
        #endregion

        private void star_Z2_r3area_JK_CheckedChanged(object sender, EventArgs e)
        {
            if (star_Z2_r3area_JK.Checked) ImgProcess.star_Z2_r3area_ = 1;
            else ImgProcess.star_Z2_r3area_ = 0;
        }

        private void Z2_r3area_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3area_ = (double)Z2_r3area_JK.Value;
        }

        private void Z2_r3_crw_min_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_crw_min_ = (double)Z2_r3_crw_min_JK.Value;
        }

        private void Z2_r3_crw_max_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_crw_max_ = (double)Z2_r3_crw_max_JK.Value;
        }

        private void Z2_r3_hrow_JK_ValueChanged(object sender, EventArgs e)
        {
            ImgProcess.Z2_r3_hrow_ = (double)Z2_r3_hrow_JK.Value;
        }
        //批量测试
        private void Zbtn_MoreTest_Click(object sender, EventArgs e)
        {
          //  HTuple hv_ImagePath = null, hv_ImageFiles = null, hv_Index = null;
          //  hv_ImagePath = "F:/A01 三厂五车间A区/2018年04月17日 夜班/焊后正面识别";
          //  HOperatorSet.ListFiles(hv_ImagePath + "/OK", (new HTuple("files")).TupleConcat(
          //                           "follow_links"), out hv_ImageFiles);
          //  HOperatorSet.TupleRegexpSelect(hv_ImageFiles, (new HTuple("\\.(bmp|jpg)$")).TupleConcat(
          //                              "ignore_case"), out hv_ImageFiles);
          //  for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ImageFiles.TupleLength()
          //)) - 1); hv_Index = (int)hv_Index + 1)
          //  {
          //      ho_Image.Dispose();
          //      HOperatorSet.ReadImage(out ho_Image, hv_ImageFiles.TupleSelect(hv_Index));

          //      HObject ExpTmpOutVar_0;
          //      HOperatorSet.Rgb1ToGray(ho_Image, out ho_Image);


          //      try
          //      {
          //          ImgProcess.Z2_show_lr = true;    //调试显示标记位
          //          ImgProcess.Z2_star_PR_z(ImgProcess.Z2_Positive_Image,img);
          //          ImgProcess.Z2_show_lr = false;
          //      }
          //      catch (System.Exception ex)
          //      { }
          //  }

        }

        //焊头更换日期
        private void tbChangData_TextChanged(object sender, EventArgs e)
        {
            ImgProcess.WeldDate = (string)tbChangData.Text;
        }

        private void tbLifeTime_TextChanged(object sender, EventArgs e)
        {
            ImgProcess.LifeTime = (string)tbLifeTime.Text;
        }

        private void pictureBox18_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.pictureBox18, "■ 本算法主要检测接线盒汇流条漏焊缺陷\r\n■ 最大长度：当汇流条漏焊时，只能提取到汇流条左边界或右边界时，提取到边界高度大于设定值时，焊点判NG!\r\n■ 左右两条长度：当汇流条漏焊时，能提取到汇流条左右边界，提取到的边界高度大于设定值时，焊点判NG！");

        }

        private void pictureBox19_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.pictureBox19, "■ 本算法主要检测接线盒汇流条焊接的饱满度和少锡缺陷\r\n■ 小于占比面积为NG：启动左右识别高度，会显示红色矩形框和蓝色区域，面积=蓝色区域面积 除以 红色矩形面积，面积小于设定值时，焊点判NG!\r\n■ 检测等级：设定值为2时，检测比较严格；设定值为1时，检测相对比较松！");

        }

        private void pictureBox20_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.pictureBox20, "■ 本算法主要检测接线盒汇流条虚焊缺陷\r\n■ 最小面积：中间红色区域面积大于设定值，焊点判NG!\r\n■ 宽度范围：中间红色区域宽度在设定的最小值和最大值之间时，焊点判NG!\r\n■ 最小高度：中间红色区域高度大于设定值，焊点判NG!");

        }


        private void rbCamNum1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCamNum1.Checked)
            {
                ImgProcess.CamSetNum = 1;

                ImgProcess.SNCamIdx = 2;
                ImgProcess.LF2CamIdx = 1;

                nudCamIndexC.Value = 1;
                nudCamIndexSN.Value = 2;

            }
        }

        private void rbCamNum2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCamNum2.Checked)
            {
                ImgProcess.CamSetNum = 2;

                ImgProcess.SNCamIdx = 1;
                ImgProcess.LF2CamIdx = 8;

                nudCamIndexC.Value = 8;
                nudCamIndexSN.Value = 1;
            }
        }

        private void rbCamNum3_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCamNum3.Checked)
            {
                ImgProcess.CamSetNum = 3;
            }
        }

        private void rbCamNum4_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCamNum4.Checked)
            {
                ImgProcess.CamSetNum = 4;
            }
        }

        private void btnCamNumSet_Click(object sender, EventArgs e)
        {
            saver_star_SN_Click(null, null);
        }

        //清洗摆动距离设置
        private void tbCleaningSwingDist_JK_TextChanged(object sender, EventArgs e)
        {
            ImgProcess.CleaningSwingDist_JK = double.Parse(tbCleaningSwingDist_JK.Text);
        }

        //清洗时间设置(单位：ms)
        private void tbCleaningTime_JK_TextChanged(object sender, EventArgs e)
        {
            ImgProcess.CleaningTime_JK = int.Parse(tbCleaningTime_JK.Text);
        }

        //清洗时摆动动作的选择项
        private void cbRunClear_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRunClear.Checked)
                ImgProcess.RunClearing_JK = 1;

            else
                ImgProcess.RunClearing_JK = 0;
        }

        //主页显示
        private void btnHome_Click(object sender, EventArgs e)
        {
            //用户登录权限设置 20180604
            #region //调试界面
            //显示信息
            this.tabPage1.Parent = this.tabControl1_主设置;
            //显示参数设置
            this.tabPage7.Parent = this.tabControl1_主设置;
            //隐藏相机设置
            tabPage23.Parent = null;
            //隐藏焊接参数
            tabPage_rightcam.Parent = null;
            //隐藏定位平台
            TabP_Table.Parent = null;
            //隐藏通讯设置
            tabPage2.Parent = null;
            //隐藏定位模板
            tabPage_IO.Parent = null;
            //隐藏焊前正面
            tabPage17.Parent = null;
            //隐藏焊前侧面
            tabPage6.Parent = null;
            //隐藏焊后正面
            tabPage9.Parent = null;
            //隐藏焊后侧面
            tabPage10.Parent = null;
            //隐藏条码设置
            tabPage14.Parent = null;
            #endregion

            #region //显示窗口
            ////显示细节窗口
            //this.tabPage_Left.Parent = this.tabControl1;
            ////显示焊后正面窗口
            //this.tabPage11.Parent = this.tabControl1;
            ////隐藏正面图像定位窗口
            //tabPage3.Parent = null;
            ////隐藏正面焊前识别窗口
            //tabPage18.Parent = null;
            ////隐藏侧面焊前识别窗口
            //tabPage8.Parent = null;
            ////隐藏条码识别窗口
            //tabPage13.Parent = null;
            ////隐藏未使用窗口
            //tabPage12.Parent = null;
            #endregion
        }

        //回车默认登录
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //绑定Enter键
            if (e.KeyCode == Keys.Enter)
            {
                //触发登录按钮
                this.用户登陆_Click(null, null);
            }
        }

        //到位模拟按钮移植
        private void btnSimulation_Click(object sender, EventArgs e)
        {
            button_siml_Click(null, null);
        }

        //送锡测试按钮移植
        private void btnDeliveryTest_Click(object sender, EventArgs e)
        {
            button_TinTrg_Click(null, null);
        }

        //保存高清照片
        private void cbZM2_saver_image_CheckedChanged(object sender, EventArgs e)
        {
            if (cbZM2_saver_image.Checked) ImgProcess._CBZM2_saver_image = 1;
            else ImgProcess._CBZM2_saver_image = -1;
        }





    }
}












