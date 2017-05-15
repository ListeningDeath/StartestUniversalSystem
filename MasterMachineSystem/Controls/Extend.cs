using MasterMachineSystem.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataProcessingSystem.Controls
{
    public interface IIniAccess
    {
        void LoadFromIni(Ini _Ini, string SectionName, string ListKeyName, string DefaultKeyName);  // 从ini读取配置参数
        void LoadFromIni(Ini _Ini, string SectionName, string DefaultKeyName);  // 从ini读取配置参数
        void WriteToIni(Ini _Ini, string SectionName, string KeyName, string DefaultKeyName, bool OnlyDefault = false);  // 保存配置参数至ini
        void WriteToIni(Ini _Ini, string SectionName, string KeyName);  // 保存配置参数至ini
    }
    public class iTextBox : TextBox, IIniAccess  // 扩展TextBox
    {
        public void LoadFromIni(Ini _Ini, string SectionName, string KeyName)  // 读取配置参数
        {
            try
            {
                Text = _Ini.Read(SectionName, KeyName);
            }
            catch
            {
                Text = "<配置文件异常>";
            }
        }
        public void WriteToIni(Ini _Ini, string SectionName, string KeyName)
        {
            try
            {
                _Ini.Write(SectionName, KeyName, Text);
            }
            catch
            {

            }
        }
        public virtual void LoadFromIni(Ini _Ini, string SectionName, string ListKeyName, string DefaultKeyName)
        {

        }
        public virtual void WriteToIni(Ini _Ini, string SectionName, string KeyName, string DefaultKeyName, bool OnlyDefault = false)
        {

        }
    }
    public class iComboBox : ComboBox, IIniAccess  // 扩展ComboBox
    {
        public void LoadFromIni(Ini _Ini, string SectionName, string ListKeyName, string DefaultKeyName)  // 读取配置参数数组并装入指定Combobox
        {
            try
            {
                string ListValueString = _Ini.Read(SectionName, ListKeyName);
                string[] ListValueArray = ListValueString.Split(new char[] { ' ', ',', '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ListValueItem in ListValueArray)
                {
                    Items.Add(ListValueItem);
                }
                int DefaultIndex = Items.IndexOf(_Ini.Read(SectionName, DefaultKeyName));
                if (DefaultIndex >= 0)
                    SelectedIndex = DefaultIndex;
                else
                    SelectedIndex = 0;
            }
            catch
            {
                Items.Clear();
                Items.Add("<配置文件异常>");
            }
        }

        public void LoadFromIni(Ini _Ini, string SectionName, string DefaultKeyName)  // 读取配置参数默认值
        {
            try
            {
                int DefaultIndex = Items.IndexOf(_Ini.Read(SectionName, DefaultKeyName));
                if (DefaultIndex >= 0)
                    SelectedIndex = DefaultIndex;
                else
                    SelectedIndex = 0;
            }
            catch
            {
                Items.Clear();
                Items.Add("<配置文件异常>");
            }
        }
        public void WriteToIni(Ini _Ini, string SectionName, string KeyName, string DefaultKeyName, bool OnlyDefault = false)
        {
            try
            {
                if (!OnlyDefault)
                {
                    string ValueString = "";
                    foreach (object _Item in Items)
                    {
                        ValueString += _Item.ToString() + ',';
                    }
                    _Ini.Write(SectionName, KeyName, ValueString.Substring(0, ValueString.Length - 1).Trim());
                }
                _Ini.Write(SectionName, DefaultKeyName, SelectedItem.ToString().Trim());
            }
            catch
            {

            }
        }
        public virtual void WriteToIni(Ini _Ini, string SectionName, string KeyName)
        {

        }
    }
}
