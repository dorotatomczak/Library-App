
using System.ComponentModel;
using System.Windows.Input;

namespace Library
{
    class NavigationViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        private object selectedViewModel;

        public static NavigationViewModel Instance { get; private set; }

        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        public NavigationViewModel()
        {   
            LoginCommand = new BaseCommand(OpenLogin);
            RegisterCommand = new BaseCommand(OpenRegister);
            selectedViewModel = new LoginViewModel();
            Instance = this;
        }

        private void OpenLogin(object obj)
        {
            SelectedViewModel = new LoginViewModel();
        }

        private void OpenRegister(object obj)
        {
            SelectedViewModel = new RegisterViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
