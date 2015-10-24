using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace GraphicTools
{
    public sealed class Door : GraphicTools.textureWall
    {
        private doorState state = doorState.Closed;
        private float teta = 0;
        private float _maxRotateAngle;

        /// <summary>
        /// default value is 0.05f for open or close door speed's
        /// </summary>
        public float speed = 0.05f;

        /// <summary>
        /// Constructor of Door Class   
        /// </summary>
        /// <param name="device3d">this Form Device3D.device</param>
        /// <param name="mirror">boolean for mirror Texture</param>
        /// <param name="wall_Points">right_Lower Point of Rectangle Point</param>
        /// <param name="srcTexturePathe">Pathe of Texture Picture's in HDD</param>
        /// <param name="maxRotateAngle">this number is between 0~7 for Rotate 360 angle</param>
        /// <param name="drawStepX">drawing steps between i point's of x-coordinate</param>
        /// <param name="drawStepY">drawing steps between j point's of y-coordinate</param>
        /// <param name="drawStepZ">drawing steps between k point's of z-coordinate</param>
        public Door(Device device3d, bool mirror,
                    threeAxisRectangle wall_Points, int drawStepX, int drawStepY, int drawStepZ,
                    string srcTexturePathe, float maxRotateAngle)
            : base(device3d, mirror, wall_Points, drawStepX, drawStepY, drawStepZ, srcTexturePathe)
        {
            _maxRotateAngle = maxRotateAngle;
        }

        /// <summary>
        /// "Open" or "Close" the this Door
        /// </summary>
        public void openClose()
        {
            switch (state)
            {
                case doorState.Closed: state = doorState.Opening; 
                    break;
                case doorState.Closing: state = doorState.Closing;
                    break;
                case doorState.Opened: state = doorState.Closing;
                    break;
                case doorState.Opening: state = doorState.Opening;
                    break;
            }
        }

        /// <summary>
        /// "Open" or "Close" the this Door by sounds
        /// </summary>
        public void openClose(SoundBox openSound, SoundBox closeSound)
        {
            switch (state)
            {
                case doorState.Closed: { state = doorState.Opening; openSound.playSound(); }
                    break;
                case doorState.Closing: state = doorState.Closing;
                    break;
                case doorState.Opened: { state = doorState.Closing; closeSound.playSound(); }
                    break;
                case doorState.Opening: state = doorState.Opening;
                    break;
            }
        }

        public override void drawWall()
        {
            if (!hasError)
            {
                _device3D.Transform.World = Matrix.Translation(-_wall.X, -_wall.Y, -_wall.Z); // Move To Center
                _device3D.Transform.World *= Matrix.RotationY(teta); // Rotate on Y axis
                _device3D.Transform.World *= Matrix.Translation(_wall.RightLowerPos); // Move to old position

                if (state == doorState.Closing)
                {
                    if (teta - speed > 0)
                        teta -= speed;
                    else
                    {
                        teta = 0;
                        state = doorState.Closed;
                    }
                }
                else if (state == doorState.Opening)
                {
                    if (teta + speed < _maxRotateAngle)
                        teta += speed;
                    else
                    {
                        teta = _maxRotateAngle;
                        state = doorState.Opened;
                    }
                }
            }
            base.drawWall();
            _device3D.Transform.World = Matrix.Identity;
        }

        public override bool checkCollision(Vector3 cameraPosition)
        {
            if (state == doorState.Closed)
                return base.checkCollision(cameraPosition);
            else return false;
        }
    }

    public enum doorState { Opened, Closed, Opening, Closing }
}
