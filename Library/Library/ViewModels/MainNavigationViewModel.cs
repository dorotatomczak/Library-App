
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Library
{
    class MainNavigationViewModel : BaseNavigationViewModel
    {
        public ICommand ShowAccountCommand { get; set; }

        public static MainNavigationViewModel Instance { get; private set; }

        public MainNavigationViewModel()
        {
            ShowAccountCommand = new BaseCommand(OpenAccount);

            selectedViewModel = new AccountViewModel();

            Instance = this;
        }

        private void OpenAccount(object obj)
        {
            selectedViewModel = new AccountViewModel();
        }
    }
}
