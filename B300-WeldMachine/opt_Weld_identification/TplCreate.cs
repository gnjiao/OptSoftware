using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Runtime.InteropServices;

namespace opt_Weld_identification
{
    class TplCreate
    {
        HObject TplRect = new HObject();
        public bool bUse = false;
        int CurW;
        int CurH;
        public int CurX;
        public int CurY;
        public HObject SrcImage;
        public HalconDotNet.HWindowControl HTplWindow;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        
         


        public void CreateRect(int X, int Y, int W, int H)
        {
            if (!bUse)
            {
                return;
            }
         //   HOperatorSet.SetDraw(HTplWindow.HalconWindow, "margin");
            HOperatorSet.SetDraw(HTplWindow.HalconWindow, "margin");
            HOperatorSet.SetLineWidth(HTplWindow.HalconWindow, 1);
            HOperatorSet.SetColor(HTplWindow.HalconWindow, "yellow");

            CurW = W;
            CurH = H;
            CurX = X;
            CurY = Y;
            TplRect.Dispose();
            HOperatorSet.GenRectangle2(out TplRect, Y, X, 0, W, H);
            HOperatorSet.DispObj(SrcImage, HTplWindow.HalconWindow);
            HOperatorSet.DispObj(TplRect, HTplWindow.HalconWindow);
        }

        public void SaveTplImage(String Path)
        {
            HObject TplImage,DstImage;
            HOperatorSet.ReduceDomain(SrcImage, TplRect, out DstImage);
            HOperatorSet.CropDomain(DstImage, out TplImage);
            HOperatorSet.WriteImage(TplImage, "jpeg", 0, Path + "\\Tpl");
            WritePrivateProfileString("TPLPOS", "X", CurX.ToString(), Path + "\\Tplcfg.ini");
            WritePrivateProfileString("TPLPOS", "Y", CurY.ToString(), Path + "\\Tplcfg.ini");
            HOperatorSet.DispObj(SrcImage, HTplWindow.HalconWindow);
            DstImage.Dispose();
            TplImage.Dispose();
            bUse = false;
        }
    }
}
