using System;
using System.Windows.Forms;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using GraphicTools;
using System.Collections.Generic;

namespace GNRoom
{
    public class InputToolBox
    {
        private Device KeyBoardDevice;
        private Device MouseDevice;
        private GraphicEngine GE_object;
        private KeyboardState KeyState;
        private MouseState MState;
        private Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured[] _vFloor;
        private float H_Buffer = 0;
        private bool _checkHeight;
        private SoundBox walkingSound;
        //
        // check collision objects
        //
        private List<textureWall> textureWalls;
        private List<Mesh3D> mesh3Ds;
        //

        public InputToolBox(GraphicEngine GE, bool checkHeight)
        {
            GE_object = GE;
            _checkHeight = checkHeight;
            try
            {
                KeyBoardDevice = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
                KeyBoardDevice.SetDataFormat(DeviceDataFormat.Keyboard);
                KeyBoardDevice.Acquire();

                MouseDevice = new Device(SystemGuid.Mouse);
                MouseDevice.SetDataFormat(DeviceDataFormat.Mouse);
                MouseDevice.Acquire();
            }
            catch (DirectXException ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }
        }

        /// <summary>
        /// Check input device without check Floor Height 
        /// </summary>
        public void Poll()
        {
            if (!_checkHeight)
            {
                KeyState = KeyBoardDevice.GetCurrentKeyboardState();
                KeyBoardJOBs();

                MState = MouseDevice.CurrentMouseState;
                MouseJOBs();
            }
        }

        /// <summary>
        /// Check input device with check Floor Height 
        /// </summary>
        /// <param name="vFloor"></param>
        public void Poll(ref Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured[] vFloor)
        {
            _vFloor = vFloor;
            KeyState = KeyBoardDevice.GetCurrentKeyboardState();
            KeyBoardJOBs();

            MState = MouseDevice.CurrentMouseState;
            MouseJOBs();
        }

