using WindowsInput;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Interop.UIAutomationClient;
using WindowsInput.Native;

namespace SpeechCommands.Desktop_Actions
{
     class Keyboard_Events
    {
        static InputSimulator inputSimulator = new InputSimulator();
        KeyboardSimulator keyboardSimulator = new KeyboardSimulator(inputSimulator);
        MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);
        ScreenCapture screenCapture = new ScreenCapture();

        int _speed = 10;

        public void pressKey(WindowsInput.Native.VirtualKeyCode virtualKeyCode) {
            keyboardSimulator.KeyPress(virtualKeyCode);
        }

        public void fullscreen()
        {
            pressKey(WindowsInput.Native.VirtualKeyCode.VK_F);
        }

        public void play()
        {
            pressKey(WindowsInput.Native.VirtualKeyCode.SPACE);
        }

        public void setMouseMoveSpeed(Boolean up) 
        {
            if(up)
            {
                _speed=10;
            }
            else
            {
                _speed=2;
            }
        }

        public void Search(bool search = false, string text = "")
        {   
            if (search)
            {
                pressKey(WindowsInput.Native.VirtualKeyCode.RETURN);
            }
            {
                inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                Thread.Sleep(100);

                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DELETE);
                Thread.Sleep(100);

                inputSimulator.Keyboard.TextEntry(text);

            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        /*public static void Input(string input)
        {
            // Get the currently active window handle
            IntPtr activeWindowHandle = GetForegroundWindow();

            // Get the process ID of the active window
            GetWindowThreadProcessId(activeWindowHandle, out uint processId);
            Process activeProcess = Process.GetProcessById((int)processId);

            // Check if the active window is Google Chrome
            if (activeProcess.ProcessName.Contains("chrome"))
            {
                // Initialize UI Automation
                IUIAutomation automation = new CUIAutomation();
                IUIAutomationElement chromeWindow = automation.ElementFromHandle(activeWindowHandle);

                // Get the focused element
                IUIAutomationElement focusedElement = automation.GetFocusedElement();

                if (focusedElement != null)
                {
                    // Get the name and control type of the focused elemen

                    var controlType = focusedElement.CurrentControlType;
                    var name = focusedElement.CurrentName;

                    Console.WriteLine("Focused Element:");
                    Console.WriteLine("Name: " + name);
                    Console.WriteLine("Control Type: " + controlType);

                    // If the focused element is an input field, you can set its value directly (if supported)
                    if (controlType == UIA_ControlTypeIds.UIA_EditControlTypeId)
                    {
                        IUIAutomationValuePattern valuePattern;

                        var pattern = focusedElement.GetCurrentPattern(c);

                        focusedElement.GetCurrentPattern(1);
                        valuePattern.SetValue("Your text here"); // Set the text directly
                    }
                }
                else
                {
                    Console.WriteLine("No focused element found in the Chrome window.");
                }
            }
            else
            {
                Console.WriteLine("The active window is not Google Chrome.");
            }
        }*/

        public void moveMouseTrackPad(double deltaX, double deltaY)
        {

            // Move the mouse to the new position
            int newX = Cursor.Position.X;
            int newY = Cursor.Position.Y;

            if(deltaX>298 && deltaY>132.21875 && deltaY < 317)
            {
                newX = Cursor.Position.X + _speed;
                //newY = Cursor.Position.Y + (int)deltaY / 24;
            }else if(deltaY > 132.21875 && deltaY< 317)
            {
                newX = Cursor.Position.X - _speed;
            }

            if (deltaY < 132.21875)
            {
                newY = Cursor.Position.Y - _speed;
                //newY = Cursor.Position.Y + (int)deltaY / 24;
            }
            else if (deltaY > 317)
            {
                newY = Cursor.Position.Y + _speed;
            }

            Cursor.Position = new Point(newX, newY);
        }

        public void moveMouse(double x, double y)
        {
            double normalizedX = 0;
            double normalizedY = 0;

            if (Screen.AllScreens.Length > 1)
            {
                // Get the primary screen's width and height
                var primaryScreenWidth = Screen.PrimaryScreen.Bounds.Width;
                var primaryScreenHeight = Screen.PrimaryScreen.Bounds.Height;

                // Get the second screen assuming it's the one after the primary
                Screen secondScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary) ?? Screen.PrimaryScreen;

                // Calculate the normalized position for the center of the second screen
                // Here we need to use the screen's Bounds which includes its relative position to the primary screen
                normalizedX = x * (65535.0 / primaryScreenWidth);
                normalizedY = y * (65535.0 / primaryScreenHeight);

                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
            }
            else
            {

                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;

                normalizedX = x * (65535.0 / screenWidth);
                normalizedY = y * (65535.0 / screenHeight);
                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
                play();
            }
        }

