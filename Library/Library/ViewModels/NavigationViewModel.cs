using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class NavigationViewModel : INotifyPropertyChanged
    {
        protected object selectedMenu;
        protected object selectedViewModel;
        public static NavigationViewModel Instance { get; private set; }

        public NavigationViewModel()
        {
            selectedViewModel = new LoginViewModel(this);
            selectedMenu = new StartMenuViewModel(this);

            Instance = this;
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
