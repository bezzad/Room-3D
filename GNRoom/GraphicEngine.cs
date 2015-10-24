using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using GraphicTools;
using System.Windows.Forms;

namespace GNRoom
{
    public class GraphicEngine
    {
        public Device device3d;
        public Vector3 CameraPosition, CameraTarget, Upvector;
        public int trainWidth = 6, trainHeight = 6, trainDepth = 6;
        private int width, height, depth = 5000;
        private InputToolBox GItb;
        private Material material;
        public int drawStep = 50;
        private bool _fullScreen;

        textureWall[] tW;
        textureWall[] roadTextures;
        Door _door;
        window _window;

        Mesh3D chairMesh3d;
        Mesh3D dinerTableMesh3d;
        Mesh3D pottryMesh3d;
        Mesh3D extLump;
        Mesh3D roadBarriers;
        SkyBox Sky;

        SplashScreen sps;
        SplashButton[] btnStart;

        SoundBox doorOpen_Sound;
        SoundBox doorClose_Sound;
        SoundBox background_Music;
        SoundBox walk_Sound;
        SoundBox windowOpen_Sound;
        SoundBox windowClose_Sound;

        public GraphicEngine(Device d, bool fullScreen)
        {
            _fullScreen = fullScreen;
            d.DeviceReset += new EventHandler(device3d_DeviceReset);
            device3d = d;

            material = new Microsoft.DirectX.Direct3D.Material();
            material.Diffuse = Color.White;
            device3d.Material = material;

            width = device3d.PresentationParameters.BackBufferWidth;
            height = device3d.PresentationParameters.BackBufferHeight;
            device3d.RenderState.CullMode = Cull.None;
            device3d.RenderState.Lighting = false;
            device3d.RenderState.FillMode = FillMode.Solid;
            //
            // Declaration Camera Information
            // 
            CameraPosition = new Vector3(drawStep * trainWidth / 2, 52, drawStep * (3 / 2));
            CameraTarget = new Vector3(drawStep * trainWidth / 2, 58, trainDepth * drawStep);
            Upvector = new Vector3(0, 1, 0);
            //
            // Note:
            //      InputToolBox must be define after declaration of camera Inforamation!
            GItb = new InputToolBox(this, false);

            createObjects();
            background_Music.playBackMusic();
            GItb.SetWalkingSound(walk_Sound);
            List<textureWall> lstTextureWall;
            List<Mesh3D> lstMesh3D;
            setObjectsToCollision(out lstTextureWall, out lstMesh3D);
            GItb.SetCollistionObjects(lstTextureWall, lstMesh3D);
            initialize_text(device3d);
        }

        private void setObjectsToCollision(out List<textureWall> lstTextureWall, out List<Mesh3D> lstMesh3D)
        {
            lstMesh3D = new List<Mesh3D>();
            lstMesh3D.Add(chairMesh3d);
            lstMesh3D.Add(dinerTableMesh3d);
            lstMesh3D.Add(pottryMesh3d);
            lstMesh3D.Add(Sky);
            //
            //
            //
            lstTextureWall = new List<textureWall>();
            lstTextureWall.AddRange(tW);
            lstTextureWall.Add(_door);
            lstTextureWall.Add(_window);
        }

        private void device3d_DeviceReset(object sender, EventArgs e)
        {
            device3d.RenderState.Lighting = false;
            device3d.RenderState.FillMode = FillMode.Solid;
            device3d.RenderState.CullMode = Cull.None;
            SetupCamera();
            createObjects();
        }

        private void SetupCamera()
        {
            device3d.Transform.View = Matrix.LookAtRH(CameraPosition, CameraTarget, Upvector);
            device3d.Transform.Projection = Matrix.PerspectiveFovRH((float)Math.PI / 2, width / height, 1f, depth);
        }

