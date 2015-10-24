using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using System.IO;   
using System.Diagnostics;
using System.Windows.Forms;

namespace GraphicTools
{
    public class Mesh3D
    {
        protected bool hasError = false;
        protected Mesh mesh;
        protected Texture[] MeshTexturs;
        protected Material[] MeshMaterials;
        protected Device _device3d;
        protected Vector3 Scal;
        protected VertexFormats _vertexFormat;
        protected Vector3 meshCenter;
        protected float Radius;

        public Mesh3D(Device device3D, string xFileName,
            Vector3 scal, VertexFormats vertexFormat)
        {
            _device3d = device3D;
            Scal = scal;
            _vertexFormat = vertexFormat;
            string currentDirectory = Application.StartupPath;
            try
            {
                ExtendedMaterial[] materialArray;
                //
                // Check DirectX |*.x file name
                //
                if (!xFileName.EndsWith(".x", true, new System.Globalization.CultureInfo("en-US")))
                {
                    throw new Exception(@"Your File is not a DirectX file |*.x" + "\n\rPlease Enter correct FileName.");
                }
                if (!System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + @"\myMeshs\" + xFileName))
                {
                    throw new NotFoundException("Given path was not found.");
                }
                Directory.SetCurrentDirectory(Application.StartupPath + @"\myMeshs\");
                mesh = Mesh.FromFile(xFileName, MeshFlags.Managed, device3D, out materialArray);
                //
                // Compute Normals
                //
                if (vertexFormat == CustomVertex.PositionNormalTextured.Format)
                {
                    mesh = mesh.Clone(mesh.Options.Value, CustomVertex.PositionNormalTextured.Format, device3D);
                    mesh.ComputeNormals();
                }
                else if (vertexFormat == CustomVertex.PositionNormalColored.Format)
                {
                    mesh = mesh.Clone(mesh.Options.Value, CustomVertex.PositionNormalColored.Format, device3D);
                    mesh.ComputeNormals();
                }
                //
                // set data
                //
                if (materialArray != null && materialArray.Length > 0)
                {
                    MeshMaterials = new Material[materialArray.Length];
                    MeshTexturs = new Texture[materialArray.Length];
                    for (int i = 0; i < materialArray.Length; i++)
                    {
                        MeshMaterials[i] = materialArray[i].Material3D;
                        MeshMaterials[i].Ambient = MeshMaterials[i].Diffuse;
                        if (!string.IsNullOrEmpty(materialArray[i].TextureFilename))
                        {
                            MeshTexturs[i] = TextureLoader.FromFile(device3D, materialArray[i].TextureFilename);
                        }
                    }
                }
                ComputeRadius();
                optimaize(Math.Max(Math.Max(scal.X, scal.Y), scal.Z));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, ex.Source,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                hasError = true;
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }
        }

        public virtual void Draw(Vector3 translate)
        {
            meshCenter = translate;
            if (!hasError)
            {
                VertexFormats vtTemp = _device3d.VertexFormat; // backup as the this device.VertexFormat
                _device3d.VertexFormat = _vertexFormat;
                // Moving the shape
                _device3d.Transform.World = Matrix.Scaling(Scal) *
                                            Matrix.Translation(translate);
                //
                Texture temp = _device3d.GetTexture(0); // Backup Texturs
                //
                for (int i = 0; i < MeshMaterials.Length; i++)
                {
                    _device3d.Material = MeshMaterials[i];
                    _device3d.SetTexture(0, MeshTexturs[i]);
                    mesh.DrawSubset(i);
                }
                //
                _device3d.SetTexture(0, temp); // Restore Texturs
                //
                // Reset the moved shape for none-effect on other shapes
                _device3d.Transform.World = Matrix.Identity;
                //
                // Restore the old VertexFormat
                _device3d.VertexFormat = vtTemp;
            }
        }

        public virtual void Draw(Vector3 translate, float angle)
        {
            meshCenter = translate;
            if (!hasError)
            {
                VertexFormats vtTemp = _device3d.VertexFormat; // backup as the this device.VertexFormat
                _device3d.VertexFormat = _vertexFormat;
                // Moving the shape
                _device3d.Transform.World = Matrix.Scaling(Scal) *
                                            Matrix.RotationY(angle) * Matrix.Translation(translate);
                //
                Texture temp = _device3d.GetTexture(0); // Backup Texturs
                //
                for (int i = 0; i < MeshMaterials.Length; i++)
                {
                    _device3d.Material = MeshMaterials[i];
                    _device3d.SetTexture(0, MeshTexturs[i]);
                    mesh.DrawSubset(i);
                }
                //
                _device3d.SetTexture(0, temp); // Restore Texturs
                //
                // Reset the moved shape for none-effect on other shapes
                _device3d.Transform.World = Matrix.Identity;
                //
                // Restore the old VertexFormat
                _device3d.VertexFormat = vtTemp;
            }
        }

        /// <summary>
        /// find the shape radius
        /// </summary>
        protected void ComputeRadius()
        {
            VertexBuffer vertices = mesh.VertexBuffer;
            GraphicsStream stream = vertices.Lock(0, 0, LockFlags.None);
            Radius = Geometry.ComputeBoundingSphere(stream, mesh.NumberVertices, mesh.VertexFormat, out meshCenter);
            vertices.Unlock();
        }

        public void optimaize(float scale)
        {
            int[] ad = new int[mesh.NumberFaces * 3];
            mesh.GenerateAdjacency(scale, ad);
            mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache, ad);
            Radius *= scale;
        }

        /// <summary>
        /// Checking the camera collision with specific radius, 
        /// with environmental objects.
        /// </summary>
        /// <param name="cameraPosition">Position of camera</param>
        /// <param name="cameraRadius">Radius of camera center position</param>
        /// <returns>Collision has occurred or not?</returns>
        public virtual bool checkCollision(Vector3 cameraPosition, float cameraRadius)
        {
            if (Distance3D(cameraPosition, this.meshCenter) < this.Radius + cameraRadius)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculate distance of two 3D-Point.
        /// </summary>
        /// <param name="point1">First 3D point</param>
        /// <param name="point2">Second 3D point</param>
        /// <returns>Distance of points</returns>
        protected float Distance3D(Vector3 point1, Vector3 point2)
        {
            return (float)(Math.Sqrt(Math.Pow((point2.Z - point1.Z), 2) + Math.Pow((point2.Y - point1.Y), 2) + Math.Pow((point2.X - point1.X), 2)));
        }
    }
}
