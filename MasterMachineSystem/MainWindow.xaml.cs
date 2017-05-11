using MasterMachineSystem.Module;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Threading;

namespace MasterMachineSystem
{
    using TestPlans = List<TestPlan>;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private enum PROJECT_STATUS  // 项目状态
        {
            None,  // 无项目
            Empty,  // 空项目
            PlanOnly,  // 有测试计划的项目
            ConfigOnly,  // 有配置信息的项目
            Entire,  // 有测试计划和配置信息的项目
        }
        private enum PLAN_STATUS
        {
            None,  // 无计划
            Empty,  // 空计划
            Nonempty,  // 非空计划
        }
        private enum CONNECT_STATUS  // 连接状态
        {
            Connected,  // 已连接
            Connecting,  //正在连接
            Disconnected,  // 断开
            Listening,  // 正在监听
        }

        // 全局变量
        private TestProject _TestProject;  // 全局测试项目
        private TestPlans _TestPlans = new TestPlans();  // 全局测试计划组
        private TestPlan _TestPlan;  // 当前测试计划
        private int FrameDataLength;  // 帧数据长度
        Thread _CheckStatusThread;  // 检查状态线程

        private PROJECT_STATUS _Project_Status;  // 全局项目状态
        private PLAN_STATUS _Plan_Status;  // 全局计划状态
        private CONNECT_STATUS _Connect_Status;  // 全局连接状态

