using MasterMachineSystem.Module;
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
using MahApps.Metro.Controls;

namespace MasterMachineSystem
{
    /// <summary>
    /// PlanAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlanAddWindow : MetroWindow
    {
        // 全局变量
        public bool IsOK = false;
        public PlanAddWindow()
        {
            InitializeComponent();
            txtPlanName.Text = string.Format("plan_{0}", DateTime.Now.ToString());
        }
        private void Enter(object sender, RoutedEventArgs e)
        {
            if (txtPlanName.Text == "")
            {
                MsgErr.Show("项目名称不能为空!");
                txtPlanName.Focus();
                return;
            }
            IsOK = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
