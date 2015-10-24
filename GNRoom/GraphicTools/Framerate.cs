using System;

namespace GraphicTools
{
    public class Framerate
    {
        static int LastTickCount = 1;
        static int Frames = 0;
        static float LastFrameRate = 0;
        
        /// <summary>
        /// Frame Per Second of render's
        /// </summary>
        /// <returns>float fps number's</returns>
        public static float UpdateFramerate()
        {
            Frames++;
            if (Math.Abs(Environment.TickCount - LastTickCount) > 1000)
            {
                LastFrameRate = (float)Frames * 1000 / Math.Abs(Environment.TickCount - LastTickCount);
                LastTickCount = Environment.TickCount;
                Frames = 0;
            }
            return LastFrameRate;
        }
    }
}
