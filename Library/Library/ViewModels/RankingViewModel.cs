using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Library
{
    public class RankingViewModel:ViewModelBase
    {
        #region Gets&Sets
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
        private ObservableCollection<Reader> _readers;

        public ObservableCollection<Reader> Readers
        {
            get { return _readers; }
            set
            {
                if (_readers != value)
                {
                    _readers = value;
                    OnPropertyChanged("Readers");
                }
            }
        }
        #endregion
        public RankingViewModel()
        {
            LoadReaders();
            LoadBooks();
        }

        public void LoadBooks()
        {
            DataTable BookTable = GetBooks();

            Books = new ObservableCollection<Book>();

            foreach (DataRow row in BookTable.Rows)
            {
                var obj = new Book()
                {
                    Title = (string)row["Title"],
                    AmountofBorrowings = (int)row["AmountofBorrowings"],
                };

                Books.Add(obj);
            }
        }
        public DataTable GetBooks()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Title", typeof(string)));
            dt.Columns.Add(new DataColumn("AmountofBorrowings", typeof(int)));

            int max1 = 0;
            int max2 = 0;
            int max3 = 0;
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = "SELECT * FROM book_tbl";
                SqlCommand cmd = new SqlCommand(query, connection);
                
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if ((int)myReader["AmountofBorrowings"] > max1)
                    {
                        id3 = id2;
                        max3 = max2;
                        id2 = id1;
                        max2 = max1;
                        max1 = (int)myReader["AmountofBorrowings"];
                        id1 = (int)myReader["BookID"];
                    }
                    else if ((int)myReader["AmountofBorrowings"] > max2)
                    {
                        id3 = id2;
                        max3 = max2;
                        max2 = (int)myReader["AmountofBorrowings"];
                        id2 = (int)myReader["BookID"];
                    }
                    else if ((int)myReader["AmountofBorrowings"] > max3)
                    {
                        max3 = (int)myReader["AmountofBorrowings"];
                        id3 = (int)myReader["BookID"];
                    }
                }
                myReader.Close();

                string query1;
                SqlCommand cmd1;
                SqlDataAdapter da;
                query1 = "SELECT * FROM book_tbl WHERE BookID=@book1";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@book1", id1);

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt);
                da.Dispose();

                query1 = "SELECT * FROM book_tbl WHERE BookID=@book1";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@book1", id2);

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt);
                da.Dispose();

                query1 = "SELECT * FROM book_tbl WHERE BookID=@book1";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@book1", id3);

                da = new SqlDataAdapter(cmd1);
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

        public void LoadReaders()
        {
            DataTable ReadersTable = GetReaders();

            Readers = new ObservableCollection<Reader>();

            foreach (DataRow row in ReadersTable.Rows)
            {
                var obj = new Reader()
                {
                    Login = (string)row["Login"],
                    ReadBooksAmount = (int)row["ReadBooksAmount"],
                };

                Readers.Add(obj);
            }
        }
        public DataTable GetReaders()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Login", typeof(string)));
            dt.Columns.Add(new DataColumn("ReadBooksAmount", typeof(int)));

            int max1 = 0;
            int max2 = 0;
            int max3 = 0;
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = "SELECT * FROM user_tbl";
                SqlCommand cmd = new SqlCommand(query, connection);

                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if ((int)myReader["UserID"]>10)
                    {
                        if ((int)myReader["ReadBooksAmount"] > max1)
                        {
                            id3 = id2;
                            max3 = max2;
                            id2 = id1;
                            max2 = max1;
                            max1 = (int)myReader["ReadBooksAmount"];
                            id1 = (int)myReader["UserID"];
                        }
                        else if ((int)myReader["ReadBooksAmount"] > max2)
                        {
                            id3 = id2;
                            max3 = max2;
                            max2 = (int)myReader["ReadBooksAmount"];
                            id2 = (int)myReader["UserID"];
                        }
                        else if ((int)myReader["ReadBooksAmount"] > max3)
                        {
                            max3 = (int)myReader["ReadBooksAmount"];
                            id3 = (int)myReader["UserID"];
                        }
                    }
                }
                myReader.Close();

                string query1;
                SqlCommand cmd1;
                SqlDataAdapter da;
                query1 = "SELECT * FROM user_tbl WHERE UserID=@user";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@user", id1);

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt);
                da.Dispose();

                query1 = "SELECT * FROM user_tbl WHERE UserID=@user";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@user", id2);

                da = new SqlDataAdapter(cmd1);
                da.Fill(dt);
                da.Dispose();

                query1 = "SELECT * FROM user_tbl WHERE UserID=@user";
                cmd1 = new SqlCommand(query1, connection);
                cmd1.Parameters.AddWithValue("@user", id3);

                da = new SqlDataAdapter(cmd1);
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
    }
}
