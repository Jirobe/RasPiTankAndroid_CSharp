using RasPiTankAndroid_CSharp.Models;
using RasPiTankAndroid_CSharp.Services;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace RasPiTankAndroid_CSharp.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        private bool isConnect = false;
        private const string errorMessage = "Error:再接続か再起動をお願いします";

        private string receivedData;
        public string ReceivedData
        {
            get { return receivedData; }
            set { SetProperty(ref receivedData, value); }
        }

        public ICommand StartBluetoothCommand => new Command(async () =>
        {
            if (isConnect == false)
            {
                try
                {
                    bool result = await DependencyService.Get<IBluetoothService>().Start("RasPiTank");

                    isConnect = true;
                }
                catch
                {
                    isConnect = false;
                    ReceivedData = errorMessage;
                }
            }
        });

        public ICommand EndBluetoothCommand => new Command(async () =>
        {
            try
            {
                isConnect = false;
                await DependencyService.Get<IBluetoothService>().End();

            }
            catch
            {
                ReceivedData = errorMessage;
            }
        });

        public ICommand LeftForwardCommand => new Command(async () =>
        {
            string command = "$DIGITAL:L,F,A#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public ICommand LeftStopCommand => new Command(async () =>
        {
            string command = "$DIGITAL:L,F,S#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public ICommand LeftBackCommand => new Command(async () =>
        {
            string command = "$DIGITAL:L,B,A#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public ICommand RightForwardCommand => new Command(async () =>
        {
            string command = "$DIGITAL:R,F,A#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public ICommand RightStopCommand => new Command(async () =>
        {
            string command = "$DIGITAL:R,F,S#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public ICommand RightBackCommand => new Command(async () =>
        {
            string command = "$DIGITAL:R,B,A#";
            if (isConnect) await MainPageModel.WriteAsync(command);
        });

        public MainPageViewModel()
        {
            new Timer(new TimerCallback(
                async (o) =>
                {
                    try
                    {
                        if (isConnect)
                        {
                            byte[] data = await DependencyService.Get<IBluetoothService>().ReadAsync();
                            ReceivedData = data != null ? System.Text.Encoding.UTF8.GetString(data) : ReceivedData;
                        }

                        else
                        {
                            ReceivedData = "none";
                        }
                    }
                    catch
                    {
                        ReceivedData = errorMessage;
                    }
                }), null, 0, 100);
        }
    }
}
