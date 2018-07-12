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
    class MainWindowViewModel
    {
        // Commands
        private ICommand hiButtonCommand;
        private ICommand toggleExecuteCommand { get; set; }
        private ICommand exit_command;

        private bool canExecute = true;

        public string HiButtonContent
        {
            get { return "click to hi"; }
        }

        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value)
                {
                    return;
                }
                this.canExecute = value;
            }
        }

        public ICommand ToggleExecuteCommand
        {
            get { return toggleExecuteCommand; }
            set { toggleExecuteCommand = value; }
        }

        public ICommand HiButtonCommand
        {
            get { return hiButtonCommand; }
            set { hiButtonCommand = value; }
        }

        public ICommand ExitCommand
        {
            get { return exit_command; }
            set { exit_command = value; }
        }

        public MainWindowViewModel()
        {
            HiButtonCommand = new DelegateCommand(ShowMessage, param => this.canExecute);
            toggleExecuteCommand = new DelegateCommand(ChangeCanExecute);
            exit_command = new DelegateCommand(Exit);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        public void ShowMessage(object obj)
        {
            MessageBox.Show(obj.ToString());
        }

        public void ChangeCanExecute(object obj)
        {
            canExecute = !canExecute;
        }
    }
}