        private void KeyBoardJOBs()
        {
            #region Properties
            float xT = GE_object.CameraTarget.X;
            float yT = GE_object.CameraTarget.Y;
            float zT = GE_object.CameraTarget.Z;
            //
            // if xP is Width of center 
            float xP = GE_object.CameraPosition.X;
            //
            // if yP is Height of center
            float yP = GE_object.CameraPosition.Y;
            //
            // if zP is Depth of center
            float zP = GE_object.CameraPosition.Z;
            //
            // Step of Moving
            float goStep = (KeyState[Key.RightShift]) ? (float)GE_object.drawStep / 10 : (float)GE_object.drawStep / 20;
            //
            // Example:
            //                 +Y
            //                  Original CameraTarget(x, y)
            //                  |    
            //                  | 
            //                  | teta?
            // +X --------------O(a,b)------------ -X
            //                  |\
            //                  | \
            //                  |  O new CameraTarget(x', y')
            //                  |
            //                  |
            //                 -Y
            //
            // 
            // [x', y'] = new CameraTarget
            // [x, y] = old CameraTarget
            // [a, b] = CameraPosition
            // teta = Angle between (cameraPosition__oldCameraTarget) and (cameraPosition__newCameraTarget)
            // 1_Formula:   x' = a + (x-a).Cos(teta) - (y-b).Sin(teta)
            // 2_Formula:   y' = b + (x-a).Sin(teta) + (y-b).Cos(teta)
            //
            // now what is teta angle?
            //
            // Result:
            //          newX = goStep.Cos(teta) + a
            //          newY = goStep.Sin(teta) + b
            //          Incline m = (newY-b) / (newX-a)
            //          teta = ArcTan(m)
            //          
            // Note:    if (x == a) the line is parallel with Y axis
            //          then    teta = NaN
            //
            //
            // if the LINE between cameraPosition and cameraTarget is 
            // Parallel with Y axis ?
            double teta = 0;
            if (xT == xP) // yes the line is parallel with Y axis
            {
                // Positive Moving
                if (zT > zP) // the cameraTarget far than cameraPosition in depth of Display
                {
                    teta = (90 * Math.PI) / 180;
                }
                // Negative Moving
                else // the cameraTarget near than cameraPosition to display
                {
                    // the target is behine at the position
                    teta = (-90 * Math.PI) / 180;
                }
            }
            else // don't parallel with Y axis
            {
                double m = (zT - zP) / (xT - xP); // Incline of Line where is between cameraTarget and cameraPosition
                teta = Math.Atan(m); // the angle between LINE and X axis
            }
            //
            // Whether the depth of the camera moves a positive or negative side?
            //
            float goCT = (float)(goStep * Math.Cos(teta));
            float goST = (float)(goStep * Math.Sin(teta));
            float goCT_Plus90 = (float)(goStep * Math.Cos(sumTeta(teta, 90)));
            float goST_Plus90 = (float)(goStep * Math.Sin(sumTeta(teta, 90)));
            #endregion
            //
            bool Moved = false;
            #region Go to UP Arrow
            if (KeyState[Key.UpArrow])
            {
                if ((xT > xP && zT > zP) || (xT > xP && zT < zP)) // Quadrant 1 or 4
                {
                    GE_object.CameraPosition.Z += goST;
                    GE_object.CameraTarget.Z += goST;
                    //
                    GE_object.CameraPosition.X += goCT;
                    GE_object.CameraTarget.X += goCT;
                }
                else if ((xT < xP && zT > zP) || (xT < xP && zT < zP)) // Quadrant 2 or 3
                {
                    GE_object.CameraPosition.Z -= goST;
                    GE_object.CameraTarget.Z -= goST;
                    //
                    GE_object.CameraPosition.X -= goCT;
                    GE_object.CameraTarget.X -= goCT;
                }
                else if (xT == xP) // the Line is Parallel with Y axis
                {
                    GE_object.CameraPosition.Z += goST;
                    GE_object.CameraTarget.Z += goST;
                }
                else // the Line is Parallel with X axis
                {
                    GE_object.CameraPosition.X += goCT;
                    GE_object.CameraTarget.X += goCT;
                }
                Moved = true;
            }
            #endregion
            #region Go to DOWN Arrow
            if (KeyState[Key.DownArrow])
            {
                if ((xT > xP && zT > zP) || (xT > xP && zT < zP)) // Quadrant 1 or 4
                {
                    GE_object.CameraPosition.Z -= goST;
                    GE_object.CameraTarget.Z -= goST;
                    //
                    GE_object.CameraPosition.X -= goCT;
                    GE_object.CameraTarget.X -= goCT;
                }
                else if ((xT < xP && zT > zP) || (xT < xP && zT < zP)) // Quadrant 2 or 3
                {
                    GE_object.CameraPosition.Z += goST;
                    GE_object.CameraTarget.Z += goST;
                    //
                    GE_object.CameraPosition.X += goCT;
                    GE_object.CameraTarget.X += goCT;
                }
                else if (xT == xP) // the Line is Parallel with Y axis
                {
                    GE_object.CameraPosition.Z -= goST;
                    GE_object.CameraTarget.Z -= goST;
                }
                else // the Line is Parallel with X axis
                {
                    GE_object.CameraPosition.X -= goCT;
                    GE_object.CameraTarget.X -= goCT;
                }
                Moved = true;
            }
            #endregion
            #region Go to RIGHT Arrow
            if (KeyState[Key.RightArrow])
            {
                if ((xT > xP && zT > zP) || (xT > xP && zT < zP)) // Quadrant 1 or 4
                {
                    GE_object.CameraPosition.Z += goST_Plus90;
                    GE_object.CameraTarget.Z += goST_Plus90;
                    //
                    GE_object.CameraPosition.X += goCT_Plus90;
                    GE_object.CameraTarget.X += goCT_Plus90;
                }
                else if ((xT < xP && zT > zP) || (xT < xP && zT < zP)) // Quadrant 2 or 3
                {
                    GE_object.CameraPosition.Z -= goST_Plus90;
                    GE_object.CameraTarget.Z -= goST_Plus90;
                    //
                    GE_object.CameraPosition.X -= goCT_Plus90;
                    GE_object.CameraTarget.X -= goCT_Plus90;
                }
                else if (xT == xP) // the Line is Parallel with Y axis
                {
                    GE_object.CameraPosition.X += goCT_Plus90;
                    GE_object.CameraTarget.X += goCT_Plus90;
                }
                else // the Line is Parallel with X axis
                {
                    GE_object.CameraPosition.Z += goST_Plus90;
                    GE_object.CameraTarget.Z += goST_Plus90;
                }
                Moved = true;
            }
            #endregion
            #region Go to LEFT Arrow
            if (KeyState[Key.LeftArrow])
            {
                if ((xT > xP && zT > zP) || (xT > xP && zT < zP)) // Quadrant 1 or 4
                {
                    GE_object.CameraPosition.Z -= goST_Plus90;
                    GE_object.CameraTarget.Z -= goST_Plus90;
                    //
                    GE_object.CameraPosition.X -= goCT_Plus90;
                    GE_object.CameraTarget.X -= goCT_Plus90;
                }
                else if ((xT < xP && zT > zP) || (xT < xP && zT < zP)) // Quadrant 2 or 3
                {
                    GE_object.CameraPosition.Z += goST_Plus90;
                    GE_object.CameraTarget.Z += goST_Plus90;
                    //
                    GE_object.CameraPosition.X += goCT_Plus90;
                    GE_object.CameraTarget.X += goCT_Plus90;
                }
                else if (xT == xP) // the Line is Parallel with Y axis
                {
                    GE_object.CameraPosition.X -= goCT_Plus90;
                    GE_object.CameraTarget.X -= goCT_Plus90;
                }
                else // the Line is Parallel with X axis
                {
                    GE_object.CameraPosition.Z -= goST_Plus90;
                    GE_object.CameraTarget.Z -= goST_Plus90;
                }
                Moved = true;
            }
            #endregion
            //
            // Check outline of moving
            //
            #region Check collision of moving
            if (Moved)
            {
                if (checkCollisions())
                {
                    GE_object.CameraPosition.X = xP;
                    GE_object.CameraPosition.Y = yP;
                    GE_object.CameraPosition.Z = zP;

                    GE_object.CameraTarget.X = xT;
                    GE_object.CameraTarget.Y = yT;
                    GE_object.CameraTarget.Z = zT;
                    Moved = false;
                }
            }
            #endregion
            //
            // play walking sounds
            //
            if (Moved && walkingSound != null)
            {
                walkingSound.playSound();
            }
            //
            // Restart this Application's
            //
            if (KeyState[Key.R])
                Application.Restart();
            //
            // Open or Close Door
            //
            if (KeyState[Key.D])
                GE_object.OpenCloseDoor();
            //
            // Open or Close Window
            //
            if (KeyState[Key.W])
                GE_object.OpenCloseWindow();
            //
            // Check Height of Surface
            //
            #region Check Height of Surface
            if (_checkHeight)
            {
                if (Moved && (zP > 0) && (zP < GE_object.trainDepth * GE_object.drawStep) &&
                             (xP > 0) && (xP < GE_object.trainWidth * GE_object.drawStep))
                {
                    try
                    {
                        int pIndex = findIndex(xP, zP);
                        // Maximum of cameraPosition and H_Buffer
                        float max_cP_Hb = Math.Max(H_Buffer, (_vFloor[pIndex].Y + GE_object.drawStep));
                        float incTP = max_cP_Hb - GE_object.CameraPosition.Y;
                        GE_object.CameraPosition.Y = max_cP_Hb;
                        GE_object.CameraTarget.Y += incTP;
                    }
                    catch { }
                }
            }
            #endregion
        }

