using RasPiTankAndroid_CSharp.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RasPiTankAndroid_CSharp.Models
{
    class MainPageModel
    {
        public static async Task WriteAsync(string command)
        {
            int checksum = 0;
            int i;
            for (i = 0; i < command.Length; i++)
            {
                checksum += (int)command[i];
            }
            checksum = (~checksum + 1) & 0xFF;
            command += string.Format("{0:X2}\n", checksum);

            await DependencyService.Get<IBluetoothService>().WriteAsync(command);
        }
    }
}