        public MainWindow()
        {
            InitializeComponent();
            if (!ConfigInitialize())
            {
                // 初始化失败，程序退出
                MsgErr.Show("配置文件初始化失败!");
                Close();
            }
            if (!BootInitialize())
            {
                // 初始化失败，程序退出
                MsgErr.Show("程序初始化失败!");
                Close();
            }
        }
        private void CheckStatus()  // 检查当前状态
        {
            while (true)
            {
                if (_TestProject._Comm.IsOpen())
                {
                    _Connect_Status = CONNECT_STATUS.Connected;
                    Dispatcher.Invoke(new Action(() => { ChangeUI(CONNECT_STATUS.Connected); })); 
                    return;
                }
            }
        }
        private void ChangeUI(object _Status = null)  // 根据当前状态改变UI
        {
            if (_Status != null)
            {
                if (_Status is PROJECT_STATUS)
                    _Project_Status = (PROJECT_STATUS)_Status;
                else if (_Status is PLAN_STATUS)
                    _Plan_Status = (PLAN_STATUS)_Status;
                else if (_Status is CONNECT_STATUS)
                    _Connect_Status = (CONNECT_STATUS)_Status;
            }
            // 工具栏按钮
            btnCreateProject.IsEnabled = true;  // 新建项目
            btnOpenProject.IsEnabled = true;  // 打开项目
            if (_Project_Status != PROJECT_STATUS.None && _Connect_Status == CONNECT_STATUS.Disconnected)  // 配置
            {
                btnConfig.IsEnabled = true;
            }
            else
            {
                btnConfig.IsEnabled = false;
            }
            if (_Project_Status == PROJECT_STATUS.ConfigOnly || _Project_Status == PROJECT_STATUS.Entire)  // 连接
            {
                if (_Connect_Status == CONNECT_STATUS.Disconnected)
                {
                    btnConnect.IsEnabled = true;
                    btnConnect.ToolTip = "连接";
                    imgBtnConnect.Source = new BitmapImage(new Uri("icon/toolbar_connect.ico", UriKind.Relative));
                }
                else if (_Connect_Status == CONNECT_STATUS.Connected || _Connect_Status == CONNECT_STATUS.Listening)
                {
                    btnConnect.IsEnabled = true;
                    btnConnect.ToolTip = "断开连接";
                    imgBtnConnect.Source = new BitmapImage(new Uri("icon/toolbar_disconnect.ico", UriKind.Relative));
                }
            }
            else
            {
                btnConnect.IsEnabled = false;
                btnConnect.ToolTip = "连接";
                imgBtnConnect.Source = new BitmapImage(new Uri("icon/toolbar_connect.ico", UriKind.Relative));
            }
            if (_Project_Status == PROJECT_STATUS.Entire)  // 监听
            {
                if (_Connect_Status == CONNECT_STATUS.Disconnected)
                {
                    btnListen.IsEnabled = false;
                    btnListen.ToolTip = "开始监听";
                    imgBtnListen.Source = new BitmapImage(new Uri("icon/toolbar_start_listening.ico", UriKind.Relative));
                }
                else if (_Connect_Status == CONNECT_STATUS.Connected)
                {
                    btnListen.IsEnabled = true;
                    btnListen.ToolTip = "开始监听";
                    imgBtnListen.Source = new BitmapImage(new Uri("icon/toolbar_start_listening.ico", UriKind.Relative));
                }
                else if (_Connect_Status == CONNECT_STATUS.Listening)
                {
                    btnListen.IsEnabled = true;
                    btnListen.ToolTip = "结束监听";
                    imgBtnListen.Source = new BitmapImage(new Uri("icon/toolbar_stop_listening.ico", UriKind.Relative));
                }
            }
            else
            {
                btnListen.IsEnabled = false;
                btnListen.ToolTip = "开始监听";
                imgBtnListen.Source = new BitmapImage(new Uri("icon/toolbar_start_listening.ico", UriKind.Relative));
            }
            if (_Project_Status == PROJECT_STATUS.PlanOnly || _Project_Status == PROJECT_STATUS.Entire)  // 图表
            {
                btnChart.IsEnabled = true;
            }
            else
            {
                btnChart.IsEnabled = false;
            }
            if (_Project_Status == PROJECT_STATUS.PlanOnly || _Project_Status == PROJECT_STATUS.Entire)  // 报告
            {
                btnReport.IsEnabled = true;
            }
            else
            {
                btnReport.IsEnabled = false;
            }
            if (_Project_Status == PROJECT_STATUS.PlanOnly || _Project_Status == PROJECT_STATUS.Entire)  // 清空计划
            {
                btnClear.IsEnabled = true;
            }
            else
            {
                btnClear.IsEnabled = false;
            }
            if (_Project_Status != PROJECT_STATUS.None)  // 导入
            {
                btnImport.IsEnabled = true;
            }
            else
            {
                btnImport.IsEnabled = false;
            }
            if (_Project_Status == PROJECT_STATUS.PlanOnly || _Project_Status == PROJECT_STATUS.Entire)  // 导出
            {
                btnExport.IsEnabled = true;
            }
            else
            {
                btnExport.IsEnabled = false;
            }
            btnSetting.IsEnabled = true;  // 设置
            btnClose.IsEnabled = true;  // 退出

            // 测试计划栏
            if (_Project_Status != PROJECT_STATUS.None)  // 测试计划下拉框
            {
                cbPlanList.IsEnabled = true;
            }
            if (_Project_Status != PROJECT_STATUS.None)  // 添加测试计划
            {
                btnPlanAdd.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.PlanOnly || _Project_Status != PROJECT_STATUS.Entire) && 
                _Plan_Status != PLAN_STATUS.None)  // 删除测试计划
            {
                btnPlanDelete.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.PlanOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                _Plan_Status != PLAN_STATUS.None)  // 查看测试计划
            {
                btnPlanView.IsEnabled = true;
            }
            if (_Project_Status != PROJECT_STATUS.None)  // 导入测试计划
            {
                btnPlanImport.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.PlanOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                _Plan_Status != PLAN_STATUS.None)  // 导出测试计划
            {
                btnPlanExport.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.PlanOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                _Plan_Status != PLAN_STATUS.None)  // 测试计划生成图表
            {
                btnPlanBuildChart.IsEnabled = true;
            }

