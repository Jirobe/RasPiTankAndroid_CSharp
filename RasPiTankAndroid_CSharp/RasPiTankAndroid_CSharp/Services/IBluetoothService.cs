using System.Threading.Tasks;

namespace RasPiTankAndroid_CSharp.Services
{
    public interface IBluetoothService
    {
        Task<bool> Start(string hostName);

        Task End();

        Task<byte[]> ReadAsync();

        Task<bool> WriteAsync(string command);
    }
}
