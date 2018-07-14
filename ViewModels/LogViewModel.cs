using BlueEyes.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class LogViewModel
    {
        private ICommand _hide_window_command;
        private string log = "";

        public ICommand HideWindowCommand
        {
            get { return _hide_window_command; }
            set { _hide_window_command = value; }
        }

        public LogViewModel()
        {
            _hide_window_command = new DelegateCommand(HideWindow);
        }

        private void HideWindow(object obj)
        {

        }
    }
}
