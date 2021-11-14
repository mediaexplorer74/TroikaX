using Android.App;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.Widget;
using cc.troikadumper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;



namespace TroikaDumper.Droid.Services
{
    public class CardReader : Java.Lang.Object, NfcAdapter.IReaderCallback
    {
        protected TextView info;

        protected NfcAdapter nfcAdapter;
        protected Dump dump;
        protected bool writeMode = false;
        protected ProgressDialog pendingWriteDialog;

        // ISO-DEP command HEADER for selecting an AID.
        // Format: [Class | Instruction | Parameter 1 | Parameter 2]
        private static readonly byte[] SELECT_APDU_HEADER = new byte[] { 0x00, 0xA4, 0x04, 0x00 };

        // AID for our loyalty card service.
        private static readonly string SAMPLE_LOYALTY_CARD_AID = "F123456789";

        // "OK" status word sent in response to SELECT AID command (0x9000)
        private static readonly byte[] SELECT_OK_SW = new byte[] { 0x90, 0x00 };

        public async void OnTagDiscovered(Tag tag)
        {
            //IsoDep isoDep = IsoDep.Get(tag);

            // check what is this: Tag or Spec. card...
            if (1==0) //(isoDep != null)
            {
                // Tag 
                /*
                try
                {
                    isoDep.Connect();

                    var aidLength = (byte)(SAMPLE_LOYALTY_CARD_AID.Length / 2);
                    var aidBytes = StringToByteArray(SAMPLE_LOYALTY_CARD_AID);
                    var command = SELECT_APDU_HEADER
                        .Concat(new byte[] { aidLength })
                        .Concat(aidBytes)
                        .ToArray();

                    var result = isoDep.Transceive(command);
                    var resultLength = result.Length;
                    byte[] statusWord = { result[resultLength - 2], result[resultLength - 1] };
                    var payload = new byte[resultLength - 2];
                    Array.Copy(result, payload, resultLength - 2);
                    var arrayEquals = SELECT_OK_SW.Length == statusWord.Length;

                    if (Enumerable.SequenceEqual(SELECT_OK_SW, statusWord))
                    {
                        var msg = Encoding.UTF8.GetString(payload);
                        await App.DisplayAlertAsync(msg);
                    }
                }
                catch (Exception e)
                {
                    await App.DisplayAlertAsync("Error communicating with card: " + e.Message);
                }
                */
            }
            else
            {
                // Not tag

                //await App.DisplayAlertAsync("Some magic card detected.");

                dump = Dump.fromTag(tag);

                string teststr = "";

                                
                //info.Append("\nCard UID: " + dump.UidAsString);
                teststr += "\nCard UID: " + dump.UidAsString;

                
                /*
                String[] blocks = dump.DataAsStrings;

                for (int i = 0; i < blocks.Length; i++)
                {
                    info.Append("\n" + i + "] " + blocks[i]);
                }
                */

                //info.Append("\n\n  --- Extracted data: ---\n");
                teststr += "\n\n  --- Extracted data: ---\n";

                //info.Append("\nCard number:      " + dump.CardNumberAsString);
                teststr += "\nCard number:      " + dump.CardNumberAsString;

                //info.Append("\nCurrent balance:  " + dump.BalanceAsString);
                teststr += "\nCurrent balance:  " + dump.BalanceAsString;

                //info.Append("\nLast usage date:  " + dump.LastUsageDateAsString);
                teststr += "\nLast usage date:  " + dump.LastUsageDateAsString;

                //info.Append("\nLast validator:   " + dump.LastValidatorIdAsString);
                teststr += "\nLast validator:   " + dump.LastValidatorIdAsString;


                Debug.WriteLine(teststr);                
                //Debug.WriteLine(info.Text);

                await App.DisplayAlertAsync("Some magic card detected. Data: " + teststr);
            }
        }

        public static byte[] StringToByteArray(string hex) => 
            Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
    }
}