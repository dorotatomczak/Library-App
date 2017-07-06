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
    public class User
    {
        public static string Username
        {
            get;
            set;
        }

        public void getReservedBooks(int[] books)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                //check if user can reserve another book
                string query = "select * from user_tbl where Login=@Login";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Login", Username);

                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    books[0] = (int)myReader["ReservedBook1"];
                    books[1] = (int)myReader["ReservedBook2"];
                    books[2] = (int)myReader["ReservedBook3"];
                }

                myReader.Close();
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

        public bool ReserveBook(int bookID)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            bool success = false;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                //check if user can reserve another book
                string query = "select * from user_tbl where Login=@Login";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Login", Username);

                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    if ((int)myReader["ReservedBook1"] == 0)
                    {
                        query = "UPDATE user_tbl SET ReservedBook1 = @BookID WHERE Login=@Login";
                        break;
                    }
                    else if ((int)myReader["ReservedBook2"] == 0)
                    {
                        query = "UPDATE user_tbl SET ReservedBook2 = @BookID WHERE Login=@Login";
                        break;
                    }
                    else if ((int)myReader["ReservedBook3"] == 0)
                    {
                        query = "UPDATE user_tbl SET ReservedBook3 = @BookID WHERE Login=@Login";
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Nie możesz zarezerwować więcej niż 3 książki.");
                        return success;
                    }
                }

                myReader.Close();

                //success
                cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BookID", bookID);
                cmd.Parameters.AddWithValue("@Login", Username);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Rezerwacja przebiegła pomyślnie. Jeśli w ciągu 3 dni nie wypożyczysz książki, rezerwacja zostanie cofnięta.");
                success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return success;
        }
    }
}
