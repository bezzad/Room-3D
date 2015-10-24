using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using DI = Microsoft.DirectX.DirectInput;

namespace GraphicTools
{
    public class SplashButton
    {
        
        public bool Enable;

        private DI.Device MouseDevice;
        private DI.MouseState mouseState;
        private D3D.Device device3d;
        private Point Location;
        private Size size;
        private D3D.CustomVertex.TransformedTextured[] vertices;
        private D3D.Texture normalTexture;
        private D3D.Texture mouseOverTexture;
        private D3D.Texture mouseDownTexture;
        private D3D.Texture disableTexture;
        private ButtonState state = 0;
        private Point mouseLocation = new Point(0, 0);

        public SplashButton(System.Windows.Forms.Form frm, 
                            D3D.Device device3D, Point location, Size dimension,
                            string normalFileName, string mouseOverFileName,
                            string mouseDownFileName, string disableFileName)
        {
            device3d = device3D;
            Location = location;
            size = dimension;

            int h = Location.Y + size.Height;
            int w = Location.X + size.Width;
            vertices = new D3D.CustomVertex.TransformedTextured[6];
            vertices[0] = new D3D.CustomVertex.TransformedTextured(new Vector4(location.X, location.Y, 0f, 1f), 0, 0);
            vertices[1] = new D3D.CustomVertex.TransformedTextured(new Vector4(w, location.Y, 0f, 1f), 1, 0);
            vertices[2] = new D3D.CustomVertex.TransformedTextured(new Vector4(location.X, h, 0f, 1f), 0, 1);
            vertices[3] = new D3D.CustomVertex.TransformedTextured(new Vector4(location.X, h, 0f, 1f), 0, 1);
            vertices[4] = new D3D.CustomVertex.TransformedTextured(new Vector4(w, location.Y, 0f, 1f), 1, 0);
            vertices[5] = new D3D.CustomVertex.TransformedTextured(new Vector4(w, h, 0f, 1f), 1, 1);
            normalTexture = D3D.TextureLoader.FromFile(device3d, normalFileName);
            mouseOverTexture = D3D.TextureLoader.FromFile(device3d, mouseOverFileName);
            mouseDownTexture = D3D.TextureLoader.FromFile(device3d, mouseDownFileName);
            disableTexture = D3D.TextureLoader.FromFile(device3d, disableFileName);
            Enable = true;
            //
            // set mouse device
            //
            try
            {
                MouseDevice = new DI.Device(DI.SystemGuid.Mouse);
                MouseDevice.SetDataFormat(DI.DeviceDataFormat.Mouse);
                MouseDevice.Acquire();
            }
            catch (DirectXException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.Source);
            }
            finally
            {
                frm.MouseMove += new System.Windows.Forms.MouseEventHandler(frm_MouseMove);
            }
        }

        void frm_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseLocation = e.Location; // e is mouse object
        }

        public void draw()
        {
            //
            // Set Button State
            //
            state = SetButtonState();
            //
            // Set data and draw button
            //
            device3d.RenderState.SourceBlend = D3D.Blend.SourceColor;
            device3d.RenderState.DestinationBlend = D3D.Blend.One;
            device3d.RenderState.AlphaBlendEnable = true;
            switch (state)
            {
                case ButtonState.Normal:
                    {
                        device3d.SetTexture(0, normalTexture);
                        device3d.SetCursor(System.Windows.Forms.Cursors.Default, true);
                    }
                    break;
                case ButtonState.MouseDown:
                    {
                        device3d.SetTexture(0, mouseDownTexture);
                        device3d.SetCursor(System.Windows.Forms.Cursors.Hand, true);
                    }
                    break;
                case ButtonState.MouseOver:
                    {
                        device3d.SetTexture(0, mouseOverTexture);
                        device3d.SetCursor(System.Windows.Forms.Cursors.Hand, true);
                    }
                    break;
                case ButtonState.Disable:
                    {
                        device3d.SetTexture(0, disableTexture);
                        device3d.SetCursor(System.Windows.Forms.Cursors.Default, true);
                    }
                    break;
            }
            device3d.DrawUserPrimitives(D3D.PrimitiveType.TriangleList, 2, vertices);
            device3d.RenderState.AlphaBlendEnable = false;
        }

        private ButtonState SetButtonState()
        {
            mouseState = MouseDevice.CurrentMouseState;
            
            if (!this.Enable)
            {
                return ButtonState.Disable;
            }
            //
            // Check mouse position on button place for 
            // fine MouseOver or MouseDown
            else if (((mouseLocation.X >= this.Location.X) && (mouseLocation.X <= this.Location.X + this.size.Width)) &&
                    ((mouseLocation.Y >= this.Location.Y) && (mouseLocation.Y <= this.Location.Y + this.size.Height)))
            {
                //
                // MouseDown state
                if (mouseState.GetMouseButtons()[0] > 0)
                {
                    return ButtonState.MouseDown;
                }
                //
                // MoseOver state
                else
                {
                    //
                    // mouse click on button event is handled
                    if (state == ButtonState.MouseDown) SplashButton_Click();
                    return ButtonState.MouseOver;
                }
            }
            //
            // Normal state
            else
            {
                return ButtonState.Normal;
            }
        }

        private void SplashButton_Click()
        {
            this.Enable = false;
        }

        private enum ButtonState
        {
            Normal = 0,
            MouseOver = 1,
            MouseDown = 2,
            Disable = 3
        };
    }
}
