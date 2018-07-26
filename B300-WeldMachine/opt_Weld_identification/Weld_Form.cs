using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//图像操作
using System.Drawing.Imaging;


//IO操作，内存操作
using System.IO;

using HalconDotNet;

namespace opt_Weld_identification
{
    public partial class Weld_Form : Form
    {
        //*******************相机序号**********************
        public static int g_index = 0;
        //图像数据长度
        public static int len = 0;
        //图像数据
        //public byte[] image_date;
        //图像数据长宽
        public int width = 2592, height = 1944;

        public Weld_Form()
        {
            InitializeComponent();
        }

        private void Weld_Form_Load_1(object sender, EventArgs e)
        {
            /*int m = 0, reso_width = 0, reso_height = 0, reso_count = 0;

            //*********************获取相机***********************
            g_index = 0;
            //获取相机数目
            JHCap.CameraGetCount(ref m);
            StringBuilder name = new StringBuilder();
            StringBuilder model = new StringBuilder();
            for (int i = 0; i < m; i++)
            {
                //相机名字
                JHCap.CameraGetName(i, name, model);
                ComboName.Items.Add(model);
                ComboName.SelectedIndex = 2;
                comboBox1.Items.Add(model);
                comboBox1.SelectedIndex = 0;
            }

            if (ComboName.Text == "JHSM500(0)" || ComboName.Text == "JHSM300(0)")
            {
                色差连接.Text = "相机:OK";
                //色差状态.BackColor = Color.DarkBlue;
                //toolStripStatusLabel6.Text = "色差相机:OK";

                //******************* 相机分辨率 ******************
                //获取分辨率总数
                JHCap.CameraGetResolutionCount(g_index, ref reso_count);
                ComboReso.Items.Clear();
                for (int j = 0; j < reso_count; j++)
                {
                    //获取分辨率
                    JHCap.CameraGetResolution(g_index, j, ref reso_width, ref reso_height);
                    ComboReso.Items.Add(reso_width + "*" + reso_height);
                    //ComboReso.SelectedIndex = 0;
                }
                ComboReso.SelectedIndex = 0;
                //设置分辨率
                //JHCap.CameraSetResolution(g_index, ComboReso.SelectedIndex, ref width, ref height);
            }
            else
            {
                //色差状态.BackColor = Color.White;
                色差连接.Text = "相机:未连接";
                MessageBox.Show("相机初始化失败");
            }*/
        }

        private void Weld_Form_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            //暂停
            JHCap.CameraStop(g_index);
            //释放相机
            JHCap.CameraFree(g_index);
            Form2.Weld_Form1 = null;
        }

