using System;
using System.Text;
using HalconDotNet;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging; 

namespace WindowsJinKo
{
    //定义焊点位置
    public class WeldLation
    {
        public HTuple nudWeldX;
        public HTuple nudWeldY;
        public HTuple nudSpaceMain;
        public HTuple nudSpaceSecond;
        public HTuple nudRectW;
        public HTuple nudRectH;
        public HTuple nudRectUp;
        public HTuple nudRectDown;

        public WeldLation()
        { 
            nudWeldX=-570;
            nudWeldY = -390;
            nudSpaceMain = 385;
            nudSpaceSecond = 30;
            nudRectW = 100;
            nudRectH = 90;
            nudRectUp = 50;
            nudRectDown = 110;       
        } 
    }
    
    //定义检测参数
    public class WeldCheck
    {
        public HTuple nudHoleMinHeight;
        public HTuple nudHoleMaxHeight;
        public HTuple nudHoleMinWidth;
        public HTuple nudHoleMaxArea;
        public HTuple nudBulkiness;

        public WeldCheck()
        {
            nudHoleMinHeight = 0;
            nudHoleMaxHeight = 0;
            nudHoleMinWidth = 0;
            nudHoleMaxArea = 0;
            nudBulkiness = 0;
        }  
    }

    //定义模板句柄
    public class ShapeModel
    {
        //模板参数
        public HTuple Model;				//定位模板
        public HObject Shape;			//模板区域
        
        public ShapeModel()
        {
            Model = -1;
            HOperatorSet.GenEmptyObj(out Shape);
        }

        ~ShapeModel()
        {
            ClearShapeModel(ref Model);
            Shape.Dispose();
        }

        public void ClearShapeModel(ref HTuple ModelID)
        {
            if (ModelID > -1)
            {
                HOperatorSet.ClearShapeModel(ModelID);
                ModelID = -1;
            }
        }
    }

    //定义其它参数
    public class Other
    {       
        //缺陷区域显示标志位
        public int DefectRegion;
        //焊点区域显示标志位
        public int ShowWeldRect;
        //接线盒焊接缺陷信号
        public HTuple OutNg;
        //接线盒焊接OK数量
        public Int32 OutOkNum;
        //接线盒焊接NG数量
        public Int32 OutNgNum;
        //接线盒检测总数量
        public Int32 WeldTotalNum;

        public Other()
        {
            DefectRegion = 1;
            ShowWeldRect=1;
            OutNg= 1;
            OutOkNum = 0;
            OutNgNum = 0;
            WeldTotalNum = 0;
        }

    }

    //高清图像显示参数
    public class HDImageDisplay
    {
        public float RowOK;
        public float ColumnOK;
        public float RowNG;
        public float ColumnNG;

        public int  Row1;
        public int Column1;
        public int width;
        public int height;
        public HDImageDisplay()
        { 
           RowOK=0;
           ColumnOK = 0;
           RowNG = 0;
           ColumnNG = 0;

           Row1 = 0;
           Column1 = 0;
           width = 0;
           height = 0;
        }
    }

    public class HalconVision
    {
        #region //单例模式
        private static HalconVision _instance = null;
        public static HalconVision Instance()
        {
            if (_instance == null)
            {
                _instance = new HalconVision();
            }
            return _instance;
        }
        #endregion

        #region //Ini文件读写
        //声明读写INI文件的API函数  
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                    int size, string filePath);
        //复制内存
        [DllImport("Kernel32.dll")]
        private static extern void CopyMemory(int dest, int source, int size);

