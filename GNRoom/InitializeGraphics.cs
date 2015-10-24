using System;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace GNRoom
{
    class InitializeGraphics
    {
        private PresentParameters pp;

        public InitializeGraphics(bool windowed)
        {
            pp = new PresentParameters();
            if (!windowed) // FullScreen
            {
                pp.BackBufferFormat = Manager.Adapters[0].CurrentDisplayMode.Format;
                pp.BackBufferHeight = Manager.Adapters[0].CurrentDisplayMode.Height;
                pp.BackBufferWidth = Manager.Adapters[0].CurrentDisplayMode.Width;
            }
            pp.Windowed = windowed;
            pp.SwapEffect = SwapEffect.Discard;
            pp.AutoDepthStencilFormat = DepthFormat.D16;
            pp.EnableAutoDepthStencil = true;
        }

        public Device getDevice(Form frm)
        {
            Caps DevCaps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
            DeviceType DevType = DeviceType.Reference;
            CreateFlags DevFlags = CreateFlags.SoftwareVertexProcessing;
            if (DevCaps.PixelShaderVersion >= new Version(2, 0))
            {
                DevType = DeviceType.Hardware;
                if (DevCaps.DeviceCaps.SupportsHardwareTransformAndLight)
                {
                    DevFlags = CreateFlags.HardwareVertexProcessing;
                    if (DevCaps.DeviceCaps.SupportsPureDevice)
                    {
                        DevFlags |= CreateFlags.PureDevice;
                    }
                }
            }
            Device device3d = new Device(0, DevType, frm, DevFlags, pp);
            return (device3d);
        }        
    }
}
