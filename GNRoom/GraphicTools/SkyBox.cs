using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace GraphicTools
{
    public class SkyBox : Mesh3D
    {
        public SkyBox(Device device3D, string xFileName,
            Vector3 scal, VertexFormats vertexFormat)
            : base(device3D, xFileName, scal, vertexFormat) { }

        public override bool checkCollision(Vector3 cameraPosition, float cameraRadius)
        {
            if (Distance3D(cameraPosition, this.meshCenter) >= 3 * this.Radius / 5 + cameraRadius)
                return true;
            else
                return false;
        }

    }
}
