using System;

namespace Bluetooth_Commands
{
    class Program
    {
        static void Main(string[] args)
        {
            BluetoothServer bluetooth = new BluetoothServer();
            bluetooth.StartServer();

            Console.WriteLine("Press 'q' to stop the server and exit.");

            while (true)
            {
                // Wait for user input
                int key = Console.Read();

                // Check if the user pressed 'q' to stop the server and exit
                if (key == 'q')
                {
                    bluetooth.StopServer();
                    break; // Exit the loop and end the application
                }
            }
        }
    }
}