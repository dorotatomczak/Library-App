using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Library
{
    public class Book
    {
        private int _bookid;
        private string _title;
        private string _author;
        private int _pages;
        private string _genre;
        private int _borrowed;
        private int _reserved;

        public Book()
        {
            Title = "nn";
            Author = "nn";
            Genre = "nn";
            Pages = 0;
            Reserved = 0;
            Borrowed = 0;
        }

        #region Gets & Sets

        public int BookID
        {
            get { return _bookid; }
            set { _bookid = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public int Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        public int Borrowed
        {
            get { return _borrowed; }
            set { _borrowed = value; }
        }

        public int Reserved
        {
            get { return _reserved; }
            set { _reserved = value; }
        }

        #endregion

        public void updateDatabase()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                //check if user can reserve another book
                string query = query = "UPDATE book_tbl SET Title = @Title, Author = @Author, Pages = @Pages," + 
                    " Genre = @Genre, Reserved = @Reserved, Borrowed = @Borrowed WHERE BookID=@BookID";
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@BookID", this.BookID);
                cmd.Parameters.AddWithValue("@Title", this.Title);
                cmd.Parameters.AddWithValue("@Author", this.Author);
                cmd.Parameters.AddWithValue("@Pages", this.Pages);
                cmd.Parameters.AddWithValue("@Genre", this.Genre);
                cmd.Parameters.AddWithValue("@Reserved", this.Reserved);
                cmd.Parameters.AddWithValue("@Borrowed", this.Borrowed);
                cmd.ExecuteNonQuery();
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
