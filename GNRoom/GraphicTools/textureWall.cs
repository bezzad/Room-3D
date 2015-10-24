using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace GraphicTools
{
    public class textureWall
    {        
        protected Device _device3D;
        public CustomVertex.PositionTextured[] vertex;
        protected short[] index;
        protected int _drawStepX, _drawStepY, _drawStepZ;
        protected int trainWidth, trainHeight, trainDepth;
        protected threeAxisRectangle _wall;
        protected Texture _texture;
        protected bool hasHeight, hasWidth, hasDepth;
        protected bool hasError;
        protected float Radius;

        public textureWall(Microsoft.DirectX.Direct3D.Device device3D, bool Mirror,
                           threeAxisRectangle wall_Points, int drawStepX, int drawStepY, int drawStepZ,
                           string srcTexturePathe)
        {
            _device3D = device3D;
            _texture = TextureLoader.FromFile(device3D, srcTexturePathe);
            //
            // don't divison of Zero
            _drawStepX = (drawStepX != 0) ? drawStepX : 1;
            _drawStepY = (drawStepY != 0) ? drawStepY : 1;
            _drawStepZ = (drawStepZ != 0) ? drawStepZ : 1;
            //
            _wall = wall_Points;
            //
            hasDepth = (wall_Points.Depth != 0);
            hasHeight = (wall_Points.Height != 0);
            hasWidth = (wall_Points.Width != 0);
            //
            // بخاطر رسم دیوار نرسیده به مختصات خواسته شده به یک اندازه ی گام 
            // ابتدا یک گام به ابعاد غیر صفر اضافه می کنیم
            _wall.Depth += (hasDepth) ? _drawStepZ : 0;
            _wall.Height += (hasHeight) ? _drawStepY : 0;
            _wall.Width += (hasWidth) ? _drawStepX : 0;
            //
            // Set Size
            //
            trainWidth = _wall.Width / _drawStepX;
            trainHeight = _wall.Height / _drawStepY;
            trainDepth = _wall.Depth / _drawStepZ;
            //
            VertexDeclaration(Mirror);
            IndexDeclaration();
            //
            //
            //
            Radius = Math.Max(Math.Max(_drawStepX, _drawStepY), _drawStepZ) / 10;
        }

        protected void VertexDeclaration(bool mirror)
        {
            int k = 0;
            float tv, tu;
            hasError = false;

            if (hasWidth && hasHeight)
            {
                //          width
                //       ___________
                //      |           |
                //      |           | 
                //      |   Front   | height
                //      |           |
                //      |__________o|
                //
                //
                vertex = new CustomVertex.PositionTextured[trainWidth * trainHeight];
                for (int j = 0; j < trainHeight; j++)
                {
                    tv = (mirror) ? (j + 1) % 2 : j;
                    for (int i = 0; i < trainWidth; i++)
                    {
                        //
                        // z is heght of pixels in 3d plane
                        tu = (mirror) ? (i + 1) % 2 : i;
                        vertex[k++] = new CustomVertex.PositionTextured((i * _drawStepX) + _wall.X, (j * _drawStepY) + _wall.Y, _wall.Z, tu, tv);
                    }
                }
            }
            else if (hasWidth && hasDepth)
            {
                //
                //              width   
                //           ___________   
                //          /           \
                //         /             \
                //        /     Floor     \ depth
                //       /                 \
                //      /__________________o\ 
                //
                vertex = new CustomVertex.PositionTextured[trainWidth * trainDepth];
                for (int j = 0; j < trainDepth; j++)
                {
                    tv = (mirror) ? (j + 1) % 2 : j;
                    for (int i = 0; i < trainWidth; i++)
                    {
                        //
                        // z is heght of pixels in 3d plane
                        tu = (mirror) ? (i + 1) % 2 : i;
                        vertex[k++] = new CustomVertex.PositionTextured((i * _drawStepX) + _wall.X, _wall.Y, (j * _drawStepZ) + _wall.Z, tu, tv);
                    }
                }
            }
            else if (hasHeight && hasDepth)
            {
                //          
                //      |\             
                //      | \ 
                // depth|  \.
                //      |   |   
                //      |   |
                //       \  | height
                //        \ |
                //         \|
                //          o
                //
                vertex = new CustomVertex.PositionTextured[trainHeight * trainDepth];
                for (int j = 0; j < trainHeight; j++)
                {
                    tv = (mirror) ? (j + 1) % 2 : j;
                    for (int i = 0; i < trainDepth; i++)
                    {
                        //
                        // z is heght of pixels in 3d plane
                        tu = (mirror) ? (i + 1) % 2 : i;
                        vertex[k++] = new CustomVertex.PositionTextured(_wall.X, (j * _drawStepY) + _wall.Y, (i * _drawStepZ) + _wall.Z, tu, tv);
                    }
                }
            }
            else // Error
            {
                hasError = true;
                System.Windows.Forms.MessageBox.Show("your wall have incorrect coordinate!");
                return;
            }
        }

        protected void IndexDeclaration()
        {
            int t, k = 0;
            if (!hasError)
            {
                if (hasWidth && hasHeight)
                {
                    index = new short[(trainWidth - 1) * (trainHeight - 1) * 6];
                    for (int j = 0; j < trainHeight - 1; j++)
                    {
                        for (int i = 0; i < trainWidth - 1; i++)
                        {
                            t = i + trainWidth * j;
                            index[k++] = (short)t;
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainWidth);
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainWidth + 1);
                            index[k++] = (short)(t + trainWidth);
                        }
                    }
                }
                else if (hasWidth && hasDepth)
                {
                    index = new short[(trainWidth - 1) * (trainDepth - 1) * 6];
                    for (int j = 0; j < trainDepth - 1; j++)
                    {
                        for (int i = 0; i < trainWidth - 1; i++)
                        {
                            t = i + trainWidth * j;
                            index[k++] = (short)t;
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainWidth);
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainWidth + 1);
                            index[k++] = (short)(t + trainWidth);
                        }
                    }
                }
                else if (hasHeight && hasDepth)
                {
                    index = new short[(trainHeight - 1) * (trainDepth - 1) * 6];
                    for (int j = 0; j < trainDepth - 1; j++)
                    {
                        for (int i = 0; i < trainHeight - 1; i++)
                        {
                            t = i + trainHeight * j;
                            index[k++] = (short)t;
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainHeight);
                            index[k++] = (short)(t + 1);
                            index[k++] = (short)(t + trainHeight + 1);
                            index[k++] = (short)(t + trainHeight);
                        }
                    }
                }
            }
        }

        public virtual void drawWall()
        {
            if (!hasError)
            {
                _device3D.SetTexture(0, _texture);
                _device3D.VertexFormat = CustomVertex.PositionTextured.Format;
                if (hasWidth && hasHeight)
                {
                    _device3D.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                                                        trainHeight * trainWidth,
                                                        (trainHeight - 1) * (trainWidth - 1) * 2,
                                                        index, true, vertex);
                }
                else if (hasWidth && hasDepth)
                {
                    _device3D.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                                                        trainDepth * trainWidth,
                                                        (trainDepth - 1) * (trainWidth - 1) * 2,
                                                        index, true, vertex);
                }
                else if (hasHeight && hasDepth)
                {
                    _device3D.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0,
                                                        trainDepth * trainHeight,
                                                        (trainDepth - 1) * (trainHeight - 1) * 2,
                                                        index, true, vertex);
                }
            }
        }

        /// <summary>
        /// Checking the camera collision with specific radius, 
        /// with environmental objects.
        /// </summary>
        /// <param name="cameraPosition">Position of camera</param>
        /// <param name="cameraRadius">Radius of camera center position</param>
        /// <returns>Collision has occurred or not?</returns>
        public virtual bool checkCollision(Vector3 cameraPosition)
        {
            if (!hasError)
            {
                //                   +Z
                //             +Y    .
                //              .   /
                //              |  / 
                //              | /
                //              |/
                // +X <---------|---------> -X
                //             /|
                //            / |
                //           /  |
                //          /   |
                //         /   -Y
                //       -Z  
                //
                if (hasWidth && hasHeight)
                {
                    //          width
                    //       ___________
                    //      |           |
                    //      |           | 
                    //      |   Front   | height
                    //      |           |
                    //      |__________o|
                    //
                    //
                    if (cameraPosition.X >= (this._wall.X - this.Radius) &&
                    cameraPosition.X <= (this._wall.X + this._wall.Width - this._drawStepX)
                    &&
                    cameraPosition.Y >= (this._wall.Y - this.Radius) &&
                    cameraPosition.Y <= (this._wall.Y + this._wall.Height - this._drawStepY)
                    &&
                    cameraPosition.Z >= (this._wall.Z - this.Radius) &&
                    cameraPosition.Z <= (this._wall.Z + this.Radius))
                        return true;
                }
                else if (hasWidth && hasDepth)
                {
                    //
                    //              width   
                    //           ___________   
                    //          /           \
                    //         /             \
                    //        /     Floor     \ depth
                    //       /                 \
                    //      /__________________o\ 
                    //
                    if (cameraPosition.X >= (this._wall.X - this.Radius) &&
                    cameraPosition.X <= (this._wall.X + this._wall.Width - this._drawStepX)
                    &&
                    cameraPosition.Y >= (this._wall.Y - this.Radius) &&
                    cameraPosition.Y <= (this._wall.Y + this.Radius)
                    &&
                    cameraPosition.Z >= (this._wall.Z - this.Radius) &&
                    cameraPosition.Z <= (this._wall.Z + this._wall.Depth - this._drawStepZ))
                        return true;
                }
                else if (hasHeight && hasDepth)
                {
                    //          
                    //      |\             
                    //      | \ 
                    // depth|  \.
                    //      |   |   
                    //      |   |
                    //       \  | height
                    //        \ |
                    //         \|
                    //          o
                    //
                    if (cameraPosition.X >= (this._wall.X - this.Radius) &&
                    cameraPosition.X <= (this._wall.X + this.Radius)
                    &&
                    cameraPosition.Y >= (this._wall.Y - this.Radius) &&
                    cameraPosition.Y <= (this._wall.Y + this._wall.Height - this._drawStepY)
                    &&
                    cameraPosition.Z >= (this._wall.Z - this.Radius) &&
                    cameraPosition.Z <= (this._wall.Z + this._wall.Depth - this._drawStepZ))
                        return true;
                }
            }
            //
            // else of all
            return false;
        }
    }
}