        public void DrawWorld()
        {
            SetupCamera();
            device3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkSlateBlue, 1.0f, 0);
            if (!btnStart[0].Enable)
            {
                device3d.BeginScene();
                if (_fullScreen)
                {
                    device3d.SetCursor(System.Windows.Forms.Cursors.No, true);
                    device3d.ShowCursor(false);
                }
                //
                //
                //
                #region textures
                // Floor
                tW[0].drawWall();
                //
                // Front Wall
                tW[1].drawWall();
                tW[2].drawWall();
                tW[3].drawWall();
                //
                // Right Wall
                tW[4].drawWall();
                //
                // Left Wall
                tW[5].drawWall();
                //
                // Back Wall
                tW[6].drawWall();
                //
                // Top
                tW[7].drawWall();
                //
                // Door 
                _door.drawWall();
                //
                // Carpet
                tW[8].drawWall();
                //
                // right Tableau
                tW[9].drawWall();
                //
                // left Tableau
                tW[10].drawWall();
                //
                // window Frame
                tW[11].drawWall();
                //
                // window
                _window.drawWall();
                #endregion
                #region road Textures
                roadTextures[0].drawWall();
                roadTextures[1].drawWall();
                roadTextures[2].drawWall();
                #endregion
                #region Mesh3Ds
                //
                // Mesh3Ds
                if (chairMesh3d != null)
                {
                    chairMesh3d.Draw(new Vector3(280, 25, 150), -4.75f);
                    chairMesh3d.Draw(new Vector3(280, 25, 80), -4.75f);
                }
                if (dinerTableMesh3d != null)
                {
                    dinerTableMesh3d.Draw(new Vector3(210, 0, 130), -1.5f);
                }
                if (pottryMesh3d != null)
                {
                    pottryMesh3d.Draw(new Vector3(210, 40.8f, 130));
                }
                if (Sky != null)
                {
                    Sky.Draw(new Vector3(0, 600, 500));
                }
                if (extLump != null)
                {
                    extLump.Draw(new Vector3(250, 0, 750));
                    extLump.Draw(new Vector3(-100, 0, 450), 1.57f);
                }
                if (roadBarriers != null)
                {
                    //     one Barriers    _ 
                    //  |<-------------->| |   
                    //  |                | | 15 px = depth.
                    //  |<-------------->| |
                    //        130 px       -
                    //        width.
                    //
                    //                  |a  a|
                    //               2  |b  b|  1
                    //                o |c  c|
                    //              ____|d  d|_____
                    //              hgfe      efghi  
                    //              ____      _____
                    //          4   edcb|a  a|bcdef  3     
                    //            .------  /------.
                    //            |       /       |
                    //            |     HOUSE     |
                    //
                    //

                    //
                    // 1
                    //
                    roadBarriers.Draw(new Vector3(-30, 0, 667));  // e
                    roadBarriers.Draw(new Vector3(-160, 0, 667)); // f
                    roadBarriers.Draw(new Vector3(-290, 0, 667)); // g
                    roadBarriers.Draw(new Vector3(-320, 0, 667)); // h
                    roadBarriers.Draw(new Vector3(-450, 0, 667)); // i
                    roadBarriers.Draw(new Vector3(51, 0, 713), 1.57f);  // d
                    roadBarriers.Draw(new Vector3(51, 0, 843), 1.57f);  // c
                    roadBarriers.Draw(new Vector3(51, 0, 973), 1.57f);  // b
                    roadBarriers.Draw(new Vector3(51, 0, 1103), 1.57f); // a
                    //
                    // 2
                    //
                    roadBarriers.Draw(new Vector3(282, 0, 667));  // e
                    roadBarriers.Draw(new Vector3(412, 0, 667)); // f
                    roadBarriers.Draw(new Vector3(542, 0, 667)); // g
                    roadBarriers.Draw(new Vector3(672, 0, 667)); // h
                    roadBarriers.Draw(new Vector3(217, 0, 713), 1.57f);  // d
                    roadBarriers.Draw(new Vector3(217, 0, 843), 1.57f);  // c
                    roadBarriers.Draw(new Vector3(217, 0, 973), 1.57f);  // b
                    roadBarriers.Draw(new Vector3(217, 0, 1103), 1.57f); // a
                    //
                    // 3
                    //
                    roadBarriers.Draw(new Vector3(-30, 0, 501));  // b
                    roadBarriers.Draw(new Vector3(-160, 0, 501)); // c
                    roadBarriers.Draw(new Vector3(-290, 0, 501)); // d
                    roadBarriers.Draw(new Vector3(-320, 0, 501)); // e
                    roadBarriers.Draw(new Vector3(-450, 0, 501)); // f
                    roadBarriers.Draw(new Vector3(51, 0, 433), 1.57f);  // a
                    //
                    // 4
                    //
                    roadBarriers.Draw(new Vector3(282, 0, 501));  // b
                    roadBarriers.Draw(new Vector3(412, 0, 501)); // c
                    roadBarriers.Draw(new Vector3(542, 0, 501)); // d
                    roadBarriers.Draw(new Vector3(672, 0, 501)); // e
                    roadBarriers.Draw(new Vector3(217, 0, 433), 1.57f);  // a
                }
                //
                //
                //
                device3d.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
                device3d.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
                device3d.SamplerState[0].AddressU = TextureAddress.Mirror;
                device3d.SamplerState[0].AddressV = TextureAddress.Mirror;
                //
                // 
                //
                #endregion
                //
                // runTime Method for Text
                //
                draw_text();
                //
                device3d.EndScene();
                device3d.Present();
                //
                GItb.Poll(); // setCurrent State for Mouse and Keyboard
            }
            else
            {
                //
                // SplashScreen
                //
                sps.Draw();
            }
        }

