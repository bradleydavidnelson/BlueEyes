using GalaSoft.MvvmLight.Messaging;
using BlueEyes.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BlueEyes.ViewModels
{
    class LogViewModel : BindableBase
    {
        #region Fields
        private ObservableCollection<TextBlock> _document = new ObservableCollection<TextBlock>();
        #endregion

        #region Constructors
        public LogViewModel()
        {
            Messenger.Default.Register<GenericMessage<string>>(this,(action) => ReceiveMessage(action));
        }
        #endregion

        #region Properties
        public ObservableCollection<TextBlock> Document
        {
            get { return _document; }
            set
            {
                _document = value;
                NotifyPropertyChanged();
                
            }
        }
        #endregion

        #region Methods
        private void ReceiveMessage(GenericMessage<string> message)
        {
            WriteLine(message.Content);
        }

        private void WriteErrorLine(string s)
        {
            WriteLineColors(s, Brushes.LightSalmon);
        }

        private void WriteDebugLine(string s)
        {
            WriteLineColors(s, Brushes.LightGreen);
        }

        private void WriteLine(string s)
        {
            WriteLineColors(s, Brushes.White);
        }

        private void WriteLine(string highlight, string normal)
        {
            WriteLineColors(highlight, Brushes.LightGoldenrodYellow, normal, Brushes.White);
        }

        private void WriteLineColors(string s, SolidColorBrush color)
        {
            WriteLineColors(s, color, "", color);
        }

        private void WriteLineColors(string string1, SolidColorBrush color1, string string2, SolidColorBrush color2)
        {
            Console.WriteLine(string1 + string2);
            TextBlock block = new TextBlock();
            block.TextWrapping = TextWrapping.Wrap;

            Run run1 = new Run(string1);
            run1.Foreground = color1;
            block.Inlines.Add(run1);

            Run run2 = new Run(string2);
            run2.Foreground = color2;
            block.Inlines.Add(run2);

            Document.Add(block);
            
        }
        #endregion
    }
}
