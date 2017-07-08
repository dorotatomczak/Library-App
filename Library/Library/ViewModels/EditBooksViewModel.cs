using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Library
{
    class EditBooksViewModel : ViewModelBase
    {
        private NavigationViewModel _navigationViewModel;
        private Book _selectedbook;
        private ObservableCollection<Book> _books;
        private SqlDataAdapter da;
        private DataTable BookTable;


        public EditBooksViewModel(NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
            DeleteCommand = new RelayCommand(DeleteBook);
            SaveChangesCommand = new RelayCommand(SaveChanges);
            LoadBooks();
        }

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

        public ICommand DeleteCommand
        {
            get;
            private set;
        }

        public ICommand SaveChangesCommand
        {
            get;
            private set;
        }

        private void SaveChanges(object sender)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query;
                SqlCommand cmd;

                foreach (Book book in Books)
                {
                    query = "SELECT COUNT(1) FROM book_tbl WHERE BookID=@BookID";
                    cmd = new SqlCommand(query, connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@BookID", book.BookID);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    //update
                    if (count == 1)
                    {
                        query = "UPDATE book_tbl SET Title=@Title, Author=@Author, Genre=@Genre, Pages=@Pages WHERE BookID=@BookID";
                    }
                    //add new
                    else
                    {
                        query = "insert into book_tbl (Title, Author, Pages, Genre, Reserve, Borrowed) values(@Title, @Author, @Pages, @Genre, 0, 0)";
                    }

                    cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@BookID", book.BookID);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Pages", book.Pages);
                    cmd.Parameters.AddWithValue("@Genre", book.Genre);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Wprowadzone zmiany zostały zapisane");
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

        private void DeleteBook(object parameter)
        {
            if (SelectedBook != null)
            {

            var InsertRecord = MessageBox.Show("Czy na pewno chcesz usunąć książkę?", "Potwierdź", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (InsertRecord == MessageBoxResult.Yes)
                {
                    if (SelectedBook.Reserved != 1)
                    {
                        SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
                        try
                        {
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();

                            string query = "DELETE FROM book_tbl WHERE BookID=@BookID";

                            SqlCommand cmd = new SqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@BookID", SelectedBook.BookID);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Książka została usunięta");
                            LoadBooks();
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
                    else
                    {
                        MessageBox.Show("Nie można usunąc zarezerwowanej książki");
                    }
                }
            }
        }

        public DataTable GetBooks()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            DataTable dt = new DataTable();

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = "SELECT * FROM book_tbl";
                SqlCommand cmd = new SqlCommand(query, connection);

                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
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
            BookTable = GetBooks();

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
