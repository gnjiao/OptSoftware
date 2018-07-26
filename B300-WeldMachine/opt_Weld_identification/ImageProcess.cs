using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

using System.Windows.Forms;
//线程
using System.Threading;
using System.Drawing.Imaging;

using HalconDotNet;
using System.IO;
//XML文件
using System.Xml;
using System.ComponentModel;
using System.Data;


namespace opt_Weld_identification
{
    class CImageProcess
    {
        public bool bTerminate;
        public bool PosStop = true;
        public bool TestStopLeft = true;
        public bool TestStopRight = true;
        public bool TestStopLeft2 = true;
        public bool TestStopRight2 = true;
        //   public bool TestStopSN = true;
        public bool bFindSN = true;

        public int CurIndex = 0;
        public int resPos = -1;
        public int resTest = -1;

        public double TPLAngleLimit = 0.5;
        public int TPLXLimit = 5;
        public int TPLYLimit = 5;
        public int TPLXDef = 5;
        public int TPLYDef = 5;


        public string _Save_image_directory_ = "D:\\Image";//图像保存目录
        //创建图像保存目录
        public void A_save_image(string save_temp)
        {
            try
            {
                if (Directory.Exists(save_temp) == false)//如果不存在就创建file文件夹
                {
                    System.IO.Directory.CreateDirectory(save_temp + "\\定位\\OK");
                    System.IO.Directory.CreateDirectory(save_temp + "\\定位\\NG");
                    ////
                    //System.IO.Directory.CreateDirectory(save_temp + "\\焊前正面识别\\OK");
                    //System.IO.Directory.CreateDirectory(save_temp + "\\焊前正面识别\\NG");
                    ////
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊前侧面识别\\OK");
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊前侧面识别\\NG");
                    //
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后正面识别\\OK");
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后正面识别\\NG");
                    //
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后侧面识别\\OK");
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后侧面识别\\NG");
                    //
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后条码识别\\OK");
                    System.IO.Directory.CreateDirectory(save_temp + "\\焊后条码识别\\NG");

                }

            }
            catch (Exception e)
            {
                MessageBox.Show("创建目录失败 " + save_temp + "    " + e.Message);
            }
        }
        //
        public UInt32 Total_OK_number = 0;          //OK    组件
        public UInt32 Total_NG_number = 0;          //NG    组件
        public UInt32 HQ_CM1_ERnumber = 0;          //焊前侧面NG数量
        public UInt32 HQ_ZM1_ERnumber = 0;          //焊前正面NG数量
        public UInt32 HH_ZM2_ERnumber = 0;          //焊后正面NG数量
        public UInt32 EROOR_Location_Number = 0;    //定位失败数量
        public UInt32 ERROR_SNnumber = 0;           //条码识别失败数量
        //
        //焊前正面图像保存（定位）
        public int _ZM1_saver_image = -1;
        //焊前正面图像保存（汇流条位置识别）
        public int _HQ_ZM1_saver_image = -1;
        //焊前侧面图像保存（汇流条按压识别）
        public int _CM1_saver_image = -1;
        //焊后正面图像保存（焊接质量识别）
        public int _ZM2_saver_image = -1;
        //焊后侧面图像保存（焊接质量识别）
        public int _CM2_saver_image = -1;
        //焊后条码图像保存
        public int _SN_saver_image = -1;
        //焊后正面高清水印图像保存（焊接质量识别）
        public int _CBZM2_saver_image = -1;
        
        
        //
        #region 焊前侧面汇流条按压识别
        //焊前侧面汇流条按压识别细节窗口
        public HalconDotNet.HWindowControl Q_cm_HWindow;
        //焊前侧面检测
        public bool Q_star_Template = true;     //拍照标记
        public int Q_resTest = -1;           //识别完成标记
        public int Q_row = 270;                //起始位置R
        public int Q_column = -541;            //起始位置W
        public int Q_HJ_spacing = 386;         //每个焊接位置的间距
        public int Q_GR_W = 110;               //检测大小W
        public int Q_GR_H = 80;                //检测大小H
        public int Q_threshold = 25;          //检测阈值强度
        public int Q_area = 1000;              //NG面积
        public int Q_high = 30;              //NG高度
        //public bool Q_show_lr = false;         //识别位置
        //20180319注销
        public bool Q_show_lr = true;         //识别位置
        //
        public int Q_star_Q_lr_checkbox = 1;//开启和关闭检测，默认开启 = 1
        //
        public string Q_Template_Catalog = ""; //模板目录
        public HObject Q_Template_Image = null;//模板图像
        public HTuple hv_Q_TemplateID = null;  //模板号码
        HObject Q_Template_region = null;       //模板区域
        public bool Q_error = false;           //识别结果 true = NG
        public HalconDotNet.HWindowControl Q_lr_HWindow;//侧面图像显示窗口
        HTuple Q_hv_pi = new HTuple();      //
        //读取模板图像
        public void Q_load_Template_image()
        {
            //HOperatorSet.SetDraw(Q_lr_HWindow.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(Q_lr_HWindow.HalconWindow, 1);
            try
            {
                HTuple hv_ImageName = new HTuple();
                hv_ImageName = TplPath + "\\Q_template_image.bmp";
                HOperatorSet.ReadImage(out Q_Template_Image, hv_ImageName);
                //建立模板
                Q_hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(Q_Template_Image, 4, -Q_hv_pi, 2 * Q_hv_pi, Q_hv_pi / 45,
                    "sort", "original", out hv_Q_TemplateID);
            }
            catch (Exception)
            {
                MessageBox.Show("焊前侧面识别模板读取失败！");
            }

        }
        //创建模板图像
        public void Q_save_tlp_image()
        {
            try
            {
                HObject ho_ImageGauss = null;
                HObject ho_ImageReduced = null;
                HObject ho_ImagePart = null;
                //     HObject ho_ImageZoomed = null;
                HOperatorSet.GenEmptyObj(out ho_ImageGauss);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                HOperatorSet.GenEmptyObj(out ho_ImagePart);

                HTuple hv_pi = new HTuple();
                ho_ImageGauss.Dispose();
                HOperatorSet.GaussImage(Q_side_Image, out ho_ImageGauss, 2.5);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageGauss, Q_Template_region, out ho_ImageReduced);
                ho_ImagePart.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                //     ho_ImageZoomed.Dispose();
                HOperatorSet.ZoomImageFactor(ho_ImagePart, out Q_Template_Image, 0.5, 0.5, "constant");
                //建立模板
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(Q_Template_Image, 4, -hv_pi, 2 * hv_pi, hv_pi / 45,
                    "sort", "original", out hv_Q_TemplateID);

                HOperatorSet.WriteImage(Q_Template_Image, "bmp", 0, TplPath + "\\Q_template_image");
                ho_ImageGauss.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImagePart.Dispose();
                MessageBox.Show("焊前侧面识别模板保存成功！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("焊前侧面识别模板保存失败！");
            }
        }
        //创建模板区域
        public void Q_tlp_region(int y, int x, int w, int h)
        {
            HOperatorSet.GenEmptyObj(out Q_Template_region);
            try
            {
                Q_Template_region.Dispose();
                HOperatorSet.GenRectangle2(out Q_Template_region, y, x, 0, w, h);
                HOperatorSet.SetColor(Q_lr_HWindow.HalconWindow, "yellow");
                HOperatorSet.DispObj(Q_side_Image, Q_lr_HWindow.HalconWindow);
                HOperatorSet.DispObj(Q_Template_region, Q_lr_HWindow.HalconWindow);

            }
            catch (System.Exception ex)
            {
                int error = 1;
            }
        }
        public HObject Q_side_Image = null;//侧面相机图像
        //焊接前侧面识别
        public void Q_star_PWR()
        {
            HObject Itempimage = null;
            HOperatorSet.GenEmptyObj(out Itempimage);
            Itempimage.Dispose();
            //
            HTuple _W = 0, _H = 0;

            Q_error = false;//OK

            HObject ho_ImageGauss1 = null, ho_ImageZoomed1 = null;
            HObject ho_ImageGauss1_AffinTrans = null, ho_EmptyObject = null;
            HObject ho_Rectangle1 = null, ho_ImageReduced1 = null;
            HObject ho_ImageMean = null, ho_ImageMean1 = null;
            HObject ho_RegionFillUp = null;
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed1);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1_AffinTrans);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);

            HObject ho_RegionDynThresh = null, ho_RegionClosing = null;
            HObject ho_RegionErosion = null, ho_RegionDilation = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null, ho_ConnectedRegions1 = null;
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);

            try
            {


                HTuple hv_Width, hv_Height, hv_Row, hv_Column, hv_Angle;
                HTuple hv_Error, hv_row_2, hv_column_2, hv_HomMat2D;
                HTuple hv_Q_row, hv_Q_column, hv_Q_HJ_spacing, hv_Q_PY_temp_spacing, hv_Index;
                HTuple hv_erNumber, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Index1;


                //开始焊接前识别
                HOperatorSet.GetImageSize(Q_side_Image, out hv_Width, out hv_Height);

                //3X3模糊图像
                //   HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                ho_ImageGauss1.Dispose();
                HOperatorSet.GaussImage(Q_side_Image, out ho_ImageGauss1, 2.5);

                //gen_rectangle2 (Rectangle2, Height/2, Width/2, 0, 900, 700)
                //reduce_domain (ImageGauss1, Rectangle2, ImageReduced2)
                ho_ImageZoomed1.Dispose();
                HOperatorSet.ZoomImageFactor(ho_ImageGauss1, out ho_ImageZoomed1, 0.5, 0.5,
                    "constant");

                //匹配位置
                HOperatorSet.BestMatchRotMg(ho_ImageZoomed1, hv_Q_TemplateID, -Q_hv_pi, 2 * Q_hv_pi,
                    40, "true", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);

                if (hv_Error <= 30.0)//
                {

                    //   HOperatorSet.DispObj(ho_ImageGauss1, hv_ExpDefaultWinHandle);
                    hv_row_2 = hv_Row * 2;
                    hv_column_2 = hv_Column * 2;

                    //if (Error)

                    //endif
                    if ((int)((new HTuple(hv_Angle.TupleLess(-0.5))).TupleOr(new HTuple(hv_Angle.TupleGreater(
                        0.5)))) != 0)
                    {
                        //从点和角度计算一个僵化的仿射变换。
                        HOperatorSet.VectorAngleToRigid(hv_row_2, hv_column_2, hv_Angle, hv_row_2,
                            hv_column_2, (new HTuple(180)).TupleRad(), out hv_HomMat2D);
                    }
                    else
                    {
                        //从点和角度计算一个僵化的仿射变换。
                        HOperatorSet.VectorAngleToRigid(hv_row_2, hv_column_2, hv_Angle, hv_row_2,
                            hv_column_2, (new HTuple(0)).TupleRad(), out hv_HomMat2D);
                    }
                    ho_ImageGauss1_AffinTrans.Dispose();
                    HOperatorSet.AffineTransImage(ho_ImageGauss1, out ho_ImageGauss1_AffinTrans,
                        hv_HomMat2D, "constant", "false");

                    HOperatorSet.DispObj(ho_ImageGauss1_AffinTrans, Q_lr_HWindow.HalconWindow);
                    //起始位置R
                    hv_Q_row = Q_row;
                    //起始位置W
                    hv_Q_column = Q_column;
                    //每个焊接位置的间距
                    hv_Q_HJ_spacing = Q_HJ_spacing;
                    //每次偏移位置累加
                    hv_Q_PY_temp_spacing = hv_Q_column + 0;
                    ho_EmptyObject.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_EmptyObject);
                    for (hv_Index = 0; (int)hv_Index <= 3; hv_Index = (int)hv_Index + 1)
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                            0, Q_GR_W, Q_GR_H);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Union2(ho_EmptyObject, ho_Rectangle1, out ExpTmpOutVar_0);
                            ho_EmptyObject.Dispose();
                            ho_EmptyObject = ExpTmpOutVar_0;
                        }
                        hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;
                        //stop ()
                    }

                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageGauss1_AffinTrans, ho_EmptyObject, out ho_ImageReduced1);
                       
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced1, out ho_ImageMean, 15, 15);
                    ho_ImageMean1.Dispose();
                    HOperatorSet.MeanImage(ho_ImageMean, out ho_ImageMean1, 200, 200);
                    //dev_display (ImageGauss1_AffinTrans)
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                        Q_threshold, "light");
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionDynThresh, out ho_RegionClosing, 3.5);
                    //dev_display (ImageGauss1_AffinTrans)
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionClosing, out ho_RegionFillUp);
                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionFillUp, out ho_RegionErosion, 1,
                        5);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion, out ho_RegionDilation,
                        1, 5);
                    //dev_display (ImageGauss1_AffinTrans)
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionDilation, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (new HTuple("area")).TupleConcat(
                        "height"), "and", (new HTuple(Q_area)).TupleConcat(Q_high), (new HTuple(9999999)).TupleConcat(
                        99999));

                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_erNumber);             
                    HOperatorSet.SetColor(Q_lr_HWindow.HalconWindow, "green");
                    HOperatorSet.SetLineWidth(Q_lr_HWindow.HalconWindow, 2);
                    HOperatorSet.DispArrow(Q_lr_HWindow.HalconWindow, hv_Row * 2, hv_Column * 2, hv_row_2 - ((hv_Angle.TupleCos()
                            ) * 50), hv_column_2 - ((hv_Angle.TupleSin()) * 50), 10);
                    HOperatorSet.DispObj(ho_EmptyObject, Q_lr_HWindow.HalconWindow);

                    if ((int)(new HTuple(hv_erNumber.TupleGreater(0))) != 0)
                    {
                        HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row3, out hv_Column3,
                            out hv_Phi, out hv_Length1, out hv_Length2);

                        HOperatorSet.SetColor(Q_lr_HWindow.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_EmptyObject, Q_lr_HWindow.HalconWindow);
                        HOperatorSet.DispObj(ho_SelectedRegions, Q_lr_HWindow.HalconWindow);
                        for (hv_Index1 = 0; (int)hv_Index1 <= (int)((new HTuple(hv_Row3.TupleLength()
                            )) - 1); hv_Index1 = (int)hv_Index1 + 1)
                        {
                            disp_message(Q_lr_HWindow.HalconWindow, "NG", "image", (hv_Row3.TupleSelect(
                                hv_Index1)) - 200, hv_Column3.TupleSelect(hv_Index1), "red", "false");
                            //新增20180319
                            disp_message(Q_cm_HWindow.HalconWindow, "NG", "image", (hv_Row3.TupleSelect(
                                hv_Index1)) - 200, hv_Column3.TupleSelect(hv_Index1), "red", "false");
                        }
                        Q_error = true;//NG


                        HOperatorSet.DumpWindowImage(out Itempimage, Q_lr_HWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(Q_cm_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //显示图像
                        HOperatorSet.DispObj(Itempimage, Q_cm_HWindow.HalconWindow);
                        HOperatorSet.DispObj(ho_EmptyObject, Q_cm_HWindow.HalconWindow);
                        Itempimage.Dispose();

                        try
                        {
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                + DateTime.Now.Minute.ToString()
                                + DateTime.Now.Second.ToString();
                            string data1 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P1-" + _DATATIME_M_;
                            HOperatorSet.WriteImage(Q_side_Image, "jpg", 0, data1);
                            string data2 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P2-" + _DATATIME_M_;
                            HOperatorSet.DumpWindow(Q_cm_HWindow.HalconWindow, "jpg", data2);
                        }
                        catch (System.Exception ex2)
                        {
                        }
                    }
                    else
                    {
                        //临时注销20180319
                        //if (Q_show_lr)
                        //{
                        HOperatorSet.SetColor(Q_lr_HWindow.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_EmptyObject, Q_lr_HWindow.HalconWindow);
                        HOperatorSet.DispObj(ho_RegionDilation, Q_lr_HWindow.HalconWindow);
                        HOperatorSet.SetColor(Q_lr_HWindow.HalconWindow, "green");
                        HOperatorSet.DispObj(ho_EmptyObject, Q_lr_HWindow.HalconWindow);
                        ho_ConnectedRegions1.Dispose();
                        HOperatorSet.Connection(ho_EmptyObject, out ho_ConnectedRegions1);
                        HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out hv_Row3, out hv_Column3,
                            out hv_Phi, out hv_Length1, out hv_Length2);
                        for (hv_Index1 = 0; (int)hv_Index1 <= 3; hv_Index1 = (int)hv_Index1 + 1)
                        {
                            disp_message(Q_lr_HWindow.HalconWindow, "OK", "image", (hv_Row3.TupleSelect(
                                hv_Index1)) - 200, hv_Column3.TupleSelect(hv_Index1), "green", "false");
                            //新增20180319
                            disp_message(Q_cm_HWindow.HalconWindow, "OK", "image", (hv_Row3.TupleSelect(
                                hv_Index1)) - 200, hv_Column3.TupleSelect(hv_Index1), "green", "false");
                        }
                        //}
                        HOperatorSet.DumpWindowImage(out Itempimage, Q_lr_HWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(Q_cm_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        HOperatorSet.DispObj(Itempimage, Q_cm_HWindow.HalconWindow);

                        //HOperatorSet.SetPart(Q_lr_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //HOperatorSet.DispObj(Itempimage, Q_lr_HWindow.HalconWindow);

                        Itempimage.Dispose();
                        try
                        {
                            if (_CM1_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                    + DateTime.Now.Minute.ToString()
                                    + DateTime.Now.Second.ToString();
                                string data1 = _Save_image_directory_ + "\\焊前侧面识别\\OK\\P1-" + _DATATIME_M_;
                                HOperatorSet.WriteImage(Q_side_Image, "jpg", 0, data1);
                                string data2 = _Save_image_directory_ + "\\焊前侧面识别\\OK\\P2-" + _DATATIME_M_;
                                HOperatorSet.DumpWindow(Q_cm_HWindow.HalconWindow, "jpg", data2);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                        }
                    }






                }
                else
                {
                    //模板定位失败
                    Q_error = true;//NG
                    Log("侧面相机：模板定位失败！");
                    HOperatorSet.DumpWindowImage(out Itempimage, Q_lr_HWindow.HalconWindow);
                    HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                    //以适应窗口的方式显示图像
                    HOperatorSet.SetPart(Q_cm_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                    //显示图像
                    HOperatorSet.DispObj(Itempimage, Q_cm_HWindow.HalconWindow);
                    Itempimage.Dispose();
                    try
                    {
                        A_save_image(_Save_image_directory_);//不存在时创建目录
                        //获取时间
                        string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                            + DateTime.Now.Minute.ToString()
                            + DateTime.Now.Second.ToString();
                        string data1 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P1-" + _DATATIME_M_;
                        HOperatorSet.WriteImage(Q_side_Image, "jpg", 0, data1);
                        string data2 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P2-" + _DATATIME_M_;
                        HOperatorSet.DumpWindow(Q_cm_HWindow.HalconWindow, "jpg", data2);
                    }
                    catch (System.Exception ex2)
                    {
                    }
                }
            }
            catch (System.Exception ex)
            {
                //识别失败
                Q_error = true;//NG
                Log("侧面相机：识别错误！");
                HOperatorSet.DumpWindowImage(out Itempimage, Q_lr_HWindow.HalconWindow);
                HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                //以适应窗口的方式显示图像
                HOperatorSet.SetPart(Q_cm_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                //显示图像
                HOperatorSet.DispObj(Itempimage, Q_cm_HWindow.HalconWindow);
                Itempimage.Dispose();
                try
                {
                    A_save_image(_Save_image_directory_);//不存在时创建目录
                    //获取时间
                    string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString();
                    string data1 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P1-" + _DATATIME_M_;
                    HOperatorSet.WriteImage(Q_side_Image, "jpg", 0, data1);
                    string data2 = _Save_image_directory_ + "\\焊前侧面识别\\NG\\P2-" + _DATATIME_M_;
                    HOperatorSet.DumpWindow(Q_cm_HWindow.HalconWindow, "jpg", data2);
                }
                catch (System.Exception ex2)
                {

                }
            }
            ho_ImageGauss1.Dispose();
            ho_ImageZoomed1.Dispose();
            ho_ImageGauss1_AffinTrans.Dispose();
            ho_EmptyObject.Dispose();
            ho_Rectangle1.Dispose();
            ho_ImageReduced1.Dispose();
            ho_ImageMean.Dispose();
            ho_ImageMean1.Dispose();
            ho_RegionDynThresh.Dispose();
            ho_RegionClosing.Dispose();
            ho_RegionErosion.Dispose();
            ho_RegionDilation.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_ConnectedRegions1.Dispose();

        }
        //线程
        public void Q_Pre_welding_recognition()
        {

            Q_load_Template_image();//加载焊接前识别模板图像

            while (!bTerminate)
            {
                while (Q_star_Template)//等待拍照标记
                {
                    Thread.Sleep(50);
                    continue;
                }

                //Q_error = false;//初始化OK
                Q_resTest = -1;

                try
                {
                    unsafe
                    {

                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[LF2CamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Q_side_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;


                                //      Log("侧面相机开始识别");
                                //     HOperatorSet.DispObj(Left_Image, HWindowL1.HalconWindow);
                                Q_star_PWR();

                            }
                            else
                            {
                                Log("侧面相机拍照失败");
                                InitLeftCam2();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam2();
                    continue;
                }

                Q_star_Template = true;//关闭标记

                Q_resTest = 0;

            }

        }
        //焊接前侧面相机拍照
        public void Q_SnapPosImage()
        {
            try
            {
                unsafe
                {

                    byte[] image_date = new byte[Len];
                    fixed (byte* p = &image_date[0])
                    {
                        if (JHCap.CameraQueryImage(CamID[LF2CamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                        {
                            //_GEN_IMAGE_ERROR_ = true;
                            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                            System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                            HOperatorSet.GenImageInterleaved(out Q_side_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);
                            //显示图像
                            HOperatorSet.DispObj(Q_side_Image, Q_lr_HWindow.HalconWindow);

                            if (_CM1_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                    + DateTime.Now.Minute.ToString()
                                    + DateTime.Now.Second.ToString();
                                string data1 = _Save_image_directory_ + "\\焊前侧面识别\\P1-" + _DATATIME_M_;
                                HOperatorSet.WriteImage(Q_side_Image, "jpg", 0, data1);
                                //string data2 = _Save_image_directory_ + "\\焊前侧面识别\\P2-" + _DATATIME_M_;
                                //HOperatorSet.DumpWindow(Q_lr_HWindow.HalconWindow, "jpg", data2);
                            }

                            img.UnlockBits(data);
                            img.Dispose();
                            image_date = null;


                            if (!CheckImg(Q_side_Image))
                            {
                                Log("侧面相机2CheckImg失败");
                                InitLeftCam2();
                            }
                        }
                        else
                        {
                            Log("侧面相机拍照失败");
                            InitLeftCam2();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                InitLeftCam2();
            }

        }
        #endregion
        //
        #region 焊后正面第二次识别
        //焊后正面第二次识别细节窗口
        public HalconDotNet.HWindowControl zm2_HWindow;
        //=================焊后:正面第二次识别=============================================================
        public HalconDotNet.HWindowControl Z2_Positive_HWindow;//侧面图像显示窗口
        //焊接后正面检测20171224
        public int Z2_CenterRow = 1360;                //起始位置R
        public int Z2_CenterCol = 778;            //起始位置W
        public int Z2_WeldDis = 360;         //每个焊接位置的间距
        public int Z2_WeldW = 95;               //检测大小W
        public int Z2_WeldH = 110;                //检测大小H 
        public int Z2_WeldDisY = 5;                //英利焊点中间间距
        //Z2_WeldDisY 
        public HObject Z2_Positive_Image = null;//正面相机图像20171222
        public HObject Z2_Template_Image = null;//正面模板图像
        //
        public int Z2_star_zm1_soldering = 1;//打开或关闭焊后正面识别
        public int Z2_star_zm1_solderingL = 1;//打开汇流条最大长度检测
        public int WeldShow;//汇流条特征显示功能
        
        //
        public HTuple hv_Z2_TemplateID = null;  //正面模板号码
        HObject Z2_Template_region = null;       //正面模板区域
        //
        public bool Z2_error = false;           //正面识别结果 true = NG
        public bool Z2_show_lr = false;         //调试显示使用
        public string WeldDate;
        public string LifeTime;

        //
        public bool Z2_star_Positive = true;     //拍照标记
        public int Z2_resTest = -1;           //识别完成标记
        //20180309注销 晶科0 协鑫1 英利2 JinKO3
        public int Z2_CcbRecognize = 2; //厂家识别模式  


        //
        //模板区域坐标
        public int Z2P_X = 0;
        public int Z2P_Y = 0;
        public int Z2P_W = 0;
        public int Z2P_H = 0;
        //防止鼠标误操作界面,先做缓存
        int TEMP_Z2P_X = 0;
        int TEMP_Z2P_Y = 0;
        int TEMP_Z2P_W = 0;
        int TEMP_Z2P_H = 0;
        //创建正面模板区域
        //***************************************************************
        //防止鼠标误操作界面,先做缓存
        int TEMP_F2P_X = 0;
        int TEMP_F2P_Y = 0;
        int TEMP_F2P_W = 0;
        int TEMP_F2P_H = 0;
        //***************************************************************
        public void Z2_tlp_region(int y, int x, int w, int h)
        {
            TEMP_Z2P_X = x;
            TEMP_Z2P_Y = y;
            TEMP_Z2P_W = w;
            TEMP_Z2P_H = h;
            try
            {
                HOperatorSet.GenEmptyObj(out Z2_Template_region);
                Z2_Template_region.Dispose();
                HOperatorSet.GenRectangle2(out Z2_Template_region, y, x, 0, w, h);
                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "yellow");
                HOperatorSet.DispObj(Z2_Positive_Image, Z2_Positive_HWindow.HalconWindow);
                HOperatorSet.DispObj(Z2_Template_region, Z2_Positive_HWindow.HalconWindow);
            }
            catch (System.Exception ex)
            {
                int error = 1;
            }
        }
        //
        #region jin-20171222-未使用
        //正面：读取模板图像
        public void Z2_load_Template_image()
        {
            try
            {
                HTuple hv_ImageName = new HTuple();
                hv_ImageName = TplPath + "\\Z2_template_image.sbm";
                //    HOperatorSet.ReadImage(out Z2_Positive_Image, hv_ImageName);
                HOperatorSet.ReadShapeModel(hv_ImageName, out hv_Z2_TemplateID);

                ////
                //HOperatorSet.GenEmptyObj(out Z2_Template_region);
                //Z2_Template_region.Dispose();
                //HOperatorSet.GenRectangle2(out Z2_Template_region, Z2P_Y, Z2P_X, 0, Z2P_W, Z2P_H);

                //HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "yellow");
                //HOperatorSet.DispObj(Z2_Template_region, Z2_Positive_HWindow.HalconWindow);


                ////建立模板
                //Q_hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                //HOperatorSet.CreateTemplateRot(Z2_Positive_Image, 4, -Q_hv_pi, 2 * Q_hv_pi, Q_hv_pi / 45,
                //    "sort", "original", out hv_Q_TemplateID);
            }
            catch (Exception)
            {
                MessageBox.Show("焊后正面识别模板图象读取失败！");
            }

        }
        //保存正面模板图像
        public void Z2_save_tlp_image()
        {
            try
            {
                //定义控制变量
                HTuple hv_ModelID = null;
                //定义图形变量
                HObject ho_ImageGauss;
                //初始化图形变量
                HOperatorSet.GenEmptyObj(out ho_ImageGauss);

                //对图像进行高斯滤波处理
                ho_ImageGauss.Dispose();
                HOperatorSet.GaussFilter(Z2_Positive_Image, out ho_ImageGauss, 2.5);
                HOperatorSet.GenEmptyObj(out Z2_Template_Image);
                Z2_Template_Image.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageGauss, Z2_Template_region, out Z2_Template_Image);
                HOperatorSet.CreateShapeModel(Z2_Template_Image, 4, 0, (new HTuple(360)).TupleRad()
                                  , "auto", "auto", "use_polarity", "auto", "auto", out hv_Z2_TemplateID);
                //     HOperatorSet.WriteImage(Z2_Template_Image, "bmp", 0, TplPath + "\\Z2_template_image");
                HOperatorSet.WriteShapeModel(hv_Z2_TemplateID, TplPath + "\\Z2_template_image.sbm");
                ho_ImageGauss.Dispose();

                //确认保存参数
                Z2P_X = TEMP_Z2P_X;
                Z2P_Y = TEMP_Z2P_Y;
                Z2P_W = TEMP_Z2P_W;
                Z2P_H = TEMP_Z2P_H;


            }
            catch (Exception ex)
            {
            }
        }
        //正面相机图像旋转jin-20171222
        public void Z2_Rotate_image(HObject ho_ImageGauss, out HObject ho_ImageAffine)
        {



            // Local iconic variables 

            HObject ho_Regions, ho_Connection, ho_SelectedRegions;
            HObject ho_RegionUnion, ho_Rectangle = null;

            // Local control variables 

            HTuple hv_Row1 = null, hv_Column1 = null, hv_Phi1 = null;
            HTuple hv_Length1 = null, hv_Length2 = null, hv_Phi = null;
            HTuple hv_Area3 = null, hv_Row = null, hv_Column = null;
            HTuple hv_HomMat2D = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageAffine);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Connection);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ImageGauss, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "margin");
                }
                ho_Regions.Dispose();
                HOperatorSet.Threshold(ho_ImageGauss, out ho_Regions, 0, 40);
                ho_Connection.Dispose();
                HOperatorSet.Connection(ho_Regions, out ho_Connection);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_Connection, out ho_SelectedRegions, "area", "and",
                    300000, 9999999);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row1, out hv_Column1,
                    out hv_Phi1, out hv_Length1, out hv_Length2);
                //校正图像角度
                if ((int)((new HTuple(hv_Phi1.TupleLess(-0.5))).TupleOr(new HTuple(hv_Phi1.TupleGreater(
                    0.5)))) != 0)
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row1 + 150, hv_Column1, 0,
                        hv_Length1 / 2, hv_Length2);
                }
                else
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row1 + 150, hv_Column1, 0,
                        hv_Length1, hv_Length2 / 2);
                }
                //获取区域的方向
                HOperatorSet.OrientationRegion(ho_Rectangle, out hv_Phi);
                //获取区域中心坐标
                HOperatorSet.AreaCenter(ho_Rectangle, out hv_Area3, out hv_Row, out hv_Column);
                if ((int)((new HTuple(hv_Phi.TupleLess(-0.5))).TupleOr(new HTuple(hv_Phi.TupleGreater(
                    0.5)))) != 0)
                {
                    //从点和角度计算一个刚性的仿射变换。
                    HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Phi, hv_Row, hv_Column,
                        (new HTuple(180)).TupleRad(), out hv_HomMat2D);
                }
                else
                {
                    //从点和角度计算一个刚性的仿射变换。
                    HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Phi, hv_Row, hv_Column,
                        (new HTuple(0)).TupleRad(), out hv_HomMat2D);
                }
                //通过仿射变换校正图像和区域
                ho_ImageAffine.Dispose();
                HOperatorSet.AffineTransImage(ho_ImageGauss, out ho_ImageAffine, hv_HomMat2D,
                    "constant", "false");
                //stop ()
                ho_Regions.Dispose();
                ho_Connection.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_Rectangle.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Regions.Dispose();
                ho_Connection.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_Rectangle.Dispose();

                throw HDevExpDefaultException;
            }
        }
        //正面相机查找模板&数据处理jin-20171223
        public void Z2_Data_judgment_process(HObject ho_ImageGauss, HObject ho_WeldRect,
            HObject ho_ModelRect, out HObject ho_ResultObjectOK, out HObject ho_ResultObjectNG,
            HTuple hv_ModelID)
        {

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_FillRegion = null, ho_ConnectedRegions;
            HObject ho_SortedRegions, ho_ObjectSelected = null, ho_ImageReduced = null;
            HObject ho_Regions = null, ho_Connections = null, ho_SelectedRegions = null;
            HObject ho_TransRegion = null, ho_ReducedImage = null, ho_Region = null;
            HObject ho_Connection = null, ho_SelectedRegion = null, ho_Rect1 = null;
            HObject ho_Intersection = null, ho_Rect2 = null, ho_Rect3 = null;
            HObject ho_RegionUnion = null, ho_ReduceImage = null, ho_RegionR = null;

            // Local control variables 

            HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
            HTuple hv_Score = null, hv_Area00 = new HTuple(), hv_Row00 = new HTuple();
            HTuple hv_Column00 = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Num = null, hv_i = null, hv_Mean = new HTuple();
            HTuple hv_Deviation = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Area2 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_BPercentage = new HTuple();
            HTuple hv_Rectangularity = new HTuple(), hv_Row11 = new HTuple();
            HTuple hv_Column11 = new HTuple(), hv_Row21 = new HTuple();
            HTuple hv_Column21 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_WWidth = new HTuple(), hv_WHeight = new HTuple();
            HTuple hv_MinR = new HTuple(), hv_MaxR = new HTuple();
            HTuple hv_RangeR = new HTuple(), hv_MeanR = new HTuple();
            HTuple hv_DeviationR = new HTuple(), hv_NumR = new HTuple();
            HTuple hv_Row1D = new HTuple(), hv_Column1D = new HTuple();
            HTuple hv_Row2D = new HTuple(), hv_Column2D = new HTuple();
            HTuple hv_Row1R = new HTuple(), hv_Column1R = new HTuple();
            HTuple hv_Row2R = new HTuple(), hv_Column2R = new HTuple();
            HTuple hv_RWidth = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_RowNG = new HTuple(), hv_ColumnNG = new HTuple();
            HTuple hv_Radius = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
            HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
            HOperatorSet.GenEmptyObj(out ho_FillRegion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Connections);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_TransRegion);
            HOperatorSet.GenEmptyObj(out ho_ReducedImage);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_Connection);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_Rect1);
            HOperatorSet.GenEmptyObj(out ho_Intersection);
            HOperatorSet.GenEmptyObj(out ho_Rect2);
            HOperatorSet.GenEmptyObj(out ho_Rect3);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ReduceImage);
            HOperatorSet.GenEmptyObj(out ho_RegionR);
            try
            {
                //查找模板
                // HOperatorSet.FindShapeModel(ho_ImageGauss, hv_ModelID, 0, (new HTuple(360)).TupleRad()
                //   , 0.5, 1, 0.9, "least_squares", 4, 0.9, out hv_Row, out hv_Column, out hv_Angle,
                // out hv_Score);

                HOperatorSet.FindShapeModel(ho_ImageGauss, hv_ModelID, 0, (new HTuple(360)).TupleRad()
                    , 0.3, 1, 0.5, "least_squares", 4, 0.5, out hv_Row, out hv_Column, out hv_Angle,
                    out hv_Score);

                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleGreater(0))) != 0)
                {
                    //       HOperatorSet.AreaCenter(ho_ModelRect, out hv_Area00, out hv_Row00, out hv_Column00);
                    //      HOperatorSet.VectorAngleToRigid(hv_Row00, hv_Column00, 0, hv_Row, hv_Column,
                    //         hv_Angle, out hv_HomMat2D);

                    //Z2P_Y, Z2P_X, 0, Z2P_W, Z2P_H
                    HOperatorSet.VectorAngleToRigid(Z2P_Y, Z2P_X, 0, hv_Row, hv_Column,
                        hv_Angle, out hv_HomMat2D);

                    ho_FillRegion.Dispose();
                    HOperatorSet.AffineTransRegion(ho_WeldRect, out ho_FillRegion, hv_HomMat2D,
                        "constant");
                }
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_FillRegion, out ho_ConnectedRegions);
                ho_SortedRegions.Dispose();
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left",
                    "false", "row");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_Num);

                //if (HDevWindowStack.IsOpen())
                //{
                //    HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                //}
                //if (HDevWindowStack.IsOpen())
                //{
                //    HOperatorSet.DispObj(ho_ImageGauss, HDevWindowStack.GetActive());
                //}
                ho_ResultObjectOK.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
                ho_ResultObjectNG.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
                HTuple end_val15 = hv_Num;
                HTuple step_val15 = 1;


                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "yellow");
                HOperatorSet.DispObj(ho_SortedRegions, Z2_Positive_HWindow.HalconWindow);


                for (hv_i = 1; hv_i.Continue(end_val15, step_val15); hv_i = hv_i.TupleAdd(step_val15))
                {
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, hv_i);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageGauss, ho_ObjectSelected, out ho_ImageReduced
                        );
                    HOperatorSet.Intensity(ho_ObjectSelected, ho_ImageReduced, out hv_Mean, out hv_Deviation);
                    //提取焊点周围黑色面积
                    ho_Regions.Dispose();
                    //HOperatorSet.Threshold(ho_ImageReduced, out ho_Regions, 0, (((new HTuple(51)).TupleConcat(
                    // hv_Deviation - 10))).TupleMin());
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Regions, 0, (((new HTuple(51)).TupleConcat(
            hv_Deviation))).TupleMin());
                    ho_Connections.Dispose();
                    HOperatorSet.Connection(ho_Regions, out ho_Connections);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_Connections, out ho_SelectedRegions, "area",
                        "and", 4900, 99999);
                    HOperatorSet.AreaCenter(ho_ObjectSelected, out hv_Area1, out hv_Row1, out hv_Column1);
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area2, out hv_Row2, out hv_Column2);
                    //计算焊点周围黑色部分所占的百分比
                    hv_BPercentage = hv_Area2 / (hv_Area1 + 0.0001);
                    //提取焊点中间白色面积
                    ho_TransRegion.Dispose();
                    HOperatorSet.ShapeTrans(ho_SelectedRegions, out ho_TransRegion, "convex");
                    ho_ReducedImage.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageGauss, ho_TransRegion, out ho_ReducedImage
                        );
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ReducedImage, out ho_Region, (((new HTuple(100)).TupleConcat(
                        hv_Mean))).TupleMax(), 255);
                    ho_Connection.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_Connection);
                    //2700改1000
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShape(ho_Connection, out ho_SelectedRegion, "area", "and",
                        500, 99999);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShapeStd(ho_SelectedRegion, out ExpTmpOutVar_0, "max_area",
                            70);
                        ho_SelectedRegion.Dispose();
                        ho_SelectedRegion = ExpTmpOutVar_0;
                    }
                    HOperatorSet.Rectangularity(ho_SelectedRegion, out hv_Rectangularity);
                    ho_TransRegion.Dispose();
                    HOperatorSet.ShapeTrans(ho_SelectedRegion, out ho_TransRegion, "rectangle1");
                    HOperatorSet.SmallestRectangle1(ho_TransRegion, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    HOperatorSet.CountObj(ho_TransRegion, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                    {
                        //解决提取不到焊点中间白色面积的情况
                        hv_Rectangularity = 1;
                    }
                    hv_WWidth = hv_Column21 - hv_Column11;
                    hv_WHeight = hv_Row21 - hv_Row11;

                    //数据判断
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), 2);
                    }
                    HOperatorSet.MinMaxGray(ho_SelectedRegion, ho_ReducedImage, 0, out hv_MinR,
                        out hv_MaxR, out hv_RangeR);
                    HOperatorSet.Intensity(ho_SelectedRegion, ho_ImageReduced, out hv_MeanR,
                        out hv_DeviationR);
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_NumR);
                    if ((int)(new HTuple(hv_NumR.TupleGreater(0))) != 0)
                    {
                        HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row1D, out hv_Column1D,
                            out hv_Row2D, out hv_Column2D);
                        ho_Rect1.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rect1, hv_Row1D, hv_Column1D, hv_Row2D - 50,
                            hv_Column2D);
                        ho_Intersection.Dispose();
                        HOperatorSet.Intersection(ho_Rect1, ho_SelectedRegions, out ho_Intersection
                            );
                        HOperatorSet.SmallestRectangle1(ho_Intersection, out hv_Row1R, out hv_Column1R,
                            out hv_Row2R, out hv_Column2R);
                        hv_RWidth = hv_Column2R - hv_Column1R;
                        //提取锡未融时左右两边的金属片
                        if ((int)(new HTuple(((hv_Row1D + 50)).TupleLess(hv_Row2D - 50))) != 0)
                        {
                            //左侧矩形
                            ho_Rect2.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rect2, hv_Row1D + 50, hv_Column1R - 30,
                                hv_Row2D - 50, hv_Column1R);
                            //右侧矩形
                            ho_Rect3.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rect3, hv_Row1D + 50, hv_Column2R, hv_Row2D - 50,
                                hv_Column2R + 30);
                            ho_RegionUnion.Dispose();
                            HOperatorSet.Union2(ho_Rect2, ho_Rect3, out ho_RegionUnion);
                            ho_ReduceImage.Dispose();
                            HOperatorSet.ReduceDomain(ho_ImageGauss, ho_RegionUnion, out ho_ReduceImage);
                            ho_RegionR.Dispose();
                            HOperatorSet.Threshold(ho_ReduceImage, out ho_RegionR, 85, 255);
                            HOperatorSet.AreaCenter(ho_RegionR, out hv_Area, out hv_Row3, out hv_Column3);
                        }
                        else
                        {
                            hv_Area = 2500;
                        }
                    }
                    else
                    {
                        hv_RWidth = 111;
                        hv_Area = 2500;
                    }

                    //结果合并
                    HOperatorSet.SmallestCircle(ho_ObjectSelected, out hv_RowNG, out hv_ColumnNG, out hv_Radius);
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_BPercentage.TupleGreater(
                        0.13))).TupleAnd(new HTuple(hv_Rectangularity.TupleLess(0.86))))).TupleAnd(
                        new HTuple(hv_RWidth.TupleGreater(110))))).TupleAnd(new HTuple(hv_Area.TupleLess(
                        1500)))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultObjectOK, ho_ObjectSelected, out ExpTmpOutVar_0);
                            ho_ResultObjectOK.Dispose();
                            ho_ResultObjectOK = ExpTmpOutVar_0;
                        }
                        // disp_message(3600, "OK", "image", hv_RowNG + 200, hv_ColumnNG - 50, "green", "false");                            
                    }
                    else
                    {
                        if ((int)((new HTuple((new HTuple(hv_MeanR.TupleLess(125))).TupleOr(new HTuple(hv_RWidth.TupleLess(
                            109))))).TupleOr(new HTuple(hv_Area.TupleGreater(2000)))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ResultObjectNG, ho_ObjectSelected, out ExpTmpOutVar_0);
                                ho_ResultObjectNG.Dispose();
                                ho_ResultObjectNG = ExpTmpOutVar_0;
                            }
                            //disp_message(3600, "NG", "image", hv_RowNG + 200, hv_ColumnNG - 50, "red","false");                                
                        }
                        else
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ResultObjectOK, ho_ObjectSelected, out ExpTmpOutVar_0);
                                ho_ResultObjectOK.Dispose();
                                ho_ResultObjectOK = ExpTmpOutVar_0;
                            }
                            // disp_message(3600, "OK", "image", hv_RowNG + 200, hv_ColumnNG - 50, "green", "false");                               
                        }
                        // stop(); only in hdevelop
                    }
                    //stop ()
                }

                ho_FillRegion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SortedRegions.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ImageReduced.Dispose();
                ho_Regions.Dispose();
                ho_Connections.Dispose();
                ho_SelectedRegions.Dispose();
                ho_TransRegion.Dispose();
                ho_ReducedImage.Dispose();
                ho_Region.Dispose();
                ho_Connection.Dispose();
                ho_SelectedRegion.Dispose();
                ho_Rect1.Dispose();
                ho_Intersection.Dispose();
                ho_Rect2.Dispose();
                ho_Rect3.Dispose();
                ho_RegionUnion.Dispose();
                ho_ReduceImage.Dispose();
                ho_RegionR.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                HOperatorSet.DispObj(ho_SortedRegions, Z2_Positive_HWindow.HalconWindow);
                disp_message(Z2_Positive_HWindow.HalconWindow, "识别失败", "image",
                100, 100, "red", "false");

                ho_FillRegion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SortedRegions.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ImageReduced.Dispose();
                ho_Regions.Dispose();
                ho_Connections.Dispose();
                ho_SelectedRegions.Dispose();
                ho_TransRegion.Dispose();
                ho_ReducedImage.Dispose();
                ho_Region.Dispose();
                ho_Connection.Dispose();
                ho_SelectedRegion.Dispose();
                ho_Rect1.Dispose();
                ho_Intersection.Dispose();
                ho_Rect2.Dispose();
                ho_Rect3.Dispose();
                ho_RegionUnion.Dispose();
                ho_ReduceImage.Dispose();
                ho_RegionR.Dispose();

                throw HDevExpDefaultException;
            }
        }
        //正面：焊接后识别jin-20171223
        public void Z2_star_PR(HObject in_image)
        {
            HObject Itempimage = null;
            HOperatorSet.GenEmptyObj(out Itempimage);
            Itempimage.Dispose();
            //
            HTuple _W = 0, _H = 0;

            Z2_error = false;//OK
            //定义控制变量
            HTuple hv_ImagePath = null, hv_ResultPath = null;
            HTuple hv_Row11 = null, hv_Column11 = null, hv_Row21 = null;
            HTuple hv_Column21 = null, hv_Area00 = null, hv_Row00 = null;
            HTuple hv_Column00 = null, hv_ModelID = null, hv_ImageFiles = null;
            HTuple hv_Index = null, hv_afileBaseName = new HTuple();
            HTuple hv_afileExt = new HTuple(), hv_afileDir = new HTuple();
            HTuple hv_ResultFileName = new HTuple(), hv_CenterRow = new HTuple();
            HTuple hv_CenterCol = new HTuple(), hv_WeldW = new HTuple();
            HTuple hv_WeldH = new HTuple(), hv_WeldDis = new HTuple();
            HTuple hv_i = new HTuple(), hv_NumOK = new HTuple(), hv_NumNG = new HTuple();
            HTuple hv_j = new HTuple(), hv_RowOK = new HTuple(), hv_ColumnOK = new HTuple();
            HTuple hv_Radius = new HTuple(), hv_k = new HTuple(), hv_RowNG = new HTuple();
            HTuple hv_ColumnNG = new HTuple();
            //定义图形变量
            HObject ho_Image, ho_GrayImage, ho_ImageGauss;
            HObject ho_ModelRect, ho_ModelReduced, ho_WeldRect = null;
            HObject ho_RectWeld = null, ho_ResultObjectOK = null, ho_ResultObjectNG = null;
            HObject ho_SelectedObjectOK = null, ho_SelectedObjectNG = null;
            //初始化图形变量
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss);
            HOperatorSet.GenEmptyObj(out ho_ModelRect);
            HOperatorSet.GenEmptyObj(out ho_ModelReduced);
            HOperatorSet.GenEmptyObj(out ho_WeldRect);
            HOperatorSet.GenEmptyObj(out ho_RectWeld);
            HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
            HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
            HOperatorSet.GenEmptyObj(out ho_SelectedObjectOK);
            HOperatorSet.GenEmptyObj(out ho_SelectedObjectNG);
            try
            {
                //ho_GrayImage.Dispose();
                //HOperatorSet.Rgb1ToGray(Z_front_Image, out ho_GrayImage);
                //ho_ImageGauss.Dispose();
                //HOperatorSet.GaussFilter(ho_GrayImage, out ho_ImageGauss, 5);
                ho_ImageGauss.Dispose();
                HOperatorSet.GaussFilter(in_image, out ho_ImageGauss, 5);

                //旋转校正图像
                HObject ExpTmpOutVar_0;
                Z2_Rotate_image(ho_ImageGauss, out ExpTmpOutVar_0);
                ho_ImageGauss.Dispose();
                ho_ImageGauss = ExpTmpOutVar_0;

                //显示旋转后图像
                //    HOperatorSet.DispObj(ho_ImageGauss, Z2_Positive_HWindow.HalconWindow);

                ho_WeldRect.Dispose();
                HOperatorSet.GenEmptyObj(out ho_WeldRect);
                /*
                hv_CenterRow = 1360;
                hv_CenterCol = 778;
                hv_WeldW = 95;
                hv_WeldH = 110;
                hv_WeldDis = 360;
                */
                hv_CenterRow = Z2_CenterRow;
                hv_CenterCol = Z2_CenterCol;
                hv_WeldW = Z2_WeldW;
                hv_WeldH = Z2_WeldH;
                hv_WeldDis = Z2_WeldDis;

                for (hv_i = 1; (int)hv_i <= 4; hv_i = (int)hv_i + 1)
                {
                    ho_RectWeld.Dispose();
                    HOperatorSet.GenRectangle2(out ho_RectWeld, hv_CenterRow, hv_CenterCol,
                        0, hv_WeldW, hv_WeldH);
                    {
                        HObject ExpTmpOutVar_00;
                        HOperatorSet.Union2(ho_RectWeld, ho_WeldRect, out ExpTmpOutVar_00);
                        ho_WeldRect.Dispose();
                        ho_WeldRect = ExpTmpOutVar_00;
                    }
                    hv_CenterCol = hv_CenterCol + hv_WeldDis;
                }

                //结果显示            
                //    HOperatorSet.ClearWindow(Z2_Positive_HWindow.HalconWindow);
                HOperatorSet.DispObj(ho_ImageGauss, Z2_Positive_HWindow.HalconWindow);

                //查找模板&数据处理
                ho_ResultObjectOK.Dispose();
                ho_ResultObjectNG.Dispose();
                Z2_Data_judgment_process(ho_ImageGauss, ho_WeldRect, Z2_Template_region, out ho_ResultObjectOK,
                    out ho_ResultObjectNG, hv_Z2_TemplateID);


                HOperatorSet.CountObj(ho_ResultObjectOK, out hv_NumOK);
                HOperatorSet.CountObj(ho_ResultObjectNG, out hv_NumNG);
                HTuple end_val66 = hv_NumOK;
                HTuple step_val66 = 1;
                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "green");
                for (hv_j = 1; hv_j.Continue(end_val66, step_val66); hv_j = hv_j.TupleAdd(step_val66))
                {
                    HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "green");
                    HOperatorSet.DispObj(ho_ResultObjectOK, Z2_Positive_HWindow.HalconWindow);
                    ho_SelectedObjectOK.Dispose();
                    HOperatorSet.SelectObj(ho_ResultObjectOK, out ho_SelectedObjectOK, hv_j);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK, out hv_Radius);
                    HOperatorSet.SetTposition(Z2_Positive_HWindow.HalconWindow, hv_RowOK + 200, hv_ColumnOK - 50);
                    HOperatorSet.WriteString(Z2_Positive_HWindow.HalconWindow, "OK");
                }

                HTuple end_val74 = hv_NumNG;
                HTuple step_val74 = 1;
                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                for (hv_k = 1; hv_k.Continue(end_val74, step_val74); hv_k = hv_k.TupleAdd(step_val74))
                {
                    HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_ResultObjectNG, Z2_Positive_HWindow.HalconWindow);
                    ho_SelectedObjectNG.Dispose();
                    HOperatorSet.SelectObj(ho_ResultObjectNG, out ho_SelectedObjectNG, hv_k);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG, out hv_ColumnNG, out hv_Radius);
                    HOperatorSet.SetTposition(Z2_Positive_HWindow.HalconWindow, hv_RowNG + 200, hv_ColumnNG - 50);
                    HOperatorSet.WriteString(Z2_Positive_HWindow.HalconWindow, "NG");
                }


                HOperatorSet.DumpWindowImage(out Itempimage, Z2_Positive_HWindow.HalconWindow);
                HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                //以适应窗口的方式显示图像
                HOperatorSet.SetPart(zm2_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                //显示图像
                HOperatorSet.DispObj(Itempimage, zm2_HWindow.HalconWindow);
                Itempimage.Dispose();
                if (hv_NumNG > 0)
                {
                    try
                    {
                        //保存图像
                        if (_ZM2_saver_image > 0)
                        {
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                + DateTime.Now.Minute.ToString()
                                + DateTime.Now.Second.ToString();
                            string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\P1-" + _DATATIME_M_;
                            HOperatorSet.WriteImage(Z2_Positive_Image, "jpg", 0, data1);
                            string data2 = _Save_image_directory_ + "\\焊后正面识别\\NG\\P2-" + _DATATIME_M_;
                            HOperatorSet.DumpWindow(zm2_HWindow.HalconWindow, "jpg", data2);
                        }
                    }
                    catch (System.Exception ex2)
                    {

                    }
                }
                else
                {
                    try
                    {
                        //保存图像
                        if (_ZM2_saver_image > 0)
                        {
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                + DateTime.Now.Minute.ToString()
                                + DateTime.Now.Second.ToString();
                            string data1 = _Save_image_directory_ + "\\焊后正面识别\\OK\\P1-" + _DATATIME_M_;
                            HOperatorSet.WriteImage(Z2_Positive_Image, "jpg", 0, data1);
                            string data2 = _Save_image_directory_ + "\\焊后正面识别\\OK\\P2-" + _DATATIME_M_;
                            HOperatorSet.DumpWindow(zm2_HWindow.HalconWindow, "jpg", data2);
                        }
                    }
                    catch (System.Exception ex2)
                    {

                    }
                }

            }
            catch (System.Exception ex)
            {
                //识别失败
                Z2_error = true;//NG
                Log("正面相机：识别错误！");
            }
            ho_Image.Dispose();
            ho_GrayImage.Dispose();
            ho_ImageGauss.Dispose();
            ho_ModelRect.Dispose();
            ho_ModelReduced.Dispose();
            ho_WeldRect.Dispose();
            ho_RectWeld.Dispose();
            ho_ResultObjectOK.Dispose();
            ho_ResultObjectNG.Dispose();
            ho_SelectedObjectOK.Dispose();
            ho_SelectedObjectNG.Dispose();
        }
        #endregion
        //正面：焊接后拍照
        public void Z2_SnapPos_image()
        {
            try
            {
                unsafe
                {
                    byte[] image_date = new byte[Len];
                    fixed (byte* p = &image_date[0])
                    {
                        if (JHCap.CameraQueryImage(CamID[0], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                        {
                            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                            System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);
                            HOperatorSet.GenImageInterleaved(out Z2_Positive_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                            img.UnlockBits(data);
                            img.Dispose();
                            image_date = null;

                            Z2_star_PR_z(Z2_Positive_Image, out ho_ResultOOK, out ho_ResultONG);//识别
                        }
                        else
                        {
                            InitPosCam();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //    Log(e.Message);
                InitPosCam();
            }

        }

        ////等待主界面初始化完毕
        public bool start_form2 = true;
        //正面：焊接后正面识别线程
        public Bitmap img;
        public void Z2_Positive_recognition()
        {
            //等待主界面初始化完毕
            while (start_form2)
            {
                Thread.Sleep(10);
            }

            //正面：初始化模板
            //     Z2_load_Template_image();
            //
            Z2_load_Template_image_z();

            while (!bTerminate)
            {
                while (Z2_star_Positive)
                {
                    Thread.Sleep(50);
                    continue;
                }

                Z2_resTest = -1;
                Z2_error = false;//初始化OK

                try
                {
                    unsafe
                    {
                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[0], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);
                                HOperatorSet.GenImageInterleaved(out Z2_Positive_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);
                                img.UnlockBits(data);
                            

                                //正面焊接后识别
                                //    Z2_star_PR(Z2_Positive_Image);//金
                                Z2_error = Z2_star_PR_z(Z2_Positive_Image, out ho_ResultOOK, out ho_ResultONG);//张
                                try
                                {
                                    if (_CBZM2_saver_image ==1)
                                    {
                                        //保存高清照片
                                        Show_Weld_Results(Z2_Positive_Image, ho_ResultONG, ho_ResultOOK, Z2_Positive_HWindow.HalconWindow, img);
                                    }
                                }
                                catch (Exception)
                                { }
                                

                                img.Dispose();
                                image_date = null;
                            }
                            else
                            {
                                Z2_error = true;//NG
                                Log("焊接后正面拍照异常");
                                InitPosCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Z2_error = true;//NG
                    Log(e.Message);
                    InitPosCam();
                    continue;
                }

                Z2_star_Positive = true;     //拍照标记
                Z2_resTest = 1;

            }

        }
        //
        //----------------------------------------
        //焊后正面识别参数
        public int star_Z2_L_ = 0;//打开或关闭最大边界识别
        public int star_Z2_L_rw_ = 1;//打开左右两条边界识别
        public int Z2_L_dyn_threshold_ = 12;//边界识别阈值
        public double Z2_L_height_max_ = 75.0;//最大单个边界
        public double Z2_L_height_min_ = 40.0;//出现2条边界的允许长度
        //
        public int star_Z2_L2_ = 1;//打开或关闭90度边界检测
        public double Z2_L_perforation_length_ = 35.0;//获取链接穿孔位置的边界长度
        public int Z2_L_AfewLines_dyn_threshold_ = 12;//行区域阈值
        public double Z2_L_AfewLines_height_ = 60.0;//行区域获取位置
        public bool show_Z2_L_AfewLines_height_ = false;//调试显示行区域获取位置
        public double Z2_L_AfewLines_length_ = 27.0;//行区域获取长度
        public double Z2_L_height_90_ = 35.0;//90度缺陷高度
        //
        public int star_Z2_r1area_ = 1;//打开或关闭左右区域识别
        public double Z2_r1_hrow_ = 55.0;//左右识别区域的高度
        public double Z2_r1_crw_ = 22.5;//左右识别区域的宽度
        public bool show_Z2_r1_hrow_crw_ = false;//调试显示左右识别区域
        public double Z2_r1area_ = 23.0;//左右单个面积小于设定比例为NG
        public int Z2_r1_number_ = 1;//左右NG数量
        //
        public int star_Z2_r2area_ = 1;//打开或关闭焊接不均匀识别
        public double Z2_r2area_ = 88.0;//两个面积大小相似度
        public double Z2_r2_aHeight_ = 41.0;//高度
        public double Z2_r2_awidth_ = 73.0;//宽度
        //
        public int star_Z2_r3area_ = 1;//打开或关闭焊接包裹度识别
        public double Z2_r3area_ = 15.0;//包裹面积
        public double Z2_r3_crw_min_ = 10.0;//包裹宽度范围min
        public double Z2_r3_crw_max_ = 50.0;//包裹宽度范围max
        public double Z2_r3_hrow_ = 50.0;//包裹高度
        //协鑫焊后正面识别参数
        public int Z2_L_dyn_threshold_Xx = 12;//边界识别阈值
        public double Z2_L_height_max_Xx = 40.0;//出现2条边界的允许长度
        public int star_Z2_L_rw_Xx = 1;//打开左右两条边界识别

        public int Z2_L_threshold_Xx = 10;//手动阈值
        public double Z2_L_Back_Area_Xx = 84.2;//黑色区域面积      
        public int star_Z2_L_rc_Xx = 1;//是否启用黑色区域检测功能

        //英利焊后正面识别参数 20180326
        public int Z2_L_dyn_threshold_YL = 100;//手动阈值

        public int nudHoleMinHeight = 100;//
        public int nudHoleMaxHeight = 100;//
        public int nudHoleMinWidth = 100;//
        public double nudBacknum = 100;//
        //public int star_Z2_L_rw_YL = 0;//打开左右两条边界识别
        public bool star_Z2_L_rw_YL = false;//调试显示左右识别区域
        public int nudHoleMaxArea = 100;//孔的最小面积


        //
        #region Z：20180102-焊后正面识别
        //Z：读取模板图像
        public void Z2_load_Template_image_z()
        {
            //HOperatorSet.SetDraw(Q_lr_HWindow.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(Q_lr_HWindow.HalconWindow, 1);
            try
            {
                HTuple hv_ImageName = new HTuple();
                hv_ImageName = TplPath + "\\Z2_template_image.bmp";
                HOperatorSet.ReadImage(out Z2_Template_Image, hv_ImageName);
                //建立模板
                Q_hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(Z2_Template_Image, 4, -Q_hv_pi, 2 * Q_hv_pi, Q_hv_pi / 45,
                    "sort", "original", out hv_Z2_TemplateID);
            }
            catch (Exception)
            {
                MessageBox.Show("焊后正面识别模板读取失败！");
            }

        }
        //Z：创建模板
        public void Z2_save_Template_image_z()
        {
            try
            {
                HObject ho_Image_Rotate_X;
                HOperatorSet.GenEmptyObj(out ho_Image_Rotate_X);
                HObject ho_ImageReduced;
                HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                HObject ho_ImagePart;
                HOperatorSet.GenEmptyObj(out ho_ImagePart);
                HTuple hv_pi;

                //建立模板之前先做图像角度矫正
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(Z2_Positive_Image, Z2_Template_region, out ho_ImageReduced);
                ho_ImagePart.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                HOperatorSet.ZoomImageFactor(ho_ImagePart, out Z2_Template_Image, 0.5, 0.5, "constant");
                //建立模板
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(Z2_Template_Image, 4, -hv_pi, 2 * hv_pi, hv_pi / 45,
                    "sort", "original", out hv_Z2_TemplateID);
                HOperatorSet.WriteImage(Z2_Template_Image, "bmp", 0, TplPath + "\\Z2_template_image");
                ho_Image_Rotate_X.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImagePart.Dispose();

                MessageBox.Show("焊后正面识别模板保存成功！");

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("焊后正面识别模板保存失败！");
            }

        }
        //显示正面第二次调试区域
        public bool Z2_show_rectangle_z = false;
        //Z：晶科正面识别算法
        public void Z2_str_Welding_recognition_z(HObject ho_Image, HObject ho_Image_AffinTrans,
            out HObject ho_out_EO_error_region, HTuple hv_row_2, HTuple hv_column_2, HTuple hv_WindowID,
            out HTuple hv_out_temp_error)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_EmptyObject_her, ho_EmptyObject_herX;
            HObject ho_Rectangle1, ho_LR_SRegionsX, ho_area_erSRegions;
            HObject ho_ImageReduced1 = null, ho_Rectangle7_t = null, ho_ImageReduced_t = null;
            HObject ho_ImageMean = null, ho_ImageMean1 = null, ho_RegionDynThresh = null;
            HObject ho_RegionDilation = null, ho_RegionClosing1 = null;
            HObject ho_RegionErosion = null, ho_ConnectedRegions = null;
            HObject ho_h_ER_sRegions = null, ho_h_sRegions = null, ho_EmptyObject_l = null;
            HObject ho_SelectedRegions = null, ho_RegionUnion1 = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions1 = null, ho_RegionClosing_er = null;
            HObject ho_Rectangle3 = null, ho_Rectangle2 = null, ho_RegionDilation1 = null;
            HObject ho_RegionUnion = null, ho_ConnectedRegions3 = null;
            HObject ho_SelectedRegions2 = null, ho_RegionDifference2 = null;
            HObject ho_ConnectedRegions4 = null, ho_EmptyObject_her2 = null;
            HObject ho_RegionUnion13 = null, ho_herX_region = null, ho_RegionUnion12 = null;
            HObject ho_h_ERROR_SRegions2 = null, ho_RegionUnion4 = null;
            HObject ho_RegionUnion6 = null, ho_RegionUnion7 = null, ho_Rectangle8 = null;
            HObject ho_RegionDifference4 = null, ho_RegionErosion3 = null;
            HObject ho_ImageReduced = null, ho_ImageMean5 = null, ho_ImageMean6 = null;
            HObject ho_RegionDynThresh2 = null, ho_RegionClosing4 = null;
            HObject ho_ConnectedRegions8 = null, ho_Rectangle4 = null, ho_SelectedRegions3 = null;
            HObject ho_RegionDilation2 = null, ho_RegionDilation3 = null;
            HObject ho_RegionUnion5 = null, ho_RegionClosing7 = null, ho_ConnectedRegions7 = null;
            HObject ho_ImageMean2 = null, ho_ImageInvert = null, ho_RegionDynThresh1 = null;
            HObject ho_RegionClosing = null, ho_Rectangle7 = null, ho_RegionIntersection1 = null;
            HObject ho_RegionClosing3_w = null, ho_RegionUnion3_w = null;
            HObject ho_ConnectedRegions5 = null, ho_dw_SRegions = null;
            HObject ho_Rectangle_left = null, ho_Rectangle_right = null;
            HObject ho_RegionUnion2 = null, ho_RegionDifference3 = null;
            HObject ho_RegionClosing3 = null, ho_RegionFillUp2 = null, ho_ConnectedRegions6 = null;
            HObject ho_lr_SRegions2 = null, ho_Rectangle9 = null, ho_RegionUnion11 = null;
            HObject ho_RegionFillUp1 = null, ho_RegionDifference5 = null;
            HObject ho_RegionErosion4 = null, ho_RegionDifference = null;
            HObject ho_RegionIntersection = null, ho_RegionUnion3 = null;
            HObject ho_RegionClosing5 = null, ho_RegionFillUp3 = null, ho_ConnectedRegions2 = null;
            HObject ho_RegionUnion10 = null, ho_Rectangle_bkd = null, ho_RegionIntersection2 = null;
            HObject ho_RegionClosing6 = null, ho_RegionUnion8 = null, ho_RegionFillUp = null;
            HObject ho_RegionIntersection4 = null, ho_RegionUnion9 = null;
            HObject ho_ConnectedRegions9 = null, ho_dh_Sregion = null;

            // Local control variables 

            HTuple hv_h_ERROR_Number = null, hv_h_erNumber = null;
            HTuple hv_h_erNumberX = null, hv_dw_erNumber = null, hv_LR_erNumberX = null;
            HTuple hv_area_erNumber = null, hv_Q_row = null, hv_Q_column = null;
            HTuple hv_Q_HJ_spacing = null, hv_Q_PY_temp_spacing = null;
            HTuple hv_Index2 = null, hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple();
            HTuple hv_tempNumber = new HTuple(), hv_Electrode_L = new HTuple();
            HTuple hv_LherX = new HTuple(), hv_Row1_temp = new HTuple();
            HTuple hv_Column1_temp = new HTuple(), hv_Row2_temp = new HTuple();
            HTuple hv_Column2_temp = new HTuple(), hv_temp_L = new HTuple();
            HTuple hv_Index_temp = new HTuple(), hv_TEMP_Lmax = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Row18 = new HTuple();
            HTuple hv_Column18 = new HTuple(), hv_Row28 = new HTuple();
            HTuple hv_Column28 = new HTuple(), hv_Row7 = new HTuple();
            HTuple hv_Column7 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row8 = new HTuple(), hv_Column8 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_w_lenght = new HTuple();
            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
            HTuple hv_Area2 = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Max2 = new HTuple();
            HTuple hv_h_erNumber2 = new HTuple(), hv_Row14 = new HTuple();
            HTuple hv_Column14 = new HTuple(), hv_Row24 = new HTuple();
            HTuple hv_Column24 = new HTuple(), hv_w_column = new HTuple();
            HTuple hv_Row16 = new HTuple(), hv_Column16 = new HTuple();
            HTuple hv_Row26 = new HTuple(), hv_Column26 = new HTuple();
            HTuple hv_temo_h = new HTuple(), hv_h_Min = new HTuple();
            HTuple hv_hmin_row = new HTuple(), hv_temper_herX = new HTuple();
            HTuple hv_h_ERROR_Number2 = new HTuple(), hv_Row13 = new HTuple();
            HTuple hv_Column13 = new HTuple(), hv_Row23 = new HTuple();
            HTuple hv_Column23 = new HTuple(), hv_Row17 = new HTuple();
            HTuple hv_Column17 = new HTuple(), hv_Row27 = new HTuple();
            HTuple hv_Column27 = new HTuple(), hv_ocrw = new HTuple();
            HTuple hv_hrow_e = new HTuple(), hv_Mean = new HTuple();
            HTuple hv_Deviation = new HTuple(), hv_m_threshold = new HTuple();
            HTuple hv_Area4 = new HTuple(), hv_Row9 = new HTuple();
            HTuple hv_Column9 = new HTuple(), hv_r1areaa = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_lr_erArea = new HTuple();
            HTuple hv_lr_erNumber2 = new HTuple(), hv__w = new HTuple();
            HTuple hv__h = new HTuple(), hv_Area1 = new HTuple(), hv_Row4 = new HTuple();
            HTuple hv_Column4 = new HTuple(), hv_Max1 = new HTuple();
            HTuple hv_areaNumber1 = new HTuple(), hv_Row15 = new HTuple();
            HTuple hv_Column15 = new HTuple(), hv_Row25 = new HTuple();
            HTuple hv_Column25 = new HTuple(), hv_a_height = new HTuple();
            HTuple hv_a_width = new HTuple(), hv_dh_ErNumber = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_out_EO_error_region);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject_her);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject_herX);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_LR_SRegionsX);
            HOperatorSet.GenEmptyObj(out ho_area_erSRegions);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle7_t);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced_t);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_h_ER_sRegions);
            HOperatorSet.GenEmptyObj(out ho_h_sRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject_l);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing_er);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject_her2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion13);
            HOperatorSet.GenEmptyObj(out ho_herX_region);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion12);
            HOperatorSet.GenEmptyObj(out ho_h_ERROR_SRegions2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion4);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion6);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion7);
            HOperatorSet.GenEmptyObj(out ho_Rectangle8);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference4);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion3);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageMean5);
            HOperatorSet.GenEmptyObj(out ho_ImageMean6);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh2);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing4);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions8);
            HOperatorSet.GenEmptyObj(out ho_Rectangle4);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation2);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation3);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion5);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing7);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions7);
            HOperatorSet.GenEmptyObj(out ho_ImageMean2);
            HOperatorSet.GenEmptyObj(out ho_ImageInvert);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_Rectangle7);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing3_w);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3_w);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions5);
            HOperatorSet.GenEmptyObj(out ho_dw_SRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_left);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_right);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference3);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing3);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions6);
            HOperatorSet.GenEmptyObj(out ho_lr_SRegions2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle9);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion11);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp1);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference5);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion4);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing5);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp3);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion10);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_bkd);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection2);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing6);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion8);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection4);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion9);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions9);
            HOperatorSet.GenEmptyObj(out ho_dh_Sregion);
            //
            hv_out_temp_error = 0;

            try
            {

                //NG区域记录
                ho_out_EO_error_region.Dispose();
                HOperatorSet.GenEmptyObj(out ho_out_EO_error_region);

                //dev_display (Image_AffinTrans)
                //1：翘起的数量
                //最大允许长度
                hv_h_ERROR_Number = 0;
                //数量
                hv_h_erNumber = 0;
                ho_EmptyObject_her.Dispose();
                HOperatorSet.GenEmptyObj(out ho_EmptyObject_her);

                //2：穿孔附近链接的严重翘起
                hv_h_erNumberX = 0;
                ho_EmptyObject_herX.Dispose();
                HOperatorSet.GenEmptyObj(out ho_EmptyObject_herX);

                //3：无包裹为NG
                hv_dw_erNumber = 0;
                ho_Rectangle1.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Rectangle1);

                //4：包裹不完整，上、左右、却2边
                hv_LR_erNumberX = 0;
                ho_LR_SRegionsX.Dispose();
                HOperatorSet.GenEmptyObj(out ho_LR_SRegionsX);

                //5：判断焊锡包裹密度
                hv_area_erNumber = 0;
                ho_area_erSRegions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_area_erSRegions);

                //
                //      hv_out_temp_error = 0;

                //起始位置R
                hv_Q_row = Z2_CenterRow;
                //起始位置W
                hv_Q_column = Z2_CenterCol;
                //每个焊接位置的间距
                hv_Q_HJ_spacing = Z2_WeldDis;
                //每次偏移位置累加
                hv_Q_PY_temp_spacing = hv_Q_column + 0;
                //gen_empty_obj (EmptyObject_Classes)
                //dev_display (ImageGauss1)
                //dev_display (ImageGauss1_AffinTrans)
                for (hv_Index2 = 0; (int)hv_Index2 <= 3; hv_Index2 = (int)hv_Index2 + 1)
                {
                    //
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                        0, Z2_WeldW, Z2_WeldH);
                    //显示调试区域
                    if (Z2_show_rectangle_z)
                    {
                        HOperatorSet.SetColor(hv_WindowID, "yellow");
                        HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                    }

                    //stop ()
                    //测试位置
                    //Q_PY_temp_spacing := Q_PY_temp_spacing+Q_HJ_spacing

                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle1, out ho_ImageReduced1
                        );
                    //
                    HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                        out hv_Row2, out hv_Column2);
                    hv_h_row = hv_Row2 - hv_Row1;
                    //h_column := Column2-Column1
                    hv_cr_w = hv_Column2 - hv_Column1;
                    //
                    //dev_display (Image_AffinTrans)
                    ho_Rectangle7_t.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle7_t, hv_Row1, hv_Column1 + (hv_cr_w * 0.03),
                        hv_Row2 + (hv_Row2 * 0.01), hv_Column2 - (hv_cr_w * 0.03));
                    ho_ImageReduced_t.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle7_t, out ho_ImageReduced_t
                        );
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced_t, out ho_ImageMean, 1, 30);
                    ho_ImageMean1.Dispose();
                    HOperatorSet.MeanImage(ho_ImageMean, out ho_ImageMean1, 13, 1);
                    //dev_display (Image_AffinTrans)
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                        Z2_L_dyn_threshold_, "dark");
                    //dev_display (Image_AffinTrans)
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionDynThresh, out ho_RegionDilation,
                        1, 15);
                    ho_RegionClosing1.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionDilation, out ho_RegionClosing1, 2.5);
                    //dev_display (Image_AffinTrans)
                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionClosing1, out ho_RegionErosion, 1,
                        15);
                    //
                    //超过设定长度，汇流条有翘起
                    //dev_display (Image_AffinTrans)
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionErosion, out ho_ConnectedRegions);

                    //最大允许长度
                    //dev_display (Image_AffinTrans)
                    ho_h_ER_sRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_h_ER_sRegions, "height",
                        "and", hv_h_row * (Z2_L_height_max_ / 100.0), 9999);
                    //判断2条特征线的距离宽度
                    HOperatorSet.CountObj(ho_h_ER_sRegions, out hv_h_ERROR_Number);


                    //dev_display (Image_AffinTrans)
                    ho_h_sRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_h_sRegions, "height",
                        "and", hv_h_row * (Z2_L_height_min_ / 100.0), 9999);
                    //判断2条特征线的距离宽度
                    HOperatorSet.CountObj(ho_h_sRegions, out hv_tempNumber);

                    //stop ()

                    //汇流条宽度
                    hv_Electrode_L = hv_cr_w * 0.35;
                    //判断2个区域的间距
                    hv_LherX = hv_cr_w * 0.5;
                    //缺陷区域
                    //gen_empty_obj (EmptyObject_her)
                    if ((int)(new HTuple(hv_tempNumber.TupleGreater(1))) != 0)
                    {

                        HOperatorSet.SmallestRectangle1(ho_h_sRegions, out hv_Row1_temp, out hv_Column1_temp,
                            out hv_Row2_temp, out hv_Column2_temp);
                        hv_temp_L = hv_Row2_temp - hv_Row1_temp;
                        //删选最长的2个
                        ho_EmptyObject_l.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_EmptyObject_l);
                        for (hv_Index_temp = 1; (int)hv_Index_temp <= 2; hv_Index_temp = (int)hv_Index_temp + 1)
                        {
                            try
                            {
                                //dev_display (Image_AffinTrans)
                                HOperatorSet.TupleMax(hv_temp_L, out hv_TEMP_Lmax);
                                //获取最大值的位置
                                HOperatorSet.TupleFind(hv_temp_L, hv_TEMP_Lmax, out hv_Indices);
                                //删除索引位置的值
                                HOperatorSet.TupleRemove(hv_temp_L, hv_Indices, out hv_temp_L);
                                //
                                //dev_display (Image_AffinTrans)
                                ho_SelectedRegions.Dispose();
                                HOperatorSet.SelectShape(ho_h_sRegions, out ho_SelectedRegions, "height",
                                    "and", hv_TEMP_Lmax, 9999);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Union2(ho_EmptyObject_l, ho_SelectedRegions, out ExpTmpOutVar_0
                                        );
                                    ho_EmptyObject_l.Dispose();
                                    ho_EmptyObject_l = ExpTmpOutVar_0;
                                }
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                                //2个一样长时可能报错，不用处理
                            }

                        }

                        //
                        ho_RegionUnion1.Dispose();
                        HOperatorSet.Union1(ho_EmptyObject_l, out ho_RegionUnion1);
                        //ho_ConnectedRegions1.Dispose();
                        //HOperatorSet.Connection(ho_RegionUnion1, out ho_ConnectedRegions1);
                        //ho_SelectedRegions1.Dispose();
                        //HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1,
                        //    "area", "and", 1, (hv_cr_w * hv_h_row) + 1);
                        //HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
                        //
                        //if (Number>1)
                        //sort_region (SelectedRegions1, SortedRegions, 'first_point', 'true', 'column')
                        //smallest_rectangle2 (SortedRegions, Row, Column, Phi2, Length12, Length22)
                        //dev_display (ImageGauss1)
                        //dev_display (Image_AffinTrans)
                        //gen_rectangle2 (Rectangle, Row[0], Column[0], Phi2[0], Length12[0], Length22[0])
                        //gen_rectangle2 (Rectangle6, Row[1], Column[1], Phi2[1], Length12[1], Length22[1])
                        //gen_region_line (RegionLines, Row[0], Column[0], Row[1], Column[1])

                        //endif


                        //   HOperatorSet.DispObj(ho_Image_AffinTrans, hv_ExpDefaultWinHandle);
                        ho_RegionClosing_er.Dispose();
                        HOperatorSet.ClosingRectangle1(ho_RegionUnion1, out ho_RegionClosing_er,
                            hv_cr_w * 0.5, 1);
                        //获取中间部分区域计算
                        HOperatorSet.SmallestRectangle1(ho_RegionClosing_er, out hv_Row18, out hv_Column18,
                            out hv_Row28, out hv_Column28);
                        HOperatorSet.SmallestRectangle2(ho_RegionClosing_er, out hv_Row7, out hv_Column7,
                            out hv_Phi, out hv_Length1, out hv_Length2);
                        //    HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                        //    HOperatorSet.DispObj(ho_Image_AffinTrans, hv_ExpDefaultWinHandle);
                        ho_Rectangle3.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row7, hv_Column7, (new HTuple(90)).TupleRad()
                            , 0.1, (hv_Column28 - hv_Column18) / 2);
                        HOperatorSet.SmallestRectangle2(ho_Rectangle3, out hv_Row8, out hv_Column8,
                            out hv_Phi1, out hv_Length11, out hv_Length21);
                        //smallest_rectangle1 (RegionClosing2, Row12, Column12, Row22, Column22)
                        hv_w_lenght = hv_Length11 * 2;
                        if ((int)((new HTuple(hv_w_lenght.TupleGreater(hv_Electrode_L))).TupleAnd(
                            new HTuple(hv_w_lenght.TupleLess(hv_LherX)))) != 0)
                        {
                            hv_h_erNumber = 2;
                            ho_EmptyObject_her.Dispose();
                            HOperatorSet.CopyObj(ho_RegionClosing_er, out ho_EmptyObject_her, 1,
                                1);
                        }
                        else
                        {
                            hv_h_erNumber = 1;

                        }
                    }
                    else
                    {
                        hv_h_erNumber = hv_tempNumber.Clone();

                    }

                    //stop ()


                    //链接穿孔处有超过设定长度，认为是较严重汇流条翘起
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_Rectangle2.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row21 - 20, hv_Column11, hv_Row21,
                        hv_Column21);
                    //
                    //dev_display (Image_AffinTrans)
                    ho_RegionDilation1.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion, out ho_RegionDilation1,
                        1, 11);
                    //
                    //dev_display (Image_AffinTrans)
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union2(ho_RegionDilation1, ho_Rectangle2, out ho_RegionUnion
                        );
                    ho_ConnectedRegions3.Dispose();
                    HOperatorSet.Connection(ho_RegionUnion, out ho_ConnectedRegions3);
                    HOperatorSet.AreaCenter(ho_ConnectedRegions3, out hv_Area2, out hv_Row5,
                        out hv_Column5);
                    HOperatorSet.TupleMax(hv_Area2, out hv_Max2);
                    //dev_display (Image_AffinTrans)
                    ho_SelectedRegions2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions3, out ho_SelectedRegions2, "area",
                        "and", hv_Max2, (hv_cr_w * hv_h_row) + 1);
                    //获取穿孔处特征
                    //dev_display (Image_AffinTrans)
                    ho_RegionDifference2.Dispose();
                    HOperatorSet.Intersection(ho_SelectedRegions2, ho_RegionClosing1, out ho_RegionDifference2
                        );
                    ho_ConnectedRegions4.Dispose();
                    HOperatorSet.Connection(ho_RegionDifference2, out ho_ConnectedRegions4);
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_EmptyObject_her2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions4, out ho_EmptyObject_her2, "height",
                        "and", hv_h_row * (Z2_L_perforation_length_ / 100.0), 9999);
                    HOperatorSet.CountObj(ho_EmptyObject_her2, out hv_h_erNumber2);

                    //判断2个区域的宽度
                    ho_RegionUnion13.Dispose();
                    HOperatorSet.Union1(ho_EmptyObject_her2, out ho_RegionUnion13);
                    HOperatorSet.SmallestRectangle1(ho_RegionUnion13, out hv_Row14, out hv_Column14,
                        out hv_Row24, out hv_Column24);
                    hv_w_column = hv_Column24 - hv_Column14;
                    //有短的情况下，要有长的才可以判定为NG EmptyObject_herX
                    hv_h_erNumberX = 0;
                    //参数1：2个区域
                    //参数2：大于汇流条设定宽度
                    //参数3：接近汇流条设定宽度
                    if ((int)((new HTuple((new HTuple(hv_h_erNumber2.TupleGreater(1))).TupleAnd(
                        new HTuple(hv_w_column.TupleGreater(hv_Electrode_L))))).TupleAnd(new HTuple(hv_w_column.TupleLess(
                        hv_LherX)))) != 0)
                    {
                        HOperatorSet.SmallestRectangle1(ho_EmptyObject_her2, out hv_Row16, out hv_Column16,
                            out hv_Row26, out hv_Column26);
                        hv_temo_h = hv_Row26 - hv_Row16;
                        HOperatorSet.TupleMin(hv_temo_h, out hv_h_Min);
                        //判断最长的长度
                        hv_hmin_row = hv_h_Min * 1.35;
                        //dev_display (Image_AffinTrans)
                        ho_herX_region.Dispose();
                        HOperatorSet.SelectShape(ho_EmptyObject_her2, out ho_herX_region, "height",
                            "and", hv_hmin_row, 9999);
                        HOperatorSet.CountObj(ho_herX_region, out hv_temper_herX);
                        if ((int)(new HTuple(hv_temper_herX.TupleGreater(0))) != 0)
                        {
                            ho_RegionUnion12.Dispose();
                            HOperatorSet.Union1(ho_EmptyObject_her2, out ho_RegionUnion12);
                            ho_EmptyObject_herX.Dispose();
                            HOperatorSet.CopyObj(ho_RegionUnion12, out ho_EmptyObject_herX, 1, 1);
                            hv_h_erNumberX = 1;
                        }
                    }
                    else
                    {
                        hv_h_erNumberX = 0;
                    }

                    //stop ()

                    hv_h_ERROR_Number2 = 0;
                    ho_h_ERROR_SRegions2.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_h_ERROR_SRegions2);
                    if ((int)((new HTuple((new HTuple(hv_h_ERROR_Number.TupleGreater(0))).TupleOr(
                        new HTuple(hv_h_erNumber.TupleGreater(0))))).TupleOr(new HTuple(hv_h_erNumber2.TupleGreater(
                        0)))) != 0)
                    {

                        //计算90°∟直角特征
                        //dev_display (Image)
                        //dev_display (Image_AffinTrans)
                        ho_RegionUnion4.Dispose();
                        HOperatorSet.Union2(ho_h_ER_sRegions, ho_EmptyObject_her2, out ho_RegionUnion4
                            );
                        ho_RegionUnion6.Dispose();
                        HOperatorSet.Union2(ho_h_sRegions, ho_RegionUnion4, out ho_RegionUnion6
                            );
                        ho_RegionUnion7.Dispose();
                        HOperatorSet.Union1(ho_RegionUnion6, out ho_RegionUnion7);
                        HOperatorSet.SmallestRectangle1(ho_RegionUnion7, out hv_Row13, out hv_Column13,
                            out hv_Row23, out hv_Column23);
                        ho_Rectangle8.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle8, hv_Row13, hv_Column13, 0,
                            500, 23);
                        //dev_display (Image_AffinTrans)
                        ho_RegionDifference4.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle8, ho_Rectangle1, out ho_RegionDifference4
                            );
                        ho_RegionErosion3.Dispose();
                        HOperatorSet.ErosionRectangle1(ho_RegionDifference4, out ho_RegionErosion3,
                            29, 1);
                        //dev_display (Image_AffinTrans)
                        //move_region (RegionDifference4, RegionMoved, -(25/2), 0)
                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_RegionErosion3, out ho_ImageReduced
                            );
                        //dev_display (Image_AffinTrans)
                        ho_ImageMean5.Dispose();
                        HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean5, 30, 1);
                        ho_ImageMean6.Dispose();
                        HOperatorSet.MeanImage(ho_ImageMean5, out ho_ImageMean6, 1, 13);
                        ho_RegionDynThresh2.Dispose();
                        HOperatorSet.DynThreshold(ho_ImageMean5, ho_ImageMean6, out ho_RegionDynThresh2,
                            Z2_L_AfewLines_dyn_threshold_, "dark");
                        ho_RegionClosing4.Dispose();
                        HOperatorSet.ClosingCircle(ho_RegionDynThresh2, out ho_RegionClosing4,
                            1.5);
                        ho_ConnectedRegions8.Dispose();
                        HOperatorSet.Connection(ho_RegionClosing4, out ho_ConnectedRegions8);

                        //dev_display (Image_AffinTrans)
                        //判断90°∟直角特征的位置，
                        //如果靠下面则判断长度超过40%（默认）
                        ho_Rectangle4.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle4, hv_Row2 - ((hv_Row2 - hv_Row1) * (Z2_L_AfewLines_height_ / 100.0)),
                            hv_Column1, hv_Row2, hv_Column2);
                        //调试显示
                        if (show_Z2_L_AfewLines_height_)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "blue");
                            HOperatorSet.DispObj(ho_Rectangle4, hv_WindowID);
                        }
                        //
                        HOperatorSet.SmallestRectangle1(ho_Rectangle4, out hv_Row17, out hv_Column17,
                            out hv_Row27, out hv_Column27);
                        //
                        hv_ocrw = hv_cr_w * (Z2_L_AfewLines_length_ / 100.0);
                        ho_SelectedRegions3.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegions8, out ho_SelectedRegions3,
                            (new HTuple("row")).TupleConcat("width"), "and", hv_Row17.TupleConcat(
                            hv_ocrw), hv_Row27.TupleConcat(9999));
                        //dev_display (Image_AffinTrans)
                        ho_RegionDilation2.Dispose();
                        HOperatorSet.DilationRectangle1(ho_SelectedRegions3, out ho_RegionDilation2,
                            15, 1);
                        ho_RegionDilation3.Dispose();
                        HOperatorSet.DilationRectangle1(ho_RegionUnion6, out ho_RegionDilation3,
                            1, 15);
                        //dev_display (Image_AffinTrans)
                        ho_RegionUnion5.Dispose();
                        HOperatorSet.Union2(ho_RegionDilation2, ho_RegionDilation3, out ho_RegionUnion5
                            );
                        ho_RegionClosing7.Dispose();
                        HOperatorSet.ClosingRectangle1(ho_RegionUnion5, out ho_RegionClosing7,
                            3, 1);
                        ho_ConnectedRegions7.Dispose();
                        HOperatorSet.Connection(ho_RegionClosing7, out ho_ConnectedRegions7);
                        //dev_display (Image_AffinTrans)
                        //
                        hv_hrow_e = hv_h_row * (Z2_L_height_90_ / 100.0);
                        ho_h_ERROR_SRegions2.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegions7, out ho_h_ERROR_SRegions2,
                            (new HTuple("width")).TupleConcat("height"), "and", hv_ocrw.TupleConcat(
                            hv_hrow_e), (new HTuple(9999)).TupleConcat(9999));
                        HOperatorSet.CountObj(ho_h_ERROR_SRegions2, out hv_h_ERROR_Number2);
                        //h_ERROR_Number2 := 0

                    }
                    else
                    {
                        hv_h_ERROR_Number2 = 0;

                    }

                    //stop ()

                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    //mean_image (ImageReduced1, ImageMean3, 3, 3)
                    //mean_image (ImageMean3, ImageMean4, 35, 1)
                    //dyn_threshold (ImageMean3, ImageMean4, RegionDynThresh3, 15, 'light')


                    //stop ()



                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_ImageMean2.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced1, out ho_ImageMean2, 5, 5);
                    ho_ImageInvert.Dispose();
                    HOperatorSet.InvertImage(ho_ImageMean2, out ho_ImageInvert);
                    HOperatorSet.Intensity(ho_Rectangle1, ho_ImageInvert, out hv_Mean, out hv_Deviation);
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    //threshold (ImageInvert, Region, 200, 255)
                    hv_m_threshold = hv_Mean - hv_Deviation;
                    ho_RegionDynThresh1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMean2, ho_ImageInvert, out ho_RegionDynThresh1,
                        hv_m_threshold, "dark");
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionDynThresh1, out ho_RegionClosing, 3.5);


                    //拟合下面的区域 Row11, Column11, Row21, Column21
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_Rectangle7.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle7, hv_Row11 - 7, hv_Column11, hv_Row21,
                        hv_Column21);
                    ho_RegionIntersection1.Dispose();
                    HOperatorSet.Intersection(ho_Rectangle7, ho_RegionClosing, out ho_RegionIntersection1
                        );
                    ho_RegionClosing3_w.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionIntersection1, out ho_RegionClosing3_w,
                        hv_cr_w * 0.1, 1);
                    ho_RegionUnion3_w.Dispose();
                    HOperatorSet.Union2(ho_RegionClosing, ho_RegionClosing3_w, out ho_RegionUnion3_w
                        );


                    //
                    //获取分析区域，面积10%
                    HOperatorSet.AreaCenter(ho_Rectangle1, out hv_Area4, out hv_Row9, out hv_Column9);
                    hv_r1areaa = hv_Area4 * 0.06;
                    //检查包裹度
                    ho_ConnectedRegions5.Dispose();
                    HOperatorSet.Connection(ho_RegionUnion3_w, out ho_ConnectedRegions5);
                    //area_center (ConnectedRegions5, Area3, Row6, Column6)
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_dw_SRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions5, out ho_dw_SRegions, (new HTuple("area")).TupleConcat(
                        "height"), "and", hv_r1areaa.TupleConcat(50), (((hv_cr_w * hv_h_row) + 1)).TupleConcat(
                        9999));
                    HOperatorSet.CountObj(ho_dw_SRegions, out hv_dw_erNumber);


                    //判断宽度包裹宽度
                    if ((int)(new HTuple(hv_dw_erNumber.TupleGreater(0))) != 0)
                    {
                        //dev_display (Image)
                        //dev_display (Image_AffinTrans)
                        //gen_rectangle1 (Rectangle_1up, Row11, Column11, Row11+h_row*0.4, Column21)
                        ho_Rectangle_left.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle_left, hv_Row21 - (hv_h_row * Z2_r1_hrow_ / 100.0),
                            hv_Column11, hv_Row21, hv_Column11 + (hv_cr_w * (Z2_r1_crw_ / 100.0)));
                        ho_Rectangle_right.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle_right, hv_Row21 - (hv_h_row * Z2_r1_hrow_ / 100.0),
                            hv_Column21 - (hv_cr_w * (Z2_r1_crw_ / 100.0)), hv_Row21, hv_Column21);
                        ho_RegionUnion2.Dispose();
                        HOperatorSet.Union2(ho_Rectangle_left, ho_Rectangle_right, out ho_RegionUnion2
                            );
                        //调试显示
                        if (show_Z2_r1_hrow_crw_)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_RegionUnion2, hv_WindowID);
                        }
                        //
                        //上区域
                        //dev_display (Image_AffinTrans)
                        //area_center (Rectangle_1up, Area_1up, Row_1up, Column_1up)
                        //intersection (Rectangle_1up, dw_SRegions, RegionIntersection3)
                        //*         connection (RegionIntersection3, ConnectedRegions10)
                        //dev_display (Image_AffinTrans)
                        //*         r1areaa2 := Area_1up*0.1
                        //*         select_shape (ConnectedRegions10, up_SRegions2, ['area','height'], 'and', [r1areaa2, h_row*0.4*0.5], [999999999,9999])
                        //*         count_obj (up_SRegions2, up_erNumber1)

                        //
                        //dev_display (Image_AffinTrans)
                        ho_RegionDifference3.Dispose();
                        HOperatorSet.Intersection(ho_RegionUnion2, ho_dw_SRegions, out ho_RegionDifference3
                            );
                        ho_RegionClosing3.Dispose();
                        HOperatorSet.ClosingRectangle1(ho_RegionDifference3, out ho_RegionClosing3,
                            1, hv_h_row * 0.21);
                        ho_RegionFillUp2.Dispose();
                        HOperatorSet.FillUp(ho_RegionClosing3, out ho_RegionFillUp2);
                        ho_ConnectedRegions6.Dispose();
                        HOperatorSet.Connection(ho_RegionFillUp2, out ho_ConnectedRegions6);

                        //左右单个方块的面积
                        //dev_display (Image_AffinTrans)
                        HOperatorSet.AreaCenter(ho_Rectangle_left, out hv_Area, out hv_Row3, out hv_Column3);
                        hv_lr_erArea = hv_Area * (Z2_r1area_ / 100.0);
                        //
                        ho_lr_SRegions2.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegions6, out ho_lr_SRegions2, (new HTuple("area")).TupleConcat(
                            "height"), "and", hv_lr_erArea.TupleConcat(hv_h_row * 0.2), (((hv_cr_w * hv_h_row) + 1)).TupleConcat(
                            9999));
                        HOperatorSet.CountObj(ho_lr_SRegions2, out hv_lr_erNumber2);

                        //判断左右黑色区域数量
                        hv_LR_erNumberX = 0;
                        if ((int)(new HTuple(hv_lr_erNumber2.TupleLess(Z2_r1_number_))) != 0)
                        {
                            hv_LR_erNumberX = 1;
                            ho_LR_SRegionsX.Dispose();
                            HOperatorSet.Union2(ho_Rectangle1, ho_lr_SRegions2, out ho_LR_SRegionsX
                                );

                        }
                        //
                        //*         if ((up_erNumber1+lr_erNumber2)<2)
                        //LR_erNumberX := 1
                        //*         union2 (up_SRegions2, lr_SRegions2, LR_SRegionsX)

                        //*         else
                        //
                        //*         endif


                    }

                    //stop ()

                    //*     h_row ()
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_Rectangle9.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle9, hv_Row11, hv_Column11, hv_Row11 + 29,
                        hv_Column21);
                    //检查焊锡包裹密度
                    //dev_display (Image_AffinTrans)
                    ho_RegionUnion11.Dispose();
                    HOperatorSet.Union2(ho_RegionClosing, ho_Rectangle9, out ho_RegionUnion11
                        );

                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_RegionFillUp1.Dispose();
                    HOperatorSet.FillUp(ho_RegionUnion11, out ho_RegionFillUp1);
                    //dev_display (Image_AffinTrans)
                    ho_RegionDifference5.Dispose();
                    HOperatorSet.Difference(ho_RegionFillUp1, ho_Rectangle9, out ho_RegionDifference5
                        );

                    hv__w = hv_cr_w * 0.3;
                    hv__h = hv_h_row * 0.6;
                    //dev_display (Image_AffinTrans)
                    ho_RegionErosion4.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Rectangle1, out ho_RegionErosion4, hv__w,
                        hv__h);
                    //获取中心部分未包裹区域，用于判断
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionErosion4, ho_RegionClosing, out ho_RegionDifference
                        );


                    //dev_display (Image_AffinTrans)
                    ho_RegionIntersection.Dispose();
                    HOperatorSet.Difference(ho_RegionDifference5, ho_RegionClosing, out ho_RegionIntersection
                        );

                    //dev_display (Image_AffinTrans)
                    ho_RegionUnion3.Dispose();
                    HOperatorSet.Union2(ho_RegionDifference, ho_RegionIntersection, out ho_RegionUnion3
                        );


                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_RegionClosing5.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionUnion3, out ho_RegionClosing5, 1.5);
                    ho_RegionFillUp3.Dispose();
                    HOperatorSet.FillUp(ho_RegionClosing5, out ho_RegionFillUp3);
                    //
                    ho_ConnectedRegions2.Dispose();
                    HOperatorSet.Connection(ho_RegionFillUp3, out ho_ConnectedRegions2);
                    HOperatorSet.AreaCenter(ho_ConnectedRegions2, out hv_Area1, out hv_Row4,
                        out hv_Column4);
                    HOperatorSet.TupleMax(hv_Area1, out hv_Max1);
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    //M_area := Area4*0.13
                    ho_area_erSRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_area_erSRegions, "area",
                        "and", hv_Max1 * (Z2_r2area_ / 100.0), (hv_cr_w * hv_h_row) + 1);//存在两个区域，最小的区域必须大于最大的区域的50%才算NG
                    HOperatorSet.CountObj(ho_area_erSRegions, out hv_areaNumber1);
                    ho_RegionUnion10.Dispose();
                    HOperatorSet.Union1(ho_area_erSRegions, out ho_RegionUnion10);
                    HOperatorSet.SmallestRectangle1(ho_RegionUnion10, out hv_Row15, out hv_Column15,
                        out hv_Row25, out hv_Column25);
                    hv_a_height = hv_Row25 - hv_Row15;
                    hv_a_width = hv_Column25 - hv_Column15;
                    if ((int)((new HTuple(hv_areaNumber1.TupleGreater(1))).TupleAnd(new HTuple(hv_a_height.TupleGreater(
                        hv_h_row * (Z2_r2_aHeight_ / 100.0))))) != 0)
                    {
                        hv_area_erNumber = 1;
                    }
                    else if ((int)((new HTuple(hv_areaNumber1.TupleGreater(1))).TupleAnd(
                        new HTuple(hv_a_width.TupleGreater(hv_cr_w * (Z2_r2_awidth_ / 100.0))))) != 0)
                    {
                        hv_area_erNumber = 1;
                    }
                    else
                    {
                        hv_area_erNumber = 0;
                    }
                    //count_obj (area_erSRegions, area_erNumber)


                    ho_Rectangle_bkd.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle_bkd, hv_Row11, hv_Column11, hv_Row11 + 31,
                        hv_Column21);
                    //判断焊锡包裹度
                    //拟合上面的区域
                    //dev_display (Image)
                    //dev_display (Image_AffinTrans)
                    ho_RegionIntersection2.Dispose();
                    HOperatorSet.Intersection(ho_Rectangle_bkd, ho_RegionClosing, out ho_RegionIntersection2
                        );
                    //dev_display (Image_AffinTrans)
                    ho_RegionClosing6.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionIntersection2, out ho_RegionClosing6,
                        hv_cr_w * 0.5, 1);
                    //dev_display (Image_AffinTrans)
                    ho_RegionUnion8.Dispose();
                    HOperatorSet.Union2(ho_RegionClosing6, ho_dw_SRegions, out ho_RegionUnion8
                        );
                    //dev_display (Image_AffinTrans)
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionUnion8, out ho_RegionFillUp);
                    //dev_display (Image_AffinTrans)
                    ho_RegionIntersection4.Dispose();
                    HOperatorSet.Difference(ho_RegionFillUp, ho_RegionUnion8, out ho_RegionIntersection4
                        );
                    ho_RegionUnion9.Dispose();
                    HOperatorSet.Union1(ho_RegionIntersection4, out ho_RegionUnion9);
                    ho_ConnectedRegions9.Dispose();
                    HOperatorSet.Connection(ho_RegionUnion9, out ho_ConnectedRegions9);
                    //dev_display (Image_AffinTrans)
                    ho_dh_Sregion.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions9, out ho_dh_Sregion, ((new HTuple("area")).TupleConcat(
                        "width")).TupleConcat("height"), "and", (((((hv_cr_w * hv_h_row) * (Z2_r3area_ / 100.0))).TupleConcat(
                        hv_cr_w * (Z2_r3_crw_min_ / 100.0)))).TupleConcat(hv_h_row * (Z2_r3_hrow_ / 100.0)), (((((hv_cr_w * hv_h_row) + 1)).TupleConcat(
                        hv_cr_w * (Z2_r3_crw_max_ / 100.0)))).TupleConcat(hv_h_row + 1));
                    HOperatorSet.CountObj(ho_dh_Sregion, out hv_dh_ErNumber);

                    //dh_ErNumber := 0

                    //stop ()


                    //dev_display (ImageGauss1)
                    //dev_display (Image_AffinTrans)
                    //1：翘起的数量       最大允许长度（超过设定最大比例也判定为NG）
                    if (star_Z2_L_ == 0)
                    {
                        hv_h_ERROR_Number = 0;
                    }
                    if (star_Z2_L_rw_ == 0)
                    {
                        hv_h_erNumber = 0;
                    }
                    //2：穿孔附近链接的严重翘起
                    //3：计算90°∟直角特征
                    if (star_Z2_L2_ == 0)
                    {
                        hv_h_erNumberX = 0;
                        hv_h_ERROR_Number2 = 0;
                    }
                    //4：无包裹为NG
                    //5：包裹不完整，却一边
                    if (star_Z2_r1area_ == 0)
                    {
                        hv_dw_erNumber = 0;
                        hv_LR_erNumberX = 0;
                    }
                    //6：判断焊锡包裹密度
                    if (star_Z2_r2area_ == 0)
                    {
                        hv_area_erNumber = 0;
                    }
                    //7：判断焊锡包裹圆度
                    if (star_Z2_r3area_ == 0)
                    {
                        hv_dh_ErNumber = 0;
                    }
                    //
                    if ((int)((new HTuple((new HTuple((new HTuple((new HTuple((new HTuple((new HTuple((new HTuple(hv_h_erNumber.TupleGreater(
                        1))).TupleOr(new HTuple(hv_h_ERROR_Number.TupleGreater(0))))).TupleOr(
                        new HTuple(hv_h_erNumberX.TupleGreater(0))))).TupleOr(new HTuple(hv_h_ERROR_Number2.TupleGreater(
                        0))))).TupleOr(new HTuple(hv_dw_erNumber.TupleLess(1))))).TupleOr(new HTuple(hv_LR_erNumberX.TupleGreater(
                        0))))).TupleOr(new HTuple(hv_area_erNumber.TupleGreater(0))))).TupleOr(
                        new HTuple(hv_dh_ErNumber.TupleGreater(0)))) != 0)
                    {

                        //1：翘起的数量       最大允许长度（超过设定最大比例也判定为NG）
                        if ((int)(new HTuple(hv_h_ERROR_Number.TupleGreater(0))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_h_ER_sRegions, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_h_ER_sRegions, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                        }
                        else if ((int)(new HTuple(hv_h_erNumber.TupleGreater(1))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_EmptyObject_her, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_EmptyObject_her, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //2：穿孔附近链接的严重翘起
                        }
                        else if ((int)(new HTuple(hv_h_erNumberX.TupleGreater(0))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_EmptyObject_her2, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_EmptyObject_herX, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //3：计算90°∟直角特征
                        }
                        else if ((int)(new HTuple(hv_h_ERROR_Number2.TupleGreater(0))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_EmptyObject_her2, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_h_ERROR_SRegions2, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //4：无包裹为NG
                        }
                        else if ((int)(new HTuple(hv_dw_erNumber.TupleLess(1))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_Rectangle1, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //5：包裹不完整，却一边
                        }
                        else if ((int)(new HTuple(hv_LR_erNumberX.TupleGreater(0))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_LR_SRegionsX, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //6：判断焊锡包裹密度
                        }
                        else if ((int)(new HTuple(hv_area_erNumber.TupleGreater(0))) != 0)
                        {
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_area_erSRegions, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_area_erSRegions, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }
                            //7：判断焊锡包裹圆度
                        }
                        else
                        {
                            //*             if (dh_ErNumber>1)
                            HOperatorSet.SetColor(hv_WindowID, "red");
                            HOperatorSet.DispObj(ho_dh_Sregion, hv_WindowID);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_out_EO_error_region, ho_dh_Sregion, out ExpTmpOutVar_0
                                    );
                                ho_out_EO_error_region.Dispose();
                                ho_out_EO_error_region = ExpTmpOutVar_0;
                            }

                        }
                        disp_message(hv_WindowID, "NG", "image", (hv_row_2 + hv_Q_row) - 200,
                            hv_column_2 + hv_Q_PY_temp_spacing, "red", "false");

                        hv_out_temp_error = hv_out_temp_error + 1;

                    }
                    else
                    {
                        HOperatorSet.SetColor(hv_WindowID, "green");
                        HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                        disp_message(hv_WindowID, "OK", "image", (hv_row_2 + hv_Q_row) - 200,
                            hv_column_2 + hv_Q_PY_temp_spacing, "green", "false");
                    }

                    //stop ()

                    hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;


                }

                ho_EmptyObject_her.Dispose();
                ho_EmptyObject_herX.Dispose();
                ho_Rectangle1.Dispose();
                ho_LR_SRegionsX.Dispose();
                ho_area_erSRegions.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Rectangle7_t.Dispose();
                ho_ImageReduced_t.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionErosion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_h_ER_sRegions.Dispose();
                ho_h_sRegions.Dispose();
                ho_EmptyObject_l.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing_er.Dispose();
                ho_Rectangle3.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDilation1.Dispose();
                ho_RegionUnion.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_RegionDifference2.Dispose();
                ho_ConnectedRegions4.Dispose();
                ho_EmptyObject_her2.Dispose();
                ho_RegionUnion13.Dispose();
                ho_herX_region.Dispose();
                ho_RegionUnion12.Dispose();
                ho_h_ERROR_SRegions2.Dispose();
                ho_RegionUnion4.Dispose();
                ho_RegionUnion6.Dispose();
                ho_RegionUnion7.Dispose();
                ho_Rectangle8.Dispose();
                ho_RegionDifference4.Dispose();
                ho_RegionErosion3.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean5.Dispose();
                ho_ImageMean6.Dispose();
                ho_RegionDynThresh2.Dispose();
                ho_RegionClosing4.Dispose();
                ho_ConnectedRegions8.Dispose();
                ho_Rectangle4.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_RegionDilation2.Dispose();
                ho_RegionDilation3.Dispose();
                ho_RegionUnion5.Dispose();
                ho_RegionClosing7.Dispose();
                ho_ConnectedRegions7.Dispose();
                ho_ImageMean2.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle7.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_RegionClosing3_w.Dispose();
                ho_RegionUnion3_w.Dispose();
                ho_ConnectedRegions5.Dispose();
                ho_dw_SRegions.Dispose();
                ho_Rectangle_left.Dispose();
                ho_Rectangle_right.Dispose();
                ho_RegionUnion2.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionClosing3.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_lr_SRegions2.Dispose();
                ho_Rectangle9.Dispose();
                ho_RegionUnion11.Dispose();
                ho_RegionFillUp1.Dispose();
                ho_RegionDifference5.Dispose();
                ho_RegionErosion4.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionUnion3.Dispose();
                ho_RegionClosing5.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_RegionUnion10.Dispose();
                ho_Rectangle_bkd.Dispose();
                ho_RegionIntersection2.Dispose();
                ho_RegionClosing6.Dispose();
                ho_RegionUnion8.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionIntersection4.Dispose();
                ho_RegionUnion9.Dispose();
                ho_ConnectedRegions9.Dispose();
                ho_dh_Sregion.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_EmptyObject_her.Dispose();
                ho_EmptyObject_herX.Dispose();
                ho_Rectangle1.Dispose();
                ho_LR_SRegionsX.Dispose();
                ho_area_erSRegions.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Rectangle7_t.Dispose();
                ho_ImageReduced_t.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionErosion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_h_ER_sRegions.Dispose();
                ho_h_sRegions.Dispose();
                ho_EmptyObject_l.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing_er.Dispose();
                ho_Rectangle3.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDilation1.Dispose();
                ho_RegionUnion.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_RegionDifference2.Dispose();
                ho_ConnectedRegions4.Dispose();
                ho_EmptyObject_her2.Dispose();
                ho_RegionUnion13.Dispose();
                ho_herX_region.Dispose();
                ho_RegionUnion12.Dispose();
                ho_h_ERROR_SRegions2.Dispose();
                ho_RegionUnion4.Dispose();
                ho_RegionUnion6.Dispose();
                ho_RegionUnion7.Dispose();
                ho_Rectangle8.Dispose();
                ho_RegionDifference4.Dispose();
                ho_RegionErosion3.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean5.Dispose();
                ho_ImageMean6.Dispose();
                ho_RegionDynThresh2.Dispose();
                ho_RegionClosing4.Dispose();
                ho_ConnectedRegions8.Dispose();
                ho_Rectangle4.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_RegionDilation2.Dispose();
                ho_RegionDilation3.Dispose();
                ho_RegionUnion5.Dispose();
                ho_RegionClosing7.Dispose();
                ho_ConnectedRegions7.Dispose();
                ho_ImageMean2.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle7.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_RegionClosing3_w.Dispose();
                ho_RegionUnion3_w.Dispose();
                ho_ConnectedRegions5.Dispose();
                ho_dw_SRegions.Dispose();
                ho_Rectangle_left.Dispose();
                ho_Rectangle_right.Dispose();
                ho_RegionUnion2.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionClosing3.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_lr_SRegions2.Dispose();
                ho_Rectangle9.Dispose();
                ho_RegionUnion11.Dispose();
                ho_RegionFillUp1.Dispose();
                ho_RegionDifference5.Dispose();
                ho_RegionErosion4.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionUnion3.Dispose();
                ho_RegionClosing5.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_RegionUnion10.Dispose();
                ho_Rectangle_bkd.Dispose();
                ho_RegionIntersection2.Dispose();
                ho_RegionClosing6.Dispose();
                ho_RegionUnion8.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionIntersection4.Dispose();
                ho_RegionUnion9.Dispose();
                ho_ConnectedRegions9.Dispose();
                ho_dh_Sregion.Dispose();

                //       throw HDevExpDefaultException;

            }
        }

        //协鑫焊后正面识别算法 20180205
        public void Z2_str_Welding_recognition_z_Xx(HObject ho_Image, HObject ho_Image_AffinTrans,
          out HObject ho_ResultOOK, out HObject ho_ResultONG, HTuple hv_row_2, HTuple hv_column_2,
          HTuple hv_WindowID, out HTuple hv_out_temp_error)
        {

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ConnectedRegions, ho_RegionsSelected;
            HObject ho_Rectangle1 = null, ho_ImageReduced1 = null, ho_ImageMean = null;
            HObject ho_ImageMean1 = null, ho_RegionDynThresh = null, ho_ConnectedRegion = null;
            HObject ho_ObjectH = null, ho_RegionSelecteds = null, ho_RegionTran = null;
            HObject ho_RegionTrans = null, ho_ImageMean2 = null, ho_ImageInvert = null;
            HObject ho_DynThreshRegion = null, ho_Rectangle2 = null, ho_RegionIntersection = null;

            // HObject ho_ResultObjectOK = null, ho_ResultObjectNG = null;
            // Local control variables 
            HTuple hv_Q_row = null, hv_Q_column = null;
            HTuple hv_Q_HJ_spacing = null, hv_Q_PY_temp_spacing = null;
            HTuple hv_I = null, hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple();
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Row22 = new HTuple(), hv_Column22 = new HTuple();
            HTuple hv_WHeight = new HTuple(), hv_i = new HTuple();
            HTuple hv_WHeight_Lmax = new HTuple(), hv_j = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Row14 = new HTuple(), hv_Column14 = new HTuple();
            HTuple hv_Row24 = new HTuple(), hv_Column24 = new HTuple();
            HTuple hv_WWidth = new HTuple(), hv_Number = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_m_threshold = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row4 = new HTuple(), hv_Column4 = new HTuple();
            HTuple hv_Num2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ResultOOK);
            HOperatorSet.GenEmptyObj(out ho_ResultONG);

            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionsSelected);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegion);
            HOperatorSet.GenEmptyObj(out ho_ObjectH);
            HOperatorSet.GenEmptyObj(out ho_RegionSelecteds);
            HOperatorSet.GenEmptyObj(out ho_RegionTran);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageMean2);
            HOperatorSet.GenEmptyObj(out ho_ImageInvert);
            HOperatorSet.GenEmptyObj(out ho_DynThreshRegion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            //输出缺陷数量
            hv_out_temp_error = 0;
            //
            try
            {

                //NG区域记录
                //ho_out_EO_error_region.Dispose();
                //HOperatorSet.GenEmptyObj(out ho_out_EO_error_region);
                //
                //      hv_out_temp_error = 0;

                //起始位置R
                hv_Q_row = Z2_CenterRow;
                //起始位置W
                hv_Q_column = Z2_CenterCol;
                //每个焊接位置的间距
                hv_Q_HJ_spacing = Z2_WeldDis;
                //每次偏移位置累加
                hv_Q_PY_temp_spacing = hv_Q_column + 0;

                HOperatorSet.DispObj(ho_Image_AffinTrans, hv_WindowID);
                ho_ResultOOK.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultOOK);
                ho_ResultONG.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultONG);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                ho_RegionsSelected.Dispose();
                HOperatorSet.GenEmptyObj(out ho_RegionsSelected);

                for (hv_I = 0; (int)hv_I <= 3; hv_I = (int)hv_I + 1)
                {
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                        0, Z2_WeldW, Z2_WeldH);
                    //显示调试区域
                    if (Z2_show_rectangle_z)
                    {
                        HOperatorSet.SetColor(hv_WindowID, "yellow");
                        HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                    }

                    //测试位置
                    hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;
                    //stop ()
                    HOperatorSet.SetColor(hv_WindowID, "yellow");
                    HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);

                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle1, out ho_ImageReduced1
                        );
                    HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                        out hv_Row2, out hv_Column2);
                    hv_h_row = hv_Row2 - hv_Row1;
                    hv_cr_w = hv_Column2 - hv_Column1;
                    //**********************************************
                    //stop ()
                    //提取未焊接之汇流条
                    //zoom_image_factor (ImageReduced1, ImageReduced2, 0.5, 0.5, 'constant')
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced1, out ho_ImageMean, 1, 30);
                    ho_ImageMean1.Dispose();
                    HOperatorSet.MeanImage(ho_ImageMean, out ho_ImageMean1, Z2_L_dyn_threshold_Xx, 1);
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                        15, "dark");
                    ho_ConnectedRegion.Dispose();
                    HOperatorSet.Connection(ho_RegionDynThresh, out ho_ConnectedRegion);
                    HOperatorSet.SmallestRectangle1(ho_ConnectedRegion, out hv_Row12, out hv_Column12,
                        out hv_Row22, out hv_Column22);
                    hv_WHeight = hv_Row22 - hv_Row12;
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShape(ho_ConnectedRegion, out ExpTmpOutVar_0, "height",
                            "and", hv_h_row * (Z2_L_height_max_Xx / 100), 999999);
                        ho_ConnectedRegion.Dispose();
                        ho_ConnectedRegion = ExpTmpOutVar_0;
                    }
                    ho_ObjectH.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ObjectH);
                    for (hv_i = 0; (int)hv_i <= 1; hv_i = (int)hv_i + 1)
                    {
                        try
                        {
                            HOperatorSet.TupleMax(hv_WHeight, out hv_WHeight_Lmax);
                            //if ((int)(new HTuple(hv_WHeight_Lmax.TupleLess(hv_h_row / 2.5))) != 0)
                            //{
                            //    hv_WHeight_Lmax = hv_h_row / 2.5;
                            //}
                            ho_RegionSelecteds.Dispose();
                            HOperatorSet.SelectShape(ho_ConnectedRegion, out ho_RegionSelecteds,
                                "height", "and", hv_WHeight_Lmax, 9999);
                            //获取最大值的位置
                            HOperatorSet.TupleFind(hv_WHeight, hv_WHeight_Lmax, out hv_j);
                            //删除索引位置的值
                            HOperatorSet.TupleRemove(hv_WHeight, hv_j, out hv_WHeight);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_ObjectH, ho_RegionSelecteds, out ExpTmpOutVar_0
                                    );
                                ho_ObjectH.Dispose();
                                ho_ObjectH = ExpTmpOutVar_0;
                            }
                            HOperatorSet.TupleLength(hv_j, out hv_Length);
                            if ((int)(new HTuple(hv_Length.TupleGreaterEqual(2))) != 0)
                            {
                                hv_i = 2;
                            }
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        }
                    }
                    ho_RegionTran.Dispose();
                    HOperatorSet.ShapeTrans(ho_ObjectH, out ho_RegionTran, "convex");
                    HOperatorSet.SmallestRectangle1(ho_RegionTran, out hv_Row14, out hv_Column14,
                        out hv_Row24, out hv_Column24);
                    hv_WWidth = hv_Column24 - hv_Column14;
                    if ((int)(new HTuple(hv_WWidth.TupleGreater(80))) != 0)
                    {
                        ho_RegionTrans.Dispose();
                        HOperatorSet.CopyObj(ho_RegionTran, out ho_RegionTrans, 1, 1);
                    }
                    else
                    {
                        ho_RegionTrans.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_RegionTrans);
                    }
                    //stop ()
                    HOperatorSet.CountObj(ho_RegionTrans, out hv_Number);

                    //提取虚焊之汇流条
                    if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                    {
                        //************************************************************
                        ho_ImageMean2.Dispose();
                        HOperatorSet.MeanImage(ho_ImageReduced1, out ho_ImageMean2, 5, 5);
                        ho_ImageInvert.Dispose();
                        HOperatorSet.InvertImage(ho_ImageMean2, out ho_ImageInvert);
                        HOperatorSet.Intensity(ho_Rectangle1, ho_ImageInvert, out hv_Mean, out hv_Deviation);
                        hv_m_threshold = hv_Mean - hv_Deviation;
                        ho_DynThreshRegion.Dispose();
                        HOperatorSet.DynThreshold(ho_ImageMean2, ho_ImageInvert, out ho_DynThreshRegion,
                            hv_m_threshold, "dark");
                        ho_DynThreshRegion.Dispose();
                        HOperatorSet.Threshold(ho_ImageInvert, out ho_DynThreshRegion, hv_m_threshold + Z2_L_threshold_Xx,
                            255);
                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_DynThreshRegion, out ho_ConnectedRegions);
                        ho_RegionsSelected.Dispose();
                        HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_RegionsSelected,
                            "max_area", 70);
                        ho_Rectangle2.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row1 + 15, hv_Column1 + 15,
                            hv_Row2 - 15, hv_Column2 - 15);
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle2, ho_RegionsSelected, out ho_RegionIntersection
                            );
                        HOperatorSet.AreaCenter(ho_Rectangle2, out hv_Area, out hv_Row4, out hv_Column4);
                        ho_RegionsSelected.Dispose();
                        HOperatorSet.SelectShape(ho_RegionIntersection, out ho_RegionsSelected,
                            "area", "and", hv_Area * (Z2_L_Back_Area_Xx / 100.0), hv_Area);
                        HOperatorSet.CountObj(ho_RegionsSelected, out hv_Num2);
                    }




                    //功能选项按钮20180208
                    if (star_Z2_L_rw_Xx == 0)
                    {
                        hv_Number = 0;
                    }
                    //功能选项按钮20180208
                    if (star_Z2_L_rc_Xx == 0)
                    {
                        hv_Num2 = 0;
                    }




                    //stop ()
                    if ((int)((new HTuple(hv_Num2.TupleGreater(0))).TupleOr(new HTuple(hv_Number.TupleGreater(
                        0)))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultONG, ho_Rectangle1, out ExpTmpOutVar_0
                                );
                            ho_ResultONG.Dispose();
                            ho_ResultONG = ExpTmpOutVar_0;
                        }
                        hv_out_temp_error = hv_out_temp_error + 1;
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultOOK, ho_Rectangle1, out ExpTmpOutVar_0
                                );
                            ho_ResultOOK.Dispose();
                            ho_ResultOOK = ExpTmpOutVar_0;
                        }
                    }

                }
                ho_ConnectedRegions.Dispose();
                ho_RegionsSelected.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegion.Dispose();
                ho_ObjectH.Dispose();
                ho_RegionSelecteds.Dispose();
                ho_RegionTran.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageMean2.Dispose();
                ho_ImageInvert.Dispose();
                ho_DynThreshRegion.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionIntersection.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ConnectedRegions.Dispose();
                ho_RegionsSelected.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegion.Dispose();
                ho_ObjectH.Dispose();
                ho_RegionSelecteds.Dispose();
                ho_RegionTran.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageMean2.Dispose();
                ho_ImageInvert.Dispose();
                ho_DynThreshRegion.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionIntersection.Dispose();

                //       throw HDevExpDefaultException;

            }
        }

        public float RowOK;
        public float ColumnOK;
        public float RowNG;
        public float ColumnNG;

        public int  Row1;
        public int Column1;
        public int width;
        public int height;

        //正面焊后结果显示20180205
        public void Show_Weld_Results(HObject ho_Image_AffinTrans, HObject ho_ResultONG,
                                    HObject ho_ResultOOK, HTuple hv_WindowID)
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
                /////////////////////////////////////////////////////////////////////////
                //string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\" + FindSN_Module + ".BMP";
                //Graphics g = Graphics.FromImage(img);
                //g.DrawImage(img, 0, 0, img.Width, img.Height);
                //Font frpnt = new Font("宋体", 70);
                //SolidBrush sbrush = new SolidBrush(Color.Green);
                //string TimeNow = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒");
                //g.DrawString(FindSN_Module, frpnt, sbrush, 100, img.Height - 200);
                //g.DrawString(TimeNow, frpnt, sbrush, 100, img.Height - 100);
                ///////////////////////////////////////////////////////////////////////
                HOperatorSet.SetColor(hv_WindowID, "red");
                HOperatorSet.DispObj(ho_ResultONG, hv_WindowID);
                HOperatorSet.SetColor(hv_WindowID, "green");
                HOperatorSet.DispObj(ho_ResultOOK, hv_WindowID);
                set_display_font(hv_WindowID, 25, "mono", "true", "false");
                ///////////////////////////////////////////////////////////////////////
                HOperatorSet.CountObj(ho_ResultOOK, out hv_NumOK);
                HOperatorSet.CountObj(ho_ResultONG, out hv_NumNG);
                HTuple end_val7 = hv_NumOK;
                HTuple step_val7 = 1;
                for (hv_j = 1; hv_j.Continue(end_val7, step_val7); hv_j = hv_j.TupleAdd(step_val7))
                {
                    HOperatorSet.SetColor(hv_WindowID, "green");
                    ho_SelectedObjectOK.Dispose();
                    HOperatorSet.SelectObj(ho_ResultOOK, out ho_SelectedObjectOK, hv_j);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK,
                        out hv_Radius);
                    HOperatorSet.SmallestRectangle1(ho_SelectedObjectOK, out hv_Row1, out hv_Column1,
                                                 out hv_Row2, out hv_Column2);
                    hv_width = hv_Column2 - hv_Column1;
                    hv_height = hv_Row2 - hv_Row1;
                    
                    disp_message(hv_WindowID, "OK", "image", hv_RowOK + 200, hv_ColumnOK - 50, "green", "false");
                    //新增20180703
                    //RowOK = float.Parse(hv_RowOK.ToString());
                    //ColumnOK = float.Parse(hv_ColumnOK.ToString());
                    //g.DrawString("OK", frpnt, sbrush, ColumnOK - 50, RowOK + 200);
                    //try
                    //{
                    //////Log("高清图像1");
                    //Row1 = int.Parse(hv_Row1.ToString());
                    //////Log("高清图像2");
                    //Column1 = int.Parse(hv_Column1.ToString());
                    //////Log("高清图像3");
                    //width = int.Parse(hv_width.ToString());
                    //////Log("高清图像4");
                    //height = int.Parse(hv_height.ToString());
                    //Log("高清图像5");
                    //Pen pen = new Pen(Color.Green, 6);
                    //Log("高清图像6");
                    //g.DrawRectangle(pen, Column1, Row1, width, height);
                    //Log("高清图像7");
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log("高清图像8"+ex.Message);
                    //}
                }
              
                

                HTuple end_val15 = hv_NumNG;
                HTuple step_val15 = 1;
                for (hv_k = 1; hv_k.Continue(end_val15, step_val15); hv_k = hv_k.TupleAdd(step_val15))
                {
                    HOperatorSet.SetColor(hv_WindowID, "red");
                    ho_SelectedObjectNG.Dispose();
                    HOperatorSet.SelectObj(ho_ResultONG, out ho_SelectedObjectNG, hv_k);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG,
                        out hv_ColumnNG, out hv_Radius);
                    HOperatorSet.SmallestRectangle1(ho_SelectedObjectNG, out hv_Row1, out hv_Column1,
                                                    out hv_Row2, out hv_Column2);
                    hv_width = hv_Column2 - hv_Column1;
                    hv_height = hv_Row2 - hv_Row1;

                    disp_message(hv_WindowID, "NG", "image", hv_RowNG + 200, hv_ColumnNG - 50, "red", "false");
                    //新增20180703
                    //RowNG = float.Parse(hv_RowNG.ToString());
                    //ColumnNG = float.Parse(hv_ColumnNG.ToString());
                    //SolidBrush brush = new SolidBrush(Color.Red);
                    //g.DrawString("NG", frpnt, brush, ColumnNG - 50, RowNG + 200);
                    //try
                    //{
                    //    ////Log("高清图像1");
                    //    Row1 = int.Parse(hv_Row1.ToString());
                    //    ////Log("高清图像2");
                    //    Column1 = int.Parse(hv_Column1.ToString());
                    //    ////Log("高清图像3");
                    //    width = int.Parse(hv_width.ToString());
                    //    ////Log("高清图像4");
                    //    height = int.Parse(hv_height.ToString());
                    //    Log("高清图像5");
                    //    Pen penNG = new Pen(Color.Red, 6);
                    //    Log("高清图像6");
                    //    g.DrawRectangle(penNG, Column1, Row1, width, height);
                    //    Log("高清图像7");
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log("高清图像8" + ex.Message);
                    //}
                    

                }

                //Log("高清图像15");
                //img.Save(data1, System.Drawing.Imaging.ImageFormat.Bmp);
                //Log("高清图像16");
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

        public void Show_Weld_Results(HObject ho_Image_AffinTrans, HObject ho_ResultONG,
                                HObject ho_ResultOOK, HTuple hv_WindowID, Bitmap img)
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
                /////////////////////////////////////////////////////////////////////////
                //string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\" + FindSN_Module + ".BMP";
                Graphics g = Graphics.FromImage(img);
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                Font frpnt = new Font("宋体", 70);
                SolidBrush sbrush = new SolidBrush(Color.Green);
                string TimeNow = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒");
                g.DrawString(FindSN_Module, frpnt, sbrush, 100, img.Height - 200);
                g.DrawString(TimeNow, frpnt, sbrush, 100, img.Height - 100);
                ///////////////////////////////////////////////////////////////////////
                HOperatorSet.CountObj(ho_ResultOOK, out hv_NumOK);
                HOperatorSet.CountObj(ho_ResultONG, out hv_NumNG);
                //HTuple end_val7 = hv_NumOK;
                //HTuple step_val7 = 1;
                //for (hv_j = 1; hv_j.Continue(end_val7, step_val7); hv_j = hv_j.TupleAdd(step_val7))
                //{
                //    ho_SelectedObjectOK.Dispose();
                //    HOperatorSet.SelectObj(ho_ResultOOK, out ho_SelectedObjectOK, hv_j);
                //    HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK,
                //        out hv_Radius);
                //    HOperatorSet.SmallestRectangle1(ho_SelectedObjectOK, out hv_Row1, out hv_Column1,
                //                                 out hv_Row2, out hv_Column2);
                //    hv_width = hv_Column2 - hv_Column1;
                //    hv_height = hv_Row2 - hv_Row1;
                //    //新增20180703
                //    RowOK = float.Parse(hv_RowOK.ToString());
                //    ColumnOK = float.Parse(hv_ColumnOK.ToString());
                //    g.DrawString("OK", frpnt, sbrush, ColumnOK - 50, RowOK + 200);
                //    try
                //    {
                //        Row1 = int.Parse(hv_Row1.ToString());
                //        Column1 = int.Parse(hv_Column1.ToString());
                //        width = int.Parse(hv_width.ToString());
                //        height = int.Parse(hv_height.ToString());
                //        Pen pen = new Pen(Color.Green, 6);
                //        g.DrawRectangle(pen, Column1, Row1, width, height);
                //    }
                //    catch (Exception ex)
                //    {
                //        Log("高清图像1" + ex.Message);
                //    }
                //}

                //HTuple end_val15 = hv_NumNG;
                //HTuple step_val15 = 1;
                //for (hv_k = 1; hv_k.Continue(end_val15, step_val15); hv_k = hv_k.TupleAdd(step_val15))
                //{
                //    ho_SelectedObjectNG.Dispose();
                //    HOperatorSet.SelectObj(ho_ResultONG, out ho_SelectedObjectNG, hv_k);
                //    HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG,
                //        out hv_ColumnNG, out hv_Radius);
                //    HOperatorSet.SmallestRectangle1(ho_SelectedObjectNG, out hv_Row1, out hv_Column1,
                //                                    out hv_Row2, out hv_Column2);
                //    hv_width = hv_Column2 - hv_Column1;
                //    hv_height = hv_Row2 - hv_Row1;
                //    //新增20180703
                //    RowNG = float.Parse(hv_RowNG.ToString());
                //    ColumnNG = float.Parse(hv_ColumnNG.ToString());
                //    SolidBrush brush = new SolidBrush(Color.Red);
                //    g.DrawString("NG", frpnt, brush, ColumnNG - 50, RowNG + 200);
                //    try
                //    {
                //        Row1 = int.Parse(hv_Row1.ToString());
                //        Column1 = int.Parse(hv_Column1.ToString());
                //        width = int.Parse(hv_width.ToString());
                //        height = int.Parse(hv_height.ToString());
                //        Pen penNG = new Pen(Color.Red, 6);
                //        g.DrawRectangle(penNG, Column1, Row1, width, height);
                //    }
                //    catch (Exception ex)
                //    {
                //        Log("高清图像2" + ex.Message);
                //    }


                //}
                if (hv_NumNG>0)
                {
                    string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\" + FindSN_Module + ".JPG";
                    img.Save(data1, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else
                {
                    string data1 = _Save_image_directory_ + "\\焊后正面识别\\OK\\" + FindSN_Module + ".JPG";
                    img.Save(data1, System.Drawing.Imaging.ImageFormat.Jpeg);
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


        //英利焊后正面识别算法 20180324
        public void Z2_str_Welding_recognition_z_YL(HObject ho_Image, HObject ho_Image_AffinTrans,
          out HObject ho_ResultOOK, out HObject ho_ResultONG, HTuple hv_row_2, HTuple hv_column_2,
          HTuple hv_WindowID, out HTuple hv_out_temp_error)
        {

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ConnectedRegions, ho_RegionsSelected;
            HObject ho_ImageAffin, ho_ImageScaled1, ho_ImageScaled2;
            HObject ho_ScaledWeld1, ho_ConnectedWeld1, ho_Rectangle = null, ho_SelectedRegions1;
            HObject ho_ScaledWeld2, ho_ConnectedWeld2, ho_SelectedRegions2;
            HObject ho_RegionUnion, ho_Rectangle1 = null;
            HObject ho_Rect = null, ho_ImageReduced1 = null, ho_ImageReduced2 = null;
            HObject ho_ImageReduced3 = null, ho_RegionReduced1 = null, ho_RegionReduced2 = null;
            HObject ho_RegionReduced3 = null, ho_RegionIntersection = null;
            HObject ho_RegionErosion = null, ho_SelectedRegions = null;

            // Local control variables 

            HTuple hv_Q_row = null, hv_Q_column = null;
            HTuple hv_Q_HJ_spacing = null, hv_Q_PY_temp_spacing = null;
            HTuple hv_Width = null, hv_Height = null, hv_Thresh1 = null;
            HTuple hv_Thresh2 = null, hv_Row11 = null, hv_Column11 = null;
            HTuple hv_Row21 = null, hv_Column21 = null, hv_I = null;
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple(), hv_Number = new HTuple();
            HTuple hv_UThresh1 = new HTuple(), hv_UThresh2 = new HTuple();
            HTuple hv_UThresh3 = new HTuple(), hv_Num1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ResultOOK);
            HOperatorSet.GenEmptyObj(out ho_ResultONG);
            //HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionsSelected);
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
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            //输出缺陷数量
            hv_out_temp_error = 0;
            //
            try
            {
                //起始位置R
                hv_Q_row = Z2_CenterRow;
                //起始位置W
                hv_Q_column = Z2_CenterCol;
                //每个焊接位置的间距
                hv_Q_HJ_spacing = Z2_WeldDis;
                //每次偏移位置累加
                hv_Q_PY_temp_spacing = hv_Q_column + 0;

                HOperatorSet.DispObj(ho_Image_AffinTrans, hv_WindowID);
                ho_ResultOOK.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultOOK);
                ho_ResultONG.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultONG);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                ho_RegionsSelected.Dispose();
                HOperatorSet.GenEmptyObj(out ho_RegionsSelected);


                HOperatorSet.GetImageSize(ho_Image_AffinTrans, out hv_Width, out hv_Height);
                //待定
                ho_ImageAffin.Dispose();
                HOperatorSet.Emphasize(ho_Image_AffinTrans, out ho_ImageAffin, hv_Width, hv_Height, 2);
                ho_ImageScaled1.Dispose();
                scale_image_range(ho_ImageAffin, out ho_ImageScaled1, 1, 75);
                ho_ImageScaled2.Dispose();
                scale_image_range(ho_ImageAffin, out ho_ImageScaled2, 90, 1);
                /////////////////////////////////////////////////////////////////////////////
                //提取四焊点区域
                ho_ScaledWeld1.Dispose();
                HOperatorSet.BinaryThreshold(ho_ImageScaled1, out ho_ScaledWeld1, "max_separability",
                    "light", out hv_Thresh1);
                ho_ConnectedWeld1.Dispose();
                HOperatorSet.Connection(ho_ScaledWeld1, out ho_ConnectedWeld1);
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedWeld1, out ho_SelectedRegions1, ((new HTuple("area")).TupleConcat(
                    "height")).TupleConcat("width"), "and", ((new HTuple(240000)).TupleConcat(
                    200)).TupleConcat(1100), ((new HTuple(350000)).TupleConcat(350)).TupleConcat(1800));

                ho_ScaledWeld2.Dispose();
                HOperatorSet.BinaryThreshold(ho_ImageScaled2, out ho_ScaledWeld2, "max_separability",
                    "dark", out hv_Thresh2);
                ho_ConnectedWeld2.Dispose();
                HOperatorSet.Connection(ho_ScaledWeld2, out ho_ConnectedWeld2);
                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedWeld2, out ho_SelectedRegions2, ((new HTuple("area")).TupleConcat(
                    "height")).TupleConcat("width"), "and", ((new HTuple(240000)).TupleConcat(
                    200)).TupleConcat(1100), ((new HTuple(350000)).TupleConcat(350)).TupleConcat(1800));

                //新增20180531
                try
                {
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union2(ho_SelectedRegions1, ho_SelectedRegions2, out ho_RegionUnion);
                    HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row11 + 50, hv_Column11, hv_Row11 + Z2_L_dyn_threshold_YL, hv_Column21);
                }
                catch (HalconException)
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_Rectangle);
                }

                if (star_Z2_L_rw_YL)
                {
                    HOperatorSet.DispObj(ho_Rectangle, Z2_Positive_HWindow.HalconWindow);
                }
                ///////////////////////////////////////////////////////////////////////////////////
                for (hv_I = 0; (int)hv_I <= 3; hv_I = (int)hv_I + 1)
                {
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                        0, Z2_WeldW, Z2_WeldH);
                    //显示调试区域
                    if (Z2_show_rectangle_z)
                    {
                        HOperatorSet.SetColor(hv_WindowID, "yellow");
                        HOperatorSet.DispObj(ho_Rectangle1, hv_WindowID);
                    }
                    //测试位置
                    if ((int)(new HTuple(hv_I.TupleEqual(1))) != 0)
                    {
                        hv_Q_PY_temp_spacing = (hv_Q_PY_temp_spacing + hv_Q_HJ_spacing) + Z2_WeldDisY;
                    }
                    else
                    {
                        hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;
                    }
                    HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                    hv_h_row = hv_Row2 - hv_Row1;
                    hv_cr_w = hv_Column2 - hv_Column1;
                    //新增20180531
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
                    HOperatorSet.ReduceDomain(ho_ImageAffin,   ho_Rect, out ho_ImageReduced3);
                    //*****************************************
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
                    //新增20180603
                    HOperatorSet.OpeningRectangle1(ho_RegionIntersection, out ho_RegionIntersection, 1, 8);
                    //////////////////////////////////////////////////////////////////
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FillUp(ho_RegionReduced3, out ExpTmpOutVar_0);
                        ho_RegionReduced3.Dispose();
                        ho_RegionReduced3 = ExpTmpOutVar_0;
                    }

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionReduced3, out ho_RegionErosion, 15, 15);
                    HOperatorSet.DilationRectangle1(ho_RegionErosion, out ho_RegionErosion, 15, 15);
            
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union2(ho_RegionErosion, ho_RegionIntersection, out ExpTmpOutVar_0);
                        ho_RegionIntersection.Dispose();
                        ho_RegionIntersection = ExpTmpOutVar_0;
                    }
                    //////////////////////////////////////////////////////////////////
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);
                    //提取目标特征
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area", 70);
                        
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, "area",
                            "and", nudHoleMaxArea, 999999);
                        ho_SelectedRegions.Dispose();
                        ho_SelectedRegions = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, "height",
                            "and", nudHoleMinHeight, nudHoleMaxHeight);
                        ho_SelectedRegions.Dispose();
                        ho_SelectedRegions = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, "width",
                            "and", nudHoleMinWidth, 99999);
                        ho_SelectedRegions.Dispose();
                        ho_SelectedRegions = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, "bulkiness",
                            "and", 0, nudBacknum);

                        ho_SelectedRegions.Dispose();
                        ho_SelectedRegions = ExpTmpOutVar_0;
                    }
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num1);
                    //注销20180709
                    //HOperatorSet.DispObj(ho_SelectedRegions, Z2_Positive_HWindow.HalconWindow);
                    if ((int)(new HTuple(hv_Num1.TupleGreater(0))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultONG, ho_Rectangle1, out ExpTmpOutVar_0);
                            ho_ResultONG.Dispose();
                            ho_ResultONG = ExpTmpOutVar_0;
                        }
                        hv_out_temp_error = hv_out_temp_error + 1;
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultOOK, ho_Rectangle1, out ExpTmpOutVar_0);
                            ho_ResultOOK.Dispose();
                            ho_ResultOOK = ExpTmpOutVar_0;
                        }
                    }
                    //*****************************************************************
                    //*****************************************************************
                    //stop ()
                }
                ho_ConnectedRegions.Dispose();
                ho_RegionsSelected.Dispose();
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
                //新增20180525 解决异常时 不判NG的情况
                hv_out_temp_error = hv_out_temp_error + 1;
            }
        }



        //Z：图像角度矫正
        public void Star_AffineTransImage(HObject ho_Image, out HObject ho_ImageGauss1_AffinTrans,
                                             HTuple hv_Height, HTuple hv_Width)
        {

            // Local iconic variables 
            HObject ho_ImageGauss1 = null, ho_ImageGauss2 = null;
            HObject ho_Region = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionFillUp = null, ho_RegionUnion = null, ho_Rectangle2 = null;
            HObject ho_RegionIntersection1 = null, ho_RegionErosion1 = null;
            HObject ho_RegionDilation = null, ho_RegionDifference = null;
            HObject ho_RegionErosion2 = null, ho_RegionDilation1 = null;
            HObject ho_RegionUnion1 = null, ho_RegionUnion2 = null;

            // Local control variables 

            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1_AffinTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss2);
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

            HTuple hv_HomMat2D = new HTuple();

            try
            {

                try
                {
                    //图像预处理
                    ho_ImageGauss1.Dispose();
                    HOperatorSet.GaussImage(ho_Image, out ho_ImageGauss1, 2.5);
                    //图像旋转有问题，图像拉伸暂时注销 20180509
                    ho_ImageGauss2.Dispose();
                    scale_image_range(ho_ImageGauss1, out ho_ImageGauss2, 50, 90);
                    //提取整个黑色接线盒
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageGauss2, out ho_Region, 0, 40);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 300000, 999999999);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionFillUp);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionFillUp, out ho_RegionUnion);
                    //提取黑色接线盒上半部分
                    HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_Rectangle2.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle2, 0, 0, hv_Row21 - (hv_Row21 * 0.5) + 80,
                        hv_Width);
                    ho_RegionIntersection1.Dispose();
                    HOperatorSet.Intersection(ho_Rectangle2, ho_RegionFillUp, out ho_RegionIntersection1);
                    ho_RegionErosion1.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionIntersection1, out ho_RegionErosion1,
                        251, 151);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion1, out ho_RegionDilation,
                        251, 151);
                    //提取黑色接线盒下半部分
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionFillUp, ho_Rectangle2, out ho_RegionDifference);
                    ho_RegionErosion2.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion2,
                        25, 25);
                    ho_RegionDilation1.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion2, out ho_RegionDilation1,
                        25, 25);
                    // 合并接线盒上下部分
                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union2(ho_RegionDilation, ho_RegionDilation1, out ho_RegionUnion1);
                    // 修改20180604
                    ho_RegionUnion2.Dispose();
                    //HOperatorSet.Union1(ho_RegionUnion1, out ho_RegionUnion2);
                    HOperatorSet.Union1(ho_RegionDilation1, out ho_RegionUnion2);

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
                    ho_ImageGauss1_AffinTrans.Dispose();
                    HOperatorSet.AffineTransImage(ho_ImageGauss1, out ho_ImageGauss1_AffinTrans,
                        hv_HomMat2D, "constant", "false");

                }
                catch (HalconException HDevExpDefaultException1)
                {
                    ho_ImageGauss1_AffinTrans.Dispose();
                    HOperatorSet.CopyObj(ho_ImageGauss1, out ho_ImageGauss1_AffinTrans, 1, 1);

                }

                ho_ImageGauss1.Dispose();
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
                ho_ImageGauss1_AffinTrans.Dispose();
                HOperatorSet.CopyObj(ho_Image, out ho_ImageGauss1_AffinTrans, 1, 1);

                ho_ImageGauss1.Dispose();
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

                //      throw HDevExpDefaultException;

            }
        }

        public void Star_AffineTransImage_YL(HObject ho_Image, out HObject ho_ImageGauss1_AffinTrans,
                                             HTuple hv_Height, HTuple hv_Width)
        {

            // Local iconic variables 
            HObject ho_ImageGauss1 = null, ho_ImageGauss2 = null;
            HObject ho_Region = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionFillUp = null, ho_RegionUnion = null, ho_Rectangle2 = null;
            HObject ho_RegionIntersection1 = null, ho_RegionErosion1 = null;
            HObject ho_RegionDilation = null, ho_RegionDifference = null;
            HObject ho_RegionErosion2 = null, ho_RegionDilation1 = null;
            HObject ho_RegionUnion1 = null, ho_RegionUnion2 = null;

            // Local control variables 

            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1_AffinTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss1);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss2);
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

            HTuple hv_HomMat2D = new HTuple();

            try
            {

                try
                {
                    //3X3模糊图像
                    //dev_display (Image)
                    ho_ImageGauss1.Dispose();
                    HOperatorSet.GaussImage(ho_Image, out ho_ImageGauss1, 2.5);
                    ho_ImageGauss2.Dispose();
                    scale_image_range(ho_ImageGauss1, out ho_ImageGauss2, 30, 100);

                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageGauss2, out ho_Region, 0, 200);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    //dev_display (ImageGauss1)
                    //area_center (ConnectedRegions, Area, Row2, Column2)
                    //tuple_max (Area, Max)
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 300000, 999999999);
                    //dev_display (ImageGauss1)
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionFillUp);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionFillUp, out ho_RegionUnion);

                    HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    //dev_display (ImageGauss1)
                    ho_Rectangle2.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle2, 0, 0, hv_Row21 - (hv_Row21 * 0.5),
                        hv_Width);
                    //dev_display (ImageGauss1)
                    ho_RegionIntersection1.Dispose();
                    HOperatorSet.Intersection(ho_Rectangle2, ho_RegionFillUp, out ho_RegionIntersection1
                        );

                    //dev_display (ImageGauss1)
                    //opening_rectangle1 (RegionIntersection1, RegionOpening, 300, 1)
                    ho_RegionErosion1.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionIntersection1, out ho_RegionErosion1,
                        251, 151);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion1, out ho_RegionDilation,
                        251, 151);

                    //dev_display (ImageGauss1)
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionFillUp, ho_Rectangle2, out ho_RegionDifference
                        );
                    //dev_display (ImageGauss1)
                    ho_RegionErosion2.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion2,
                        25, 25);
                    //dev_display (ImageGauss1)
                    ho_RegionDilation1.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionErosion2, out ho_RegionDilation1,
                        25, 25);
                    //dev_display (ImageGauss1)
                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union2(ho_RegionDilation, ho_RegionDilation1, out ho_RegionUnion1
                        );
                    ho_RegionUnion2.Dispose();
                    HOperatorSet.Union1(ho_RegionUnion1, out ho_RegionUnion2);

                    HOperatorSet.SmallestRectangle2(ho_RegionUnion2, out hv_Row3, out hv_Column3,
                        out hv_Phi1, out hv_Length11, out hv_Length21);


                    if ((int)((new HTuple(hv_Phi1.TupleLess(-0.5))).TupleOr(new HTuple(hv_Phi1.TupleGreater(
                        0.5)))) != 0)
                    {
                        //从点和角度计算一个僵化的仿射变换。
                        HOperatorSet.VectorAngleToRigid(hv_Row3, hv_Column3, hv_Phi1, hv_Row3,
                            hv_Column3, (new HTuple(180)).TupleRad(), out hv_HomMat2D);
                    }
                    else
                    {
                        //从点和角度计算一个僵化的仿射变换。
                        HOperatorSet.VectorAngleToRigid(hv_Row3, hv_Column3, hv_Phi1, hv_Row3,
                            hv_Column3, (new HTuple(0)).TupleRad(), out hv_HomMat2D);
                    }

                    //dev_display (ImageGauss1)
                    ho_ImageGauss1_AffinTrans.Dispose();
                    HOperatorSet.AffineTransImage(ho_ImageGauss1, out ho_ImageGauss1_AffinTrans,
                        hv_HomMat2D, "constant", "false");

                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    //     HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    ho_ImageGauss1_AffinTrans.Dispose();
                    HOperatorSet.CopyObj(ho_ImageGauss1, out ho_ImageGauss1_AffinTrans, 1, 1);

                }

                ho_ImageGauss1.Dispose();
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
                ho_ImageGauss1_AffinTrans.Dispose();
                HOperatorSet.CopyObj(ho_Image, out ho_ImageGauss1_AffinTrans, 1, 1);

                ho_ImageGauss1.Dispose();
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

                //      throw HDevExpDefaultException;

            }
        }

        //JinKO识别 新算法 20180420
        #region JinKO识别 新算法 20180420
        public void Weld_Edge_Length_JK(HObject ho_ImageReduced, HObject ho_Rectangle1,
                                         out HObject ho_ObjectEdge, out HTuple hv_EdgeNum1, out HTuple hv_EdgeNum2)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageMean1, ho_ImageMean2, ho_RegionDynThresh;
            HObject ho_RegionDilation, ho_RegionClosing, ho_RegionErosion;
            HObject ho_ConnectedRegions, ho_h_ER_sRegions, ho_h_sRegions;
            HObject ho_EdgeObject = null, ho_SelectedRegions = null, ho_UnionRegion = null;
            HObject ho_RegionClosing_er = null, ho_Rectangle3 = null;

            // Local control variables 

            HTuple hv_Z2_L_height_max = null, hv_Z2_L_height_min_ = null;
            HTuple hv_Z2_L_dyn_threshold_ = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_Row2 = null, hv_Column2 = null, hv_h_row = null;
            HTuple hv_cr_w = null, hv_EdgeNumT = null, hv_WMin_L = null;
            HTuple hv_WMax_L = null, hv_Row1_temp = new HTuple(), hv_Column1_temp = new HTuple();
            HTuple hv_Row2_temp = new HTuple(), hv_Column2_temp = new HTuple();
            HTuple hv_temp_L = new HTuple(), hv_t = new HTuple(), hv_TEMP_Lmax = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Row18 = new HTuple(), hv_Column18 = new HTuple();
            HTuple hv_Row28 = new HTuple(), hv_Column28 = new HTuple();
            HTuple hv_Row7 = new HTuple(), hv_Column7 = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Row8 = new HTuple();
            HTuple hv_Column8 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HTuple hv_WLenght = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ObjectEdge);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_ImageMean2);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_h_ER_sRegions);
            HOperatorSet.GenEmptyObj(out ho_h_sRegions);
            HOperatorSet.GenEmptyObj(out ho_EdgeObject);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_UnionRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing_er);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            hv_EdgeNum2 = new HTuple();
            try
            {
                //*****xxxxxxx******20180418参数设置
                //最大单个边界A01
                hv_Z2_L_height_max = Z2_L_height_max_;//80
                //出现2条边界的允许长度A02
                hv_Z2_L_height_min_ = Z2_L_height_min_;//60
                //汇流条边界识别阈值A03
                hv_Z2_L_dyn_threshold_ = Z2_L_dyn_threshold_;//12
                //*****xxxxxxx******20180418参数设置

                //计算焊点区域的高度和宽度
                HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                hv_h_row = hv_Row2 - hv_Row1;
                hv_cr_w = hv_Column2 - hv_Column1;
                //提取漏焊汇流条
                //scale_image_range (ImageReduced, ImageReduced, 100, 150)
                //提取漏焊接时汇流条左右边界
                ho_ImageMean1.Dispose();
                HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean1, 1, 30);
                ho_ImageMean2.Dispose();
                HOperatorSet.MeanImage(ho_ImageMean1, out ho_ImageMean2, 13, 1);
                ho_RegionDynThresh.Dispose();
                HOperatorSet.DynThreshold(ho_ImageMean1, ho_ImageMean2, out ho_RegionDynThresh,
                    hv_Z2_L_dyn_threshold_, "dark");
                ho_RegionDilation.Dispose();
                HOperatorSet.DilationRectangle1(ho_RegionDynThresh, out ho_RegionDilation,
                    1, 15);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionDilation, out ho_RegionClosing, 2.5);
                ho_RegionErosion.Dispose();
                HOperatorSet.ErosionRectangle1(ho_RegionClosing, out ho_RegionErosion, 1, 1);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionErosion, out ho_ConnectedRegions);
                //提取竖直方向最大高度的一条边
                ho_ObjectEdge.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ObjectEdge);
                ho_h_ER_sRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_h_ER_sRegions, "height",
                    "and", (hv_h_row / 100) * hv_Z2_L_height_max, 2 * hv_h_row);
                HOperatorSet.SelectShapeStd(ho_h_ER_sRegions, out ho_h_ER_sRegions, "max_area", 70);
                HOperatorSet.CountObj(ho_h_ER_sRegions, out hv_EdgeNum1);

                //不检测单边
                //hv_EdgeNum1 = 0;

                if ((int)(new HTuple(hv_EdgeNum1.TupleGreater(0))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ObjectEdge, ho_h_ER_sRegions, out ExpTmpOutVar_0
                            );
                        ho_ObjectEdge.Dispose();
                        ho_ObjectEdge = ExpTmpOutVar_0;
                    }
                    //HOperatorSet.DispObj(ho_ObjectEdge, Z2_Positive_HWindow.HalconWindow);
                }
                //提取竖直方向高度比较大的2条边
                ho_h_sRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_h_sRegions, "height",
                    "and", (hv_h_row / 100) * hv_Z2_L_height_min_, 2 * hv_h_row);
                //判断2条特征线的距离宽度
                HOperatorSet.CountObj(ho_h_sRegions, out hv_EdgeNumT);

                //汇流条宽度
                hv_WMin_L = hv_cr_w * 0.363;
                //判断2个区域的间距
                hv_WMax_L = hv_cr_w * 0.6;
                //缺陷区域
                if ((int)(new HTuple(hv_EdgeNumT.TupleGreater(1))) != 0)
                {
                    HOperatorSet.SmallestRectangle1(ho_h_sRegions, out hv_Row1_temp, out hv_Column1_temp,
                        out hv_Row2_temp, out hv_Column2_temp);
                    hv_temp_L = hv_Row2_temp - hv_Row1_temp;
                    //删选最长的2个
                    //够叼：提取高度最高的和次高的特征
                    ho_EdgeObject.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_EdgeObject);
                    for (hv_t = 1; (int)hv_t <= 2; hv_t = (int)hv_t + 1)
                    {
                        try
                        {
                            HOperatorSet.TupleMax(hv_temp_L, out hv_TEMP_Lmax);
                            //获取最大值的位置
                            HOperatorSet.TupleFind(hv_temp_L, hv_TEMP_Lmax, out hv_Indices);
                            //删除索引位置的值
                            HOperatorSet.TupleRemove(hv_temp_L, hv_Indices, out hv_temp_L);
                            ho_SelectedRegions.Dispose();
                            HOperatorSet.SelectShape(ho_h_sRegions, out ho_SelectedRegions, "height",
                                "and", hv_TEMP_Lmax, 999999);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_EdgeObject, ho_SelectedRegions, out ExpTmpOutVar_0
                                    );
                                ho_EdgeObject.Dispose();
                                ho_EdgeObject = ExpTmpOutVar_0;
                            }
                        }
                        // catch (Exception) 
                        catch (HalconException HDevExpDefaultException1)
                        {
                            HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            //两个一样长时可能报错，不用处理
                        }
                    }
                    ho_UnionRegion.Dispose();
                    HOperatorSet.Union1(ho_EdgeObject, out ho_UnionRegion);
                    ho_RegionClosing_er.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_UnionRegion, out ho_RegionClosing_er, hv_cr_w * 0.5,
                        1);
                    //获取中间部分区域计算
                    HOperatorSet.SmallestRectangle1(ho_RegionClosing_er, out hv_Row18, out hv_Column18,
                        out hv_Row28, out hv_Column28);
                    HOperatorSet.SmallestRectangle2(ho_RegionClosing_er, out hv_Row7, out hv_Column7,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle3.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row7, hv_Column7, (new HTuple(90)).TupleRad()
                        , 0.1, (hv_Column28 - hv_Column18) / 2);
                    HOperatorSet.SmallestRectangle2(ho_Rectangle3, out hv_Row8, out hv_Column8,
                        out hv_Phi1, out hv_Length11, out hv_Length21);
                    hv_WLenght = hv_Length11 * 2;
                    if ((int)((new HTuple(hv_WLenght.TupleGreater(hv_WMin_L))).TupleAnd(new HTuple(hv_WLenght.TupleLess(
                        hv_WMax_L)))) != 0)
                    {
                        hv_EdgeNum2 = 1;
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ObjectEdge, ho_UnionRegion, out ExpTmpOutVar_0
                                );
                            ho_ObjectEdge.Dispose();
                            ho_ObjectEdge = ExpTmpOutVar_0;
                        }
                        //HOperatorSet.DispObj(ho_ObjectEdge, Z2_Positive_HWindow.HalconWindow);
                    }
                    else
                    {
                        hv_EdgeNum2 = 0;
                    }
                }
                else
                {
                    hv_EdgeNum2 = 0;
                }
                HOperatorSet.DispObj(ho_ObjectEdge, Z2_Positive_HWindow.HalconWindow);
                //stop ()
                ho_ImageMean1.Dispose();
                ho_ImageMean2.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionErosion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_h_ER_sRegions.Dispose();
                ho_h_sRegions.Dispose();
                ho_EdgeObject.Dispose();
                ho_SelectedRegions.Dispose();
                ho_UnionRegion.Dispose();
                ho_RegionClosing_er.Dispose();
                ho_Rectangle3.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageMean1.Dispose();
                ho_ImageMean2.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionErosion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_h_ER_sRegions.Dispose();
                ho_h_sRegions.Dispose();
                ho_EdgeObject.Dispose();
                ho_SelectedRegions.Dispose();
                ho_UnionRegion.Dispose();
                ho_RegionClosing_er.Dispose();
                ho_Rectangle3.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Weld_Rosin_Joint_JK(HObject ho_ImageReduced, HObject ho_Rectangle1,
                                        out HObject ho_ObjectRJ, out HTuple hv_RJNum)
        {



            // Local iconic variables 

            HObject ho_ImageMean, ho_ImageInvert, ho_RegionDynThresh1;
            HObject ho_RegionClosing, ho_Rectangle, ho_RegionIntersection1;
            HObject ho_RegionClosing3_w, ho_RegionUnion3_w, ho_ConnectedRegions;
            HObject ho_dw_SRegions, ho_RectLeft = null, ho_RectRight = null;
            HObject ho_RegionUnion2 = null, ho_RegionDifference3 = null;
            HObject ho_RegionClosing3 = null, ho_RegionFillUp2 = null, ho_ConnectedRegions6 = null;
            HObject ho_lr_SRegions2 = null, ho_LR_SRegionsX = null;

            // Local control variables 

            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_h_row = null, hv_cr_w = null;
            HTuple hv_Mean = null, hv_Deviation = null, hv_m_thresh = null;
            HTuple hv_Area = null, hv_Row = null, hv_Column = null;
            HTuple hv_PArea = null, hv_RJNumber = null, hv_Z2_r1area = null;
            HTuple hv_Z2_r1_number = null, hv_Z2_r1_hrow = null, hv_Z2_r1_crw = null;
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_LR_erArea = new HTuple(), hv_lr_erNumber2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ObjectRJ);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageInvert);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing3_w);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3_w);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_dw_SRegions);
            HOperatorSet.GenEmptyObj(out ho_RectLeft);
            HOperatorSet.GenEmptyObj(out ho_RectRight);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference3);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing3);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions6);
            HOperatorSet.GenEmptyObj(out ho_lr_SRegions2);
            HOperatorSet.GenEmptyObj(out ho_LR_SRegionsX);
            hv_RJNum = new HTuple();
            try
            {
                //计算焊点的高度和宽度
                ho_ObjectRJ.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ObjectRJ);
                HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                hv_h_row = hv_Row2 - hv_Row1;
                hv_cr_w = hv_Column2 - hv_Column1;

                //BB 查找锡流失或虚焊
                ho_ImageMean.Dispose();
                HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 5, 5);
                ho_ImageInvert.Dispose();
                HOperatorSet.InvertImage(ho_ImageMean, out ho_ImageInvert);
                HOperatorSet.Intensity(ho_Rectangle1, ho_ImageInvert, out hv_Mean, out hv_Deviation);
                hv_m_thresh = hv_Mean - hv_Deviation;
                ho_RegionDynThresh1.Dispose();
                HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageInvert, out ho_RegionDynThresh1,
                    hv_m_thresh, "dark");

                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionDynThresh1, out ho_RegionClosing, 3.5);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1 - 7, hv_Column1, hv_Row2,
                    hv_Column2);
                ho_RegionIntersection1.Dispose();
                HOperatorSet.Intersection(ho_Rectangle, ho_RegionClosing, out ho_RegionIntersection1
                    );
                ho_RegionClosing3_w.Dispose();
                HOperatorSet.ClosingRectangle1(ho_RegionIntersection1, out ho_RegionClosing3_w,
                    hv_cr_w * 0.1, 1);
                ho_RegionUnion3_w.Dispose();
                HOperatorSet.Union2(ho_RegionClosing, ho_RegionClosing3_w, out ho_RegionUnion3_w
                    );
                //获取分析区域，面积10%
                HOperatorSet.AreaCenter(ho_Rectangle1, out hv_Area, out hv_Row, out hv_Column);
                hv_PArea = hv_Area * 0.06;
                //检查包裹度
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionUnion3_w, out ho_ConnectedRegions);
                ho_dw_SRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_dw_SRegions, (new HTuple("area")).TupleConcat(
                    "height"), "and", hv_PArea.TupleConcat(50), (new HTuple(999999999)).TupleConcat(
                    9999));
                HOperatorSet.CountObj(ho_dw_SRegions, out hv_RJNumber);

                //小矩形框面积的百分数
                hv_Z2_r1area = Z2_r1area_;//0.23
                //判断左右黑色区域数量
                //检测等级 （2级严 1级松）
                hv_Z2_r1_number = Z2_r1_number_;//2
                //左右矩形的高度
                hv_Z2_r1_hrow = Z2_r1_hrow_;//0.8
                //左右矩形的宽度
                hv_Z2_r1_crw = Z2_r1_crw_;//0.225


                //判断宽度包裹宽度
                if ((int)(new HTuple(hv_RJNumber.TupleGreater(0))) != 0)
                {
                    ho_RectLeft.Dispose();
                    HOperatorSet.GenRectangle1(out ho_RectLeft, hv_Row2 - (hv_h_row * (hv_Z2_r1_hrow / 100)),
                        hv_Column1, hv_Row2, hv_Column1 + ((hv_cr_w * hv_Z2_r1_crw) / 100));
                    ho_RectRight.Dispose();
                    HOperatorSet.GenRectangle1(out ho_RectRight, hv_Row2 - (hv_h_row * (hv_Z2_r1_hrow / 100)),
                        hv_Column2 - ((hv_cr_w * hv_Z2_r1_crw) / 100), hv_Row2, hv_Column2);

                    ho_RegionUnion2.Dispose();
                    HOperatorSet.Union2(ho_RectLeft, ho_RectRight, out ho_RegionUnion2);
                    if (show_Z2_r1_hrow_crw_)
                    {
                        HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_RegionUnion2, Z2_Positive_HWindow.HalconWindow);
                    }


                    //stop ()
                    //上区域
                    ho_RegionDifference3.Dispose();
                    HOperatorSet.Intersection(ho_RegionUnion2, ho_dw_SRegions, out ho_RegionDifference3
                        );
                    ho_RegionClosing3.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference3, out ho_RegionClosing3,
                        1, hv_h_row * 0.21);
                    ho_RegionFillUp2.Dispose();
                    HOperatorSet.FillUp(ho_RegionClosing3, out ho_RegionFillUp2);
                    ho_ConnectedRegions6.Dispose();
                    HOperatorSet.Connection(ho_RegionFillUp2, out ho_ConnectedRegions6);
                    //左右单个方块的面积
                    HOperatorSet.AreaCenter(ho_RectLeft, out hv_Area, out hv_Row3, out hv_Column3);
                    hv_LR_erArea = hv_Area * (hv_Z2_r1area / 100.00);
                    ho_lr_SRegions2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions6, out ho_lr_SRegions2, (new HTuple("area")).TupleConcat(
                        "height"), "and", hv_LR_erArea.TupleConcat(hv_h_row * 0.23), (new HTuple(999999999)).TupleConcat(
                        9999));


                    if (show_Z2_r1_hrow_crw_)
                    {
                        HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "blue");
                        HOperatorSet.DispObj(ho_lr_SRegions2, Z2_Positive_HWindow.HalconWindow);
                        HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                    }

                    HOperatorSet.CountObj(ho_lr_SRegions2, out hv_lr_erNumber2);
                    //判断左右黑色区域数量
                    if ((int)(new HTuple(hv_lr_erNumber2.TupleLess(hv_Z2_r1_number))) != 0)
                    {
                        hv_RJNum = 1;
                        ho_LR_SRegionsX.Dispose();
                        HOperatorSet.Union2(ho_Rectangle1, ho_lr_SRegions2, out ho_LR_SRegionsX
                            );
                        ho_ObjectRJ.Dispose();
                        HOperatorSet.ConcatObj(ho_RectLeft, ho_RectRight, out ho_ObjectRJ);
                    }
                    else
                    {
                        hv_RJNum = 0;
                    }
                }
               // 新增20180708
                else
                {
                    hv_RJNum = 0;
                }
                //stop ()
                ho_ImageMean.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_RegionClosing3_w.Dispose();
                ho_RegionUnion3_w.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_dw_SRegions.Dispose();
                ho_RectLeft.Dispose();
                ho_RectRight.Dispose();
                ho_RegionUnion2.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionClosing3.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_lr_SRegions2.Dispose();
                ho_LR_SRegionsX.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageMean.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_RegionClosing3_w.Dispose();
                ho_RegionUnion3_w.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_dw_SRegions.Dispose();
                ho_RectLeft.Dispose();
                ho_RectRight.Dispose();
                ho_RegionUnion2.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionClosing3.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_lr_SRegions2.Dispose();
                ho_LR_SRegionsX.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Weld_Loss_Area_JK(HObject ho_ImageReduced, HObject ho_Rectangle1,
                                       out HTuple hv_LSNum)
        {



            // Local iconic variables 

            HObject ho_ImageMean, ho_ImageInvert, ho_RegionDynThresh;
            HObject ho_RegionClosing, ho_Rectangle2, ho_RegionIntersection;
            HObject ho_ClosingRegion, ho_RegionUnion, ho_ConnectedRegions;
            HObject ho_SRegions, ho_Rectangle3, ho_IntersectionRegion;
            HObject ho_RegionsClosing, ho_RegionsUnion, ho_RegionFillUp;
            HObject ho_RegionsIntersection, ho_UnionRegions, ho_dh_Sregion;

            // Local control variables 

            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null;
            HTuple hv_Column2 = null, hv_h_row = null, hv_cr_w = null;
            HTuple hv_HHArea = null, hv_Mean = null, hv_Deviation = null;
            HTuple hv_m_threshold = null, hv_Area = null, hv_Row = null;
            HTuple hv_Column = null, hv_BGArea = null, hv_Z2_r3area = null;
            HTuple hv_Z2_r3_crw_min = null, hv_Z2_r3_crw_max = null;
            HTuple hv_Z2_r3_hrow = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageInvert);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_ClosingRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_IntersectionRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionsClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionsUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionsIntersection);
            HOperatorSet.GenEmptyObj(out ho_UnionRegions);
            HOperatorSet.GenEmptyObj(out ho_dh_Sregion);
            try
            {
                //本算子用来计算焊锡的包裹度
                HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                //焊点检测区域的高度和宽度
                hv_h_row = hv_Row2 - hv_Row1;
                hv_cr_w = hv_Column2 - hv_Column1;
                //焊点区域的面积
                hv_HHArea = hv_h_row * hv_cr_w;

                ho_ImageMean.Dispose();
                HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 5, 5);
                ho_ImageInvert.Dispose();
                HOperatorSet.InvertImage(ho_ImageMean, out ho_ImageInvert);
                //自动阈值分割
                HOperatorSet.Intensity(ho_Rectangle1, ho_ImageInvert, out hv_Mean, out hv_Deviation);
                hv_m_threshold = hv_Mean - hv_Deviation;
                ho_RegionDynThresh.Dispose();
                HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageInvert, out ho_RegionDynThresh,
                    hv_m_threshold, "dark");
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionDynThresh, out ho_RegionClosing, 3.5);

                //此处矩形暂时不明是做什么的
                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row1 - 7, hv_Column1, hv_Row2,
                    hv_Column2);
                ho_RegionIntersection.Dispose();
                HOperatorSet.Intersection(ho_Rectangle2, ho_RegionClosing, out ho_RegionIntersection
                    );
                ho_ClosingRegion.Dispose();
                HOperatorSet.ClosingRectangle1(ho_RegionIntersection, out ho_ClosingRegion,
                    hv_cr_w * 0.1, 1);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union2(ho_RegionClosing, ho_ClosingRegion, out ho_RegionUnion);

                //获取分析区域，面积6%
                HOperatorSet.AreaCenter(ho_Rectangle1, out hv_Area, out hv_Row, out hv_Column);
                hv_BGArea = hv_Area * 0.06;
                //检查包裹度
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionUnion, out ho_ConnectedRegions);
                ho_SRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SRegions, (new HTuple("area")).TupleConcat(
                    "height"), "and", hv_BGArea.TupleConcat(50), (new HTuple(999999999)).TupleConcat(
                    9999));

                //stop ()
                ho_Rectangle3.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle3, hv_Row1, hv_Column1, hv_Row1 + 31,
                    hv_Column2);
                //判断焊锡包裹度
                //拟合上面的区域
                ho_IntersectionRegion.Dispose();
                HOperatorSet.Intersection(ho_Rectangle3, ho_RegionClosing, out ho_IntersectionRegion
                    );

                ho_RegionsClosing.Dispose();
                HOperatorSet.ClosingRectangle1(ho_IntersectionRegion, out ho_RegionsClosing,
                    hv_cr_w * 0.5, 1);
                ho_RegionsUnion.Dispose();
                HOperatorSet.Union2(ho_RegionsClosing, ho_SRegions, out ho_RegionsUnion);

                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionsUnion, out ho_RegionFillUp);
                ho_RegionsIntersection.Dispose();
                HOperatorSet.Difference(ho_RegionFillUp, ho_RegionsUnion, out ho_RegionsIntersection
                    );
                ho_UnionRegions.Dispose();
                HOperatorSet.Union1(ho_RegionsIntersection, out ho_UnionRegions);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_UnionRegions, out ho_ConnectedRegions);
                //*****xxxxxxx******20180327上
                //E
                //焊锡包裹面积
                hv_Z2_r3area = Z2_r3area_ / 100;//0.19
                //焊锡包裹宽度系数
                hv_Z2_r3_crw_min = Z2_r3_crw_min_ / 100;//0.1
                hv_Z2_r3_crw_max = Z2_r3_crw_max_ / 100;//0.5
                //焊锡包裹高度系数
                hv_Z2_r3_hrow = Z2_r3_hrow_ / 100;//0.56
                //*****xxxxxxx******20180327下
                ho_dh_Sregion.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_dh_Sregion, ((new HTuple("area")).TupleConcat(
                    "width")).TupleConcat("height"), "and", (((((hv_h_row * hv_cr_w) * hv_Z2_r3area)).TupleConcat(
                    hv_cr_w * hv_Z2_r3_crw_min))).TupleConcat(hv_h_row * hv_Z2_r3_hrow), ((((hv_h_row * hv_cr_w)).TupleConcat(
                    hv_cr_w * hv_Z2_r3_crw_max))).TupleConcat(hv_h_row + 1));

                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "blue");
                HOperatorSet.DispObj(ho_dh_Sregion, Z2_Positive_HWindow.HalconWindow);
                HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");



                HOperatorSet.CountObj(ho_dh_Sregion, out hv_LSNum);
                ho_ImageMean.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionIntersection.Dispose();
                ho_ClosingRegion.Dispose();
                ho_RegionUnion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SRegions.Dispose();
                ho_Rectangle3.Dispose();
                ho_IntersectionRegion.Dispose();
                ho_RegionsClosing.Dispose();
                ho_RegionsUnion.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionsIntersection.Dispose();
                ho_UnionRegions.Dispose();
                ho_dh_Sregion.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageMean.Dispose();
                ho_ImageInvert.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionClosing.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionIntersection.Dispose();
                ho_ClosingRegion.Dispose();
                ho_RegionUnion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SRegions.Dispose();
                ho_Rectangle3.Dispose();
                ho_IntersectionRegion.Dispose();
                ho_RegionsClosing.Dispose();
                ho_RegionsUnion.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionsIntersection.Dispose();
                ho_UnionRegions.Dispose();
                ho_dh_Sregion.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Pure_Black_Welding_JK(HObject ho_ImageReduced1, HObject ho_Rectangle1,
                                       out HTuple hv_PBNum)
        {



            // Local iconic variables 

            HObject ho_RegionBack, ho_ConnectedRegions;
            HObject ho_SelectedRegions, ho_RegionFillUp, ho_RegionTrans;
            HObject ho_ImageReduced, ho_RegionWhite, ho_RegionsConnected;
            HObject ho_RegionsSelected;

            // Local control variables 

            HTuple hv_Threshold = null, hv_AreaBack = null;
            HTuple hv_Row = null, hv_Column = null, hv_AreaWhite = null;
            HTuple hv_PreArea = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionBack);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_RegionWhite);
            HOperatorSet.GenEmptyObj(out ho_RegionsConnected);
            HOperatorSet.GenEmptyObj(out ho_RegionsSelected);
            hv_PBNum = new HTuple();
            try
            {
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ImageReduced1, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Rectangle1, HDevWindowStack.GetActive());
                }
                //提取最大黑色面积
                ho_RegionBack.Dispose();
                HOperatorSet.BinaryThreshold(ho_ImageReduced1, out ho_RegionBack, "max_separability",
                    "dark", out hv_Threshold);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionBack, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionFillUp);
                //封闭最大黑色区域
                ho_RegionTrans.Dispose();
                HOperatorSet.ShapeTrans(ho_RegionFillUp, out ho_RegionTrans, "convex");
                //提取黑色面积中白色区域
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageReduced1, ho_RegionTrans, out ho_ImageReduced
                    );
                ho_RegionWhite.Dispose();
                HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_RegionWhite, "max_separability",
                    "light", out hv_Threshold);
                ho_RegionsConnected.Dispose();
                HOperatorSet.Connection(ho_RegionWhite, out ho_RegionsConnected);
                ho_RegionsSelected.Dispose();
                HOperatorSet.SelectShape(ho_RegionsConnected, out ho_RegionsSelected, "area",
                    "and", 590, 99999999);
                //黑白区域作比较
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_AreaBack, out hv_Row, out hv_Column);
                HOperatorSet.AreaCenter(ho_RegionsSelected, out hv_AreaWhite, out hv_Row, out hv_Column);
                hv_PreArea = (hv_AreaWhite + 0.0001) / hv_AreaBack;
                if ((int)(new HTuple(((hv_PreArea * 100)).TupleLess(10))) != 0)
                {
                    //有存黑色缺陷
                    //PBNum := 1
                    hv_PBNum = 0;
                }
                else
                {
                    //无缺陷
                    hv_PBNum = 0;
                }



                //stop ()
                ho_RegionBack.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_RegionWhite.Dispose();
                ho_RegionsConnected.Dispose();
                ho_RegionsSelected.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionBack.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_RegionWhite.Dispose();
                ho_RegionsConnected.Dispose();
                ho_RegionsSelected.Dispose();

                throw HDevExpDefaultException;
            }
        }

        //新增20180707
        public void Weld_Pretreatment(HObject ho_ImageScaled2, out HObject ho_UnionRegion,
                                      out HTuple hv_Number)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionScaled2, ho_FillUp2, ho_RegionDifference;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionUnion;
            HObject ho_RegionTrans, ho_Rectangle, ho_RegionIntersection;
            HObject ho_Rectangle2, ho_DifferenceRegion1, ho_ConnectedRegions1;
            HObject ho_SelectedRegions1, ho_RectObject, ho_ObjectSelected = null;
            HObject ho_Rectangle3 = null, ho_RegionsConnected;

            // Local control variables 

            HTuple hv_UsedThresh = null, hv_Row11 = null;
            HTuple hv_Column11 = null, hv_Row21 = null, hv_Column21 = null;
            HTuple hv_Row = null, hv_Column = null, hv_Radius = null;
            HTuple hv_Row12 = null, hv_Column12 = null, hv_Row22 = null;
            HTuple hv_Column22 = null, hv_Num = null, hv_j = null;
            HTuple hv_Row13 = new HTuple(), hv_Column13 = new HTuple();
            HTuple hv_Row23 = new HTuple(), hv_Column23 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_UnionRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionScaled2);
            HOperatorSet.GenEmptyObj(out ho_FillUp2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_DifferenceRegion1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RectObject);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_RegionsConnected);
            try
            {
                ho_RegionScaled2.Dispose();
                HOperatorSet.BinaryThreshold(ho_ImageScaled2, out ho_RegionScaled2, "max_separability",
                    "dark", out hv_UsedThresh);
                ho_FillUp2.Dispose();
                HOperatorSet.FillUp(ho_RegionScaled2, out ho_FillUp2);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_FillUp2, ho_RegionScaled2, out ho_RegionDifference
                    );
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", 88000, 9999999999999);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                ho_RegionTrans.Dispose();
                HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "rectangle1");
                HOperatorSet.SmallestRectangle1(ho_RegionTrans, out hv_Row11, out hv_Column11,
                    out hv_Row21, out hv_Column21);
                HOperatorSet.SmallestCircle(ho_RegionTrans, out hv_Row, out hv_Column, out hv_Radius);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, 0, (hv_Column21 - hv_Column11) / 2,
                    (hv_Row21 - hv_Row11) / 8);
                ho_RegionIntersection.Dispose();
                HOperatorSet.Intersection(ho_Rectangle, ho_RegionUnion, out ho_RegionIntersection
                    );
                HOperatorSet.SmallestRectangle1(ho_RegionIntersection, out hv_Row12, out hv_Column12,
                    out hv_Row22, out hv_Column22);

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row21 - 200, hv_Column12 + 0,
                    hv_Row21, hv_Column22 - 0);
                ho_DifferenceRegion1.Dispose();
                HOperatorSet.Difference(ho_Rectangle2, ho_RegionScaled2, out ho_DifferenceRegion1
                    );
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_DifferenceRegion1, out ho_ConnectedRegions1);
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "width",
                    "and", 185, 250);
                HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Num);
                ho_RectObject.Dispose();
                HOperatorSet.GenEmptyObj(out ho_RectObject);
                HTuple end_val19 = hv_Num;
                HTuple step_val19 = 1;
                for (hv_j = 1; hv_j.Continue(end_val19, step_val19); hv_j = hv_j.TupleAdd(step_val19))
                {
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_ObjectSelected, hv_j);
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_Row13, out hv_Column13,
                        out hv_Row23, out hv_Column23);
                    ho_Rectangle3.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle3, hv_Row23 - 300, hv_Column13 + 10,
                        hv_Row23, hv_Column23 - 5);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Rectangle3, ho_RectObject, out ExpTmpOutVar_0);
                        ho_RectObject.Dispose();
                        ho_RectObject = ExpTmpOutVar_0;
                    }
                }
                ho_UnionRegion.Dispose();
                HOperatorSet.Union1(ho_RectObject, out ho_UnionRegion);
                ho_RegionsConnected.Dispose();
                HOperatorSet.Connection(ho_UnionRegion, out ho_RegionsConnected);
                HOperatorSet.CountObj(ho_RegionsConnected, out hv_Number);
                ho_RegionScaled2.Dispose();
                ho_FillUp2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionIntersection.Dispose();
                ho_Rectangle2.Dispose();
                ho_DifferenceRegion1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RectObject.Dispose();
                ho_ObjectSelected.Dispose();
                ho_Rectangle3.Dispose();
                ho_RegionsConnected.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionScaled2.Dispose();
                ho_FillUp2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionIntersection.Dispose();
                ho_Rectangle2.Dispose();
                ho_DifferenceRegion1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RectObject.Dispose();
                ho_ObjectSelected.Dispose();
                ho_Rectangle3.Dispose();
                ho_RegionsConnected.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void str_Welding_recognition_JK(HObject ho_Image, HObject ho_Image_AffinTrans,
                                             out HObject ho_ResultOOK, out HObject ho_ResultONG, HTuple hv_row_2, HTuple hv_column_2,
                                              HTuple hv_WindowID, out HTuple hv_out_temp_error)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageScaled1, ho_ImageScaled2, ho_Rectangle1 = null, ho_UnionRegion;
            HObject ho_ImageReduced = null, ho_ImageReduced1 = null, ho_ObjectEdge = null;
            HObject ho_ObjectRJ = null;

            // Local control variables 

            HTuple hv_Q_row = null, hv_Q_column = null;
            HTuple hv_Q_HJ_spacing = null, hv_Q_PY_temp_spacing = null;
            HTuple hv_I = null, hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple();
            HTuple hv_EdgeNum1 = new HTuple(), hv_EdgeNum2 = new HTuple();
            HTuple hv_RJNum = new HTuple(), hv_LSNum = new HTuple();
            HTuple hv_PBNum = new HTuple();
            HTuple hv_Number = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ResultOOK);
            HOperatorSet.GenEmptyObj(out ho_ResultONG);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ObjectEdge);
            HOperatorSet.GenEmptyObj(out ho_ObjectRJ);
            HOperatorSet.GenEmptyObj(out ho_UnionRegion);
            try
            {
                //输出缺陷
                hv_out_temp_error = 0;
                //起始位置Y
                hv_Q_row = Z2_CenterRow;
                //起始位置X
                hv_Q_column = Z2_CenterCol;
                //每个焊接位置的间距
                hv_Q_HJ_spacing = Z2_WeldDis;
                //每次偏移位置累加
                hv_Q_PY_temp_spacing = hv_Q_column + 0;
                ho_ResultOOK.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultOOK);
                ho_ResultONG.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ResultONG);

                ho_ImageScaled1.Dispose();
                scale_image_range(ho_Image_AffinTrans, out ho_ImageScaled1, 100, 150);
                ho_ImageScaled2.Dispose();
                scale_image_range(ho_Image_AffinTrans, out ho_ImageScaled2, 90, 91);
                //新增20180707
                //ho_UnionRegion.Dispose();
                //Weld_Pretreatment(ho_ImageScaled2, out ho_UnionRegion, out hv_Number);

                for (hv_I = 0; (int)hv_I <= 3; hv_I = (int)hv_I + 1)
                {
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                        0, Z2_WeldW, Z2_WeldH);
                    //修改20180707
                    //if ((int)(new HTuple(hv_Number.TupleEqual(4))) != 0)
                    //{
                    //    ho_Rectangle1.Dispose();
                    //    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                    //        0, Z2_WeldW, Z2_WeldH);
                    //        HOperatorSet.Intersection(ho_Rectangle1, ho_UnionRegion, out ho_Rectangle1);
                    //}
                    //else
                    //{
                    //    ho_Rectangle1.Dispose();
                    //    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                    //        0, Z2_WeldW, Z2_WeldH);
                    //}

                    //测试位置
                    hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle1, out ho_ImageReduced
                        );
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageScaled2, ho_Rectangle1, out ho_ImageReduced1
                        );
                    //stop ()
                    HOperatorSet.GetImageSize(ho_Image_AffinTrans, out hv_Width, out hv_Height);
                    HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                        out hv_Row2, out hv_Column2);
                    hv_h_row = hv_Row2 - hv_Row1;
                    hv_cr_w = hv_Column2 - hv_Column1;
                    //*****************************************************************
                    //*****************************************************************
                    //在此处写入主程序
                    //AA 查找漏焊缺陷
                    ho_ObjectEdge.Dispose();
                    Weld_Edge_Length_JK(ho_ImageReduced, ho_Rectangle1, out ho_ObjectEdge, out hv_EdgeNum1,
                        out hv_EdgeNum2);

                    //不检测单边最大高度
                    if (star_Z2_L_ == 0)
                    {
                        hv_EdgeNum1 = 0;
                    }
                    //不检测左右双边高度
                    if (star_Z2_L_rw_ == 0)
                    {
                        hv_EdgeNum2 = 0;
                    }

                    if ((int)((new HTuple(hv_EdgeNum1.TupleLessEqual(0))).TupleOr(new HTuple(hv_EdgeNum2.TupleLessEqual(
                        0)))) != 0)
                    {
                        //BB 查找锡流失或虚焊
                        ho_ObjectRJ.Dispose();
                        Weld_Rosin_Joint_JK(ho_ImageReduced, ho_Rectangle1, out ho_ObjectRJ, out hv_RJNum);
                    }
                    else
                    {
                        hv_RJNum = 0;
                    }

                    //不检测锡流失或虚焊
                    if (star_Z2_r1area_ == 0)
                    {
                        hv_RJNum = 0;
                    }

                    if ((int)(new HTuple(hv_RJNum.TupleLessEqual(0))) != 0)
                    {
                        //CC 检测焊锡的包裹度(老算法第五种缺陷)
                        Weld_Loss_Area_JK(ho_ImageReduced, ho_Rectangle1, out hv_LSNum);
                    }
                    else
                    {
                        hv_LSNum = 0;
                    }

                    //焊锡的包裹度不检测
                    if (star_Z2_r3area_ == 0)
                    {
                        hv_LSNum = 0;
                    }



                    if ((int)(new HTuple(hv_LSNum.TupleLessEqual(0))) != 0)
                    {
                        //DD 查找存黑色虚焊
                        Pure_Black_Welding_JK(ho_ImageReduced1, ho_Rectangle1, out hv_PBNum);
                    }
                    else
                    {
                        hv_PBNum = 0;
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        //dev_display (Image_AffinTrans)
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        //dev_display (ObjectEdge)
                    }
                    //stop ()
                    //*****************************************************************
                    //*****************************************************************
                    //结果判断
                    if ((int)((new HTuple((new HTuple((new HTuple((new HTuple(hv_EdgeNum1.TupleGreater(
                        0))).TupleOr(new HTuple(hv_EdgeNum2.TupleGreater(0))))).TupleOr(new HTuple(hv_RJNum.TupleGreater(
                        0))))).TupleOr(new HTuple(hv_LSNum.TupleGreater(0))))).TupleOr(new HTuple(hv_PBNum.TupleGreater(
                        0)))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultONG, ho_Rectangle1, out ExpTmpOutVar_0);
                            ho_ResultONG.Dispose();
                            ho_ResultONG = ExpTmpOutVar_0;
                        }
                        hv_out_temp_error = hv_out_temp_error + 1;
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultOOK, ho_Rectangle1, out ExpTmpOutVar_0);
                            ho_ResultOOK.Dispose();
                            ho_ResultOOK = ExpTmpOutVar_0;
                        }
                    }
                    //*****************************************************************
                    //*****************************************************************
                    //stop ()
                }
                ho_ImageScaled1.Dispose();
                ho_ImageScaled2.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ObjectEdge.Dispose();
                ho_ObjectRJ.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageScaled1.Dispose();
                ho_ImageScaled2.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ObjectEdge.Dispose();
                ho_ObjectRJ.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Show_Weld_Results(HObject ho_Image_AffinTrans, HObject ho_ResultONG,
                                   HObject ho_ResultOOK, HTuple hv_WindowID, out HTuple hv_NumNG)
        {




            // Local iconic variables 

            HObject ho_SelectedObjectOK = null, ho_SelectedObjectNG = null;

            // Local control variables 

            HTuple hv_NumOK = null, hv_j = null, hv_RowOK = new HTuple();
            HTuple hv_ColumnOK = new HTuple(), hv_Radius = new HTuple();
            HTuple hv_k = null, hv_RowNG = new HTuple(), hv_ColumnNG = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedObjectOK);
            HOperatorSet.GenEmptyObj(out ho_SelectedObjectNG);
            try
            {
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image_AffinTrans, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), "red");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ResultONG, HDevWindowStack.GetActive());
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_ResultOOK, HDevWindowStack.GetActive());
                }
                HOperatorSet.CountObj(ho_ResultOOK, out hv_NumOK);
                HOperatorSet.CountObj(ho_ResultONG, out hv_NumNG);
                HTuple end_val7 = hv_NumOK;
                HTuple step_val7 = 1;
                for (hv_j = 1; hv_j.Continue(end_val7, step_val7); hv_j = hv_j.TupleAdd(step_val7))
                {
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), "green");
                    }
                    ho_SelectedObjectOK.Dispose();
                    HOperatorSet.SelectObj(ho_ResultOOK, out ho_SelectedObjectOK, hv_j);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK,
                        out hv_Radius);
                    HOperatorSet.SetTposition(3600, hv_RowOK + 200, hv_ColumnOK - 50);
                    HOperatorSet.WriteString(3600, "OK");
                }
                //
                HTuple end_val15 = hv_NumNG;
                HTuple step_val15 = 1;
                for (hv_k = 1; hv_k.Continue(end_val15, step_val15); hv_k = hv_k.TupleAdd(step_val15))
                {
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetColor(HDevWindowStack.GetActive(), "red");
                    }
                    ho_SelectedObjectNG.Dispose();
                    HOperatorSet.SelectObj(ho_ResultONG, out ho_SelectedObjectNG, hv_k);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG, out hv_ColumnNG,
                        out hv_Radius);
                    HOperatorSet.SetTposition(3600, hv_RowNG + 200, hv_ColumnNG - 50);
                    HOperatorSet.WriteString(3600, "NG");
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
        #endregion

        public HObject ho_ResultOOK = null, ho_ResultONG = null;
        public bool Z2_star_PR_z(HObject in_image,out HObject ho_ResultOOK,out HObject ho_ResultONG)
        {
            bool _ERROR = false;//初始化OK
            HObject Itempimage = null;
            HOperatorSet.GenEmptyObj(out Itempimage);
            Itempimage.Dispose();
            HTuple _W, _H;

            //
            HObject ho_Image_AffinTrans = null;
            HObject ho_ImageZoomed1 = null;
            HObject ho_out_EO_error_region = null;
            HOperatorSet.GenEmptyObj(out ho_Image_AffinTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed1);
            HOperatorSet.GenEmptyObj(out ho_out_EO_error_region);
            HOperatorSet.GenEmptyObj(out ho_ResultOOK);
            HOperatorSet.GenEmptyObj(out ho_ResultONG);
            //20180205 新增
            //HObject ho_ResultOOK = null, ho_ResultONG = null;
            HTuple _Width, _Height;
            HTuple hv_Width, hv_Height, hv_pi, hv_Row, hv_Column, hv_Angle, hv_Error;
            HTuple hv_row_2;
            HTuple hv_column_2;
            HTuple hv_HomMat2D, hv_out_temp_error = 0;//赋值为0
            HTuple hv_NumNG = new HTuple();
            try
            {

                HOperatorSet.GetImageSize(in_image, out hv_Width, out hv_Height);

                ho_Image_AffinTrans.Dispose();

                if (Z2_CcbRecognize == 2)//英利
                {
                    Star_AffineTransImage_YL(in_image, out ho_Image_AffinTrans, hv_Height, hv_Width);
                }
                else
                {
                    Star_AffineTransImage(in_image, out ho_Image_AffinTrans, hv_Height, hv_Width);
                }


                ho_ImageZoomed1.Dispose();
                HOperatorSet.ZoomImageFactor(ho_Image_AffinTrans, out ho_ImageZoomed1, 0.5,
                    0.5, "constant");

                //清除窗口20180415
                HOperatorSet.ClearWindow(zm2_HWindow.HalconWindow);
                HOperatorSet.ClearWindow(Z2_Positive_HWindow.HalconWindow);
                HOperatorSet.SetLineWidth(Z2_Positive_HWindow.HalconWindow, 2);
                HOperatorSet.SetLineWidth(zm2_HWindow.HalconWindow, 2);


                //匹配位置
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.BestMatchRotMg(ho_ImageZoomed1, hv_Z2_TemplateID, -hv_pi, 2 * hv_pi,
                    40, "true", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                //新增匹配参数打印20180415
                write(hv_Error.ToString());
                if ((int)(new HTuple(hv_Error.TupleLess(30))) != 0)
                {
                    hv_row_2 = hv_Row * 2;
                    hv_column_2 = hv_Column * 2;
                    //
                    HOperatorSet.SetColor(Z2_Positive_HWindow.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_Image_AffinTrans, Z2_Positive_HWindow.HalconWindow);
                    if (Z2_CcbRecognize == 0)//晶科
                    {
                        ho_out_EO_error_region.Dispose();
                        Z2_str_Welding_recognition_z(in_image, ho_Image_AffinTrans, out ho_out_EO_error_region,
                            hv_row_2, hv_column_2, Z2_Positive_HWindow.HalconWindow, out hv_out_temp_error);
                    }
                    else if (Z2_CcbRecognize == 1)//协鑫
                    {
                        Z2_str_Welding_recognition_z_Xx(in_image, ho_Image_AffinTrans, out ho_ResultOOK,
                            out ho_ResultONG, hv_row_2, hv_column_2, Z2_Positive_HWindow.HalconWindow, out hv_out_temp_error);
                        Show_Weld_Results(ho_Image_AffinTrans, ho_ResultONG, ho_ResultOOK, Z2_Positive_HWindow.HalconWindow);
                    }
                    else if (Z2_CcbRecognize == 2)//英利
                    {
                        Z2_str_Welding_recognition_z_YL(in_image, ho_Image_AffinTrans, out ho_ResultOOK,
                            out ho_ResultONG, hv_row_2, hv_column_2, Z2_Positive_HWindow.HalconWindow, out hv_out_temp_error);
                        //结果显示
                        Show_Weld_Results(ho_Image_AffinTrans, ho_ResultONG, ho_ResultOOK, Z2_Positive_HWindow.HalconWindow);
                    }
                    else if (Z2_CcbRecognize == 3)//JinKO识别 金帅
                    {

                        str_Welding_recognition_JK(in_image, ho_Image_AffinTrans,
                                             out ho_ResultOOK, out ho_ResultONG, hv_row_2, hv_column_2,
                                              Z2_Positive_HWindow.HalconWindow, out  hv_out_temp_error);
                        //结果显示
                        Show_Weld_Results(ho_Image_AffinTrans, ho_ResultONG, ho_ResultOOK, Z2_Positive_HWindow.HalconWindow);
                    }

                    HOperatorSet.DispArrow(Z2_Positive_HWindow.HalconWindow,
                    hv_Row * 2, hv_Column * 2, (hv_Row * 2) - ((hv_Angle.TupleCos()
                    ) * 50), (hv_Column * 2) - ((hv_Angle.TupleSin()) * 50), 15);

                    string TimeNow = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒");
                    HOperatorSet.GetImageSize(ho_Image_AffinTrans, out _Width, out _Height);
                    disp_message(Z2_Positive_HWindow.HalconWindow, FindSN_Module, "image", _Height - 200, 100, "green", "false");
                    disp_message(Z2_Positive_HWindow.HalconWindow, TimeNow, "image", _Height - 100, 100, "green", "false");

                    HOperatorSet.DumpWindowImage(out Itempimage, Z2_Positive_HWindow.HalconWindow);
                    HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                    //以适应窗口的方式显示图像
                    HOperatorSet.SetPart(zm2_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                    //显示图像
                    HOperatorSet.DispObj(Itempimage, zm2_HWindow.HalconWindow);
                    Itempimage.Dispose();
                    if (hv_out_temp_error > 0)
                    {
                        try
                        {
                            //保存图像
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                + DateTime.Now.Minute.ToString()
                                + DateTime.Now.Second.ToString();
                   
                            //if (_CBZM2_saver_image == 0)
                            //{
                               string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\P-" + FindSN_Module;
                               HOperatorSet.WriteImage(Z2_Positive_Image, "jpg", 0, data1);
                            //}   
                            string data2 = _Save_image_directory_ + "\\焊后正面识别\\NG\\" + FindSN_Module;
                            HOperatorSet.DumpWindow(zm2_HWindow.HalconWindow, "png", data2);
                         
                        }
                        catch (System.Exception ex2)
                        {
                            //新增20180415
                            write("图像保存失败1");
                            HOperatorSet.DispObj(ho_Image_AffinTrans, zm2_HWindow.HalconWindow);
                            HOperatorSet.DispObj(ho_Image_AffinTrans, Z2_Positive_HWindow.HalconWindow);

                            disp_message(Z2_Positive_HWindow.HalconWindow, "保存图像失败！", "image",
                                100, 100, "red", "false");
                            disp_message(zm2_HWindow.HalconWindow, "保存图像失败！", "image",
                                100, 100, "red", "false");
                        }
                        _ERROR = true;//NG

                    }
                    else
                    {
                        try
                        {
                            //保存图像
                            if (_ZM2_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                    + DateTime.Now.Minute.ToString()
                                    + DateTime.Now.Second.ToString();
                                
                                //if (_CBZM2_saver_image == 0)
                                //{
                                    string data1 = _Save_image_directory_ + "\\焊后正面识别\\OK\\P-" + FindSN_Module;
                                    HOperatorSet.WriteImage(Z2_Positive_Image, "jpg", 0, data1);
                                //}
                                string data2 = _Save_image_directory_ + "\\焊后正面识别\\OK\\" + FindSN_Module;
                                HOperatorSet.DumpWindow(zm2_HWindow.HalconWindow, "png", data2);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            //新增20180415
                            write("图像保存失败2");
                            HOperatorSet.DispObj(ho_Image_AffinTrans, zm2_HWindow.HalconWindow);
                            HOperatorSet.DispObj(ho_Image_AffinTrans, Z2_Positive_HWindow.HalconWindow);
                            disp_message(Z2_Positive_HWindow.HalconWindow, "保存图像失败！", "image",
                                100, 100, "red", "false");
                            disp_message(zm2_HWindow.HalconWindow, "保存图像失败！", "image",
                               100, 100, "red", "false");
                        }
                    }

                }

            }
            catch (HalconException HDevExpDefaultException)
            {
                try
                {
                    HOperatorSet.DispObj(in_image, Z2_Positive_HWindow.HalconWindow);
                    disp_message(Z2_Positive_HWindow.HalconWindow, "识别失败！", "image", 100, 100, "red", "false");
                    //
                    HOperatorSet.DumpWindowImage(out Itempimage, Z2_Positive_HWindow.HalconWindow);
                    HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                    //以适应窗口的方式显示图像
                    HOperatorSet.SetPart(zm2_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                    //显示图像
                    HOperatorSet.DispObj(Itempimage, zm2_HWindow.HalconWindow);
                    Itempimage.Dispose();
                    A_save_image(_Save_image_directory_);//不存在时创建目录
                    //获取时间
                    string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString();
                   
                    //if (_CBZM2_saver_image == 0)
                    //{
                        string data1 = _Save_image_directory_ + "\\焊后正面识别\\NG\\P-" + FindSN_Module;
                        HOperatorSet.WriteImage(Z2_Positive_Image, "jpg", 0, data1);
                    //}
                    string data2 = _Save_image_directory_ + "\\焊后正面识别\\NG\\"  + FindSN_Module;
                    HOperatorSet.DumpWindow(zm2_HWindow.HalconWindow, "png", data2);

                    ho_Image_AffinTrans.Dispose();
                    ho_ImageZoomed1.Dispose();
                    ho_out_EO_error_region.Dispose();
                    Itempimage.Dispose();
                }
                catch (System.Exception ex)
                {
                    //新增20180415
                    HOperatorSet.DispObj(ho_Image_AffinTrans, zm2_HWindow.HalconWindow);
                    HOperatorSet.DispObj(ho_Image_AffinTrans, Z2_Positive_HWindow.HalconWindow);
                    disp_message(Z2_Positive_HWindow.HalconWindow, "保存图像失败！", "image",
                        100, 100, "red", "false");
                    disp_message(zm2_HWindow.HalconWindow, "保存图像失败！", "image",
                       100, 100, "red", "false");

                }
                _ERROR = true;//NG

            }

            ho_Image_AffinTrans.Dispose();
            ho_ImageZoomed1.Dispose();
            ho_out_EO_error_region.Dispose();
            Itempimage.Dispose();

            return _ERROR;

        }


        
        #endregion
        //
        #endregion
        //
        #region 焊后侧面第二次识别-(暂时未使用)
        //=================================================================
        public bool C2_Side = true;     //拍照标记
        public int C2_resTest = -1;           //识别完成标记
        //
        public HalconDotNet.HWindowControl C2_Side_HWindow;//侧面图像显示窗口
        //
        public HObject C2_Side_Image = null;//模板图像
        //侧面：保存焊接后拍照图像
        public void C2_SnapPos_image()
        {
            try
            {
                unsafe
                {
                    byte[] image_date = new byte[Len];
                    fixed (byte* p = &image_date[0])
                    {
                        if (JHCap.CameraQueryImage(CamID[LF2CamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                        {
                            //_GEN_IMAGE_ERROR_ = true;
                            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                            System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                            HOperatorSet.GenImageInterleaved(out C2_Side_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                            img.UnlockBits(data);
                            img.Dispose();
                            image_date = null;

                            if (!CheckImg(C2_Side_Image))
                            {
                                Log("侧面相机CheckImg失败");
                                InitLeftCam2();
                            }

                            //显示图像
                            HOperatorSet.DispObj(C2_Side_Image, C2_Side_HWindow.HalconWindow);

                            try
                            {
                                if (_CM2_saver_image > 0)
                                {
                                    A_save_image(_Save_image_directory_);//不存在时创建目录
                                    //获取时间
                                    string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                        + DateTime.Now.Minute.ToString()
                                        + DateTime.Now.Second.ToString();
                                    string data1 = _Save_image_directory_ + "\\焊后侧面识别\\P1-" + _DATATIME_M_;
                                    HOperatorSet.WriteImage(C2_Side_Image, "jpg", 0, data1);

                                }
                            }
                            catch (System.Exception ex2)
                            {

                            }

                        }
                        else
                        {
                            Log("侧面相机拍照失败");
                            InitLeftCam2();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                InitLeftCam2();
            }
        }
        //侧面：焊接后识别线程
        public void C2_Side_recognition()
        {
            while (!bTerminate)
            {
                while (C2_Side)
                {
                    Thread.Sleep(50);
                    continue;
                }
                C2_resTest = -1;

                try
                {
                    unsafe
                    {
                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[LF2CamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out C2_Side_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;


                                if (!CheckImg(C2_Side_Image))
                                {
                                    Log("侧面相机CheckImg失败");
                                    InitLeftCam2();
                                    continue;
                                }

                                //显示图像
                                HOperatorSet.DispObj(C2_Side_Image, C2_Side_HWindow.HalconWindow);

                                //Log("焊接后侧面相机开始检测");

                                try
                                {
                                    if (_CM2_saver_image > 0)
                                    {
                                        A_save_image(_Save_image_directory_);//不存在时创建目录
                                        //获取时间
                                        string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                            + DateTime.Now.Minute.ToString()
                                            + DateTime.Now.Second.ToString();
                                        string data1 = _Save_image_directory_ + "\\焊后侧面识别\\OK\\P1-" + _DATATIME_M_;
                                        HOperatorSet.WriteImage(C2_Side_Image, "jpg", 0, data1);
                                        //string data2 = _Save_image_directory_ + "\\焊后侧面识别\\OK\\P2-" + _DATATIME_M_;
                                        //HOperatorSet.DumpWindow(C2_Side_HWindow.HalconWindow, "jpg", data2);

                                    }
                                }
                                catch (System.Exception ex2)
                                {

                                }


                            }
                            else
                            {
                                Log("侧面相机拍照失败");
                                InitLeftCam2();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam2();
                    continue;
                }

                C2_Side = true;

                C2_resTest = 1;

            }

        }
        #endregion
        //
        public int SN_star_SN_checkbox = 1;//打开或关闭条码识别
        //
        public HTuple FindSN_Module = "0";        //条码
        #region 条码识别
        //条码识别细节窗口
        public HalconDotNet.HWindowControl sn_HWindow;
        //========================================================
        public bool TestStopSN = true;      //拍照标记
        public int SN_resTest = -1;         //拍照完成标记
        public bool SN_error = false;       //条码失败
        //条码图像
        public HObject SN_image = null;
        //
        public HalconDotNet.HWindowControl SN_HWindow;//条码识别窗口
        //
        //条码相机拍照
        public void SN_SnapPosImage()
        {
            try
            {
                unsafe
                {
                    byte[] image_date = new byte[Len];
                    fixed (byte* p = &image_date[0])
                    {
                        if (JHCap.CameraQueryImage(CamID[SNCamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                        {
                            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                            System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                            HOperatorSet.GenImageInterleaved(out SN_image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                            img.UnlockBits(data);
                            img.Dispose();
                            image_date = null;

                            SN_Recognize(SN_image);

                        }
                        else
                        {
                            InitSnCam();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log(e.Message);
                InitSnCam();
            }
        }
        //条码识别：线程
        private void SNProcess()
        {
            while (!bTerminate)
            {
                while (TestStopSN)
                {
                    Thread.Sleep(50);
                    continue;
                }

                SN_resTest = -1;
                SN_error = false;//初始化OK

                try
                {
                    unsafe
                    {
                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[SNCamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out SN_image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;

                                //识别
                                SN_error = SN_Recognize(SN_image);

                            }
                            else
                            {
                                SN_error = false;
                                InitSnCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    SN_error = false;
                    Log(e.Message);
                    InitSnCam();
                    continue;
                }

                TestStopSN = true;
                SN_resTest = 0;

            }
        }
        //条码识别
        public bool SN_Recognize(HObject in_SNimage)
        {
            bool _ERROR = false;//返回OKNG
            HObject Itempimage = null;
            HOperatorSet.GenEmptyObj(out Itempimage);
            Itempimage.Dispose();
            //
            HTuple _W = 0, _H = 0;

            //
            //FindSN_Module = "0";
            string _DATATIME_M_11 = DateTime.Now.ToString("yyyyMMddHHmmssff");
            string _DATATIME_M_21 = DateTime.Now.ToString("yyyyMMdd");
            FindSN_Module = _DATATIME_M_11 + _DATATIME_M_21;


            HObject ho_Rectangle_SN = null, ho_ImageReduced5 = null, ho_SymbolRegions = null;
            HTuple hv_Width, hv_Height, hv_Decoded_name, hv_BarCodeHandle;
            HTuple hv_DecodedDataStrings, hv_Decoded_Number;

            HOperatorSet.GenEmptyObj(out ho_Rectangle_SN);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced5);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);

            try
            {
                HOperatorSet.SetDraw(SN_HWindow.HalconWindow, "margin");
                HOperatorSet.SetLineWidth(SN_HWindow.HalconWindow, 1);
                HOperatorSet.GetImageSize(in_SNimage, out hv_Width, out hv_Height);
                //条码类型
                hv_Decoded_name = "Code 128";
                HOperatorSet.DispObj(in_SNimage, SN_HWindow.HalconWindow);
                ho_Rectangle_SN.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle_SN, 100, 100, hv_Height - 100, hv_Width - 100);
                HOperatorSet.SetColor(SN_HWindow.HalconWindow, "yellow");
                HOperatorSet.DispObj(ho_Rectangle_SN, SN_HWindow.HalconWindow);
                ho_ImageReduced5.Dispose();
               
                HOperatorSet.ReduceDomain(in_SNimage, ho_Rectangle_SN, out ho_ImageReduced5 );
                HOperatorSet.ScaleImageMax(ho_ImageReduced5,out ho_ImageReduced5);
                //scale_image_range(ho_ImageReduced5, out ho_ImageReduced5,100,200);
                HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
                //We expect to decode a single bar code per image
                HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "stop_after_result_num", 1);
                //Read bar code, the resulting string includes the check character
                HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "check_char", "absent");
                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_ImageReduced5, out ho_SymbolRegions, hv_BarCodeHandle,
                    hv_Decoded_name, out hv_DecodedDataStrings);
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Decoded_Number);
                if ((int)(new HTuple(hv_Decoded_Number.TupleGreater(0))) != 0)
                {
                    try
                    {
                        HOperatorSet.SetColor(SN_HWindow.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_SymbolRegions, SN_HWindow.HalconWindow);
                        disp_message(SN_HWindow.HalconWindow, "条码:" + hv_DecodedDataStrings, "image",
                            100, 100, "green", "false");

                        //正面窗口显示条码信息
                        disp_message(HPosWindow.HalconWindow, "条码:" + hv_DecodedDataStrings, "image",
                            hv_Height, 100, "green", "false");

                        FindSN_Module = hv_DecodedDataStrings;

                        HOperatorSet.DumpWindowImage(out Itempimage, SN_HWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(sn_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //显示图像
                        HOperatorSet.DispObj(Itempimage, sn_HWindow.HalconWindow);
                        Itempimage.Dispose();
                        try
                        {
                            if (_SN_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                //string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                //    + DateTime.Now.Minute.ToString()
                                //    + DateTime.Now.Second.ToString();
                                ////string _DATATIME_M_ = DateTime.Now.ToString("yyyyMMddHHmmssff ");
                                ////FindSN_Module = _DATATIME_M_;

                                //string data1 = _Save_image_directory_ + "\\焊后条码识别\\OK\\P1-" + _DATATIME_M_ + "：" + FindSN_Module;
                                //HOperatorSet.WriteImage(in_SNimage, "jpg", 0, data1);
                                //string data2 = _Save_image_directory_ + "\\焊后条码识别\\OK\\P2-" + _DATATIME_M_ + "：" + FindSN_Module;
                                //HOperatorSet.DumpWindow(sn_HWindow.HalconWindow, "jpg", data2);

                                string data1 = _Save_image_directory_ + "\\焊后条码识别\\OK\\P1-"  + FindSN_Module;
                                HOperatorSet.WriteImage(in_SNimage, "jpg", 0, data1);
                                string data2 = _Save_image_directory_ + "\\焊后条码识别\\OK\\P2-" + FindSN_Module;
                                HOperatorSet.DumpWindow(sn_HWindow.HalconWindow, "jpg", data2);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                            string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                            FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;
                        }

                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        //FindSN_Module = "0";
                        disp_message(SN_HWindow.HalconWindow, "条码识别失败", "image",
                            100, 100, "red", "false");

                        //正面窗口显示条码信息
                        disp_message(HPosWindow.HalconWindow, "条码识别失败", "image",
                            hv_Height, 100, "red", "false");

                        HOperatorSet.DumpWindowImage(out Itempimage, SN_HWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(sn_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //显示图像
                        HOperatorSet.DispObj(Itempimage, sn_HWindow.HalconWindow);
                        Itempimage.Dispose();
                        try
                        {
                            //   if (_SN_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                //string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                //    + DateTime.Now.Minute.ToString()
                                //    + DateTime.Now.Second.ToString();
                                string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                                string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                                FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;

                                string data1 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P1-" + FindSN_Module;
                                HOperatorSet.WriteImage(in_SNimage, "jpg", 0, data1);
                                string data2 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P2-" + FindSN_Module;
                                HOperatorSet.DumpWindow(sn_HWindow.HalconWindow, "jpg", data2);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                            string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                            FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;
                        }
                        _ERROR = true;//NG
                    }
                }
                else
                {
                    //FindSN_Module = "0";
                    disp_message(SN_HWindow.HalconWindow, "条码识别失败", "image",
                        100, 100, "red", "false");
                    //正面窗口显示条码信息
                    disp_message(HPosWindow.HalconWindow, "条码识别失败", "image",
                        hv_Height, 100, "red", "false");

                    HOperatorSet.DumpWindowImage(out Itempimage, SN_HWindow.HalconWindow);
                    HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                    //以适应窗口的方式显示图像
                    HOperatorSet.SetPart(sn_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                    //显示图像
                    HOperatorSet.DispObj(Itempimage, sn_HWindow.HalconWindow);
                    Itempimage.Dispose();
                    try
                    {
                        if (_SN_saver_image > 0)
                        {
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            //string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                            //    + DateTime.Now.Minute.ToString()
                            //    + DateTime.Now.Second.ToString();

                            string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                            string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                            FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;

                            string data1 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P1-" + FindSN_Module;
                            HOperatorSet.WriteImage(in_SNimage, "jpg", 0, data1);
                            string data2 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P2-" + FindSN_Module;
                            HOperatorSet.DumpWindow(sn_HWindow.HalconWindow, "jpg", data2);
                        }
                    }
                    catch (System.Exception ex2)
                    {
                        string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                        FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;
                    }
                    _ERROR = true;//NG
                }
                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
            }
            catch (System.Exception ex)
            {
                //FindSN_Module = "0";
                HOperatorSet.DumpWindowImage(out Itempimage, SN_HWindow.HalconWindow);
                HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                //以适应窗口的方式显示图像
                HOperatorSet.SetPart(sn_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                //显示图像
                HOperatorSet.DispObj(Itempimage, sn_HWindow.HalconWindow);
                Itempimage.Dispose();
                try
                {
                    if (_SN_saver_image > 0)
                    {
                        A_save_image(_Save_image_directory_);//不存在时创建目录
                        //获取时间
                        //string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                        //    + DateTime.Now.Minute.ToString()
                        //    + DateTime.Now.Second.ToString();
                        string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                        string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                        FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;

                        string data1 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P1-" + FindSN_Module;
                        HOperatorSet.WriteImage(in_SNimage, "jpg", 0, data1);
                        string data2 = _Save_image_directory_ + "\\焊后条码识别\\NG\\P2-" + FindSN_Module;
                        HOperatorSet.DumpWindow(sn_HWindow.HalconWindow, "jpg", data2);
                    }
                }
                catch (System.Exception ex2)
                {
                    string _DATATIME_M_1 = DateTime.Now.ToString("yyyyMMddHHmmssff");
                    string _DATATIME_M_2 = DateTime.Now.ToString("yyyyMMdd");
                    FindSN_Module = _DATATIME_M_1 + _DATATIME_M_2;
                }
                _ERROR = true;//NG
            }
            ho_Rectangle_SN.Dispose();
            ho_ImageReduced5.Dispose();
            ho_SymbolRegions.Dispose();
            return _ERROR;//返回OKNG消息
        }



        //界面按键测试条码
        public void str_SN()
        {
            bool error = SN_Recognize(SN_image);
        }
        #endregion
        //
        //定位细节窗口
        public HalconDotNet.HWindowControl zm1_HWindow;
        #region 焊前正面识别
        //焊前正面识别显示窗口
        public HalconDotNet.HWindowControl zm1_soldering_HWindow;
        //J-20180106：读取模板图像
        //public HObject zm1_Image = null;//调用焊前定位图像
        //
        public HObject zm1_Positive_Image = null;//焊前正面识别图像
        public HObject zm1_Template_img = null;//焊前正面识别模板图像
        public HTuple hv_zm1_TemplateID = null;  //焊前正面识别模板号码
        HObject zm1_Template_region = null;       //焊前正面识别模板区域
        //
        public bool zm1_show_lr = false;         //调试显示使用
        //
        //焊前正面检测20180108jin
        //   public bool zm1_star_cam = true;     //拍照标记
        //    public int zm1_resTest = -1;           //识别完成标记
        //
        public int zm1_row = 380;                //起始位置R
        public int zm1_column = -434;            //起始位置W
        public int zm1_HJ_spacing = 380;         //每个焊接位置的间距
        public int zm1_GR_W = 81;               //检测大小W
        public int zm1_GR_H = 55;                //检测大小H
        public int hv_WeldBalckH = 5;            //黑色汇流条最高点距离矩形中心的距离
        public int hv_WeldWhiteH = 25;           //白色汇流条最高点距离矩形中心的距离
        public int hv_WeldWhiteL = 25;  //最大长度检测

        //创建正面焊前模板区域-jin20180107
        public void zm1_tlp_region(int y, int x, int w, int h)
        {
            TEMP_F2P_X = x;
            TEMP_F2P_Y = y;
            TEMP_F2P_W = w;
            TEMP_F2P_H = h;
            try
            {
                HOperatorSet.GenEmptyObj(out zm1_Template_region);
                zm1_Template_region.Dispose();
                HOperatorSet.GenRectangle2(out zm1_Template_region, y, x, 0, w, h);
                HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "yellow");
                HOperatorSet.DispObj(zm1_Positive_Image, zm1_soldering_HWindow.HalconWindow);
                HOperatorSet.DispObj(zm1_Template_region, zm1_soldering_HWindow.HalconWindow);
            }
            catch (System.Exception ex)
            {
                int error = 1;
            }
        }
        //读取模板
        public void zm1_load_Template_img_j()
        {
            //HOperatorSet.SetDraw(Q_lr_HWindow.HalconWindow, "margin");
            //HOperatorSet.SetLineWidth(Q_lr_HWindow.HalconWindow, 1);
            try
            {
                HTuple hv_ImageName = new HTuple();
                hv_ImageName = TplPath + "\\zm1_template_img.bmp";
                HOperatorSet.ReadImage(out zm1_Template_img, hv_ImageName);
                //建立模板
                HTuple hv_pi;
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(zm1_Template_img, 4, -hv_pi, 2 * hv_pi, hv_pi / 45,
                    "sort", "original", out hv_zm1_TemplateID);
            }
            catch (Exception)
            {
                MessageBox.Show("焊前正面识别模板读取失败！");
            }

        }
        //zm1：创建模板
        public void zm1_save_Template_img_j()
        {
            try
            {
                HObject ho_ImageReduced;
                HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                HObject ho_ImagePart;
                HOperatorSet.GenEmptyObj(out ho_ImagePart);
                HTuple hv_pi;

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(zm1_Positive_Image, zm1_Template_region, out ho_ImageReduced);
                ho_ImagePart.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                HOperatorSet.ZoomImageFactor(ho_ImagePart, out zm1_Template_img, 0.5, 0.5, "constant");
                //建立模板
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                HOperatorSet.CreateTemplateRot(zm1_Template_img, 4, -hv_pi, 2 * hv_pi, hv_pi / 45,
                    "sort", "original", out hv_zm1_TemplateID);

                HOperatorSet.WriteImage(zm1_Template_img, "bmp", 0, TplPath + "\\zm1_template_img.bmp");

                ho_ImageReduced.Dispose();
                ho_ImagePart.Dispose();

                MessageBox.Show("焊前正面识别模板保存成功！");

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("焊前正面识别模板保存失败！");
            }

        }
        //焊前正面识别图像矫正（调用焊后正面识别图像矫正函数）
        //焊前正面识别函数
        #region 注销
        //public bool zm1_Positive_weld_before_check(HObject ho_Image_AffinTrans,  HTuple hv_TemplateID)
        //{
        //    int NUM_ERROR = 0;//OK

        //    // Stack for temporary objects 
        //    HObject[] OTemp = new HObject[20];

        //    // Local iconic variables 

        //    HObject ho_ImageZoomed1, ho_Rectangle1 = null;
        //    HObject ho_ImageReduced1 = null, ho_ImageInvert = null, ho_Region2 = null;
        //    HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
        //    HObject ho_WhiteObject = null, ho_ObjectSelected = null, ho_ObjectsConcat = null;
        //    HObject ho_ImageFFT = null, ho_ImageGauss = null, ho_ImageConvol = null;
        //    HObject ho_ImageFFT1 = null, ho_ImageSub = null, ho_InvertImage = null;
        //    HObject ho_Region01 = null, ho_Region02 = null, ho_Region = null;
        //    HObject ho_ConnectedRegion = null, ho_SelectedRegion = null;

        //    // Local control variables 

        //    HTuple hv_Width = null, hv_Height = null, hv_pi = null;
        //    HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
        //    HTuple hv_Error = null, hv_row_2 = new HTuple(), hv_column_2 = new HTuple();
        //    HTuple hv_out_temp_error = new HTuple(), hv_Q_row = new HTuple();
        //    HTuple hv_Q_column = new HTuple(), hv_Q_HJ_spacing = new HTuple();
        //    HTuple hv_Q_PY_temp_spacing = new HTuple(), hv_Index2 = new HTuple();
        //    HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
        //    HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
        //    HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple();
        //    HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
        //    HTuple hv_Number = new HTuple(), hv_i = new HTuple(), hv_Min = new HTuple();
        //    HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
        //    HTuple hv_Num = new HTuple(), hv_Mean2 = new HTuple();
        //    HTuple hv_Deviation2 = new HTuple(), hv_Row31 = new HTuple();
        //    HTuple hv_Column31 = new HTuple(), hv_Row41 = new HTuple();
        //    HTuple hv_Column41 = new HTuple(), hv_Row11 = new HTuple();
        //    HTuple hv_Column11 = new HTuple(), hv_Row21 = new HTuple();
        //    HTuple hv_Column21 = new HTuple(), hv_Area = new HTuple();
        //    HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
        //    // HTuple hv_WeldBalckH = new HTuple(), hv_WeldWhiteH = new HTuple();
        //    // Initialize local and output iconic variables 

        //    HObject ho_ResultObjectOK, ho_ResultObjectNG = null;
        //    HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
        //    HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);

        //    HOperatorSet.GenEmptyObj(out ho_ImageZoomed1);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageInvert);
        //    HOperatorSet.GenEmptyObj(out ho_Region2);
        //    HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
        //    HOperatorSet.GenEmptyObj(out ho_WhiteObject);
        //    HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
        //    HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);
        //    HOperatorSet.GenEmptyObj(out ho_ImageFFT);
        //    HOperatorSet.GenEmptyObj(out ho_ImageGauss);
        //    HOperatorSet.GenEmptyObj(out ho_ImageConvol);
        //    HOperatorSet.GenEmptyObj(out ho_ImageFFT1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageSub);
        //    HOperatorSet.GenEmptyObj(out ho_InvertImage);
        //    HOperatorSet.GenEmptyObj(out ho_Region01);
        //    HOperatorSet.GenEmptyObj(out ho_Region02);
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_ConnectedRegion);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegion);

        //    try
        //    {
        //        HOperatorSet.GetImageSize(ho_Image_AffinTrans, out hv_Width, out hv_Height);
        //        ho_ImageZoomed1.Dispose();
        //        HOperatorSet.ZoomImageFactor(ho_Image_AffinTrans, out ho_ImageZoomed1, 0.5,
        //            0.5, "constant");
        //        hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
        //        //匹配位置
        //        HOperatorSet.BestMatchRotMg(ho_ImageZoomed1, hv_TemplateID, -hv_pi, 2 * hv_pi,
        //            40, "true", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
        //        //disp_arrow (WindowID, Row*2, Column*2, Row*2 - cos(Angle) * 50, Column*2 - sin(Angle) * 50, 15)
        //        if ((int)(new HTuple(hv_Error.TupleLess(30))) != 0)
        //        {

        //            hv_row_2 = hv_Row * 2;
        //            hv_column_2 = hv_Column * 2;
        //            //输出缺陷
        //            hv_out_temp_error = 0;
        //            //起始位置R
        //            hv_Q_row = zm1_row;
        //            //起始位置W
        //            hv_Q_column = zm1_column;
        //            //每个焊接位置的间距
        //            hv_Q_HJ_spacing = zm1_HJ_spacing;
        //            //每次偏移位置累加
        //            hv_Q_PY_temp_spacing = hv_Q_column + 0;

        //            ho_ResultObjectOK.Dispose();
        //            HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
        //            ho_ResultObjectNG.Dispose();
        //            HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
        //            for (hv_Index2 = 0; (int)hv_Index2 <= 3; hv_Index2 = (int)hv_Index2 + 1)
        //            {
        //                //
        //                ho_Rectangle1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
        //                    0, zm1_GR_W, zm1_GR_H );
        //                //测试位置
        //                hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;

        //                //stop ()

        //                ho_ImageReduced1.Dispose();
        //                HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle1, out ho_ImageReduced1
        //                    );
        //                //
        //                HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
        //                    out hv_Row2, out hv_Column2);
        //                hv_h_row = hv_Row2 - hv_Row1;
        //                hv_cr_w = hv_Column2 - hv_Column1;
        //                //提取白色状态下的汇流条

        //                ho_ImageInvert.Dispose();
        //                HOperatorSet.InvertImage(ho_ImageReduced1, out ho_ImageInvert);
        //                HOperatorSet.Intensity(ho_Rectangle1, ho_ImageInvert, out hv_Mean, out hv_Deviation);
        //                ho_Region2.Dispose();
        //                HOperatorSet.Threshold(ho_ImageInvert, out ho_Region2, 0, hv_Deviation);
        //                ho_ConnectedRegions.Dispose();
        //                HOperatorSet.Connection(ho_Region2, out ho_ConnectedRegions);
        //                ho_SelectedRegions.Dispose();
        //                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "width",
        //                    "and", 65, 87);
        //                HOperatorSet.SelectShapeStd(ho_SelectedRegions, out ho_SelectedRegions, "max_area", 70);
        //                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
        //                ho_WhiteObject.Dispose();
        //                HOperatorSet.GenEmptyObj(out ho_WhiteObject);
        //                HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);
        //                if ((int)(new HTuple(hv_Number.TupleGreaterEqual(1))) != 0)
        //                {
        //                    HTuple end_val50 = hv_Number;
        //                    HTuple step_val50 = 1;
        //                    for (hv_i = 1; hv_i.Continue(end_val50, step_val50); hv_i = hv_i.TupleAdd(step_val50))
        //                    {
        //                        ho_ObjectSelected.Dispose();
        //                        HOperatorSet.SelectObj(ho_SelectedRegions, out ho_ObjectSelected, hv_i);
        //                        HOperatorSet.MinMaxGray(ho_ObjectSelected, ho_Image_AffinTrans, 0,
        //                            out hv_Min, out hv_Max, out hv_Range);
        //                        if ((int)(new HTuple(hv_Max.TupleGreater(230))) != 0)
        //                        {
        //                            ho_ObjectsConcat.Dispose();
        //                            HOperatorSet.ConcatObj(ho_WhiteObject, ho_ObjectSelected, out ho_ObjectsConcat);  
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    ho_ObjectsConcat.Dispose();
        //                    HOperatorSet.ConcatObj(ho_WhiteObject, ho_SelectedRegions, out ho_ObjectsConcat
        //                        );
        //                }

        //                //提取黑色状态下的汇流条
        //                HOperatorSet.CountObj(ho_ObjectsConcat, out hv_Num);
        //                if ((int)(new HTuple(hv_Num.TupleEqual(0))) != 0)
        //                {
        //                    ho_ImageFFT.Dispose();
        //                    HOperatorSet.RftGeneric(ho_ImageReduced1, out ho_ImageFFT, "to_freq",
        //                        "sqrt", "complex", hv_Width);
        //                    ho_ImageGauss.Dispose();
        //                    HOperatorSet.GenGaussFilter(out ho_ImageGauss, 100, 100, 0, "n", "rft",
        //                        hv_Width, hv_Height);
        //                    ho_ImageConvol.Dispose();
        //                    HOperatorSet.ConvolFft(ho_ImageFFT, ho_ImageGauss, out ho_ImageConvol
        //                        );
        //                    ho_ImageFFT1.Dispose();
        //                    HOperatorSet.RftGeneric(ho_ImageConvol, out ho_ImageFFT1, "from_freq",
        //                        "none", "byte", hv_Width);
        //                    ho_ImageSub.Dispose();
        //                    HOperatorSet.SubImage(ho_ImageReduced1, ho_ImageFFT1, out ho_ImageSub,
        //                        1, 100);
        //                    ho_InvertImage.Dispose();
        //                    HOperatorSet.InvertImage(ho_ImageSub, out ho_InvertImage);
        //                    HOperatorSet.Intensity(ho_Rectangle1, ho_InvertImage, out hv_Mean2, out hv_Deviation2);
        //                    ho_Region01.Dispose();
        //                    HOperatorSet.Threshold(ho_InvertImage, out ho_Region01, 0, hv_Deviation2 / 4);
        //                    ho_Region02.Dispose();
        //                    HOperatorSet.Threshold(ho_InvertImage, out ho_Region02, hv_Mean2 + hv_Deviation2,
        //                        255);
        //                    ho_Region.Dispose();
        //                    HOperatorSet.Union2(ho_Region01, ho_Region02, out ho_Region);
        //                    ho_ConnectedRegion.Dispose();
        //                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegion);
        //                    ho_SelectedRegion.Dispose();
        //                    HOperatorSet.SelectShape(ho_ConnectedRegion, out ho_SelectedRegion, "width",
        //                        "and", 65, 85);
        //                }
        //                else
        //                {
        //                    ho_SelectedRegion.Dispose();
        //                    HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
        //                }
        //                //union2 (SelectedRegion, ObjectsConcat, RegionUnion)
        //                //汇流条长短结果判定

        //                //白色
        //                HOperatorSet.SmallestRectangle1(ho_ObjectsConcat, out hv_Row31, out hv_Column31,
        //                    out hv_Row41, out hv_Column41);
        //                //黑色
        //                HOperatorSet.SmallestRectangle1(ho_SelectedRegion, out hv_Row11, out hv_Column11,
        //                    out hv_Row21, out hv_Column21);
        //                HOperatorSet.AreaCenter(ho_Rectangle1, out hv_Area, out hv_Row3, out hv_Column3);

        //                //黑色状态下的汇流条
        //               // hv_WeldBalckH = 5;
        //                if ((int)(new HTuple(hv_Num.TupleEqual(0))) != 0)
        //                {
        //                    if ((int)(new HTuple(hv_Row11.TupleLess(hv_Row3 + hv_WeldBalckH))) != 0)
        //                    {
        //                        {
        //                            HObject ExpTmpOutVar_0;
        //                            HOperatorSet.ConcatObj(ho_ResultObjectOK, ho_Rectangle1, out ExpTmpOutVar_0
        //                                );
        //                            ho_ResultObjectOK.Dispose();
        //                            ho_ResultObjectOK = ExpTmpOutVar_0;
        //                        }
        //                        //disp_message (3600, 'OK', 'image', Row3+100, Column3-50, 'green', 'false')
        //                        disp_message(zm1_soldering_HWindow.HalconWindow, "OK", "image",
        //                                hv_Row3 + 100, hv_Column3 - 50, "green", "false");
        //                        //HPosWindow.HalconWindow
        //                        disp_message(HPosWindow.HalconWindow, "OK", "image",
        //                         hv_Row3 + 100, hv_Column3 - 50, "green", "false");
        //                    }
        //                    else
        //                    {
        //                        {
        //                            HObject ExpTmpOutVar_0;
        //                            HOperatorSet.ConcatObj(ho_ResultObjectNG, ho_Rectangle1, out ExpTmpOutVar_0
        //                                );
        //                            ho_ResultObjectNG.Dispose();
        //                            ho_ResultObjectNG = ExpTmpOutVar_0;
        //                        }
        //                        //disp_message (3600, 'NG', 'image', Row3+100, Column3-50, 'red', 'false')
        //                        disp_message(zm1_soldering_HWindow.HalconWindow, "NG", "image",
        //                                hv_Row3 + 100, hv_Column3 - 50, "red", "false");
        //                        //HPosWindow.HalconWindow
        //                        disp_message(HPosWindow.HalconWindow, "NG", "image",
        //                                hv_Row3 + 100, hv_Column3 - 50, "red", "false");

        //                        NUM_ERROR++;//NG

        //                    }
        //                }
        //                else
        //                {
        //                    //白色状态下汇流条
        //                    //hv_WeldWhiteH = 25;
        //                    if ((int)(new HTuple(hv_Row31.TupleLess(hv_Row3 + hv_WeldWhiteH))) != 0)
        //                    {
        //                        {
        //                            HObject ExpTmpOutVar_0;
        //                            HOperatorSet.ConcatObj(ho_ResultObjectOK, ho_Rectangle1, out ExpTmpOutVar_0
        //                                );
        //                            ho_ResultObjectOK.Dispose();
        //                            ho_ResultObjectOK = ExpTmpOutVar_0;
        //                        }
        //                        //disp_message (3600, 'OK', 'image', Row3+100, Column3-50, 'green', 'false')
        //                        disp_message(zm1_soldering_HWindow.HalconWindow, "OK", "image",
        //                                     hv_Row3 + 100, hv_Column3 - 50, "green", "false");
        //                        //HPosWindow.HalconWindow
        //                        disp_message(HPosWindow.HalconWindow, "OK", "image",
        //                                     hv_Row3 + 100, hv_Column3 - 50, "green", "false");
        //                    }
        //                    else
        //                    {
        //                        {
        //                            HObject ExpTmpOutVar_0;
        //                            HOperatorSet.ConcatObj(ho_ResultObjectNG, ho_Rectangle1, out ExpTmpOutVar_0
        //                                );
        //                            ho_ResultObjectNG.Dispose();
        //                            ho_ResultObjectNG = ExpTmpOutVar_0;
        //                        }
        //                        //disp_message (3600, 'NG', 'image', Row3+100, Column3-50, 'red', 'false')
        //                        disp_message(zm1_soldering_HWindow.HalconWindow, "NG", "image",
        //                        hv_Row3 + 100, hv_Column3 - 50, "red", "false");
        //                        //HPosWindow.HalconWindow
        //                        disp_message(HPosWindow.HalconWindow, "NG", "image",
        //                        hv_Row3 + 100, hv_Column3 - 50, "red", "false");

        //                        NUM_ERROR++;//NG

        //                    }
        //                }
        //                HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "red");
        //                HOperatorSet.DispObj(ho_ResultObjectNG, zm1_soldering_HWindow.HalconWindow);
        //                HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "green");
        //                HOperatorSet.DispObj(ho_ResultObjectOK, zm1_soldering_HWindow.HalconWindow);

        //            }
        //        }
        //        ho_ImageZoomed1.Dispose();
        //        ho_Rectangle1.Dispose();
        //        ho_ImageReduced1.Dispose();
        //        ho_ImageInvert.Dispose();
        //        ho_Region2.Dispose();
        //        ho_ConnectedRegions.Dispose();
        //        ho_SelectedRegions.Dispose();
        //        ho_WhiteObject.Dispose();
        //        ho_ObjectSelected.Dispose();
        //        ho_ObjectsConcat.Dispose();
        //        ho_ImageFFT.Dispose();
        //        ho_ImageGauss.Dispose();
        //        ho_ImageConvol.Dispose();
        //        ho_ImageFFT1.Dispose();
        //        ho_ImageSub.Dispose();
        //        ho_InvertImage.Dispose();
        //        ho_Region01.Dispose();
        //        ho_Region02.Dispose();
        //        ho_Region.Dispose();
        //        ho_ConnectedRegion.Dispose();
        //        ho_SelectedRegion.Dispose();

        //        //stop ()
        //        if (NUM_ERROR<1)
        //        {
        //            return false;//OK
        //        }
        //        else
        //        {
        //            return true;//NG
        //        }

        //    }
        //    catch (HalconException HDevExpDefaultException)
        //    {
        //        ho_ImageZoomed1.Dispose();
        //        ho_Rectangle1.Dispose();
        //        ho_ImageReduced1.Dispose();
        //        ho_ImageInvert.Dispose();
        //        ho_Region2.Dispose();
        //        ho_ConnectedRegions.Dispose();
        //        ho_SelectedRegions.Dispose();
        //        ho_WhiteObject.Dispose();
        //        ho_ObjectSelected.Dispose();
        //        ho_ObjectsConcat.Dispose();
        //        ho_ImageFFT.Dispose();
        //        ho_ImageGauss.Dispose();
        //        ho_ImageConvol.Dispose();
        //        ho_ImageFFT1.Dispose();
        //        ho_ImageSub.Dispose();
        //        ho_InvertImage.Dispose();
        //        ho_Region01.Dispose();
        //        ho_Region02.Dispose();
        //        ho_Region.Dispose();
        //        ho_ConnectedRegion.Dispose();
        //        ho_SelectedRegion.Dispose();

        //    //    throw HDevExpDefaultException;
        //        return true;//NG

        //    }
        //}

        #endregion
        public bool zm1_Positive_weld_before_check(HObject ho_Image_AffinTrans, out HObject ho_ResultObjectOK,
                          out HObject ho_ResultObjectNG, out HObject ho_ObjectR, HTuple hv_TemplateID)
        // public bool zm1_Positive_weld_before_check(HObject ho_Image_AffinTrans, HTuple hv_TemplateID)
        {
            int NUM_ERROR = 0;//OK

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageZoomed1, ho_Rectangle1 = null;
            HObject ho_ImageReduced1 = null, ho_ImageMean = null, ho_ImageMean1 = null;
            HObject ho_RegionDynThresh = null, ho_ConnectedRegion = null;
            HObject ho_ObjectH = null, ho_RegionSelecteds = null, ho_RegionTran = null;
            HObject ho_RegionTrans = null, ho_WhiteObject = null, ho_ObjectSelected = null;
            HObject ho_ObjectRegions = null, ho_ImageMedian = null, ho_Region1 = null;
            HObject ho_Region3 = null, ho_ConnectedRegions1 = null, ho_SelectedRegions1 = null;
            HObject ho_ImageEmphasize = null, ho_Region4 = null, ho_Region5 = null;
            HObject ho_ConnectedRegions2 = null, ho_SelectedRegions2 = null, Itempimage = null;
            //HObject ho_ObjectR ;
            // Local control variables 

            HTuple hv_Width = null, hv_Height = null, hv_pi = null;
            HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
            HTuple hv_Error = null, hv_row_2 = new HTuple(), hv_column_2 = new HTuple();
            HTuple hv_out_temp_error = new HTuple(), hv_Q_row = new HTuple();
            HTuple hv_Q_column = new HTuple(), hv_Q_HJ_spacing = new HTuple();
            HTuple hv_Q_PY_temp_spacing = new HTuple(), hv_Index2 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_h_row = new HTuple(), hv_cr_w = new HTuple();
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Row22 = new HTuple(), hv_Column22 = new HTuple();
            HTuple hv_WHeight = new HTuple(), hv_i = new HTuple();
            HTuple hv_WHeight_Lmax = new HTuple(), hv_j = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Row13 = new HTuple(), hv_Column13 = new HTuple();
            HTuple hv_Row23 = new HTuple(), hv_Column23 = new HTuple();
            HTuple hv_WWidth = new HTuple(), hv_HHeight = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_UsedThreshold = new HTuple(), hv_Number1 = new HTuple();
            HTuple hv_UsedThreshold1 = new HTuple(), hv_Number2 = new HTuple();
            HTuple hv_Row31 = new HTuple(), hv_Column31 = new HTuple();
            HTuple hv_Row41 = new HTuple(), hv_Column41 = new HTuple();
            HTuple hv_HeightOver = new HTuple(), hv_HeightLess = new HTuple();
            // Initialize local and output iconic variables 
            //HObject ho_ResultObjectOK, ho_ResultObjectNG = null;
            HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
            HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);

            HOperatorSet.GenEmptyObj(out ho_ImageZoomed1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegion);
            HOperatorSet.GenEmptyObj(out ho_ObjectH);
            HOperatorSet.GenEmptyObj(out ho_RegionSelecteds);
            HOperatorSet.GenEmptyObj(out ho_RegionTran);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_WhiteObject);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_ObjectRegions);
            HOperatorSet.GenEmptyObj(out ho_ImageMedian);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_Region3);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_Region4);
            HOperatorSet.GenEmptyObj(out ho_Region5);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_ObjectR);
            try
            {
                HOperatorSet.GetImageSize(ho_Image_AffinTrans, out hv_Width, out hv_Height);
                ho_ImageZoomed1.Dispose();
                HOperatorSet.ZoomImageFactor(ho_Image_AffinTrans, out ho_ImageZoomed1, 0.5,
                    0.5, "constant");
                hv_pi = ((new HTuple(0.0)).TupleAcos()) * 2;
                //匹配位置
                HOperatorSet.BestMatchRotMg(ho_ImageZoomed1, hv_TemplateID, -hv_pi, 2 * hv_pi,
                    40, "true", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                //disp_arrow (WindowID, Row*2, Column*2, Row*2 - cos(Angle) * 50, Column*2 - sin(Angle) * 50, 15)
                if ((int)(new HTuple(hv_Error.TupleLess(30))) != 0)
                {

                    hv_row_2 = hv_Row * 2;
                    hv_column_2 = hv_Column * 2;
                    //输出缺陷
                    hv_out_temp_error = 0;
                    //起始位置R
                    hv_Q_row = zm1_row;
                    //起始位置W
                    hv_Q_column = zm1_column;
                    //每个焊接位置的间距
                    hv_Q_HJ_spacing = zm1_HJ_spacing;
                    //每次偏移位置累加
                    hv_Q_PY_temp_spacing = hv_Q_column + 0;

                    ho_ResultObjectOK.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
                    ho_ResultObjectNG.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
                    for (hv_Index2 = 0; (int)hv_Index2 <= 3; hv_Index2 = (int)hv_Index2 + 1)
                    {
                        //
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_row_2 + hv_Q_row, hv_column_2 + hv_Q_PY_temp_spacing,
                            0, zm1_GR_W, zm1_GR_H);
                        //测试位置
                        hv_Q_PY_temp_spacing = hv_Q_PY_temp_spacing + hv_Q_HJ_spacing;

                        //stop ()

                        ho_ImageReduced1.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image_AffinTrans, ho_Rectangle1, out ho_ImageReduced1
                            );
                        //
                        HOperatorSet.SmallestRectangle1(ho_Rectangle1, out hv_Row1, out hv_Column1,
                            out hv_Row2, out hv_Column2);
                        hv_h_row = hv_Row2 - hv_Row1;
                        hv_cr_w = hv_Column2 - hv_Column1;

                        ///////////////////////////////////////////////////////////////////////////////////////
                        ho_ImageMean.Dispose();
                        HOperatorSet.MeanImage(ho_ImageReduced1, out ho_ImageMean, 1, 30);
                        ho_ImageMean1.Dispose();
                        HOperatorSet.MeanImage(ho_ImageMean, out ho_ImageMean1, 13, 1);
                        //stop ()
                        ho_RegionDynThresh.Dispose();
                        // hv_WeldBalckH = 12;
                        HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                            hv_WeldBalckH, "dark");
                        ho_ConnectedRegion.Dispose();
                        HOperatorSet.Connection(ho_RegionDynThresh, out ho_ConnectedRegion);
                        HOperatorSet.SmallestRectangle1(ho_ConnectedRegion, out hv_Row12, out hv_Column12,
                            out hv_Row22, out hv_Column22);
                        hv_WHeight = hv_Row22 - hv_Row12;
                        ho_ObjectH.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ObjectH);
                        for (hv_i = 0; (int)hv_i <= 1; hv_i = (int)hv_i + 1)
                        {
                            try
                            {
                                HOperatorSet.TupleMax(hv_WHeight, out hv_WHeight_Lmax);
                                if ((int)(new HTuple(hv_WHeight_Lmax.TupleLess(hv_h_row / 5))) != 0)
                                {
                                    hv_WHeight_Lmax = hv_h_row / 5;
                                }
                                ho_RegionSelecteds.Dispose();
                                HOperatorSet.SelectShape(ho_ConnectedRegion, out ho_RegionSelecteds,
                                    "height", "and", hv_WHeight_Lmax, 9999);
                                //获取最大值的位置
                                HOperatorSet.TupleFind(hv_WHeight, hv_WHeight_Lmax, out hv_j);
                                //删除索引位置的值
                                HOperatorSet.TupleRemove(hv_WHeight, hv_j, out hv_WHeight);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Union2(ho_ObjectH, ho_RegionSelecteds, out ExpTmpOutVar_0
                                        );
                                    ho_ObjectH.Dispose();
                                    ho_ObjectH = ExpTmpOutVar_0;
                                }
                                HOperatorSet.TupleLength(hv_j, out hv_Length);
                                if ((int)(new HTuple(hv_Length.TupleGreaterEqual(2))) != 0)
                                {
                                    hv_i = 2;
                                }
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException1)
                            {
                                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                            }
                        }
                        ho_RegionTran.Dispose();
                        HOperatorSet.ShapeTrans(ho_ObjectH, out ho_RegionTran, "convex");
                        HOperatorSet.SmallestRectangle1(ho_RegionTran, out hv_Row13, out hv_Column13,
                            out hv_Row23, out hv_Column23);
                        hv_WWidth = hv_Column23 - hv_Column13;
                        hv_HHeight = hv_Row23 - hv_Row13;
                        if ((int)((new HTuple(hv_WWidth.TupleGreater(30))).TupleAnd(new HTuple(hv_WWidth.TupleLess(125)))) != 0)
                        {
                            ho_RegionTrans.Dispose();
                            HOperatorSet.CopyObj(ho_RegionTran, out ho_RegionTrans, 1, 1);
                        }
                        else
                        {
                            ho_RegionTrans.Dispose();
                            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
                        }
                        HOperatorSet.CountObj(ho_RegionTrans, out hv_Number);
                        ho_WhiteObject.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_WhiteObject);
                        if ((int)(new HTuple(hv_Number.TupleGreaterEqual(1))) != 0)
                        {
                            HTuple end_val75 = hv_Number;
                            HTuple step_val75 = 1;
                            for (hv_i = 1; hv_i.Continue(end_val75, step_val75); hv_i = hv_i.TupleAdd(step_val75))
                            {
                                ho_ObjectSelected.Dispose();
                                HOperatorSet.SelectObj(ho_RegionTrans, out ho_ObjectSelected, hv_i);
                                HOperatorSet.MinMaxGray(ho_ObjectSelected, ho_Image_AffinTrans, 0,
                                    out hv_Min, out hv_Max, out hv_Range);
                                if ((int)(new HTuple(hv_Max.TupleGreaterEqual(44))) != 0)
                                {
                                    ho_ObjectRegions.Dispose();
                                    HOperatorSet.ConcatObj(ho_WhiteObject, ho_ObjectSelected, out ho_ObjectRegions
                                        );
                                }
                            }
                        }
                        else
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_ImageReduced1, HDevWindowStack.GetActive());
                            }
                            ho_ImageMedian.Dispose();
                            HOperatorSet.MedianImage(ho_ImageReduced1, out ho_ImageMedian, "circle",
                                4, "mirrored");
                            ho_Region1.Dispose();
                            HOperatorSet.BinaryThreshold(ho_ImageMedian, out ho_Region1, "max_separability",
                                "light", out hv_UsedThreshold);
                            ho_Region3.Dispose();
                            HOperatorSet.Threshold(ho_ImageMedian, out ho_Region3, hv_UsedThreshold + 20,
                                255);
                            ho_ConnectedRegions1.Dispose();
                            HOperatorSet.Connection(ho_Region3, out ho_ConnectedRegions1);
                            ho_SelectedRegions1.Dispose();
                            HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1,
                                "area", "and", 1290, 999999);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.SelectGray(ho_SelectedRegions1, ho_ImageMedian, out ExpTmpOutVar_0,
                                    "mean", "and", 200, 255);
                                ho_SelectedRegions1.Dispose();
                                ho_SelectedRegions1 = ExpTmpOutVar_0;
                            }
                            ho_ObjectRegions.Dispose();
                            HOperatorSet.ConcatObj(ho_WhiteObject, ho_SelectedRegions1, out ho_ObjectRegions
                                );
                            HOperatorSet.CountObj(ho_ObjectRegions, out hv_Number1);
                            if ((int)(new HTuple(hv_Number1.TupleEqual(0))) != 0)
                            {
                                ho_ImageEmphasize.Dispose();
                                HOperatorSet.Emphasize(ho_ImageReduced1, out ho_ImageEmphasize, hv_Width,
                                    hv_Height, 1);
                                ho_Region4.Dispose();
                                HOperatorSet.BinaryThreshold(ho_ImageEmphasize, out ho_Region4, "max_separability",
                                    "dark", out hv_UsedThreshold1);
                                ho_Region5.Dispose();
                                HOperatorSet.Threshold(ho_ImageEmphasize, out ho_Region5, 0, ((hv_UsedThreshold - 40)).TupleAbs()
                                    );
                                ho_ConnectedRegions2.Dispose();
                                HOperatorSet.Connection(ho_Region5, out ho_ConnectedRegions2);
                                ho_SelectedRegions2.Dispose();
                                HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions2,
                                    "area", "and", 1000, 999999);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.SelectShape(ho_SelectedRegions2, out ExpTmpOutVar_0, "width",
                                        "and", 10, (3 * hv_cr_w) / 4);
                                    ho_SelectedRegions2.Dispose();
                                    ho_SelectedRegions2 = ExpTmpOutVar_0;
                                }
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.SelectGray(ho_SelectedRegions2, ho_ImageReduced1, out ExpTmpOutVar_0,
                                        "mean", "and", 0, 60);
                                    ho_SelectedRegions2.Dispose();
                                    ho_SelectedRegions2 = ExpTmpOutVar_0;
                                }
                                ho_ObjectRegions.Dispose();
                                HOperatorSet.ConcatObj(ho_WhiteObject, ho_SelectedRegions2, out ho_ObjectRegions
                                    );
                                //stop ()
                            }

                        }
                        //汇流条长短结果判定
                        //HOperatorSet.DispObj(ho_Image_AffinTrans, zm1_soldering_HWindow.HalconWindow);
                        // HOperatorSet.DispObj(ho_Rectangle1, zm1_soldering_HWindow.HalconWindow);
                        //假如只能提取到提取单边高度
                        hv_HHeight = hv_Row23 - hv_Row13;
                        hv_h_row = hv_Row2 - hv_Row1;
                        HOperatorSet.CountObj(ho_RegionTran, out hv_Number2);
                        if ((int)((new HTuple(hv_HHeight.TupleGreater(0.8 * hv_h_row))).TupleAnd(
                            new HTuple(hv_Number2.TupleEqual(1)))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ObjectRegions, ho_RegionTran, out ExpTmpOutVar_0
                                    );
                                ho_ObjectRegions.Dispose();
                                ho_ObjectRegions = ExpTmpOutVar_0;
                            }
                            //stop ()
                        }

                        HOperatorSet.SmallestRectangle1(ho_ObjectRegions, out hv_Row31, out hv_Column31,
                            out hv_Row41, out hv_Column41);
                        //汇流条超出底边高度
                        hv_HeightOver = hv_Row2 - hv_Row31;
                        //hv_WeldWhiteH = 35;



                        if (Z2_star_zm1_solderingL == 1)
                        {
                            //判断汇流条过长
                            hv_HeightLess = hv_Row31 - hv_Row1;
                        }
                        else
                        {
                            //判断汇流条过长
                            hv_HeightLess = hv_h_row;
                        }


                        //hv_HeightLess=40;
                        if ((int)((new HTuple(hv_HeightOver.TupleGreater((hv_WeldWhiteH / (hv_h_row + 0.000001)) * 100)))
                            .TupleAnd(new HTuple(hv_HeightLess.TupleGreater((hv_WeldWhiteL / (hv_h_row + 0.000001)) * 100)))) != 0)
                        //if ((int)(new HTuple(hv_HeightOver.TupleGreater((hv_WeldWhiteH / (hv_h_row+0.000001))*100))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ResultObjectOK, ho_Rectangle1, out ExpTmpOutVar_0
                                    );
                                ho_ResultObjectOK.Dispose();
                                ho_ResultObjectOK = ExpTmpOutVar_0;
                                //disp_message(zm1_soldering_HWindow.HalconWindow, "OK", "image",
                                //       hv_Row2 + 100, hv_Column2 - 50, "green", "false");
                            }
                        }
                        else
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ConcatObj(ho_ResultObjectNG, ho_Rectangle1, out ExpTmpOutVar_0
                                    );
                                ho_ResultObjectNG.Dispose();
                                ho_ResultObjectNG = ExpTmpOutVar_0;
                                //disp_message(zm1_soldering_HWindow.HalconWindow, "NG", "image",
                                //       hv_Row2 + 100, hv_Column2 - 50, "red", "false");
                                NUM_ERROR++;//NG
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ObjectR, ho_ObjectRegions, out ExpTmpOutVar_0
                                );
                            ho_ObjectR.Dispose();
                            ho_ObjectR = ExpTmpOutVar_0;
                        }
                        //新增显示功能 20180419
                        HOperatorSet.SetLineWidth(HPosWindow.HalconWindow, 2);
                        if (WeldShow == 0)
                        {
                            HOperatorSet.SetColor(HPosWindow.HalconWindow, "yellow");
                            HOperatorSet.DispObj(ho_ObjectR, HPosWindow.HalconWindow);
                        }
                    }
                }
                ho_ImageZoomed1.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegion.Dispose();
                ho_ObjectH.Dispose();
                ho_RegionSelecteds.Dispose();
                ho_RegionTran.Dispose();
                ho_RegionTrans.Dispose();
                ho_WhiteObject.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ObjectRegions.Dispose();
                ho_ImageMedian.Dispose();
                ho_Region1.Dispose();
                ho_Region3.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ImageEmphasize.Dispose();
                ho_Region4.Dispose();
                ho_Region5.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions2.Dispose();

                //stop ()
                if (NUM_ERROR < 1)
                {
                    return false;//OK
                }
                else
                {
                    return true;//NG
                }

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageZoomed1.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegion.Dispose();
                ho_ObjectH.Dispose();
                ho_RegionSelecteds.Dispose();
                ho_RegionTran.Dispose();
                ho_RegionTrans.Dispose();
                ho_WhiteObject.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ObjectRegions.Dispose();
                ho_ImageMedian.Dispose();
                ho_Region1.Dispose();
                ho_Region3.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ImageEmphasize.Dispose();
                ho_Region4.Dispose();
                ho_Region5.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions2.Dispose();

                //    throw HDevExpDefaultException;
                return true;//NG

            }
        }


        //焊前正面识别显示
        public void Weld_result_show(HObject ho_Image_AffinTrans, HObject ho_ResultObjectOK,
                  HObject ho_ResultObjectNG, HObject ho_ObjectR)
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
                HOperatorSet.ClearWindow(zm1_soldering_HWindow.HalconWindow);
                HOperatorSet.SetLineWidth(zm1_soldering_HWindow.HalconWindow, 3);
                HOperatorSet.DispObj(ho_Image_AffinTrans, zm1_soldering_HWindow.HalconWindow);
              
                    HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "yellow");
                    HOperatorSet.DispObj(ho_ObjectR, zm1_soldering_HWindow.HalconWindow);
              
                HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "green");
                HOperatorSet.DispObj(ho_ResultObjectOK, zm1_soldering_HWindow.HalconWindow);
                HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "red");
                HOperatorSet.DispObj(ho_ResultObjectNG, zm1_soldering_HWindow.HalconWindow);
                ///////////////////////////////////////////////////////////////////////
                if (WeldShow == 0)
                {
                    //新增显示20180419
                    HOperatorSet.SetColor(HPosWindow.HalconWindow, "green");
                    HOperatorSet.DispObj(ho_ResultObjectOK, HPosWindow.HalconWindow);
                    HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_ResultObjectNG, HPosWindow.HalconWindow);
                }





                HOperatorSet.CountObj(ho_ResultObjectOK, out hv_NumOK);
                HOperatorSet.CountObj(ho_ResultObjectNG, out hv_NumNG);
                HTuple end_val13 = hv_NumOK;
                HTuple step_val13 = 1;
                for (hv_j = 1; hv_j.Continue(end_val13, step_val13); hv_j = hv_j.TupleAdd(step_val13))
                {
                    HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "green");
                    ho_SelectedObjectOK.Dispose();
                    HOperatorSet.SelectObj(ho_ResultObjectOK, out ho_SelectedObjectOK, hv_j);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectOK, out hv_RowOK, out hv_ColumnOK,
                        out hv_Radius);
                    HOperatorSet.SetTposition(zm1_soldering_HWindow.HalconWindow, hv_RowOK + 200, hv_ColumnOK - 50);
                    HOperatorSet.WriteString(zm1_soldering_HWindow.HalconWindow, "OK");
                    if (WeldShow == 0)
                    {
                    //新增显示20180419
                    HOperatorSet.SetColor(HPosWindow.HalconWindow, "green");
                    HOperatorSet.SetTposition(HPosWindow.HalconWindow, hv_RowOK + 200, hv_ColumnOK - 50);
                    HOperatorSet.WriteString(HPosWindow.HalconWindow, "OK");
                    }
                }

                HTuple end_val21 = hv_NumNG;
                HTuple step_val21 = 1;
                for (hv_k = 1; hv_k.Continue(end_val21, step_val21); hv_k = hv_k.TupleAdd(step_val21))
                {
                    HOperatorSet.SetColor(zm1_soldering_HWindow.HalconWindow, "red");
                    ho_SelectedObjectNG.Dispose();
                    HOperatorSet.SelectObj(ho_ResultObjectNG, out ho_SelectedObjectNG, hv_k);
                    HOperatorSet.SmallestCircle(ho_SelectedObjectNG, out hv_RowNG, out hv_ColumnNG,
                        out hv_Radius);
                    HOperatorSet.SetTposition(zm1_soldering_HWindow.HalconWindow, hv_RowNG + 200, hv_ColumnNG - 50);
                    HOperatorSet.WriteString(zm1_soldering_HWindow.HalconWindow, "NG");
                    if (WeldShow == 0)
                    {
                        //新增显示20180419
                        HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");
                        HOperatorSet.SetTposition(HPosWindow.HalconWindow, hv_RowNG + 200, hv_ColumnNG - 50);
                        HOperatorSet.WriteString(HPosWindow.HalconWindow, "NG");
                    }
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
        //焊前正面识别
        public bool zm1_soldering_Recognize(HObject in_image)
        {
            try
            {
                HTuple hv_Width, hv_Height;
                HObject ho_Image_AffinTrans = null;
                HObject ho_ResultObjectOK = null;
                HObject ho_ResultObjectNG = null;
                HObject ho_ObjectR = null;
                HOperatorSet.GenEmptyObj(out ho_Image_AffinTrans);
                HOperatorSet.GenEmptyObj(out ho_ResultObjectOK);
                HOperatorSet.GenEmptyObj(out ho_ResultObjectNG);
                HOperatorSet.GenEmptyObj(out ho_ObjectR);
                HOperatorSet.GetImageSize(in_image, out hv_Width, out hv_Height);
                ho_Image_AffinTrans.Dispose();
                //图像角度矫正
                Star_AffineTransImage(in_image, out ho_Image_AffinTrans, hv_Height, hv_Width);
                //开始识别函数
                //bool NUM_ER = zm1_Positive_weld_before_check(ho_Image_AffinTrans, hv_zm1_TemplateID);
                bool NUM_ER = zm1_Positive_weld_before_check(ho_Image_AffinTrans, out ho_ResultObjectOK, out ho_ResultObjectNG, out ho_ObjectR, hv_zm1_TemplateID);

                Weld_result_show(ho_Image_AffinTrans, ho_ResultObjectOK, ho_ResultObjectNG, ho_ObjectR);
                return NUM_ER;
            }
            catch (System.Exception ex)
            {
                //失败暂时不做处理
                return true;//NG
            }
        }
        #endregion
        #region 正面拍照
        //正面拍照
        public void SnapPosImage()
        {
            try
            {
                unsafe
                {
                    byte[] image_date = new byte[Len];
                    fixed (byte* p = &image_date[0])
                    {
                        if (JHCap.CameraQueryImage(CamID[0], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                        {
                            //_GEN_IMAGE_ERROR_ = true;
                            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                            System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                            HOperatorSet.GenImageInterleaved(out Pos_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                            try
                            {
                                if (_ZM1_saver_image > 0)
                                {
                                    A_save_image(_Save_image_directory_);//不存在时创建目录
                                    //获取时间
                                    string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                        + DateTime.Now.Minute.ToString()
                                        + DateTime.Now.Second.ToString();
                                    string data1 = _Save_image_directory_ + "\\定位\\P1-" + _DATATIME_M_;
                                    HOperatorSet.WriteImage(Pos_Image, "jpg", 0, data1);

                                }
                            }
                            catch (System.Exception ex2)
                            {

                            }

                            img.UnlockBits(data);
                            img.Dispose();
                            image_date = null;

                        }
                        else
                        {
                            InitPosCam();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
                InitPosCam();

            }
        }
        #endregion
        #region 中间模板定位
        public bool PosCreateTPL(HObject ho_TplImage, ref HTuple hv_TemplateID)
        {
            try
            {
                HOperatorSet.CreateTemplateRot(ho_TplImage, 4, -0.2, 0.2, 0.02, "sort", "original",
                    out hv_TemplateID);
                return true;
            }
            catch (HalconException HDevExpDefaultException)
            {
                return false;
            }
        }
        public bool PosActionTPL(HTuple hv_TemplateID, HObject ho_MatchImage)
        {
            //HObject Itempimage = null;
            //HOperatorSet.GenEmptyObj(out Itempimage);
            //Itempimage.Dispose();
            ////
            //HTuple _W = 0, _H = 0;

            // Local control variables 

            HTuple hv_Row, hv_Column;
            HTuple hv_Angle, hv_Error, hv_Yp, hv_Xp, hv_rr;
            try
            {
                HOperatorSet.DispObj(ho_MatchImage, HPosWindow.HalconWindow);//定位窗口显示图片

                HOperatorSet.BestMatchRotMg(ho_MatchImage, hv_TemplateID, -0.2, 0.2, 60, "false",
                    4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);

                if (hv_Error <= 60)
                {

                    hv_Yp = hv_Row.Clone();
                    hv_Xp = hv_Column.Clone();
                    HOperatorSet.SetColor(HPosWindow.HalconWindow, "green");
                    HOperatorSet.SetLineWidth(HPosWindow.HalconWindow, 3);
                    HOperatorSet.DispArrow(HPosWindow.HalconWindow, hv_Yp - 100, hv_Xp, hv_Yp + 100, hv_Xp, 5);
                    HOperatorSet.DispArrow(HPosWindow.HalconWindow, hv_Yp, hv_Xp - 100, hv_Yp, hv_Xp + 100, 5);

                    hv_rr = (hv_Angle * 180) / 3.1416;
                    //坐标显示
                    Pos_X = (hv_Xp - TPLXDef) * PixelScale;
                    Pos_Y = -(hv_Yp - TPLYDef) * PixelScale;
                    Pos_R = -hv_rr;
                    Pos_X_Show = Pos_X;
                    Pos_Y_Show = Pos_Y;
                    Pos_R_Show = hv_rr;
                    HOperatorSet.SetTposition(HPosWindow.HalconWindow, 20, 20);
                    HOperatorSet.WriteString(HPosWindow.HalconWindow, "角度：" + Pos_R_Show.ToString("0.000") + "X：" + Pos_X_Show.ToString("0.000") + "Y：" + Pos_Y_Show.ToString("0.000"));

                    if ((Math.Abs(Pos_X_Show) > TPLXLimit) || (Math.Abs(Pos_Y_Show) > TPLYLimit) || (Math.Abs(Pos_R_Show) > TPLAngleLimit))
                    {
                        HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");
                        HOperatorSet.SetTposition(HPosWindow.HalconWindow, 80, 20);
                        HOperatorSet.WriteString(HPosWindow.HalconWindow, "接线盒偏移超差");
                        return true;
                    }
                    return false;
                }
                else
                {
                    //无检测对象或对象定标错误
                    HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");
                    HOperatorSet.SetTposition(HPosWindow.HalconWindow, 100, 100);
                    HOperatorSet.WriteString(HPosWindow.HalconWindow, "定位失败");
                    return true;
                }
            }
            catch (HalconException HDevExpDefaultException)
            {
                return true;
            }

        }
        #endregion
        #region 检测图像有效性
        private bool CheckImg(HObject ho_SrcImage)
        {
            HObject ho_ImageFilled, ho_Red, ho_Green, ho_Blue;
            HObject ho_Rectangle;


            // Local control variables 

            HTuple hv_Width, hv_Height, hv_Mean, hv_Deviation;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageFilled);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {

                ho_ImageFilled.Dispose();

                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                //HOperatorSet.Decompose3(ho_SrcImage, out ho_Red, out ho_Green, out ho_Blue
                //     );
                HOperatorSet.GetImageSize(ho_SrcImage, out hv_Width, out hv_Height);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 50, 50, hv_Height - 50, hv_Width - 50);
                HOperatorSet.Intensity(ho_Rectangle, ho_SrcImage, out hv_Mean, out hv_Deviation);
                if (hv_Mean > 5)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                ho_ImageFilled.Dispose();
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_Rectangle.Dispose();
            }
        }

        #endregion
        public bool _pos_error = false;//定位失败标记
        public bool _HQ_error = false;//焊前检测NG标记
        #region 定位线程
        private void PosProcess()
        {
            //正面定位前识别模板初始化
            try
            {
                zm1_load_Template_img_j();//读取焊前识别模板图像
            }
            catch (System.Exception ex)
            {
                Log("读取焊前识别模板失败" + ex.Message);
            }

            //定位模板初始化
            HObject ho_MODELS_Image = null;
            HTuple hv_ImageName = new HTuple();
            HTuple hv_TPLID = new HTuple();
            hv_ImageName = TplPath + "\\TPL.jpg";
            StringBuilder temp = new StringBuilder(500);
            GetPrivateProfileString("TPLPOS", "X", "1360", temp, 500, TplPath + "\\Tplcfg.ini");
            TPLXDef = int.Parse(temp.ToString());
            GetPrivateProfileString("TPLPOS", "Y", "1024", temp, 500, TplPath + "\\Tplcfg.ini");
            TPLYDef = int.Parse(temp.ToString());
            try
            {   //读取定位模板
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                PosCreateTPL(ho_MODELS_Image, ref hv_TPLID);
            }
            catch (Exception e)
            {
                Log("读取定位模板失败" + e.Message);
            }

            while (!bTerminate)
            {
                while (PosStop)
                {
                    Thread.Sleep(50);
                    continue;
                }
                PosStop = true;//拍照标记
                resPos = -1;
                _pos_error = false;//定位失败标记
                _HQ_error = false;//焊前检测NG标记

                try
                {
                    unsafe
                    {
                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[0], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Pos_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;
                                if (!CheckImg(Pos_Image))
                                {
                                    _pos_error = true;//定位失败标记
                                    //      _HQ_error = true;//焊前检测NG标记
                                    resPos = 0;//定位和识别结束s
                                    //    PosStop = true;
                                    Log("定位相机CheckImg失败");
                                    InitPosCam();
                                    continue;
                                }
                            }
                            else
                            {
                                _pos_error = true;//定位失败标记
                                //     _HQ_error = true;//焊前检测NG标记
                                resPos = 0;//定位和识别结束
                                //    PosStop = true;
                                Log("定位相机拍照异常");
                                InitPosCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    _pos_error = true;//定位失败标记
                    //      _HQ_error = true;//焊前检测NG标记
                    resPos = 0;//定位和识别结束
                    //     PosStop = true;
                    Log(e.Message);
                    InitPosCam();
                    continue;
                }

                try
                {
                    //定位
                    _pos_error = PosActionTPL(hv_TPLID, Pos_Image);
                    try
                    {
                        _HQ_error = zm1_soldering_Recognize(Pos_Image);
                    }
                    catch (System.Exception ex)
                    {
                        _HQ_error = true;//焊前检测NG标记
                    }


                    //新增 汇流条长度判断显示功能
                    if (_HQ_error == true)
                    {
                        if (WeldShow == 0)
                        {
                        HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");
                        HOperatorSet.SetTposition(HPosWindow.HalconWindow, 280, 20);
                        HOperatorSet.WriteString(HPosWindow.HalconWindow, "汇流条长度过长或过短，请直接流出手工焊接！！！");
                        }
                        }


                    //
                    HObject Itempimage = null;
                    HOperatorSet.GenEmptyObj(out Itempimage);
                    Itempimage.Dispose();
                    HTuple _W = 0, _H = 0;
                    //   if ((_pos_error == false) && _HQ_error)
                    if (_pos_error == false)
                    {//OK
                        HOperatorSet.DumpWindowImage(out Itempimage, HPosWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(zm1_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //显示图像
                        HOperatorSet.DispObj(Itempimage, zm1_HWindow.HalconWindow);
                        Itempimage.Dispose();
                        try
                        {
                            if (_ZM1_saver_image > 0)
                            {
                                A_save_image(_Save_image_directory_);//不存在时创建目录
                                //获取时间
                                string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                    + DateTime.Now.Minute.ToString()
                                    + DateTime.Now.Second.ToString();
                                string data1 = _Save_image_directory_ + "\\定位\\OK\\P1-" + _DATATIME_M_;
                                HOperatorSet.WriteImage(Pos_Image, "jpg", 0, data1);
                                string data2 = _Save_image_directory_ + "\\定位\\OK\\P2-" + _DATATIME_M_;
                                HOperatorSet.DumpWindow(zm1_HWindow.HalconWindow, "jpg", data2);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            //
                        }
                    }
                    else
                    {//NG
                        HOperatorSet.DumpWindowImage(out Itempimage, HPosWindow.HalconWindow);
                        HOperatorSet.GetImageSize(Itempimage, out _W, out _H);
                        //以适应窗口的方式显示图像
                        HOperatorSet.SetPart(zm1_HWindow.HalconWindow, 0, 0, _H - 1, _W - 1);
                        //显示图像
                        HOperatorSet.DispObj(Itempimage, zm1_HWindow.HalconWindow);
                        Itempimage.Dispose();
                        try
                        {
                            //  if (_ZM1_saver_image > 0)
                            //   {
                            A_save_image(_Save_image_directory_);//不存在时创建目录
                            //获取时间
                            string _DATATIME_M_ = DateTime.Now.Hour.ToString()
                                + DateTime.Now.Minute.ToString()
                                + DateTime.Now.Second.ToString();
                            string data1 = _Save_image_directory_ + "\\定位\\NG\\P1-" + _DATATIME_M_;
                            HOperatorSet.WriteImage(Pos_Image, "jpg", 0, data1);
                            string data2 = _Save_image_directory_ + "\\定位\\NG\\P2-" + _DATATIME_M_;
                            HOperatorSet.DumpWindow(zm1_HWindow.HalconWindow, "jpg", data2);
                            //    }
                        }
                        catch (System.Exception ex2)
                        {
                            //
                        }
                        _pos_error = true;//定位失败标记
                    }
                    ////临时
                    //if (_pos_error)
                    //{
                    ////    resPos = 0;//OK
                    //    PosStop = true;//线程标记位暂停置位

                    //}
                    //else
                    //{
                    ////    resPos = 1;//NG
                    //    PosStop = true;
                    //}

                }
                catch (Exception e)
                {
                    ////   resPos = 1;
                    //   PosStop = true;
                    _pos_error = true;//定位失败标记
                }

                resPos = 0;//定位和识别结束

            }

            Thread.Sleep(50);

        }
        #endregion
        //
        #region 文字显示
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = new HTuple(), hv_HeightWin = null;
            HTuple hv_MaxAscent = null, hv_MaxDescent = null, hv_MaxWidth = null;
            HTuple hv_MaxHeight = null, hv_R1 = new HTuple(), hv_C1 = new HTuple();
            HTuple hv_FactorRow = new HTuple(), hv_FactorColumn = new HTuple();
            HTuple hv_UseShadow = null, hv_ShadowColor = null, hv_Exception = new HTuple();
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
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part,
                out hv_Row2Part, out hv_Column2Part);
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
                    HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                        0));
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
                HOperatorSet.SetColor(HPosWindow.HalconWindow, hv_ShadowColor);
                if ((int)(hv_UseShadow) != 0)
                {
                    HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1,
                        hv_C2 + 1);
                }
                //Set box color
                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                    0));
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
        #endregion
        //
        //晶科方案2参数
        //焊接方案选择：1=晶科方案1，2=晶科方案2 英利方案3
        public int star_soldering_mode = 2;                //焊接方案1-2选择，默认：晶科方案1
        //
        public int time_add_soldering = 2000;             //加锡前等待时间
        public double star_add_soldering_L = 30.0;            //加锡长度
        public int time_fornt_pushing = 1000;             //下压前等待时间
        public int time_pushing_soldering = 4000;         //下压焊接时间

        public int time_pushing_soldering_jk1 = 4000;       //Z轴下压后到位后等待时间
        public int time_pushing_soldering_jk2 = 4000;       //Z轴抬起后等待时间
        public double before_compensation_soldering_jk = 4.8;        //Z轴微抬起距离
        public int ClearNum = 0;//焊接多少次清洗1次

        public double before_compensation_soldering = 4.8;         //补锡前Z轴抬起距离
        public double compensation_soldering_L = 30;          //补锡长度
        //开始涂抹
        public int before_time_paint = 1000;    //涂抹前等待时间
        public double paint_X_displacement_distance = 1.7;//涂抹X轴移动距离
        public int paint_speed = 100;//涂抹速度
        public int paint_time = 200;//涂抹暂停时间
        public int paint_number = 1;//涂抹左右摆动次数
        public double paint_Y_displacement_distance = 3.0;//涂抹完成Y轴移动距离
        public bool _cB_paint_X_displacement_distance_ = false;//改为X轴移动距离  //Z:20180418
        //涂抹完成压针下压
        public double accomplish_lift_dist = 2.0;     //X轴或Y轴移动前Z轴抬起距离
        public double accomplish_lift_distance = 5.0;     //涂抹完成Z轴抬起距离
        public int time_accomplish_soldering = 2000;   //焊接完成压针按压与吹起延时时间
        //
        //英利焊接方案3参数 动作设置 参数定义
        #region //英利焊接方案3动作设置 参数定义 金帅20180317

        public int time_add_soldering_YL = 2000;             //加锡前等待时间1
        public double star_add_soldering_L_YL = 30.0;            //加锡长度1
        public int time_fornt_pushing_YL = 1000;             //下压前等待时间1
        public int time_pushing_soldering_YL = 4000;         //下压焊接时间1
        public double before_compensation_soldering_YL = 4.8;         //补锡前Z轴抬起距离1
        public double compensation_soldering_L_YL = 30;          //补锡长度1
        //开始涂抹
        public int before_time_paint_YL = 1000;    //涂抹前等待时间1
        public double paint_X_displacement_distance_YL = 1.7;//涂抹X轴移动距离1
        public int paint_speed_YL = 100;//涂抹速度1
        public int paint_time_YL = 200;//涂抹暂停时间1
        public int paint_number_YL = 1;//涂抹左右摆动次数1
        public double paint_Y_displacement_distance_YL = 3.0;//涂抹完成Y轴移动距离1

        //定义填孔变量
        public double compensation_soldering_L_YL2 = 30;          //二次补锡长度1
        //开始涂抹
        public int before_time_paint_YL2 = 1000;    //二次涂抹前等待时间1
        public double paint_X_displacement_distance_YL2 = 1.7;//二次涂抹X轴移动距离1
        public int paint_speed_YL2 = 100;//二次涂抹速度1
        public int paint_time_YL2 = 200;//二次涂抹暂停时间1
        public int paint_number_YL2 = 1;//二次涂抹左右摆动次数1


        //涂抹完成压针下压
        public double accomplish_lift_distance_YL = 5.0;     //涂抹完成Z轴抬起距离
        public int time_accomplish_soldering_YL = 2000;   //焊接完成压针按压与吹起延时时间








        #endregion

        //应唐勇飞要求新增清洗动作 20180515
        public double CleaningSwingDist_JK = 5;            //清洗摆动距离设置
        public int CleaningTime_JK = 500;                  //清洗时间设置(单位：ms)
        public int RunClearing_JK = 1;                     //清洗动作的选择开关

        public double Pos_X;
        public double Pos_Y;
        public double Pos_R;
        public double Pos_X_Show;
        public double Pos_Y_Show;
        public double Pos_R_Show;
        public double PixelScale;

        public string TplPath;
        //   public string FindSN_Module;//条码
        public string FindSN_Box;
        public int resVision;


        int Width;
        int Height;
        int Len;


        public VisionParam_JK VisionParam_JKObject;

        int[] CamID = new int[8];
        public int weldWidthRight = 0;
        public int weldHeightRight = 0;
        public double weldGrayRight1 = 0;
        public int weldGrayRight2 = 0;
        public int weldGrayRight3 = 0;
        public int weldGrayRight4 = 0;
        public double weldSmallSelLength = 15;
        public double weldSmallSelWidth = 7;
        public int holeWidthLeft = 0;
        public int holeHeightLeft = 0;
        public int holeGrayLeft1 = 0;
        public int holeGrayLeft2 = 0;
        public int holeGrayLeft3 = 0;
        public int holeGrayLeft4 = 0;
        public int holeAreaSel = 200;
        public int holeWidthSel = 30;
        public int weldBackGray = 70;


        public int weldBackPercent = 50;
        public double weldLengthPercent = 80;
        public int weldLimitPercent = 60;
        public int HoleAreaPercent = 60;
        public int BoxSnSelGray = 60;
        public int ErrorBackY = 60;
        public int ErrorHoleW = 60;
        public int ErrorHoleY = 60;
        //public int ErrorBackY = 60;
        //public int ErrorBackY = 60;


        //int SNCamIdx = 2;
        //int LF2CamIdx = 1;


        public int[] resWeldLeft = new int[5];
        public int[] resWeldRight = new int[5];

        public HalconDotNet.HWindowControl HPosWindow;
        public HalconDotNet.HWindowControl HLeftWindow;

        public HalconDotNet.HWindowControl HWindowL1;
        public HalconDotNet.HWindowControl HWindowL2;
        public HalconDotNet.HWindowControl HWindowL3;
        public HalconDotNet.HWindowControl HWindowL4;
        public HalconDotNet.HWindowControl HWindowR1;
        public HalconDotNet.HWindowControl HWindowR2;
        public HalconDotNet.HWindowControl HWindowR3;
        public HalconDotNet.HWindowControl HWindowR4;


        public HObject Pos_Image = null, Left_Image = null, Right_Image = null;

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        void Log(String contents)
        {
            try
            {

                contents = DateTime.Now.ToShortTimeString() + "    " + contents + "\r\n";
                System.IO.File.AppendAllText(Application.StartupPath + "\\CamLog.txt", contents);


            }
            catch (Exception e)
            {

                //System::Windows::Forms::MessageBox::Show("证书打开失败");
            }
            finally
            {

            }

        }

        //
        #region 初始化相机部分（有无用代码，暂时不管）
        public int CamSetNum = 2000;
        //int SNCamIdx = 2;
        //int LF2CamIdx = 1;
        //20180428：设备要求不带侧面

        public int SNCamIdx = 1;
        public int LF2CamIdx = 8;//跳过侧面相机编号

        public bool Init(String[] CamVec)
        {
            try
            {
            if (CamSetNum == 1)
            {
                //正面：定位拍照
                Thread PosThread = new Thread(new ThreadStart(PosProcess));
                PosThread.Start();

                //侧面：焊前汇流条按压检测
                Thread Q_Thread = new Thread(new ThreadStart(Q_Pre_welding_recognition));
                Q_Thread.Start();

                //焊后正面
                Thread Z2_Positive_Thread = new Thread(new ThreadStart(Z2_Positive_recognition));
                Z2_Positive_Thread.Start();

                //条码检测
                Thread SNThread = new Thread(new ThreadStart(SNProcess));
                SNThread.Start();
            }
            else if (CamSetNum == 2)
            {
                //正面：定位拍照
                Thread PosThread = new Thread(new ThreadStart(PosProcess));
                PosThread.Start();

                //焊后正面
                Thread Z2_Positive_Thread = new Thread(new ThreadStart(Z2_Positive_recognition));
                Z2_Positive_Thread.Start();

                //条码检测
                Thread SNThread = new Thread(new ThreadStart(SNProcess));
                SNThread.Start();
            }
            else if (CamSetNum == 3)
            {
                //正面：定位拍照
                Thread PosThread = new Thread(new ThreadStart(PosProcess));
                PosThread.Start();

                //焊后正面
                Thread Z2_Positive_Thread = new Thread(new ThreadStart(Z2_Positive_recognition));
                Z2_Positive_Thread.Start();
            }
            else if (CamSetNum == 4)
            {
                //正面：定位拍照
                Thread PosThread = new Thread(new ThreadStart(PosProcess));
                PosThread.Start();

                //侧面：焊前汇流条按压检测
                Thread Q_Thread = new Thread(new ThreadStart(Q_Pre_welding_recognition));
                Q_Thread.Start();

                //焊后正面
                Thread Z2_Positive_Thread = new Thread(new ThreadStart(Z2_Positive_recognition));
                Z2_Positive_Thread.Start();
            }
            else
            {
                //正面：定位拍照
                Thread PosThread = new Thread(new ThreadStart(PosProcess));
                PosThread.Start();

                //侧面：焊前汇流条按压检测
                Thread Q_Thread = new Thread(new ThreadStart(Q_Pre_welding_recognition));
                Q_Thread.Start();

                //焊后正面
                Thread Z2_Positive_Thread = new Thread(new ThreadStart(Z2_Positive_recognition));
                Z2_Positive_Thread.Start();

                //条码检测
                Thread SNThread = new Thread(new ThreadStart(SNProcess));
                SNThread.Start();
            }
            }
            catch (Exception)
            {
            }
            return InitCam(CamVec);
        }


        public bool InitCam(String[] CamVec)
        {
            try
            {
            int CamCount = 0;
            int res = JHCap.CameraGetCount(ref CamCount);

            if (CamSetNum == 1)
            {
                if ((res != 0) || (CamCount != 3))
                    //if ((res != 0) )
                    return false;

                for (int i = 0; i < CamCount; i++)
                {
                    JHCap.CameraInit(i);
                    StringBuilder ids = new StringBuilder();
                    JHCap.CameraReadSerialNumber(i, ids, 12);
                    if (ids.ToString() == "" || ids.Length < 12)
                        continue;
                    string id = ids.ToString().Substring(0, 12);
                    for (int j = 0; j < CamCount; j++)
                    {
                        if (id.Contains(CamVec[j]))
                        {
                            CamID[i] = j;
                            Log("匹配相机" + i.ToString());
                        }
                    }
                }
                if (!InitPosCam())
                    return false;
                if (!InitLeftCam2())
                    return false;
                if (!InitSnCam())
                    return false;
            }
            else if (CamSetNum == 2)
            {
                if ((res != 0) || (CamCount != 2))
                    //if ((res != 0))
                    return false;

                for (int i = 0; i < CamCount; i++)
                {
                    JHCap.CameraInit(i);
                    StringBuilder ids = new StringBuilder();
                    JHCap.CameraReadSerialNumber(i, ids, 12);
                    if (ids.ToString() == "" || ids.Length < 12)
                        continue;
                    string id = ids.ToString().Substring(0, 12);
                    for (int j = 0; j < CamCount; j++)
                    {
                        if (id.Contains(CamVec[j]))
                        {
                            CamID[i] = j;
                            Log("匹配相机" + i.ToString());
                        }
                    }
                }
                if (!InitPosCam())
                    return false;
                //if (!InitLeftCam2())
                //    return false;
                if (!InitSnCam())
                    return false;
            }
            else if (CamSetNum == 3)
            {
                if ((res != 0) || (CamCount != 1))
                    //if ((res != 0))
                    return false;

                for (int i = 0; i < CamCount; i++)
                {
                    JHCap.CameraInit(i);
                    StringBuilder ids = new StringBuilder();
                    JHCap.CameraReadSerialNumber(i, ids, 12);
                    if (ids.ToString() == "" || ids.Length < 12)
                        continue;
                    string id = ids.ToString().Substring(0, 12);
                    for (int j = 0; j < CamCount; j++)
                    {
                        if (id.Contains(CamVec[j]))
                        {
                            CamID[i] = j;
                            Log("匹配相机" + i.ToString());
                        }
                    }
                }
                if (!InitPosCam())
                    return false;
                //if (!InitLeftCam2())
                //    return false;
                //if (!InitSnCam())
                //    return false;
            }
            else if (CamSetNum == 4)
            {
                if ((res != 0) || (CamCount != 2))
                    //if ((res != 0))
                    return false;

                for (int i = 0; i < CamCount; i++)
                {
                    JHCap.CameraInit(i);
                    StringBuilder ids = new StringBuilder();
                    JHCap.CameraReadSerialNumber(i, ids, 12);
                    if (ids.ToString() == "" || ids.Length < 12)
                        continue;
                    string id = ids.ToString().Substring(0, 12);
                    for (int j = 0; j < CamCount; j++)
                    {
                        if (id.Contains(CamVec[j]))
                        {
                            CamID[i] = j;
                            Log("匹配相机" + i.ToString());
                        }
                    }
                }
                if (!InitPosCam())
                    return false;
                if (!InitLeftCam2())
                    return false;
                //if (!InitSnCam())
                //    return false;
            }
            else
            {
                if ((res != 0) || (CamCount != 3))
                    //if ((res != 0))
                    return false;
                for (int i = 0; i < CamCount; i++)
                {
                    JHCap.CameraInit(i);
                    StringBuilder ids = new StringBuilder();
                    JHCap.CameraReadSerialNumber(i, ids, 12);
                    if (ids.ToString() == "" || ids.Length < 12)
                        continue;
                    string id = ids.ToString().Substring(0, 12);
                    for (int j = 0; j < CamCount; j++)
                    {
                        if (id.Contains(CamVec[j]))
                        {
                            CamID[i] = j;
                            Log("匹配相机" + i.ToString());
                        }
                    }
                }
                if (!InitPosCam())
                    return false;
                if (!InitLeftCam2())
                    return false;
                if (!InitSnCam())
                    return false;
            }
            }
            catch (Exception)
            {
            }
            return true;
        }


        public bool InitPosCam()
        {
            JHCap.CameraFree(CamID[0]);
            while (true)
            {


                int CamCount = 0;
                //int res = JHCap.CameraGetCount(ref CamCount);
                try
                {
                    if (JHCap.CameraInit(CamID[0]) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                    {
                        //高速模式
                        //JHCap.CameraSetHighspeed(g_index, true);
                        //JHCap.CameraSetDelay(g_index, 0);
                        JHCap.CameraSetSnapMode(CamID[0], JHCap.CAMERA_SNAP_CONTINUATION);
                        //获取分辨率
                        JHCap.CameraGetResolution(CamID[0], 0, ref Width, ref Height);
                        JHCap.CameraGetImageBufferSize(CamID[0], ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                        return true;
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);

                }
            }
        }
        public bool InitLeftCam()
        {
            JHCap.CameraFree(CamID[0]);
            while (true)
            {

                int CamCount = 0;
                try
                {
                    //int res = JHCap.CameraGetCount(ref CamCount);
                    if (JHCap.CameraInit(CamID[0]) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                    {
                        //高速模式
                        //JHCap.CameraSetHighspeed(g_index, true);
                        //JHCap.CameraSetDelay(g_index, 0);
                        JHCap.CameraSetSnapMode(CamID[0], JHCap.CAMERA_SNAP_CONTINUATION);
                        //获取分辨率
                        JHCap.CameraGetResolution(CamID[0], 0, ref Width, ref Height);
                        JHCap.CameraGetImageBufferSize(CamID[0], ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                        return true;
                    }
                    //return true;
                }
                catch (Exception e)
                {
                    Log(e.Message);

                }
            }
        }
        public bool InitLeftCam2()
        {
            JHCap.CameraFree(CamID[LF2CamIdx]);
            while (true)
            {

                int CamCount = 0;
                try
                {
                    //int res = JHCap.CameraGetCount(ref CamCount);
                    if (JHCap.CameraInit(CamID[LF2CamIdx]) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                    {
                        //高速模式
                        //JHCap.CameraSetHighspeed(g_index, true);
                        //JHCap.CameraSetDelay(g_index, 0);
                        JHCap.CameraSetSnapMode(CamID[LF2CamIdx], JHCap.CAMERA_SNAP_CONTINUATION);
                        //获取分辨率
                        JHCap.CameraGetResolution(CamID[LF2CamIdx], 0, ref Width, ref Height);
                        JHCap.CameraGetImageBufferSize(CamID[LF2CamIdx], ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                        return true;
                    }
                    //return true;
                }
                catch (Exception e)
                {
                    Log(e.Message);

                }
            }
        }
        public bool InitSnCam()
        {
            JHCap.CameraFree(CamID[SNCamIdx]);
            while (true)
            {

                int CamCount = 0;
                try
                {
                    //int res = JHCap.CameraGetCount(ref CamCount);
                    if (JHCap.CameraInit(CamID[SNCamIdx]) == 0)//(ComboName_admin.Text == "JHSM500(0)")
                    {
                        //高速模式
                        //JHCap.CameraSetHighspeed(g_index, true);
                        //JHCap.CameraSetDelay(g_index, 0);
                        JHCap.CameraSetSnapMode(CamID[SNCamIdx], JHCap.CAMERA_SNAP_CONTINUATION);
                        //获取分辨率
                        JHCap.CameraGetResolution(CamID[SNCamIdx], 0, ref Width, ref Height);
                        JHCap.CameraGetImageBufferSize(CamID[SNCamIdx], ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC); //RGB24
                        return true;
                    }
                    //return true;
                }
                catch (Exception e)
                {
                    Log(e.Message);

                }
            }
        }
        #endregion
        //
        //------------------------------------------------------
        #region SN获取

        private bool SNCreateTPL(HObject ho_TplImage, ref HTuple hv_BarCodeHandle)
        {

            return true;

        }

        private bool SNActionTPL(HTuple hv_TemplateID, HObject ho_MatchImage)
        {

            HSystem sys = new HSystem();

            HObject ho_ImageRotate, ho_Red, ho_Green;
            HObject ho_Blue, ho_ImageScaleMax, ho_SymbolRegions;


            // Local control variables 

            HTuple hv_BarCodeHandle, hv_DecodedDataStrings;

            // Initialize local and output iconic variables 


            // Local control variables 


            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);


            HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "check_char", "absent");
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "num_scanlines", 10);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "min_identical_scanlines", 1);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_min", 1.5);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_max", 8);

            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "timeout", 500);
            //HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_max", 50);


            try
            {
                FindSN_Module = "";





                ho_ImageRotate.Dispose();
                //HOperatorSet.RotateImage(ho_MatchImage, out ho_ImageRotate, 180, "constant");
                //ho_Red.Dispose();
                //ho_Green.Dispose();
                //ho_Blue.Dispose();
                //HOperatorSet.Decompose3(ho_ImageRotate, out ho_Red, out ho_Green, out ho_Blue
                //    );

                //ho_ImageScaleMax.Dispose();
                //HOperatorSet.ScaleImageMax(ho_Blue, out ho_ImageScaleMax);
                HOperatorSet.BinomialFilter(ho_MatchImage, out ho_ImageRotate, 5, 5);

                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_ImageRotate, out ho_SymbolRegions, hv_BarCodeHandle,
                    "Code 128", out hv_DecodedDataStrings);


                FindSN_Module = hv_DecodedDataStrings.ToString();
                //
                return true;





            }
            catch (HalconException HDevExpDefaultException)
            {


                return false;

            }
            finally
            {
                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_ImageRotate.Dispose();
                ho_SymbolRegions.Dispose();
            }
        }
        public void Find_ModuleSN2(HObject ho_DstImage)
        {
            if (!bFindSN)
            {
                return;
            }
            HObject[] OTemp = new HObject[20];
            HObject ho_Image, ho_ImageRotate, ho_Red, ho_Green;
            HObject ho_Blue, ho_Rectangle, ho_ImageReduced, ho_Region;
            HObject ho_ImageReducedSmall, ho_ImageScaled1, ho_SymbolRegions;

            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionUnion, ho_SelectedRegions1;

            HTuple barres;
            // Local control variables 

            HTuple hv_WindowID, hv_Row, hv_Column, hv_Phi;
            HTuple hv_Length1, hv_Length2, hv_BarCodeHandle, hv_Decoded;
            HTuple hv_DecodedDataStrings, hv_Number;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedSmall);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "check_char", "absent");
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "num_scanlines", 10);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "min_identical_scanlines", 1);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_min", 7);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_max", 7);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "meas_thresh", 0.15);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "meas_thresh_abs", 5);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "timeout", 1000);
            try
            {


                FindSN_Module = "";









                //ho_ImageRotate.Dispose();
                //HOperatorSet.RotateImage(ho_DstImage, out ho_ImageRotate, 180, "constant");
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                HOperatorSet.Decompose3(ho_DstImage, out ho_Red, out ho_Green, out ho_Blue
                    );
                ho_Green.Dispose();
                HOperatorSet.SmoothImage(ho_Red, out ho_Green, "gauss", 3);

                //scale_image_range (Blue, ImageScaled, 50, 100)
                //ho_Rectangle.Dispose();
                //HOperatorSet.GenRectangle2(out ho_Rectangle, 1400, 1300, 0, 900, 600);
                //ho_ImageReduced.Dispose();
                //HOperatorSet.ReduceDomain(ho_Red, ho_Rectangle, out ho_ImageReduced);

                //ho_Region.Dispose();
                //HOperatorSet.Threshold(ho_Red, out ho_Region, 0, 120);
                //ho_ConnectedRegions.Dispose();
                //HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);

                //ho_SelectedRegions.Dispose();
                //HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "height",
                //    "and", 50, 70);
                //ho_SelectedRegions1.Dispose();
                //HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions1, "width",
                //    "and", 0, 100);
                //ho_RegionUnion.Dispose();
                //HOperatorSet.Union1(ho_SelectedRegions1, out ho_RegionUnion);
                //HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row, out hv_Column,
                //    out hv_Phi, out hv_Length1, out hv_Length2);
                //ho_Rectangle.Dispose();
                //HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1+20,
                //    hv_Length2 + 20);
                //ho_ImageReducedSmall.Dispose();
                //HOperatorSet.ReduceDomain(ho_Red, ho_Rectangle, out ho_ImageReducedSmall);

                ho_ImageScaled1.Dispose();
                HOperatorSet.ScaleImageMax(ho_Red, out ho_ImageScaled1);
                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_Red, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_ImageScaled1, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_Green, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "min_identical_scanlines", 1);

                    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_max", 32);
                    HOperatorSet.FindBarCode(ho_Green, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_ImageScaled1, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }







                //    HOperatorSet.SmallestRectangle2(ho_SymbolRegions, out hv_Row, out hv_Column,
                //        out hv_Phi, out hv_Length1, out hv_Length2);

                //    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "timeout", 100);
                //    HOperatorSet.DecodeBarCodeRectangle2(ho_Red, hv_BarCodeHandle, "Code 128",
                //        hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2, out hv_Decoded);

                //    //HOperatorSet.GetBarCodeResult(hv_BarCodeHandle, 0, "status_id", out barres);
                //    FindSN_Module = hv_Decoded.ToString();
                //}
                FindSN_Module = hv_DecodedDataStrings.ToString();


            }
            catch (HalconException HDevExpDefaultException)
            {

            }
            finally
            {

                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
                ho_Image.Dispose();
                ho_ImageRotate.Dispose();
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_Region.Dispose();
                ho_SymbolRegions.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionUnion.Dispose();
                ho_ImageReducedSmall.Dispose();
                ho_ImageScaled1.Dispose();
            }

        }


        public void Find_ModuleSN(HObject ho_DstImage)
        {

            if (!bFindSN)
            {
                return;
            }
            HObject[] OTemp = new HObject[20];
            HObject ho_Image, ho_ImageRotate, ho_Red, ho_Green;
            HObject ho_Blue, ho_Rectangle, ho_ImageReduced, ho_Region;
            HObject ho_ImageReducedSmall, ho_ImageScaled1, ho_SymbolRegions;

            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionUnion, ho_SelectedRegions1;

            HTuple barres;
            // Local control variables 

            HTuple hv_WindowID, hv_Row, hv_Column, hv_Phi;
            HTuple hv_Length1, hv_Length2, hv_BarCodeHandle, hv_Decoded;
            HTuple hv_DecodedDataStrings, hv_Number;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedSmall);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.CreateBarCodeModel(new HTuple(), new HTuple(), out hv_BarCodeHandle);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "check_char", "absent");
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "num_scanlines", 10);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "min_identical_scanlines", 1);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_min", 7);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "element_size_max", 7);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "meas_thresh", 0.15);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "meas_thresh_abs", 5);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "barcode_height_min", 64);
            HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "timeout", 500);

            try
            {
                FindSN_Module = "";


                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                HOperatorSet.Decompose3(ho_DstImage, out ho_Red, out ho_Green, out ho_Blue
                    );
                ho_Green.Dispose();
                HOperatorSet.SmoothImage(ho_Red, out ho_Green, "gauss", 5);


                ho_ImageScaled1.Dispose();
                HOperatorSet.ScaleImageMax(ho_Red, out ho_ImageScaled1);
                HOperatorSet.DispObj(ho_ImageScaled1, HLeftWindow.HalconWindow);
                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_Red, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_ImageScaled1, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_Green, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "min_identical_scanlines", 1);

                    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "barcode_height_min", 64);
                    HOperatorSet.FindBarCode(ho_Green, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }
                HOperatorSet.CountObj(ho_SymbolRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(1))) == 0)
                {
                    HOperatorSet.FindBarCode(ho_ImageScaled1, out ho_SymbolRegions, hv_BarCodeHandle,
                  "Code 128", out hv_DecodedDataStrings);
                }


                //    HOperatorSet.SmallestRectangle2(ho_SymbolRegions, out hv_Row, out hv_Column,
                //        out hv_Phi, out hv_Length1, out hv_Length2);

                //    HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "timeout", 100);
                //    HOperatorSet.DecodeBarCodeRectangle2(ho_Red, hv_BarCodeHandle, "Code 128",
                //        hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2, out hv_Decoded);

                //    //HOperatorSet.GetBarCodeResult(hv_BarCodeHandle, 0, "status_id", out barres);
                //    FindSN_Module = hv_Decoded.ToString();
                //}
                FindSN_Module = hv_DecodedDataStrings.ToString();


            }
            catch (HalconException HDevExpDefaultException)
            {
                String i = HDevExpDefaultException.Message;
            }
            finally
            {

                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
                ho_Image.Dispose();
                ho_ImageRotate.Dispose();
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_Region.Dispose();
                ho_SymbolRegions.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionUnion.Dispose();
                ho_ImageReducedSmall.Dispose();
                ho_ImageScaled1.Dispose();
            }

        }


        #endregion
        #region 右侧判断
        private bool RightTplCreate(HObject ho_TplImage, ref HTuple hv_TemplateID_1, ref HTuple hv_TemplateID_2, ref HTuple hv_TemplateID_3, ref HTuple hv_TemplateID_4)
        {
            HObject ho_MODELS_Image, ho_ImageRotate, ho_Rectangle2, ho_ImageReduced;

            // Local control variables 

            HTuple hv_TplX, hv_TplY, hv_TplW, hv_TplH, hv_TplR;
            HTuple hv_ImageName = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_MODELS_Image);

            // Initialize local and output iconic variables 


            try
            {
                int RotAngle = -107;

                hv_ImageName = TplPath + "\\5.jpg";
                ho_MODELS_Image.Dispose();
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_MODELS_Image, out ho_ImageRotate, RotAngle, "constant");

                hv_TplY = 1100;
                hv_TplX = 1216;
                hv_TplW = 600;
                hv_TplH = 150;
                hv_TplR = 0.0;

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_TplY, hv_TplX, hv_TplR, hv_TplW,
                    hv_TplH);



                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_Rectangle2, out ho_ImageReduced);


                HOperatorSet.CreateTemplateRot(ho_ImageReduced, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_1);






                hv_ImageName = TplPath + "\\6.jpg";
                ho_MODELS_Image.Dispose();
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_MODELS_Image, out ho_ImageRotate, RotAngle, "constant");

                hv_TplY = 1100;
                hv_TplX = 1216;
                hv_TplW = 600;
                hv_TplH = 150;
                hv_TplR = 0.0;

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_TplY, hv_TplX, hv_TplR, hv_TplW,
                    hv_TplH);



                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_Rectangle2, out ho_ImageReduced);


                HOperatorSet.CreateTemplateRot(ho_ImageReduced, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_2);




                hv_ImageName = TplPath + "\\7.jpg";
                ho_MODELS_Image.Dispose();
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_MODELS_Image, out ho_ImageRotate, RotAngle, "constant");

                hv_TplY = 1100;
                hv_TplX = 1216;
                hv_TplW = 600;
                hv_TplH = 150;
                hv_TplR = 0.0;

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_TplY, hv_TplX, hv_TplR, hv_TplW,
                    hv_TplH);



                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_Rectangle2, out ho_ImageReduced);


                HOperatorSet.CreateTemplateRot(ho_ImageReduced, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_3);






                hv_ImageName = TplPath + "\\8.jpg";
                ho_MODELS_Image.Dispose();
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_MODELS_Image, out ho_ImageRotate, RotAngle, "constant");

                hv_TplY = 1100;
                hv_TplX = 1216;
                hv_TplW = 600;
                hv_TplH = 150;
                hv_TplR = 0.0;

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_TplY, hv_TplX, hv_TplR, hv_TplW,
                    hv_TplH);



                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_Rectangle2, out ho_ImageReduced);


                HOperatorSet.CreateTemplateRot(ho_ImageReduced, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_4);



                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                ho_ImageReduced.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageRotate.Dispose();
                ho_MODELS_Image.Dispose();
            }

        }

        public bool RightAction(HTuple hv_TemplateID_1, HTuple hv_TemplateID_2, HTuple hv_TemplateID_3, HTuple hv_TemplateID_4, HObject ho_MatchImage)
        {
            HSystem sys = new HSystem();
            HObject ho_ImageRotate, ho_Rectangle2;
            HObject ho_ImageReduced, ho_Rectanglehole1, ho_ImageDst = null;
            HObject ho_ImageDstRotate = null, ho_ImageBinomial = null, ho_Region = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionUnionhole2 = null, ho_RectangleSide1 = null;
            HObject ho_ImageReduced1 = null, ho_Region1 = null, ho_RegionFillUp = null;
            HObject ho_ConnectedRegions1 = null, ho_SelectedRegions1 = null;
            HObject ho_RegionClosing = null, ho_RegionClosing1 = null, ho_RegionDilation;

            HObject ho_Rectanglehole2, ho_Regionback = null, ho_Regionserosion = null;
            HObject ho_Regionsdilation = null, ho_SelectedRegions2 = null;
            // Local control variables 

            HTuple hv_WindowID, hv_TplY2, hv_TplX2, hv_TplW2, hv_HoleSelGray;
            HTuple hv_TplH2, hv_TplR2, hv_TplY, hv_TplX, hv_Area, hv_holex1;
            HTuple hv_holey1, hv_holeW1, hv_holeH1, hv_holeAgl1, hv_TemplateID;
            HTuple hv__Index_, hv_String1 = new HTuple(), hv_ImageScale = new HTuple();
            HTuple hv_defRotAngle = new HTuple(), hv_HoleSelAreaMin = new HTuple();
            HTuple hv_defWeldAreaErrorAngle = new HTuple(), hv_WeldAreaW = new HTuple();
            HTuple hv_WeldAreaH = new HTuple(), hv_WeldAreaYShifting = new HTuple();
            HTuple hv_WeldSelBlackGray = new HTuple(), hv_WeldSelBlackW = new HTuple();
            HTuple hv_WeldSelBlackH = new HTuple(), hv_RectangularityMin = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Error = new HTuple(), hv_xx = new HTuple(), hv_yy = new HTuple();
            HTuple hv_realholeX1 = new HTuple(), hv_realholeY1 = new HTuple();
            HTuple hv_realholeAgl1 = new HTuple(), hv_Mean = new HTuple();
            HTuple hv_Deviation = new HTuple(), hv_RowHole2 = new HTuple();
            HTuple hv_ColumnHole2 = new HTuple(), hv_PhiHole2 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Length22 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_LengthBlackH = new HTuple(), hv_LengthBlackW = new HTuple();
            HTuple hv_Rectangularity = new HTuple();
            HTuple hv_holey2, hv_holeW2, hv_holeH2, hv_holeAgl2, hv_holex2;
            HTuple hv_realholeX2 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_realholeY2 = new HTuple(), hv_realholeAgl2 = new HTuple();
            // Initialize local and output iconic variables 

            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole1);
            HOperatorSet.GenEmptyObj(out ho_ImageDst);
            HOperatorSet.GenEmptyObj(out ho_ImageDstRotate);
            HOperatorSet.GenEmptyObj(out ho_ImageBinomial);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionhole2);
            HOperatorSet.GenEmptyObj(out ho_RectangleSide1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole2);
            HOperatorSet.GenEmptyObj(out ho_Regionback);
            HOperatorSet.GenEmptyObj(out ho_Regionserosion);
            HOperatorSet.GenEmptyObj(out ho_Regionsdilation);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            try
            {
                //resWeldRight[CurIndex] = 0;
                hv_ImageScale = 0.015;
                hv_defRotAngle = -107;
                hv_HoleSelAreaMin = holeAreaSel;
                hv_defWeldAreaErrorAngle = 0.15;
                hv_WeldAreaW = weldWidthRight;
                hv_WeldAreaH = weldHeightRight;
                hv_WeldAreaYShifting = -264;
                hv_WeldSelBlackGray = weldGrayRight1;
                hv_WeldSelBlackW = weldSmallSelWidth;
                hv_WeldSelBlackH = weldSmallSelLength;
                hv_RectangularityMin = 0.5;







                ho_ImageDstRotate.Dispose();
                HOperatorSet.RotateImage(ho_MatchImage, out ho_ImageDstRotate, hv_defRotAngle,
                    "constant");
                ho_ImageBinomial.Dispose();
                HOperatorSet.BinomialFilter(ho_ImageDstRotate, out ho_ImageBinomial, 5, 5);
                hv_holex1 = 1490;
                hv_holey1 = 1030;
                hv_holeW1 = 320;
                hv_holeH1 = 40;
                hv_holeAgl1 = 0.0;
                hv_holex2 = 1490;
                hv_holey2 = 1030;
                hv_holeW2 = 320;
                hv_holeH2 = 40;
                hv_holeAgl2 = 0.0;
                hv_TplY = 1100;
                hv_TplX = 1216;
                hv_HoleSelGray = 10;
                if (CurIndex == 1)
                {

                    hv_holex1 = 1250;
                    hv_holey1 = 850;
                    hv_holeW1 = 320;
                    hv_holeH1 = 40;
                    hv_holeAgl1 = 0.0;
                    hv_holex2 = 1250;
                    hv_holey2 = 1000;
                    hv_holeW2 = 300;
                    hv_holeH2 = 130;
                    hv_holeAgl2 = 0.0;
                    hv_TplY = 1100;
                    hv_TplX = 1216;
                    hv_WeldSelBlackGray = weldGrayRight1;
                    hv_HoleSelGray = holeGrayLeft1;
                    HLeftWindow = HWindowR1;
                    HOperatorSet.BestMatchRotMg(ho_ImageDstRotate, hv_TemplateID_1, -0.1, 0.1, 50,
                    "false", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                }
                else if (CurIndex == 2)
                {
                    hv_holex1 = 1250;
                    hv_holey1 = 850;
                    hv_holeW1 = 320;
                    hv_holeH1 = 40;
                    hv_holeAgl1 = 0.0;
                    hv_holex2 = 1250;
                    hv_holey2 = 1000;
                    hv_holeW2 = 300;
                    hv_holeH2 = 130;
                    hv_holeAgl2 = 0.0;
                    hv_TplY = 1100;
                    hv_TplX = 1216;
                    hv_WeldSelBlackGray = weldGrayRight2;
                    hv_HoleSelGray = holeGrayLeft2;
                    HLeftWindow = HWindowR2;
                    HOperatorSet.BestMatchRotMg(ho_ImageDstRotate, hv_TemplateID_2, -0.1, 0.1, 50,
                    "false", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                }
                else if (CurIndex == 3)
                {
                    hv_holex1 = 1250;
                    hv_holey1 = 850;
                    hv_holeW1 = 320;
                    hv_holeH1 = 40;
                    hv_holeAgl1 = 0.0;
                    hv_holex2 = 1250;
                    hv_holey2 = 1000;
                    hv_holeW2 = 300;
                    hv_holeH2 = 130;
                    hv_holeAgl2 = 0.0;
                    hv_TplY = 1100;
                    hv_TplX = 1216;
                    hv_WeldSelBlackGray = weldGrayRight3;
                    hv_HoleSelGray = holeGrayLeft3;
                    HLeftWindow = HWindowR3;
                    HOperatorSet.BestMatchRotMg(ho_ImageDstRotate, hv_TemplateID_3, -0.1, 0.1, 50,
                    "false", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                }
                else if (CurIndex == 4)
                {
                    hv_holex1 = 1250;
                    hv_holey1 = 865;
                    hv_holeW1 = 320;
                    hv_holeH1 = 40;
                    hv_holeAgl1 = 0.0;
                    hv_holex2 = 1250;
                    hv_holey2 = 1015;
                    hv_holeW2 = 300;
                    hv_holeH2 = 130;
                    hv_holeAgl2 = 0.0;
                    hv_TplY = 1100;
                    hv_TplX = 1216;
                    hv_WeldSelBlackGray = weldGrayRight4;
                    hv_HoleSelGray = holeGrayLeft4;
                    HLeftWindow = HWindowR4;
                    HOperatorSet.BestMatchRotMg(ho_ImageDstRotate, hv_TemplateID_4, -0.1, 0.1, 50,
                    "false", 4, out hv_Row, out hv_Column, out hv_Angle, out hv_Error);
                }

                if (hv_Error > 50 || hv_Row <= 0)
                {
                    resWeldRight[CurIndex] = 2;
                }



                hv_xx = (hv_holex1 - hv_TplX) * (hv_Angle.TupleCos());
                hv_yy = (hv_holey1 - hv_TplY) * (hv_Angle.TupleSin());
                //创建匹配的识别位置1
                hv_realholeX1 = (hv_Column + ((hv_holex1 - hv_TplX) * (hv_Angle.TupleCos()))) - ((hv_holey1 - hv_TplY) * (hv_Angle.TupleSin()
                    ));
                hv_realholeY1 = (hv_Row - ((hv_holex1 - hv_TplX) * (hv_Angle.TupleSin()))) + ((hv_holey1 - hv_TplY) * (hv_Angle.TupleCos()
                    ));
                hv_realholeAgl1 = hv_Angle + hv_holeAgl1;
                HOperatorSet.DispObj(ho_ImageDstRotate, HLeftWindow.HalconWindow);
                ho_Rectanglehole1.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectanglehole1, hv_realholeY1, hv_realholeX1,
                    hv_realholeAgl1, hv_holeW1, hv_holeH1);

                HOperatorSet.SetColor(HLeftWindow.HalconWindow, "green");
                HOperatorSet.DispObj(ho_Rectanglehole1, HLeftWindow.HalconWindow);


                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageBinomial, ho_Rectanglehole1, out ho_ImageReduced
                    );
                HOperatorSet.Intensity(ho_Rectanglehole1, ho_ImageDstRotate, out hv_Mean, out hv_Deviation);
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, hv_HoleSelGray, hv_Mean - (hv_Deviation / 2));

                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);



                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", hv_HoleSelAreaMin, 99999);

                ho_RegionUnionhole2.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnionhole2);

                HOperatorSet.SmallestRectangle2(ho_RegionUnionhole2, out hv_RowHole2, out hv_ColumnHole2,
                    out hv_PhiHole2, out hv_Length21, out hv_Length22);


                ho_RectangleSide1.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectangleSide1, hv_RowHole2 + hv_WeldAreaYShifting,
                    (hv_ColumnHole2 + hv_Length21) - hv_WeldAreaW, hv_PhiHole2 + hv_defWeldAreaErrorAngle,
                    hv_WeldAreaW, hv_WeldAreaH);
                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageBinomial, ho_RectangleSide1, out ho_ImageReduced1
                    );
                HOperatorSet.SetColor(HLeftWindow.HalconWindow, "green");
                HOperatorSet.DispObj(ho_RectangleSide1, HLeftWindow.HalconWindow);

                ho_Region1.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, 0, hv_WeldSelBlackGray);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Region1, out ho_RegionFillUp);
                ho_RegionDilation.Dispose();
                HOperatorSet.DilationRectangle1(ho_RegionFillUp, out ho_RegionDilation, 5,
                    60);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionDilation, out ho_ConnectedRegions1);

                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1,
                    "max_area", 70);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_SelectedRegions1, out ho_RegionClosing, 30);

                HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                HOperatorSet.DispObj(ho_RegionClosing, HLeftWindow.HalconWindow);

                HOperatorSet.SmallestRectangle2(ho_RegionClosing, out hv_Row1, out hv_Column1,
                    out hv_Phi, out hv_LengthBlackH, out hv_LengthBlackW);
                if ((int)(new HTuple(hv_LengthBlackH.TupleGreater(hv_WeldSelBlackH))) != 0)
                {
                    //ng
                    resWeldRight[CurIndex] = 1;
                }
                if ((int)((new HTuple(hv_LengthBlackH.TupleGreater(hv_WeldSelBlackH))).TupleAnd(
                    new HTuple(hv_LengthBlackW.TupleGreater(hv_WeldSelBlackW)))) != 0)
                {
                    //ng
                    resWeldRight[CurIndex] = 2;
                }
                HOperatorSet.Rectangularity(ho_RegionClosing1, out hv_Rectangularity);
                if ((int)(new HTuple(hv_Rectangularity.TupleGreater(hv_RectangularityMin))) != 0)
                {
                    resWeldRight[CurIndex] = 1;
                }


                hv_realholeX2 = (hv_Column + ((hv_holex2 - hv_TplX) * (hv_Angle.TupleCos()))) - ((hv_holey2 - hv_TplY) * (hv_Angle.TupleSin()
          ));
                hv_realholeY2 = (hv_Row - ((hv_holex2 - hv_TplX) * (hv_Angle.TupleSin()))) + ((hv_holey2 - hv_TplY) * (hv_Angle.TupleCos()
                    ));
                hv_realholeAgl2 = hv_Angle + hv_holeAgl2;

                ho_Rectanglehole2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectanglehole2, hv_realholeY2, hv_realholeX2,
                    hv_realholeAgl2, hv_holeW2, hv_holeH2);

                HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                HOperatorSet.DispObj(ho_Rectanglehole2, HLeftWindow.HalconWindow);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageBinomial, ho_Rectanglehole2, out ho_ImageReduced
                    );

                ho_Regionback.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Regionback, hv_Mean,
                    255);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_Regionback, out ho_ConnectedRegions1);
                ho_Regionserosion.Dispose();
                HOperatorSet.ErosionRectangle1(ho_ConnectedRegions1, out ho_Regionserosion,
                    150, 30);

                ho_Regionsdilation.Dispose();
                HOperatorSet.DilationRectangle1(ho_Regionserosion, out ho_Regionsdilation,
                    100, 30);

                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShape(ho_Regionsdilation, out ho_SelectedRegions2, (new HTuple("height")).TupleConcat(
                    "width"), "and", (new HTuple(130)).TupleConcat(200), (new HTuple(99999)).TupleConcat(
                    99999));
                HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                {
                    HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_SelectedRegions2, HLeftWindow.HalconWindow);

                    resWeldLeft[CurIndex] = 2;
                }








                return true;
            }
            catch (Exception e)
            {
                resWeldRight[CurIndex] = 2;
                return false;
            }
            finally
            {

                if (resWeldRight[CurIndex] == 1)
                {
                    HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                    HOperatorSet.SetTposition(HLeftWindow.HalconWindow, 50, 300);
                    HOperatorSet.WriteString(HLeftWindow.HalconWindow, "NG1");

                }
                else if (resWeldRight[CurIndex] == 2)
                {
                    HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                    HOperatorSet.SetTposition(HLeftWindow.HalconWindow, 50, 300);
                    HOperatorSet.WriteString(HLeftWindow.HalconWindow, "NG2");

                }
                string PATH = "E:\\L" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                HOperatorSet.DumpWindow(HLeftWindow.HalconWindow, "jpeg", PATH);


                ho_ImageRotate.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageReduced.Dispose();
                ho_Rectanglehole1.Dispose();
                ho_ImageDst.Dispose();
                ho_ImageDstRotate.Dispose();
                ho_ImageBinomial.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnionhole2.Dispose();
                ho_RectangleSide1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionDilation.Dispose();
                ho_Rectanglehole2.Dispose();
                ho_Regionback.Dispose();
                ho_Regionserosion.Dispose();
                ho_Regionsdilation.Dispose();
                ho_SelectedRegions2.Dispose();
            }
        }
        #endregion
        #region 右侧判断2
        private bool RightTplCreate2(HObject ho_TplImage, ref HTuple hv_TemplateID_1, ref HTuple hv_TemplateID_2)
        {
            HObject ho_ImageFilled, ho_Red, ho_Green, ho_Blue;
            HObject ho_Rectangle11, ho_Rectangle12, ho_RegionUnion1;
            HObject ho_ImageReduced1, ho_Rectangle21, ho_Rectangle22;
            HObject ho_RegionUnion2, ho_ImageReduced2, ho_Rectangle31;
            HObject ho_Rectangle32, ho_RegionUnion3, ho_ImageReduced3;
            HObject ho_Rectangle41, ho_Rectangle42, ho_RegionUnion4;
            HObject ho_ImageReduced4, ho_Rectanglehole1, ho_Rectanglehole2;
            HObject ho_Rectanglehole3, ho_Rectanglehole4, ho_ImageAffinTrans;
            HObject ho_Rectangledef1, ho_Rectangledef2, ho_Rectangledef3;
            HObject ho_Rectangledef4;


            // Local control variables 

            HTuple hv_TplX, hv_TplY, hv_Width, hv_Height;
            HTuple hv_TplW11, hv_TplH11, hv_TplX11, hv_TplY11, hv_TplW12;
            HTuple hv_TplH12, hv_TplX12, hv_TplY12, hv_Area, hv_TplYA;
            HTuple hv_TplXA, hv_TplW21, hv_TplH21, hv_TplX21, hv_TplY21;
            HTuple hv_TplW22, hv_TplH22, hv_TplX22, hv_TplY22, hv_TplYB;
            HTuple hv_TplXB;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageFilled);
            HOperatorSet.GenEmptyObj(out ho_Red);
            HOperatorSet.GenEmptyObj(out ho_Green);
            HOperatorSet.GenEmptyObj(out ho_Blue);
            HOperatorSet.GenEmptyObj(out ho_Rectangle11);
            HOperatorSet.GenEmptyObj(out ho_Rectangle12);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle21);
            HOperatorSet.GenEmptyObj(out ho_Rectangle22);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle31);
            HOperatorSet.GenEmptyObj(out ho_Rectangle32);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle41);
            HOperatorSet.GenEmptyObj(out ho_Rectangle42);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion4);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced4);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole1);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole2);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole3);
            HOperatorSet.GenEmptyObj(out ho_Rectanglehole4);
            HOperatorSet.GenEmptyObj(out ho_ImageAffinTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangledef1);
            HOperatorSet.GenEmptyObj(out ho_Rectangledef2);
            HOperatorSet.GenEmptyObj(out ho_Rectangledef3);
            HOperatorSet.GenEmptyObj(out ho_Rectangledef4);


            try
            {

                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                HOperatorSet.Decompose3(ho_TplImage, out ho_Red, out ho_Green, out ho_Blue
                    );
                //dev_display (Red)
                //dev_display (Green)
                //dev_display (Blue)
                HOperatorSet.GetImageSize(ho_TplImage, out hv_Width, out hv_Height);


                hv_TplX = 650;
                hv_TplY = 1400;
                hv_TplW11 = 200;
                hv_TplH11 = 150;
                hv_TplX11 = 473;
                hv_TplY11 = 583;
                hv_TplW12 = 260;
                hv_TplH12 = 190;
                hv_TplX12 = 473;
                hv_TplY12 = 1560;
                //gen_rectangle1 (Rectangle, 350, 320, 654, 1322)
                //HOperatorSet.DispObj(ho_Blue, hv_ExpDefaultWinHandle);
                ho_Rectangle11.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle11, hv_TplY11, hv_TplX11, 0, hv_TplW11,
                    hv_TplH11);
                ho_Rectangle12.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle12, hv_TplY12, hv_TplX12, 0, hv_TplW12,
                    hv_TplH12);
                ho_RegionUnion1.Dispose();
                HOperatorSet.Union2(ho_Rectangle11, ho_Rectangle12, out ho_RegionUnion1);
                HOperatorSet.AreaCenter(ho_RegionUnion1, out hv_Area, out hv_TplYA, out hv_TplXA);
                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_Red, ho_RegionUnion1, out ho_ImageReduced1);
                //smallest_rectangle2 (RegionUnion, TplY, TplX, PhiHole1, Length1, Length2)


                hv_TplW21 = 150;
                hv_TplH21 = 150;
                hv_TplX21 = 1050;
                hv_TplY21 = 580;
                hv_TplW22 = 260;
                hv_TplH22 = 175;
                hv_TplX22 = 1050;
                hv_TplY22 = 1500;
                //gen_rectangle1 (Rectangle, 350, 320, 654, 1322)
                //HOperatorSet.DispObj(ho_Blue, hv_ExpDefaultWinHandle);
                ho_Rectangle21.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle21, hv_TplY21, hv_TplX21, 0, hv_TplW21,
                    hv_TplH21);
                ho_Rectangle22.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle22, hv_TplY22, hv_TplX22, 0, hv_TplW22,
                    hv_TplH22);
                ho_RegionUnion2.Dispose();
                HOperatorSet.Union2(ho_Rectangle21, ho_Rectangle22, out ho_RegionUnion2);
                HOperatorSet.AreaCenter(ho_RegionUnion2, out hv_Area, out hv_TplYB, out hv_TplXB);
                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain(ho_Red, ho_RegionUnion2, out ho_ImageReduced2);






                //Preparing a pattern for template matching with rotation
                HOperatorSet.CreateTemplateRot(ho_ImageReduced1, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_1);
                HOperatorSet.CreateTemplateRot(ho_ImageReduced2, 4, -0.1, 0.1, 0.02, "sort",
                    "original", out hv_TemplateID_2);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                ho_ImageFilled.Dispose();
                ho_Red.Dispose();
                ho_Green.Dispose();
                ho_Blue.Dispose();
                ho_Rectangle11.Dispose();
                ho_Rectangle12.Dispose();
                ho_RegionUnion1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Rectangle21.Dispose();
                ho_Rectangle22.Dispose();
                ho_RegionUnion2.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Rectangle31.Dispose();
                ho_Rectangle32.Dispose();
                ho_RegionUnion3.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Rectangle41.Dispose();
                ho_Rectangle42.Dispose();
                ho_RegionUnion4.Dispose();
                ho_ImageReduced4.Dispose();
                ho_Rectanglehole1.Dispose();
                ho_Rectanglehole2.Dispose();
                ho_Rectanglehole3.Dispose();
                ho_Rectanglehole4.Dispose();
                ho_ImageAffinTrans.Dispose();
                ho_Rectangledef1.Dispose();
                ho_Rectangledef2.Dispose();
                ho_Rectangledef3.Dispose();
                ho_Rectangledef4.Dispose();
            }
        }

        //private bool RightAction2(HTuple hv_TemplateID_1, HTuple hv_TemplateID_2,  HObject ho_MatchImage)
        //{
        //    HSystem sys = new HSystem();

        //    HObject[] OTemp = new HObject[20];
        //    long SP_O = 0;
        //    HObject ho_Red, ho_Green, ho_Blue;
        //    HObject ho_Rectangle11, ho_Rectangle12, ho_RegionUnion1;
        //    HObject ho_ImageReduced1, ho_Rectangle21, ho_Rectangle22;
        //    HObject ho_RegionUnion2, ho_ImageReduced2, ho_Rectanglehole1;
        //    HObject ho_Rectanglehole2, ho_ImageAffinTrans, ho_RectangleMatch;
        //    HObject ho_ImageReduced, ho_Rectangledef1, ho_Rectangledef2;
        //    HObject ho_ImageReducedhole1, ho_Regionhole1, ho_RegionErosion1;
        //    HObject ho_RegionDilation, ho_RegionFillUp, ho_ConnectedRegions;
        //    HObject ho_SelectedRegions, ho_RegionUnionhole1, ho_RectangleSide1 = null;
        //    HObject ho_Rectangleback1 = null, ho_ImageReducedSide1, ho_RegionSide1;
        //    HObject ho_ImageReducedhole2, ho_Regionhole2, ho_RegionErosion2;
        //    HObject ho_RegionUnionhole2, ho_RectangleSide2 = null, ho_Rectangleback2 = null;
        //    HObject ho_ImageReducedSide2, ho_RegionSide2, ho_ImageReducedBack1;
        //    HObject ho_ImageReducedBack2, ho_RegionBack1, ho_RegionBack2;
        //    HObject ho_RegionSideFill1, ho_RegionDifference1, ho_SelectedRegionsBlack1;
        //    HObject ho_RegionUnionholeBlack1, ho_RegionDifference2;
        //    HObject ho_SelectedRegionsBlack2, ho_RegionUnionholeBlack2;

        //    HObject ho_SelectedRegionsBlackH1, ho_SelectedRegionsBlackW1, ho_SelectedRegionsBlackH2, ho_SelectedRegionsBlackW2;
        //    HObject ho_RegionSideFill2;

        //    HObject ho_RectangleDown1;
        //    HObject ho_ImaAmp, ho_ImaDir, ho_RegionColor, ho_RectangleDown2;


        //    HTuple hv_RowDown1, hv_ColumnDown1, hv_Row21, hv_Column21;
        //    HTuple hv_RowDown2, hv_ColumnDown2;

        //    // Local control variables 

        //    HTuple hv_TplX, hv_TplY, hv_Width;
        //    HTuple hv_Height, hv_TplW11, hv_TplH11, hv_TplX11, hv_TplY11;
        //    HTuple hv_TplW12, hv_TplH12, hv_TplX12, hv_TplY12, hv_Area;
        //    HTuple hv_TplYA, hv_TplXA, hv_PhiHole1, hv_Length1, hv_Length2;
        //    HTuple hv_TplW21, hv_TplH21, hv_TplX21, hv_TplY21, hv_TplW22;
        //    HTuple hv_TplH22, hv_TplX22, hv_TplY22, hv_TplYB, hv_TplXB;
        //    HTuple hv_holex1, hv_holey1, hv_holeAg, hv_holex2, hv_holey2;
        //    HTuple hv_pi;
        //    HTuple hv_Row11, hv_Column11, hv_Angle11, hv_Error, hv_Row22;
        //    HTuple hv_Column22, hv_Angle22, hv_realholeX1, hv_realholeY1;
        //    HTuple hv_realholeAgl1, hv_realholeX2, hv_realholeY2, hv_realholeAgl2;
        //    HTuple hv_FindHoleArea, hv_i = new HTuple(), hv_Row1, hv_Column1, hv_Row2;
        //    HTuple hv_Column2, hv_RowHole1, hv_ColumnHole1, hv_RowHole2;
        //    HTuple hv_ColumnHole2, hv_PhiHole2, hv_Length21, hv_Length22;
        //    HTuple hv_SideArea2, hv_SideY, hv_SideX, hv_SideArea1;
        //    HTuple hv_FullSideArea1, hv_FullSideArea2, hv_RowBlack1;
        //    HTuple hv_ColumnBlack1, hv_PhiBlack1, hv_LengthBlack11;
        //    HTuple hv_LengthBlack12, hv_RowBlack2, hv_ColumnBlack2;
        //    HTuple hv_PhiBlack2, hv_LengthBlack21, hv_LengthBlack22;
        //    HTuple hv_Area1, hv_Row, hv_Column;
        //    HTuple RealHoleW1, RealHoleW2, hv_BackArea2, hv_BackArea1, hv_FullBackArea1, hv_FullSideY1, hv_FullBackArea2, hv_FullSideY2;

        //    // Initialize local and output iconic variables 

        //    HOperatorSet.GenEmptyObj(out ho_Red);
        //    HOperatorSet.GenEmptyObj(out ho_Green);
        //    HOperatorSet.GenEmptyObj(out ho_Blue);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle11);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle12);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle21);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle22);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
        //    HOperatorSet.GenEmptyObj(out ho_Rectanglehole1);
        //    HOperatorSet.GenEmptyObj(out ho_Rectanglehole2);
        //    HOperatorSet.GenEmptyObj(out ho_ImageAffinTrans);
        //    HOperatorSet.GenEmptyObj(out ho_RectangleMatch);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReduced);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangledef1);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangledef2);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedhole1);
        //    HOperatorSet.GenEmptyObj(out ho_Regionhole1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionErosion1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionDilation);
        //    HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
        //    HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnionhole1);
        //    HOperatorSet.GenEmptyObj(out ho_RectangleSide1);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangleback1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedSide1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionSide1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedhole2);
        //    HOperatorSet.GenEmptyObj(out ho_Regionhole2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionErosion2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnionhole2);
        //    HOperatorSet.GenEmptyObj(out ho_RectangleSide2);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangleback2);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedSide2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionSide2);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedBack1);
        //    HOperatorSet.GenEmptyObj(out ho_ImageReducedBack2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionBack1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionBack2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionSideFill1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlack1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnionholeBlack1);
        //    HOperatorSet.GenEmptyObj(out ho_RegionDifference2);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlack2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionUnionholeBlack2);

        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlackH1);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlackH2);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlackW1);
        //    HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlackW2);
        //    HOperatorSet.GenEmptyObj(out ho_RegionSideFill2);



        //    HOperatorSet.GenEmptyObj(out ho_RectangleDown1);
        //    HOperatorSet.GenEmptyObj(out ho_ImaAmp);
        //    HOperatorSet.GenEmptyObj(out ho_ImaDir);
        //    HOperatorSet.GenEmptyObj(out ho_RegionColor);
        //    HOperatorSet.GenEmptyObj(out ho_RectangleDown2);


        //    try
        //    {
        //        ho_Red.Dispose();
        //        ho_Green.Dispose();
        //        ho_Blue.Dispose();

        //        bool HoleFull1 = false;
        //        bool HoleFull2 = false;
        //        bool HoleFull3 = false;
        //        bool HoleFull4 = false;

        //        hv_TplX = 650;
        //        hv_TplY = 1400;
        //        hv_TplW11 = 200;
        //        hv_TplH11 = 150;
        //        hv_TplX11 = 473;
        //        hv_TplY11 = 583;
        //        hv_TplW12 = 260;
        //        hv_TplH12 = 190;
        //        hv_TplX12 = 473;
        //        hv_TplY12 = 1560;
        //        //gen_rectangle1 (Rectangle, 350, 320, 654, 1322)
        //        //HOperatorSet.DispObj(ho_Blue, hv_ExpDefaultWinHandle);
        //        ho_Rectangle11.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangle11, hv_TplY11, hv_TplX11, 0, hv_TplW11,
        //            hv_TplH11);
        //        ho_Rectangle12.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangle12, hv_TplY12, hv_TplX12, 0, hv_TplW12,
        //            hv_TplH12);
        //        ho_RegionUnion1.Dispose();
        //        HOperatorSet.Union2(ho_Rectangle11, ho_Rectangle12, out ho_RegionUnion1);
        //        HOperatorSet.AreaCenter(ho_RegionUnion1, out hv_Area, out hv_TplYA, out hv_TplXA);
        //        ho_ImageReduced1.Dispose();
        //        //HOperatorSet.ReduceDomain(ho_Red, ho_RegionUnion1, out ho_ImageReduced1);
        //        //smallest_rectangle2 (RegionUnion, TplY, TplX, PhiHole1, Length1, Length2)


        //        hv_TplW21 = 150;
        //        hv_TplH21 = 150;
        //        hv_TplX21 = 1050;
        //        hv_TplY21 = 580;
        //        hv_TplW22 = 260;
        //        hv_TplH22 = 175;
        //        hv_TplX22 = 1050;
        //        hv_TplY22 = 1500;
        //        //gen_rectangle1 (Rectangle, 350, 320, 654, 1322)
        //        //HOperatorSet.DispObj(ho_Blue, hv_ExpDefaultWinHandle);
        //        ho_Rectangle21.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangle21, hv_TplY21, hv_TplX21, 0, hv_TplW21,
        //            hv_TplH21);
        //        ho_Rectangle22.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangle22, hv_TplY22, hv_TplX22, 0, hv_TplW22,
        //            hv_TplH22);
        //        ho_RegionUnion2.Dispose();
        //        HOperatorSet.Union2(ho_Rectangle21, ho_Rectangle22, out ho_RegionUnion2);
        //        HOperatorSet.AreaCenter(ho_RegionUnion2, out hv_Area, out hv_TplYB, out hv_TplXB);
        //        ho_ImageReduced2.Dispose();
        //        //HOperatorSet.ReduceDomain(ho_Green, ho_RegionUnion2, out ho_ImageReduced2);










        //        //创建匹配的识别位置
        //        hv_holex1 = 473;
        //        hv_holey1 = 1185;
        //        hv_holeAg = -0.02;
        //        //HOperatorSet.DispObj(ho_Green, hv_ExpDefaultWinHandle);
        //        //ho_Rectanglehole1.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_Rectanglehole1, hv_holey1, hv_holex1, hv_holeAg,
        //        //    110, 8);
        //        hv_holex2 = 1050;
        //        hv_holey2 = 1200;
        //        //HOperatorSet.DispObj(ho_ImageFilled, hv_ExpDefaultWinHandle);
        //        //ho_Rectanglehole2.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_Rectanglehole2, hv_holey2, hv_holex2, hv_holeAg,
        //        //    62, 8);




        //        //read_image (ImageAffinTrans, 'H:/1217/t2015年12月17日15330.png')
        //        ho_Red.Dispose();
        //        ho_Green.Dispose();
        //        ho_Blue.Dispose();
        //        HOperatorSet.Decompose3(ho_MatchImage, out ho_Red, out ho_Green, out ho_Blue
        //            );
        //        HOperatorSet.BestMatchRotMg(ho_Red, hv_TemplateID_1, -0.1, 0.1, 80, "false",
        //4, out hv_Row11, out hv_Column11, out hv_Angle11, out hv_Error);
        //        HOperatorSet.BestMatchRotMg(ho_Red, hv_TemplateID_2, -0.1, 0.1, 80, "false",
        //            4, out hv_Row22, out hv_Column22, out hv_Angle22, out hv_Error);




        //        //创建匹配的识别位置1
        //        hv_realholeX1 = (hv_Column11 + ((hv_holex1 - hv_TplXA) * (hv_Angle11.TupleCos()))) - ((hv_holey1 - hv_TplYA) * (hv_Angle11.TupleSin()
        //            ));
        //        hv_realholeY1 = (hv_Row11 - ((hv_holex1 - hv_TplXA) * (hv_Angle11.TupleSin()))) + ((hv_holey1 - hv_TplYA) * (hv_Angle11.TupleCos()
        //            ));
        //        hv_realholeAgl1 = hv_Angle11 + hv_holeAg;
        //        // HOperatorSet.DispObj(ho_ImageAffinTrans, hv_ExpDefaultWinHandle);
        //        int RealWeldHeight1 = 70 + weldHeightRight;
        //        int RealWeldHeight2 = 65 + weldHeightRight;


        //        RealHoleW1 = 120 + holeWidthLeft;
        //        ho_Rectanglehole1.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectanglehole1, hv_realholeY1, hv_realholeX1,
        //            hv_realholeAgl1, RealHoleW1, 14 + holeHeightLeft);
        //        ho_Rectangledef1.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangledef1, hv_realholeY1 - 105, hv_realholeX1 - 55,
        //            hv_realholeAgl1, 130, RealWeldHeight1);

        //        hv_realholeX2 = (hv_Column22 + ((hv_holex2 - hv_TplXB) * (hv_Angle22.TupleCos()))) - ((hv_holey2 - hv_TplYB) * (hv_Angle22.TupleSin()
        //            ));
        //        hv_realholeY2 = (hv_Row22 - ((hv_holex2 - hv_TplXB) * (hv_Angle22.TupleSin()))) + ((hv_holey2 - hv_TplYB) * (hv_Angle22.TupleCos()
        //            ));
        //        hv_realholeAgl2 = hv_Angle22 + hv_holeAg;

        //        RealHoleW2 = 115 + holeWidthLeft;
        //        ho_Rectanglehole2.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectanglehole2, hv_realholeY2, hv_realholeX2,
        //            hv_realholeAgl2, RealHoleW2, 14 + holeHeightLeft);
        //        ho_Rectangledef2.Dispose();
        //        HOperatorSet.GenRectangle2(out ho_Rectangledef2, hv_realholeY2 - 100, hv_realholeX2 - 60,
        //            hv_realholeAgl2, 110, RealWeldHeight2);


        //        //ho_RectangleDown1.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_RectangleDown1, hv_realholeY1 - 230, hv_realholeX1,
        //        //    hv_realholeAgl1, 200, 20);
        //        //ho_ImageReduced.Dispose();
        //        //HOperatorSet.ReduceDomain(ho_Red, ho_RectangleDown1, out ho_ImageReduced);

        //        //ho_ImaAmp.Dispose();
        //        //ho_ImaDir.Dispose();
        //        //HOperatorSet.EdgesImage(ho_ImageReduced, out ho_ImaAmp, out ho_ImaDir, "canny",
        //        //    0.5, "nms", 10, 20);
        //        //ho_RegionColor.Dispose();
        //        //HOperatorSet.Threshold(ho_ImaAmp, out ho_RegionColor, 1, 255);
        //        //HOperatorSet.SmallestRectangle1(ho_RegionColor, out hv_RowDown1, out hv_ColumnDown1,
        //        //    out hv_Row21, out hv_Column21);
        //        //ho_Rectangledef1.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_Rectangledef1, hv_RowDown1 +140, hv_ColumnDown1 ,
        //        //    hv_realholeAgl1, 35, 65);

        //        //ho_RectangleDown2.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_RectangleDown2, hv_realholeY2 - 220, hv_realholeX2,
        //        //    hv_realholeAgl2, 200, 20);
        //        //ho_ImageReduced.Dispose();
        //        //HOperatorSet.ReduceDomain(ho_Red, ho_RectangleDown2, out ho_ImageReduced);

        //        //ho_ImaAmp.Dispose();
        //        //ho_ImaDir.Dispose();
        //        //HOperatorSet.EdgesImage(ho_ImageReduced, out ho_ImaAmp, out ho_ImaDir, "canny",
        //        //    0.5, "nms", 10, 20);
        //        //ho_RegionColor.Dispose();
        //        //HOperatorSet.Threshold(ho_ImaAmp, out ho_RegionColor, 1, 255);
        //        //HOperatorSet.SmallestRectangle1(ho_RegionColor, out hv_RowDown2, out hv_ColumnDown2,
        //        //    out hv_Row21, out hv_Column21);
        //        //ho_Rectangledef2.Dispose();
        //        //HOperatorSet.GenRectangle2(out ho_Rectangledef2, hv_RowDown2 + 130, hv_ColumnDown2 ,
        //        //    hv_realholeAgl2, 35, 60);




        //        HOperatorSet.SetColor(HPosWindow.HalconWindow, "red");

        //        HOperatorSet.DispObj(ho_Rectanglehole2, HRightWindow2.HalconWindow);
        //        HOperatorSet.DispObj(ho_Rectanglehole1, HRightWindow2.HalconWindow);
        //        ho_ImageReducedhole1.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_Rectanglehole1, out ho_ImageReducedhole1
        //            );
        //        ho_Regionhole1.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedhole1, out ho_Regionhole1, holeGrayLeft3, 255);
        //        ho_RegionErosion1.Dispose();
        //        HOperatorSet.ErosionRectangle1(ho_Regionhole1, out ho_RegionErosion1, 4, 4);
        //        ho_RegionDilation.Dispose();
        //        HOperatorSet.DilationRectangle1(ho_RegionErosion1, out ho_RegionDilation, 4,
        //            4);
        //        ho_RegionFillUp.Dispose();
        //        HOperatorSet.FillUp(ho_RegionDilation, out ho_RegionFillUp);
        //        ho_ConnectedRegions.Dispose();
        //        HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
        //        //dev_display (ImageAffinTrans)
        //        ho_SelectedRegions.Dispose();
        //        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
        //            "and", holeAreaSel, 99999);
        //        HOperatorSet.CountObj(ho_SelectedRegions, out hv_FindHoleArea);
        //        if ((int)(hv_FindHoleArea) > 0)
        //        {
        //            ho_RegionUnionhole1.Dispose();
        //            HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnionhole1);
        //            HOperatorSet.InnerRectangle1(ho_RegionUnionhole1, out hv_Row1, out hv_Column1,
        //                out hv_Row2, out hv_Column2);
        //            HOperatorSet.SmallestRectangle2(ho_RegionUnionhole1, out hv_RowHole1, out hv_ColumnHole1,
        //                out hv_PhiHole1, out hv_Length1, out hv_Length2);



        //            //过小那么就判定焊带有问题
        //            //dev_display (ImageAffinTrans)
        //            if (hv_Length1 >= (RealHoleW1 - 2))
        //            {
        //                HoleFull1 = true;
        //                HOperatorSet.SetColor(HRightWindow2.HalconWindow, "green");
        //                HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY1 - 55, hv_realholeX1);
        //                HOperatorSet.WriteString(HRightWindow2.HalconWindow, "满焊");
        //                ho_Rectangleback1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_realholeY1 - 145, hv_realholeX1,
        //                    hv_realholeAgl1, 120, 35);
        //            }
        //            else if (holeWidthSel > hv_Length1)
        //            {
        //                ho_RectangleSide1.Dispose();
        //                ho_RectangleSide1 = ho_Rectangledef1;
        //                ho_Rectangleback1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_realholeY1 - 145, hv_realholeX1,
        //                    hv_realholeAgl1, 120, 35);
        //            }
        //            else if ((int)((new HTuple(hv_ColumnHole1.TupleGreater(hv_realholeX1))).TupleOr(new HTuple(((hv_ColumnHole1 - hv_Length1)).TupleGreater(
        //                hv_realholeX1 - (RealHoleW1 - 1))))) != 0)
        //            {
        //                //边缘在左边
        //                ho_RectangleSide1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide1, hv_RowHole1 - 110, hv_ColumnHole1 - hv_Length1 + 5,
        //                    hv_realholeAgl1, 25 + weldWidthRight, RealWeldHeight1);
        //                ho_Rectangleback1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_RowHole1 - 145, (hv_ColumnHole1 - hv_Length1 + 70),
        //                    hv_realholeAgl1, 120, 35);
        //            }
        //            else if ((int)(new HTuple(hv_ColumnHole1.TupleLess(hv_realholeX2))) != 0)
        //            {
        //                ho_RectangleSide1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide1, hv_RowHole1 - 110, (hv_ColumnHole1 + hv_Length1) - 220,
        //                    hv_realholeAgl1, 25 + weldWidthRight, RealWeldHeight1);
        //                ho_Rectangleback1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_RowHole1 - 145, (hv_ColumnHole1 + hv_Length1) - 75,
        //                    hv_realholeAgl1, 120, 35);
        //            }
        //            else
        //            {
        //                ho_RectangleSide1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide1, hv_RowHole1 - 110, hv_ColumnHole1 - 110,
        //                    hv_PhiHole1, 25 + weldWidthRight, RealWeldHeight1);
        //                ho_Rectangleback1.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_RowHole1 - 145, (hv_ColumnHole1 + hv_Length1) - 75,
        //                    hv_PhiHole1, 120, 35);
        //            }
        //        }
        //        else
        //        {
        //            ho_RectangleSide1.Dispose();
        //            ho_RectangleSide1 = ho_Rectangledef1;
        //            ho_Rectangleback1.Dispose();
        //            HOperatorSet.GenRectangle2(out ho_Rectangleback1, hv_realholeY1 - 145, hv_realholeX1,
        //                hv_realholeAgl1, 120, 35);
        //        }
        //        //找两个边缘
        //        ho_ImageReducedSide1.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_RectangleSide1, out ho_ImageReducedSide1
        //            );
        //        //dev_display (ImageAffinTrans)
        //        //threshold (ImageReducedSide1, RegionSide1, 120, 255)

        //        //创建匹配的识别位置2

        //        //dev_display (ImageAffinTrans)

        //        ho_ImageReducedhole2.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_Rectanglehole2, out ho_ImageReducedhole2
        //            );
        //        ho_Regionhole2.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedhole2, out ho_Regionhole2, holeGrayLeft4, 255);
        //        ho_RegionErosion2.Dispose();
        //        HOperatorSet.ErosionRectangle1(ho_Regionhole2, out ho_RegionErosion2, 4, 4);
        //        ho_RegionDilation.Dispose();
        //        HOperatorSet.DilationRectangle1(ho_RegionErosion2, out ho_RegionDilation, 4,
        //            4);
        //        ho_RegionFillUp.Dispose();
        //        HOperatorSet.FillUp(ho_RegionDilation, out ho_RegionFillUp);
        //        ho_ConnectedRegions.Dispose();
        //        HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
        //        //dev_display (ImageAffinTrans)
        //        ho_SelectedRegions.Dispose();
        //        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
        //            "and", holeAreaSel, 99999);
        //        HOperatorSet.CountObj(ho_SelectedRegions, out hv_FindHoleArea);
        //        if ((int)(hv_FindHoleArea) > 0)
        //        {
        //            ho_RegionUnionhole2.Dispose();
        //            HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnionhole2);

        //            HOperatorSet.SmallestRectangle2(ho_RegionUnionhole2, out hv_RowHole2, out hv_ColumnHole2,
        //                out hv_PhiHole2, out hv_Length21, out hv_Length22);
        //            //dev_display (ImageAffinTrans)
        //            if (hv_Length21 >= (RealHoleW2 - 2))
        //            {
        //                HoleFull2 = true;
        //                HOperatorSet.SetColor(HRightWindow2.HalconWindow, "green");
        //                HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY2 - 55, hv_realholeX2);
        //                HOperatorSet.WriteString(HRightWindow2.HalconWindow, "满焊");
        //                ho_Rectangleback2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_realholeY2 - 145, hv_realholeX2-30,
        //                    hv_realholeAgl2, 110, 35);
        //            }
        //            else if (holeWidthSel > hv_Length21)
        //            {
        //                ho_RectangleSide2.Dispose();
        //                ho_RectangleSide2 = ho_Rectangledef2;
        //                ho_Rectangleback2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_realholeY2 - 140, hv_realholeX2 - 30,
        //                    hv_realholeAgl2, 110, 35);
        //            }
        //            else if ((int)((new HTuple(hv_ColumnHole2.TupleGreater(hv_realholeX2))).TupleOr(new HTuple(((hv_ColumnHole2 - hv_Length21)).TupleGreater(
        //    hv_realholeX2 - (RealHoleW2 - 1))))) != 0)
        //            {
        //                //边缘在左边
        //                ho_RectangleSide2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide2, hv_RowHole2 - 100, hv_ColumnHole2 - hv_Length21 + 5,
        //                    hv_realholeAgl2, 25 + weldWidthRight, RealWeldHeight2);
        //                ho_Rectangleback2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_RowHole2 - 145, (hv_ColumnHole2 - hv_Length21 + 80),
        //                    hv_realholeAgl2, 110, 35);
        //            }
        //            else if ((int)(new HTuple(hv_ColumnHole2.TupleLess(hv_realholeX2))) != 0)
        //            {
        //                ho_RectangleSide2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide2, hv_RowHole2 - 100, (hv_ColumnHole2 + hv_Length21) - 190,
        //                    hv_realholeAgl2, 25 + weldWidthRight, RealWeldHeight2);
        //                ho_Rectangleback2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_RowHole2 - 145, (hv_ColumnHole2 + hv_Length21) - 110,
        //                    hv_realholeAgl2, 110, 35);
        //            }
        //            else
        //            {
        //                ho_RectangleSide2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_RectangleSide2, hv_RowHole2 - 100, hv_ColumnHole2 - 80,
        //                    hv_realholeAgl2, 25 + weldWidthRight, RealWeldHeight2);
        //                ho_Rectangleback2.Dispose();
        //                HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_RowHole2 - 145, (hv_ColumnHole2) - 80,
        //                    hv_realholeAgl2, 110, 35);
        //            }
        //        }
        //        else
        //        {
        //            ho_RectangleSide2.Dispose();
        //            ho_RectangleSide2 = ho_Rectangledef2;
        //            ho_Rectangleback2.Dispose();
        //            HOperatorSet.GenRectangle2(out ho_Rectangleback2, hv_realholeY2 - 140, hv_realholeX2,
        //                hv_realholeAgl2, 110, 35);
        //        }
        //        //找两个边缘
        //        ho_ImageReducedSide2.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_RectangleSide2, out ho_ImageReducedSide2
        //            );
        //        //dev_display (ImageAffinTrans)
        //        //threshold (ImageReducedSide2, RegionSide2, 120, 255)





        //        ho_RegionSide2.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedSide2, out ho_RegionSide2, weldGrayRight4, 255);
        //        ho_RegionSide1.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedSide1, out ho_RegionSide1, weldGrayRight3, 255);

        //        HOperatorSet.SetColor(HPosWindow.HalconWindow, "green");

        //        HOperatorSet.DispObj(ho_RegionSide2, HRightWindow2.HalconWindow);
        //        HOperatorSet.DispObj(ho_RegionSide1, HRightWindow2.HalconWindow);

        //        ho_ImageReducedBack1.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_Rectangleback1, out ho_ImageReducedBack1
        //            );
        //        ho_ImageReducedBack2.Dispose();
        //        HOperatorSet.ReduceDomain(ho_Blue, ho_Rectangleback2, out ho_ImageReducedBack2
        //            );


        //        ho_RegionBack1.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedBack1, out ho_RegionBack1, weldBackGray, 255);
        //        ho_RegionBack2.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedBack2, out ho_RegionBack2, weldBackGray, 255);



        //        //显示
        //        //增加判据 中间孔洞将会被填充 面积过小长度过短的区域被过滤。

        //        ho_RegionSideFill1.Dispose();
        //        HOperatorSet.FillUp(ho_RegionSide1, out ho_RegionSideFill1);

        //        ho_RegionDifference1.Dispose();
        //        HOperatorSet.Difference(ho_ImageReducedSide1, ho_RegionSideFill1, out ho_RegionDifference1
        //            );

        //        ho_ConnectedRegions.Dispose();
        //        HOperatorSet.Connection(ho_RegionDifference1, out ho_ConnectedRegions);

        //        ho_SelectedRegionsBlackH1.Dispose();
        //        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegionsBlackH1, "height",
        //            "and", weldSmallSelLength, 99999);
        //        ho_SelectedRegionsBlackW1.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlackH1, out ho_SelectedRegionsBlackW1, "width",
        //            "and", weldSmallSelWidth, 99999);

        //        ho_SelectedRegionsBlack1.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlackW1, out ho_SelectedRegionsBlack1, "area",
        //            "and", 30, 99999);

        //        ho_RegionUnionholeBlack1.Dispose();
        //        HOperatorSet.Union1(ho_SelectedRegionsBlack1, out ho_RegionUnionholeBlack1);
        //        HOperatorSet.SmallestRectangle2(ho_RegionUnionholeBlack1, out hv_RowBlack1, out hv_ColumnBlack1,
        //            out hv_PhiBlack1, out hv_LengthBlack11, out hv_LengthBlack12);
        //        ho_SelectedRegionsBlackH1.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlack1, out ho_SelectedRegionsBlackH1, "height",
        //            "and", RealWeldHeight1 * 0.7, 99999);
        //        HTuple LargeNum1;
        //        HOperatorSet.CountObj(ho_SelectedRegionsBlackH1, out LargeNum1);

        //        ho_RegionSideFill2.Dispose();
        //        HOperatorSet.FillUp(ho_RegionSide2, out ho_RegionSideFill2);

        //        ho_RegionDifference2.Dispose();
        //        HOperatorSet.Difference(ho_ImageReducedSide2, ho_RegionSideFill2, out ho_RegionDifference2
        //            );

        //        ho_ConnectedRegions.Dispose();
        //        HOperatorSet.Connection(ho_RegionDifference2, out ho_ConnectedRegions);

        //        ho_SelectedRegionsBlackH2.Dispose();
        //        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegionsBlackH2, "height",
        //            "and", weldSmallSelLength, 99999);
        //        ho_SelectedRegionsBlackW2.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlackH2, out ho_SelectedRegionsBlackW2, "width",
        //            "and", weldSmallSelWidth, 99999);

        //        ho_SelectedRegionsBlack2.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlackW2, out ho_SelectedRegionsBlack2, "area",
        //            "and", 30, 99999);


        //        ho_RegionUnionholeBlack2.Dispose();
        //        HOperatorSet.Union1(ho_SelectedRegionsBlack2, out ho_RegionUnionholeBlack2);
        //        HOperatorSet.SmallestRectangle2(ho_RegionUnionholeBlack2, out hv_RowBlack2, out hv_ColumnBlack2,
        //            out hv_PhiBlack2, out hv_LengthBlack21, out hv_LengthBlack22);

        //        ho_SelectedRegionsBlackH2.Dispose();
        //        HOperatorSet.SelectShape(ho_SelectedRegionsBlack2, out ho_SelectedRegionsBlackH2, "height",
        //            "and", RealWeldHeight2 * 0.85, 99999);
        //        HTuple LargeNum2;
        //        HOperatorSet.CountObj(ho_SelectedRegionsBlackH2, out LargeNum2);
        //        ho_RegionBack1.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedBack1, out ho_RegionBack1, weldBackGray, 255);
        //        ho_RegionBack2.Dispose();
        //        HOperatorSet.Threshold(ho_ImageReducedBack2, out ho_RegionBack2, weldBackGray, 255);



        //        HOperatorSet.AreaCenter(ho_RegionBack2, out hv_BackArea2, out hv_SideY, out hv_SideX);
        //        HOperatorSet.AreaCenter(ho_RegionBack1, out hv_BackArea1, out hv_SideY, out hv_SideX);

        //        HOperatorSet.AreaCenter(ho_Rectangleback1, out hv_FullBackArea1, out hv_FullSideY1, out hv_SideX);
        //        HOperatorSet.AreaCenter(ho_Rectangleback2, out hv_FullBackArea2, out hv_FullSideY2, out hv_SideX);






        //        HOperatorSet.AreaCenter(ho_RegionSide2, out hv_SideArea2, out hv_SideY, out hv_SideX);
        //        HOperatorSet.AreaCenter(ho_RegionSide1, out hv_SideArea1, out hv_SideY, out hv_SideX);
        //        //hv_FullSideArea1 = 1869;
        //        //hv_FullSideArea2 = 1869;
        //        //hv_FullSideArea3 = 1701;
        //        //hv_FullSideArea4 = 1701;
        //        HOperatorSet.AreaCenter(ho_RectangleSide1, out hv_FullSideArea1, out hv_FullSideY1, out hv_SideX);
        //        HOperatorSet.AreaCenter(ho_RectangleSide2, out hv_FullSideArea2, out hv_FullSideY2, out hv_SideX);





        //        int realPercent1 = weldLimitPercent;

        //        int realPercent2 = weldLimitPercent;

        //        if (((hv_SideArea2 * 100 / hv_FullSideArea2) < realPercent2) && !HoleFull2 &&
        //            ((int)(new HTuple(hv_LengthBlack21.TupleGreater(((RealWeldHeight2) * weldLengthPercent / 100)))) != 0)
        //            || ((hv_BackArea2 * 100 / hv_FullBackArea2) < weldBackPercent)|| LargeNum2 > 0)
        //        {
        //            HOperatorSet.SetColor(HRightWindow2.HalconWindow, "red");
        //            HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY2 + 55, hv_realholeX2);
        //           // HOperatorSet.WriteString(HRightWindow2.HalconWindow, "NG");
        //            HOperatorSet.DispRegion(ho_RegionSide2, HRightWindow2.HalconWindow);
        //            HOperatorSet.DispRegion(ho_RegionBack2, HRightWindow2.HalconWindow);
        //            resWeldRight[4] = false;

        //        }
        //        else
        //        {
        //            HOperatorSet.SetColor(HRightWindow2.HalconWindow, "green");
        //            HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY2 + 55, hv_realholeX2);
        //          //  HOperatorSet.WriteString(HRightWindow2.HalconWindow, "OK");
        //            HOperatorSet.DispRegion(ho_RegionSide2, HRightWindow2.HalconWindow);
        //            HOperatorSet.DispRegion(ho_RegionBack2, HRightWindow2.HalconWindow);
        //            resWeldRight[4] = true;
        //        }
        //        if (((hv_SideArea1 * 100 / hv_FullSideArea1) < realPercent1) && !HoleFull1 &&
        //            ((int)(new HTuple(hv_LengthBlack11.TupleGreater(((RealWeldHeight1) * weldLengthPercent / 100)))) != 0)
        //            || ((hv_BackArea1 * 100 / hv_FullBackArea1) < weldBackPercent) || LargeNum1 >0)
        //        {
        //            HOperatorSet.SetColor(HRightWindow2.HalconWindow, "red");
        //            HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY1 + 55, hv_realholeX1);
        //          //  HOperatorSet.WriteString(HRightWindow2.HalconWindow, "NG");
        //            HOperatorSet.DispRegion(ho_RegionSide1, HRightWindow2.HalconWindow);
        //            HOperatorSet.DispRegion(ho_RegionBack1, HRightWindow2.HalconWindow);
        //            resWeldRight[3] = false;

        //        }
        //        else
        //        {
        //            HOperatorSet.SetColor(HRightWindow2.HalconWindow, "green");
        //            HOperatorSet.SetTposition(HRightWindow2.HalconWindow, hv_realholeY1 + 55, hv_realholeX1);
        //         //   HOperatorSet.WriteString(HRightWindow2.HalconWindow, "OK");
        //            HOperatorSet.DispRegion(ho_RegionSide1, HRightWindow2.HalconWindow);
        //            HOperatorSet.DispRegion(ho_RegionBack1, HRightWindow2.HalconWindow);
        //            resWeldRight[3] = true;
        //        }

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        string PATH = "E:\\L2" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

        //        HOperatorSet.DumpWindow(HRightWindow.HalconWindow, "jpeg", PATH);

        //        ho_Red.Dispose();
        //        ho_Green.Dispose();
        //        ho_Blue.Dispose();
        //        ho_Rectangle11.Dispose();
        //        ho_Rectangle12.Dispose();
        //        ho_RegionUnion1.Dispose();
        //        ho_ImageReduced1.Dispose();
        //        ho_Rectangle21.Dispose();
        //        ho_Rectangle22.Dispose();
        //        ho_RegionUnion2.Dispose();
        //        ho_ImageReduced2.Dispose();
        //        ho_Rectanglehole1.Dispose();
        //        ho_Rectanglehole2.Dispose();
        //        ho_ImageAffinTrans.Dispose();
        //        ho_RectangleMatch.Dispose();
        //        ho_ImageReduced.Dispose();
        //        ho_Rectangledef1.Dispose();
        //        ho_Rectangledef2.Dispose();
        //        ho_ImageReducedhole1.Dispose();
        //        ho_Regionhole1.Dispose();
        //        ho_RegionErosion1.Dispose();
        //        ho_RegionDilation.Dispose();
        //        ho_RegionFillUp.Dispose();
        //        ho_ConnectedRegions.Dispose();
        //        ho_SelectedRegions.Dispose();
        //        ho_RegionUnionhole1.Dispose();
        //        ho_RectangleSide1.Dispose();
        //        ho_Rectangleback1.Dispose();
        //        ho_ImageReducedSide1.Dispose();
        //        ho_RegionSide1.Dispose();
        //        ho_ImageReducedhole2.Dispose();
        //        ho_Regionhole2.Dispose();
        //        ho_RegionErosion2.Dispose();
        //        ho_RegionUnionhole2.Dispose();
        //        ho_RectangleSide2.Dispose();
        //        ho_Rectangleback2.Dispose();
        //        ho_ImageReducedSide2.Dispose();
        //        ho_RegionSide2.Dispose();
        //        ho_ImageReducedBack1.Dispose();
        //        ho_ImageReducedBack2.Dispose();
        //        ho_RegionBack1.Dispose();
        //        ho_RegionBack2.Dispose();
        //        ho_RegionSideFill1.Dispose();
        //        ho_RegionDifference1.Dispose();
        //        ho_SelectedRegionsBlack1.Dispose();
        //        ho_RegionUnionholeBlack1.Dispose();
        //        ho_RegionDifference2.Dispose();
        //        ho_SelectedRegionsBlack2.Dispose();
        //        ho_RegionUnionholeBlack2.Dispose();

        //        ho_SelectedRegionsBlackH1.Dispose();
        //        ho_SelectedRegionsBlackH2.Dispose();
        //        ho_SelectedRegionsBlackW1.Dispose();
        //        ho_SelectedRegionsBlackW2.Dispose();
        //        ho_RegionSideFill2.Dispose();


        //        ho_RectangleDown1.Dispose();
        //        ho_ImaAmp.Dispose();
        //        ho_ImaDir.Dispose();
        //        ho_RegionColor.Dispose();
        //        ho_RectangleDown2.Dispose();

        //    }
        //}
        #endregion
        #region 左侧判断


        public bool LeftTplCreate(String Path, ref HTuple hv_TemplateID_1, ref HTuple hv_TemplateID_2, ref HTuple hv_TemplateID_3, ref HTuple hv_TemplateID_4)
        {

            HObject ho_MODELS_Image, ho_Border, ho_RectangleTpl, ho_ImageReducedTpl, ho_SelectedContours, ho_Rot_Image;

            // Local control variables 

            HTuple hv_TplX, hv_TplY, hv_TplW, hv_TplH, hv_TplR;
            HTuple hv_ImageName = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_RectangleTpl);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedTpl);
            HOperatorSet.GenEmptyObj(out ho_MODELS_Image);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_Rot_Image);

            // Initialize local and output iconic variables 


            try
            {
                int RotAngle = -3;

                hv_ImageName = TplPath + "\\1.jpg";
                ho_MODELS_Image.Dispose();
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                ho_RectangleTpl.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectangleTpl, 1330, 1268, 0, 1300, 250);
                ho_Rot_Image.Dispose();
                HOperatorSet.RotateImage(ho_MODELS_Image, out ho_Rot_Image, RotAngle, "constant");
                //创建模板
                ho_ImageReducedTpl.Dispose();
                HOperatorSet.ReduceDomain(ho_Rot_Image, ho_RectangleTpl, out ho_ImageReducedTpl);
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_ImageReducedTpl, out ho_Border, 50);
                HOperatorSet.DispObj(ho_Rot_Image, HPosWindow.HalconWindow);



                ho_SelectedContours.Dispose();
                HOperatorSet.SelectContoursXld(ho_Border, out ho_SelectedContours, "contour_length",
                    2000, 3000, -0.5, 0.5);
                HOperatorSet.DispObj(ho_SelectedContours, HPosWindow.HalconWindow);
                HOperatorSet.CreateShapeModelXld(ho_SelectedContours, "auto", -0.39, 0.79, "auto",
                    "auto", "ignore_local_polarity", 5, out hv_TemplateID_1);



                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                ho_MODELS_Image.Dispose();
                ho_Border.Dispose();
                ho_RectangleTpl.Dispose();
                ho_ImageReducedTpl.Dispose();
                ho_SelectedContours.Dispose();
                ho_Rot_Image.Dispose(); ;

            }
        }

        public bool LeftAction(HTuple hv_TemplateID_1, HTuple hv_TemplateID_2, HTuple hv_TemplateID_3, HTuple hv_TemplateID_4, HObject ho_MatchImage)
        {

            // Initialize local and output iconic variables 
            HObject ho_ImageAffinTrans = null, ho_ImageSmooth = null;
            HObject ho_ImageRotate = null, ho_RegionLines = null, ho_RectangleWeldRoi = null;
            HObject ho_ImageReducedWeldRoi = null, ho_RegionWeldRoi = null;
            HObject ho_RegionDilationWeldRoi = null, ho_ConnectedRegionsWeldRoi = null;
            HObject ho_SortedRegionsWeldRoi = null, ho_RectangleWelds = null;
            HObject ho_RegionUnionWelds = null, ho_ImageReducedWelds = null;
            HObject ho_RegionWeldBlack = null, ho_RegionClosingWeldBlack = null;
            HObject ho_RegionClosingBlack = null, ho_SelectedRegionsBlack = null;
            HObject ho_RegionFillUpBlack = null, ho_RegionUnionBlack = null;
            HObject ho_ImageReducedBlack = null, ho_RegionWhiteCore = null;
            HObject ho_RegionErosionWhiteCore = null, ho_RegionDilationWhiteCore = null;
            HObject ho_ConnectedRegionsWhiteCore = null, ho_SelectedRegionsWhiteCore = null;
            HObject ho_RegionTransWhiteCore = null, ho_RegionRealBlack = null;
            HObject ho_RegionDilationRealBlack = null, ho_ConnectedRegionsRealBlackMax = null;
            HObject ho_SelectedRegionsRealBlackMax = null, ho_RegionFillUpWhiteCore = null;
            HObject ho_SelectedRegionsRealBlackWidth = null, ho_SelectedRegionsRealBlackHeight = null;
            HObject ho_UnoinRegionsRealBlackMax = null, ho_SelectedRegionsWhiteCoreMax = null;
            HObject ho_RegionErosion = null, ho_RegionClosing = null, ImageScaled = null;

            HTuple hv_AngleErr = null, hv_WeldRectOffset = null, hv_GrayVal_WeldRegions = null;
            HTuple hv_GrayVal_WeldBlack = null, hv_GrayVal_WhiteCore = null;
            HTuple hv_Index = null, hv_String1 = new HTuple(), hv_ImageName = new HTuple();
            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Angle11 = new HTuple(), hv_Error = new HTuple();
            HTuple hv_RotAngle = new HTuple(), hv_RowPos = new HTuple();
            HTuple hv_ColumnPos = new HTuple(), hv_AnglePos = new HTuple();
            HTuple hv_WeldOffsetX = new HTuple(), hv_RowWeldRoi = new HTuple();
            HTuple hv_ColumnWeldRoi = new HTuple(), hv_PhiWeldRoi = new HTuple();
            HTuple hv_LengthWeldRoi1 = new HTuple(), hv_LengthWeldRoi2 = new HTuple();
            HTuple hv_NumberWeldRegions = new HTuple(), hv_IndexWeldRegions = new HTuple();
            HTuple hv_NumberBlack = new HTuple(), hv_NumberFindWhite = new HTuple();
            HTuple hv_error = new HTuple(), hv_Compactness = new HTuple();
            HTuple hv_RowWhiteCore = new HTuple(), hv_ColumnWhiteCore = new HTuple();
            HTuple hv_PhiWhiteCore = new HTuple(), hv_LengthWhiteCore1 = new HTuple();
            HTuple hv_LengthWhiteCore2 = new HTuple(), hv_GrayVal_RealBlack = new HTuple(), hv_NumberRealBlack = new HTuple();
            HTuple hv_mincenter = new HTuple(), hv_mincenterIdx = new HTuple();
            HTuple hv_Index1 = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_RowRectangleWelds = new HTuple(), hv_ColumnRectangleWelds = new HTuple();


            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ImageAffinTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageSmooth);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_RectangleWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_RegionWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_RegionDilationWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegionsWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_SortedRegionsWeldRoi);
            HOperatorSet.GenEmptyObj(out ho_RectangleWelds);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionWelds);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedWelds);
            HOperatorSet.GenEmptyObj(out ho_RegionWeldBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionClosingWeldBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionClosingBlack);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUpBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionBlack);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_RegionErosionWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_RegionDilationWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegionsWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_RegionTransWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_RegionRealBlack);
            HOperatorSet.GenEmptyObj(out ho_RegionDilationRealBlack);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegionsRealBlackMax);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsRealBlackMax);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUpWhiteCore);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsRealBlackWidth);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsRealBlackHeight);
            HOperatorSet.GenEmptyObj(out ho_UnoinRegionsRealBlackMax);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsWhiteCoreMax);
            HOperatorSet.GenEmptyObj(out ImageScaled);


            try
            {
                resWeldLeft[0] = 0;
                resWeldLeft[1] = 0;
                resWeldLeft[2] = 0;
                resWeldLeft[3] = 0;
                hv_AngleErr = -0.3;
                hv_WeldRectOffset = -240;
                hv_GrayVal_WeldRegions = VisionParam_JKObject.grayVal_weldRegions;// 110;//区域选取灰度
                hv_GrayVal_WeldBlack = VisionParam_JKObject.grayVal_weldShadow;// 100;//焊点阴影选取灰度
                hv_GrayVal_WhiteCore = VisionParam_JKObject.grayVal_whiteCore;// 120;//焊点弧顶白色区域选取灰度
                hv_GrayVal_RealBlack = VisionParam_JKObject.grayVal_realShadow;// 60;//焊点有效阴影灰度
                int WhiteCore_Valid_Width = VisionParam_JKObject.whiteCore_valid_width_min;// 44;//焊点弧顶白色区域有效宽度
                int Black_Valid_Width = VisionParam_JKObject.Shadow_Valid_Width_Min;
                int Black_Valid_Height = VisionParam_JKObject.Shadow_Valid_Height_Min;
                int Black_Valid_Part = VisionParam_JKObject.Shadow_Valid_Thick_Min;
                //for (hv_Index = 1; (int)hv_Index <= 120; hv_Index = (int)hv_Index + 1)
                {




                    ho_ImageSmooth.Dispose();
                    HOperatorSet.SmoothImage(ho_MatchImage, out ho_ImageSmooth, "gauss", 2.5);
                    ImageScaled.Dispose();
                    scale_image_range(ho_ImageSmooth, out ImageScaled, 0, 120);
                    HOperatorSet.FindShapeModel(ImageScaled, hv_TemplateID_1, -0.39, 0.79, 0.5,
                        1, 0.5, "least_squares", 0, 0.9, out hv_Row11, out hv_Column11, out hv_Angle11,
                        out hv_Error);
                    hv_RotAngle = ((hv_Angle11 * 180) / 3.1416) + hv_AngleErr;
                    ho_ImageRotate.Dispose();
                    HOperatorSet.RotateImage(ImageScaled, out ho_ImageRotate, (-hv_RotAngle),
                        "constant");

                    HOperatorSet.FindShapeModel(ho_ImageRotate, hv_TemplateID_1, -0.39, 0.79, 0.7,
                        1, 0.7, "least_squares", 0, 0.8, out hv_RowPos, out hv_ColumnPos, out hv_AnglePos,
                        out hv_Error);

                    if ((int)(new HTuple(hv_Error.TupleLess(0.7))) != 0)
                    {
                        resWeldLeft[0] = 5;
                        resWeldLeft[1] = 5;
                        resWeldLeft[2] = 5;
                        resWeldLeft[3] = 5;
                        return true;
                    }
                    ho_ImageRotate.Dispose();
                    HOperatorSet.RotateImage(ho_ImageSmooth, out ho_ImageRotate, (-hv_RotAngle),
                        "constant");

                    ho_RegionLines.Dispose();
                    HOperatorSet.DispObj(ho_ImageRotate, HLeftWindow.HalconWindow);
                    HOperatorSet.DispLine(HLeftWindow.HalconWindow, hv_RowPos, hv_ColumnPos - 400,
                      hv_RowPos, hv_ColumnPos + 400);
                    hv_WeldOffsetX = hv_RowPos + hv_WeldRectOffset;



                    ho_RectangleWeldRoi.Dispose();
                    HOperatorSet.GenRectangle2(out ho_RectangleWeldRoi, hv_WeldOffsetX, hv_ColumnPos,
                        0, 900, 80);
                    ho_ImageReducedWeldRoi.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageRotate, ho_RectangleWeldRoi, out ho_ImageReducedWeldRoi
                        );
                    ho_RegionWeldRoi.Dispose();
                    HOperatorSet.Threshold(ho_ImageReducedWeldRoi, out ho_RegionWeldRoi, hv_GrayVal_WeldRegions,
                        255);
                    ho_RegionDilationWeldRoi.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionWeldRoi, out ho_RegionDilationWeldRoi,
                        95, 51);
                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDilationWeldRoi, out ho_RegionErosion,
                        95, 51);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionErosion, out ho_RegionClosing, 100,
                        100);
                    ho_ConnectedRegionsWeldRoi.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegionsWeldRoi);

                    ho_SortedRegionsWeldRoi.Dispose();
                    HOperatorSet.SortRegion(ho_ConnectedRegionsWeldRoi, out ho_SortedRegionsWeldRoi,
                        "upper_left", "true", "column");
                    HOperatorSet.SmallestRectangle2(ho_ConnectedRegionsWeldRoi, out hv_RowWeldRoi,
                        out hv_ColumnWeldRoi, out hv_PhiWeldRoi, out hv_LengthWeldRoi1, out hv_LengthWeldRoi2);

                    HOperatorSet.CountObj(ho_ConnectedRegionsWeldRoi, out hv_NumberWeldRegions);
                    HTuple end_val59 = hv_NumberWeldRegions - 1;
                    HTuple step_val59 = 1;
                    for (hv_IndexWeldRegions = 0; hv_IndexWeldRegions.Continue(end_val59, step_val59); hv_IndexWeldRegions = hv_IndexWeldRegions.TupleAdd(step_val59))
                    {
                        ho_RectangleWelds.Dispose();
                        HOperatorSet.GenRectangle2(out ho_RectangleWelds, ((hv_RowWeldRoi.TupleSelect(
            hv_IndexWeldRegions)) + (hv_LengthWeldRoi2.TupleSelect(hv_IndexWeldRegions))) - 150,
            hv_ColumnWeldRoi.TupleSelect(hv_IndexWeldRegions), hv_PhiWeldRoi.TupleSelect(
            hv_IndexWeldRegions), hv_LengthWeldRoi1.TupleSelect(hv_IndexWeldRegions),
            (hv_LengthWeldRoi2.TupleSelect(hv_IndexWeldRegions)) + 25);
                        ho_RegionUnionWelds.Dispose();
                        HOperatorSet.Union1(ho_RectangleWelds, out ho_RegionUnionWelds);
                        ho_ImageReducedWelds.Dispose();
                        HOperatorSet.ReduceDomain(ho_ImageRotate, ho_RegionUnionWelds, out ho_ImageReducedWelds
                            );

                        HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                        HOperatorSet.DispObj(ho_RectangleWelds, HLeftWindow.HalconWindow);
                        //判断黑的部分


                        ho_RegionRealBlack.Dispose();
                        HOperatorSet.Threshold(ho_ImageReducedWelds, out ho_RegionRealBlack, 0, hv_GrayVal_RealBlack);
                        ho_RegionDilationRealBlack.Dispose();
                        HOperatorSet.DilationRectangle1(ho_RegionRealBlack, out ho_RegionDilationRealBlack,
                            5, 5);
                        ho_ConnectedRegionsRealBlackMax.Dispose();
                        HOperatorSet.Connection(ho_RegionDilationRealBlack, out ho_ConnectedRegionsRealBlackMax
                            );

                        ho_SelectedRegionsRealBlackWidth.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegionsRealBlackMax, out ho_SelectedRegionsRealBlackWidth,
                            (new HTuple("width")).TupleConcat("height"), "and", (new HTuple(Black_Valid_Width - 30)).TupleConcat(
                            Black_Valid_Part), (new HTuple(99999999)).TupleConcat(99999999));
                        ho_SelectedRegionsRealBlackHeight.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegionsRealBlackMax, out ho_SelectedRegionsRealBlackHeight,
                            (new HTuple("width")).TupleConcat("height"), "and", (new HTuple(Black_Valid_Part)).TupleConcat(
                            Black_Valid_Height - 20), (new HTuple(99999999)).TupleConcat(99999999));
                        ho_UnoinRegionsRealBlackMax.Dispose();
                        HOperatorSet.Union1(ho_SelectedRegionsRealBlackHeight, out ho_UnoinRegionsRealBlackMax);

                        ho_SelectedRegionsRealBlackMax.Dispose();
                        HOperatorSet.SelectShape(ho_UnoinRegionsRealBlackMax, out ho_SelectedRegionsRealBlackMax,
                            (new HTuple("width")).TupleConcat("height"), "and", (new HTuple(Black_Valid_Width)).TupleConcat(
                            Black_Valid_Height), (new HTuple(99999999)).TupleConcat(99999999));
                        HOperatorSet.CountObj(ho_SelectedRegionsRealBlackMax, out hv_NumberRealBlack);
                        int RealBlack = hv_NumberRealBlack;
                        //如果没有找到阴影那么就认为必须NG，因为可能没有加锡或者焊接
                        if (RealBlack == 0)
                        {
                            resWeldLeft[hv_IndexWeldRegions] = 10;
                        }


                        HOperatorSet.SetColor(HLeftWindow.HalconWindow, "yellow");
                        HOperatorSet.DispObj(ho_SelectedRegionsRealBlackMax, HLeftWindow.HalconWindow);

                        ho_RegionWeldBlack.Dispose();
                        HOperatorSet.Threshold(ho_ImageReducedWelds, out ho_RegionWeldBlack, 0, hv_GrayVal_WeldBlack);


                        ho_RegionClosingWeldBlack.Dispose();
                        HOperatorSet.ClosingRectangle1(ho_RegionWeldBlack, out ho_RegionClosingWeldBlack,
                            100, 100);

                        //difference (RegionClosing1, RegionWeldBlack, RegionDifference)

                        ho_RegionClosingBlack.Dispose();
                        HOperatorSet.Closing(ho_RegionClosingWeldBlack, ho_RegionClosingWeldBlack,
                            out ho_RegionClosingBlack);

                        ho_SelectedRegionsBlack.Dispose();
                        HOperatorSet.SelectShape(ho_RegionClosingBlack, out ho_SelectedRegionsBlack,
                            "area", "and", 1150, 99999);
                        ho_RegionFillUpBlack.Dispose();
                        HOperatorSet.FillUp(ho_SelectedRegionsBlack, out ho_RegionFillUpBlack);
                        ho_RegionUnionBlack.Dispose();
                        HOperatorSet.Union1(ho_RegionFillUpBlack, out ho_RegionUnionBlack);
                        HOperatorSet.CountObj(ho_RegionFillUpBlack, out hv_NumberBlack);
                        //*         for Index2 := 0 to Number-1 by 1
                        ho_ImageReducedBlack.Dispose();
                        HOperatorSet.ReduceDomain(ho_ImageReducedWelds, ho_RegionUnionBlack, out ho_ImageReducedBlack
                            );
                        //
                        ho_RegionWhiteCore.Dispose();
                        HOperatorSet.Threshold(ho_ImageReducedBlack, out ho_RegionWhiteCore, hv_GrayVal_WhiteCore,
                            255);
                        ho_RegionFillUpWhiteCore.Dispose();
                        HOperatorSet.FillUp(ho_RegionWhiteCore, out ho_RegionFillUpWhiteCore);
                        ho_RegionErosionWhiteCore.Dispose();
                        HOperatorSet.ErosionRectangle1(ho_RegionFillUpWhiteCore, out ho_RegionErosionWhiteCore,
                            12, 11);

                        ho_RegionDilationWhiteCore.Dispose();
                        HOperatorSet.DilationRectangle1(ho_RegionErosionWhiteCore, out ho_RegionDilationWhiteCore,
                            6, 11);
                        ho_ConnectedRegionsWhiteCore.Dispose();
                        HOperatorSet.Connection(ho_RegionDilationWhiteCore, out ho_ConnectedRegionsWhiteCore
                            );


                        ho_SelectedRegionsWhiteCore.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegionsWhiteCore, out ho_SelectedRegionsWhiteCore,
                            "area", "and", 2500, 99999);
                        HOperatorSet.AreaCenter(ho_SelectedRegionsWhiteCore, out hv_Area, out hv_Row,
            out hv_Column);
                        HOperatorSet.AreaCenter(ho_RectangleWelds, out hv_Area, out hv_RowRectangleWelds,
                            out hv_ColumnRectangleWelds);
                        HOperatorSet.CountObj(ho_SelectedRegionsWhiteCore, out hv_NumberFindWhite);
                        hv_mincenter = 99999;
                        hv_mincenterIdx = -1;
                        HTuple end_val116 = hv_NumberFindWhite - 1;
                        HTuple step_val116 = 1;
                        for (hv_Index1 = 0; hv_Index1.Continue(end_val116, step_val116); hv_Index1 = hv_Index1.TupleAdd(step_val116))
                        {
                            HOperatorSet.DistancePp(hv_Row.TupleSelect(hv_Index1), hv_Column.TupleSelect(
                                hv_Index1), hv_RowRectangleWelds, hv_ColumnRectangleWelds, out hv_Distance);
                            if ((int)(new HTuple(hv_Distance.TupleLess(hv_mincenter))) != 0)
                            {
                                hv_mincenter = hv_Distance.Clone();
                                hv_mincenterIdx = hv_Index1.Clone();
                            }
                        }
                        //select_shape_std (SelectedRegionsWhiteCore, SelectedRegionsWhiteCoreMax, 'max_area', 70)
                        ho_RegionTransWhiteCore.Dispose();
                        HOperatorSet.ShapeTrans(ho_SelectedRegionsWhiteCore, out ho_RegionTransWhiteCore,
                            "convex");


                        //HOperatorSet.SetColor(HLeftWindow.HalconWindow, "green");
                        // HOperatorSet.DispObj(ho_RegionTransWhiteCore, HLeftWindow.HalconWindow);

                        //HOperatorSet.CountObj(ho_RegionTransWhiteCore, out hv_NumberFindWhite);
                        if ((int)(new HTuple(hv_NumberFindWhite.TupleEqual(0))) != 0)
                        {
                            resWeldLeft[hv_IndexWeldRegions] += 3;

                        }
                        else
                        {
                            HOperatorSet.Compactness(ho_SelectedRegionsWhiteCore, out hv_Compactness);
                            HOperatorSet.SmallestRectangle2(ho_RegionTransWhiteCore, out hv_RowWhiteCore,
                                out hv_ColumnWhiteCore, out hv_PhiWhiteCore, out hv_LengthWhiteCore1,
                                out hv_LengthWhiteCore2);
                            if (RealBlack == 0)
                            {
                                if ((int)((new HTuple(hv_PhiWhiteCore.TupleLess(0.8))).TupleAnd(new HTuple(hv_PhiWhiteCore.TupleGreater(
                                    -0.8)))) != 0)
                                {
                                    if ((int)(new HTuple(((hv_LengthWhiteCore1.TupleSelect(hv_mincenterIdx))).TupleLess(
              WhiteCore_Valid_Width))) != 0)
                                    {
                                        resWeldLeft[hv_IndexWeldRegions] += 99;

                                    }
                                }
                                else
                                {
                                    if ((int)(new HTuple(((hv_LengthWhiteCore2.TupleSelect(hv_mincenterIdx))).TupleLess(
              WhiteCore_Valid_Width))) != 0)
                                    {
                                        resWeldLeft[hv_IndexWeldRegions] += 99;

                                    }
                                }
                            }
                            else
                            {
                                if ((int)((new HTuple(hv_PhiWhiteCore.TupleLess(0.8))).TupleAnd(new HTuple(hv_PhiWhiteCore.TupleGreater(
                                    -0.8)))) != 0)
                                {
                                    if ((int)(new HTuple(((hv_LengthWhiteCore1.TupleSelect(hv_mincenterIdx))).TupleLess(
              30))) != 0)
                                    {
                                        resWeldLeft[hv_IndexWeldRegions] += 99;

                                    }
                                }
                                else
                                {
                                    if ((int)(new HTuple(((hv_LengthWhiteCore2.TupleSelect(hv_mincenterIdx))).TupleLess(
              30))) != 0)
                                    {
                                        resWeldLeft[hv_IndexWeldRegions] += 99;

                                    }
                                }
                            }
                            if ((int)(new HTuple(((hv_Compactness.TupleSelect(hv_mincenterIdx))).TupleGreater(
           3.6))) != 0)
                            {
                                resWeldLeft[hv_IndexWeldRegions] += 99;

                            }
                        }


                        if (resWeldLeft[hv_IndexWeldRegions] > 0)
                        {
                            HOperatorSet.SetColor(HLeftWindow.HalconWindow, "red");
                            HOperatorSet.SetTposition(HLeftWindow.HalconWindow, ((hv_RowWeldRoi.TupleSelect(
                            hv_IndexWeldRegions)) + (hv_LengthWeldRoi2.TupleSelect(hv_IndexWeldRegions))) + 170,
                            hv_ColumnWeldRoi.TupleSelect(hv_IndexWeldRegions));
                            HOperatorSet.WriteString(HLeftWindow.HalconWindow, "NG" + resWeldLeft[hv_IndexWeldRegions].ToString());
                        }
                        else
                        {
                            HOperatorSet.SetColor(HLeftWindow.HalconWindow, "green");
                            HOperatorSet.SetTposition(HLeftWindow.HalconWindow, ((hv_RowWeldRoi.TupleSelect(
                            hv_IndexWeldRegions)) + (hv_LengthWeldRoi2.TupleSelect(hv_IndexWeldRegions))) + 170,
                            hv_ColumnWeldRoi.TupleSelect(hv_IndexWeldRegions));
                            HOperatorSet.WriteString(HLeftWindow.HalconWindow, "OK");
                        }

                    }
                }

                return true;
            }
            catch (Exception e)
            {
                resWeldLeft[1] = 4;
                resWeldLeft[2] = 4;
                resWeldLeft[3] = 4;
                resWeldLeft[0] = 4;
                return false;
            }
            finally
            {

                int right = 0;
                for (int i = 0; i < 4; i++)
                {
                    right += resWeldLeft[i];

                }
                //  if ((resWeldLeft[0] > 0) || (resWeldLeft[1] > 0) || (resWeldLeft[2] > 0) || (resWeldLeft[3] > 0))
                if (right > 0)
                {
                    string PATH = "E:\\NG_t1" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    HOperatorSet.WriteImage(ho_MatchImage, "jpeg", 0, PATH);
                }
                else
                {
                    string PATH = "E:\\OK_t1" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    HOperatorSet.WriteImage(ho_MatchImage, "jpeg", 0, PATH);
                }


                ho_ImageAffinTrans.Dispose();
                ImageScaled.Dispose();
                ho_RegionErosion.Dispose();
                ho_RegionClosing.Dispose();
                ho_ImageSmooth.Dispose();
                ho_ImageRotate.Dispose();
                ho_RegionLines.Dispose();
                ho_RectangleWeldRoi.Dispose();
                ho_ImageReducedWeldRoi.Dispose();
                ho_RegionWeldRoi.Dispose();
                ho_RegionDilationWeldRoi.Dispose();
                ho_ConnectedRegionsWeldRoi.Dispose();
                ho_SortedRegionsWeldRoi.Dispose();
                ho_RectangleWelds.Dispose();
                ho_RegionUnionWelds.Dispose();
                ho_ImageReducedWelds.Dispose();
                ho_RegionWeldBlack.Dispose();
                ho_RegionClosingWeldBlack.Dispose();
                ho_RegionClosingBlack.Dispose();
                ho_SelectedRegionsBlack.Dispose();
                ho_RegionFillUpBlack.Dispose();
                ho_RegionUnionBlack.Dispose();
                ho_ImageReducedBlack.Dispose();
                ho_RegionWhiteCore.Dispose();
                ho_RegionErosionWhiteCore.Dispose();
                ho_RegionDilationWhiteCore.Dispose();
                ho_ConnectedRegionsWhiteCore.Dispose();
                ho_SelectedRegionsWhiteCore.Dispose();
                ho_RegionTransWhiteCore.Dispose();
                ho_RegionRealBlack.Dispose();
                ho_RegionDilationRealBlack.Dispose();
                ho_ConnectedRegionsRealBlackMax.Dispose();
                ho_SelectedRegionsRealBlackMax.Dispose();
                ho_RegionFillUpWhiteCore.Dispose();
                ho_SelectedRegionsRealBlackWidth.Dispose();
                ho_SelectedRegionsRealBlackHeight.Dispose();
                ho_UnoinRegionsRealBlackMax.Dispose();
                ho_SelectedRegionsWhiteCoreMax.Dispose();

            }
        }
        #endregion
        #region 左侧判断2


        public bool LeftTplCreate2(HObject ho_TplImage, ref HTuple hv_TemplateID_1, ref HTuple hv_TemplateID_2)
        {
            HObject ho_ImageSmooth, ho_RectangleTpl, ho_ImageReducedTpl, ho_Border, ho_SelectedContours;

            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_RectangleTpl);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedTpl);
            HOperatorSet.GenEmptyObj(out ho_ImageSmooth);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            //HOperatorSet.GenEmptyObj(out ho_Rot_Image);

            try
            {

                ho_ImageSmooth.Dispose();
                HOperatorSet.SmoothImage(ho_TplImage, out ho_ImageSmooth, "gauss", 2.5);

                ho_RectangleTpl.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectangleTpl, 590, 1268, 0, 1200, 250);


                //创建模板
                ho_ImageReducedTpl.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageSmooth, ho_RectangleTpl, out ho_ImageReducedTpl
                    );
                ho_Border.Dispose();
                HOperatorSet.ThresholdSubPix(ho_ImageReducedTpl, out ho_Border, 150);

                ho_SelectedContours.Dispose();
                HOperatorSet.SelectContoursXld(ho_Border, out ho_SelectedContours, "contour_length",
                    800, 3000, -0.5, 0.5);


                HOperatorSet.DispObj(ho_ImageSmooth, HPosWindow.HalconWindow);
                HOperatorSet.DispObj(ho_SelectedContours, HPosWindow.HalconWindow);
                HOperatorSet.CreateShapeModelXld(ho_SelectedContours, "auto", -0.39, 0.79, "auto",
                    "auto", "ignore_local_polarity", 5, out hv_TemplateID_1);


                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                ho_ImageSmooth.Dispose();
                ho_RectangleTpl.Dispose();
                ho_ImageReducedTpl.Dispose();
                ho_Border.Dispose();
                ho_SelectedContours.Dispose();

            }
        }
        public void scale_image_range(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Min,
     HTuple hv_Max)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_SelectedChannel = null, ho_LowerRegion = null;
            HObject ho_UpperRegion = null;

            HObject ho_Image_COPY_INP_TMP;
            ho_Image_COPY_INP_TMP = ho_Image.CopyObj(1, -1);


            // Local control variables 

            HTuple hv_LowerLimit = new HTuple(), hv_UpperLimit = new HTuple();
            HTuple hv_Mult, hv_Add, hv_Channels, hv_Index, hv_MinGray = new HTuple();
            HTuple hv_MaxGray = new HTuple(), hv_Range = new HTuple();

            HTuple hv_Max_COPY_INP_TMP = hv_Max.Clone();
            HTuple hv_Min_COPY_INP_TMP = hv_Min.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_SelectedChannel);
            HOperatorSet.GenEmptyObj(out ho_LowerRegion);
            HOperatorSet.GenEmptyObj(out ho_UpperRegion);

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
            OTemp[SP_O] = ho_Image_COPY_INP_TMP.CopyObj(1, -1);
            SP_O++;
            ho_Image_COPY_INP_TMP.Dispose();
            HOperatorSet.ScaleImage(OTemp[SP_O - 1], out ho_Image_COPY_INP_TMP, hv_Mult, hv_Add);
            OTemp[SP_O - 1].Dispose();
            SP_O = 0;
            //
            //Clip gray values if necessary
            //This must be done for each channel separately
            HOperatorSet.CountChannels(ho_Image_COPY_INP_TMP, out hv_Channels);
            for (hv_Index = 1; hv_Index.Continue(hv_Channels, 1); hv_Index = hv_Index.TupleAdd(1))
            {
                ho_SelectedChannel.Dispose();
                HOperatorSet.AccessChannel(ho_Image_COPY_INP_TMP, out ho_SelectedChannel, hv_Index);
                HOperatorSet.MinMaxGray(ho_SelectedChannel, ho_SelectedChannel, 0, out hv_MinGray,
                    out hv_MaxGray, out hv_Range);
                ho_LowerRegion.Dispose();
                HOperatorSet.Threshold(ho_SelectedChannel, out ho_LowerRegion, ((hv_MinGray.TupleConcat(
                    hv_LowerLimit))).TupleMin(), hv_LowerLimit);
                ho_UpperRegion.Dispose();
                HOperatorSet.Threshold(ho_SelectedChannel, out ho_UpperRegion, hv_UpperLimit,
                    ((hv_UpperLimit.TupleConcat(hv_MaxGray))).TupleMax());
                OTemp[SP_O] = ho_SelectedChannel.CopyObj(1, -1);
                SP_O++;
                ho_SelectedChannel.Dispose();
                HOperatorSet.PaintRegion(ho_LowerRegion, OTemp[SP_O - 1], out ho_SelectedChannel,
                    hv_LowerLimit, "fill");
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                OTemp[SP_O] = ho_SelectedChannel.CopyObj(1, -1);
                SP_O++;
                ho_SelectedChannel.Dispose();
                HOperatorSet.PaintRegion(ho_UpperRegion, OTemp[SP_O - 1], out ho_SelectedChannel,
                    hv_UpperLimit, "fill");
                OTemp[SP_O - 1].Dispose();
                SP_O = 0;
                if ((int)(new HTuple(hv_Index.TupleEqual(1))) != 0)
                {
                    ho_ImageScaled.Dispose();
                    HOperatorSet.CopyObj(ho_SelectedChannel, out ho_ImageScaled, 1, 1);
                }
                else
                {
                    OTemp[SP_O] = ho_ImageScaled.CopyObj(1, -1);
                    SP_O++;
                    ho_ImageScaled.Dispose();
                    HOperatorSet.AppendChannel(OTemp[SP_O - 1], ho_SelectedChannel, out ho_ImageScaled
                        );
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;
                }
            }
            ho_Image_COPY_INP_TMP.Dispose();
            ho_SelectedChannel.Dispose();
            ho_LowerRegion.Dispose();
            ho_UpperRegion.Dispose();

            return;
        }
        public void CheckAreaProportion(HObject ho_ImageRotate, HObject ho_RegionIntersectionPosRoi, int Index)
        {



            // Local iconic variables 

            HObject ho_ImageReduced, ho_Region1;

            // Local control variables 

            HTuple hv_Area2 = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_Area1 = null, hv_pp;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region1);

            try
            {


                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_RegionIntersectionPosRoi, out ho_ImageReduced
                    );
                ho_Region1.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region1, 100, 255);
                HOperatorSet.AreaCenter(ho_RegionIntersectionPosRoi, out hv_Area2, out hv_Row1,
                    out hv_Column1);
                HOperatorSet.AreaCenter(ho_Region1, out hv_Area1, out hv_Row1, out hv_Column1);
                if ((int)(new HTuple(hv_Area1.TupleGreater(0))) != 0)
                {
                    hv_pp = (hv_Area1 * 100) / hv_Area2;
                    if ((int)(new HTuple(hv_pp.TupleGreater(50))) != 0)
                    {
                        resWeldRight[Index] = 5;
                    }
                }
                else
                {
                    resWeldRight[Index] = 5;
                }

            }
            catch (System.Exception ex)
            {
                resWeldRight[Index] = 5;
            }
            finally
            {
                ho_ImageReduced.Dispose();
                ho_Region1.Dispose();
            }



            return;
        }
        public bool LeftAction2(HTuple hv_TemplateID_1, HTuple hv_TemplateID_2, HObject ho_MatchImage)
        {
            HObject ho_Image, ho_ImageSmooth, ho_RectangleTpl, ho_ImageScale;
            HObject ho_ImageReducedTpl, ho_Border, ho_SelectedContours;
            HObject ho_ImageAffinTrans = null, ho_ImageRotate = null, ho_RegionLines = null;
            HObject ho_RectanglePosRoi1 = null, ho_RectanglePosRoi2 = null;
            HObject ho_RectanglePosRoi3 = null, ho_RectanglePosRoi4 = null;
            HObject ho_RectangleROI = null, ho_ImageReducedROI = null, ho_RegionWeldBack = null;
            HObject ho_RegionDilationWeldBack = null, ho_RegionErosionWeldBack = null;
            HObject ho_ConnectedRegionsWeldBack = null, ho_SelectedRegionsWeldBack = null;
            HObject ho_SortedRegions = null, ho_RegionIntersectionPosRoi1 = null;
            HObject ho_RegionIntersectionPosRoi2 = null, ho_RegionIntersectionPosRoi3 = null;
            HObject ho_RegionIntersectionPosRoi4 = null, ho_RegionFillUpWeldBack = null;
            HObject ho_RegionWeldWhite = null, ho_ConnectedRegionsWeldWhite = null;
            HObject ho_SelectedRegions1 = null, ho_SelectedRegions = null;
            HObject ho_RegionIntersectionWhite1 = null, ho_RegionIntersectionWhite2 = null;
            HObject ho_RegionIntersectionWhite3 = null, ho_RegionIntersectionWhite4 = null;
            HObject ho_RegionUnion1 = null, ho_RegionUnion2 = null, ho_RegionUnion3 = null;
            HObject ho_RegionIntersectionWeldBack = null, ho_RegionUnionWeldBack = null, ho_ImageReducedWeldBack = null;
            // Local control variables 

            HTuple hv_WindowID = null;
            HTuple hv_SideAngleErr = null, hv_SideWeldBackRectOffset = null;
            HTuple hv_SideGrayVal_WeldRegions = null, hv_SideGrayVal_WeldWhite = null;
            HTuple hv_Index = null, hv_String1 = new HTuple(), hv_ImageName = new HTuple();
            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Angle11 = new HTuple(), hv_Error = new HTuple();
            HTuple hv_RotAngle = new HTuple(), hv_RowPos = new HTuple();
            HTuple hv_ColumnPos = new HTuple(), hv_AnglePos = new HTuple();
            HTuple hv_RowWeldBack = new HTuple(), hv_ColumnWeldBack = new HTuple();
            HTuple hv_PhiWeldBack = new HTuple(), hv_LengthWeldBack1 = new HTuple();
            HTuple hv_LengthWeldBack2 = new HTuple(), hv_NumberWeldBack = new HTuple();
            HTuple hv_Number = new HTuple(), hv_error = new HTuple();
            HTuple hv_AreaWhite = new HTuple(), hv_RowWhite = new HTuple(), hv_ColWhite = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedWeldBack);
            HOperatorSet.GenEmptyObj(out ho_ImageSmooth);
            HOperatorSet.GenEmptyObj(out ho_RectangleTpl);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedTpl);
            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_ImageAffinTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageRotate);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_RectanglePosRoi1);
            HOperatorSet.GenEmptyObj(out ho_RectanglePosRoi2);
            HOperatorSet.GenEmptyObj(out ho_RectanglePosRoi3);
            HOperatorSet.GenEmptyObj(out ho_RectanglePosRoi4);
            HOperatorSet.GenEmptyObj(out ho_RectangleROI);
            HOperatorSet.GenEmptyObj(out ho_ImageReducedROI);
            HOperatorSet.GenEmptyObj(out ho_RegionWeldBack);
            HOperatorSet.GenEmptyObj(out ho_RegionDilationWeldBack);
            HOperatorSet.GenEmptyObj(out ho_RegionErosionWeldBack);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegionsWeldBack);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegionsWeldBack);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionPosRoi1);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionPosRoi2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionPosRoi3);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionPosRoi4);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUpWeldBack);
            HOperatorSet.GenEmptyObj(out ho_RegionWeldWhite);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegionsWeldWhite);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionWhite1);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionWhite2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionWhite3);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionWhite4);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion3);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionWeldBack);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersectionWeldBack);
            HOperatorSet.GenEmptyObj(out ho_ImageScale);







            try
            {
                resWeldRight[0] = 0;
                resWeldRight[1] = 0;
                resWeldRight[2] = 0;
                resWeldRight[3] = 0;


                hv_SideAngleErr = -0.3;
                hv_SideWeldBackRectOffset = 80;
                hv_SideGrayVal_WeldRegions = VisionParam_JKObject.sideGrayVal_WeldRegions;//110焊盘区域灰度
                hv_SideGrayVal_WeldWhite = VisionParam_JKObject.focusGrayVal_WeldRegions;//220;//焦点灰度
                int Region_Valid_Height = VisionParam_JKObject.region_Valid_Height;// 60;//焊盘有效高度最小值
                int FoucsWhite_Valid_Width = VisionParam_JKObject.foucsWhite_Valid_Width;// 50;//焦点有效宽度

                ho_ImageSmooth.Dispose();
                HOperatorSet.SmoothImage(ho_MatchImage, out ho_ImageSmooth, "gauss", 2.5);
                ho_ImageScale.Dispose();
                scale_image_range(ho_ImageSmooth, out ho_ImageScale, 30, 110);
                HOperatorSet.FindShapeModel(ho_ImageScale, hv_TemplateID_1, -0.39, 0.79, 0.5,
                    1, 0.5, "least_squares", 0, 0.9, out hv_Row11, out hv_Column11, out hv_Angle11,
                    out hv_Error);
                hv_RotAngle = ((hv_Angle11 * 180) / 3.1416) + hv_SideAngleErr;
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_ImageScale, out ho_ImageRotate, (-hv_RotAngle) / 2,
                    "constant");

                HOperatorSet.FindShapeModel(ho_ImageRotate, hv_TemplateID_1, -0.39, 0.79, 0.5,
                    1, 0.5, "least_squares", 0, 0.9, out hv_RowPos, out hv_ColumnPos, out hv_AnglePos,
                    out hv_Error);
                ho_ImageRotate.Dispose();
                HOperatorSet.RotateImage(ho_ImageSmooth, out ho_ImageRotate, (-hv_RotAngle) / 2,
                    "constant");
                ho_RegionLines.Dispose();
                HOperatorSet.DispObj(ho_ImageRotate, HWindowL1.HalconWindow);
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_RowPos, hv_ColumnPos - 400,
                    hv_RowPos, hv_ColumnPos + 400);

                if (hv_Error < 0.5)
                {
                    resWeldRight[0] = 99;
                    resWeldRight[1] = 99;
                    resWeldRight[2] = 99;
                    resWeldRight[3] = 99;
                    return false;
                }


                //创建 4个检测区域
                ho_RectanglePosRoi1.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectanglePosRoi1, hv_RowPos + hv_SideWeldBackRectOffset,
                    hv_ColumnPos - 600, hv_AnglePos, 150, 150);
                ho_RectanglePosRoi2.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectanglePosRoi2, hv_RowPos + hv_SideWeldBackRectOffset,
                    hv_ColumnPos - 200, hv_AnglePos, 150, 150);
                ho_RectanglePosRoi3.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectanglePosRoi3, hv_RowPos + hv_SideWeldBackRectOffset,
                    hv_ColumnPos + 230, hv_AnglePos, 150, 150);
                ho_RectanglePosRoi4.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectanglePosRoi4, hv_RowPos + hv_SideWeldBackRectOffset,
                    hv_ColumnPos + 650, hv_AnglePos, 150, 150);

                ho_RegionUnion1.Dispose();
                HOperatorSet.Union2(ho_RectanglePosRoi1, ho_RectanglePosRoi2, out ho_RegionUnion1
                    );
                ho_RegionUnion2.Dispose();
                HOperatorSet.Union2(ho_RegionUnion1, ho_RectanglePosRoi3, out ho_RegionUnion2
                    );
                ho_RegionUnion3.Dispose();
                HOperatorSet.Union2(ho_RegionUnion2, ho_RectanglePosRoi4, out ho_RegionUnion3
                    );


                ho_RectangleROI.Dispose();
                HOperatorSet.GenRectangle2(out ho_RectangleROI, hv_RowPos + 120, hv_ColumnPos,
                    hv_AnglePos, 850, 130);
                ho_ImageReducedROI.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_RectangleROI, out ho_ImageReducedROI
                    );
                ho_RegionWeldBack.Dispose();
                HOperatorSet.Threshold(ho_ImageReducedROI, out ho_RegionWeldBack, hv_SideGrayVal_WeldRegions,
                    255);
                ho_RegionErosionWeldBack.Dispose();
                HOperatorSet.ErosionRectangle1(ho_RegionWeldBack, out ho_RegionErosionWeldBack,
                    10, 31);

                ho_RegionDilationWeldBack.Dispose();
                HOperatorSet.DilationRectangle1(ho_RegionErosionWeldBack, out ho_RegionDilationWeldBack,
                    1, 51);
                //dilation_circle (RegionWeldBack, RegionDilationWeldBack, 35.5)
                //erosion_rectangle1 (RegionDilationWeldBack, RegionErosionWeldBack, 1, 31)
                //erosion_circle (RegionDilationWeldBack, RegionErosionWeldBack, 35.5)
                ho_RegionIntersectionWeldBack.Dispose();
                HOperatorSet.Intersection(ho_RegionDilationWeldBack, ho_RegionUnion3, out ho_RegionIntersectionWeldBack
                    );
                ho_ConnectedRegionsWeldBack.Dispose();
                HOperatorSet.Connection(ho_RegionIntersectionWeldBack, out ho_ConnectedRegionsWeldBack
                    );

                ho_SelectedRegionsWeldBack.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegionsWeldBack, out ho_SelectedRegionsWeldBack,
                    "area", "and", 20000, 9999900);


                ho_SortedRegions.Dispose();
                HOperatorSet.SortRegion(ho_SelectedRegionsWeldBack, out ho_SortedRegions, "upper_left",
                    "true", "column");
                HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_RowWeldBack, out hv_ColumnWeldBack,
                    out hv_PhiWeldBack, out hv_LengthWeldBack1, out hv_LengthWeldBack2);

                HOperatorSet.CountObj(ho_SortedRegions, out hv_NumberWeldBack);
                if (hv_NumberWeldBack == 0)
                {
                    resWeldRight[0] = 1;
                    resWeldRight[1] = 1;
                    resWeldRight[2] = 1;
                    resWeldRight[3] = 1;
                }
                HOperatorSet.DispRectangle2(HWindowL1.HalconWindow, hv_RowWeldBack, hv_ColumnWeldBack,
                    hv_PhiWeldBack, hv_LengthWeldBack1, hv_LengthWeldBack2);
                //找到真实的边

                ho_RegionIntersectionPosRoi1.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi1, ho_SelectedRegionsWeldBack,
                    out ho_RegionIntersectionPosRoi1);
                HOperatorSet.CountObj(ho_RegionIntersectionPosRoi1, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[0] = 1;
                }
                CheckAreaProportion(ho_ImageRotate, ho_RegionIntersectionPosRoi1, 1);
                ho_RegionIntersectionPosRoi2.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi2, ho_SelectedRegionsWeldBack,
                    out ho_RegionIntersectionPosRoi2);
                HOperatorSet.CountObj(ho_RegionIntersectionPosRoi2, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[1] = 1;
                }
                CheckAreaProportion(ho_ImageRotate, ho_RegionIntersectionPosRoi2, 2);
                ho_RegionIntersectionPosRoi3.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi3, ho_SelectedRegionsWeldBack,
                    out ho_RegionIntersectionPosRoi3);
                HOperatorSet.CountObj(ho_RegionIntersectionPosRoi3, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[2] = 1;
                }
                CheckAreaProportion(ho_ImageRotate, ho_RegionIntersectionPosRoi3, 3);
                ho_RegionIntersectionPosRoi4.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi4, ho_SelectedRegionsWeldBack,
                    out ho_RegionIntersectionPosRoi4);
                HOperatorSet.CountObj(ho_RegionIntersectionPosRoi4, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[3] = 1;
                }
                CheckAreaProportion(ho_ImageRotate, ho_RegionIntersectionPosRoi4, 4);
                HTuple hv_Index1;
                HTuple end_val84 = hv_NumberWeldBack - 1;
                HTuple step_val84 = 1;
                for (hv_Index1 = 0; hv_Index1.Continue(end_val84, step_val84); hv_Index1 = hv_Index1.TupleAdd(step_val84))
                {

                    if ((int)((new HTuple(((hv_PhiWeldBack.TupleSelect(hv_Index1))).TupleLess(
                        0.8))).TupleAnd(new HTuple(((hv_PhiWeldBack.TupleSelect(hv_Index1))).TupleGreater(
                        -0.8)))) != 0)
                    {
                        if ((int)(new HTuple(((hv_LengthWeldBack2.TupleSelect(hv_Index1))).TupleGreater(
                            Region_Valid_Height))) != 0)
                        {
                            resWeldRight[hv_Index1] = 99;
                        }
                    }
                    else
                    {
                        if ((int)(new HTuple(((hv_LengthWeldBack1.TupleSelect(hv_Index1))).TupleGreater(
                            Region_Valid_Height))) != 0)
                        {
                            resWeldRight[hv_Index1] = 99;
                        }
                    }
                }
                ho_RegionFillUpWeldBack.Dispose();
                HOperatorSet.FillUp(ho_SortedRegions, out ho_RegionFillUpWeldBack);

                ho_RegionUnionWeldBack.Dispose();
                HOperatorSet.Union1(ho_RegionFillUpWeldBack, out ho_RegionUnionWeldBack);
                ho_ImageReducedWeldBack.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageRotate, ho_RegionUnionWeldBack, out ho_ImageReducedWeldBack
                    );

                ho_RegionWeldWhite.Dispose();
                HOperatorSet.Threshold(ho_ImageReducedWeldBack, out ho_RegionWeldWhite, hv_SideGrayVal_WeldWhite,
                    255);
                ho_ConnectedRegionsWeldWhite.Dispose();
                HOperatorSet.Connection(ho_RegionWeldWhite, out ho_ConnectedRegionsWeldWhite
                    );

                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegionsWeldWhite, out ho_SelectedRegions1,
                    "area", "and", 1000, 99999);

                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_SelectedRegions1, out ho_SelectedRegions, "width",
                    "and", FoucsWhite_Valid_Width, 99999);

                // HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                //HOperatorSet.DispObj(ho_SelectedRegions, HWindowL1.HalconWindow);


                ho_RegionIntersectionWhite1.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi1, ho_SelectedRegions, out ho_RegionIntersectionWhite1
                    );
                HOperatorSet.CountObj(ho_RegionIntersectionWhite1, out hv_Number);


                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[0] += 2;
                }
                HOperatorSet.AreaCenter(ho_RegionIntersectionWhite1, out hv_AreaWhite, out hv_RowWhite, out hv_ColWhite);
                if (hv_AreaWhite < 50)
                {
                    resWeldRight[0] += 2;
                }
                ho_RegionIntersectionWhite2.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi2, ho_SelectedRegions, out ho_RegionIntersectionWhite2
                    );
                HOperatorSet.CountObj(ho_RegionIntersectionWhite2, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[1] += 2;
                }
                HOperatorSet.AreaCenter(ho_RegionIntersectionWhite2, out hv_AreaWhite, out hv_RowWhite, out hv_ColWhite);
                if (hv_AreaWhite < 50)
                {
                    resWeldRight[1] += 2;
                }
                ho_RegionIntersectionWhite3.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi3, ho_SelectedRegions, out ho_RegionIntersectionWhite3
                    );
                HOperatorSet.CountObj(ho_RegionIntersectionWhite3, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[2] += 2;
                }
                HOperatorSet.AreaCenter(ho_RegionIntersectionWhite3, out hv_AreaWhite, out hv_RowWhite, out hv_ColWhite);
                if (hv_AreaWhite < 50)
                {
                    resWeldRight[2] += 2;
                }
                ho_RegionIntersectionWhite4.Dispose();
                HOperatorSet.Intersection(ho_RectanglePosRoi4, ho_SelectedRegions, out ho_RegionIntersectionWhite4
                    );
                HOperatorSet.CountObj(ho_RegionIntersectionWhite4, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    resWeldRight[3] += 2;
                }
                HOperatorSet.AreaCenter(ho_RegionIntersectionWhite4, out hv_AreaWhite, out hv_RowWhite, out hv_ColWhite);
                if (hv_AreaWhite < 50)
                {
                    resWeldRight[3] += 2;
                }

                if (resWeldRight[0] > 0)
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos - 600);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[0].ToString());
                }
                else
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos - 600);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                }
                if (resWeldRight[1] > 0)
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos - 200);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[1].ToString());
                }
                else
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos - 200);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                }
                if (resWeldRight[2] > 0)
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos + 200);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[2].ToString());
                }
                else
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos + 200);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                }
                if (resWeldRight[3] > 0)
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos + 600);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[2].ToString());
                }
                else
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                    hv_ColumnPos + 600);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                }


                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {



                int right = 0;
                for (int i = 0; i < 4; i++)
                {
                    right += resWeldRight[i];

                }
                //  if ((resWeldLeft[0] > 0) || (resWeldLeft[1] > 0) || (resWeldLeft[2] > 0) || (resWeldLeft[3] > 0))
                if (right > 0)
                {
                    string PATH = "E:\\NG_E2" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    HOperatorSet.WriteImage(ho_MatchImage, "jpeg", 0, PATH);
                }
                else
                {
                    string PATH = "E:\\OK_E2" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    HOperatorSet.WriteImage(ho_MatchImage, "jpeg", 0, PATH);
                }

                if (hv_RowPos.Length > 0)
                {

                    if (resWeldRight[0] > 0)
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos - 600);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[0].ToString());
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos - 600);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                    }
                    if (resWeldRight[1] > 0)
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos - 200);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[1].ToString());
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos - 200);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                    }
                    if (resWeldRight[2] > 0)
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos + 200);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[2].ToString());
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos + 200);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                    }
                    if (resWeldRight[3] > 0)
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos + 600);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "NG" + resWeldRight[2].ToString());
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWindowL1.HalconWindow, "green");
                        HOperatorSet.SetTposition(HWindowL1.HalconWindow, hv_RowPos,
                        hv_ColumnPos + 600);
                        HOperatorSet.WriteString(HWindowL1.HalconWindow, "OK");
                    }
                }
                else
                {
                    HOperatorSet.SetColor(HWindowL1.HalconWindow, "red");
                    HOperatorSet.SetTposition(HWindowL1.HalconWindow, 1000,
                    1000);
                    HOperatorSet.WriteString(HWindowL1.HalconWindow, "定位失败");
                }


                ho_Image.Dispose();
                ho_ImageSmooth.Dispose();
                ho_RectangleTpl.Dispose();
                ho_ImageScale.Dispose();
                ho_ImageReducedTpl.Dispose();
                ho_Border.Dispose();
                ho_SelectedContours.Dispose();
                ho_ImageAffinTrans.Dispose();
                ho_ImageRotate.Dispose();
                ho_RegionLines.Dispose();
                ho_RectanglePosRoi1.Dispose();
                ho_RectanglePosRoi2.Dispose();
                ho_RectanglePosRoi3.Dispose();
                ho_RectanglePosRoi4.Dispose();
                ho_RectangleROI.Dispose();
                ho_ImageReducedROI.Dispose();
                ho_RegionWeldBack.Dispose();
                ho_RegionDilationWeldBack.Dispose();
                ho_RegionErosionWeldBack.Dispose();
                ho_ConnectedRegionsWeldBack.Dispose();
                ho_SelectedRegionsWeldBack.Dispose();
                ho_SortedRegions.Dispose();
                ho_RegionIntersectionPosRoi1.Dispose();
                ho_RegionIntersectionPosRoi2.Dispose();
                ho_RegionIntersectionPosRoi3.Dispose();
                ho_RegionIntersectionPosRoi4.Dispose();
                ho_RegionFillUpWeldBack.Dispose();
                ho_RegionWeldWhite.Dispose();
                ho_ConnectedRegionsWeldWhite.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionIntersectionWhite1.Dispose();
                ho_RegionIntersectionWhite2.Dispose();
                ho_RegionIntersectionWhite3.Dispose();
                ho_RegionIntersectionWhite4.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionUnion2.Dispose();
                ho_RegionUnion3.Dispose();
                ho_RegionIntersectionWeldBack.Dispose();
                ho_RegionUnionWeldBack.Dispose();
                ho_ImageReducedWeldBack.Dispose();










            }
        }

        #endregion

        //-------------------------------------
        #region My_temp-20171225
        public bool ShowResult(String SavePath, bool flagLimit)
        {
            int right, right2;
            bool isOk = true;
            try
            {



                right = 0;
                for (int i = 0; i < 4; i++)
                {
                    right += resWeldRight[i];
                    Log("侧拍" + i.ToString() + "  *  " + resWeldRight[i].ToString());

                }
                Log("侧拍结果:" + right.ToString());


                right2 = 0;
                for (int i = 0; i < 4; i++)
                {
                    right2 += resWeldLeft[i];
                    Log("正拍" + i.ToString() + "  *  " + resWeldLeft[i].ToString());
                }


                Log("总结果:" + right2.ToString());

                if ((right > 90) || (right2 > 90))
                {
                    //严重NG结果
                    isOk = false;
                }
                else if ((right > 0) && (right2 > 0))
                {
                    isOk = false;
                }
                string PATH = "";
                string 日期 = DateTime.Now.ToLongDateString() + "\\";// 2014年11月09日
                if (isOk && flagLimit)
                {
                    SavePath += "\\OK\\";
                    PATH = "OK_";
                    String ErrorPath = "";

                    if ((FindSN_Module != null) && (FindSN_Module != ""))
                    {
                        ErrorPath = FindSN_Module;
                        StringBuilder rBuilder = new StringBuilder(ErrorPath);
                        foreach (char rInvalidChar in System.IO.Path.GetInvalidFileNameChars())
                            rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                        ErrorPath = rBuilder.ToString();
                    }
                    else
                        ErrorPath = PATH + DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("000");
                    HOperatorSet.DumpWindow(HLeftWindow.HalconWindow, "jpeg", SavePath + ErrorPath);
                    //HOperatorSet.WriteImage(Left_Image, "jpeg", 0, SavePath + ErrorPath);
                }
                else
                {
                    SavePath += "\\NG\\";
                    PATH = "NG_";
                    String ErrorPath = "";
                    if ((FindSN_Module != null) && (FindSN_Module != ""))
                    {
                        ErrorPath = FindSN_Module;
                        StringBuilder rBuilder = new StringBuilder(ErrorPath);
                        foreach (char rInvalidChar in System.IO.Path.GetInvalidFileNameChars())
                            rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                        ErrorPath = rBuilder.ToString();
                    }
                    else
                        ErrorPath = PATH + DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("000");
                    HOperatorSet.DumpWindow(HLeftWindow.HalconWindow, "jpeg", SavePath + ErrorPath);
                    //HOperatorSet.WriteImage(Left_Image, "jpeg", 0, SavePath  + ErrorPath);
                }


                Log("返回结果:" + isOk.ToString());
                return isOk;
            }
            catch (Exception e)
            {
                Log("返回结果:" + e.Message);
                return isOk;
            }
            finally
            {

            }
        }
        #endregion
        //---------------------------------------
        #region 于工-20171225
        public HTuple IDLeft1;
        private void LeftProcess()
        {
            HTuple hv_TemplateIDLeft1 = new HTuple();
            HTuple hv_TemplateIDLeft2 = new HTuple();
            HTuple hv_TemplateIDLeft3 = new HTuple();
            HTuple hv_TemplateIDLeft4 = new HTuple();
            HObject ho_MODELS_Image = null;
            HTuple hv_ImageName = new HTuple();
            hv_ImageName = TplPath + "\\1.jpg";
            try
            {
                //HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                LeftTplCreate(TplPath, ref hv_TemplateIDLeft1, ref hv_TemplateIDLeft2, ref hv_TemplateIDLeft3, ref hv_TemplateIDLeft4);
                IDLeft1 = hv_TemplateIDLeft1;
            }
            catch (Exception e)
            {
                MessageBox.Show("加载模板图片出错");
            }
            while (!bTerminate)
            {
                while (TestStopLeft)
                {
                    Thread.Sleep(50);
                    continue;
                }
                //continue;
                resTest = -1;
                try
                {
                    unsafe
                    {


                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[0], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Left_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;
                                if (!CheckImg(Left_Image))
                                {
                                    Log("检测相机CheckImg失败");
                                    InitLeftCam();
                                    continue;
                                }
                            }
                            else
                            {
                                Log("检测相机拍照失败");
                                InitLeftCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam();
                    continue;
                }
                TestStopLeft = true;

                HOperatorSet.DispObj(Left_Image, HLeftWindow.HalconWindow);
                LeftAction(hv_TemplateIDLeft1, hv_TemplateIDLeft2, hv_TemplateIDLeft3, hv_TemplateIDLeft4, Left_Image);

                resTest = 0;
                //GC.Collect();
                //string PATH = "E:\\t" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() +"--"+ CurIndex.ToString();
                //img.Save(PATH);
                // HOperatorSet.WriteImage(Left_Image, "jpeg", 0, PATH);



            }
        }
        private void LeftProcess2()
        {
            HTuple hv_TemplateIDLeft1 = new HTuple();
            HTuple hv_TemplateIDLeft2 = new HTuple();

            HObject ho_MODELS_Image = null;
            HTuple hv_ImageName = new HTuple();
            hv_ImageName = TplPath + "\\2.jpg";
            try
            {
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                LeftTplCreate2(ho_MODELS_Image, ref hv_TemplateIDLeft1, ref hv_TemplateIDLeft2);
            }
            catch (Exception e)
            {
                MessageBox.Show("加载模板2图片出错   " + hv_ImageName + "    " + e.Message);
            }

            while (!bTerminate)
            {
                while (TestStopLeft2)
                {
                    Thread.Sleep(50);
                    continue;
                }
                //continue;
                resTest = -1;
                try
                {
                    unsafe
                    {


                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[LF2CamIdx], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Left_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);
                                string PATH = "D:\\IMG" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                                img.Save(PATH);
                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;


                                if (!CheckImg(Left_Image))
                                {
                                    Log("检测相机2CheckImg失败");
                                    InitLeftCam2();
                                    continue;
                                }
                            }
                            else
                            {
                                Log("检测相机拍照失败");
                                InitLeftCam2();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam2();
                    continue;
                }
                TestStopLeft2 = true;
                Log("相机2开始检测");
                HOperatorSet.DispObj(Left_Image, HWindowL1.HalconWindow);
                LeftAction2(hv_TemplateIDLeft1, hv_TemplateIDLeft2, Left_Image);
                resTest = 0;
                //GC.Collect();
                //string PATH = "E:\\t" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                //img.Save(PATH);
                // HOperatorSet.WriteImage(Left_Image, "jpeg", 0, PATH);

            }
        }
        private void RightProcess()
        {
            HTuple hv_TemplateIDRight1 = new HTuple();
            HTuple hv_TemplateIDRight2 = new HTuple();
            HTuple hv_TemplateIDRight3 = new HTuple();
            HTuple hv_TemplateIDRight4 = new HTuple();

            HObject ho_MODELS_Image = null;
            HTuple hv_ImageName = new HTuple();
            hv_ImageName = TplPath + "\\TPLRIGHT.jpg";
            try
            {
                //HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                RightTplCreate(ho_MODELS_Image, ref hv_TemplateIDRight1, ref hv_TemplateIDRight2, ref hv_TemplateIDRight3, ref hv_TemplateIDRight4);
            }
            catch (Exception e)
            {
            }
            while (!bTerminate)
            {
                while (TestStopRight)
                {
                    Thread.Sleep(50);
                }
                //continue;
                resTest = -1;
                try
                {
                    unsafe
                    {


                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[1], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Left_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;
                                if (!CheckImg(Left_Image))
                                {
                                    InitLeftCam();
                                    continue;
                                }
                            }
                            else
                            {
                                InitLeftCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam();
                    continue;
                }
                //resWeldRight[1] = true;
                //resWeldRight[2] = true;
                //resWeldRight[3] = true;
                //resWeldRight[4] = true;
                TestStopRight = true;


                RightAction(hv_TemplateIDRight1, hv_TemplateIDRight2, hv_TemplateIDRight3, hv_TemplateIDRight4, Left_Image);
                resTest = 0;
                GC.Collect();
                string PATH = "E:\\t" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "--" + CurIndex.ToString(); ;
                //img.Save(PATH);
                HOperatorSet.WriteImage(Left_Image, "jpeg", 0, PATH);
                //HOperatorSet.DispObj(Left_Image, HPosWindow.HalconWindow);
            }
        }
        private void RightProcess2()
        {
            HTuple hv_TemplateIDRight1 = new HTuple();
            HTuple hv_TemplateIDRight2 = new HTuple();
            HTuple hv_TemplateIDRight3 = new HTuple();
            HTuple hv_TemplateIDRight4 = new HTuple();

            HObject ho_MODELS_Image = null;
            HTuple hv_ImageName = new HTuple();
            hv_ImageName = TplPath + "\\TPLRIGHT2.jpg";
            try
            {
                HOperatorSet.ReadImage(out ho_MODELS_Image, hv_ImageName);
                RightTplCreate2(ho_MODELS_Image, ref hv_TemplateIDRight1, ref hv_TemplateIDRight2);
            }
            catch (Exception e)
            {
            }
            while (!bTerminate)
            {
                while (TestStopRight2)
                {
                    Thread.Sleep(50);
                }
                //continue;
                resTest = -1;
                try
                {
                    unsafe
                    {


                        byte[] image_date = new byte[Len];
                        fixed (byte* p = &image_date[0])
                        {
                            if (JHCap.CameraQueryImage(CamID[1], (IntPtr)p, ref Len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)//RGB24
                            {
                                //_GEN_IMAGE_ERROR_ = true;
                                Bitmap img = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, Len);

                                HOperatorSet.GenImageInterleaved(out Left_Image, (long)data.Scan0, "rgb", Width, Height, 0, "byte", Width, Height, 0, 0, -1, 0);

                                img.UnlockBits(data);
                                img.Dispose();
                                image_date = null;
                                if (!CheckImg(Left_Image))
                                {
                                    InitLeftCam();
                                    continue;
                                }
                            }
                            else
                            {
                                InitLeftCam();
                                continue;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Log(e.Message);
                    InitLeftCam();
                    continue;
                }
                TestStopRight2 = true;
                resTest = 0;
                //RightAction2(hv_TemplateIDRight1, hv_TemplateIDRight2,  Left_Image);
                GC.Collect();
                string PATH = "E:\\t" + DateTime.Now.ToLongDateString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                //img.Save(PATH);
                HOperatorSet.WriteImage(Left_Image, "jpeg", 0, PATH);
                HOperatorSet.DispObj(Left_Image, HPosWindow.HalconWindow);
            }
        }
        #endregion

        #region //打印正面模板匹配日志

        //将日志信息写入日志文件
        public void write(string msg)
        {
            //获取当前程序目录
            string logPath = Path.GetDirectoryName(Application.ExecutablePath);
            //新建文件夹
            System.IO.StreamWriter sw = System.IO.File.AppendText(logPath + "/焊后正面模板匹配日志.txt");
            //写入日志信息
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff ") + msg);
            //关闭文件
            sw.Close();
            //释放内存
            sw.Dispose();
        }
        #endregion

    }

}