        private void createObjects()
        {
            try
            {
                #region textures
                //
                // Define Array for 6 face of a Cube (my Room)
                // tW[0] = Floor
                // tW[1] = Front Wall
                // tW[2] = Front Wall
                // tW[3] = Front Wall
                // _door = Door
                // tW[4] = Right Wall
                // tW[5] = Left Wall
                // tW[6] = Back Wall
                // tW[7] = Top
                // tW[8] = Carpet
                // tW[9] = right Tableau
                // tW[10] = left Tableau
                // tW[11] = window Frame
                // _window = window
                tW = new textureWall[12];
                //
                // Floor
                tW[0] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(0, 0, 0),
                    (trainWidth * drawStep), 0, (trainDepth * drawStep)), drawStep, 0, drawStep,
                    @"myTextures/ConcreteFloor.jpg");
                //
                // Front Wall
                tW[1] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(150, 0, (trainDepth * drawStep)),
                    (2 * 75), (4 * 75), 0), 75, 75, 0,
                    @"myTextures/TilesOrnate_Front.jpg");
                tW[2] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(0, 0, (trainDepth * drawStep)),
                    (1 * 75), (4 * 75), 0), 75, 75, 75,
                    @"myTextures/TilesOrnate_Front.jpg");
                tW[3] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(75, 225, (trainDepth * drawStep)),
                    (1 * 75), (1 * 75), 0), 75, 75, 75,
                    @"myTextures/TilesOrnate_Front.jpg");
                //
                // Door 
                _door = new Door(device3d, true,
                    new threeAxisRectangle(new Vector3(74, 0, (trainDepth * drawStep)), 77, 226, 0),
                    77, 226, 0, @"myTextures/door.jpg", 1.8f);
                _door.speed = 0.04f;
                //
                // Right Wall
                tW[4] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(0, 0, 0),
                    0, (trainHeight * drawStep), (trainDepth * drawStep)), drawStep, drawStep, drawStep,
                    @"myTextures/TilesOrnate.jpg");
                //
                // Left Wall
                tW[5] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3((trainWidth * drawStep), 0, 0),
                    0, (trainHeight * drawStep), (trainDepth * drawStep)), drawStep, drawStep, drawStep,
                    @"myTextures/TilesOrnate.jpg");
                //
                // Back Wall
                tW[6] = new textureWall(device3d, false, new threeAxisRectangle(new Vector3(0, 0, 0),
                    (trainWidth * drawStep), (trainHeight * drawStep), 0), drawStep, drawStep, drawStep,
                    @"myTextures/TilesOrnate.jpg");
                //
                // Top 
                tW[7] = new textureWall(device3d, true, new threeAxisRectangle(new Vector3(0, (trainHeight * drawStep), 0),
                    300, 0, 300), 300, 0, 300,
                    @"myTextures/OrnamentsRound.jpg");
                //
                // Carpet
                tW[8] = new textureWall(device3d, true,
                    new threeAxisRectangle(new Vector3(25, 0.1f, 50), 250, 0, 150),
                    250, 0, 150, @"myTextures/PersianCarpets.jpg");
                //
                // right Tableau
                tW[9] = new textureWall(device3d, true,
                    new threeAxisRectangle(new Vector3(1, (trainHeight / 2) * drawStep, (trainDepth / 3) * drawStep), 0, 90, 100),
                    0, 90, 100, @"myTextures/rightTableau.jpg");
                //
                // left Tableau
                tW[10] = new textureWall(device3d, true,
                    new threeAxisRectangle(new Vector3((trainWidth * drawStep) - 1, ((trainHeight / 2) * drawStep) - 20, (trainDepth / 3) * drawStep),
                    0, 100, 75), 0, 100, 75, @"myTextures/leftTableau.jpg");
                //
                // window Frame
                tW[11] = new textureWall(device3d, true,
                    new threeAxisRectangle(new Vector3(((trainWidth / 2) * drawStep), ((trainHeight / 2) * drawStep) - 20, 0.1f),
                    100, 75, 0), 100, 75, 0, @"myTextures/view_out_our_window.png");
                //
                // window
                _window = new window(device3d, true, openTranslationVector.Right, 40f,
                    new threeAxisRectangle(new Vector3(((trainWidth / 2) * drawStep) + 50.8f, ((trainHeight / 2) * drawStep) - 15, 0.2f),
                    46, 67, 0), 46, 67, 0, @"myTextures/window.png", true);

                #region road Textures
                roadTextures = new textureWall[3];
                roadTextures[0] = new textureWall(device3d, false,
                    new threeAxisRectangle(new Vector3(50, 1, 305), 150, 0, 900),
                    150, 0, 150, @"myTextures/FloorHerringbone.jpg");
                roadTextures[1] = new textureWall(device3d, false,
                    new threeAxisRectangle(new Vector3(200, 1, 500), 600, 0, 150),
                    150, 0, 150, @"myTextures/FloorHerringbone.jpg");
                roadTextures[2] = new textureWall(device3d, false,
                   new threeAxisRectangle(new Vector3(-550, 1, 500), 600, 0, 150),
                   150, 0, 150, @"myTextures/FloorHerringbone.jpg");
                #endregion
                #endregion     
                #region Mesh3Ds
                //
                // Mesh3Ds
                chairMesh3d = new Mesh3D(device3d, "MALLETCH.x", new Vector3(0.025f, 0.025f, 0.025f),
                    CustomVertex.PositionTextured.Format);
                dinerTableMesh3d = new Mesh3D(device3d, "DinerTable.x", new Vector3(1.2f, 1.2f, 1.2f),
                    CustomVertex.PositionTextured.Format);
                pottryMesh3d = new Mesh3D(device3d, "POTTRY2.x", new Vector3(3f, 3f, 3f),
                    CustomVertex.PositionTextured.Format);
                Sky = new SkyBox(device3d, "skybox.x", new Vector3(300.5f, 200.5f, 300f),
                    CustomVertex.PositionTextured.Format);
                extLump = new Mesh3D(device3d, "Lump1.x", new Vector3(3f, 2.5f, 3f),
                    CustomVertex.PositionTextured.Format);
                roadBarriers = new Mesh3D(device3d, "Barriers.x", new Vector3(1f, 1f, 1f),
                    CustomVertex.PositionTextured.Format);
                #endregion
                #region SplashScreen objects
                //
                // SplashScreen
                //
                btnStart = new SplashButton[1];
                btnStart[0] = new SplashButton((Form)device3d.CreationParameters.FocusWindow, device3d, new Point((width / 2) - 125, height - 300), new Size(250, 100),
                         @"SplashScreen\START_Normal.png", @"SplashScreen\START_MouseOver.png",
                         @"SplashScreen\START_MouseDown.png", @"SplashScreen\START_Disable.png");
                sps = new SplashScreen(device3d, @"SplashScreen\SplashScreen.png", btnStart);
                #endregion
                #region SoundBox
                //
                // sounds
                //
                doorOpen_Sound = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow,
                                    @"mySounds\door_open.wav");
                doorClose_Sound = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow,
                                    @"mySounds\door_close.wav");
                windowOpen_Sound = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow,
                                    @"mySounds\folding-door-open.wav");
                windowClose_Sound = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow,
                                    @"mySounds\folding-door-close.wav");
                walk_Sound = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow,
                                    @"mySounds\walk.wav");
                background_Music = new SoundBox((System.Windows.Forms.Form)device3d.CreationParameters.FocusWindow);
                background_Music.SetBackMusic(@"mySounds\backMusic.mp3");
                background_Music.backMusicVolume = -700;
                #endregion
                //
                //
                //
                device3d.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
                device3d.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
                device3d.SamplerState[0].AddressU = TextureAddress.Mirror;
                device3d.SamplerState[0].AddressV = TextureAddress.Mirror;
                //
                // 
                //
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Textures or Mesh3Ds are not OK.");
                System.Environment.Exit(0);
            }
        }

        public void OpenCloseDoor()
        {
            if (_door != null)
                _door.openClose(doorOpen_Sound, doorClose_Sound);
        }

        public void OpenCloseWindow()
        {
            if (_window != null)
                _window.openClose(windowOpen_Sound, windowClose_Sound);
        }

        #region Text
        private Microsoft.DirectX.Direct3D.Font text_Resulation;
        private Microsoft.DirectX.Direct3D.Font text_Framerate;
        private Microsoft.DirectX.Direct3D.Font text_Position;
        private Microsoft.DirectX.Direct3D.Font text_Target;
        private Microsoft.DirectX.Direct3D.Font text_exit;
        /// <summary>
        /// initialize tex
        /// </summary>
        /// <param name="device"></param>
        public void initialize_text(Device device)
        {
            //
            // Create a Font for any Text in Display
            //
            System.Drawing.Font sysFont = new System.Drawing.Font("tahoma", 14f, FontStyle.Bold);
            //
            //initialize text Resulation
            text_Resulation = new Microsoft.DirectX.Direct3D.Font(device, sysFont);
            //initialize text Framerate
            text_Framerate = new Microsoft.DirectX.Direct3D.Font(device, sysFont);
            //initialize text Camera Position
            text_Position = new Microsoft.DirectX.Direct3D.Font(device, sysFont);
            //initialize text Camera Target
            text_Target = new Microsoft.DirectX.Direct3D.Font(device, sysFont);
            //initialize text exit ESC
            text_exit = new Microsoft.DirectX.Direct3D.Font(device, sysFont);
        }
        /// <summary>
        /// draw text
        /// </summary>
        /// <param name="device"></param>
        public void draw_text()
        {
            //
            // Set Graphic Resulations
            //
            text_Resulation.DrawText(null, string.Format("Resulation ({0} , {1})", this.width, this.height), new Point(5, 5), Color.Red);
            //
            // Display ESC key's
            //
            text_exit.DrawText(null, "Please press ESC key's for exit.", new Point(5, 30), Color.LightBlue);
            //
            // Display FPS number
            //
            text_Framerate.DrawText(null, string.Format("Framerate : {0:0.00} fps", Framerate.UpdateFramerate()), new Point(5, 55), Color.LightGreen);
            //
            // Display Camera Position and Target
            //
            text_Position.DrawText(null, string.Format("Camera Position = ({0:0.0}x , {1:0.0}y , {2:0.0}z)",
                CameraPosition.X, CameraPosition.Y, CameraPosition.Z), new Point(5, 80), Color.Aqua);
            text_Target.DrawText(null, string.Format("Camera Target = ({0:0.0}x , {1:0.0}y , {2:0.0}z)",
                CameraTarget.X, CameraTarget.Y, CameraTarget.Z), new Point(5, 105), Color.Aqua);
        }
        #endregion  
    }

}

