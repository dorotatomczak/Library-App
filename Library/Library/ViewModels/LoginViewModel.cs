
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Library
{
    class LoginViewModel : ViewModelBase
    {
        private string _PasswordInVM;
        private string _username;

        private NavigationViewModel _navigationViewModel;

        public LoginViewModel(NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
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

        /// <summary>
        /// Signing in to the account
        /// </summary>
        /// <param name="parameter"></param>
        private void Submit(object parameter)
        {
            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                PasswordInVM = ConvertToUnsecureString(secureString);
                PasswordInVM = sha256_hash(PasswordInVM);
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
                    User.Username = UserName;

                    query = "SELECT * FROM user_tbl WHERE Login=@Login AND Password=@Password";
                    cmd = new SqlCommand(query, connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Login", UserName);
                    cmd.Parameters.AddWithValue("@Password", PasswordInVM);

                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Type"] == "Normal")
                        {
                            User.Type = "Normal";
                            _navigationViewModel.SelectedViewModel = new AccountViewModel(_navigationViewModel);
                            _navigationViewModel.SelectedMenu = new UserMenuViewModel(_navigationViewModel);
                        }
                        else
                        {
                            User.Type = "Librarian";
                            _navigationViewModel.SelectedViewModel = new EditBooksViewModel(_navigationViewModel);
                            _navigationViewModel.SelectedMenu = new LibrarianMenuViewModel(_navigationViewModel);
                        }
                        User.UserID = (int)myReader["UserID"];
                    }

                    myReader.Close();
                }
                else
                {
                    MessageBox.Show("Login lub hasło są nieprawidłowe");
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

        public static String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }
    }
}
