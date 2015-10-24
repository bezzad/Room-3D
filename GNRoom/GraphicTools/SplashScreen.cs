using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace GraphicTools
{
    public class SplashScreen
    {
        public bool Enable = false;
        private Device device3d;
        private CustomVertex.TransformedTextured[] vertices;
        private Texture texture;
        private SplashButton[] sButtons;

        public SplashScreen(Device device3D, string fileName, SplashButton[] buttons)
        {
            sButtons = buttons;
            device3d = device3D;
            int h = device3d.PresentationParameters.BackBufferHeight;
            int w = device3d.PresentationParameters.BackBufferWidth;
            vertices = new CustomVertex.TransformedTextured[6];
            vertices[0] = new CustomVertex.TransformedTextured(new Vector4(0, 0, 0.1f, 1f), 0, 0);
            vertices[1] = new CustomVertex.TransformedTextured(new Vector4(w, 0, 0.1f, 1f), 1, 0);
            vertices[2] = new CustomVertex.TransformedTextured(new Vector4(0, h, 0.1f, 1f), 0, 1);
            vertices[3] = new CustomVertex.TransformedTextured(new Vector4(0, h, 0.1f, 1f), 0, 1);
            vertices[4] = new CustomVertex.TransformedTextured(new Vector4(w, 0, 0.1f, 1f), 1, 0);
            vertices[5] = new CustomVertex.TransformedTextured(new Vector4(w, h, 0.1f, 1f), 1, 1);
            texture = TextureLoader.FromFile(device3d, fileName);
            Enable = true;
        }

        public void Draw()
        {
            if (!Enable) return;
            device3d.VertexFormat = CustomVertex.TransformedTextured.Format;
            device3d.BeginScene();
            //
            // Draw Splash Screen
            //
            device3d.SetTexture(0, texture);
            device3d.DrawUserPrimitives(PrimitiveType.TriangleList, 2, vertices);
            //
            // Draw any Splash Buttons
            //
            if (sButtons != null)
            {
                foreach (SplashButton anyBtn in sButtons)
                {
                    anyBtn.draw();
                }
            }
            //
            device3d.EndScene();
            device3d.Present();
        }
    }
}
