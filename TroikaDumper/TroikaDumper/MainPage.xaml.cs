using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Nfc; //!
using Plugin.Nfc.Abstractions;

namespace TroikaDumper
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //connector 
        INfc MyCrossNfc;

        public MainPage()
        {
            MyCrossNfc = DependencyService.Get<INfc>();


            InitializeComponent();
        }

        // ..
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (1==1)//(!await MyCrossNfc.IsAvailableAsync())
            {
                lblStatus.Text = "NFC Reading not supported";
                return;
            }

            if (1==1)//(!await MyCrossNfc.IsEnabledAsync())
            {
                lblStatus.Text = "NFC Reader not enabled. Please turn it on in the settings.";
                return;
            }

            //await CrossNfc.Current.StartListeningAsync();
            lblStatus.Text = "Contact a NFC tag to read it.";
        }

        // ..
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await MyCrossNfc.StopListeningAsync();
        }

        // ..
        public void Button_OnClicked(object sender, EventArgs e)
        {
            MyCrossNfc.StartListeningAsync();
        }
    }
}
