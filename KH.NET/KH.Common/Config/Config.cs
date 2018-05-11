using System.Configuration;
using System.Web;
using System.Xml;

namespace KH.Common.Config
{
    /// <summary>
    /// Config文件操作
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 根据Key取Value值
        /// </summary>
        /// <param name="key"></param>
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key].Trim();
        }

        /// <summary>
        /// 根据Key修改Value
        /// </summary>
        /// <param name="key">要修改的Key</param>
        /// <param name="value">要修改为的值</param>
        public static void SetValue(string key, string value)
        {
            var xDoc = new XmlDocument();
            xDoc.Load(HttpContext.Current.Server.MapPath("~/XmlConfig/system.config"));
            var xNode = xDoc.SelectSingleNode("//appSettings");

            var xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", value);
            else
            {
                var xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", key);
                xElem2.SetAttribute("value", value);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(HttpContext.Current.Server.MapPath("~/XmlConfig/system.config"));
        }

        /// <summary>
        /// 获取配置节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">配置节</param>
        /// <returns></returns>
        public static T GetSection<T>(string section)
        {
            return (T)ConfigurationManager.GetSection(section);
        }
    }
}
