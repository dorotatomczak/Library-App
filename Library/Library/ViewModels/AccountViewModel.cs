
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Library
{
    class AccountViewModel : ViewModelBase
    {

        public ICommand LogOutCommand
        {
            get;
            private set;
        }

        private string _username;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _welcome;

        public string Welcome
        {
            get { return _welcome; }
            set
            {
                _welcome = value;
                OnPropertyChanged("Welcome");
            }
        }

        private ObservableCollection<Book> _books;

        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set
            {
                if (_books != value)
                {
                    _books = value;
                    OnPropertyChanged("Books");
                }
            }
        }

        public AccountViewModel()
        {
            UserName = User.Username;
            Welcome = $"Witaj, {UserName}!";
            LogOutCommand = new RelayCommand(LogOut);
            LoadBooks();
        }

        private void LogOut(object parameter)
        {
            StartWindow newWindow = new StartWindow();
            newWindow.Show();
            var myWindow = Window.GetWindow(parameter as AccountView);
            myWindow.Close();
        }

        public DataTable GetReservedBooks()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("BookID", typeof(int)));
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("Author", typeof(string)));
            dt.Columns.Add(new DataColumn("Genre", typeof(string)));
            dt.Columns.Add(new DataColumn("Pages", typeof(int)));

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                User user = new User();
                int[] BooksIDs = new int[3];
                user.getReservedBooks(BooksIDs);

                string query = "SELECT * FROM book_tbl WHERE BookID=@book1 OR BookID=@book2 OR BookID=@book3";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@book1", BooksIDs[0]);
                cmd.Parameters.AddWithValue("@book2", BooksIDs[1]);
                cmd.Parameters.AddWithValue("@book3", BooksIDs[2]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }

        public void LoadBooks()
        {
            DataTable BookTable = GetReservedBooks();

            Books = new ObservableCollection<Book>();

            foreach (DataRow row in BookTable.Rows)
            {
                var obj = new Book()
                {
                    BookID = (int)row["BookID"],
                    Title = (string)row["Title"],
                    Author = (string)row["Author"],
                    Pages = (int)row["Pages"],
                    Genre = (string)row["Genre"],
                };

                Books.Add(obj);
            }
        }
    }
}
