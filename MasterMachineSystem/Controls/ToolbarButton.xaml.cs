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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MasterMachineSystem.Controls
{
    /// <summary>
    /// ToolbarButton.xaml 的交互逻辑
    /// </summary>
    public partial class ToolbarButton : UserControl
    {
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty;

        static ToolbarButton()
        {
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
            ImageSourceProperty = DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(ToolbarButton), metadata);
        }
    }
}