        private void updateParam()
        {
            //更新参数
            int gain = 0, exposure = 0, blacklevel = 0, target = 0, delay = 0;
            double saturation = 0, rg = 0, gg = 0, bg = 0;
            double gamma = 0, contrast = 0;
            bool agc = false, aec = false, high = false, enhance = false;
            //获取自动增益状态
            JHCap.CameraGetAGC(g_index, ref agc);
            //获取自动曝光状态
            JHCap.CameraGetAEC(g_index, ref aec);
            if (agc)
            {
                自动增益.Checked = true;
                增益.Enabled = false;
                增益显示.Enabled = false;
            }
            else
            {
                //获取增益
                JHCap.CameraGetGain(g_index, ref gain);
                增益.Enabled = true;
                增益显示.Enabled = true;
                增益.Value = gain;
                增益显示.Text = 增益.Value.ToString();
                自动增益.Checked = false;
            }
            if (aec)
            {
                自动曝光.Checked = true;
                曝光.Enabled = false;
                曝光显示.Enabled = false;
            }
            else
            {
                //获取曝光值
                JHCap.CameraGetExposure(g_index, ref exposure);
                曝光.Enabled = true;
                曝光显示.Enabled = true;
                曝光.Value = exposure;
                曝光显示.Text = 曝光.Value.ToString();
                自动曝光.Checked = false;

            }
            //获取伽马值
            JHCap.CameraGetGamma(g_index, ref gamma);
            伽马.Value = (int)gamma * 100;
            伽马显示.Text = gamma.ToString();
            //获取对比度
            JHCap.CameraGetContrast(g_index, ref contrast);
            对比度.Value = (int)contrast * 100;
            对比度显示.Text = contrast.ToString();
            //获取饱和度
            JHCap.CameraGetSaturation(g_index, ref saturation);
            饱和度.Value = (int)saturation * 100;
            饱和度显示.Text = saturation.ToString();
            //获取黑电平
            JHCap.CameraGetBlackLevel(g_index, ref blacklevel);
            黑电平.Value = blacklevel;
            黑电平显示.Text = blacklevel.ToString();

            //获取红、绿、蓝增益
            JHCap.CameraGetWBGain(g_index, ref rg, ref gg, ref bg);
            红色增益.Value = (int)rg * 100;
            绿色增益.Value = (int)gg * 100;
            蓝色增益.Value = (int)bg * 100;
            红色显示.Text = rg.ToString();
            绿色显示.Text = gg.ToString();
            蓝色显示.Text = bg.ToString();

            //获取颜色增强
            JHCap.CameraGetEnhancement(g_index, ref enhance);
            颜色增强.Checked = enhance;

            //获取自动曝光目标值
            JHCap.CameraGetAETarget(g_index, ref target);
            自动曝光目标.Value = target;
            自动曝光目标显示.Text = target.ToString();

            bool horizontal = false, vertical = false;
            //获取水平镜像
            JHCap.CameraGetMirrorX(g_index, ref horizontal);
            //获取垂直镜像
            JHCap.CameraGetMirrorY(g_index, ref vertical);
            //设置水平镜像
            JHCap.CameraSetMirrorX(g_index, horizontal);
            //设置垂直镜像
            JHCap.CameraSetMirrorY(g_index, vertical);
            水平翻转.Checked = horizontal;
            垂直翻转.Checked = vertical;
            //获取高速
            JHCap.CameraGetHighspeed(g_index, ref high);
            高速.Checked = high;
            //获取延迟
            JHCap.CameraGetDelay(g_index, ref delay);
            延时.Value = delay;
            延时显示.Text = delay.ToString();

            //ComboReso.SelectedIndex = 0;
        }

        private void 打开预览_Click(object sender, EventArgs e)
        {
            JHCap.CameraPlayWithoutCallback(g_index, pictureBox1.Handle);
        }

