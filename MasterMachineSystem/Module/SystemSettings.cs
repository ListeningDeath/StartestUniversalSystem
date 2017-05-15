using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;

namespace MasterMachineSystem.Module
{
    public class Ini
    {
        private string FileName;  // ini文件路径

        public Ini(string ConfigFileName)
        {
            FileName = ConfigFileName;
        }
        /// <summary>  
        /// 写操作  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);


        /// <summary>  
        /// 读操作  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="def">未读取到的默认值</param>  
        /// <param name="retVal">读取到的值</param>  
        /// <param name="size">大小</param>  
        /// <param name="filePath">路径</param>  
        /// <returns></returns>  
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>  
        /// 读ini文件  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="defValue">未读取到值时的默认值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public string Read(string section, string key)
        {
            string IniFilePath = FileName;
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, IniFilePath);
            return temp.ToString().Trim();
        }

        /// <summary>  
        /// 写入ini文件  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public void Write(string section, string key, string value)
        {
            string IniFilePath = FileName;
            WritePrivateProfileString(section, key, value, IniFilePath);
        }
        /// <summary>  
        /// 删除节  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="filePath"></param>  
        /// <returns></returns>  
        public long DeleteSection(string section)
        {
            string IniFilePath = FileName;
            return WritePrivateProfileString(section, null, null, IniFilePath);
        }

        /// <summary>  
        /// 删除键  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public long DeleteKey(string section, string key)
        {
            string IniFilePath = FileName;
            return WritePrivateProfileString(section, key, null, IniFilePath);
        }

        public void WriteDefault()
        {
            // 数据帧配置
            Write("FRAME", "LENGTH", "8");
        }
    }
    class FrameConfig  // 帧配置
    {
        public int Length { set; get; }  // 帧字节数
        public int StartBitsLength { set; get; }  // 起始位长
        public int EndBitsLength { set; get; }  // 终止位长
    }
}
