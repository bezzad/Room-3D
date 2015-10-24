using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;
using System.Collections;
using System.Windows.Forms;

namespace GraphicTools
{
    public class SoundBox
    {
        private Device soundDevice;
        private SecondaryBuffer sound;
        private ArrayList soundList;

        private Audio backMusic;
        private double lastMusicPosition = 0;
        private Timer timer;

        #region Constructors
        public SoundBox(System.Windows.Forms.Form frm, string soundFileName, string backMusicFileName)
        {
            soundDevice = new Device();
            soundDevice.SetCooperativeLevel(frm, CooperativeLevel.Normal);
            BufferDescription description = new BufferDescription();
            description.ControlVolume = true;
            description.ControlEffects = false;
            sound = new SecondaryBuffer(soundFileName, description, soundDevice);
            soundList = new ArrayList();
            //
            // set back Music
            //
            SetBackMusic(backMusicFileName);
        }
        public SoundBox(System.Windows.Forms.Form frm, string soundFileName)
        {
            soundDevice = new Device();
            soundDevice.SetCooperativeLevel(frm, CooperativeLevel.Normal);
            BufferDescription description = new BufferDescription();
            description.ControlVolume = true;
            description.ControlEffects = false;
            sound = new SecondaryBuffer(soundFileName, description, soundDevice);
            soundList = new ArrayList();
        }
        public SoundBox(System.Windows.Forms.Form frm)
        {
            soundDevice = new Device();
            soundDevice.SetCooperativeLevel(frm, CooperativeLevel.Normal);
            soundList = new ArrayList();
        }
        #endregion

        #region SoundBox
        public void SetSound(string soundFileName)
        {
            BufferDescription description = new BufferDescription();
            description.ControlVolume = true;
            description.ControlEffects = false;
            sound = new SecondaryBuffer(soundFileName, description, soundDevice);
        }
        public void playSound()
        {
            if (sound != null)
            {
                SecondaryBuffer newShotSound = sound.Clone(soundDevice);
                soundList.Add(newShotSound);
                sound.Play(0, BufferPlayFlags.Default);
                SoundAffairs();
            }
        }
        public void playSound(BufferPlayFlags bufPlayFlag)
        {
            if (sound != null)
            {
                SecondaryBuffer newShotSound = sound.Clone(soundDevice);
                soundList.Add(newShotSound);
                sound.Play(0, bufPlayFlag);
                SoundAffairs();
            }
        }
        private void SoundAffairs()
        {
            if (soundList.Count > 0)
            {
                for (int i = soundList.Count - 1; i > -1; i--)
                {
                    SecondaryBuffer currentSound = (SecondaryBuffer)soundList[i];
                    if (!currentSound.Status.Playing)
                    {
                        currentSound.Dispose();
                        soundList.RemoveAt(i);
                    }
                }
            }
        }
        /// <summary>
        /// the volume number must be between -65536 ~ 0
        /// </summary>
        public int soundVolume
        { get { return sound.Volume; } set { sound.Volume = value; } }
        #endregion

        #region backMusicBox
        public void SetBackMusic(string musicFileName)
        {
            backMusic = new Audio(musicFileName);
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new System.EventHandler(timer_Tick);
        }
        private void timer_Tick(object sender, System.EventArgs e)
        {
            // if music is in end and stoped
            if (backMusic.CurrentPosition == lastMusicPosition)
            {
                backMusic.SeekCurrentPosition(0, SeekPositionFlags.AbsolutePositioning);
            }
            lastMusicPosition = backMusic.CurrentPosition;
        }

        public void playBackMusic()
        {
            if (backMusic != null)
            {
                backMusic.Play();
                timer.Start();
            }
        }
        public void stopBackMusic()
        {
            if (backMusic != null)
            {
                timer.Stop();
                backMusic.Stop();
            }
        }
        public void pauseBackMusic()
        {
            if (backMusic != null)
            {
                timer.Stop();
                backMusic.Pause();
            }
        }
        /// <summary>
        /// the volume number must be between -65536 ~ 0
        /// </summary>
        public int backMusicVolume
        { get { return backMusic.Volume; } set { backMusic.Volume = value; } }
        
        #endregion
    }
}
