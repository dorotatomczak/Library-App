
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace Library
{
    class LoginViewModel : ViewModelBase
    {
        private string _PasswordInVM;
        private string _username;

        public LoginViewModel()
        {
            SubmitCommand = new RelayCommand(Submit);
        }

        public ICommand SubmitCommand
        {
            get;
            private set;
        }

        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("UserName");
            }
        }

        public string PasswordInVM
        {
            get
            {
                return _PasswordInVM;
            }
            set
            {
                _PasswordInVM = value;
                OnPropertyChanged("PasswordInVM");
            }
        }

        private void Submit(object parameter)
        {
            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                PasswordInVM = ConvertToUnsecureString(secureString);
            }

            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                String query = "SELECT COUNT(1) FROM user_tbl WHERE Login=@Login AND Password=@Password";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Login", UserName);
                cmd.Parameters.AddWithValue("@Password", PasswordInVM);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count == 1)
                {
                    MessageBox.Show("You logged in");
                    /*MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    var myWindow = Window.GetWindow(this);
                    myWindow.Close();*/
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
