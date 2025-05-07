using GraphicPanels;
using GraphicPanelWpf;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf2D_Example2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Note: The following must be added to the Wpf2D_Example2.csproj file to enable use of the WinForm-GraphicPanel2D under WPF:
            //<UseWindowsForms>true</UseWindowsForms>
            var panel = new GraphicPanel2D() { Width = 100, Height = 100, Mode = Mode2D.OpenGL_Version_3_0 };
            this.graphicControlBorder.Child = new GraphicControl(panel); //View and ViewModel both know the GraphicPanel2D to display it

            this.DataContext = new ViewModel(panel);

        }
    }
}