using BlueEyes.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace BlueEyes
{
    public static class MessageWriter
    {
        public static void BLEWrite(byte[] cmd)
        {
            Messenger.Default.Send<GenericMessage<byte[]>, MainWindowViewModel>(new GenericMessage<byte[]>(cmd));
        }

        public static void DebugWrite(string s)
        {
            Console.WriteLine(s);
            //Messenger.Default.Send<GenericMessage<string[]>, LogViewModel>(new GenericMessage<string[]>(new string[] { s, null, null }));
        }

        public static void LogWrite(string s)
        {
            Console.WriteLine(s);
            //Messenger.Default.Send<GenericMessage<string>, LogViewModel>(new GenericMessage<string>(s));
        }

        public static void LogWrite(string highlight, string normal)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(highlight);
            Console.ResetColor();
            Console.WriteLine(normal);
            //Messenger.Default.Send<GenericMessage<string[]>, LogViewModel>(new GenericMessage<string[]>(new string[] { highlight, normal }));
        }
    }
}