        //保存参数到ini文件  
        public void IniWriteValue(string Section, string Key, string Value, string Path)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        //读取ini文件中的参数   
        public string IniReadValue(string Section, string Key, string Path)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, Path);
            return temp.ToString();
        }
        #endregion

        #region //实例化定义变量
        public ShapeModel ShapeModel = new ShapeModel();
        public Other Other = new Other();
        public WeldLation WeldLation = new WeldLation();
        public WeldCheck WeldCheck = new WeldCheck();   
        public HDImageDisplay HDImage = new HDImageDisplay();   
        #endregion

        #region //定义文件路径
       //参数路径->数据路径
        public string DatePath = Application.StartupPath + "\\Data\\" + "参数设置.ini";
        //public string RunPath = "D:/C# Demo/WindowsJinKo/WindowsJinKo/bin/Debug/Data/" ;
        public string RunPath = Application.StartupPath + "\\Data\\" ;
       //参数路径->日志路径
       public string LogPath = Application.StartupPath + "\\Log\\" + "操作日志.txt";
       //参数路径->图像路径
       public string ModelPath = Application.StartupPath + "\\Model" ;
       //图像读取路径
       public string hv_ImgPath = "D:/C# Demo/2018年05月25日 白班/焊后正面识别/OK/";
       //结果图像保存路径
       public string ImgResultPath = "D:/Image/";
       #endregion

       //图像处理算法
       public void Weld_Image_Processing(HObject ho_Image, HTuple hv_Model, HTuple hv_WindowHandle, out HTuple hv_OutNgNum) 
       {
           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 
           HObject ho_ImageOut = null, ho_ImageTrans, ho_ImageZoomed1;
           HObject ho_ResultOOK = null, ho_ResultONG = null;

           // Local control variables 
           HTuple hv_S1 = null, hv_Width = null, hv_Height = null,hv_Channels = null;
           HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
           HTuple hv_Error = null, hv_S2 = null, hv_S3 = null, hv_Msg = null, hv_S4 = null;
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_ImageOut);
           HOperatorSet.GenEmptyObj(out ho_ImageTrans);
           HOperatorSet.GenEmptyObj(out ho_ImageZoomed1);
           HOperatorSet.GenEmptyObj(out ho_ResultOOK);
           HOperatorSet.GenEmptyObj(out ho_ResultONG);
           hv_OutNgNum = new HTuple();
           try
           {
               ho_ImageOut.Dispose();
               ho_ImageOut = ho_Image.CopyObj(1, -1);
               HOperatorSet.Rgb1ToGray(ho_ImageOut, out ho_ImageOut);
               HOperatorSet.SetColor(hv_WindowHandle, "red");
               set_display_font(hv_WindowHandle, 24, "mono", "true", "false");
               HOperatorSet.SetLineWidth(hv_WindowHandle, 2);

               HOperatorSet.CountSeconds(out hv_S1);
               HOperatorSet.GetImageSize(ho_ImageOut, out hv_Width, out hv_Height);      
               //清除窗口显示内容 注销
               //HOperatorSet.ClearWindow(hv_WindowHandle);
              
               ho_ImageTrans.Dispose();
               image_rotate(ho_ImageOut, out ho_ImageTrans);
               ho_ImageZoomed1.Dispose();
               HOperatorSet.ZoomImageFactor(ho_ImageTrans, out ho_ImageZoomed1, 0.5, 0.5, "constant");                  
               //匹配位置
               HOperatorSet.CountSeconds(out hv_S1);
               HOperatorSet.FindShapeModel(ho_ImageZoomed1, hv_Model, 0, (new HTuple(360)).TupleRad()
                   , 0.15, 1, 0.3, "least_squares", 0, 0.4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);                
               hv_Row = hv_Row * 2;
               hv_Column = hv_Column * 2;
               

               if ((int)(new HTuple(hv_Error.TupleLessEqual(1))) != 0)
               {
                   HOperatorSet.CountSeconds(out hv_S2);
                   ho_ResultOOK.Dispose(); ho_ResultONG.Dispose();
                   Weld_Recognition(ho_ImageTrans, out ho_ResultOOK, out ho_ResultONG, hv_Row,
                       hv_Column, out hv_OutNgNum, hv_WindowHandle);
                   Show_Weld_Results(ho_ImageTrans, ho_ResultONG, ho_ResultOOK, hv_WindowHandle);
                   HOperatorSet.CountSeconds(out hv_S3);

                   //图像格式转换 保存高清图像
                   Bitmap GaryBitmap = null;
                   HOperatorSet.CountChannels(ho_ImageTrans, out hv_Channels);
                   if ((int)(new HTuple(hv_Channels.TupleEqual(1))) != 0)
                   {
                       //Halcon灰度图转为Bitmap图像
                       HalconGrayToBitmap(ho_ImageTrans, out GaryBitmap);
                   }
                   else
                   {
                       //Halcon彩色图转为Bitmap图像
                       HalconRGBToBitmap(ho_ImageTrans, out GaryBitmap);
                   }
                   //创建一个新图像
                   Image img = GaryBitmap;
                       //如果原图片是索引像素格式之列的，则需要转换
                       if (IsPixelFormatIndexed(img.PixelFormat))
                       {
                           Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                           using (Graphics g = Graphics.FromImage(bmp))
                           {
                               g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                               g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                               g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                               g.DrawImage(img, 0, 0);
                           }
                           HOperatorSet.CountSeconds(out hv_S4);
                           hv_Msg = new HTuple();
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[0] = ("模板匹配时间：" + (((1000 * (hv_S2 - hv_S1))).TupleString(".2f"))) + " ms";
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[1] = ("算法运行时间：" + ((((hv_S3 - hv_S2) * 1000)).TupleString(".2f"))) + " ms";
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[2] = ("图像转换时间：" + ((((hv_S4 - hv_S3) * 1000)).TupleString(".2f"))) + " ms";
                          
                           string[] ParmTime = new string[3];
                           ParmTime[0] = hv_Msg[0];
                           ParmTime[1] = hv_Msg[1];
                           ParmTime[2] = hv_Msg[2];

                           //直接对 bmp 进行水印操作
                           Show_Weld_Results(ho_ImageTrans, ho_ResultONG, ho_ResultOOK, hv_WindowHandle, hv_Msg, bmp);
                       }
                       else //否则直接操作
                       {
                           HOperatorSet.CountSeconds(out hv_S4);
                           hv_Msg = new HTuple();
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[0] = ("模板匹配时间：" + (((1000 * (hv_S2 - hv_S1))).TupleString(".2f"))) + " ms";
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[1] = ("算法运行时间：" + ((((hv_S3 - hv_S2) * 1000)).TupleString(".2f"))) + " ms";
                           if (hv_Msg == null)
                               hv_Msg = new HTuple();
                           hv_Msg[2] = ("图像转换时间：" + ((((hv_S4 - hv_S3) * 1000)).TupleString(".2f"))) + " ms";

                           string[] ParmTime = new string[3];
                           ParmTime[0] = hv_Msg[0];
                           ParmTime[1] = hv_Msg[1];
                           ParmTime[2] = hv_Msg[2];

                           //直接对 img 进行水印操作
                           Show_Weld_Results(ho_ImageTrans, ho_ResultONG, ho_ResultOOK, hv_WindowHandle, ParmTime, GaryBitmap);
                       }
               }

               //获取当前程序目录
               string TimeNow=DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss:ff");
               
               //模板匹配分数 类型转换
               double ModelScore = hv_Error;
               double Score = Convert.ToDouble(ModelScore.ToString("0.000"));

               //模板匹配后Y坐标 类型转换
               double ModelRow = hv_Row;
               double Row = Convert.ToDouble(ModelRow.ToString("0.000"));

               //模板匹配后X坐标 类型转换
               double ModelColumn = hv_Column;
               double Column = Convert.ToDouble(ModelColumn.ToString("0.000"));

               //模板匹配时间 类型转换
               double ModelMatchTime = (hv_S2 - hv_S1) * 1000;
               string MatchTime = ModelMatchTime.ToString("0.000");

               //算法运行时间 类型转换
               double AlgorithmRunTime = (hv_S3 - hv_S2) * 1000;
               string AlgorithmTime = AlgorithmRunTime.ToString("0.000");

               //图像格式转换时间 类型转换
               double HalconToBitmap = (hv_S4 - hv_S3) * 1000;
               string HalconBitmap = HalconToBitmap.ToString("0.000");

               //把运行参数保存到CSV文件中去
               writeCSV(RunPath, TimeNow, Score, Row, Column, MatchTime, AlgorithmTime, HalconBitmap, true);
               
               //显示箭头
               HOperatorSet.DispArrow(hv_WindowHandle, hv_Row, hv_Column, hv_Row - ((hv_Angle.TupleCos()
                   ) * 50), hv_Column - ((hv_Angle.TupleSin()) * 50), 15);

               disp_message(hv_WindowHandle, hv_Msg, "window", 25, 25, "green", "false");
               ho_ImageOut.Dispose();
               ho_ImageTrans.Dispose();
               ho_ImageZoomed1.Dispose();
               ho_ResultOOK.Dispose();
               ho_ResultONG.Dispose();
               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_ImageOut.Dispose();
               ho_ImageTrans.Dispose();
               ho_ImageZoomed1.Dispose();
               ho_ResultOOK.Dispose();
               ho_ResultONG.Dispose();

               throw HDevExpDefaultException;
           }
       }

        //旋转图像
       public void image_rotate(HObject ho_Image, out HObject ho_ImageTrans)
       {
           // Local iconic variables 
           HObject ho_ImageGauss1 = null, ho_ImageScaled = null;
           HObject ho_Region = null, ho_ConnectedRegions = null, ho_SelectedRegions = null;
           HObject ho_RegionFillUp = null, ho_RegionUnion = null, ho_Rectangle2 = null;
           HObject ho_RegionIntersection1 = null, ho_RegionErosion1 = null;
           HObject ho_RegionDilation = null, ho_RegionDifference = null;
           HObject ho_RegionErosion2 = null, ho_RegionDilation1 = null;
           HObject ho_RegionUnion1 = null, ho_RegionUnion2 = null;

           // Local control variables 

           HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
           HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
           HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
           HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
           HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
           HTuple hv_Length21 = new HTuple(), hv_HomMat2D = new HTuple();
           HTuple hv_Exception = null;
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_ImageTrans);
           HOperatorSet.GenEmptyObj(out ho_ImageGauss1);
           HOperatorSet.GenEmptyObj(out ho_ImageScaled);
           HOperatorSet.GenEmptyObj(out ho_Region);
           HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
           HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
           HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
           HOperatorSet.GenEmptyObj(out ho_RegionUnion);
           HOperatorSet.GenEmptyObj(out ho_Rectangle2);
           HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
           HOperatorSet.GenEmptyObj(out ho_RegionErosion1);
           HOperatorSet.GenEmptyObj(out ho_RegionDilation);
           HOperatorSet.GenEmptyObj(out ho_RegionDifference);
           HOperatorSet.GenEmptyObj(out ho_RegionErosion2);
           HOperatorSet.GenEmptyObj(out ho_RegionDilation1);
           HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
           HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
           try
           {
               try
               {
                   //图像高斯处理
                   HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                   ho_ImageGauss1.Dispose();
                   HOperatorSet.GaussImage(ho_Image, out ho_ImageGauss1, 2.5);
                   ho_ImageScaled.Dispose();
                   scale_image_range(ho_ImageGauss1, out ho_ImageScaled, 30, 100);
                   ho_Region.Dispose();
                   HOperatorSet.Threshold(ho_ImageScaled, out ho_Region, 0, 200);
                   ho_ConnectedRegions.Dispose();
                   HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                   ho_SelectedRegions.Dispose();
                   HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", 300000, 999999999);                      
                   ho_RegionFillUp.Dispose();
                   HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionFillUp);
                   ho_RegionUnion.Dispose();
                   HOperatorSet.Union1(ho_RegionFillUp, out ho_RegionUnion);
                   HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11, out hv_Row21, out hv_Column21);                    
                   //提取接线盒上半部分
                   ho_Rectangle2.Dispose();
                   HOperatorSet.GenRectangle1(out ho_Rectangle2, 0, 0, hv_Row21 - (hv_Row21 * 0.5), hv_Width);                    
                   ho_RegionIntersection1.Dispose();
                   HOperatorSet.Intersection(ho_Rectangle2, ho_RegionFillUp, out ho_RegionIntersection1);                      
                   ho_RegionErosion1.Dispose();
                   HOperatorSet.ErosionRectangle1(ho_RegionIntersection1, out ho_RegionErosion1, 251, 151);                   
                   ho_RegionDilation.Dispose();
                   HOperatorSet.DilationRectangle1(ho_RegionErosion1, out ho_RegionDilation, 251, 151);                    
                   //提取接线盒下半部分
                   ho_RegionDifference.Dispose();
                   HOperatorSet.Difference(ho_RegionFillUp, ho_Rectangle2, out ho_RegionDifference);                      
                   ho_RegionErosion2.Dispose();
                   HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion2, 25, 25);                     
                   ho_RegionDilation1.Dispose();
                   HOperatorSet.DilationRectangle1(ho_RegionErosion2, out ho_RegionDilation1, 25, 25);                      
                   //合并接线盒上下部分
                   ho_RegionUnion1.Dispose();
                   HOperatorSet.Union2(ho_RegionDilation, ho_RegionDilation1, out ho_RegionUnion1);                       
                   ho_RegionUnion2.Dispose();
                   HOperatorSet.Union1(ho_RegionUnion1, out ho_RegionUnion2);
                   HOperatorSet.SmallestRectangle2(ho_RegionUnion2, out hv_Row3, out hv_Column3,
                       out hv_Phi1, out hv_Length11, out hv_Length21);
                   if ((int)((new HTuple(hv_Phi1.TupleLess(-0.5))).TupleOr(new HTuple(hv_Phi1.TupleGreater(
                       0.5)))) != 0)
                   {
                       //从点和角度计算一个刚性的仿射变换
                       HOperatorSet.VectorAngleToRigid(hv_Row3, hv_Column3, hv_Phi1, hv_Row3,
                           hv_Column3, (new HTuple(180)).TupleRad(), out hv_HomMat2D);
                   }
                   else
                   {
                       //从点和角度计算一个刚性的仿射变换
                       HOperatorSet.VectorAngleToRigid(hv_Row3, hv_Column3, hv_Phi1, hv_Row3,
                           hv_Column3, (new HTuple(0)).TupleRad(), out hv_HomMat2D);
                   }
                   ho_ImageTrans.Dispose();
                   HOperatorSet.AffineTransImage(ho_ImageGauss1, out ho_ImageTrans, hv_HomMat2D,
                       "constant", "false");
               }
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   ho_ImageTrans.Dispose();
                   HOperatorSet.CopyObj(ho_ImageGauss1, out ho_ImageTrans, 1, 1);
               }
               ho_ImageGauss1.Dispose();
               ho_ImageScaled.Dispose();
               ho_Region.Dispose();
               ho_ConnectedRegions.Dispose();
               ho_SelectedRegions.Dispose();
               ho_RegionFillUp.Dispose();
               ho_RegionUnion.Dispose();
               ho_Rectangle2.Dispose();
               ho_RegionIntersection1.Dispose();
               ho_RegionErosion1.Dispose();
               ho_RegionDilation.Dispose();
               ho_RegionDifference.Dispose();
               ho_RegionErosion2.Dispose();
               ho_RegionDilation1.Dispose();
               ho_RegionUnion1.Dispose();
               ho_RegionUnion2.Dispose();

               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_ImageGauss1.Dispose();
               ho_ImageScaled.Dispose();
               ho_Region.Dispose();
               ho_ConnectedRegions.Dispose();
               ho_SelectedRegions.Dispose();
               ho_RegionFillUp.Dispose();
               ho_RegionUnion.Dispose();
               ho_Rectangle2.Dispose();
               ho_RegionIntersection1.Dispose();
               ho_RegionErosion1.Dispose();
               ho_RegionDilation.Dispose();
               ho_RegionDifference.Dispose();
               ho_RegionErosion2.Dispose();
               ho_RegionDilation1.Dispose();
               ho_RegionUnion1.Dispose();
               ho_RegionUnion2.Dispose();
               throw HDevExpDefaultException;
           }
       }
      
        //结果显示
       public void Show_Weld_Results(HObject ho_Image_AffinTrans, HObject ho_ResultONG,
                                     HObject ho_ResultOOK, HTuple hv_WindowHandle)
       {
           // Local iconic variables 

           HObject ho_SelectedObjectOK = null, ho_SelectedObjectNG = null;

           // Local control variables 

           HTuple hv_NumOK = null, hv_NumNG = null, hv_j = null;
           HTuple hv_RowOK = new HTuple(), hv_ColumnOK = new HTuple();
           HTuple hv_Radius = new HTuple(), hv_k = null, hv_RowNG = new HTuple();
           HTuple hv_ColumnNG = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_SelectedObjectOK);
           HOperatorSet.GenEmptyObj(out ho_SelectedObjectNG);
           try
           { 
               //HOperatorSet.DispObj(ho_Image_AffinTrans, hv_WindowHandle);         
               HOperatorSet.SetColor(hv_WindowHandle, "red");      
               HOperatorSet.DispObj(ho_ResultONG, hv_WindowHandle);       
               HOperatorSet.SetColor(hv_WindowHandle, "green");           
               HOperatorSet.DispObj(ho_ResultOOK, hv_WindowHandle);
               HOperatorSet.CountObj(ho_ResultOOK, out hv_NumOK);
               HOperatorSet.CountObj(ho_ResultONG, out hv_NumNG);

               HTuple end_val8 = hv_NumOK;
               HTuple step_val8 = 1;
               for (hv_j = 1; hv_j.Continue(end_val8, step_val8); hv_j = hv_j.TupleAdd(step_val8))
               {             
                   HOperatorSet.SetColor(hv_WindowHandle, "green");                 
                   ho_SelectedObjectOK.Dispose();
                   HOperatorSet.SelectObj(ho_ResultOOK, out ho_SelectedObjectOK, hv_j);
                   HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK, out hv_Radius);                     
                   HOperatorSet.SetTposition(hv_WindowHandle, hv_RowOK + 200, hv_ColumnOK - 50);
                   HOperatorSet.WriteString(hv_WindowHandle, "OK");
               }

               HTuple end_val16 = hv_NumNG;
               HTuple step_val16 = 1;
               for (hv_k = 1; hv_k.Continue(end_val16, step_val16); hv_k = hv_k.TupleAdd(step_val16))
               {               
                   HOperatorSet.SetColor(hv_WindowHandle, "red");               
                   ho_SelectedObjectNG.Dispose();
                   HOperatorSet.SelectObj(ho_ResultONG, out ho_SelectedObjectNG, hv_k);
                   HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG, out hv_ColumnNG, out hv_Radius);                      
                   HOperatorSet.SetTposition(hv_WindowHandle, hv_RowNG + 200, hv_ColumnNG - 50);
                   HOperatorSet.WriteString(hv_WindowHandle, "NG");
               }
               ho_SelectedObjectOK.Dispose();
               ho_SelectedObjectNG.Dispose();
               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_SelectedObjectOK.Dispose();
               ho_SelectedObjectNG.Dispose();
               throw HDevExpDefaultException;
           }
       }       
       
        //接线盒缺陷识别检测算法
       public void Weld_Recognition(HObject ho_Image_AffinTrans, out HObject ho_ResultOOK,
                                    out HObject ho_ResultONG, HTuple hv_row_2, HTuple hv_column_2, out HTuple hv_OutNgNum,HTuple hv_WindowHandle)
       {
           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_ConnectedRegions, ho_RegionsSelected;
           HObject ho_Regions, ho_ImageAffin, ho_ImageScaled1, ho_ImageScaled2;
           HObject ho_ScaledWeld1, ho_ConnectedWeld1, ho_SelectedRegions1;
           HObject ho_ScaledWeld2, ho_ConnectedWeld2, ho_SelectedRegions2;
           HObject ho_RegionUnion = null, ho_Rectangle = null, ho_Rectangle1 = null;
           HObject ho_Rect = null, ho_ImageReduced1 = null, ho_ImageReduced2 = null;
           HObject ho_ImageReduced3 = null, ho_RegionReduced1 = null, ho_RegionReduced2 = null;
           HObject ho_RegionReduced3 = null, ho_RegionIntersection = null;
           HObject ho_RegionErosion = null, ho_SelectedRegions = null;

           // Local control variables 

           HTuple hv_WeldRow = null, hv_WeldColumn = null;
           HTuple hv_WeldSpace = null, hv_WeldTempSpace = null, hv_Width = null;
           HTuple hv_Height = null, hv_Thresh1 = null, hv_Thresh2 = null;
           HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
           HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
           HTuple hv_Exception = null, hv_I = null, hv_Row1 = new HTuple();
           HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
           HTuple hv_Column2 = new HTuple(), hv_h_row = new HTuple();
           HTuple hv_cr_w = new HTuple(), hv_Number = new HTuple();
           HTuple hv_UThresh1 = new HTuple(), hv_UThresh2 = new HTuple();
           HTuple hv_UThresh3 = new HTuple(), hv_Num1 = new HTuple();
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_ResultOOK);
           HOperatorSet.GenEmptyObj(out ho_ResultONG);
           HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
           HOperatorSet.GenEmptyObj(out ho_RegionsSelected);
           HOperatorSet.GenEmptyObj(out ho_Regions);
           HOperatorSet.GenEmptyObj(out ho_ImageAffin);
           HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
           HOperatorSet.GenEmptyObj(out ho_ImageScaled2);
           HOperatorSet.GenEmptyObj(out ho_ScaledWeld1);
           HOperatorSet.GenEmptyObj(out ho_ConnectedWeld1);
           HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
           HOperatorSet.GenEmptyObj(out ho_ScaledWeld2);
           HOperatorSet.GenEmptyObj(out ho_ConnectedWeld2);
           HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
           HOperatorSet.GenEmptyObj(out ho_RegionUnion);
           HOperatorSet.GenEmptyObj(out ho_Rectangle);
           HOperatorSet.GenEmptyObj(out ho_Rectangle1);
           HOperatorSet.GenEmptyObj(out ho_Rect);
           HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
           HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
           HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
           HOperatorSet.GenEmptyObj(out ho_RegionReduced1);
           HOperatorSet.GenEmptyObj(out ho_RegionReduced2);
           HOperatorSet.GenEmptyObj(out ho_RegionReduced3);
           HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
           HOperatorSet.GenEmptyObj(out ho_RegionErosion);
           HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
           try
           {
               //输出缺陷
               hv_OutNgNum = 0;
               //起始位置Y
               hv_WeldRow = WeldLation.nudWeldY;
               //起始位置X
               hv_WeldColumn = WeldLation.nudWeldX;
               //每个焊接位置的间距
               hv_WeldSpace = WeldLation.nudSpaceMain;
               //每次偏移位置累加
               hv_WeldTempSpace = hv_WeldColumn + 0;

               HOperatorSet.DispObj(ho_Image_AffinTrans, hv_WindowHandle); 
               ho_ResultOOK.Dispose();
               HOperatorSet.GenEmptyObj(out ho_ResultOOK);
               ho_ResultONG.Dispose();
               HOperatorSet.GenEmptyObj(out ho_ResultONG);
               ho_ConnectedRegions.Dispose();
               HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
               ho_RegionsSelected.Dispose();
               HOperatorSet.GenEmptyObj(out ho_RegionsSelected);
               ho_Regions.Dispose();
               HOperatorSet.GenEmptyObj(out ho_Regions);
               HOperatorSet.GetImageSize(ho_Image_AffinTrans, out hv_Width, out hv_Height);
               //待定
               ho_ImageAffin.Dispose();
               HOperatorSet.Emphasize(ho_Image_AffinTrans, out ho_ImageAffin, hv_Width, hv_Height, 2);                 
               ho_ImageScaled1.Dispose();
               scale_image_range(ho_ImageAffin, out ho_ImageScaled1, 1, 75);
               ho_ImageScaled2.Dispose();
               scale_image_range(ho_ImageAffin, out ho_ImageScaled2, 90, 1);

               //提取四焊点区域
               ho_ScaledWeld1.Dispose();
               HOperatorSet.BinaryThreshold(ho_ImageScaled1, out ho_ScaledWeld1, "max_separability", "light", out hv_Thresh1);                 
               ho_ConnectedWeld1.Dispose();
               HOperatorSet.Connection(ho_ScaledWeld1, out ho_ConnectedWeld1);
               ho_SelectedRegions1.Dispose();
               HOperatorSet.SelectShape(ho_ConnectedWeld1, out ho_SelectedRegions1, ((new HTuple("area")).TupleConcat(
                   "height")).TupleConcat("width"), "and", ((new HTuple(240000)).TupleConcat(
                   200)).TupleConcat(1100), ((new HTuple(350000)).TupleConcat(350)).TupleConcat(1800));
               ho_ScaledWeld2.Dispose();
               HOperatorSet.BinaryThreshold(ho_ImageScaled2, out ho_ScaledWeld2, "max_separability", "dark", out hv_Thresh2);            
               ho_ConnectedWeld2.Dispose();
               HOperatorSet.Connection(ho_ScaledWeld2, out ho_ConnectedWeld2);
               ho_SelectedRegions2.Dispose();
               HOperatorSet.SelectShape(ho_ConnectedWeld2, out ho_SelectedRegions2, ((new HTuple("area")).TupleConcat(
                   "height")).TupleConcat("width"), "and", ((new HTuple(240000)).TupleConcat(
                   200)).TupleConcat(1100), ((new HTuple(350000)).TupleConcat(350)).TupleConcat(1800));                 
               try
               {
                   ho_RegionUnion.Dispose();
                   HOperatorSet.Union2(ho_SelectedRegions1, ho_SelectedRegions2, out ho_RegionUnion);
                   HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11, out hv_Row21, out hv_Column21);               
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row11 + WeldLation.nudRectUp, hv_Column11, hv_Row11 + WeldLation.nudRectDown, hv_Column21);
                  //显示提取的4个焊点的大矩形
                   if (Other.ShowWeldRect==1)
                   {
                       HOperatorSet.DispObj(ho_Rectangle, hv_WindowHandle);   
                   }
                   
               }
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   ho_Rectangle.Dispose();
                   HOperatorSet.GenEmptyObj(out ho_Rectangle);
               }
               for (hv_I = 0; (int)hv_I <= 3; hv_I = (int)hv_I + 1)
               {
                   ho_Rectangle1.Dispose();
                   HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_WeldRow, hv_column_2 + hv_WeldTempSpace,
                       0, WeldLation.nudRectW, WeldLation.nudRectH);
                   //测试位置
                   if ((int)(new HTuple(hv_I.TupleEqual(1))) != 0)
                   {
                       hv_WeldTempSpace = (hv_WeldTempSpace + hv_WeldSpace) + WeldLation.nudSpaceSecond;
                   }
                   else
                   {
                       hv_WeldTempSpace = hv_WeldTempSpace + hv_WeldSpace;
                   }
                   HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);                    
                   hv_h_row = hv_Row2 - hv_Row1;
                   hv_cr_w = hv_Column2 - hv_Column1;
                   HOperatorSet.CountObj(ho_Rectangle, out hv_Number);
                   if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                   {
                       ho_Rect.Dispose();
                       HOperatorSet.Intersection(ho_Rectangle, ho_Rectangle1, out ho_Rect);
                   }
                   else
                   {
                       ho_Rect.Dispose();
                       HOperatorSet.Intersection(ho_Rectangle1, ho_Rectangle1, out ho_Rect);
                   }
                   ho_ImageReduced1.Dispose();
                   HOperatorSet.ReduceDomain(ho_ImageScaled1, ho_Rect, out ho_ImageReduced1);
                   ho_ImageReduced2.Dispose();
                   HOperatorSet.ReduceDomain(ho_ImageScaled2, ho_Rect, out ho_ImageReduced2);
                   ho_ImageReduced3.Dispose();
                   HOperatorSet.ReduceDomain(ho_ImageAffin, ho_Rect, out ho_ImageReduced3);
                   //**********************************************
                   //提取漏焊大孔位
                   ho_RegionReduced1.Dispose();
                   HOperatorSet.BinaryThreshold(ho_ImageReduced1, out ho_RegionReduced1, "max_separability",
                       "dark", out hv_UThresh1);
                   ho_RegionReduced2.Dispose();
                   HOperatorSet.BinaryThreshold(ho_ImageReduced2, out ho_RegionReduced2, "max_separability",
                       "light", out hv_UThresh2);
                   ho_RegionReduced3.Dispose();
                   HOperatorSet.BinaryThreshold(ho_ImageReduced3, out ho_RegionReduced3, "max_separability",
                       "dark", out hv_UThresh3);
                   ho_RegionIntersection.Dispose();
                   HOperatorSet.Intersection(ho_RegionReduced1, ho_RegionReduced2, out ho_RegionIntersection);
                   HOperatorSet.OpeningRectangle1(ho_RegionIntersection, out ho_RegionIntersection, 1, 8);
                   HOperatorSet.FillUp(ho_RegionReduced3, out ho_RegionReduced3);                 
                   ho_RegionErosion.Dispose();
                   HOperatorSet.ErosionRectangle1(ho_RegionReduced3, out ho_RegionErosion, 15,15);
                   HOperatorSet.DilationRectangle1(ho_RegionErosion, out ho_RegionErosion, 15,15);
                   HOperatorSet.Union2(ho_RegionErosion, ho_RegionIntersection, out ho_RegionIntersection);
                   ho_ConnectedRegions.Dispose();
                   HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);
                   //提取目标特征
                   ho_SelectedRegions.Dispose();
                   HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions,"max_area", 70);
                   HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "area", "and", WeldCheck.nudHoleMaxArea, 999999);
                   HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "height", "and", WeldCheck.nudHoleMinHeight, WeldCheck.nudHoleMaxHeight);
                   HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "width", "and", WeldCheck.nudHoleMinWidth, 99999);
                   HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "bulkiness", "and", 0, WeldCheck.nudBulkiness);
                   HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num1);
                   if ((int)(new HTuple(hv_Num1.TupleGreater(0))) != 0)
                   {                  
                       HOperatorSet.ConcatObj(ho_ResultONG, ho_Rectangle1, out ho_ResultONG);
                       hv_OutNgNum = 1;
                   }
                   else
                   {
                       HOperatorSet.ConcatObj(ho_ResultOOK, ho_Rectangle1, out ho_ResultOOK);                    
                   }
                   HOperatorSet.ConcatObj(ho_Regions, ho_SelectedRegions, out ho_Regions);
                   
                   if (Other.DefectRegion == 1)
                   {
                       HOperatorSet.DispObj(ho_Regions, hv_WindowHandle);  
                   }
                   
               }
               ho_ConnectedRegions.Dispose();
               ho_RegionsSelected.Dispose();
               ho_Regions.Dispose();
               ho_ImageAffin.Dispose();
               ho_ImageScaled1.Dispose();
               ho_ImageScaled2.Dispose();
               ho_ScaledWeld1.Dispose();
               ho_ConnectedWeld1.Dispose();
               ho_SelectedRegions1.Dispose();
               ho_ScaledWeld2.Dispose();
               ho_ConnectedWeld2.Dispose();
               ho_SelectedRegions2.Dispose();
               ho_RegionUnion.Dispose();
               ho_Rectangle.Dispose();
               ho_Rectangle1.Dispose();
               ho_Rect.Dispose();
               ho_ImageReduced1.Dispose();
               ho_ImageReduced2.Dispose();
               ho_ImageReduced3.Dispose();
               ho_RegionReduced1.Dispose();
               ho_RegionReduced2.Dispose();
               ho_RegionReduced3.Dispose();
               ho_RegionIntersection.Dispose();
               ho_RegionErosion.Dispose();
               ho_SelectedRegions.Dispose();

               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_ConnectedRegions.Dispose();
               ho_RegionsSelected.Dispose();
               ho_Regions.Dispose();
               ho_ImageAffin.Dispose();
               ho_ImageScaled1.Dispose();
               ho_ImageScaled2.Dispose();
               ho_ScaledWeld1.Dispose();
               ho_ConnectedWeld1.Dispose();
               ho_SelectedRegions1.Dispose();
               ho_ScaledWeld2.Dispose();
               ho_ConnectedWeld2.Dispose();
               ho_SelectedRegions2.Dispose();
               ho_RegionUnion.Dispose();
               ho_Rectangle.Dispose();
               ho_Rectangle1.Dispose();
               ho_Rect.Dispose();
               ho_ImageReduced1.Dispose();
               ho_ImageReduced2.Dispose();
               ho_ImageReduced3.Dispose();
               ho_RegionReduced1.Dispose();
               ho_RegionReduced2.Dispose();
               ho_RegionReduced3.Dispose();
               ho_RegionIntersection.Dispose();
               ho_RegionErosion.Dispose();
               ho_SelectedRegions.Dispose();

               throw HDevExpDefaultException;
           }
       }

        //设置字体大小
       public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
                                     HTuple hv_Bold, HTuple hv_Slant)
       {
           // Local iconic variables 

           // Local control variables 

           HTuple hv_OS = null, hv_BufferWindowHandle = new HTuple();
           HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
           HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
           HTuple hv_Scale = new HTuple(), hv_Exception = new HTuple();
           HTuple hv_SubFamily = new HTuple(), hv_Fonts = new HTuple();
           HTuple hv_SystemFonts = new HTuple(), hv_Guess = new HTuple();
           HTuple hv_I = new HTuple(), hv_Index = new HTuple(), hv_AllowedFontSizes = new HTuple();
           HTuple hv_Distances = new HTuple(), hv_Indices = new HTuple();
           HTuple hv_FontSelRegexp = new HTuple(), hv_FontsCourier = new HTuple();
           HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
           HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
           HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
           HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();

           // Initialize local and output iconic variables 
           //This procedure sets the text font of the current window with
           //the specified attributes.
           //It is assumed that following fonts are installed on the system:
           //Windows: Courier New, Arial Times New Roman
           //Mac OS X: CourierNewPS, Arial, TimesNewRomanPS
           //Linux: courier, helvetica, times
           //Because fonts are displayed smaller on Linux than on Windows,
           //a scaling factor of 1.25 is used the get comparable results.
           //For Linux, only a limited number of font sizes is supported,
           //to get comparable results, it is recommended to use one of the
           //following sizes: 9, 11, 14, 16, 20, 27
           //(which will be mapped internally on Linux systems to 11, 14, 17, 20, 25, 34)
           //
           //Input parameters:
           //WindowHandle: The graphics window for which the font will be set
           //Size: The font size. If Size=-1, the default of 16 is used.
           //Bold: If set to 'true', a bold font is used
           //Slant: If set to 'true', a slanted font is used
           //
           HOperatorSet.GetSystem("operating_system", out hv_OS);
           // dev_get_preferences(...); only in hdevelop
           // dev_set_preferences(...); only in hdevelop
           if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
               new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
           {
               hv_Size_COPY_INP_TMP = 16;
           }
           if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
           {
               //Set font on Windows systems
               try
               {
                   //Check, if font scaling is switched on
                   HOperatorSet.OpenWindow(0, 0, 256, 256, 0, "buffer", "", out hv_BufferWindowHandle);
                   HOperatorSet.SetFont(hv_BufferWindowHandle, "-Consolas-16-*-0-*-*-1-");
                   HOperatorSet.GetStringExtents(hv_BufferWindowHandle, "test_string", out hv_Ascent,
                       out hv_Descent, out hv_Width, out hv_Height);
                   //Expected width is 110
                   hv_Scale = 110.0 / hv_Width;
                   hv_Size_COPY_INP_TMP = ((hv_Size_COPY_INP_TMP * hv_Scale)).TupleInt();
                   HOperatorSet.CloseWindow(hv_BufferWindowHandle);
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   //throw (Exception)
               }
               if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                   new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Courier New";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
               {
                   hv_Font_COPY_INP_TMP = "Consolas";
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
                   //throw (Exception)
               }
           }
           else if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Dar"))) != 0)
           {
               //Set font on Mac OS X systems. Since OS X does not have a strict naming
               //scheme for font attributes, we use tables to determine the correct font
               //name.
               hv_SubFamily = 0;
               if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_SubFamily = hv_SubFamily.TupleBor(1);
               }
               else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
               {
                   hv_Exception = "Wrong value of control parameter Slant";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
               {
                   hv_SubFamily = hv_SubFamily.TupleBor(2);
               }
               else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
               {
                   hv_Exception = "Wrong value of control parameter Bold";
                   throw new HalconException(hv_Exception);
               }
               if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "Menlo-Regular";
                   hv_Fonts[1] = "Menlo-Italic";
                   hv_Fonts[2] = "Menlo-Bold";
                   hv_Fonts[3] = "Menlo-BoldItalic";
               }
               else if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                   new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "CourierNewPSMT";
                   hv_Fonts[1] = "CourierNewPS-ItalicMT";
                   hv_Fonts[2] = "CourierNewPS-BoldMT";
                   hv_Fonts[3] = "CourierNewPS-BoldItalicMT";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "ArialMT";
                   hv_Fonts[1] = "Arial-ItalicMT";
                   hv_Fonts[2] = "Arial-BoldMT";
                   hv_Fonts[3] = "Arial-BoldItalicMT";
               }
               else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
               {
                   hv_Fonts = new HTuple();
                   hv_Fonts[0] = "TimesNewRomanPSMT";
                   hv_Fonts[1] = "TimesNewRomanPS-ItalicMT";
                   hv_Fonts[2] = "TimesNewRomanPS-BoldMT";
                   hv_Fonts[3] = "TimesNewRomanPS-BoldItalicMT";
               }
               else
               {
                   //Attempt to figure out which of the fonts installed on the system
                   //the user could have meant.
                   HOperatorSet.QueryFont(hv_WindowHandle, out hv_SystemFonts);
                   hv_Fonts = new HTuple();
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP);
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Regular");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "MT");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[0] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of slanted font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Italic");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-ItalicMT");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Oblique");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[1] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of bold font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Bold");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldMT");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[2] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
                   //Guess name of bold slanted font
                   hv_Guess = new HTuple();
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalic");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalicMT");
                   hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldOblique");
                   for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                   {
                       HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                       if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                       {
                           if (hv_Fonts == null)
                               hv_Fonts = new HTuple();
                           hv_Fonts[3] = hv_Guess.TupleSelect(hv_I);
                           break;
                       }
                   }
               }
               hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_SubFamily);
               try
               {
                   HOperatorSet.SetFont(hv_WindowHandle, (hv_Font_COPY_INP_TMP + "-") + hv_Size_COPY_INP_TMP);
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   //throw (Exception)
               }
           }
           else
           {
               //Set font for UNIX systems
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
                   if ((int)((new HTuple(((hv_OS.TupleSubstr(0, 4))).TupleEqual("Linux"))).TupleAnd(
                       new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                   {
                       HOperatorSet.QueryFont(hv_WindowHandle, out hv_Fonts);
                       hv_FontSelRegexp = (("^-[^-]*-[^-]*[Cc]ourier[^-]*-" + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP;
                       hv_FontsCourier = ((hv_Fonts.TupleRegexpSelect(hv_FontSelRegexp))).TupleRegexpMatch(
                           hv_FontSelRegexp);
                       if ((int)(new HTuple((new HTuple(hv_FontsCourier.TupleLength())).TupleEqual(
                           0))) != 0)
                       {
                           hv_Exception = "Wrong font name";
                           //throw (Exception)
                       }
                       else
                       {
                           try
                           {
                               HOperatorSet.SetFont(hv_WindowHandle, (((hv_FontsCourier.TupleSelect(
                                   0)) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                           }
                           // catch (Exception) 
                           catch (HalconException HDevExpDefaultException2)
                           {
                               HDevExpDefaultException2.ToHTuple(out hv_Exception);
                               //throw (Exception)
                           }
                       }
                   }
                   //throw (Exception)
               }
           }
           // dev_set_preferences(...); only in hdevelop

           return;
       }
       
        //图像拉伸
       public void scale_image_range(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Min, HTuple hv_Max)
       {
           // Stack for temporary objects 
           HObject[] OTemp = new HObject[20];

           // Local iconic variables 

           HObject ho_SelectedChannel = null, ho_LowerRegion = null;
           HObject ho_UpperRegion = null;

           // Local copy input parameter variables 
           HObject ho_Image_COPY_INP_TMP;
           ho_Image_COPY_INP_TMP = ho_Image.CopyObj(1, -1);



           // Local control variables 

           HTuple hv_LowerLimit = new HTuple(), hv_UpperLimit = new HTuple();
           HTuple hv_Mult = null, hv_Add = null, hv_Channels = null;
           HTuple hv_Index = null, hv_MinGray = new HTuple(), hv_MaxGray = new HTuple();
           HTuple hv_Range = new HTuple();
           HTuple hv_Max_COPY_INP_TMP = hv_Max.Clone();
           HTuple hv_Min_COPY_INP_TMP = hv_Min.Clone();

           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_ImageScaled);
           HOperatorSet.GenEmptyObj(out ho_SelectedChannel);
           HOperatorSet.GenEmptyObj(out ho_LowerRegion);
           HOperatorSet.GenEmptyObj(out ho_UpperRegion);
           try
           {
               //Convenience procedure to scale the gray values of the
               //input image Image from the interval [Min,Max]
               //to the interval [0,255] (default).
               //Gray values < 0 or > 255 (after scaling) are clipped.
               //
               //If the image shall be scaled to an interval different from [0,255],
               //this can be achieved by passing tuples with 2 values [From, To]
               //as Min and Max.
               //Example:
               //scale_image_range(Image:ImageScaled:[100,50],[200,250])
               //maps the gray values of Image from the interval [100,200] to [50,250].
               //All other gray values will be clipped.
               //
               //input parameters:
               //Image: the input image
               //Min: the minimum gray value which will be mapped to 0
               //     If a tuple with two values is given, the first value will
               //     be mapped to the second value.
               //Max: The maximum gray value which will be mapped to 255
               //     If a tuple with two values is given, the first value will
               //     be mapped to the second value.
               //
               //output parameter:
               //ImageScale: the resulting scaled image
               //
               if ((int)(new HTuple((new HTuple(hv_Min_COPY_INP_TMP.TupleLength())).TupleEqual(
                   2))) != 0)
               {
                   hv_LowerLimit = hv_Min_COPY_INP_TMP[1];
                   hv_Min_COPY_INP_TMP = hv_Min_COPY_INP_TMP[0];
               }
               else
               {
                   hv_LowerLimit = 0.0;
               }
               if ((int)(new HTuple((new HTuple(hv_Max_COPY_INP_TMP.TupleLength())).TupleEqual(
                   2))) != 0)
               {
                   hv_UpperLimit = hv_Max_COPY_INP_TMP[1];
                   hv_Max_COPY_INP_TMP = hv_Max_COPY_INP_TMP[0];
               }
               else
               {
                   hv_UpperLimit = 255.0;
               }
               //
               //Calculate scaling parameters
               hv_Mult = (((hv_UpperLimit - hv_LowerLimit)).TupleReal()) / (hv_Max_COPY_INP_TMP - hv_Min_COPY_INP_TMP);
               hv_Add = ((-hv_Mult) * hv_Min_COPY_INP_TMP) + hv_LowerLimit;
               //
               //Scale image
               {
                   HObject ExpTmpOutVar_0;
                   HOperatorSet.ScaleImage(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, hv_Mult,
                       hv_Add);
                   ho_Image_COPY_INP_TMP.Dispose();
                   ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
               }
               //
               //Clip gray values if necessary
               //This must be done for each channel separately
               HOperatorSet.CountChannels(ho_Image_COPY_INP_TMP, out hv_Channels);
               HTuple end_val48 = hv_Channels;
               HTuple step_val48 = 1;
               for (hv_Index = 1; hv_Index.Continue(end_val48, step_val48); hv_Index = hv_Index.TupleAdd(step_val48))
               {
                   ho_SelectedChannel.Dispose();
                   HOperatorSet.AccessChannel(ho_Image_COPY_INP_TMP, out ho_SelectedChannel,
                       hv_Index);
                   HOperatorSet.MinMaxGray(ho_SelectedChannel, ho_SelectedChannel, 0, out hv_MinGray,
                       out hv_MaxGray, out hv_Range);
                   ho_LowerRegion.Dispose();
                   HOperatorSet.Threshold(ho_SelectedChannel, out ho_LowerRegion, ((hv_MinGray.TupleConcat(
                       hv_LowerLimit))).TupleMin(), hv_LowerLimit);
                   ho_UpperRegion.Dispose();
                   HOperatorSet.Threshold(ho_SelectedChannel, out ho_UpperRegion, hv_UpperLimit,
                       ((hv_UpperLimit.TupleConcat(hv_MaxGray))).TupleMax());
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.PaintRegion(ho_LowerRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                           hv_LowerLimit, "fill");
                       ho_SelectedChannel.Dispose();
                       ho_SelectedChannel = ExpTmpOutVar_0;
                   }
                   {
                       HObject ExpTmpOutVar_0;
                       HOperatorSet.PaintRegion(ho_UpperRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                           hv_UpperLimit, "fill");
                       ho_SelectedChannel.Dispose();
                       ho_SelectedChannel = ExpTmpOutVar_0;
                   }
                   if ((int)(new HTuple(hv_Index.TupleEqual(1))) != 0)
                   {
                       ho_ImageScaled.Dispose();
                       HOperatorSet.CopyObj(ho_SelectedChannel, out ho_ImageScaled, 1, 1);
                   }
                   else
                   {
                       {
                           HObject ExpTmpOutVar_0;
                           HOperatorSet.AppendChannel(ho_ImageScaled, ho_SelectedChannel, out ExpTmpOutVar_0
                               );
                           ho_ImageScaled.Dispose();
                           ho_ImageScaled = ExpTmpOutVar_0;
                       }
                   }
               }
               ho_Image_COPY_INP_TMP.Dispose();
               ho_SelectedChannel.Dispose();
               ho_LowerRegion.Dispose();
               ho_UpperRegion.Dispose();

               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_Image_COPY_INP_TMP.Dispose();
               ho_SelectedChannel.Dispose();
               ho_LowerRegion.Dispose();
               ho_UpperRegion.Dispose();

               throw HDevExpDefaultException;
           }
       }
      
        //消息显示
       public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
                                HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
       {
           // Local iconic variables 

           // Local control variables 

           HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
           HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
           HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
           HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
           HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
           HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
           HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
           HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
           HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
           HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
           HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
           HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
           HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
           HTuple hv_CurrentColor = new HTuple();
           HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
           HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
           HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
           HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
           HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

           // Initialize local and output iconic variables 
           //This procedure displays text in a graphics window.
           //
           //Input parameters:
           //WindowHandle: The WindowHandle of the graphics window, where
           //   the message should be displayed
           //String: A tuple of strings containing the text message to be displayed
           //CoordSystem: If set to 'window', the text position is given
           //   with respect to the window coordinate system.
           //   If set to 'image', image coordinates are used.
           //   (This may be useful in zoomed images.)
           //Row: The row coordinate of the desired text position
           //   If set to -1, a default value of 12 is used.
           //Column: The column coordinate of the desired text position
           //   If set to -1, a default value of 12 is used.
           //Color: defines the color of the text as string.
           //   If set to [], '' or 'auto' the currently set color is used.
           //   If a tuple of strings is passed, the colors are used cyclically
           //   for each new textline.
           //Box: If Box[0] is set to 'true', the text is written within an orange box.
           //     If set to' false', no box is displayed.
           //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
           //       the text is written in a box of that color.
           //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
           //       'true' -> display a shadow in a default color
           //       'false' -> display no shadow (same as if no second value is given)
           //       otherwise -> use given string as color string for the shadow color
           //
           //Prepare window
           HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
           HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
               out hv_Column2Part);
           HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
               out hv_WidthWin, out hv_HeightWin);
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
           HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
               out hv_MaxWidth, out hv_MaxHeight);
           if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
           {
               hv_R1 = hv_Row_COPY_INP_TMP.Clone();
               hv_C1 = hv_Column_COPY_INP_TMP.Clone();
           }
           else
           {
               //Transform image to window coordinates
               hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
               hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
               hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
               hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
           }
           //
           //Display text box depending on text size
           hv_UseShadow = 1;
           hv_ShadowColor = "gray";
           if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
           {
               if (hv_Box_COPY_INP_TMP == null)
                   hv_Box_COPY_INP_TMP = new HTuple();
               hv_Box_COPY_INP_TMP[0] = "#fce9d4";
               hv_ShadowColor = "#f28d26";
           }
           if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
               1))) != 0)
           {
               if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
               {
                   //Use default ShadowColor set above
               }
               else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                   "false"))) != 0)
               {
                   hv_UseShadow = 0;
               }
               else
               {
                   hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                   //Valid color?
                   try
                   {
                       HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                           1));
                   }
                   // catch (Exception) 
                   catch (HalconException HDevExpDefaultException1)
                   {
                       HDevExpDefaultException1.ToHTuple(out hv_Exception);
                       hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                       throw new HalconException(hv_Exception);
                   }
               }
           }
           if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
           {
               //Valid color?
               try
               {
                   HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
               }
               // catch (Exception) 
               catch (HalconException HDevExpDefaultException1)
               {
                   HDevExpDefaultException1.ToHTuple(out hv_Exception);
                   hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                   throw new HalconException(hv_Exception);
               }
               //Calculate box extents
               hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
               hv_Width = new HTuple();
               for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                   )) - 1); hv_Index = (int)hv_Index + 1)
               {
                   HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                       hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                   hv_Width = hv_Width.TupleConcat(hv_W);
               }
               hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                   ));
               hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
               hv_R2 = hv_R1 + hv_FrameHeight;
               hv_C2 = hv_C1 + hv_FrameWidth;
               //Display rectangles
               HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
               HOperatorSet.SetDraw(hv_WindowHandle, "fill");
               //Set shadow color
               HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
               if ((int)(hv_UseShadow) != 0)
               {
                   HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
               }
               //Set box color
               HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
               HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
               HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
           }
           //Write text.
           for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
               )) - 1); hv_Index = (int)hv_Index + 1)
           {
               hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                   )));
               if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                   "auto")))) != 0)
               {
                   HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
               }
               else
               {
                   HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
               }
               hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
               HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
               HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                   hv_Index));
           }
           //Reset changed window settings
           HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
           HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
               hv_Column2Part);

           return;
       }

        //保存高清照片
       public void Show_Weld_Results(HObject ho_Image_AffinTrans, HObject ho_ResultONG,
                           HObject ho_ResultOOK, HTuple hv_WindowID,String[] RunTime, Bitmap img)
       {
           // Local iconic variables 
           HObject ho_SelectedObjectOK = null, ho_SelectedObjectNG = null;
           // Local control variables 
           HTuple hv_NumOK = null, hv_NumNG = null, hv_j = null;
           HTuple hv_RowOK = new HTuple(), hv_ColumnOK = new HTuple();
           HTuple hv_Radius = new HTuple(), hv_k = null, hv_RowNG = new HTuple();
           HTuple hv_ColumnNG = new HTuple();
           HTuple hv_Width = new HTuple();
           HTuple hv_Height = new HTuple();
           HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
           HTuple hv_Column2 = null, hv_width = null, hv_height = null;
           // Initialize local and output iconic variables 
           HOperatorSet.GenEmptyObj(out ho_SelectedObjectOK);
           HOperatorSet.GenEmptyObj(out ho_SelectedObjectNG);
           try
           {
               Graphics g = Graphics.FromImage(img);
               g.DrawImage(img, 0, 0, img.Width, img.Height);
               Font frpnt = new Font("宋体",65);
               SolidBrush sbrush = new SolidBrush(Color.Green);
               string TimeNow = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒");
               g.DrawString(TimeNow, frpnt, sbrush, 100, img.Height - 100);
               g.DrawString(RunTime[0], frpnt, sbrush, 100, 100);
               g.DrawString(RunTime[1], frpnt, sbrush, 100, 200);
               g.DrawString(RunTime[2], frpnt, sbrush, 100, 300);
               ///////////////////////////////////////////////////////////////////////
               HOperatorSet.CountObj(ho_ResultOOK, out hv_NumOK);
               HOperatorSet.CountObj(ho_ResultONG, out hv_NumNG);
               HTuple end_val7 = hv_NumOK;
               HTuple step_val7 = 1;
               for (hv_j = 1; hv_j.Continue(end_val7, step_val7); hv_j = hv_j.TupleAdd(step_val7))
               {
                   ho_SelectedObjectOK.Dispose();
                   HOperatorSet.SelectObj(ho_ResultOOK, out ho_SelectedObjectOK, hv_j);
                   HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK,
                       out hv_Radius);
                   HOperatorSet.SmallestRectangle1(ho_SelectedObjectOK, out hv_Row1, out hv_Column1,
                                                out hv_Row2, out hv_Column2);
                   hv_width = hv_Column2 - hv_Column1;
                   hv_height = hv_Row2 - hv_Row1;
                   //新增20180703
                   HDImage.RowOK = float.Parse(hv_RowOK.ToString());
                   HDImage.ColumnOK = float.Parse(hv_ColumnOK.ToString());
                   g.DrawString("OK", frpnt, sbrush, HDImage.ColumnOK - 50, HDImage.RowOK + 200);
                   try
                   {
                       HDImage.Row1 = int.Parse(hv_Row1.ToString());
                       HDImage.Column1 = int.Parse(hv_Column1.ToString());
                       HDImage.width = int.Parse(hv_width.ToString());
                       HDImage.height = int.Parse(hv_height.ToString());
                       Pen pen = new Pen(Color.Green, 6);
                       g.DrawRectangle(pen, HDImage.Column1, HDImage.Row1, HDImage.width, HDImage.height);
                   }
                   catch (Exception)
                   { }                  
               }

               HTuple end_val15 = hv_NumNG;
               HTuple step_val15 = 1;
               for (hv_k = 1; hv_k.Continue(end_val15, step_val15); hv_k = hv_k.TupleAdd(step_val15))
               {
                   ho_SelectedObjectNG.Dispose();
                   HOperatorSet.SelectObj(ho_ResultONG, out ho_SelectedObjectNG, hv_k);
                   HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG,
                       out hv_ColumnNG, out hv_Radius);
                   HOperatorSet.SmallestRectangle1(ho_SelectedObjectNG, out hv_Row1, out hv_Column1,
                                                   out hv_Row2, out hv_Column2);
                   hv_width = hv_Column2 - hv_Column1;
                   hv_height = hv_Row2 - hv_Row1;
                   HDImage.RowNG = float.Parse(hv_RowNG.ToString());
                   HDImage.ColumnNG = float.Parse(hv_ColumnNG.ToString());
                   SolidBrush brush = new SolidBrush(Color.Red);
                   g.DrawString("NG", frpnt, brush, HDImage.ColumnNG - 50, HDImage.RowNG + 200);
                   try
                   {
                       HDImage.Row1 = int.Parse(hv_Row1.ToString());
                       HDImage.Column1 = int.Parse(hv_Column1.ToString());
                       HDImage.width = int.Parse(hv_width.ToString());
                       HDImage.height = int.Parse(hv_height.ToString());
                       Pen penNG = new Pen(Color.Red, 6);
                       g.DrawRectangle(penNG, HDImage.Column1, HDImage.Row1, HDImage.width, HDImage.height);
                   }
                   catch (Exception)
                   { }
               }

               //图像保存路径与名字 待修改20180724
               string TimeNowName = DateTime.Now.ToString("yyyyMMddHHmmssfff");              
               //获取当前小时
               int JudgHour = DateTime.Now.Hour;
               //获取当前班次
               string strName;
               if (JudgHour >= 8 && JudgHour <= 20)
               {
                   //对当前日期进行格式化
                   strName = DateTime.Now.ToString("yyyy年MM月dd日 白班");
               }
               else if (JudgHour < 8)
               {
                   //以前一天的日期来命名文件夹
                   strName = DateTime.Now.AddDays(-1).ToString("yyyy年MM月dd日 夜班");
               }
               else
               {
                   //对当前日期进行格式化
                   strName = DateTime.Now.ToString("yyyy年MM月dd日 夜班");
               }
               string data1 = ImgResultPath + '/' + strName + '/' + TimeNowName + ".BMP";
               img.Save(data1, System.Drawing.Imaging.ImageFormat.Jpeg); 
                 
               ho_SelectedObjectOK.Dispose();
               ho_SelectedObjectNG.Dispose();
               return;
           }
           catch (HalconException HDevExpDefaultException)
           {
               ho_SelectedObjectOK.Dispose();
               ho_SelectedObjectNG.Dispose();
               throw HDevExpDefaultException;
           }
       }

        //分白夜班按日期创建文件夹
       public void creation_date_file(string ImgResultPath)
       {      
            //获取当前小时
            int JudgHour = DateTime.Now.Hour;
            if (JudgHour >= 8 && JudgHour <= 20)
            {
                //对当前日期进行格式化
                string strName = DateTime.Now.ToString("yyyy年MM月dd日 白班");
                //创建DirectoryInfo对象
                DirectoryInfo DInfo = new DirectoryInfo(ImgResultPath + strName);
                //判断白班文件夹是否存在
                if (Directory.Exists(@"ImgResultPath + strName"))
                {
                    return;
                }
                else
                {
                    //创建文件夹
                    DInfo.Create();
                }
            }
            else if (JudgHour < 8)
            {
                //以前一天的日期来命名文件夹
                string strName = DateTime.Now.AddDays(-1).ToString("yyyy年MM月dd日 夜班");
                //创建DirectoryInfo对象
                DirectoryInfo DInfo = new DirectoryInfo(ImgResultPath + strName);
                //判断夜班文件夹是否存在
                if (Directory.Exists(@"ImgResultPath + strName"))
                {
                    return;
                }
                else
                {
                    //创建文件夹
                    DInfo.Create();
                }
            }
            else
            {
                //对当前日期进行格式化
                string strName = DateTime.Now.ToString("yyyy年MM月dd日 夜班");
                //创建DirectoryInfo对象
                DirectoryInfo DInfo = new DirectoryInfo(ImgResultPath + strName);
                //判断夜班文件夹是否存在
                if (Directory.Exists(@"ImgResultPath + strName"))
                {
                    return;
                }
                else
                {
                    //创建文件夹
                    DInfo.Create();
                }
            }
       }

       //将Halcon中灰度图转换为Bitmap图像      
       private void HalconGrayToBitmap(HObject image, out Bitmap res)
       {
           HTuple hpoint, type, width, height;
           const int Alpha = 255;
           int[] ptr = new int[2];
           HOperatorSet.GetImagePointer1(image, out hpoint, out type, out width, out height);
           res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
           ColorPalette pal = res.Palette;
           for (int i = 0; i <= 255; i++)
           {
               pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
           }
           res.Palette = pal;
           Rectangle rect = new Rectangle(0, 0, width, height);
           BitmapData bitmapData = res.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
           int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
           ptr[0] = bitmapData.Scan0.ToInt32();
           ptr[1] = hpoint.I;
           if (width % 4 == 0)
               CopyMemory(ptr[0], ptr[1], width * height * PixelSize);
           else
           {
               for (int i = 0; i < height - 1; i++)
               {
                   ptr[1] += width;
                   CopyMemory(ptr[0], ptr[1], width * PixelSize);
                   ptr[0] += bitmapData.Stride;
               }
           }
           res.UnlockBits(bitmapData);
       }
 
       //将Halcon中彩色图像转换为Bitmap图像       
       private void HalconRGBToBitmap(HObject image, out Bitmap res)
       {
           HTuple hred, hgreen, hblue, type, width, height;
           HOperatorSet.GetImagePointer3(image, out hred, out hgreen, out hblue, out type, out width, out height);
           res = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
           Rectangle rect = new Rectangle(0, 0, width, height);
           BitmapData bitmapData = res.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
           unsafe
           {
               byte* bptr = (byte*)bitmapData.Scan0;
               byte* r = ((byte*)hred.I);
               byte* g = ((byte*)hgreen.I);
               byte* b = ((byte*)hblue.I);
               for (int i = 0; i < width * height; i++)
               {
                   bptr[i * 4] = (b)[i];
                   bptr[i * 4 + 1] = (g)[i];
                   bptr[i * 4 + 2] = (r)[i];
                   bptr[i * 4 + 3] = 255;
               }
           }
           res.UnlockBits(bitmapData);
       }

       //判断图片的PixelFormat是否在 引发异常的 PixelFormat 之中,imgPixelFormat为原图片的PixelFormat
       private static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
       {
           foreach (PixelFormat pf in indexedPixelFormats)
           {
               if (pf.Equals(imgPixelFormat)) return true;
           }
           return false;
       }

       //会产生graphics异常的PixelFormat
       private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare,
                PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed,
                PixelFormat.Format8bppIndexed  };

       //保存参数记录
       public void writeCSV(string path,string Time, double id, double data1, double data2, 
                                      string str1, string str2, string str3, bool flag)
       {
           lock (this)
           {
               //获取当前程序目录
               string TimeNow = DateTime.Now.ToString("yyyyMMdd");
               string filePath = path + "模板匹配-"+TimeNow+".CSV";
               Console.WriteLine(filePath);

               if (!System.IO.File.Exists(filePath))//文件不存在时,创建新文件,并写入文件标题
               {
                   //创建文件流对象
                   FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                   //创建文件流写入对象,绑定文件流对象
                   StreamWriter sw = new StreamWriter(fs);
                   //创建数据对象
                   StringBuilder sb = new StringBuilder();
                   sb.Append("RunTime").Append(",").Append("ModelScore").Append(",").Append("ModelRow").Append(",").Append("ModelColumn").Append(",").Append("MatchTime").Append(",").Append("AlgoriTime").Append(",").Append("BitmapTime").Append(",").Append("Flag");
                   //把标题内容写入到文件流中
                   sw.WriteLine(sb);
                   sw.Flush();
                   sw.Close();
                   fs.Close();
               }

               //向CSV文件中写入数据内容
               StreamWriter msw = new StreamWriter(filePath, true, Encoding.Default);
               //创建数据对象
               StringBuilder msb = new StringBuilder();
               msb.Append(Time).Append(",").Append(id).Append(",").Append(data1).Append(",").Append(data2).Append(",").Append(str1).Append(",").Append(str2).Append(",").Append(str3).Append(",").Append(flag);

               //把数据内容写入文件中
               msw.WriteLine(msb);
               msw.Flush();
               msw.Close();
           }
       }
                                            

    }
}
