using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MasterMachineSystem.Module;
using System.IO.Ports;
using MahApps.Metro.Controls;

namespace MasterMachineSystem
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : MetroWindow
    {
        // 全局变量
        public bool IsOK = false;
        public CommType _CommType;

        public ConfigWindow()
        {
            InitializeComponent();
            BindSPPort();
            BindSPBaudRate();
            BindSPParity();
            BindSPDataBits();
            BindSPStopBits();
            BindEthProtocol();
            BindEthIP();
            BindEthPort();
        }

        private void BindSPPort()
        {
            foreach (string PortName in SerialPort.GetPortNames())
            {
                cbSPPort.Items.Add(PortName);
            }
            cbSPPort.SelectedIndex = 0;
        }
        private void BindSPBaudRate()
        {
            cbSPBaudRate.Items.Add(300);
            cbSPBaudRate.Items.Add(600);
            cbSPBaudRate.Items.Add(1200);
            cbSPBaudRate.Items.Add(2400);
            cbSPBaudRate.Items.Add(4800);
            cbSPBaudRate.Items.Add(9600);
            cbSPBaudRate.Items.Add(19200);
            cbSPBaudRate.Items.Add(38400);
            cbSPBaudRate.Items.Add(43000);
            cbSPBaudRate.Items.Add(56000);
            cbSPBaudRate.Items.Add(57600);
            cbSPBaudRate.Items.Add(115200);
            cbSPBaudRate.SelectedItem = 9600;
        }
        private void BindSPParity()
        {
            cbSPParity.Items.Add("NONE");
            cbSPParity.Items.Add("ODD");
            cbSPParity.Items.Add("EVEN");
            cbSPParity.Items.Add("MARK");
            cbSPParity.Items.Add("SPACE");
            cbSPParity.SelectedItem = "NONE";
        }
        private void BindSPDataBits()
        {
            cbSPDataBits.Items.Add(5);
            cbSPDataBits.Items.Add(6);
            cbSPDataBits.Items.Add(7);
            cbSPDataBits.Items.Add(8);
            cbSPDataBits.SelectedItem = 8;
        }
        private void BindSPStopBits()
        {
            cbSPStopBits.Items.Add("1");
            cbSPStopBits.Items.Add("1.5");
            cbSPStopBits.Items.Add("2");
            cbSPStopBits.SelectedItem = "1";
        }
        private void BindEthProtocol()
        {
            cbEthProtocol.Items.Add("TCP Server");
            cbEthProtocol.Items.Add("TCP Client");
            cbEthProtocol.SelectedIndex = 0;
        }
        private void BindEthIP()
        {
            txtEthIP.Text = "127.0.0.1";
        }
        private void BindEthPort()
        {
            txtEthPort.Text = "8081";
        }

        private void Enter(object sender, RoutedEventArgs e)  // 确定按钮
        {
            switch ((CommType)tcType.SelectedIndex)
            {
                case CommType.COMM_TYPE_SERIAL_PORT:
                    if (cbSPPort.SelectedValue == null)
                    {
                        MsgErr.Show("端口不能为空!");
                        return;
                    }
                    if (cbSPBaudRate.SelectedValue == null)
                    {
                        MsgErr.Show("波特率不能为空!");
                        return;
                    }
                    if (cbSPParity.SelectedValue == null)
                    {
                        MsgErr.Show("校验位不能为空!");
                        return;
                    }
                    if (cbSPDataBits.SelectedValue == null)
                    {
                        MsgErr.Show("数据位不能为空!");
                        return;
                    }
                    if (cbSPStopBits.SelectedValue == null)
                    {
                        MsgErr.Show("停止位不能为空!");
                        return;
                    }
                    break;
                case CommType.COMM_TYPE_ETHERNET:
                    if (cbEthProtocol.SelectedValue == null)
                    {
                        MsgErr.Show("协议类型不能为空!");
                        return;
                    }
                    if (txtEthIP.Text == null)
                    {
                        MsgErr.Show("IP地址不能为空!");
                        return;
                    }
                    if (txtEthPort.Text == null)
                    {
                        MsgErr.Show("端口号不能为空!");
                        return;
                    }
                    break;
            }
            IsOK = true;
            _CommType = (CommType)tcType.SelectedIndex;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Test(object sender, RoutedEventArgs e)
        {

        }

        private void Reset(object sender, RoutedEventArgs e)
        {

        }
    }
}