        private void MouseJOBs()
        {
            #region Mouse Properties
            float xT = GE_object.CameraTarget.X;
            float yT = GE_object.CameraTarget.Y;
            float zT = GE_object.CameraTarget.Z;
            //
            // if a is Width of center 
            float xP = GE_object.CameraPosition.X;
            //
            // if b is Height of center
            float yP = GE_object.CameraPosition.Y;
            //
            // if a is height of center 
            float zP = GE_object.CameraPosition.Z;
            //
            // Moving SpeedUp
            int SpeedUP = 5;
            #endregion
            #region Mouse Left or Right Moving
            if (MState.X != 0) // CameraTarget Rightward or Leftward rotation axis Z in the center CameraPosition.
            {
                //
                // teta = (degree * PI) / 180
                double teta = ((MState.X / SpeedUP) * Math.PI) / 180;
                //
                // x' = a + (x-a).Cos(t) - (z-b).Sin(t)
                GE_object.CameraTarget.X = (float)(xP + (xT - xP) * Math.Cos(teta) - (zT - zP) * Math.Sin(teta));
                //
                // z' = b + (x-a).Sin(t) + (z-b).Cos(t)
                GE_object.CameraTarget.Z = (float)(zP + (xT - xP) * Math.Sin(teta) + (zT - zP) * Math.Cos(teta));
            }
            #endregion
            #region Mouse Up or Down
            if (MState.Y != 0) // CameraTarget Upward or Downward rotation axis X in the center CameraPosition.
            {
                double teta = 0;
                
                if (Math.Abs(zT - zP) >= Math.Abs(xT - zP))
                {
                    if (zT >= zP) // Positive
                    {
                        //
                        // teta = (degree * PI) / 180
                        teta = ((MState.Y / SpeedUP) * Math.PI) / 180;
                    }
                    else // Negative
                    {
                        //
                        // teta = (degree * PI) / 180
                        teta = ((-1 * MState.Y / SpeedUP) * Math.PI) / 180;
                    }
                    //
                    // y' = b + (z-c).Cos(t) - (y-b).Sin(t)
                    GE_object.CameraTarget.Y = (float)(yP + (yT - yP) * Math.Cos(teta) - (zT - zP) * Math.Sin(teta));
                    //
                    // z' = c + (z-c).Sin(t) + (y-b).Cos(t)
                    GE_object.CameraTarget.Z = (float)(zP + (yT - yP) * Math.Sin(teta) + (zT - zP) * Math.Cos(teta));
                }
                else
                {
                    if (xT >= xP) // Positive
                    {
                        //
                        // teta = (degree * PI) / 180
                        teta = ((MState.Y / SpeedUP) * Math.PI) / 180;
                    }
                    else // Negative
                    {
                        //
                        // teta = (degree * PI) / 180
                        teta = ((-1 * MState.Y / SpeedUP) * Math.PI) / 180;
                    }
                    //
                    // y' = b + (z-c).Cos(t) - (y-b).Sin(t)
                    GE_object.CameraTarget.Y = (float)(yP + (yT - yP) * Math.Cos(teta) - (xT - xP) * Math.Sin(teta));
                    //
                    // z' = c + (z-c).Sin(t) + (y-b).Cos(t)
                    GE_object.CameraTarget.X = (float)(xP + (yT - yP) * Math.Sin(teta) + (xT - xP) * Math.Cos(teta));
                }
            }
            #endregion
            #region Mouse Wheel
            if (MState.Z != 0)
            {
                int UpDown = MState.Z / 10;
                GE_object.CameraPosition.Y += UpDown;
                GE_object.CameraTarget.Y += UpDown;
                if (checkCollisions())
                {
                    GE_object.CameraPosition.X = xP;
                    GE_object.CameraPosition.Y = yP;
                    GE_object.CameraPosition.Z = zP;

                    GE_object.CameraTarget.X = xT;
                    GE_object.CameraTarget.Y = yT;
                    GE_object.CameraTarget.Z = zT;
                }
            }
            #endregion
            #region Mouse Button
            //
            // Mouse Left Button Pressed
            if (MState.GetMouseButtons()[0] > 0)
            {
                //
                // teta = (degree * PI) / 180
                double teta = (-1) * Math.PI / 180;
                //
                // x' = a + (x-a).Cos(t) - (z-b).Sin(t)
                GE_object.CameraTarget.X = (float)(xP + (xT - xP) * Math.Cos(teta) - (zT - zP) * Math.Sin(teta));
                //
                // z' = b + (x-a).Sin(t) + (z-b).Cos(t)
                GE_object.CameraTarget.Z = (float)(zP + (xT - xP) * Math.Sin(teta) + (zT - zP) * Math.Cos(teta));
            }
            //
            // Mouse Right Button Pressed
            if (MState.GetMouseButtons()[1] > 0)
            {
                //
                // teta = (degree * PI) / 180
                double teta = Math.PI / 180;
                //
                // x' = a + (x-a).Cos(t) - (z-b).Sin(t)
                GE_object.CameraTarget.X = (float)(xP + (xT - xP) * Math.Cos(teta) - (zT - zP) * Math.Sin(teta));
                //
                // z' = b + (x-a).Sin(t) + (z-b).Cos(t)
                GE_object.CameraTarget.Z = (float)(zP + (xT - xP) * Math.Sin(teta) + (zT - zP) * Math.Cos(teta));
            }
            #endregion
        }

