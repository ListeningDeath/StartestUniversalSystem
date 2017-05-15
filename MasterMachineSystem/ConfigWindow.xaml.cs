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
        private Ini _InitializationFile;  // 全局配置文件
        public Comm _Comm;  // 全局通信接口
        public bool IsOK = false;
        public CommType _CommType;

        public ConfigWindow(Ini _IniFile, Comm _CommInfo)  // 无通信接口从配置文件读入,有通信接口直接传入引用
        {
            InitializeComponent();
            _InitializationFile = _IniFile;
            _Comm = _CommInfo;
        }
        private void BindSPPort()  // 获取系统开放的串口号，根据配置文件设置当前值
        {
            foreach (string PortName in SerialPort.GetPortNames())
            {
                cbSPPort.Items.Add(PortName);
            }
            cbSPPort.LoadFromIni(_InitializationFile, "CONNECT", "S_DEFAULT_PORT");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_SERIAL_PORT)
            {
                cbSPPort.SelectedItem = ((SerialPortComm)_Comm)._PortName;
            }
        }
        private void BindSPBaudRate()  // 获取配置文件的波特率及当前值
        {
            cbSPBaudRate.LoadFromIni(_InitializationFile, "CONNECT", "S_BAUDRATE", "S_DEFAULT_BAUDRATE");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_SERIAL_PORT)
            {
                cbSPBaudRate.SelectedItem = ((SerialPortComm)_Comm)._BaudRate.ToString();
            }
            //cbSPBaudRate.Items.Add(300);
            //cbSPBaudRate.Items.Add(600);
            //cbSPBaudRate.Items.Add(1200);
            //cbSPBaudRate.Items.Add(2400);
            //cbSPBaudRate.Items.Add(4800);
            //cbSPBaudRate.Items.Add(9600);
            //cbSPBaudRate.Items.Add(19200);
            //cbSPBaudRate.Items.Add(38400);
            //cbSPBaudRate.Items.Add(43000);
            //cbSPBaudRate.Items.Add(56000);
            //cbSPBaudRate.Items.Add(57600);
            //cbSPBaudRate.Items.Add(115200);
            //cbSPBaudRate.SelectedItem = 9600;
        }
        private void BindSPParity()  // 获取配置文件的校验位及当前值
        {
            cbSPParity.LoadFromIni(_InitializationFile, "CONNECT", "S_PARITY", "S_DEFAULT_PARITY");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_SERIAL_PORT)
            {
                cbSPParity.SelectedItem = ((SerialPortComm)_Comm)._Parity;
            }
            //cbSPParity.Items.Add("NONE");
            //cbSPParity.Items.Add("ODD");
            //cbSPParity.Items.Add("EVEN");
            //cbSPParity.Items.Add("MARK");
            //cbSPParity.Items.Add("SPACE");
            //cbSPParity.SelectedItem = "NONE";
        }
        private void BindSPDataBits()
        {
            cbSPDataBits.LoadFromIni(_InitializationFile, "CONNECT", "S_DATABITS", "S_DEFAULT_DATABITS");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_SERIAL_PORT)
            {
                cbSPDataBits.SelectedItem = ((SerialPortComm)_Comm)._DataBits.ToString();
            }
            //cbSPDataBits.Items.Add(5);
            //cbSPDataBits.Items.Add(6);
            //cbSPDataBits.Items.Add(7);
            //cbSPDataBits.Items.Add(8);
            //cbSPDataBits.SelectedItem = 8;
        }
        private void BindSPStopBits()
        {
            cbSPStopBits.LoadFromIni(_InitializationFile, "CONNECT", "S_STOPBITS", "S_DEFAULT_STOPBITS");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_SERIAL_PORT)
            {
                cbSPStopBits.SelectedItem = ((SerialPortComm)_Comm)._StopBits;
            }
            //cbSPStopBits.Items.Add("1");
            //cbSPStopBits.Items.Add("1.5");
            //cbSPStopBits.Items.Add("2");
            //cbSPStopBits.SelectedItem = "1";
        }
        private void BindEthProtocol()
        {
            cbEthProtocol.Items.Add("TCP Server");
            cbEthProtocol.Items.Add("TCP Client");
            cbEthProtocol.LoadFromIni(_InitializationFile, "CONNECT", "E_DEFAULT_PROTOCOL");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_ETHERNET)
            {
                cbEthProtocol.SelectedIndex = (int)((EthernetComm)_Comm)._EthernetType;
            }
            //cbEthProtocol.SelectedIndex = 0;
        }
        private void BindEthIP()
        {
            txtEthIP.LoadFromIni(_InitializationFile, "CONNECT", "E_IP");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_ETHERNET)
            {
                txtEthIP.Text = ((EthernetComm)_Comm)._IPAddress;
            }
            //txtEthIP.Text = "127.0.0.1";
        }
        private void BindEthPort()
        {
            txtEthPort.LoadFromIni(_InitializationFile, "CONNECT", "E_PORT");
            if (_Comm != null && _Comm.MyType == CommType.COMM_TYPE_ETHERNET)
            {
                txtEthPort.Text = ((EthernetComm)_Comm)._Port.ToString();
            }
            //txtEthPort.Text = "8081";
        }
        private void Load(object sender, RoutedEventArgs e)  // 页面载入
        {
            if (_Comm == null)  // 没有通信接口，从ini读取配置信息
            {
                string DefaultConnectType = _InitializationFile.Read("CONNECT", "TYPE");
                if (DefaultConnectType != "")
                    DefaultConnectType = DefaultConnectType.Substring(0, 1);
                switch (DefaultConnectType)  // 获取默认连接方式
                {
                    case "S":
                    default:
                        _CommType = CommType.COMM_TYPE_SERIAL_PORT;
                        tcType.SelectedIndex = 0;
                        break;
                    case "E":
                        _CommType = CommType.COMM_TYPE_ETHERNET;
                        tcType.SelectedIndex = 1;
                        break;
                }
            }
            else  // 存在通信接口直接读取
            {
                 tcType.SelectedIndex = (int)_CommType;
            }
            BindSPPort();
            BindSPBaudRate();
            BindSPParity();
            BindSPDataBits();
            BindSPStopBits();
            BindEthProtocol();
            BindEthIP();
            BindEthPort();
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
            // 根据标签页选择实例化通信接口类型
            switch (_CommType)
            {
                case CommType.COMM_TYPE_SERIAL_PORT:
                    _Comm = new SerialPortComm(cbSPPort.SelectedValue.ToString(),
                            int.Parse(cbSPBaudRate.SelectedValue.ToString()),
                            cbSPParity.SelectedValue.ToString(),
                            ushort.Parse(cbSPDataBits.SelectedValue.ToString()),
                            cbSPStopBits.SelectedValue.ToString());
                    break;
                case CommType.COMM_TYPE_ETHERNET:
                    _Comm = new EthernetComm(txtEthIP.Text,
                            int.Parse(txtEthPort.Text),
                            (EthernetType)cbEthProtocol.SelectedIndex);
                    break;
            }
            _InitializationFile.Write("CONNECT", "TYPE", 
                _CommType == CommType.COMM_TYPE_SERIAL_PORT ? "SERIALPORT" : "ETHERNET");
            cbSPPort.WriteToIni(_InitializationFile, "CONNECT", "", "S_DEFAULT_PORT", true);
            cbSPBaudRate.WriteToIni(_InitializationFile, "CONNECT", "S_BAUDRATE", "S_DEFAULT_BAUDRATE");
            cbSPParity.WriteToIni(_InitializationFile, "CONNECT", "S_PARITY", "S_DEFAULT_PARITY");
            cbSPDataBits.WriteToIni(_InitializationFile, "CONNECT", "S_DATABITS", "S_DEFAULT_DATABITS");
            cbSPStopBits.WriteToIni(_InitializationFile, "CONNECT", "S_STOPBITS", "S_DEFAULT_STOPBITS");
            cbEthProtocol.WriteToIni(_InitializationFile, "CONNECT", "", "E_DEFAULT_PROTOCOL", true);
            txtEthIP.WriteToIni(_InitializationFile, "CONNECT", "E_IP");
            txtEthIP.WriteToIni(_InitializationFile, "CONNECT", "E_PORT");
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
