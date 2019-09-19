using RasPiTankAndroid_CSharp.ViewModels;
using Xamarin.Forms;

namespace RasPiTankAndroid_CSharp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            BindingContext = new MainPageViewModel();
        }
    }
}
