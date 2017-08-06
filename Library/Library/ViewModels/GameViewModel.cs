using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;

namespace Library
{
    public class GameViewModel : ViewModelBase
    {
        private string userName;
        private static int fluentMoveRate = 5;
        private static int cols = 10;
        private static int rows = 20 * fluentMoveRate;
        private static int difficultyRate = rows * 10;
        private static int timeSpan = 500;
        private int score;
        private int rekord;
        private int superSpeed = 100;
        private double difficultyLevel;
        private int difficultyCounter;
        private bool gameover;
        private int level { get; set; }
        public DispatcherTimer dispatcherTimer { get; set; }
        private Block currBlock;
        private ObservableCollection<ObservableCollection<Brush>> board;
        private Brush noBrush;
        private NavigationViewModel nvm;
        private string timer;
        #region Gets&Sets
        public ICommand startGameButtonCommand { get; set; }
        public string Timer
        {
            get { return timer; }
            set
            {
                if (timer != value)
                    timer = value;

                OnPropertyChanged("Timer");
            }
        }
        public ObservableCollection<ObservableCollection<Brush>> Board
        {
            get { return board; }
            set
            {
                if (board != value)
                {
                    board = value;
                    OnPropertyChanged("Board");
                }
            }
        }
        public int Score
        {
            get { return score; }
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged("Score");
                }
            }
        }
        public int Rekord
        {
            get { return rekord; }
            set
            {
                if (rekord != value)
                {
                    rekord = value;
                    OnPropertyChanged("Rekord");
                }
            }
        }
        public ICommand MoveLeftCommand
        {
            get;
            private set;
        }
        public ICommand MoveRightCommand
        {
            get;
            private set;
        }
        public ICommand RotateCommand
        {
            get;
            private set;
        }
        public ICommand MoveFasterCommand
        {
            get;
            private set;
        }
        public ICommand BeginMultiplayerButtonCommand
        {
            get;
            private set;
        }
        public int Level
        {
            get { return level; }
            set
            {
                this.level = value;
                OnPropertyChanged("Level");
            }
        }
        public int getScore()
        {
            return score;
        }
        #endregion

        public GameViewModel(NavigationViewModel nvm)
        {
            this.nvm = nvm;
            userName = User.Username;
            prepareButtonsAndCommands();
            board = new ObservableCollection<ObservableCollection<Brush>>();
            timer = "0";
            score = 0;
            currBlock = new Block();
            noBrush = currBlock.getNoBrush();
            this.rekord = 0;
            for (int i = 0; i < rows; i++)
            {
                board.Add(new ObservableCollection<Brush>());
                for (int j = 0; j < cols; j++)
                {
                    board[i].Add(noBrush);
                }
            }
        }
        private void currBlockDraw()
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            Brush color = currBlock.getColor();
            bool canBeDrawn = true;

            //check if can be drawn
            for (int i = 0; i < currBlock.getHeight(); i++)
            {
                for (int j = 0; j < currBlock.getWidth(); j++)
                {
                    if (shape[j, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if ((int)(position.Y) + i * fluentMoveRate + a >= rows || (int)(position.X) + j >= cols
                                || (int)(position.Y) + i * fluentMoveRate + a < 0 || (int)(position.X) + j < 0) continue;
                            if (board[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                            {
                                canBeDrawn = false;
                            }
                        }
                    }
                }
            }

            if (canBeDrawn)
            {
                for (int i = 0; i < currBlock.getWidth(); i++)
                {
                    for (int j = 0; j < currBlock.getHeight(); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            for (int k = 0; k < fluentMoveRate; k++)
                            {
                                if ((int)(j * fluentMoveRate + position.Y + k) >= rows || (int)(i + position.X) >= cols
                                || (int)(j * fluentMoveRate + position.Y + k) < 0 || (int)(i + position.X) < 0) continue;
                                board[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = color;
                            }
                        }
                    }
                }
            }
            else
            {
                currBlock.moveUp();
                currBlockDraw();
            }
        }
        private void currBlockErase()
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        for (int k = 0; k < fluentMoveRate; k++)
                        {

                            board[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = noBrush;
                        }
                    }
                }
            }
        }
        private void checkRows()
        {
            bool full;
            for (int i = rows - 1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < cols; j++)
                {
                    if (board[i][j] == noBrush)
                    {
                        full = false;
                    }
                }
                if (full)
                {
                    for (int a = 0; a < fluentMoveRate; a++)
                    {
                        removeRows(i);
                    }
                    i++;
                    Score += 100;
                }
            }
        }
        private void removeRows(int row)
        {
            for (int i = row; i > 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i][j] = board[i - 1][j];
                }
            }
        }
        #region Block moves
        public void moveLeft(object param)
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            bool move = true;
            currBlockErase();
            if (position.X <= 0)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock.getHeight(); i++)
                {

                    if (shape[0, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) - 1] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }

                }
            }

            if (move)
            {
                currBlock.moveLeft();
            }
            currBlockDraw();
        }
        public void moveRight(object param)
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            currBlockErase();
            bool move = true;
            if (position.X + currBlock.getWidth() >= cols)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock.getHeight(); i++)
                {

                    if (shape[currBlock.getHeight() - 1, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + currBlock.getWidth()] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }
                }
            }
            if (move)
            {
                currBlock.moveRight();
            }
            currBlockDraw();
        }
        public void moveFaster(object param)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / superSpeed / difficultyLevel));
        }
        public void moveSlower()
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / difficultyLevel));
        }
        public void moveDown()
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            bool move = true;
            if (position.Y + currBlock.getHeight() * fluentMoveRate >= rows)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock.getWidth(); j++)
                    {
                        if (shape[j, i] == 1 && (i == currBlock.getHeight() - 1 || shape[j, i + 1] == 0))
                        {
                            if (board[(int)(position.Y) + i * fluentMoveRate + fluentMoveRate][(int)(position.X) + j] != noBrush)
                            {
                                move = false;
                            }

                        }
                    }
                }
            }
            if (move)
            {
                currBlockErase();
                currBlock.moveDown();
                currBlockDraw();
            }
            else
            {
                if (position.Y == 0)
                {
                    gameover = true;
                    return;
                }
                checkRows();
                currBlock = new Block();
            }
        }
        public void rotate(object param)
        {
            Point position = currBlock.getPosition();
            int[,] shape = currBlock.getMatrix();
            currBlockErase();
            currBlock.turnRight();
            bool success = true;
            if (position.X + currBlock.getWidth() > cols) success = false;
            else
                for (int i = 0; i < currBlock.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock.getWidth(); j++)
                    {
                        if (shape[j, i] == 1)
                        {
                            for (int a = 0; a < fluentMoveRate; a++)
                            {
                                if (board[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                                {
                                    success = false;
                                }
                            }
                        }
                    }
                }
            if (!success)
            {
                currBlock.turnLeft();
            }
            currBlockDraw();
        }
        #endregion
        private void goToMultiplayer(object param)
        {
            this.nvm.SelectedViewModel = new LoginSecondUserViewModel(this.nvm);
        }
        private void DispatcherTimerSetup()
        {
            this.TimerStart = DateTime.Now;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, timeSpan / fluentMoveRate);
            dispatcherTimer.Tick += new EventHandler(TimeTick);
            dispatcherTimer.Start();
        }

        private DateTime TimerStart { get; set; }

        public void TimeTick(object sender, EventArgs e)
        {
            if (gameover)
            {
                ((DispatcherTimer)sender).Stop();
                dispatcherTimer.Tick -= TimeTick;
                MessageBox.Show("Koniec gry");
                gameover = false;
                return;
            }
            var currentValue = DateTime.Now - this.TimerStart;
            if (difficultyCounter % difficultyRate == 0)
            {
                difficultyLevel *= 1.1;
                difficultyCounter = 1;
                Level++;

                #region get book proposition
                SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    string query = "select * from user_tbl where Login=@Login";
                    var cmd = new SqlCommand(query, connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Login", User.Username);

                    SqlDataReader myReader = cmd.ExecuteReader();
                    string[] prevIDs = new string[2];
                    string title="", genre="";

                    while (myReader.Read())
                    {
                        if ((string)myReader["BorrowedBooksIDs"] != "none")
                        {
                            prevIDs = ((string)myReader["BorrowedBooksIDs"]).Split(',');
                        }
                        else
                        {
                            prevIDs = new string[2];
                            prevIDs[0] = "0";
                            prevIDs[1] = "0";
                        }
                    }
                    myReader.Close();

                    if (prevIDs[0] == "0" && prevIDs[1] == "0")
                    {
                        query = "select * from book_tbl";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;

                        myReader = cmd.ExecuteReader();
                        while (myReader.Read())
                        {
                            title = (string)myReader["Title"];
                            genre = (string)myReader["Genre"];
                            break;
                        }
                        myReader.Close();
                    }
                    else if (prevIDs[0] == "0")
                    {
                        query = "select * from book_tbl where BookID=@bookID";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@bookID", Int32.Parse(prevIDs[1]));

                        myReader = cmd.ExecuteReader();
                        while (myReader.Read())
                        {
                            title = (string)myReader["Title"];
                            genre = (string)myReader["Genre"];
                            break;
                        }
                        myReader.Close();

                        query = "select * from book_tbl where Genre=@genre";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@genre", genre);

                        myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            if ((int)myReader["BookID"] == Int32.Parse(prevIDs[1])) continue;
                            title = (string)myReader["Title"];
                            break;
                        }
                        myReader.Close();

                    }

                    else
                    {
                        string  genre1="";
                        string  genre2="";

                        query = "select * from book_tbl where BookID=@bookID";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@bookID", Int32.Parse(prevIDs[0]));

                        myReader = cmd.ExecuteReader();
                        while (myReader.Read())
                        {
                            genre1 = (string)myReader["Genre"];
                            break;
                        }
                        myReader.Close();

                        query = "select * from book_tbl where BookID=@bookID";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@bookID", Int32.Parse(prevIDs[1]));

                        myReader = cmd.ExecuteReader();
                        while (myReader.Read())
                        {
                            genre2 = (string)myReader["Genre"];
                            break;
                        }
                        myReader.Close();

                        if(genre1 == genre2)
                        {
                            genre = genre1;
                        }
                        else
                        {
                            Random rand = new Random();
                            if ((rand.Next()) % 2 ==0)
                            {
                                genre = genre1;
                            }
                            else
                            {
                                genre = genre2;
                            }
                        }
                        query = "select * from book_tbl where Genre=@genre";
                        cmd = new SqlCommand(query, connection);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@genre", genre);

                        myReader = cmd.ExecuteReader();

                        while (myReader.Read())
                        {
                            if ((int)myReader["BookID"] == Int32.Parse(prevIDs[0]) || (int)myReader["BookID"] == Int32.Parse(prevIDs[1])) continue;
                            title = (string)myReader["Title"];
                            break;
                        }
                        myReader.Close();
                    }

                    query = "UPDATE user_tbl SET MostPopularGenre = @genre WHERE Login=@Login";
                    cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.Parameters.AddWithValue("@Login", User.Username);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(string.Format("Następny poziom!\nCzy czytałeś już książkę o tytule: {0}?", title));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                #endregion
            }
            difficultyCounter++;
            timer = currentValue.TotalSeconds.ToString("000");
            Timer = timer;
            Board = board;
            moveDown();
            moveSlower();
            updateRekord();
        }

        private void startGameClick(object sender)
        {
            prepareNewGame();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i][j] = noBrush;
                }
            }
            currBlock = new Block();
            currBlockDraw();
        }

        private void prepareNewGame()
        {
            DispatcherTimerSetup();
            this.Level = 1;
            this.score = 0;
            gameover = false;
            difficultyLevel = 1;
            difficultyCounter = 1;

            NormalUser user = new NormalUser();
            Rekord = user.getTetrisScore(User.Username);
        }
        private void prepareButtonsAndCommands()
        {
            startGameButtonCommand = new RelayCommand(o => startGameClick("startGameButton"));
            BeginMultiplayerButtonCommand = new RelayCommand(goToMultiplayer);
            MoveLeftCommand = new RelayCommand(moveLeft);
            MoveRightCommand = new RelayCommand(moveRight);
            RotateCommand = new RelayCommand(rotate);
            MoveFasterCommand = new RelayCommand(moveFaster);
        }
        private void updateRekord()
        {
            if (score > rekord)
                Rekord = score;

            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS; Initial Catalog=LibraryDB; Integrated Security=True;");
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query;
                SqlCommand cmd;
                query = "SELECT COUNT(1) FROM user_tbl WHERE Login=@Login";
                cmd = new SqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Login", User.Username);


                query = "UPDATE user_tbl SET TetrisScore=@TetrisScore WHERE Login=@Login";

                cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TetrisScore", Rekord);
                cmd.Parameters.AddWithValue("@Login", User.Username);
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