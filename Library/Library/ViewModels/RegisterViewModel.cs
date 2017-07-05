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
    class RegisterViewModel : ViewModelBase
    {
        private string _PasswordInVM;
        private string _username;
        private string _email;

        public RegisterViewModel()
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

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
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
        /// Submiting a new account
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

                string query = "insert into user_tbl (Login, Email, Password) values('"+this.UserName+"', '"+this.Email+"', '"+this.PasswordInVM+"')";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Saved");
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
