using System;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace Common
{
    public class ComputerInfo
    {
        public static string CpuID;
        public static string MacAddress;
        public static string DiskID;
        public static string IpAddress;
        public static string LoginUserName;
        public static string Computer_SerialNumberName;
        public static string SystemType;
        public static string TotalPhysicalMemory; //单位：M   
        //private static ComputerInfo _instance;
        //public static ComputerInfo Instance()
        //{
        //    if (_instance == null)
        //        _instance = new ComputerInfo();
        //    return _instance;
        //}
        //protected ComputerInfo()
        //{
        //    CpuID = GetCpuID();
        //    MacAddress = GetMacAddress();
        //    DiskID = GetDiskID();
        //    IpAddress = GetIPAddress();
        //    LoginUserName = GetUserName();
        //    SystemType = GetSystemType();
        //    TotalPhysicalMemory = GetTotalPhysicalMemory();
        //    Computer_SerialNumberName = GetComputer_SerialNumberName();
        //}

        public static string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码   
                string cpuInfo = "";//cpu序列号   
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "ERROR";
            }

        }
        public static string GetMacAddress()
        {
            try
            {
                //获取网卡Mac地址   
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "ERROR";
            }

        }
        public static string GetIPAddress()
        {
            try
            {
                //获取IP地址   
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString();   
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "ERROR";
            }

        }

        public static string GetDiskID()
        {
            try
            {
                //获取硬盘ID   
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "ERROR";
            }
            //finally
            //{
            //}

        }
        ///    
        /// 操作系统的登录用户名   
        ///    
        ///    
        public static string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_Computer_SerialNumberSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "ERROR";
            }

        }

        ///    
        /// PC类型   
        ///    
        ///    
        public static string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_Computer_SerialNumberSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "ERROR";
            }

        }
        ///    
        /// 物理内存   
        ///    
        ///    
        public static string GetTotalPhysicalMemory()
        {
            try
            {

                string st = "";
                ManagementClass mc = new ManagementClass("Win32_Computer_SerialNumberSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["TotalPhysicalMemory"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "ERROR";
            }

        }
        ///    
        /// 计算机名称   
        ///    
        ///    
        public static string GetComputer_SerialNumberName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("Computer_SerialNumberName");
            }
            catch
            {
                return "ERROR";
            }

        }

        public static string  info = "";
        public static string GetComputerInfo(string cameraID)
        {
            //string info = string.Empty;
            //string cpu = GetCPUInfo();
            //string baseBoard = GetBaseBoardInfo();
            //string bios = GetBIOSInfo();
            //string mac = GetMACInfo();
            //info = string.Concat(cpu, baseBoard, bios, mac, cameraID);
            //CpuID = GetCpuID();
            //MacAddress = GetMacAddress();
            //DiskID = GetDiskID();
            //IpAddress = GetIPAddress();
            //LoginUserName = GetUserName();
            //SystemType = GetSystemType();
            //TotalPhysicalMemory = GetTotalPhysicalMemory();
            //Computer_SerialNumberName = GetComputer_SerialNumberName();
            info = string.Concat( cameraID);
            return info;
        }

        /*
        private static string GetCPUInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return info;
        }
        private static string GetBIOSInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return info;
        }
        private static string GetBaseBoardInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }
        private static string GetMACInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }
        private static string GetHardWareInfo(string typePath, string key)
        {
            try
            {
                ManagementClass managementClass = new ManagementClass(typePath);
                ManagementObjectCollection mn = managementClass.GetInstances();
                PropertyDataCollection properties = managementClass.Properties;
                foreach (PropertyData property in properties)
                {
                    if (property.Name == key)
                    {
                        foreach (ManagementObject m in mn)
                        {
                            return m.Properties[property.Name].Value.ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理  
            }
            return string.Empty;
        }
        private static string GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        string fRegistryKey = key + adapter.Id + "\\Connection";
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                            int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 &&
                                fPnpInstanceID.Substring(0, 3) == "PCI")
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                for (int i = 1; i < 6; i++)
                                {
                                    macAddress = macAddress.Insert(3 * i - 1, ":");
                                }
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理  
            }
            return macAddress;
        }
         */
    }
}
