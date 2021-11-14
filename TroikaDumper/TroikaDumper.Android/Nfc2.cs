﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Plugin.Nfc.Abstractions;
using Xamarin.Forms;

[assembly: Dependency(typeof(TroikaDumper.Droid.NfcDefTag))]
namespace TroikaDumper.Droid
{
    public class NfcDefTag : INfcDefTag
    {
       
        
        public string GetInfo()
        {
            return $"Android {Build.VERSION.Release}";
        }

        public bool IsWriteable { get; }
        public NfcDefRecord[] Records { get; }

        public NfcDefTag(Ndef tag, IEnumerable<NdefRecord> records)
        {
            IsWriteable = tag.IsWritable;

            
            Records = records
                .Select(r => new AndroidNdefRecord(r))
                .ToArray();
            
        }

        /*
        public static void SetCurrentActivityResolver(Func<global::Nfc.Sample.Droid.MainActivity> p)
        {
            throw new NotImplementedException();
        }

        public static void OnNewIntent(global::Android.Content.Intent intent)
        {
            throw new NotImplementedException();
        }
        */

    }//class end


    public class AndroidNdefRecord : NfcDefRecord
    {
        // AndroidNdefRecord
        public AndroidNdefRecord(NdefRecord nativeRecord)
        {
            TypeNameFormat = GetTypeNameFormat(nativeRecord.Tnf);
            Payload = nativeRecord.GetPayload();
        }

        // GetTypeNameFormat
        private NDefTypeNameFormat GetTypeNameFormat(short nativeRecordTnf)
        {
            switch (nativeRecordTnf)
            {
                case NdefRecord.TnfAbsoluteUri:
                    return NDefTypeNameFormat.AbsoluteUri;
                case NdefRecord.TnfEmpty:
                    return NDefTypeNameFormat.Empty;
                case NdefRecord.TnfExternalType:
                    return NDefTypeNameFormat.External;
                case NdefRecord.TnfMimeMedia:
                    return NDefTypeNameFormat.Media;
                case NdefRecord.TnfUnchanged:
                    return NDefTypeNameFormat.Unchanged;
                case NdefRecord.TnfUnknown:
                    return NDefTypeNameFormat.Unchanged;
                case NdefRecord.TnfWellKnown:
                    return NDefTypeNameFormat.WellKnown;
                default:
                    break;
            }

            return NDefTypeNameFormat.Unknown;

        }//GetTypeNameFormat end

    }// AndroidNdefRecord class end

}//namespace end

/*
namespace Plugin.Nfc
{
    internal class NfcImplementation : Java.Lang.Object, INfc, NfcAdapter.IReaderCallback
    {
        private readonly NfcAdapter _nfcAdapter;

        public event TagDetectedDelegate TagDetected;

        public NfcImplementation()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
                return;

            //if (Build.VERSION.SdkInt < BuildVersionCodes.Gingerbread)
            //    return;

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(CrossNfc.CurrentActivity);
        }

        public async Task<bool> IsAvailableAsync()
        {
            var context = Application.Context;
            if (context.CheckCallingOrSelfPermission(Manifest.Permission.Nfc) != Permission.Granted)
                return false;

            return _nfcAdapter != null;
        }

        public Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(_nfcAdapter?.IsEnabled ?? false);
        }

        public async Task StartListeningAsync()
        {
            if (!await IsAvailableAsync())
                throw new InvalidOperationException("NFC not available");

            if (!await IsEnabledAsync()) // todo: offer possibility to open dialog
                throw new InvalidOperationException("NFC is not enabled");

            var activity = CrossNfc.CurrentActivity;
            var tagDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            tagDetected.AddDataType("* / * "); // убрать пробелы!!
            var filters = new[] { tagDetected };
            var intent = new Intent(activity, activity.GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(activity, 0, intent, 0);
            _nfcAdapter.EnableForegroundDispatch(activity, pendingIntent, filters, new[] { new[] { Java.Lang.Class.FromType(typeof(Ndef)).Name } });
            //_nfcAdapter.EnableReaderMode(activity, this, NfcReaderFlags.NfcA | NfcReaderFlags.NoPlatformSounds, null);
        }

        public async Task StopListeningAsync()
        {
            //_nfcAdapter?.DisableReaderMode(CrossNfc.CurrentActivity);
            _nfcAdapter?.DisableForegroundDispatch(CrossNfc.CurrentActivity);
        }

        internal void CheckForNfcMessage(Intent intent)
        {
            //if (intent.Action != NfcAdapter.ActionTagDiscovered)
            //    return;

            //var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
            //if (tag == null)
            //    return;

            //var nativeMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            //if (nativeMessages == null)
            //    return;

            //var messages = nativeMessages
            //    .Cast<NdefMessage>()
            //    .Select(m => new AndroidNdefMessage(m));
        }

        public void OnTagDiscovered(Tag tag)
        {
            try
            {
                var techs = tag.GetTechList();
                if (!techs.Contains(Java.Lang.Class.FromType(typeof(Ndef)).Name))
                    return;

                var ndef = Ndef.Get(tag);
                ndef.Connect();
                var ndefMessage = ndef.NdefMessage;
                var records = ndefMessage.GetRecords();
                ndef.Close();

                var nfcTag = new NfcDefTag(ndef, records);
                TagDetected?.Invoke(nfcTag);
            }
            catch
            {
                // handle errors
            }
        }
    }

    public class NfcDefTag : INfcDefTag
    {
        public bool IsWriteable { get; }
        public NfcDefRecord[] Records { get; }

        public NfcDefTag(Ndef tag, IEnumerable<NdefRecord> records)
        {
            IsWriteable = tag.IsWritable;
            Records = records
                .Select(r => new AndroidNdefRecord(r))
                .ToArray();
        }
    }

    public class AndroidNdefRecord : NfcDefRecord
    {
        public AndroidNdefRecord(NdefRecord nativeRecord)
        {
            TypeNameFormat = GetTypeNameFormat(nativeRecord.Tnf);
            Payload = nativeRecord.GetPayload();
        }

        private NDefTypeNameFormat GetTypeNameFormat(short nativeRecordTnf)
        {
            switch (nativeRecordTnf)
            {
                case NdefRecord.TnfAbsoluteUri:
                    return NDefTypeNameFormat.AbsoluteUri;
                case NdefRecord.TnfEmpty:
                    return NDefTypeNameFormat.Empty;
                case NdefRecord.TnfExternalType:
                    return NDefTypeNameFormat.External;
                case NdefRecord.TnfMimeMedia:
                    return NDefTypeNameFormat.Media;
                case NdefRecord.TnfUnchanged:
                    return NDefTypeNameFormat.Unchanged;
                case NdefRecord.TnfUnknown:
                    return NDefTypeNameFormat.Unchanged;
                case NdefRecord.TnfWellKnown:
                    return NDefTypeNameFormat.WellKnown;
            }

            return NDefTypeNameFormat.Unknown;
        }
    }
}
*/