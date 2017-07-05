
using System.Windows;
using System.Windows.Input;

namespace Library
{
    class AccountViewModel : ViewModelBase
    {

        public ICommand LogOutCommand
        {
            get;
            private set;
        }

        private string _username;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _welcome;

        public string Welcome
        {
            get { return _welcome; }
            set
            {
                _welcome = value;
                OnPropertyChanged("Welcome");
            }
        }

        public AccountViewModel()
        {
            UserName = User.Username;
            Welcome = $"Witaj, {UserName}!";
            LogOutCommand = new RelayCommand(LogOut);
        }

        private void LogOut(object parameter)
        {
            StartWindow newWindow = new StartWindow();
            newWindow.Show();
            var myWindow = Window.GetWindow(parameter as AccountView);
            myWindow.Close();
        }
    }
}
