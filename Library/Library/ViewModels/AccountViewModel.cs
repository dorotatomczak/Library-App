
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
        #region Gets&Sets
        public ICommand LogOutCommand
        {
            get;
            private set;
        }

        public ICommand CancelReservationCommand
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
        private ObservableCollection<Book> _borowwedBooks;

        public ObservableCollection<Book> BorrowedBooks
        {
            get { return _borowwedBooks; }
            set
            {
                if (_borowwedBooks != value)
                {
                    _borowwedBooks = value;
                    OnPropertyChanged("BorrowedBooks");
                }
            }
        }

        private Book _selectedbook;

        public Book SelectedBook
        {
            get { return _selectedbook; }

            set
            {
                _selectedbook = value;
                OnPropertyChanged("SelectedBook");
            }
        }

        private NavigationViewModel _navigationViewModel;
        #endregion
        public AccountViewModel(NavigationViewModel nvm)
        {
            _navigationViewModel = nvm;
            UserName = User.Username;
            Welcome = $"Witaj, {UserName}!";
            LogOutCommand = new RelayCommand(LogOut);
            CancelReservationCommand = new RelayCommand(CancelReservation);
            LoadBooks();
        }

        private void LogOut(object parameter)
        {
            User.Username = string.Empty;
            User.Type = string.Empty;
            _navigationViewModel.SelectedViewModel = new LoginViewModel(_navigationViewModel);
            _navigationViewModel.SelectedMenu = new StartMenuViewModel(_navigationViewModel);
        }

        private void CancelReservation(object parameter)
        {
            if (SelectedBook != null)
            {
                int bookID = SelectedBook.BookID;
                NormalUser user = new NormalUser();
                user.CancelReservation(bookID);
                SelectedBook.ReservedBy = "0";
                SelectedBook.updateDatabase();
                LoadBooks();
            }
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

                NormalUser user = new NormalUser();
                int[] BooksIDs = new int[3];
                user.getReservedBooks(BooksIDs);
                #region Check if books are still reserved
                if (BooksIDs[0] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[0]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Reserved"] != User.Username)
                        {
                            BooksIDs[0] = 0;
                            query = "UPDATE user_tbl SET ReservedBook1 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Reserved"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (BooksIDs[1] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[1]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Reserved"] != User.Username)
                        {
                            BooksIDs[1] = 0;
                            query = "UPDATE user_tbl SET ReservedBook2 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Reserved"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (BooksIDs[2] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[2]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Reserved"] != User.Username)
                        {
                            BooksIDs[2] = 0;
                            query = "UPDATE user_tbl SET ReservedBook3 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Reserved"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }
                #endregion

                string query1;
                SqlCommand cmd1;
                query1 = "SELECT * FROM book_tbl WHERE BookID=@book1 OR BookID=@book2 OR BookID=@book3";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@book1", BooksIDs[0]);
                cmd1.Parameters.AddWithValue("@book2", BooksIDs[1]);
                cmd1.Parameters.AddWithValue("@book3", BooksIDs[2]);

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
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
        public DataTable GetBorrowedBooks()
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

                NormalUser user = new NormalUser();
                int[] BooksIDs = new int[5];
                user.getBorrowedBooks(BooksIDs);

                #region Check if books are still borrowed
                if (BooksIDs[0] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[0]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Borrowed"] != User.Username)
                        {
                            BooksIDs[0] = 0;
                            query = "UPDATE user_tbl SET BorrowedBook1 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Borrowed"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (BooksIDs[1] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[1]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Borrowed"] != User.Username)
                        {
                            BooksIDs[1] = 0;
                            query = "UPDATE user_tbl SET BorrowedBook2 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Borrowed"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (BooksIDs[2] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[2]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Borrowed"] != User.Username)
                        {
                            BooksIDs[2] = 0;
                            query = "UPDATE user_tbl SET BorrowedBook3 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Borrowed"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }


                if (BooksIDs[3] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[3]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Borrowed"] != User.Username)
                        {
                            BooksIDs[3] = 0;
                            query = "UPDATE user_tbl SET BorrowedBook4 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Borrowed"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (BooksIDs[4] != 0)
                {
                    bool changed = false;
                    string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@book", BooksIDs[4]);
                    SqlDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if ((string)myReader["Borrowed"] != User.Username)
                        {
                            BooksIDs[4] = 0;
                            query = "UPDATE user_tbl SET BorrowedBook5 = @zero WHERE Login=@Login";
                            changed = true;
                            break;
                        }
                        else if ((string)myReader["Borrowed"] == User.Username) break;
                    }
                    myReader.Close();
                    if (changed)
                    {
                        cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@zero", "0");
                        cmd.Parameters.AddWithValue("@Login", User.Username);
                        cmd.ExecuteNonQuery();
                    }
                }

                #endregion


                string query1 = "SELECT * FROM book_tbl WHERE BookID=@book1 OR BookID=@book2 OR BookID=@book3 OR BookID=@book4 OR BookID=@book5";

                SqlCommand cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@book1", BooksIDs[0]);
                cmd1.Parameters.AddWithValue("@book2", BooksIDs[1]);
                cmd1.Parameters.AddWithValue("@book3", BooksIDs[2]);
                cmd1.Parameters.AddWithValue("@book4", BooksIDs[3]);
                cmd1.Parameters.AddWithValue("@book5", BooksIDs[4]);

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
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
            DataTable BorrowedBookTable = GetBorrowedBooks();

            Books = new ObservableCollection<Book>();
            BorrowedBooks = new ObservableCollection<Book>();

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
            foreach (DataRow row1 in BorrowedBookTable.Rows)
            {
                var ob = new Book()
                {
                    BookID = (int)row1["BookID"],
                    Title = (string)row1["Title"],
                    Author = (string)row1["Author"],
                    Pages = (int)row1["Pages"],
                    Genre = (string)row1["Genre"],
                };

                BorrowedBooks.Add(ob);
            }
        }
    }
}
