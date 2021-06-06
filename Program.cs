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
    class Program
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern Boolean AllocConsole();
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //Dlls for getting key input from anywhere
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        //some variables for key press stuff
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x0100;
        static LowLevelKeyboardProc _proc = HookCallback;
        static IntPtr _hookID = IntPtr.Zero;
        static char ToggleBind;
        public static bool Bindpressed = false;
        public static int ClickPerMS;

        static void Main(string[] args)
        {
            NativeMethods.AllocConsole();
            Console.WriteLine("Starting macro!");
            Console.WriteLine("please enter a letter bind for the macro (HAS TO BE THE CAPITAL LETTER OF IT)");
            ToggleBind = Convert.ToChar(Console.ReadLine());
            Console.WriteLine("enter the MS between each click");
            ClickPerMS = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"{ToggleBind} set as bind");
            Thread bindChecker = new Thread(() => InitiateKeyPressLog());
            Thread clicker = new Thread(() => clickerPart.macroClicker());
            bindChecker.Start();
            clicker.Start();
        }

        public static void InitiateKeyPressLog()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (Convert.ToChar((Keys)vkCode) == ToggleBind && !Bindpressed)
                {
                    Bindpressed = true;
                }
                else
                {
                    Bindpressed = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
