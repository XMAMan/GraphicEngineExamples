using GraphicMinimal;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinForm2D
{
    public partial class Form1 : Form
    {
        private string DataDirectory = @"..\..\..\..\Data\";

        private Vector2D mousePosition = new Vector2D(0, 0);
        private Vector2D ballPosition = new Vector2D(100, 100);
        private Vector2D ballSpeed = new Vector2D(100, 10);
        private int ballRadius = 10;

        public Form1()
        {
            InitializeComponent();

            this.panel.Mode = GraphicPanels.Mode2D.Direct3D_11;

            this.panel.MouseClick += new MouseEventHandler(GraphicPanel2D_MouseClick);


            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveBall(this.timer1.Interval / 1000.0f);
            Draw();
        }

        private void Draw()
        {
            this.panel.ClearScreen(Color.AliceBlue);

            this.panel.DrawRectangle(new Pen(Color.Black, 2), 0, 0, this.panel.Width, this.panel.Height);

            this.panel.DrawFillCircle(Color.Green, this.ballPosition, ballRadius);
            this.panel.DrawCircle(Pens.Red, this.mousePosition, 10);

            this.panel.DrawFillRectangle(DataDirectory + "Face_04.png", 10, 10, 300, 200, true, Color.White);

            this.panel.DrawString(10, 10, Color.Black, 15, "Left mouse click for placing red circle");
            this.panel.DrawString(10, 30, Color.Black, 15, "Press 1,2,3 or 4 for switching mode. Current mode: " + this.panel.Mode);


            this.panel.FlipBuffer();
        }

        private void MoveBall(float time)
        {
           this.ballPosition += this.ballSpeed * time;
           
           if (this.ballPosition.X - ballRadius < 0 || this.ballPosition.X + ballRadius > this.panel.Width)
           {
               this.ballSpeed.X *= -1;
           }

           if (this.ballPosition.Y - ballRadius < 0 || this.ballPosition.Y + ballRadius > this.panel.Height)
           {
             this.ballSpeed.Y *= -1;
           }
        }

        private void GraphicPanel2D_MouseClick(object sender, MouseEventArgs e)
        {
            this.mousePosition = new Vector2D(e.X, e.Y);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.D1) this.panel.Mode = GraphicPanels.Mode2D.OpenGL_Version_1_0;
            if (keyData == Keys.D2) this.panel.Mode = GraphicPanels.Mode2D.OpenGL_Version_3_0;
            if (keyData == Keys.D3) this.panel.Mode = GraphicPanels.Mode2D.Direct3D_11;
            if (keyData == Keys.D4) this.panel.Mode = GraphicPanels.Mode2D.CPU;

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
