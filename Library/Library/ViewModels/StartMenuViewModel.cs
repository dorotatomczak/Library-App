using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library
{
    class StartMenuViewModel : ViewModelBase
    {
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        private NavigationViewModel _navigationViewModel;

        public StartMenuViewModel(NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
            LoginCommand = new BaseCommand(OpenLogin);
            RegisterCommand = new BaseCommand(OpenRegister);
        }

        private void OpenLogin(object obj)
        {
            _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel);
        }

        private void OpenRegister(object obj)
        {
            _navigationViewModel.SelectedViewModel = new RegisterViewModel();
        }
    }
}
