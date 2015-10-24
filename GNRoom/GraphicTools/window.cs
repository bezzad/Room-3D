using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace GraphicTools
{
    public sealed class window : GraphicTools.textureWall
    {
        private doorState state = doorState.Closed;
        private float reLocation = 0;
        private openTranslationVector openTo;
        private float maxTranslationDistance;
        private bool _isTransparent;

        /// <summary>
        /// default value is 0.2f for open or close window speed's
        /// </summary>
        public float speed = 0.2f;

        /// <summary>
        /// Constructor of window Class   
        /// </summary>
        /// <param name="device3d">this Form Device3D.device</param>
        /// <param name="mirror">boolean for mirror Texture</param>
        /// <param name="wall_Points">right_Lower Point of Rectangle Point</param>
        /// <param name="srcTexturePathe">Pathe of Texture Picture's in HDD</param>
        /// <param name="AlphaBlendOperation">Transparent Color's in window operation</param>
        /// <param name="openWindowTo">Translation window to this vector in opening state</param>
        /// <param name="maxTD">Maximum Distance for Translation</param>
        /// <param name="drawStepX">drawing steps between i point's of x-coordinate</param>
        /// <param name="drawStepY">drawing steps between j point's of y-coordinate</param>
        /// <param name="drawStepZ">drawing steps between k point's of z-coordinate</param>
        /// <param name="isTransparent">do you want your window texture's is the transparent?</param>
        public window(Device device3d, bool mirror, openTranslationVector openWindowTo, float maxTD,
                    threeAxisRectangle wall_Points, int drawStepX, int drawStepY, int drawStepZ,
                    string srcTexturePathe, bool isTransparent)
            : base(device3d, mirror, wall_Points, drawStepX, drawStepY, drawStepZ, srcTexturePathe) 
        {
            maxTranslationDistance = maxTD;
            openTo = openWindowTo;
            _isTransparent = isTransparent;
        }

        /// <summary>
        /// "Open" or "Close" the this window
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
        /// "Open" or "Close" the this window by sounds
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
                switch (openTo)
                {
                    case openTranslationVector.Up:
                        _device3D.Transform.World = Matrix.Translation(0, reLocation, 0);
                        break;
                    case openTranslationVector.Down:
                        _device3D.Transform.World = Matrix.Translation(0, -reLocation, 0);
                        break;
                    case openTranslationVector.Right:
                        _device3D.Transform.World = Matrix.Translation(-reLocation, 0, 0);
                        break;
                    case openTranslationVector.Left:
                        _device3D.Transform.World = Matrix.Translation(reLocation, 0, 0);
                        break;
                    case openTranslationVector.Depth:
                        _device3D.Transform.World = Matrix.Translation(0, 0, reLocation);
                        break;
                    case openTranslationVector.OutSide:
                        _device3D.Transform.World = Matrix.Translation(0, 0, -reLocation);
                        break;
                }

                if (state == doorState.Closing)
                {
                    if (reLocation - speed > 0)
                        reLocation -= speed;
                    else
                    {
                        reLocation = 0;
                        state = doorState.Closed;
                    }
                }
                else if (state == doorState.Opening)
                {
                    if (reLocation + speed < maxTranslationDistance)
                        reLocation += speed;
                    else
                    {
                        reLocation = maxTranslationDistance;
                        state = doorState.Opened;
                    }
                }
            }
       
            if (_isTransparent)
            {
                _device3D.RenderState.SourceBlend = Blend.One;
                _device3D.RenderState.DestinationBlend = Blend.SourceColor;
            }
            _device3D.RenderState.AlphaBlendEnable = true;
            base.drawWall();
            _device3D.RenderState.AlphaBlendEnable = false;
            _device3D.Transform.World = Matrix.Identity;
        }
    }

    public enum openTranslationVector { Up, Down, Left, Right, Depth, OutSide }
}
