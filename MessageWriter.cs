using BlueEyes.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace BlueEyes
{
    public static class MessageWriter
    {
        public static void LogWrite(string s)
        {
            Messenger.Default.Send<GenericMessage<string>, LogViewModel>(new GenericMessage<string>(s));
        }
    }
}
