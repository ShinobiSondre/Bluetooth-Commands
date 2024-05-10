using WindowsInput;
using System.Diagnostics;

namespace SpeechCommands.Desktop_Actions
{
     class Keyboard_Events
    {
        static InputSimulator inputSimulator = new InputSimulator();
        KeyboardSimulator keyboardSimulator = new KeyboardSimulator(inputSimulator);
        MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);

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

        public void moveMouseTrackPad(double deltaX, double deltaY)
        {

            // Move the mouse to the new position

            int deltaX1 = (int)deltaX;
            int deltaY1 = (int)deltaY;



            int newX = Cursor.Position.X;
            int newY = Cursor.Position.Y;

            if(deltaX>298 && deltaY>132.21875 && deltaY < 317)
            {
                newX = Cursor.Position.X + 10;
                //newY = Cursor.Position.Y + (int)deltaY / 24;
            }else if(deltaY > 132.21875 && deltaY< 317)
            {
                newX = Cursor.Position.X -10;
            }

            if (deltaY < 132.21875)
            {
                newY = Cursor.Position.Y-10;
                //newY = Cursor.Position.Y + (int)deltaY / 24;
            }
            else if (deltaY > 317)
            {
                newY = Cursor.Position.Y+10;
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

    }
}
