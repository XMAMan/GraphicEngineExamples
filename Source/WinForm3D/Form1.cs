using GraphicMinimal;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinForm3D
{
    public partial class Form1 : Form
    {
        private string DataDirectory = @"..\..\..\..\Data\";

        private int[] legoIds;

        public Form1()
        {
            InitializeComponent();

            this.panel.Mode = GraphicPanels.Mode3D.OpenGL_Version_3_0;

            Add3DObjects();

            this.timer1.Start();
        }

        private void Add3DObjects()
        {
            panel.RemoveAllObjekts();

            //LegoMan from Wavefront file
            this.legoIds = panel.AddWaveFrontFileAndSplit(DataDirectory + "LegoMan.obj", false, new ObjectPropertys()
            {
                NormalInterpolation = InterpolationMode.Smooth,
                SpecularHighlightPowExponent = 50,
                Size = 0.1f,
            });

            //This is nessessary to rotate all objects around the same point
            panel.SetCenterOfObjectOrigin(this.legoIds);

            //Set texture mode to clamp
            foreach (int id in this.legoIds)
            {
                var prop = this.panel.GetObjectById(id);
                if (prop.Color is ColorFromTexture texture)
                {
                    texture.TextureMode = TextureMode.Clamp;
                }
            }

            //LightSource
            panel.AddSphere(1, 10, 10, new ObjectPropertys()
            {
                Position = new Vector3D(0, 75, 30),
                Orientation = new Vector3D(0, 0, 45),
                Size = 10.0f,
                TextureFile = "#FFFFFF",
                ShowFromTwoSides = true,
                RasterizerLightSource = new RasterizerLightSourceDescription() { SpotDirection = Vector3D.Normalize(new Vector3D(0, 30, 0) - new Vector3D(0, 75, 30)), SpotCutoff = 180.0f, SpotExponent = 1, ConstantAttenuation = 1.1f },
                RaytracingLightSource = new DiffuseSphereLightDescription() { Emission = 2200 }
            });

            panel.GlobalSettings.BackgroundImage = "#FFFFFF";
            panel.GlobalSettings.Camera = new Camera(new Vector3D(0, 5, 10), new Vector3D(0, -0.5f, -1), 45.0f);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Move(this.timer1.Interval / 1000.0f);
            Draw();
        }

        private void Move(float time)
        {
            foreach (int id in this.legoIds)
            {
                this.panel.GetObjectById(id).Orientation.Y += time * 100;
            }
        }

        private void Draw()
        {
            this.panel.DrawWithoutFlip();
            this.panel.DrawString(10, 10, Color.Red, 15, "Press 1,2,3 or 4 for switching mode. Current mode: " + this.panel.Mode);
            this.panel.FlipBuffer();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.D1) this.panel.Mode = GraphicPanels.Mode3D.OpenGL_Version_1_0;
            if (keyData == Keys.D2) this.panel.Mode = GraphicPanels.Mode3D.OpenGL_Version_3_0;
            if (keyData == Keys.D3) this.panel.Mode = GraphicPanels.Mode3D.Direct3D_11;
            if (keyData == Keys.D4) this.panel.Mode = GraphicPanels.Mode3D.CPU;

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
