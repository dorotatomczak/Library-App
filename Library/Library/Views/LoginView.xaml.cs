
using System.Windows.Controls;


namespace Library
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
	public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
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
