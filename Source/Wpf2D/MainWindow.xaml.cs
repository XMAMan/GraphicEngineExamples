﻿using GraphicMinimal;
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

namespace Wpf2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //new GlobalObjectPropertys();

            

            //Achtung: In der Wpf2D.csproj mus noch folgendes rein, damit man unter
            //WPF das WinForm-GraphicPanel2D nutzen kann: <UseWindowsForms>true</UseWindowsForms>
            var panel = new GraphicPanel2D() { Width = 100, Height = 100, Mode = Mode2D.OpenGL_Version_3_0 }; //Unter .NET Core kann man leider kein DirectX nutzen
            this.graphicControlBorder.Child = new GraphicControl(panel); //Sowohl die View kennt das GraphicPanel2D um es darstellen zu können

            this.DataContext = new ViewModel(panel); //Das ViewModel kennt das GraphicPanel2D auch, um Zeichenbefehle hinsenden zu können
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);

            window.KeyDown += (this.DataContext as ViewModel).HandleKeyDown;
            window.KeyUp += (this.DataContext as ViewModel).HandleKeyUp;
        }
    }
}