        /// <summary>
        /// give (teta + angle) in radius degree base
        /// </summary>
        /// <param name="t">teta in radius base</param>
        /// <param name="angle">angle is a degree but is not converted to radius base</param>
        /// <returns>t + angle</returns>
        private double sumTeta(double t, float angle)
        {
            t = t * 180 / Math.PI; // convert to old state (decarti)
            t += angle;
            t = t * Math.PI / 180; // convert to radian
            return t;
        }

        private int findIndex(float xP, float zP)
        {
            // Preamble:
                    //  drawStep = 10
                    //
                    //       (40,30)   (30,30)   (20,30)   (10,30)
                    //          O---------O---------O---------O
                    //        11|       10|        9|        8|
                    //          |         |         |         |
                    //          |         |  .p     |         |
                    //          |         | (27,22) |         |
                    //          O---------O---------O---------O (10,20)
                    //         7|        6|        5|        4|
                    //          |         |         |         |
                    //          |         |         |         |
                    //          |         |         |         |
                    //          O---------O---------O---------O (10,10)
                    //          3         2         1         0
                    //
                    //  p = (27, 22) 
                    //  xIndex = p.X % drawStep = 7
                    //  zIndex = p.Z % drawStep = 2
                    //  
                    //  if(xIndex > drawStep/2) xIndex = (drawSetp - xIndex) + p.X;
                    //  else xIndex = p.X - xIndex;
                    //
                    //  if(zIndex > drawStep/2) zIndex = (drawStep - zIndex) + p.Z;
                    //  else zIndex = p.Z - zIndex;
                    //
                    //  xIndex = 30
                    //  zIndex = 20
                    //
                    //  xIndex = xIndex/drawStep = 3
                    //  zIndex = zIndex/drawStep = 2
                    //
                    //  Index = ((zIndex-1) * trianWidth) + (xIndex-1) = (1*4) + 2 = 6
                    //
                    // Best index is:
                    //
                    int xIndex = (int)(xP % GE_object.drawStep);
                    int zIndex = (int)(zP % GE_object.drawStep);
                    xIndex = (xIndex > (GE_object.drawStep / 2)) ? (int)((GE_object.drawStep - xIndex) + xP) : (int)(xP - xIndex);
                    zIndex = (zIndex > (GE_object.drawStep / 2)) ? (int)((GE_object.drawStep - zIndex) + zP) : (int)(zP - zIndex);
                    //
                    // debug Height of bestVertex
                    //
                    xIndex /= GE_object.drawStep;
                    zIndex /= GE_object.drawStep;
                    return (((zIndex - 1) * (GE_object.trainHeight)) + xIndex - 1);
        }

        /// <summary>
        /// Add walking sound effects.
        /// </summary>
        /// <param name="walkSound">walking sound effects as a *.midi | *.wav files.</param>
        public void SetWalkingSound(GraphicTools.SoundBox walkSound)
        {
            walkingSound = walkSound;
        }

        public void SetCollistionObjects(List<textureWall> lstTextureWall, List<Mesh3D> lstMesh3D)
        {
            textureWalls = lstTextureWall;
            mesh3Ds = lstMesh3D;
        }

        private bool checkCollisions()
        {
            if (textureWalls != null)
            {
                foreach (textureWall tW in textureWalls)
                {
                    try
                    {
                        if (tW.checkCollision(GE_object.CameraPosition))
                            return true;
                    }
                    catch (DirectXException ex) { MessageBox.Show(ex.Message, ex.Source); }
                }
            }
            if (mesh3Ds != null)
            {
                foreach (Mesh3D m in mesh3Ds)
                {
                    try
                    {
                        if (m.checkCollision(GE_object.CameraPosition, 10f))
                            return true;
                    }
                    catch (DirectXException ex) { MessageBox.Show(ex.Message, ex.Source); }
                }
            }
            return false;
        }
    }
}
