using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MasterMachineSystem.Module
{
    public static class MsgErr
    {
        public static void Show(string ErrorString)
        {
            MessageBox.Show(ErrorString, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
