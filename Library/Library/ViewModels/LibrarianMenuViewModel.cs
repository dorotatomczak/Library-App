using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library
{
    class LibrarianMenuViewModel : ViewModelBase
    {
        public ICommand LogOutCommand { get; set; }
        private NavigationViewModel _navigationViewModel;

        public LibrarianMenuViewModel(NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
            LogOutCommand = new BaseCommand(LogOut);
        }

        private void LogOut(object obj)
        {
            User.Username = string.Empty;
            User.Type = string.Empty;
            _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel);
            _navigationViewModel.SelectedMenu = new StartMenuViewModel(_navigationViewModel);
        }
    }
}