        private void ComboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            int reso_count = 0, reso_width = 0, reso_height = 0;
            ComboReso.Items.Clear();
            //g_index = ComboName.SelectedIndex;
            //初始化相机
            int c = JHCap.CameraInit(g_index);
            if (c == 1)
            {
                色差连接.Text = "相机:未连接";
                MessageBox.Show("相机初始化失败");
            }
            else
            {
                //获取分辨率总数
                JHCap.CameraGetResolutionCount(g_index, ref reso_count);
                for (int j = 0; j < reso_count; j++)
                {
                    //获取分辨率
                    JHCap.CameraGetResolution(g_index, j, ref reso_width, ref reso_height);
                    ComboReso.Items.Add(reso_width + "*" + reso_height);
                    ComboReso.SelectedIndex = 0;
                }
                updateParam();
            }
        }

        private void ComboReso_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
            //设置分辨率
            JHCap.CameraSetResolution(g_index, ComboReso.SelectedIndex, ref width, ref height);
            //JHCap.CameraGetImageBufferSize(g_index, ref len, JHCap.CAMERA_IMAGE_RGB24);
            //image_date = new byte[len];
        }

        private void 停止预览_Click(object sender, EventArgs e)
        {
            JHCap.CameraStop(g_index);
        }

        private void 伽马_Scroll(object sender, EventArgs e)
        {
            double temp;
            temp = 伽马.Value / 100.0;
            JHCap.CameraSetGamma(g_index, temp);
            temp = 伽马.Value / 100.0;
            伽马显示.Text = temp.ToString();
        }

        private void 对比度_Scroll(object sender, EventArgs e)
        {
            double temp;
            temp = 对比度.Value / 100.0;
            JHCap.CameraSetContrast(g_index, temp);
            对比度显示.Text = temp.ToString();
        }

        private void 饱和度_Scroll(object sender, EventArgs e)
        {
            double temp;
            temp = 饱和度.Value / 100.0;
            JHCap.CameraSetSaturation(g_index, temp);
            饱和度显示.Text = temp.ToString();
        }

        private void 黑电平_Scroll(object sender, EventArgs e)
        {
            JHCap.CameraSetBlackLevel(g_index, 黑电平.Value);
            黑电平显示.Text = 黑电平.Value.ToString();
        }

        private void 增益_Scroll(object sender, EventArgs e)
        {
            JHCap.CameraSetGain(g_index, 增益.Value);
            增益显示.Text = 增益.Value.ToString();
        }

        private void 曝光_Scroll(object sender, EventArgs e)
        {
            JHCap.CameraSetExposure(g_index, 曝光.Value);
            曝光显示.Text = 曝光.Value.ToString();
        }

        private void 颜色增强_CheckedChanged(object sender, EventArgs e)
        {
            JHCap.CameraSetEnhancement(g_index, 颜色增强.Checked);
        }

        private void 一键白平衡_Click(object sender, EventArgs e)
        {
            JHCap.CameraOnePushWB(g_index);
        }

        private void 红色增益_Scroll(object sender, EventArgs e)
        {
            //获取红、绿、蓝增益
            JHCap.CameraSetWBGain(g_index, (double)红色增益.Value, (double)绿色增益.Value, (double)蓝色增益.Value);
            红色显示.Text = 红色增益.Value.ToString();
            绿色显示.Text = 绿色增益.Value.ToString();
            蓝色显示.Text = 蓝色增益.Value.ToString();
        }

        private void 绿色增益_Scroll(object sender, EventArgs e)
        {
            //获取红、绿、蓝增益
            JHCap.CameraSetWBGain(g_index, (double)红色增益.Value, (double)绿色增益.Value, (double)蓝色增益.Value);
            红色显示.Text = 红色增益.Value.ToString();
            绿色显示.Text = 绿色增益.Value.ToString();
            蓝色显示.Text = 蓝色增益.Value.ToString();
        }

        private void 蓝色增益_Scroll(object sender, EventArgs e)
        {
            //获取红、绿、蓝增益
            JHCap.CameraSetWBGain(g_index, (double)红色增益.Value, (double)绿色增益.Value, (double)蓝色增益.Value);
            红色显示.Text = 红色增益.Value.ToString();
            绿色显示.Text = 绿色增益.Value.ToString();
            蓝色显示.Text = 蓝色增益.Value.ToString();
        }

        private void 水平翻转_CheckedChanged(object sender, EventArgs e)
        {
            bool horizontal = false;
            JHCap.CameraGetMirrorY(g_index, ref horizontal);
            JHCap.CameraSetMirrorY(g_index, !horizontal); 
        }

        private void 垂直翻转_CheckedChanged(object sender, EventArgs e)
        {
            bool vetrical = false;
            JHCap.CameraGetMirrorX(g_index, ref vetrical);
            JHCap.CameraSetMirrorX(g_index, !vetrical);  
        }

        private void 高速_CheckedChanged(object sender, EventArgs e)
        {
            JHCap.CameraSetHighspeed(g_index, 高速.Checked);
        }

        private void 默认参数_Click(object sender, EventArgs e)
        {
            JHCap.CameraLoadParameter(g_index, 1);
            updateParam();
        }

        private void 保存参数_Click(object sender, EventArgs e)
        {
            JHCap.CameraSaveParameter(g_index, 0);
        }

        private void 保存图片_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "保存";
            saveFileDialog1.Filter = "jpg files (*.jpg)|*.jpg";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StringBuilder p = new StringBuilder(saveFileDialog1.FileName);
                JHCap.CameraSaveJpegB(g_index, p, true);

            }
        }



       /* private void HALCON数据测试_Click(object sender, EventArgs e)
        {
            HObject ho_Image = null;

            HOperatorSet.GenEmptyObj(out ho_Image);

            int len = 0, width = 0, height = 0;
            JHCap.CameraGetImageSize(g_index, ref width, ref height);

            JHCap.CameraGetImageBufferSize(g_index, ref len, JHCap.CAMERA_IMAGE_RGB24); //RGB24

            byte[] image_date = new byte[len];

            if (JHCap.CameraQueryImage(g_index, image_date, ref len, JHCap.CAMERA_IMAGE_RGB24 | JHCap.CAMERA_IMAGE_SYNC) == 0)   //RGB24
            {
                //锁定内存数据
                Bitmap img = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                //输入颜色数据
                BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                //将托管数据复制到非托管数据
                System.Runtime.InteropServices.Marshal.Copy(image_date, 0, data.Scan0, len);

                HOperatorSet.GenImageInterleaved(out ho_Image, (long)data.Scan0, "rgb", width, height, 0, "byte", width, height, 0, 0, -1, 0);
                HOperatorSet.DispObj(ho_Image, hWindowControl3.HalconWindow);
            }

            ho_Image.Dispose();
        }*/



    }
}
