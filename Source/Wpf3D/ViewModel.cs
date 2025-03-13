using GraphicMinimal;
using GraphicPanels;

namespace Wpf3D
{
    class ViewModel
    {
        private GraphicPanel3D panel;

        private System.Windows.Threading.DispatcherTimer timer;
        private string DataDirectory = @"..\..\..\..\..\Data\";

        private int playerId = -1;
        private Vector3D playerDirection = new Vector3D(0, 0, 0);

        public ViewModel(GraphicPanel3D panel)
        {
            this.panel = panel;

            Add3DObjects();

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 30);//30 ms
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Add3DObjects()
        {
            panel.RemoveAllObjekts();

            //Ground
            int groundId = panel.AddSquareXY(1, 1, 1, new ObjectPropertys() 
            { 
                Position = new Vector3D(0, 0, 0), 
                Orientation = new Vector3D(-90, 0, 180), 
                Size = 80, 
                Color = new ColorFromTexture() 
                { 
                    TextureFile = DataDirectory + "Decal.bmp", 
                    TextureMatrix = Matrix3x3.Scale(6, 6) 
                }, 
                ShowFromTwoSides = true, 
                BrdfModel = BrdfModel.Diffus, 
                
            });

            //Use Parallax Mapping for the ground
            var tex = this.panel.GetObjectById(groundId).Color.As<ColorFromTexture>();
            this.panel.GetObjectById(groundId).NormalSource = new NormalFromParallax() 
            { 
                ParallaxMap = tex.TextureFile, 
                TextureMatrix = tex.TextureMatrix, 
                ConvertNormalMapFromColor = true, 
                TexturHeightFactor = 0.04f, 
                IsParallaxEdgeCutoffEnabled = true 
            };


            //Player
            this.playerId = panel.AddCylinder(2, 1, 1, true, 10, new ObjectPropertys() 
            { 
                Position = new Vector3D(0, 2, 0), 
                Orientation = new Vector3D(0, 0, 0), 
                Color = new ColorFromRgb() { Rgb = new Vector3D(1, 0, 0) },
                BrdfModel = BrdfModel.DiffuseAndMirror,
                HasStencilShadow = true,
            });

            panel.AddRing(0.3f, 2, 5, 20, new ObjectPropertys() 
            { 
                Position = new Vector3D(0, 2, 0), 
                Orientation = new Vector3D(0, 0, 45), 
                Size = 1, 
                HasStencilShadow = true, 
                NormalInterpolation = InterpolationMode.Smooth, 
                TextureFile = "#004400", 
                SpecularHighlightPowExponent = 50, 
                ShowFromTwoSides = false 
            });

            //LightSource
            panel.AddSphere(0.1f, 10, 10, new ObjectPropertys() 
            {
                Position = new Vector3D(0, 10, 3),
                Orientation = new Vector3D(0, 0, 45),
                Size = 1.0f,
                TextureFile = "#FFFFFF",
                ShowFromTwoSides = true,
                RasterizerLightSource = new RasterizerLightSourceDescription() { SpotDirection = Vector3D.Normalize(new Vector3D(0, 30, 0) - new Vector3D(0, 75, 30)), SpotCutoff = 180.0f, SpotExponent = 1, ConstantAttenuation = 1.1f },
                RaytracingLightSource = new DiffuseSphereLightDescription() { Emission = 2200}
            });

            panel.GlobalSettings.BackgroundImage = "#FFFFFF";
            panel.GlobalSettings.ShadowsForRasterizer = RasterizerShadowMode.Shadowmap;
            panel.GlobalSettings.Camera = new Camera(new Vector3D(0, 7, 10), new Vector3D(0, -0.5f, -1), 45.0f);

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Draw();
            panel.GetObjectById(playerId).Position += this.playerDirection;
        }

        public void HandleKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return; //So verhindere ich, dass bei gedrückter Taste der Handler mehrmals aufgerufen wird
            e.Handled = true;

            if (e.Key == System.Windows.Input.Key.D1) this.panel.Mode = GraphicPanels.Mode3D.OpenGL_Version_1_0;
            if (e.Key == System.Windows.Input.Key.D2) this.panel.Mode = GraphicPanels.Mode3D.OpenGL_Version_3_0;
            if (e.Key == System.Windows.Input.Key.D3) this.panel.Mode = GraphicPanels.Mode3D.Direct3D_11;
            if (e.Key == System.Windows.Input.Key.D4) this.panel.Mode = GraphicPanels.Mode3D.CPU;

            if (e.Key == System.Windows.Input.Key.W)
            {
                this.playerDirection.Z = -1;
            }
            if (e.Key == System.Windows.Input.Key.S)
            {
                this.playerDirection.Z = +1;
            }
            if (e.Key == System.Windows.Input.Key.A)
            {
                this.playerDirection.X = -1;
            }
            if (e.Key == System.Windows.Input.Key.D)
            {
                this.playerDirection.X = +1;
            }
        }

        public void HandleKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            e.Handled = true;

            if (e.Key == System.Windows.Input.Key.W)
            {
                this.playerDirection.Z = 0;
            }
            if (e.Key == System.Windows.Input.Key.S)
            {
                this.playerDirection.Z = 0;
            }
            if (e.Key == System.Windows.Input.Key.A)
            {
                this.playerDirection.X = 0;
            }
            if (e.Key == System.Windows.Input.Key.D)
            {
                this.playerDirection.X = 0;
            }
        }

        private void Draw()
        {
            this.panel.DrawWithoutFlip();
            this.panel.DrawString(10, 10, Color.Red, 15, "Press W,A,S,D for moving the cylinder");
            this.panel.DrawString(10, 30, Color.Red, 15, "Press 1,2,3 or 4 for switching mode. Current mode: " + this.panel.Mode);
            this.panel.FlipBuffer();
        }
    }
}
