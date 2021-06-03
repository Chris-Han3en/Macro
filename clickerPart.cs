using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace Macro
{
    class clickerPart
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        public static void macroClicker()
        {
            while (true)
            {
                if (Program.Bindpressed)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(Program.ClickPerMS);
                }
            }
        }
    }
}
