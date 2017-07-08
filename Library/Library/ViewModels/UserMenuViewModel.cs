using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library
{
    class UserMenuViewModel : ViewModelBase
    {
        public ICommand ShowAccountCommand { get; set; }
        public ICommand ShowBooksCommand { get; set; }

        private NavigationViewModel _navigationViewModel;

        public UserMenuViewModel (NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
            ShowAccountCommand = new BaseCommand(OpenAccount);
            ShowBooksCommand = new BaseCommand(OpenBooks);
        }

        private void OpenAccount(object obj)
        {
            _navigationViewModel.SelectedViewModel = new AccountViewModel(_navigationViewModel);
        }

        private void OpenBooks(object obj)
        {
            _navigationViewModel.SelectedViewModel = new BooksViewModel();
        }
    }
}
