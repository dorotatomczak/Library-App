
using System.ComponentModel;
using System.Windows.Input;

namespace Library
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        protected object selectedMenu;
        protected object selectedViewModel;
        public static NavigationViewModel Instance { get; private set; }

        public NavigationViewModel()
        {
            selectedViewModel = new LoginViewModel(this);
            selectedMenu = new StartMenuViewModel(this);

            LoginCommand = new BaseCommand(OpenLogin);
            RegisterCommand = new BaseCommand(OpenRegister);

            Instance = this;
        }

        private void OpenLogin(object obj)
        {
            SelectedViewModel = new LoginViewModel(this);
        }

        private void OpenRegister(object obj)
        {
            SelectedViewModel = new RegisterViewModel();
        }

        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        public object SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; OnPropertyChanged("SelectedMenu"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