            // 数据帧栏
            if ((_Project_Status != PROJECT_STATUS.PlanOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                _Plan_Status == PLAN_STATUS.Nonempty)  // 数据帧
            {
                lstReceiveFrameList.IsEnabled = true;
            }

            // 简单数据交互栏
            if ((_Project_Status != PROJECT_STATUS.ConfigOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                _Connect_Status == CONNECT_STATUS.Listening && chkSimpleReceived.IsChecked == true)  // 简单数据接收
            {
                txtSimpleReceived.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.ConfigOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                (_Connect_Status == CONNECT_STATUS.Connected || _Connect_Status == CONNECT_STATUS.Listening))  // 简单数据接收使能
            {
                chkSimpleReceived.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.ConfigOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                (_Connect_Status == CONNECT_STATUS.Connected || _Connect_Status == CONNECT_STATUS.Listening))  // 简单数据发送框
            {
                txtSimpleSend.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.ConfigOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                (_Connect_Status == CONNECT_STATUS.Connected || _Connect_Status == CONNECT_STATUS.Listening))  // 简单数据发送按钮
            {
                btnSimpleSend.IsEnabled = true;
            }
            if ((_Project_Status != PROJECT_STATUS.ConfigOnly || _Project_Status != PROJECT_STATUS.Entire) &&
                (_Connect_Status == CONNECT_STATUS.Connected || _Connect_Status == CONNECT_STATUS.Listening))  // 简单数据发送设置
            {
                btnSimpleSendConfig.IsEnabled = true;
            }
        }
        private bool ConfigInitialize()
        {
            string ConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            INI _INI = new INI(ConfigFilePath);
            // 如果没有系统配置文件，则生成默认配置文件
            if (!File.Exists(ConfigFilePath))
            {
                _INI.WriteDefault();
            }
            // 读取系统配置文件
            
            FrameDataLength = int.Parse(_INI.Read("FRAME", "LENGTH"));
            return true;
        }
        private bool BootInitialize()  // 启动初始化
        {
            // 按键初始化
            _Project_Status = PROJECT_STATUS.None;
            _Plan_Status = PLAN_STATUS.None;
            _Connect_Status = CONNECT_STATUS.Disconnected;
            ChangeUI();
            // 计划列表初始化
            cbPlanList.SelectedValuePath = "_GUID";
            cbPlanList.DisplayMemberPath = "_Name";

            return true;
        }
        private bool ProjectInitialize()  // 项目初始化
        {
            if (_TestProject == null)
            {
                MsgErr.Show("初始化项目失败，项目文件为空!");
                return false;
            }
            Title = _TestProject._Name;
            
            return true;
        }
        public void ReceivedMessage(byte[] readBuffer, int offset, int length)  // 接收线程
        {
            _TestProject._Comm.KeepReading = false;
            // 在简单接收中显示
            string readString = Encoding.Default.GetString(readBuffer, offset, length);
            Dispatcher.Invoke(new Action(() => { txtSimpleReceived.Text += readString; }));
            _TestProject._Comm.KeepReading = true;
        }
        private void InsertPlan(TestPlan Item)  // 插入一条计划
        {
            _TestPlans.Add(Item);
            cbPlanList.Items.Add(Item);
            cbPlanList.SelectedItem = Item;
        }

        private void RemovePlan(TestPlan Item)  // 移除一条计划
        {
            _TestPlans.Remove(Item);
            cbPlanList.Items.Remove(Item);
            Item = null;
        }
        private void SelectPlan(object sender, SelectionChangedEventArgs e)
        {
            _TestPlan = (TestPlan)cbPlanList.SelectedItem;
            if (_TestPlan == null)
                ChangeUI(PLAN_STATUS.None);
            else if (_TestPlan.ItemCount() == 0)
                ChangeUI(PLAN_STATUS.Empty);
            else
                ChangeUI(PLAN_STATUS.Nonempty);
            if (_Project_Status == PROJECT_STATUS.ConfigOnly || _Project_Status == PROJECT_STATUS.Entire)
            {
                if (_TestPlans.Count == 0)
                {
                    ChangeUI(PROJECT_STATUS.ConfigOnly);
                }
                else
                {
                    ChangeUI(PROJECT_STATUS.Entire);
                }
            }
            else
            {
                if (_TestPlans.Count == 0)
                {
                    ChangeUI(PROJECT_STATUS.Empty);
                }
                else
                {
                    ChangeUI(PROJECT_STATUS.PlanOnly);
                }
            }
        }


        private void CreateProject(object sender, RoutedEventArgs e)  // 新建项目
        {
            CreateProjectWindow _CreateProjectWindow = new CreateProjectWindow();
            _CreateProjectWindow.ShowDialog();
            if (_CreateProjectWindow.IsOK)
            {
                _TestProject = new TestProject(_CreateProjectWindow.txtProjectName.Text);
                if (ProjectInitialize())
                {
                    ChangeUI(PROJECT_STATUS.Empty);
                }
            }
        }
        private void OpenProject(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow _CreateProjectWindow = new CreateProjectWindow();
            _CreateProjectWindow.ShowDialog();
        }
        private void Config(object sender, RoutedEventArgs e)  // 配置连接
        {
            ConfigWindow _ConfigWindow = new ConfigWindow();
            _ConfigWindow.ShowDialog();
            if (_ConfigWindow.IsOK)
            {
                switch (_ConfigWindow._CommType)
                {
                    case CommType.COMM_TYPE_SERIAL_PORT:
                        // 通信接口串口实例化
                        _TestProject._Comm = new SerialPortComm(
                            _ConfigWindow.cbSPPort.SelectedValue.ToString(),
                            Convert.ToInt32(_ConfigWindow.cbSPBaudRate.SelectedValue),
                            _ConfigWindow.cbSPParity.SelectedValue.ToString(),
                            Convert.ToChar(_ConfigWindow.cbSPDataBits.SelectedValue),
                            _ConfigWindow.cbSPStopBits.SelectedValue.ToString());
                        break;
                    case CommType.COMM_TYPE_ETHERNET:
                        // 通信接口以太网实例化
                        _TestProject._Comm = new EthernetComm(
                            _ConfigWindow.txtEthIP.Text, 
                            int.Parse(_ConfigWindow.txtEthPort.Text), 
                            (EthernetType)_ConfigWindow.cbEthProtocol.SelectedIndex);
                        break;
                    case CommType.COMM_TYPE_USB:
                        // 通信接口USB实例化
                        break;
                }
                if (_Project_Status == PROJECT_STATUS.Empty)
                {
                    ChangeUI(PROJECT_STATUS.ConfigOnly);
                }
                else
                {
                    ChangeUI(PROJECT_STATUS.Entire);
                }
                
            }
        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            if (_Connect_Status == CONNECT_STATUS.Disconnected)  // 未连接状态进行连接
            {
                ChangeUI(CONNECT_STATUS.Connecting);
                _TestProject._Comm.Open();
                // 启动状态检查线程
                _CheckStatusThread = new Thread(new ThreadStart(CheckStatus));
                _CheckStatusThread.Start();
                //if (_TestProject._Comm.IsOpen())
                //{
                //    ChangeUI(CONNECT_STATUS.Connected);
                //}
                //else
                //{
                //    ChangeUI(CONNECT_STATUS.Disconnected);
                //}
            }
            else  // 已连接状态断开连接
            {
                //if (_Connect_Status == CONNECT_STATUS.Listening)  // 监听状态结束监听
                //{
                //    _TestProject._Comm.DataReceived -= new Comm.EventHandle(ReceivedMessage);
                //    _TestProject._Comm.StopReading();
                //}
                _TestProject._Comm.Close();
                ChangeUI(CONNECT_STATUS.Disconnected);
            }
        }
        private void Listen(object sender, RoutedEventArgs e)
        {
            if (_Connect_Status == CONNECT_STATUS.Connected)
            {
                _TestProject._Comm.StartReading(FrameDataLength);
                _TestProject._Comm.DataReceived += new Comm.EventHandle(ReceivedMessage);
                ChangeUI(CONNECT_STATUS.Listening);
            }
            else if (_Connect_Status == CONNECT_STATUS.Listening)
            {
                _TestProject._Comm.DataReceived -= new Comm.EventHandle(ReceivedMessage);
                _TestProject._Comm.StopReading();
                ChangeUI(CONNECT_STATUS.Connected);
            }
            else
            {
                MsgErr.Show("未建立连接，无法监听!");
                return;
            }
        }
        private void PlanAdd(object sender, RoutedEventArgs e)  // 打开添加测试计划窗口
        {
            if (_TestProject == null)
            {
                MsgErr.Show("缺少测试项目!");
                return;
            }
            PlanAddWindow _PlanAddWindow = new PlanAddWindow();
            _PlanAddWindow.ShowDialog();
            if (_PlanAddWindow.IsOK)
            {
                TestPlan _NewTestPlan = new TestPlan(_PlanAddWindow.txtPlanName.Text);
                InsertPlan(_NewTestPlan);
            }
        }
        private void PlanDelete(object sender, RoutedEventArgs e)  // 删除测试计划
        {
            if (_TestProject == null)
            {
                MsgErr.Show("缺少测试项目!");
                return;
            }
            if (_TestPlan == null)
            {
                MsgErr.Show("缺少测试计划!");
                return;
            }
            RemovePlan(_TestPlan);
        }

        private void SimpleSend(object sender, RoutedEventArgs e)
        {
            if (_Project_Status != PROJECT_STATUS.ConfigOnly && _Project_Status != PROJECT_STATUS.Entire)
                return;
            if (_Connect_Status != CONNECT_STATUS.Connected && _Connect_Status != CONNECT_STATUS.Listening)
                return;
            if (txtSimpleSend.Text.Length == 0)
                return;
            byte[] SendBuffer = Encoding.Default.GetBytes(txtSimpleSend.Text);
            _TestProject._Comm.Send(SendBuffer, 0, SendBuffer.Length);
        }


    }
}
