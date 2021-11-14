using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TroikaDumper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        /*
        protected override void OnStart()
        {
           //
        }

        protected override void OnSleep()
        {
           // 
        }

        protected override void OnResume()
        {
           //
        }
        */

       public static async Task DisplayAlertAsync(string msg) =>
            await Device.InvokeOnMainThreadAsync(async () => 
              await Current.MainPage.DisplayAlert("message from service", msg, "ok")
       );
    }
}
