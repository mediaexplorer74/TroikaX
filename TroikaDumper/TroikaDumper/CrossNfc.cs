using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Nfc.Abstractions;
using Xamarin.Forms;

namespace Plugin.Nfc
{
    public partial class CrossNfc
    {
        //private static Lazy<INfc> _implementation = new Lazy<INfc>(CreateNfc, LazyThreadSafetyMode.PublicationOnly);
        //public static INfc Current => _implementation.Value;

        /*
        private static INfc CreateNfc()
        {
            //#if PORTABLE
            //            throw NotImplementedInReferenceAssembly();
            //#else
            return null;//new NfcImplementation();
//#endif
        }
        */

        //INfc myCrossNfc = DependencyService.Get<INfc>();

        //public static global::Android.Content.Context? CurrentActivity { get; set; }
    
    }//CrossNfc class end


    // TEMP
    /*
    internal class NfcImplementation : INfc
    {
        public event TagDetectedDelegate TagDetected;

        public Task<bool> IsAvailableAsync()
        {
            //throw new NotImplementedException();
            return null;
        }

        public Task<bool> IsEnabledAsync()
        {
            //throw new NotImplementedException();
            return null;
        }

        public Task StartListeningAsync()
        {
            //throw new NotImplementedException();
            return null;
        }

        public Task StopListeningAsync()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
    */
}