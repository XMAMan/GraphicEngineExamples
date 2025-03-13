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

namespace Wpf3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Achtung: In der Wpf3D.csproj mus noch folgendes rein, damit man unter
            //WPF das WinForm-GraphicPanel3D nutzen kann: <UseWindowsForms>true</UseWindowsForms>
            var panel = new GraphicPanel3D() { Width = 100, Height = 100, Mode = Mode3D.OpenGL_Version_3_0 }; //Unter .NET Core kann man leider kein DirectX nutzen
            this.graphicControlBorder.Child = new GraphicControl(panel); //Sowohl die View kennt das GraphicPanel2D um es darstellen zu können

            this.DataContext = new ViewModel(panel); //Das ViewModel kennt das GraphicPanel2D auch, um Zeichenbefehle hinsenden zu können

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.PreviewKeyDown += (this.DataContext as ViewModel).HandleKeyDown;
            window.PreviewKeyUp += (this.DataContext as ViewModel).HandleKeyUp;
        }
    }
}