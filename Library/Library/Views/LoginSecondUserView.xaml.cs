using System.Windows.Controls;

namespace Library
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
	public partial class LoginSecondUserView : UserControl, IHavePassword
    {
        public LoginSecondUserView()
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

