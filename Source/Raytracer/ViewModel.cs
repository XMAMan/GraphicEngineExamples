using GraphicMinimal;
using GraphicPanels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace Raytracer
{
    class ViewModel : ReactiveObject
    {
        private GraphicPanel3D panel;
        private Panel3DProgressText progressText;

        private System.Windows.Threading.DispatcherTimer timer;
        private string DataDirectory = @"..\..\..\..\..\Data\";
        private DateTime startTime;
        private Exception lastError = null;

        [Reactive] public float Progress { get; set; } = 0;
        [Reactive] public bool IsRendering { get; set; } = false;
        [Reactive] public Mode3D SelectedRenderMode { get; set; } = Mode3D.BidirectionalPathTracing;
        public IEnumerable<Mode3D> RenderModeValues
        {
            get
            {
                return new List<Mode3D>() 
                { 
                    Mode3D.OpenGL_Version_3_0,
                    Mode3D.RaytracerWithPointLights,
                    Mode3D.Raytracer, 
                    Mode3D.BidirectionalPathTracing,
                    Mode3D.FullBidirectionalPathTracing,
                    Mode3D.VertexConnectionMerging,
                    Mode3D.RadiosityHemicube,
                    Mode3D.RadiositySolidAngle
                };
            }
        }
        [Reactive] public int SampleCount { get; set; } = 1;
        [Reactive] public string OutputText { get; set; } = "";
        [Reactive] public bool UseEnvironmentLight { get; set; } = false;
        public ReactiveCommand<Unit, Unit> StartRenderingClick { get; private set; }

        public ViewModel(GraphicPanel3D panel)
        {
            this.panel = panel;
            this.progressText = new Panel3DProgressText(panel);
            this.panel.Mode = this.SelectedRenderMode;

            this.StartRenderingClick = ReactiveCommand.Create(() =>
            {
                this.startTime = DateTime.Now;
                this.panel.Mode = this.SelectedRenderMode;
                this.panel.GlobalSettings.RaytracerRenderMode = RaytracerRenderMode.SmallBoxes;
                this.panel.GlobalSettings.SamplingCount = this.SampleCount;
                this.lastError = null;

                if (this.panel.Mode == Mode3D.RadiosityHemicube || this.panel.Mode == Mode3D.RadiositySolidAngle)
                {
                    this.panel.GlobalSettings.SamplingCount = 1;
                }

                if (this.panel.IsRaytracingNow)
                {
                    this.panel.StopRaytracing();
                }

                if (GraphicPanel3D.IsRasterizerMode(this.panel.Mode))
                {
                    this.panel.DrawAndFlip();
                }else
                {
                    this.panel.StartRaytracing(this.panel.Width, this.panel.Height, (result) =>
                    {
                        //result.Bitmap.Save(DataDirectory + "Result.bmp");
                    }, (error) => { this.lastError = error; });
                }               
            });

            //Update the 3D scene when the environment light is changed
            this.WhenAnyValue(x => x.UseEnvironmentLight).Subscribe(_ =>
            {
                Add3DObjects();
            });
            

            Add3DObjects();

            this.timer = new System.Windows.Threading.DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 30);//30 ms
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Add3DObjects()
        {
            panel.RemoveAllObjekts();
            panel.AddWaveFrontFileAndSplit(DataDirectory + "3DScene.obj", false, new ObjectPropertys() 
            { 
                SpecularHighlightPowExponent = 20, 
                NormalInterpolation = InterpolationMode.Flat, 
                Albedo = 0.5f 
            });

            //Add Glas-Sphere
            panel.AddSphere(0.25f, 6, 6, new ObjectPropertys()
            {
                Position = new Vector3D(5, 0.5f, -2),
                BrdfModel = BrdfModel.TextureGlass,
                ShowFromTwoSides = true,
                RefractionIndex = 1.5f,
                TextureFile = "#FFFFFF"
            });

            Vector3D cameraStart = panel.GetObjectByNameStartsWith("CameraStart").Position;
            Vector3D cameraRichtung = Vector3D.Normalize(panel.GetObjectByNameStartsWith("CameraEnd").Position - cameraStart);
            panel.RemoveObjectStartsWith("CameraStart");
            panel.RemoveObjectStartsWith("CameraEnd");

            panel.GetObjectByNameStartsWith("LightSource").TextureFile = "#FFFFFF";
            panel.GetObjectByNameStartsWith("LightSource").RaytracingLightSource = new DiffuseSurfaceLightDescription() 
            { 
                Emission = 1000 
            };
            panel.GetObjectByNameStartsWith("LightSource").RasterizerLightSource = new RasterizerLightSourceDescription() 
            { 
                SpotDirection = new Vector3D(0, -1, 0), 
                SpotCutoff = 180.0f, 
                SpotExponent = 1, 
                ConstantAttenuation = 1.5f,
            };

            if (this.UseEnvironmentLight)
            {
                panel.RemoveObjectStartsWith("LightSource");
                panel.AddSphere(1, 10, 10, new ObjectPropertys()
                {
                    Position = new Vector3D(0, -5, 0),
                    TextureFile = DataDirectory + "wide_street_01_1k.hdr", //https://hdrihaven.com/hdri/?h=wide_street_01
                    RaytracingLightSource = new EnvironmentLightDescription() { Emission = 1, Rotate = 0.5f }
                });
            }
           

            panel.GetObjectByNameStartsWith("Torus").TextureFile = "#009999";            
            panel.GetObjectByNameStartsWith("Torus").NormalInterpolation = InterpolationMode.Smooth;

            panel.GetObjectByNameStartsWith("Cone").TextureFile = "#005500";
            panel.GetObjectByNameStartsWith("Icosphere").TextureFile = "#000099";
            panel.GetObjectByNameStartsWith("Ground").TextureFile = "#444444";
            panel.GetObjectByNameStartsWith("Stick").TextureFile = "#999900";

            panel.GetObjectByNameStartsWith("Ground").TextureFile = DataDirectory + "Decal.bmp";
            panel.GetObjectByNameStartsWith("Ground").Color.As<ColorFromTexture>().TextureMatrix = Matrix3x3.Scale(1.5f, 1.5f);

            panel.GlobalSettings.BackgroundImage = "#FFFFFF";
            panel.GlobalSettings.Camera = new Camera(cameraStart, cameraRichtung, 30);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            this.IsRendering = this.panel.IsRaytracingNow;
            this.Progress = this.panel.ProgressPercent;
            string progress = this.progressText.GetProgressText();
            if (progress != null)
            {
                this.OutputText = progress;
            }

            if (this.lastError != null)
            {
                this.OutputText = this.lastError.Message;
            }

            if (GraphicPanel3D.IsRasterizerMode(this.panel.Mode))
            {
                this.panel.DrawAndFlip();
            }
        }
    }
}
