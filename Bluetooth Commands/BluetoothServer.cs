using System;
using System.IO;
using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace Bluetooth_Commands
{
    using SpeechCommands.Desktop_Actions;

    class BluetoothServer
    {
        private BluetoothListener bluetoothListener;
        private Keyboard_Events keyboard = new Keyboard_Events();
        private VolumeController volumeController = new VolumeController();
        private bool isServerRunning = false;

        public void StartServer()
        {
            var serviceClassId = BluetoothService.SerialPort;

            bluetoothListener = new BluetoothListener(serviceClassId);
            bluetoothListener.Start();
            isServerRunning = true;

            Console.WriteLine("Bluetooth server started. Waiting for clients...");

            Task.Run(() => AcceptClients());
        }

        private void AcceptClients()
        {
            while (true)
            {
                try
                {
                    BluetoothClient client = bluetoothListener.AcceptBluetoothClient(); // This is a blocking call
                    Console.WriteLine("Client connected!");

                    // Handle client communication in a separate thread
                    Task.Run(() => HandleClient(client));
                }
                catch (ObjectDisposedException)
                {
                    // The listener has been disposed, indicating the server is shutting down
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting client: " + ex.Message);
                }
            }
        }

        private void HandleClient(BluetoothClient client)
        {
            try
            {
                Stream peerStream = client.GetStream();

                while (isServerRunning)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytes = peerStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            string commandReceived = System.Text.Encoding.UTF8.GetString(buffer, 0, bytes);
                            Console.WriteLine("Received: " + commandReceived);
                            // TODO: Handle the command received as needed

                            // Handle different commands
                            HandleCommand(commandReceived);
                        }
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Client disconnected");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error handling client communication: " + ex.Message);
                        
                    }
                }
            }
            finally
            {
                // Close the client connection
                client.Close();
            }
        }

        private void HandleCommand(string commandReceived)
        {

         

            if (commandReceived.Contains("Mouse Move"))
            {
                try
                {
                    string numberedString = commandReceived.Split(": ")[1];
                    double x = Double.Parse(numberedString.Split(",")[0]);
                    double y = Double.Parse(numberedString.Split(",")[1]);
                    keyboard.moveMouseTrackPad(x, y);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }


            switch (commandReceived)
            {
                case "Off":
                keyboard.SleepComputer();
                    break;
                case "Play":
                    keyboard.play();
                    break;
                case "Stop":
                    keyboard.play();
                    break;
                case "Skip":
                    keyboard.skip();
                    break;
                case "Previous":
                    keyboard.regress();
                    break;
                case "Focus":
                    keyboard.click();
                    break;
                case "Fullscreen":
                    keyboard.fullscreen();
                    break;
                case "Volume Up":
                    float currentVolume = volumeController.GetCurrentVolume();
                    float newVolume = currentVolume + 25;
                    if (newVolume > 100.0f) newVolume = 100.0f; // Limit volume to 100%
                    volumeController.SetVolume(newVolume);
                    break;
                case "Volume Down":
                    currentVolume = volumeController.GetCurrentVolume();
                    newVolume = currentVolume - 25;
                    if (newVolume < 0.0f) newVolume = 0.0f; // Limit volume to 0%
                    volumeController.SetVolume(newVolume);
                    break;
                case "Speed Up":
                    keyboard.setMouseMoveSpeed(true);
                    break;
                case "Speed Down":
                    keyboard.setMouseMoveSpeed(false);
                    break;
                case "Chrometab": 
                    keyboard.openChromeTab(commandReceived.Split(",")[1]);
                    break;
                default:
                    //Console.WriteLine("Unknown command: " + commandReceived);
                    break;
            }
        }

        public void StopServer()
        {
            isServerRunning = false;
            bluetoothListener.Stop();
        }
    }
}
