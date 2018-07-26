using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace opt_Weld_identification
{
    public static class XmlHelper
    {
        
        public static string xmlFileName = null;

      

      

        public static String GetValueString(string Node, string valuetype, string name)
        {
            String Value = "";
            XmlDocument xmldoc1 = new XmlDocument();
            if (!File.Exists(xmlFileName))
            {
            }
            xmldoc1.Load(xmlFileName); //加载xml文件
            XmlNode node = xmldoc1.SelectSingleNode("Param");
            XmlNodeList xnl = node.ChildNodes;
            foreach (XmlNode child in xnl)
            {
                if (child.Name == Node)//Values1
                {
                    if (valuetype == "value0")
                    {
                        XmlNodeList vo = child.ChildNodes;
                        foreach (XmlNode value in vo)
                        {
                            XmlElement elem = (XmlElement)value;
                            if (elem.GetAttribute("name") == name)
                            {
                                Value = (elem.GetAttribute("default"));
                                break;
                            }
                        }
                    }
                    XmlNodeList _value1s = child.ChildNodes;
                    foreach (XmlNode value in _value1s)
                    {
                        if (value.Name == valuetype)
                        {
                            XmlNodeList _v = value.ChildNodes;
                            foreach (XmlNode v in _v)
                            {
                                XmlElement elem = (XmlElement)v;
                                if (elem.GetAttribute("name") == name)
                                {
                                    Value = (elem.GetAttribute("default"));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return Value;
        }

        public static double GetValueDouble(string Node,string valuetype, string name)
        {
         //   MessageBox.Show(name);
            double Value = 0;
            XmlDocument xmldoc1 = new XmlDocument();
            if (!File.Exists(xmlFileName))
            {
            }
            xmldoc1.Load(xmlFileName); //加载xml文件
            XmlNode node = xmldoc1.SelectSingleNode("Param");
            XmlNodeList xnl = node.ChildNodes;
            foreach (XmlNode child in xnl)
            {
                if (child.Name == Node)//Values1
                {
                     if (valuetype == "value0")
                    {
                        XmlNodeList vo = child.ChildNodes;
                        foreach (XmlNode value in vo)
                        {
                            XmlElement elem = (XmlElement)value;
                            if (elem.GetAttribute("name") == name)
                             {
                            
                                Value = Convert.ToDouble(elem.GetAttribute("default"));
                                break;
                            }
                        }
                    }
                    XmlNodeList _value1s = child.ChildNodes;
                    foreach (XmlNode value in _value1s)
                    {
                        if (value.Name == valuetype)
                        {
                            XmlNodeList _v = value.ChildNodes;
                            foreach (XmlNode v in _v)
                            {
                                XmlElement elem = (XmlElement)v;
                                if (elem.GetAttribute("name") == name)
                                {
                                    Value = Convert.ToDouble(elem.GetAttribute("default"));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return Value;
        }
    }
   
}
