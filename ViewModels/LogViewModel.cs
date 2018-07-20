using BlueEyes.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace BlueEyes.ViewModels
{
    class LogViewModel : BindableBase
    {
        #region Fields
        private RelayCommand<CancelEventArgs> _closingCommand;
        private ObservableCollection<TextBlock> _document = new ObservableCollection<TextBlock>();
        private ICommand _hideCommand;
        private ICommand _showCommand;
        private bool _isVisible = false;
        #endregion

        #region Constructors
        public LogViewModel()
        {
            
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

        /*public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                if (_closingCommand == null)
                {
                    _closingCommand = new RelayCommand<CancelEventArgs>(Closing);
                }
                return _closingCommand;
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }*/
        #endregion

        #region Methods
        /*public void Closing(CancelEventArgs e)
        {
            // Hide instead of close
            //e.Cancel = true;
            //Hide(null);
            Application.Current.Shutdown();
        }

        public void Hide(object obj)
        {
            IsVisible = false;
        }

        public ICommand HideCommand
        {
            get
            {
                if (_hideCommand == null)
                {
                    _hideCommand = new RelayCommand(Hide, param => _isVisible);
                }
                return _hideCommand;
            }
            set { _hideCommand = value; }
        }

        public void Show(object obj)
        {
            IsVisible = true;
        }

        public ICommand ShowCommand
        {
            get
            {
                if (_showCommand == null)
                {
                    _showCommand = new RelayCommand(Show, param => !_isVisible);
                }
                return _showCommand;
            }
            set { _showCommand = value; }
        }*/

        public void WriteErrorLine(string s)
        {
            WriteLineColors(s, Brushes.LightSalmon);
        }

        public void WriteLine(string s)
        {
            WriteLineColors(s, Brushes.White);
        }

        public void WriteLine(string highlight, string normal)
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
