
using System.Windows.Input;

namespace Library
{
    class MainNavigationViewModel : BaseNavigationViewModel
    {
        public ICommand ShowAccountCommand { get; set; }
        public ICommand ShowBooksCommand { get; set; }

        public static MainNavigationViewModel Instance { get; private set; }

        public MainNavigationViewModel()
        {
            ShowAccountCommand = new BaseCommand(OpenAccount);
            ShowBooksCommand = new BaseCommand(OpenBooks);
            selectedViewModel = new AccountViewModel();

            Instance = this;
        }

        private void OpenAccount(object obj)
        {
            SelectedViewModel = new AccountViewModel();
        }

        private void OpenBooks(object obj)
        {
            SelectedViewModel = new BooksViewModel();
        }
    }
}
