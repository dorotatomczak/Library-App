using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Library
{
    class BooksViewModel : ViewModelBase
    {
        public BooksViewModel()
        {
            LoadBooks();
            ReserveCommand = new RelayCommand(ReserveBook);
        }

        private ObservableCollection<Book> _books;
        private Book _selectedbook;

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

        public Book SelectedBook
        {
            get { return _selectedbook; }

            set
            {
                _selectedbook = value;
                OnPropertyChanged("SelectedBook");
            }
        }

        public ICommand ReserveCommand
        {
            get;
            private set;
        }

        private void ReserveBook(object parameter)
        {
            if (SelectedBook != null)
            {
                int bookID = SelectedBook.BookID;
                NormalUser user = new NormalUser();
                if (user.ReserveBook(bookID))
                {
                    SelectedBook.ReservedBy = User.Username;
                    SelectedBook.updateDatabase();
                    LoadBooks();
                }
            }
        }

        public DataTable GetBooks()
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

                string query = "SELECT * FROM book_tbl WHERE Reserved=@zero AND Borrowed=@zero";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@zero", "0");
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
            DataTable BookTable = GetBooks();

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
