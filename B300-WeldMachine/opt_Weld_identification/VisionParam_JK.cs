using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace opt_Weld_identification
{
    class VisionParam_JK
    {
            private int GrayVal_WeldRegions ;// 110;//区域选取灰度
            private int GrayVal_WeldBlack ;// 100;//焊点阴影选取灰度
            private int GrayVal_WhiteCore;// 120;//焊点弧顶白色区域选取灰度
            private int GrayVal_RealBlack;// 60;//焊点有效阴影灰度
            private int WhiteCore_Valid_Width;// 44;//焊点弧顶白色区域有效宽度
            private int Black_Valid_Width;
            private int Black_Valid_Height;
            private int Black_Valid_Part;
            
            private int SideGrayVal_WeldRegions ;//110焊盘区域灰度
            private int SideGrayVal_WeldWhite;//220;//焦点灰度
            private int Region_Valid_Height;// 60;//焊盘有效高度最小值
            private int FoucsWhite_Valid_Width;// 50;//焦点有效宽度


            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影部分面积阈值")]
            public int grayVal_weldRegions
            {
                get { return GrayVal_WeldRegions; }
                set { GrayVal_WeldRegions = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影区域选取灰度（0~255）")]
            public int grayVal_weldShadow
            {
                get { return GrayVal_WeldBlack; }
                set { GrayVal_WeldBlack = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("汇流条反光区域选取灰度（0~255）")]
            public int grayVal_whiteCore
            {
                get { return GrayVal_WhiteCore; }
                set { GrayVal_WhiteCore = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("有效阴影区域选取灰度（0~255）")]
            public int grayVal_realShadow
            {
                get { return GrayVal_RealBlack; }
                set { GrayVal_RealBlack = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影部分面积阈值（像素）")]
            public int whiteCore_valid_width_min
            {
                get { return WhiteCore_Valid_Width; }
                set { WhiteCore_Valid_Width = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影最小宽度（像素）")]
            public int Shadow_Valid_Width_Min
            {
                get { return Black_Valid_Width; }
                set { Black_Valid_Width = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影最小高度（像素）")]
            public int Shadow_Valid_Height_Min
            {
                get { return Black_Valid_Height; }
                set { Black_Valid_Height = value; }
            }
            [CategoryAttribute("正面影像"), DefaultValueAttribute(0), DescriptionAttribute("阴影最小厚度（像素）")]
            public int Shadow_Valid_Thick_Min
            {
                get { return Black_Valid_Part; }
                set { Black_Valid_Part = value; }
            }

            [CategoryAttribute("侧面影像"), DefaultValueAttribute(0), DescriptionAttribute("焊盘区域选取灰度（0~255）")]
            public int sideGrayVal_WeldRegions
            {
                get { return SideGrayVal_WeldRegions; }
                set { SideGrayVal_WeldRegions = value; }
            }
            [CategoryAttribute("侧面影像"), DefaultValueAttribute(0), DescriptionAttribute("感兴趣区域选取灰度（0~255）")]
            public int focusGrayVal_WeldRegions
            {
                get { return SideGrayVal_WeldWhite; }
                set { SideGrayVal_WeldWhite = value; }
            }
            [CategoryAttribute("侧面影像"), DefaultValueAttribute(0), DescriptionAttribute("焊盘有效高度（像素）")]
            public int region_Valid_Height
            {
                get { return Region_Valid_Height; }
                set { Region_Valid_Height = value; }
            }
            [CategoryAttribute("侧面影像"), DefaultValueAttribute(0), DescriptionAttribute("兴趣区域有效宽度（像素）")]
            public int foucsWhite_Valid_Width
            {
                get { return FoucsWhite_Valid_Width; }
                set { FoucsWhite_Valid_Width = value; }
            }
    }
}
