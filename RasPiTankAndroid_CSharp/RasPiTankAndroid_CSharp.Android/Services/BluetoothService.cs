using Android.App;
using Android.Bluetooth;
using Android.Content;
using Java.IO;
using RasPiTankAndroid_CSharp.Droid.Services;
using RasPiTankAndroid_CSharp.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothService))]
namespace RasPiTankAndroid_CSharp.Droid.Services
{
    class BluetoothService : IBluetoothService
    {
        private const string UUID = "00001101-0000-1000-8000-00805F9B34FB";
        private const int timeOut = 2000;

        private BluetoothSocket socket = null;
        private Stream inputStream;
        private Stream outputStream;

        public async Task<bool> Start(string hostName)
        {
            
            foreach (BluetoothDevice element in BluetoothAdapter.DefaultAdapter.BondedDevices)
            {
                if (element.Name == hostName)
                {
                    socket = element.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString(UUID));
                    break;
                }
            }

            //nullではないとき
            if (socket != null)
            {
                int time = timeOut;

                return await Task.Run(async () =>
                {
                    try
                    {
                        socket.Connect();
                    }
                    catch (Java.IO.IOException)
                    {
                        socket.Close();
                        return false;
                    }

                    int waitTime = 100;
                    while (socket.IsConnected == false)
                    {
                    //タイムアウト
                    if (time <= 0)
                        {
                            socket.Close();
                            return false;
                        }

                        await Task.Delay(waitTime);
                        time -= waitTime;
                    }

                    inputStream = socket.InputStream;
                    outputStream = socket.OutputStream;
                    return true;

                }
                ).ContinueWith(
                    (t) => {
                        return t.Exception != null;
                    }
                );
            }

            else
            {
                //ユーザーにスマホのBluetoothを有効にしてもらう
                Activity activity = (Activity)Forms.Context;
                activity.StartActivityForResult(new Intent(BluetoothAdapter.ActionRequestEnable), 1);

                return false;
            }
            
        }

        public async Task End()
        {
            await Task.Delay(1000);

            socket?.Close();
        }

        public async Task<byte[]> ReadAsync()
        {
            ByteArrayOutputStream stream = new ByteArrayOutputStream();

            if (inputStream != null)
            {
                int interval = 100;
                int time = timeOut;
                while (!inputStream.IsDataAvailable() && time > 0)
                {
                    await Task.Delay(interval);
                    time -= interval;
                    if (time <= 0) return null;
                }
                
                byte[] buffer = new byte[1024];
                if (inputStream.IsDataAvailable())
                {
                    int length = inputStream.Read(buffer, 0, buffer.Count());
                    if (length > 0)
                    {
                        stream.Write(buffer, 0, length);
                    }
                }
            }

            return stream.Size() != 0 ? stream.ToByteArray() : null;
        }

        public async Task<bool> WriteAsync(string command)
        {
            try
            {
                byte[] _command = System.Text.Encoding.UTF8.GetBytes(command);

                await outputStream.WriteAsync(_command, 0, _command.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}