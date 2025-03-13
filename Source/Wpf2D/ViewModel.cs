using GraphicMinimal;
using GraphicPanels;

namespace Wpf2D
{
    class ViewModel
    {
        private GraphicPanel2D panel;

        private System.Windows.Threading.DispatcherTimer timer;
        private string DataDirectory = @"..\..\..\..\..\Data\";

        private Vector2D mousePosition = new Vector2D(0, 0);
        private Vector2D ballPosition = new Vector2D(100, 100);
        private Vector2D ballSpeed = new Vector2D(100, 10);
        private int ballRadius = 10;

        public ViewModel(GraphicPanel2D panel)
        {
            this.panel = panel;

            this.panel.Mode = Mode2D.OpenGL_Version_3_0;
            this.panel.MouseClick += GraphicPanel2D_MouseClick;

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 30);//30 ms
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void GraphicPanel2D_MouseClick(object sender, MouseEventArgs e)
        {
            this.mousePosition = new Vector2D(e.X, e.Y);
        }

        public void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return; //So verhindere ich, dass bei gedrückter Taste der Handler mehrmals aufgerufen wird

            if (e.Key == System.Windows.Input.Key.D1) this.panel.Mode = GraphicPanels.Mode2D.OpenGL_Version_1_0;
            if (e.Key == System.Windows.Input.Key.D2) this.panel.Mode = GraphicPanels.Mode2D.OpenGL_Version_3_0;
            if (e.Key == System.Windows.Input.Key.D3) this.panel.Mode = GraphicPanels.Mode2D.Direct3D_11;
            if (e.Key == System.Windows.Input.Key.D4) this.panel.Mode = GraphicPanels.Mode2D.CPU;
        }

        public void HandleKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return;

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            MoveBall((float)(this.timer.Interval.TotalMilliseconds / 1000.0f));
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
    }
}
