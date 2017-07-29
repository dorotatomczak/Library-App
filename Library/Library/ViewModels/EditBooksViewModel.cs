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
            BorrowCommand = new RelayCommand(BorrowBook);
            LoadBooks();
        }
        #region Gets&Sets
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

        public ICommand BorrowCommand
        {
            get;
            private set;
        }
        #endregion
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
                        query = "UPDATE book_tbl SET Title=@Title, Author=@Author, Genre=@Genre, Pages=@Pages, Borrowed=@Borrowed WHERE BookID=@BookID";
                    }
                    //add new
                    else
                    {
                        query = "insert into book_tbl (Title, Author, Pages, Genre, Reserved, Borrowed, AmountofBorrowings) values(@Title, @Author, @Pages, @Genre, '0', '0', 0)";
                    }

                    cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@BookID", book.BookID);
                    cmd.Parameters.AddWithValue("@Author", book.Author);
                    cmd.Parameters.AddWithValue("@Title", book.Title);
                    cmd.Parameters.AddWithValue("@Pages", book.Pages);
                    cmd.Parameters.AddWithValue("@Genre", book.Genre);
                    cmd.Parameters.AddWithValue("@Borrowed", book.Borrowed);
                    cmd.Parameters.AddWithValue("@AmountofBorrowings", book.AmountofBorrowings);
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
            LoadBooks();
        }

        private void DeleteBook(object parameter)
        {
            if (SelectedBook != null)
            {

                var InsertRecord = MessageBox.Show("Czy na pewno chcesz usunąć książkę?", "Potwierdź", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (InsertRecord == MessageBoxResult.Yes)
                {
                    if (SelectedBook.ReservedBy == "0")
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

        private void BorrowBook(object parameter)
        {
            if (SelectedBook != null)
            {
                {
                    if (SelectedBook.ReservedBy == "0")
                    {
                        MessageBox.Show("Czytelnik musi zarezerwować książkę");
                    }
                    else
                    {
                        SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
                        try
                        {
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            #region Check if books are still borrowed
                            NormalUser user = new NormalUser();
                            int[] BooksIDs = new int[5];
                            user.getBorrowedBooks(BooksIDs, SelectedBook.ReservedBy);



                            if (BooksIDs[0] != 0)
                            {
                                bool changed = false;
                                string query = "SELECT * FROM book_tbl WHERE BookID=@book";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@book", BooksIDs[0]);
                                SqlDataReader myReader = cmd.ExecuteReader();
                                while (myReader.Read())
                                {
                                    if ((string)myReader["Borrowed"] != SelectedBook.ReservedBy)
                                    {
                                        BooksIDs[0] = 0;
                                        query = "UPDATE user_tbl SET BorrowedBook1 = @zero WHERE Login=@Login";
                                        changed = true;
                                        break;
                                    }
                                    else if ((string)myReader["Borrowed"] == SelectedBook.ReservedBy) break;
                                }
                                myReader.Close();
                                if (changed)
                                {
                                    cmd = new SqlCommand(query, connection);
                                    cmd.Parameters.AddWithValue("@zero", "0");
                                    cmd.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
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
                                    if ((string)myReader["Borrowed"] != SelectedBook.ReservedBy)
                                    {
                                        BooksIDs[1] = 0;
                                        query = "UPDATE user_tbl SET BorrowedBook2 = @zero WHERE Login=@Login";
                                        changed = true;
                                        break;
                                    }
                                    else if ((string)myReader["Borrowed"] == SelectedBook.ReservedBy) break;
                                }
                                myReader.Close();
                                if (changed)
                                {
                                    cmd = new SqlCommand(query, connection);
                                    cmd.Parameters.AddWithValue("@zero", "0");
                                    cmd.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
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
                                    if ((string)myReader["Borrowed"] != SelectedBook.ReservedBy)
                                    {
                                        BooksIDs[2] = 0;
                                        query = "UPDATE user_tbl SET BorrowedBook3 = @zero WHERE Login=@Login";
                                        changed = true;
                                        break;
                                    }
                                    else if ((string)myReader["Borrowed"] == SelectedBook.ReservedBy) break;
                                }
                                myReader.Close();
                                if (changed)
                                {
                                    cmd = new SqlCommand(query, connection);
                                    cmd.Parameters.AddWithValue("@zero", "0");
                                    cmd.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
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
                                    if ((string)myReader["Borrowed"] != SelectedBook.ReservedBy)
                                    {
                                        BooksIDs[3] = 0;
                                        query = "UPDATE user_tbl SET BorrowedBook4 = @zero WHERE Login=@Login";
                                        changed = true;
                                        break;
                                    }
                                    else if ((string)myReader["Borrowed"] == SelectedBook.ReservedBy) break;
                                }
                                myReader.Close();
                                if (changed)
                                {
                                    cmd = new SqlCommand(query, connection);
                                    cmd.Parameters.AddWithValue("@zero", "0");
                                    cmd.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
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
                                    if ((string)myReader["Borrowed"] != SelectedBook.ReservedBy)
                                    {
                                        BooksIDs[4] = 0;
                                        query = "UPDATE user_tbl SET BorrowedBook5 = @zero WHERE Login=@Login";
                                        changed = true;
                                        break;
                                    }
                                    else if ((string)myReader["Borrowed"] == SelectedBook.ReservedBy) break;
                                }
                                myReader.Close();
                                if (changed)
                                {
                                    cmd = new SqlCommand(query, connection);
                                    cmd.Parameters.AddWithValue("@zero", "0");
                                    cmd.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            #endregion

                            #region find an empty slot for a new borrowed book
                            string query1 = "select * from user_tbl where Login=@Login";
                            SqlCommand cmd1 = new SqlCommand(query1, connection);
                            cmd1.CommandType = CommandType.Text;
                            cmd1.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);

                            SqlDataReader myReader11 = cmd1.ExecuteReader();
                            bool success = true;

                            while (myReader11.Read())
                            {
                                if ((int)myReader11["BorrowedBook1"] == 0)
                                {
                                    query1 = "UPDATE user_tbl SET BorrowedBook1 = @SelectedBookID WHERE Login=@Login";
                                    break;
                                }
                                else if ((int)myReader11["BorrowedBook2"] == 0)
                                {
                                    query1 = "UPDATE user_tbl SET BorrowedBook2 = @SelectedBookID WHERE Login=@Login";
                                    break;
                                }
                                else if ((int)myReader11["BorrowedBook3"] == 0)
                                {
                                    query1 = "UPDATE user_tbl SET BorrowedBook3 = @SelectedBookID WHERE Login=@Login";
                                    break;
                                }
                                else if ((int)myReader11["BorrowedBook4"] == 0)
                                {
                                    query1 = "UPDATE user_tbl SET BorrowedBook4 = @SelectedBookID WHERE Login=@Login";
                                    break;
                                }
                                else if ((int)myReader11["BorrowedBook5"] == 0)
                                {
                                    query1 = "UPDATE user_tbl SET BorrowedBook5 = @SelectedBookID WHERE Login=@Login";
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Nie można wypożyczyć więcej książek");
                                    success = false;
                                }
                            }
                            myReader11.Close();
                            #endregion

                            #region borrow the book
                            if (success)
                            {
                                int TotalBorrowedBooks = 0;
                                string bookIDs = string.Empty;

                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
                                cmd1.Parameters.AddWithValue("@SelectedBookID", SelectedBook.BookID);
                                cmd1.ExecuteNonQuery();

                                query1 = "select * from user_tbl where Login=@Login";
                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.CommandType = CommandType.Text;
                                cmd1.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);

                                SqlDataReader myReader2 = cmd1.ExecuteReader();

                                while (myReader2.Read())
                                {
                                    TotalBorrowedBooks = (int)myReader2["ReadBooksAmount"];
                                    TotalBorrowedBooks++;

                                    string[] prevIDs;
                                    if ((string)myReader2["BorrowedBooksIDs"] != "none")
                                    {
                                        prevIDs = ((string)myReader2["BorrowedBooksIDs"]).Split(',');
                                    }
                                    else
                                    {
                                        prevIDs = new string[2];
                                        prevIDs[1] = "0";
                                    }
                                    bookIDs = (string)myReader2["BorrowedBooksIDs"];
                                    bookIDs = string.Concat(prevIDs[1]);
                                    bookIDs = string.Concat(bookIDs,',');
                                    bookIDs = string.Concat(bookIDs,SelectedBook.BookID.ToString());

                                    if ((int)myReader2["ReservedBook1"] == SelectedBook.BookID)
                                    {
                                        query1 = "UPDATE user_tbl SET ReservedBook1 = 0, ReadBooksAmount=@ReadBooksAmount WHERE Login=@Login";
                                        break;
                                    }
                                    else if ((int)myReader2["ReservedBook2"] == SelectedBook.BookID)
                                    {
                                        query1 = "UPDATE user_tbl SET ReservedBook2 = 0, ReadBooksAmount=@ReadBooksAmount WHERE Login=@Login";
                                        break;
                                    }
                                    else if ((int)myReader2["ReservedBook3"] == SelectedBook.BookID)
                                    {
                                        query1 = "UPDATE user_tbl SET ReservedBook3 = 0, ReadBooksAmount=@ReadBooksAmount WHERE Login=@Login";
                                        break;
                                    }
                                    cmd1 = new SqlCommand(query1, connection);
                                    cmd1.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
                                    cmd1.Parameters.AddWithValue("@ReadBooksAmount", TotalBorrowedBooks);
                                    cmd1.ExecuteNonQuery();
                                }
                                myReader2.Close();

                                query1 = "UPDATE book_tbl SET Reserved=@zero, Borrowed=@Borrowed WHERE BookID=@BookID";

                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.Parameters.AddWithValue("@Borrowed", SelectedBook.ReservedBy);
                                cmd1.Parameters.AddWithValue("@BookID", SelectedBook.BookID);
                                cmd1.Parameters.AddWithValue("@zero", "0");
                                cmd1.ExecuteNonQuery();

                                query1 = "UPDATE user_tbl SET ReadBooksAmount=@ReadBooksAmount, BorrowedBooksIDs=@BorrowedBooksIDs WHERE Login=@Login";

                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.Parameters.AddWithValue("@Login", SelectedBook.ReservedBy);
                                cmd1.Parameters.AddWithValue("@ReadBooksAmount", TotalBorrowedBooks);
                                cmd1.Parameters.AddWithValue("@BorrowedBooksIDs", bookIDs);
                                cmd1.ExecuteNonQuery();



                                MessageBox.Show("Książka została wypożyczona");
                                #endregion

                                #region increase borrowing amount in book tbl
                                query1 = "select * from book_tbl where BookID=@BookID";
                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.CommandType = CommandType.Text;
                                cmd1.Parameters.AddWithValue("@BookID", SelectedBook.BookID);

                                int borrowingsAmount = -1;
                                myReader2 = cmd1.ExecuteReader();
                                while (myReader2.Read())
                                {
                                    borrowingsAmount = (int)myReader2["AmountofBorrowings"];
                                    break;
                                }
                                myReader2.Close();
                                borrowingsAmount += 1;
                                SelectedBook.AmountofBorrowings = borrowingsAmount;

                                query1 = "UPDATE book_tbl SET AmountofBorrowings=@AmountofBorrowings WHERE BookID=@BookID";

                                cmd1 = new SqlCommand(query1, connection);
                                cmd1.Parameters.AddWithValue("@BookID", SelectedBook.BookID);
                                cmd1.Parameters.AddWithValue("@AmountofBorrowings", SelectedBook.AmountofBorrowings);
                                cmd1.ExecuteNonQuery();

                                #endregion
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
                }
            }
            LoadBooks();
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
                    ReservedBy = (string)row["Reserved"],
                    Borrowed = (string)row["Borrowed"],
                    AmountofBorrowings = (int)row["AmountofBorrowings"]
                };

                Books.Add(obj);
            }
        }
    }
}
