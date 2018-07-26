using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public class JHCap
{
    public const int API_OK = 0;
    public const int API_ERROR = 1;

    public const int CAMERA_IMAGE_RAW8 = 0x1;
    public const int CAMERA_IMAGE_GRAY8 = 0x2;
    public const int CAMERA_IMAGE_RGB24 = 0x4;
    public const int CAMERA_IMAGE_BGR = 0x100;
    public const int CAMERA_IMAGE_QUAD = 0x200;
    public const int CAMERA_IMAGE_SYNC = 0x10000;
    public const int CAMERA_IMAGE_TRIG = 0x20000;
    public const int CAMERA_IMAGE_STRETCH = 0x1000000;

    public const int CAMERA_IMAGE_BMP = (CAMERA_IMAGE_RGB24 | CAMERA_IMAGE_BGR);

    public const int CAMERA_IN = 0x02;
    public const int CAMERA_OUT = 0x80;

    public const int CAMERA_SNAP_CONTINUATION = 1;
    public const int CAMERA_SNAP_TRIGGER = 2;

    public const int CAMERA_RESOLUTION_CROPPING = 0;
    public const int CAMERA_RESOLUTION_SKIPPING = 1;
    public const int CAMERA_RESOLUTION_BINNING = 2;

    public delegate int SnapThreadCallback(IntPtr pImageBuffer, int width, int height, int format);//回调函数申明

    //////////////////////////////////////////////////////////////////////////////
    ////////////////////////  初始化 ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    //获取API版本
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetVersion", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetVersion(ref int major, ref int minor);

    //获取相机固件版本
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetFirmVersion", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraFirmGetVersion(int major, ref int ver);

    //获取相机数目
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetCount", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetCount(ref int count);

    //相机名字
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetName", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetName(int index, StringBuilder name, StringBuilder model);

    //相机型号
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetID", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetID(int index, ref int modelID, ref int productID);

    //获取相机工作的最后一个错误
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetLastError", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetLastError(ref int lastError);


    //初始化相机
    [DllImport("JHCap2.DLL", EntryPoint = "CameraInit", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraInit(int index);

    //播放（有回调函数）
    [DllImport("JHCap2.DLL", EntryPoint = "CameraPlay", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraPlay(int m_index, IntPtr p, SnapThreadCallback callback);

    //播放(无回调函数)
    [DllImport("JHCap2.DLL", EntryPoint = "CameraPlayWithoutCallback", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraPlayWithoutCallback(int m_index, IntPtr p);


    //暂停
    [DllImport("JHCap2.DLL", EntryPoint = "CameraStop", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraStop(int m_index);

    //释放相机
    [DllImport("JHCap2.DLL", EntryPoint = "CameraFree", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraFree(int m_index);

    //低分辨率预览高分辨率拍照(index为捕获分辨率序号)
    [DllImport("JHCap2.DLL", EntryPoint = "CameraCaptureImage", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraCaptureImage(int device_id, int index, IntPtr imgbuf, ref int length, int option);

    //设置单帧最大时间
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetTimeout", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetTimeout(int m_index, int timeout);

    //获取单帧最大时间
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetTimeout", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetTimeout(int m_index, ref int timeout);
    //////////////////////////////////////////////////////////////////////////////
    ////////////////////////  分辨率 ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    //设置分辨率
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetResolution", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetResolution(int m_index, int index, ref int width, ref int height);

    //获取分辨率模式
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetResolutionMode", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetResolutionMode(int m_index, int mode);

    //设置分辨率模式
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetResolutionMode", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetResolutionMode(int m_index, ref int mode);

    //获取分辨率总数
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetResolutionCount", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetResolutionCount(int m_index, ref int count);

    //获取最大分辨率
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetResolutionMax", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetResolutionMax(int m_index, ref int width, ref int height);

    //获取分辨率
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetResolution", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetResolution(int m_index, int index, ref  int width, ref int height);

    //设置获取图像格式
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetOption", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetOption(int m_index, int Format);

    //获取获取图像格式
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetOption", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetOption(int m_index, ref int Format);
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////// 相机参数 ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    //获取增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetGain", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetGain(int m_index, ref int gain);

    //设置增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetGain", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetGain(int m_index, int gain);

    //获取曝光值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetExposure", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetExposure(int m_index, ref int exposure);

    //设置曝光值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetExposure", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetExposure(int m_index, int exposure);

    //获取伽马值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetGamma", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetGamma(int m_index, ref double gamma);

    //设置伽马值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetGamma", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetGamma(int m_index, double gamma);

    //获取对比度
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetContrast", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetContrast(int m_index, ref double contrast);

    //设置对比度
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetContrast", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetContrast(int m_index, double contrast);

    //获取饱和度
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetSaturation", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetSaturation(int m_index, ref double contrast);

    //设置饱和度
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetSaturation", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetSaturation(int m_index, double contrast);


    //设置黑电平
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetBlackLevel", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetBlackLevel(int m_index, int blacklevel);

    //获取黑电平
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetBlackLevel", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetBlackLevel(int m_index, ref int blacklevel);

    //设置自动曝光值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetAEC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetAEC(int m_index, bool aec);

    //获取自动曝光值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetAEC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetAEC(int m_index, ref bool aec);

    //设置自动增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetAGC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetAGC(int m_index, bool agc);

    //获取自动增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetAGC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetAGC(int m_index, ref bool agc);

    //设置自动白平衡
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetAWB", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetAWB(int m_index, bool awb);

    //获取自动白平衡
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetAWB", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetAWB(int m_index, ref bool awb);

    //设置图像增强
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetEnhancement", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetEnhancement(int m_index, bool enhance);

    //获取图像增强
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetEnhancement", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetEnhancement(int m_index, ref bool enhance);

    //设置插值算法种类
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetInterpolation", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetInterpolation(int m_index, int interpolation);

    //获取插值算法种类
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetInterpolation", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetInterpolation(int m_index, ref int interpolation);

    //设置自动曝光目标值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetAETarget", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetAETarget(int m_index, int target);

    //获取自动曝光目标值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetAETarget", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetAETarget(int m_index, ref int target);

    //设置延迟
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetDelay", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetDelay(int m_index, int delay);

    //获取延迟
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetDelay", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetDelay(int m_index, ref int delay);

    //获取水平镜像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetMirrorX", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetMirrorX(int m_index, ref bool flag);

    //获取垂直镜像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetMirrorY", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetMirrorY(int m_index, ref bool flag);

    //设置水平镜像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetMirrorX", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetMirrorX(int m_index, bool flag);

    //设置垂直镜像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetMirrorY", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetMirrorY(int m_index, bool flag);

    //设置旋转角度
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetRotate", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetRotate(int m_index, int rotate);

    //感兴趣区域
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetROI", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetROI(int m_index, int offset_width, int offset_height, int width, int height);

    //设置白平衡增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetWBGain", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetWBGain(int m_index, double rg, double gg, double bg);

    //获取白平衡增益
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetWBGain", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetWBGain(int m_index, ref double rg, ref double gg, ref double bg);

    //图像大小
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetImageSize", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetImageSize(int m_index, ref int width, ref int height);

    //一键白平衡
    [DllImport("JHCap2.DLL", EntryPoint = "CameraOnePushWB", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraOnePushWB(int m_index);

    //设置频闪值  
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetAntiFlicker", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetAntiFlicker(int m_index, int flicker); //flicker==1/50HZ  flicker==2/60HZ

    //获取频闪值
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetAntiFlicker", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetAntiFlicker(int m_index, ref int flicker);

    //设置高速
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetHighspeed", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetHighspeed(int m_index, bool high);

    //获取高速
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetHighspeed", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetHighspeed(int m_index, ref bool high);

    //载入参数
    [DllImport("JHCap2.DLL", EntryPoint = "CameraLoadParameter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraLoadParameter(int m_index, int group);

    //保存参数
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveParameter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveParameter(int m_index, int group);

    //读取GUID
    [DllImport("JHCap2.dll", EntryPoint = "CameraReadSerialNumber")]
    public static extern int CameraReadSerialNumber(int device_id, StringBuilder id, int length);

    //写入用户数据
    [DllImport("JHCap2.dll", EntryPoint = "CameraWriteUserData")]
    public static extern int CameraWriteUserData(int device_id, String data, int length);

    //读取用户数据
    [DllImport("JHCap2.dll", EntryPoint = "CameraReadUserData")]
    public static extern int CameraReadUserData(int device_id, StringBuilder data, int length);
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////// 外触发设置  ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////	
    //开启闪光输出
    [DllImport("JHCap2.DLL", EntryPoint = "CameraEnableStrobe", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraEnableStrobe(int m_index, bool en);

    //设置闪光电平
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetStrobePolarity", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetStrobePolarity(int m_index, bool high);

    //设置图像获取模式 CAMERA_SNAP_CONTINUATION 或者CAMERA_SNAP_TRIGGER
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetSnapMode", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetSnapMode(int m_index, int snap_mode);

    //获取图像获取模式
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetSnapMode", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetSnapMode(int m_index, ref int snap_mode);

    //触发拍照
    [DllImport("JHCap2.DLL", EntryPoint = "CameraTriggerShot", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraTriggerShot(int m_index);

    //获取GPI
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetGPIO", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetGPIO(int m_index, ref int val);

    //设置GPO
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSetGPIO", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSetGPIO(int m_index, int mask, int val);

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////// 采集图像  ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////	
    //获取图像保存为BMP文件
    [DllImport("JHCap2.DLL", EntryPoint = "CameraShowImage", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraShowImage(int m_index, IntPtr p, int x, int y, int cx, int cy, SnapThreadCallback callback);

    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveBMPB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveBMPB(int m_index, StringBuilder p);

    //获取图像保存为JPG文件
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveJpegB", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveJpegB(int m_index, StringBuilder p, bool flag);

    //采集一帧图像转化为HBITMAP内存对象，供其他Windows API函数使用
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveHBITMAP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveHBITMAP(int m_index, ref IntPtr hBitmap);

    //采集一帧图像保存为对应格式的文件
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveImage", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveImage(int m_index, StringBuilder p, bool flag, int option);

    //获取图像缓存区大小
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetImageBufferSize", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetImageBufferSize(int m_index, ref int size, int option);

    //获取ISP图像需要分配的内存大小
    [DllImport("JHCap2.DLL", EntryPoint = "CameraGetISPImageBufferSize", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraGetISPImageBufferSize(int m_index, ref int size, int width, int height, int option);

    //获取图像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraQueryImage", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraQueryImage(int m_index, IntPtr p, ref int length, int option);

    //把RAW数据转换成RGB数据
    [DllImport("JHCap2.DLL", EntryPoint = "CameraISP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraISP(int m_index, IntPtr pdata, IntPtr imgbuf, int width, int height, int option);

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////// 辅助函数  ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    //BMP文件(buffer RGB 24bit)
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveBMP", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveBMP(string fileName, IntPtr buf, int width, int height);

    //BMP文件(buffer Gray 8bit)
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveBMP8", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveBMP8(string fileName, IntPtr buf, int width, int height);

    //JPG文件(buffer)
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveJpeg", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveJpeg(string fileName, IntPtr buf, int width, int height, bool color, int quality);//buf:RGB buffer, color  TRUE = RGB   FALSE = Grayscale quality 0-100

    //显示buffer图像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraShowBufferImage", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraShowBufferImage(IntPtr hWnd, IntPtr buf, int width, int height, bool color, bool showStretchMode);  //color :true/RGB  false/Gray showStretchMode:true/strecth

    //显示buffer图像
    [DllImport("JHCap2.DLL", EntryPoint = "CameraSaveBufferImage", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern int CameraSaveBufferImage(StringBuilder p, IntPtr buf, int width, int height, bool color, int quality, int option);

    //发送消息
    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    //重新连接
    [DllImport("JHCap2.DLL", EntryPoint = "CameraReconnect", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern void CameraReconnect(int m_index);

    //重新重置
    [DllImport("JHCap2.DLL", EntryPoint = "CameraReset", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
    public static extern void CameraReset(int m_index);
}