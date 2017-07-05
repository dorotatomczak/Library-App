
using System.Windows;

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainNavigationViewModel();
            viewModel.RequestClose += this.Close;
            this.DataContext = viewModel;
        }
    }
}
