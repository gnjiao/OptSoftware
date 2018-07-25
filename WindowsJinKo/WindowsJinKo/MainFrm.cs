using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;
using System.Collections;
using System.IO;

namespace WindowsJinKo
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        //实例化相关类
        RunLog RLog = new RunLog();
        HalconVision HVision = new HalconVision();


        //定义坐标，方便图像缩放
        public HTuple m_ImageRow0;			//当前在窗口显示的图像的左上角坐标y(图像坐标系)
        public HTuple m_ImageCol0;			//当前在窗口显示的图像的左上角坐标x(图像坐标系)
        public HTuple m_ImageRow1;			//当前在窗口显示的图像的右下角坐标y(图像坐标系)
        public HTuple m_ImageCol1;			//当前在窗口显示的图像的右下角坐标x(图像坐标系)
        public HTuple Width;                //定义图像宽度
        public HTuple Height;               //定义图像高度

        public HObject ho_Shape;               //定义模板区域
        //定义图像变量
        HObject ho_Image = null;
        //定义启动线程的bool变量
        public bool m_bBusy = false;
        //定义一个线程
        public Thread m_GrabThread;
        //定义相机句柄
        HTuple hv_AcqHandle = null;
        //模板方式选择
        public int ModelSelect = 0;


        //主窗体加载
        private void MainFrm_Load(object sender, EventArgs e)
        {
            //读取参数设置
            Read_Parameter();
            //创建文件夹
            RLog.CreateFile();
            //加载相机
            CamFramegrabber(tsslPath.Text);
            //创建结果图像文件夹
            HVision.creation_date_file(tbImgResultPath.Text);
            
            try
            {
            //读取模板和区域
            HOperatorSet.ReadRegion(out ho_Shape, (HVision.ModelPath + "/") + "Shape");
            HOperatorSet.ReadShapeModel((HVision.ModelPath + "/") + "Model", out HVision.ShapeModel.Model);
            }
            catch (Exception)
            {}
           
            //按钮使用限制

            //保存模板按钮
            btnSaveShapeModel.Enabled = false;
            //保存区域按钮
            btnSaveShapeRegion.Enabled = false;
            //停止按钮
            btnQuit.Enabled = false;
            //图像测试按钮
            btnTest.Enabled = false;
            //设置开始按钮上的图像
            btnStart.Image = global::WindowsJinKo.Properties.Resources.启动1;
            //设置停止按钮上的图像
            btnQuit.Image = global::WindowsJinKo.Properties.Resources.停止2;
            
            //启动定时器（定时创建文件夹）
            tmrPath.Start(); 

            //窗体运行界面初始化
            this.tpRun.Parent = this.tcSet;
            //隐藏参数设置
            tpParam.Parent = null;
            //隐藏相机调试
            tpModel.Parent = null;
            //隐藏光源调试
            tpPath.Parent = null;
        }

        //打开图像
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();

            Dlg.Filter = "(*.bmp; *.jpg; *.tif)|*.bmp; *.jpg; *.tif";
            Dlg.Multiselect = false;
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //定义图像变量
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    //打开图像
                    ho_Image.Dispose();
                    HOperatorSet.ReadImage(out ho_Image, Dlg.FileName);
                    //获取图像大小并显示全图
                    HOperatorSet.GetImageSize(ho_Image, out Width, out Height);
                    //对坐标赋值，方便图像缩放
                    m_ImageRow0 = 0;
                    m_ImageRow1 = Height - 1;
                    m_ImageCol0 = 0;
                    m_ImageCol1 = Width - 1;
                    HOperatorSet.SetPart(hWindowMainID.HalconWindow, 0, 0, Height - 1, Width - 1);
                    HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);

                    //内存释放
                    Dlg.Dispose();

                    //按钮使用限制 图像测试按钮
                    btnTest.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("图像打开失败！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        //保存设置
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save_Parameter();
        }
   
        //开始检测
        private void btnStart_Click(object sender, EventArgs e)
        {
            m_bBusy = true;
            //开辟一个线程
            m_GrabThread = new Thread(new ThreadStart(GrabImageThread));
            //开始线程
            m_GrabThread.Start();

            //停用开始按钮
            btnStart.Enabled = false;
            //启用停止按钮
            btnQuit.Enabled = true;
            //设置开始按钮上的图像
            btnStart.Image = global::WindowsJinKo.Properties.Resources.启动2;
            //设置停止按钮上的图像
            btnQuit.Image = global::WindowsJinKo.Properties.Resources.停止1;
        }

        //停止检测
        private void btnQuit_Click(object sender, EventArgs e)
        {
            //停止检测
            m_bBusy = false;

            //启用开始按钮
            btnStart.Enabled = true;
            //停用停止按钮
            btnQuit.Enabled = false;
            //设置开始按钮上的图像
            btnStart.Image = global::WindowsJinKo.Properties.Resources.启动1;
            //设置停止按钮上的图像
            btnQuit.Image = global::WindowsJinKo.Properties.Resources.停止2;
        }

        //关闭窗体
        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bBusy = false;
            ho_Image.Dispose();
            //关闭所有窗体
            Application.Exit();
        }

        //图像测试
        private void btnTest_Click(object sender, EventArgs e)
        {
            //LoadBatchImage(RLog.hv_ImgPath+"/");
            HVision.Weld_Image_Processing(ho_Image, HVision.ShapeModel.Model, hWindowMainID.HalconWindow, out HVision.Other.OutNg);
        }

        //单帧采集
        private void btnOneShot_Click(object sender, EventArgs e)
        {
            try
            {
                ho_Image.Dispose();
                HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);
                //图像宽,高
                HTuple Width, Height;
                //获取图像大小
                HOperatorSet.GetImageSize(ho_Image, out Width, out Height);
                //对坐标赋值，方便图像缩放
                m_ImageRow0 = 0;
                m_ImageRow1 = Height - 1;
                m_ImageCol0 = 0;
                m_ImageCol1 = Width - 1;
                //全图显示
                HOperatorSet.SetPart(hWindowMainID.HalconWindow, 0, 0, Height - 1, Width - 1);
                //显示图形
                HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);
               
                //按钮使用限制 图像测试按钮
                btnTest.Enabled = true;
            }
            catch (System.Exception)
            {
            }
        }

        //创建模板
        private void btnCreateShapeModel_Click(object sender, EventArgs e)
        {
            try
            {        
            HObject ho_ImageAffin, ho_ImageReduced, ho_ImageZoomed, ho_ShapeOrigi;
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Phi1 = new HTuple();
            HTuple hv_S2 = null, hv_S = null, hv_S1 = null;
            HOperatorSet.GenEmptyObj(out ho_ImageAffin);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Shape);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed);
            HOperatorSet.GenEmptyObj(out ho_ShapeOrigi);

            HOperatorSet.SetColored(hWindowMainID.HalconWindow, 12);
            HOperatorSet.SetColor(hWindowMainID.HalconWindow, "green");
            HOperatorSet.SetLineWidth(hWindowMainID.HalconWindow, 2);

            ho_ImageAffin.Dispose();
            HVision.image_rotate(ho_Image, out ho_ImageAffin);

            HOperatorSet.CountSeconds(out hv_S1);
            if (ModelSelect==0)
            {
                HOperatorSet.DrawRectangle1(hWindowMainID.HalconWindow, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                ho_Shape.Dispose();
                HOperatorSet.GenRectangle1(out ho_Shape, hv_Row1, hv_Column1, hv_Row2, hv_Column2);  
            }
            else if (ModelSelect == 1)
            {
                HOperatorSet.DrawCircle(hWindowMainID.HalconWindow, out hv_Row1, out hv_Column1,out hv_Column2);
                ho_Shape.Dispose();
                HOperatorSet.GenCircle(out ho_Shape, hv_Row1, hv_Column1, hv_Column2);
            }
            else if (ModelSelect == 2)
            {
                HOperatorSet.DrawRectangle2(hWindowMainID.HalconWindow, out hv_Row1, out hv_Column1,out hv_Phi1, out hv_Row2, out hv_Column2);
                ho_Shape.Dispose();
                HOperatorSet.GenRectangle2(out ho_Shape, hv_Row1, hv_Column1, hv_Phi1, hv_Row2, hv_Column2); 
            }
            else if (ModelSelect == 3)
            {
                HOperatorSet.DrawEllipse(hWindowMainID.HalconWindow, out hv_Row1, out hv_Column1, out hv_Phi1, out hv_Row2, out hv_Column2);
                ho_Shape.Dispose();
                HOperatorSet.GenEllipse(out ho_Shape, hv_Row1, hv_Column1, hv_Phi1, hv_Row2, hv_Column2); 
            }

            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageAffin, ho_Shape, out ho_ImageReduced);
            HOperatorSet.CopyObj(ho_Shape, out ho_ShapeOrigi, 1, 1);
            HOperatorSet.ZoomRegion(ho_Shape, out  ho_Shape, 0.5, 0.5);
        
            ho_ImageZoomed.Dispose();
            HOperatorSet.ZoomImageFactor(ho_ImageReduced, out ho_ImageZoomed, 0.5, 0.5,
                "constant");
            //建立模板
            HOperatorSet.CreateShapeModel(ho_ImageZoomed, "auto", 0, (new HTuple(360)).TupleRad()
                , "auto", "auto", "use_polarity", "auto", "auto", out HVision.ShapeModel.Model);
            //HOperatorSet.CountSeconds(out hv_S2);
            //hv_S = (hv_S2 - hv_S1) * 1000;

            HOperatorSet.DispObj(ho_ShapeOrigi, hWindowMainID.HalconWindow);
             //获取当前程序目录
            //string TimeNow=DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss");
            //RLog.writeCSV(HVision.DatePath, hv_TemplateID, hv_S, 1, false);
            MessageBox.Show("模板创建成功！");


            //按钮使用限制
            //保存模板按钮
            btnSaveShapeModel.Enabled = true;
            //保存区域按钮
            btnSaveShapeRegion.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("模板创建失败！");
            }
        }

        //保存模板
        private void btnSaveShapeModel_Click(object sender, EventArgs e)
        {
            HOperatorSet.WriteShapeModel(HVision.ShapeModel.Model, (HVision.ModelPath + "/") + "Model");
            MessageBox.Show("模板保存成功！");
        }

        //模板创建shape形状选择
        private void cbDrawShap_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbDrawShap.SelectedIndex)
            {
                case 0:
                    ModelSelect = 0;
                    break;
                case 1:
                    ModelSelect = 1;
                    break;
                case 2:
                    ModelSelect = 2;
                    break;
                case 3:
                    ModelSelect = 3;
                    break;
                default:
                    break;
            }
        }

        //单击左键显示全图
        private void hWindowMainID_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                HTuple hv_Button;
                HTuple ptX, ptY;
                //获取鼠标位置
                HOperatorSet.GetMposition(hWindowMainID.HalconWindow, out ptY, out ptX, out hv_Button);
                //当鼠标左键按下，显示原图
                if (hv_Button == 1)
                {
                    //设置显示窗口
                    HOperatorSet.SetPart(hWindowMainID.HalconWindow, 0, 0, Height - 1, Width - 1);
                    //显示原图
                    HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);
                }
            }
            catch (Exception ex)
            {
                //保存异常日志
                RLog.RunRecord("单击右键显示全图 异常-> " + ex.Message);
            }
        }

        //显示灰度值和坐标位置
        private void hWindowMainID_HMouseMove(object sender, HMouseEventArgs e)
        {
            HTuple hv_Button;
            HTuple ptX, ptY;
            HTuple GreyR, GreyG, GreyB;
            HTuple hv_Channels = null;

            HObject Cross;
            HOperatorSet.GenEmptyObj(out Cross);
            HObject ho_ImageR, ho_ImageG, ho_ImageB;
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            try
            {
                //获取鼠标位置Location
                HOperatorSet.GetMposition(hWindowMainID.HalconWindow, out ptY, out ptX, out hv_Button);
                tsslLocation.Text = "Location " + "X: " + ptX.ToString() + " Y: " + ptY.ToString();
                HOperatorSet.CountChannels(ho_Image, out hv_Channels);
                if ((int)(new HTuple(hv_Channels.TupleGreater(2))) != 0)
                {
                    ho_ImageR.Dispose(); ho_ImageG.Dispose(); ho_ImageB.Dispose();
                    HOperatorSet.Decompose3(ho_Image, out ho_ImageR, out ho_ImageG, out ho_ImageB);
                    HOperatorSet.GetGrayval(ho_ImageR, ptY, ptX, out GreyR);
                    HOperatorSet.GetGrayval(ho_ImageG, ptY, ptX, out GreyG);
                    HOperatorSet.GetGrayval(ho_ImageB, ptY, ptX, out GreyB);
                    tsslGray.Text = "Gray " + "R: " + GreyR.ToString() + " G: " + GreyG.ToString() + " B: " + GreyB.ToString();
                }
                else
                {
                    HOperatorSet.Rgb1ToGray(ho_Image, out ho_Image);
                    HOperatorSet.GetGrayval(ho_Image, ptY, ptX, out GreyR); ;
                    tsslGray.Text = "Gray " + "R: " + GreyR.ToString() + " G: " + GreyR.ToString() + " B: " + GreyR.ToString();
                }
            }
            catch (Exception ex)
            {
                // 保存异常日志
                RLog.RunRecord("获取图像灰度值和坐标位置 异常-> " + ex.Message);
            }
        }

        //滚动鼠标图像窗口缩放
        private void hWindowMainID_HMouseWheel(object sender, HMouseEventArgs e)
        {
            try
            {
                HTuple hv_Button;
                HTuple ImagePtX, ImagePtY;
                HTuple Row0_1, Col0_1, Row1_1, Col1_1;
                HTuple Scale = 0.1;	//缩放步长
                HTuple MaxScale = 40;//最大放大系数
                HTuple ptX, ptY;
                try
                {
                    //查询鼠标位置
                    HOperatorSet.GetMposition(hWindowMainID.HalconWindow, out ptY, out ptX, out hv_Button);
                }
                catch (Exception ex)
                {
                    ptX = -1;
                    ptY = -1;
                    hv_Button = -1;
                    // 保存异常日志
                    RLog.RunRecord("图像放大缩小->查询鼠标位置 异常-> " + ex.Message);
                }
                //获取图像尺寸
                HOperatorSet.GetImageSize(ho_Image, out Width, out Height);
                //判断鼠标右键是否按下
                //         0:No button
                //         1:Left button 
                //         2:Middle button
                //         4:Right button
                if (hv_Button == 0)
                {
                    //向上滑动滚轮，图像缩小。以当前鼠标的坐标为支点进行缩小或放大
                    if (e.Delta > 0)
                    {
                        //把当前鼠标的坐标由窗口坐标转化为图像坐标
                        ImagePtX = ptX;
                        ImagePtY = ptY;
                        //重新计算缩小后的图像区域
                        Row0_1 = ImagePtY - 1 / (1 - Scale) * (ImagePtY - m_ImageRow0);
                        Row1_1 = ImagePtY - 1 / (1 - Scale) * (ImagePtY - m_ImageRow1);
                        Col0_1 = ImagePtX - 1 / (1 - Scale) * (ImagePtX - m_ImageCol0);
                        Col1_1 = ImagePtX - 1 / (1 - Scale) * (ImagePtX - m_ImageCol1);
                        //限定缩小范围
                        if ((Col1_1 - Col0_1).TupleAbs() / Width <= 2)
                        {
                            //设置在图形窗口中显示局部图像
                            m_ImageRow0 = Row0_1;
                            m_ImageCol0 = Col0_1;
                            m_ImageRow1 = Row1_1;
                            m_ImageCol1 = Col1_1;
                        }
                    }
                    //向下滑动滚轮，图像放大
                    else
                    {
                        //把当前鼠标的坐标由窗口坐标转化为图像坐标
                        ImagePtX = ptX;
                        ImagePtY = ptY;
                        //重新计算放大后的图像区域
                        Row0_1 = ImagePtY - 1 / (1 + Scale) * (ImagePtY - m_ImageRow0);
                        Row1_1 = ImagePtY - 1 / (1 + Scale) * (ImagePtY - m_ImageRow1);
                        Col0_1 = ImagePtX - 1 / (1 + Scale) * (ImagePtX - m_ImageCol0);
                        Col1_1 = ImagePtX - 1 / (1 + Scale) * (ImagePtX - m_ImageCol1);
                        //限定放大范围
                        if ((Width / (Col1_1 - Col0_1).TupleAbs()) <= MaxScale)
                        {
                            //设置在图形窗口中显示局部图像
                            m_ImageRow0 = Row0_1;
                            m_ImageCol0 = Col0_1;
                            m_ImageRow1 = Row1_1;
                            m_ImageCol1 = Col1_1;
                        }
                    }
                    HOperatorSet.ClearWindow(hWindowMainID.HalconWindow);
                    HOperatorSet.SetColor(hWindowMainID.HalconWindow, "green");
                    HOperatorSet.SetPart(hWindowMainID.HalconWindow, m_ImageRow0, m_ImageCol0, m_ImageRow1, m_ImageCol1);
                    //显示图像
                    HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);
                }
            }
            catch (Exception)
            {
                // 保存异常日志
                //RLog.RunRecord("图像放大缩小 异常-> " + ex.Message);
            }
        }

        //保存图像
        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.Filter = "(*.bmp; *.jpg; *.png)|*.bmp; *.jpg; *.png";
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                //保存图像
                HOperatorSet.WriteImage(ho_Image, "bmp", 0, Dlg.FileName);
            }
            Dlg.Dispose();
        }

        //保存模板区域
        private void btnSaveShapeRegion_Click(object sender, EventArgs e)
        {
            HOperatorSet.WriteRegion(ho_Shape, HVision.ModelPath + "\\Shape");
            MessageBox.Show("区域保存成功！");
        }

        //选择图像读取路径
        private void btnCamPath_Click(object sender, EventArgs e)
        {
            //创建FolderBrowserDialog对象
            FolderBrowserDialog FBDialog = new FolderBrowserDialog();
            //判断是否选择文件夹
            if (FBDialog.ShowDialog() == DialogResult.OK)
            {
                //记录选择的文件夹
                HVision.hv_ImgPath = FBDialog.SelectedPath;
                if (HVision.hv_ImgPath.EndsWith("\\"))
                {
                    tsslPath.Text = HVision.hv_ImgPath;//显示选择的文件夹
                }
                else
                {
                    tsslPath.Text = HVision.hv_ImgPath + "\\";//显示选择的文件夹
                }
            }
        }

        //统计结果清除
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("确认清除数据？", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            if (DR == DialogResult.OK)
               {
                  HVision.Other.OutNgNum = 0;         
                  HVision.Other.OutOkNum = 0;
                  HVision.Other.WeldTotalNum = 0;

                  lbTotalNum.Text = "总量:"+"0";
                  lb_OK.Text = "OK:" + "0";
                  lb_NG.Text = "NG:" + "0";
                }
        }

        #region //自定义函数
        //读取固定位置照片
        public void CamFramegrabber(string hv_ImgPath)
        {
            try
            {
                HOperatorSet.GenEmptyObj(out ho_Image);
                HOperatorSet.SetDraw(hWindowMainID.HalconWindow, "margin");
                HOperatorSet.OpenFramegrabber("File", -1, -1, -1, -1, -1, -1, "default", -1,
                                              "default", -1, "default", hv_ImgPath, "default", -1, -1, out hv_AcqHandle);
            }
            catch (Exception)
            { }
        }

        //批量读取照片(未使用)
        public void LoadBatchImage(string ImagePath)
        {
            try
            {
                ArrayList Al = new ArrayList();

                //遍历文件夹,获取所有文件的路径 
                DirectoryInfo theFolder = new DirectoryInfo(ImagePath);
                FileInfo[] fileInfo = theFolder.GetFiles();
                foreach (FileInfo NextFile in fileInfo) //遍历文件 
                {
                    Al.Add(ImagePath + NextFile.Name);
                }

                //开始批量读取图片 
                int ImageIndex = 0;
                foreach (object FileName in Al)
                {
                    ImageIndex++;
                    //定义图像变量
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    //打开图像
                    ho_Image.Dispose();
                    HOperatorSet.ReadImage(out ho_Image, (string)FileName);
                    //获取图像大小并显示全图
                    HOperatorSet.GetImageSize(ho_Image, out Width, out Height);
                    //对坐标赋值，方便图像缩放
                    m_ImageRow0 = 0;
                    m_ImageRow1 = Height - 1;
                    m_ImageCol0 = 0;
                    m_ImageCol1 = Width - 1;
                    HOperatorSet.SetPart(hWindowMainID.HalconWindow, 0, 0, Height - 1, Width - 1);
                    HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);
                    //这里是图像处理代码，此处省略.......

                    
                }
            }
            catch (Exception)
            { }
        }

        //线程采图
        public void GrabImageThread()
        {
            while (m_bBusy)
            {
                try
                {
                    if (hv_AcqHandle > -1 && hv_AcqHandle != null)
                    {
                        ho_Image.Dispose();
                        HOperatorSet.GrabImage(out ho_Image, hv_AcqHandle);
                        //HOperatorSet.WaitSeconds(0.1);
                        if (ho_Image != null)
                        {
                            //图像宽,高
                            HTuple Width, Height;
                            //获取图像大小
                            HOperatorSet.GetImageSize(ho_Image, out Width, out Height);
                            //显示全图
                            HOperatorSet.SetPart(hWindowMainID.HalconWindow, 0, 0, Height - 1, Width - 1);
                            HOperatorSet.DispObj(ho_Image, hWindowMainID.HalconWindow);
                            //添加图像处理算法
                            HVision.Weld_Image_Processing(ho_Image, HVision.ShapeModel.Model,hWindowMainID.HalconWindow ,out HVision.Other.OutNg);

                            //统计数量
                            if (HVision.Other.OutNg == 1)
                            {
                                HVision.Other.OutNgNum = HVision.Other.OutNgNum + 1;
                            }
                            else
                            {
                                HVision.Other.OutOkNum = HVision.Other.OutOkNum + 1;
                            }
                            HVision.Other.WeldTotalNum = HVision.Other.OutOkNum + HVision.Other.OutNgNum;
                            Updata_Total_Number();
                            

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (System.Exception)
                {
                    Thread.Sleep(10);
                }
            }
            m_GrabThread.Abort();
        }

        //保存设置参数
        public void Save_Parameter()
        {
            try
            {
            HVision.IniWriteValue("图像路径", "ImgResultPath", tbImgResultPath.Text, HVision.DatePath);
            HVision.IniWriteValue("图像路径", "ImagePath", tsslPath.Text, HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudWeldX", Convert.ToString(nudWeldX.Value), HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudWeldY", Convert.ToString(nudWeldY.Value), HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudSpaceMain", Convert.ToString(nudSpaceMain.Value), HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudSpaceSecond", Convert.ToString(nudSpaceSecond.Value), HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudRectW", Convert.ToString(nudRectW.Value), HVision.DatePath);
            HVision.IniWriteValue("焊点位置", "nudRectH", Convert.ToString(nudRectH.Value), HVision.DatePath);
            HVision.IniWriteValue("矩形位置", "nudRectUp", Convert.ToString(nudRectUp.Value), HVision.DatePath);
            HVision.IniWriteValue("矩形位置", "nudRectDown", Convert.ToString(nudRectDown.Value), HVision.DatePath);
            HVision.IniWriteValue("调试参数", "nudHoleMinHeight", Convert.ToString(nudHoleMinHeight.Value), HVision.DatePath);
            HVision.IniWriteValue("调试参数", "nudHoleMaxHeight", Convert.ToString(nudHoleMaxHeight.Value), HVision.DatePath);
            HVision.IniWriteValue("调试参数", "nudHoleMinWidth", Convert.ToString(nudHoleMinWidth.Value), HVision.DatePath);
            HVision.IniWriteValue("调试参数", "nudHoleMaxArea", Convert.ToString(nudHoleMaxArea.Value), HVision.DatePath);
            HVision.IniWriteValue("调试参数", "nudBulkiness", Convert.ToString(nudBulkiness.Value), HVision.DatePath);
            if (cbDefectRegion.Checked)
            {
                HVision.Other.DefectRegion = 1;
            }
            else
            {
                HVision.Other.DefectRegion = 0;
            }
            HVision.IniWriteValue("显示参数", "HVision.Other.DefectRegion", Convert.ToString(HVision.Other.DefectRegion), HVision.DatePath);
             if (cbShowRect.Checked)
            {
                HVision.Other.ShowWeldRect = 1;
            }
            else
            {
                HVision.Other.ShowWeldRect = 0;
            }
             HVision.IniWriteValue("显示参数", "HVision.Other.ShowWeldRect", Convert.ToString(HVision.Other.ShowWeldRect), HVision.DatePath);






            }
            catch (Exception)
            {
                MessageBox.Show("参数保存失败！");
            }
        }

        //读取设置参数
        public void Read_Parameter()
        {
            try
            {
            tbImgResultPath.Text = HVision.IniReadValue("图像路径", "ImgResultPath", HVision.DatePath);
            tsslPath.Text = HVision.IniReadValue("图像路径", "ImagePath", HVision.DatePath);
           
            nudWeldX.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudWeldX", HVision.DatePath));
            HVision.WeldLation.nudWeldX = float.Parse((nudWeldX.Value).ToString()); 
            
            nudWeldY.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudWeldY", HVision.DatePath));
            HVision.WeldLation.nudWeldY = float.Parse((nudWeldY.Value).ToString()); 
           
            nudSpaceMain.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudSpaceMain", HVision.DatePath));
            HVision.WeldLation.nudSpaceMain = float.Parse((nudSpaceMain.Value).ToString()); 
           
            nudSpaceSecond.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudSpaceSecond", HVision.DatePath));
            HVision.WeldLation.nudSpaceSecond = float.Parse((nudSpaceSecond.Value).ToString());               
            
            nudRectW.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudRectW", HVision.DatePath));
            HVision.WeldLation.nudRectW = float.Parse((nudRectW.Value).ToString()); 
           
            nudRectH.Value = Convert.ToDecimal(HVision.IniReadValue("焊点位置", "nudRectH", HVision.DatePath));
            HVision.WeldLation.nudRectH = float.Parse((nudRectH.Value).ToString()); 
           
            nudRectUp.Value = Convert.ToDecimal(HVision.IniReadValue("矩形位置", "nudRectUp", HVision.DatePath));
            HVision.WeldLation.nudRectUp = float.Parse((nudRectUp.Value).ToString()); 
            nudRectDown.Value = Convert.ToDecimal(HVision.IniReadValue("矩形位置", "nudRectDown", HVision.DatePath));
            HVision.WeldLation.nudRectDown = float.Parse((nudRectDown.Value).ToString()); 

            nudHoleMinHeight.Value = Convert.ToDecimal(HVision.IniReadValue("调试参数", "nudHoleMinHeight", HVision.DatePath));
            nudHoleMaxHeight.Value = Convert.ToDecimal(HVision.IniReadValue("调试参数", "nudHoleMaxHeight", HVision.DatePath));
            nudHoleMinWidth.Value = Convert.ToDecimal(HVision.IniReadValue("调试参数", "nudHoleMinWidth", HVision.DatePath));
            nudHoleMaxArea.Value = Convert.ToDecimal(HVision.IniReadValue("调试参数", "nudHoleMaxArea", HVision.DatePath));
            nudBulkiness.Value = Convert.ToDecimal(HVision.IniReadValue("调试参数", "nudBulkiness", HVision.DatePath));
            HVision.Other.DefectRegion = Convert.ToInt32(HVision.IniReadValue("显示参数", "HVision.Other.DefectRegion", HVision.DatePath));
            if (HVision.Other.DefectRegion == 1)
            {
                cbDefectRegion.Checked = true;
            }
            else
            {
                cbDefectRegion.Checked = false;
            }

            HVision.Other.ShowWeldRect = Convert.ToInt32(HVision.IniReadValue("显示参数", "HVision.Other.ShowWeldRect", HVision.DatePath));
            if (HVision.Other.ShowWeldRect == 1)
            {
                cbShowRect.Checked = true;
            }
            else
            {
                cbShowRect.Checked = false;
            }
            
            }
            catch (Exception)
            {
                MessageBox.Show("参数读取失败！");
            }
        }

        //参数统计计算
        private void Statistical_Data()
        {
            double OK_Percentage = Math.Round((HVision.Other.OutOkNum * 1.0) / ((HVision.Other.WeldTotalNum) * 1.0) * 100.0, 2);
            double NG_Percentage = Math.Round((HVision.Other.OutNgNum * 1.0) / ((HVision.Other.WeldTotalNum) * 1.0) * 100.0, 2);
            if (HVision.Other.OutOkNum == 0)
            {
                OK_Percentage = 0;
            }
            if (HVision.Other.OutNgNum == 0)
            {
                NG_Percentage = 0;
            }

            lbTotalNum.Text = "总量:" + HVision.Other.WeldTotalNum + "  " ;
            lb_OK.Text = "OK:" + HVision.Other.OutOkNum + "  " + OK_Percentage.ToString() + "%";
            lb_NG.Text = "NG:" + HVision.Other.OutNgNum + "  " + NG_Percentage.ToString() + "%";
        }
        //声明委托
        public delegate void Updata_Delegate();       
        //调用委托
        private void Updata_Total_Number()
        {
            Updata_Delegate Updata_Total_Number = new Updata_Delegate(Statistical_Data);
            //异步调用委托,调用后立即返回并立即执行下面的语句
            BeginInvoke(Updata_Total_Number); 
        }

        //图像保存路径的定时器
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
                DirectoryInfo DInfo = new DirectoryInfo(tbImgResultPath.Text + strName);
                //创建文件夹
                DInfo.Create();
            }

            // 设置　每天的20:00:00创建一个夜班文件夹 
            if (intHour == iBHour && intMinute == iBMinute && intSecond == iBSecond)
            {
                //对当前日期进行格式化
                string strName = DateTime.Now.ToString("yyyy年MM月dd日 夜班");
                //创建DirectoryInfo对象
                DirectoryInfo DInfo = new DirectoryInfo(tbImgResultPath.Text + strName);
                //创建文件夹
                DInfo.Create();
            }

        }
        #endregion

        //实时传递焊点X方向坐标
        private void nudWeldX_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudWeldX = (int)nudWeldX.Value;
        }

        //实时传递焊点Y方向坐标
        private void nudWeldY_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudWeldY = (int)nudWeldY.Value;
        }

        //实时传递焊点主间距
        private void nudSpaceMain_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudSpaceMain = (int)nudSpaceMain.Value;
        }

        //实时传递焊点中间间距
        private void nudSpaceSecond_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudSpaceSecond = (int)nudSpaceSecond.Value;
        }

        //实时传递焊点矩形宽度
        private void nudRectW_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudRectW = (int)nudRectW.Value;
        }

        //实时传递焊点矩形高度
        private void nudRectH_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudRectH = (int)nudRectH.Value;
        }

        //实时传递焊点大矩形上坐标
        private void nudRectUp_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudRectUp = (int)nudRectUp.Value;
        }

        //实时传递焊点大矩形下坐标
        private void nudRectDown_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldLation.nudRectDown = (int)nudRectDown.Value;
        }

        //实时传递焊点缺陷孔的最小高度
        private void nudHoleMinHeight_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldCheck.nudHoleMinHeight = (int)nudHoleMinHeight.Value;
        }

        //实时传递焊点缺陷孔的最大高度
        private void nudHoleMaxHeight_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldCheck.nudHoleMaxHeight = (int)nudHoleMaxHeight.Value;
        }

        //实时传递焊点缺陷孔的最小宽度
        private void nudHoleMinWidth_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldCheck.nudHoleMinWidth = (int)nudHoleMinWidth.Value;
        }

        //实时传递焊点缺陷孔的最小面积
        private void nudHoleMaxArea_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldCheck.nudHoleMaxArea = (int)nudHoleMaxArea.Value;
        }

        //实时传递焊点缺陷孔的彭松度
        private void nudBulkiness_ValueChanged(object sender, EventArgs e)
        {
            HVision.WeldCheck.nudBulkiness = (int)nudBulkiness.Value;
        }

        //主页按钮的提示功能
        private void btnHome_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnHome, "主页");
        }

        //开始按钮的提示功能
        private void btnStart_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnStart, "开始");
        }

        //停止按钮的提示功能
        private void btnQuit_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnQuit, "停止");
        }

        //打开文件路径按钮的提示功能
        private void btnCamPath_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnCamPath, "文件路径");
        }

        //打开文件按钮提示功能
        private void btnOpen_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnOpen, "打开文件");
        }

        //图像测试按钮提示功能
        private void btnTest_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnTest, "图像测试");
        }

        //参数保存按钮提示功能
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnSave, "参数保存");
        }

        //清除数据按钮提示功能
        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            ToolTip prompt = new ToolTip();
            prompt.ShowAlways = true;
            prompt.SetToolTip(this.btnDelete, "清除数据");

        }

        //主页刷新登录按钮功能
        private void btnHome_Click(object sender, EventArgs e)
        {
            //设置登录按钮上的图像
            btnLog.Image = global::WindowsJinKo.Properties.Resources.用户2;
            //显示结果信息
            this.tpRun.Parent = this.tcSet;
            //隐藏参数设置
            tpParam.Parent = null;
            //隐藏相机调试
            tpModel.Parent = null;
            //隐藏光源调试
            tpPath.Parent = null;
        }

        //图像结果保存路径
        private void btnImgResultPath_Click(object sender, EventArgs e)
        {
            //创建FolderBrowserDialog对象
            FolderBrowserDialog FBDialog = new FolderBrowserDialog();
            //判断是否选择文件夹
            if (FBDialog.ShowDialog() == DialogResult.OK)
            {
                //记录选择的文件夹
                HVision.ImgResultPath = FBDialog.SelectedPath;
                if (HVision.ImgResultPath.EndsWith("\\"))
                {
                    tbImgResultPath.Text = HVision.ImgResultPath;//显示选择的文件夹
                }
                else
                {
                    tbImgResultPath.Text = HVision.ImgResultPath + "\\";//显示选择的文件夹
                }
            }
        }

        //登录按钮
        private void btnLog_Click(object sender, EventArgs e)
        {
            if (cbUser.Text == "用户登录")
            {
                if (tbPass.Text == "111")
                {
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //隐藏参数设置
                    tpModel.Parent = null;
                    //隐藏相机调试
                    tpParam.Parent = null;
                    //隐藏光源调试
                    tpPath.Parent = null;
                    //信息提示
                    MessageBox.Show("已默认登录!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //登录成功后，密码设置为空
                    tbPass.Text = "";
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户1;
                }
                else
                {
                    //信息提示
                    MessageBox.Show("密码错误!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //登录失败后，密码设置为空
                    tbPass.Text = "";
                    //设置光标位置
                    tbPass.Focus();
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户2;
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //隐藏参数设置
                    tpModel.Parent = null;
                    //隐藏相机调试
                    tpParam.Parent = null;
                    //隐藏光源调试
                    tpPath.Parent = null;
                }             
            }
            else if (cbUser.Text == "售后调试")
            {
                if (tbPass.Text == "112")
                {
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //显示参数设置
                    this.tpModel.Parent = this.tcSet;
                    //隐藏相机调试
                    tpParam.Parent = null;
                    //隐藏光源调试
                    tpPath.Parent = null;
                    //登录成功后，密码设置为空
                    tbPass.Text = "";
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户1;
                }
                else
                {
                    //信息提示
                    MessageBox.Show("密码错误!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //登录失败后，密码设置为空
                    tbPass.Text = "";
                    //设置光标位置
                    tbPass.Focus();
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户2;
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //隐藏参数设置
                    tpModel.Parent = null;
                    //隐藏相机调试
                    tpParam.Parent = null;
                    //隐藏光源调试
                    tpPath.Parent = null;
                }
            }
            else if (cbUser.Text == "软件调试")
            {
                if (tbPass.Text == "113")
                {
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //显示参数设置
                    this.tpModel.Parent = this.tcSet;
                    //显示相机调试
                    this.tpParam.Parent = this.tcSet;
                    //显示光源调试
                    this.tpPath.Parent = this.tcSet;
                    //登录成功后，密码设置为空
                    tbPass.Text = "";
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户1;
                }
                else
                {
                    //信息提示
                    MessageBox.Show("密码错误!", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //登录失败后，密码设置为空
                    tbPass.Text = "";
                    //设置光标位置
                    tbPass.Focus();
                    //设置登录按钮上的图像
                    btnLog.Image = global::WindowsJinKo.Properties.Resources.用户2;
                    //显示结果信息
                    this.tpRun.Parent = this.tcSet;
                    //隐藏参数设置
                    tpModel.Parent = null;
                    //隐藏相机调试
                    tpParam.Parent = null;
                    //隐藏光源调试
                    tpPath.Parent = null;
                }
            }
            else
            {
                //显示结果信息
                this.tpRun.Parent = this.tcSet;
                //隐藏参数设置
                tpModel.Parent = null;
                //隐藏相机调试
                tpParam.Parent = null;
                //隐藏光源调试
                tpPath.Parent = null;
                //信息提示
                MessageBox.Show("登录失败！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //登录失败后，密码设置为空
                tbPass.Text = "";
                //设置光标位置
                tbPass.Focus();
                //设置光标位置
                cbUser.Focus();
                //设置登录按钮上的图像
                btnLog.Image = global::WindowsJinKo.Properties.Resources.用户2;
            }
        }

        //回车确认登录
        private void tbPass_KeyDown(object sender, KeyEventArgs e)
        {
            //绑定Enter键
            if (e.KeyCode == Keys.Enter | e.KeyCode == Keys.Space)
            {
                //触发登录按钮
                this.btnLog_Click(null, null);
            }
        }
    }
}
