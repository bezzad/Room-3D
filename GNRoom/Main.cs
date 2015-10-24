using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace GNRoom
{
    public partial class MainForm : Form
    {
        GraphicEngine ge;
        public static bool Paused = false;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            InitializeGraphics ig;
            bool fullScreen = false;

            fullScreen = (MessageBox.Show("Will you to display FullScreen by DirectX?\n\r"+
                "آیا می خواهید پنجره نمایش به حالت تمام صفحه باز شود؟", 
                "DirectX Windowsed", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);

            ig = new InitializeGraphics(!fullScreen);

            ge = new GraphicEngine(ig.getDevice(this), fullScreen);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Paused)
            {
                ge.DrawWorld();
            }
            else
            {
                draw_text(e);
            }
            this.Invalidate(true);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if ((int)e.KeyChar == (int)System.Windows.Forms.Keys.Escape)
            {
                Application.Exit();
            }
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Pause || e.KeyData == Keys.P)
                Paused = !Paused;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Paused = true;
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Paused = false;
        }
        private void MainForm_Activated(object sender, EventArgs e)
        {
            Paused = false;
        }

        public void draw_text(PaintEventArgs e)
        {
            //
            // Create a Font for any Text in Display
            //
            System.Drawing.Font FontlblPoint = new System.Drawing.Font(FontFamily.Families[0].Name, 15.75F,
                ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))),
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            System.Drawing.Font pauseFont = new System.Drawing.Font("Arial", 90.75F,
                ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))),
                System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //
            // Set Pause Text
            //
            e.Graphics.DrawString("PAUSE", pauseFont, Brushes.White,
                new PointF((this.ClientRectangle.Width / 2) - 240, (this.ClientRectangle.Height / 2) - 100));
            //
            // Display ESC key's
            //
            e.Graphics.DrawString("Please press ESC key's for exit.", FontlblPoint, Brushes.Blue, new PointF(5, 30));

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < System.Diagnostics.Process.GetCurrentProcess().Threads.Count; i++)
                System.Diagnostics.Process.GetCurrentProcess().Threads[i].Dispose();

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

    }
}
