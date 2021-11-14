using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using TroikaDumper.Droid.Services;

using Activity = Android.App.Activity;
using PendingIntent = Android.App.PendingIntent;
using ProgressDialog = Android.App.ProgressDialog;
using Context = Android.Content.Context;
using DialogInterface = Android.Content.DialogInterface;
using Intent = Android.Content.Intent;
using IntentFilter = Android.Content.IntentFilter;
using NfcAdapter = Android.Nfc.NfcAdapter;
using NfcManager = Android.Nfc.NfcManager;
using Tag = Android.Nfc.Tag;
using MifareClassic = Android.Nfc.Tech.MifareClassic;
using Bundle = Android.OS.Bundle;
using FloatingActionButton = Android.Widget.Button;//.FloatingActionButton;
//using AppCompatActivity = Android.Support.V7.App.AppCompatActivity;
using Toolbar = Android.Widget.Toolbar;
using View = Android.Views.View;
using Menu = Android.Views.Menu;
//using MenuItem = Android.Views.MenuItem;
using TextView = Android.Widget.TextView;
using cc.troikadumper;

namespace TroikaDumper.Droid
{

    [Activity(Label = "TroikaDumper", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal const int REQUEST_OPEN_DUMP = 1;
        internal const string INTENT_READ_DUMP = "cc.troikadumper.INTENT_READ_DUMP";

        protected internal FloatingActionButton btnLoad;
        protected internal FloatingActionButton btnWrite;
        protected internal TextView info;

        protected internal NfcAdapter nfcAdapter;
        protected internal Dump dump;
        protected internal bool writeMode = false;
        protected internal ProgressDialog pendingWriteDialog;


        public CardReader cardReader;

        public NfcReaderFlags READER_FLAGS = NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck;

        private void EnableReaderMode()
        {
            var nfc = NfcAdapter.GetDefaultAdapter(this);
            if (nfc != null) nfc.EnableReaderMode(this, cardReader, READER_FLAGS, null);
        }

        private void DisableReaderMode()
        {
            var nfc = NfcAdapter.GetDefaultAdapter(this);
            if (nfc != null) nfc.DisableReaderMode(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            cardReader = new CardReader();

            EnableReaderMode();

            LoadApplication(new App());

            // new

            
            //setContentView(R.layout.activity_main);
            //info = (TextView)findViewById(R.id.textView);
            //Toolbar toolbar = (Toolbar)findViewById(R.id.toolbar);
            //setSupportActionBar(toolbar);
            /*
            NfcManager nfcManager = (NfcManager)getSystemService(Context.NFC_SERVICE);

            nfcAdapter = nfcManager.getDefaultAdapter();

            if (nfcAdapter == null)
            {
                info.setText(R.@string.error_no_nfc);
            }

            if (nfcAdapter != null && !nfcAdapter.isEnabled())
            {
                info.setText(R.@string.error_nfc_is_disabled);
            }

            pendingWriteDialog = new ProgressDialog(MainActivity.this);
            pendingWriteDialog.setIndeterminate(true);
            pendingWriteDialog.setMessage("Waiting for card...");
            pendingWriteDialog.setCancelable(true);
            pendingWriteDialog.setOnCancelListener(new OnCancelListenerAnonymousInnerClass(this));

            //btnWrite = (FloatingActionButton)findViewById(R.id.btn_write);

            //btnWrite.setOnClickListener(new OnClickListenerAnonymousInnerClass(this));
            
            //btnLoad = (FloatingActionButton)findViewById(R.id.btn_load);
            
            //btnLoad.setOnClickListener(new OnClickListenerAnonymousInnerClass2(this));

            //Intent startIntent = GetIntent();
            
            //if (startIntent != null && startIntent.getAction().Equals(NfcAdapter.ACTION_TECH_DISCOVERED))
            //{
            //    handleIntent(startIntent);
            //}
            */
        }

        protected override void OnPause()
        {
            base.OnPause();
            DisableReaderMode();
        }

        protected override void OnResume()
        {
            base.OnResume();
            EnableReaderMode();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
   
}