using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System;
using System.Reflection;
using System.Collections.Generic;
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
        private static int timeSpan = 400;
        bool started;
        private int score;
        private int rekord;
        private int superSpeed = 100;
        private double difficultyLevel;
        private int difficultyCounter;
        private bool gameover;
        private int level { get; set; }
        private DispatcherTimer dispatcherTimer { get; set; }
        private Block currBlock;
        private ObservableCollection<ObservableCollection<Brush>> board;
        private Brush noBrush;
        private string timer;
        public ICommand startGameButtonCommand { get; set; }
        #region Get&Set
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

        public GameViewModel()
        {
            userName = User.Username;
            startGameButtonCommand = new RelayCommand(o => startGameClick("startGameButton"));
            MoveLeftCommand = new RelayCommand(moveLeft);
            MoveRightCommand = new RelayCommand(moveRight);
            RotateCommand = new RelayCommand(rotate);
            MoveFasterCommand = new RelayCommand(moveFaster);

            started = false;
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
                    var newBrush = noBrush;
                    board[i].Add(newBrush);
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
                            if ((int)(position.Y) + i * fluentMoveRate + a >= rows || (int)(position.X) + j >= cols) break;
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

        private void DispatcherTimerSetup()
        {
            this.TimerStart = DateTime.Now;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, timeSpan / fluentMoveRate);
            dispatcherTimer.Tick += new EventHandler(TimeTick);
            dispatcherTimer.Start();
        }

        private DateTime TimerStart { get; set; }

        private void TimeTick(object sender, EventArgs e)
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
            if (!started)
            {
                DispatcherTimerSetup();
                started = true;
            }
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
            Rekord = user.getTetrisScore();
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