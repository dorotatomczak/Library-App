
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
            this.DataContext = new NavigationViewModel();
            /*string connectionString = "SERVER=localhost; DATABASE=test_db; UID=root;PASSWORD=;";
            MySqlConnection connection = new MySqlConnection(connectionString);*/
        }
    }
}
