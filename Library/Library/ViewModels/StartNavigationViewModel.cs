
using System.Windows.Input;

namespace Library
{
    class StartNavigationViewModel : BaseNavigationViewModel
    {
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }


        public static StartNavigationViewModel Instance { get; private set; }

        public StartNavigationViewModel()
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

    }
}
