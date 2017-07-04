
using System.Windows.Controls;

namespace Library
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
	public partial class RegisterView : UserControl, IHavePassword
    {
        public RegisterView()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }

        public System.Security.SecureString Password
        {
            get
            {
                return txtPassword.SecurePassword;
            }
        }
    }
}