        public void skipIntro()
        {
            var position = ScreenCapture.FindTextOnScreen("Skip");
            if (position.X != null)
            {
                Console.WriteLine("X: " + position.X + " Y: " + position.Y);
                mouseSimulator.MoveMouseTo(position.X, position.Y);
                Thread.Sleep(50);
                mouseSimulator.LeftButtonDoubleClick();
            }

        }

        public void skip()
        {
            for (int i = 0; i < 1000; i++)
            {
                pressKey(WindowsInput.Native.VirtualKeyCode.RIGHT);
            }

            double normalizedX = 0;
            double normalizedY = 0;

            if (Screen.AllScreens.Length > 1)
            {
                // Get the primary screen's width and height
                var primaryScreenWidth = Screen.PrimaryScreen.Bounds.Width;
                var primaryScreenHeight = Screen.PrimaryScreen.Bounds.Height;

                // Get the second screen assuming it's the one after the primary
                Screen secondScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary) ?? Screen.PrimaryScreen;

                // Calculate the normalized position for the center of the second screen
                // Here we need to use the screen's Bounds which includes its relative position to the primary screen
                normalizedX = (secondScreen.Bounds.Left + secondScreen.Bounds.Width / 2) * (65535.0 / primaryScreenWidth);
                normalizedY = (secondScreen.Bounds.Top + secondScreen.Bounds.Height / 2) * (65535.0 / primaryScreenHeight);

                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
                Thread.Sleep(1900);
                mouseSimulator.LeftButtonDoubleClick();
                Thread.Sleep(500);
                play();
            }
            else
            {

                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;

                normalizedX = (screenWidth / 2) * (65535.0 / screenWidth);
                normalizedY = (screenHeight / 2) * (65535.0 / screenHeight);
                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
                Thread.Sleep(1900);
                mouseSimulator.LeftButtonDoubleClick();
                Thread.Sleep(50);
                play();
            }
        }

        public void openChromeTab(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

        }

        public void click()
        {
            mouseSimulator.LeftButtonClick();
        }


        public void focus()
        {

            double normalizedX = 0;
            double normalizedY = 0;


            if (Screen.AllScreens.Length > 1)
            {
                // Get the primary screen's width and height
                var primaryScreenWidth = Screen.PrimaryScreen.Bounds.Width;
                var primaryScreenHeight = Screen.PrimaryScreen.Bounds.Height;

                // Get the second screen assuming it's the one after the primary
                Screen secondScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary) ?? Screen.PrimaryScreen;

                // Calculate the normalized position for the center of the second screen
                // Here we need to use the screen's Bounds which includes its relative position to the primary screen
                normalizedX = (secondScreen.Bounds.Left + secondScreen.Bounds.Width / 2) * (65535.0 / primaryScreenWidth);
                normalizedY = (secondScreen.Bounds.Top + secondScreen.Bounds.Height / 2) * (65535.0 / primaryScreenHeight);

                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
                Thread.Sleep(200);
                mouseSimulator.LeftButtonDoubleClick();
            }
            else
            {
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;

                normalizedX = (screenWidth / 2) * (65535.0 / screenWidth);
                normalizedY = (screenHeight / 2) * (65535.0 / screenHeight);
                mouseSimulator.MoveMouseTo(normalizedX, normalizedY);
                Thread.Sleep(200);
                mouseSimulator.LeftButtonDoubleClick();
            }
        }


        public void regress()
        {
            for(int i = 0; i < 10; i++)
            {
                pressKey(WindowsInput.Native.VirtualKeyCode.LEFT);
            }
            
        }

        public void ShutDownComputer()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /f /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        private const int SC_MONITORPOWER = 0xF170;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int MONITOR_OFF = 2;


        public void SleepComputer()
        {
            // Turn off the screen
            SendMessage(new IntPtr(0xFFFF), WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MONITOR_OFF);

            // Put the computer to sleep
            var psi = new ProcessStartInfo("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }
    }
}
