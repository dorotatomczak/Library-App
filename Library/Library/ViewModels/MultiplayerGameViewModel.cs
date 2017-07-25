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
    public class MultiplayerGameViewModel : ViewModelBase
    {
        
        #region Game Info
        private static int fluentMoveRate = 5;
        private static int cols = 10;
        private static int rows = 20 * fluentMoveRate;
        private static int difficultyRate = rows * 30;
        private static int timeSpan = 400;
        private int superSpeed = 100;
        private double difficultyLevel;
        private int difficultyCounter;
        private int level { get; set; }
        private Brush noBrush;
        private NavigationViewModel nvm;
        private DateTime TimerStart;
        private string timer;
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
        
        
        public ICommand EndMultiplayerButtonCommand
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

        #endregion
        #region Player 1 info
        private string userName1;
        public string UserName1
        {
            get { return userName1; }
            set
            {
                this.userName1 = value;
                OnPropertyChanged("UserName1");
            }
        }
        private int score1;
        private int rekord1;
        private bool gameover1;
        private DispatcherTimer dispatcherTimer1 { get; set; }
        private Block currBlock1;
        private ObservableCollection<ObservableCollection<Brush>> board1;
        public ObservableCollection<ObservableCollection<Brush>> Board1
        {
            get { return board1; }
            set
            {
                if (board1 != value)
                {
                    board1 = value;
                    OnPropertyChanged("Board1");
                }
            }
        }
        public int Score1
        {
            get { return score1; }
            set
            {
                if (score1 != value)
                {
                    score1 = value;
                    OnPropertyChanged("Score1");
                }
            }
        }
        public int Rekord1
        {
            get { return rekord1; }
            set
            {
                if (rekord1 != value)
                {
                    rekord1 = value;
                    OnPropertyChanged("Rekord1");
                }
            }
        }
        public ICommand MoveLeftCommand1
        {
            get;
            private set;
        }
        public ICommand MoveRightCommand1
        {
            get;
            private set;
        }
        public ICommand RotateCommand1
        {
            get;
            private set;
        }
        public ICommand MoveFasterCommand1
        {
            get;
            private set;
        }
        public int getScore1()
        {
            return score1;
        }
        private void currBlockDraw1()
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            Brush color = currBlock1.getColor();
            bool canBeDrawn = true;

            //check if can be drawn
            for (int i = 0; i < currBlock1.getHeight(); i++)
            {
                for (int j = 0; j < currBlock1.getWidth(); j++)
                {
                    if (shape[j, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if ((int)(position.Y) + i * fluentMoveRate + a >= rows || (int)(position.X) + j >= cols) break;
                            if (board1[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                            {
                                canBeDrawn = false;
                            }
                        }
                    }
                }
            }

            if (canBeDrawn)
            {
                for (int i = 0; i < currBlock1.getWidth(); i++)
                {
                    for (int j = 0; j < currBlock1.getHeight(); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            for (int k = 0; k < fluentMoveRate; k++)
                            {
                                board1[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = color;
                            }
                        }
                    }
                }
            }
            else
            {
                currBlock1.moveUp();
                currBlockDraw1();
            }
        }
        private void currBlockErase1()
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        for (int k = 0; k < fluentMoveRate; k++)
                        {

                            board1[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = noBrush;
                        }
                    }
                }
            }
        }
        private void checkRows1()
        {
            bool full;
            for (int i = rows - 1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < cols; j++)
                {
                    if (board1[i][j] == noBrush)
                    {
                        full = false;
                    }
                }
                if (full)
                {
                    for (int a = 0; a < fluentMoveRate; a++)
                    {
                        removeRows1(i);
                    }
                    i++;
                    Score1 += 100;
                }
            }
        }
        private void removeRows1(int row)
        {
            for (int i = row; i > 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    board1[i][j] = board1[i - 1][j];
                }
            }
        }
        #region Block moves
        public void moveLeft1(object param)
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            bool move = true;
            currBlockErase1();
            if (position.X <= 0)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock1.getHeight(); i++)
                {

                    if (shape[0, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board1[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) - 1] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }

                }
            }

            if (move)
            {
                currBlock1.moveLeft();
            }
            currBlockDraw1();
        }
        public void moveRight1(object param)
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            currBlockErase1();
            bool move = true;
            if (position.X + currBlock1.getWidth() >= cols)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock1.getHeight(); i++)
                {

                    if (shape[currBlock1.getHeight() - 1, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board1[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + currBlock1.getWidth()] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }
                }
            }
            if (move)
            {
                currBlock1.moveRight();
            }
            currBlockDraw1();
        }
        public void moveFaster1(object param)
        {
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / superSpeed / difficultyLevel));
        }
        public void moveSlower1()
        {
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / difficultyLevel));
        }
        public void moveDown1()
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            bool move = true;
            if (position.Y + currBlock1.getHeight() * fluentMoveRate >= rows)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock1.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock1.getWidth(); j++)
                    {
                        if (shape[j, i] == 1 && (i == currBlock1.getHeight() - 1 || shape[j, i + 1] == 0))
                        {
                            if (board1[(int)(position.Y) + i * fluentMoveRate + fluentMoveRate][(int)(position.X) + j] != noBrush)
                            {
                                move = false;
                            }

                        }
                    }
                }
            }
            if (move)
            {
                currBlockErase1();
                currBlock1.moveDown();
                currBlockDraw1();
            }
            else
            {
                if (position.Y == 0)
                {
                    gameover1 = true;
                    return;
                }
                checkRows1();
                currBlock1 = new Block();
            }
        }
        public void rotate1(object param)
        {
            Point position = currBlock1.getPosition();
            int[,] shape = currBlock1.getMatrix();
            currBlockErase1();
            currBlock1.turnRight();
            bool success = true;
            if (position.X + currBlock1.getWidth() > cols) success = false;
            else
                for (int i = 0; i < currBlock1.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock1.getWidth(); j++)
                    {
                        if (shape[j, i] == 1)
                        {
                            for (int a = 0; a < fluentMoveRate; a++)
                            {
                                if (board1[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                                {
                                    success = false;
                                }
                            }
                        }
                    }
                }
            if (!success)
            {
                currBlock1.turnLeft();
            }
            currBlockDraw1();
        }
        #endregion
        private void DispatcherTimer1Setup()
        {
            TimerStart = DateTime.Now;
            dispatcherTimer1 = new DispatcherTimer();
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, timeSpan / fluentMoveRate);
            dispatcherTimer1.Tick += new EventHandler(TimeTick1);
            dispatcherTimer1.Start();
        }
        

        private void TimeTick1(object sender, EventArgs e)
        {
            if (gameover1 || gameover2)
            {
                ((DispatcherTimer)sender).Stop();
                dispatcherTimer1.Tick -= TimeTick1;
                dispatcherTimer2.Stop();
                dispatcherTimer2.Tick -= TimeTick2;
                if (gameover1)
                {
                    MessageBox.Show($"Koniec gry. Wygrał {userName2}");
                }
                else
                {
                    MessageBox.Show($"Koniec gry. Wygrał {userName1}");
                }
                gameover1 = gameover2 = false;
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
            Board1 = board1;
            moveDown1();
            moveSlower1();
            updateRekord1();
        }

       
        private void updateRekord1()
        {
            if (score1 > rekord1)
                Rekord1 = score1;

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
                cmd.Parameters.AddWithValue("@TetrisScore", Rekord1);
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

        #endregion        #region Player 1 info
        #region Player 2 info
        private string userName2;
        public string UserName2
        {
            get { return userName2; }
            set
            {
                this.userName2 = value;
                OnPropertyChanged("UserName2");
            }
        }
        private int score2;
        private int rekord2;
        private bool gameover2;
        private DispatcherTimer dispatcherTimer2 { get; set; }
        private Block currBlock2;
        private ObservableCollection<ObservableCollection<Brush>> board2;
        public ObservableCollection<ObservableCollection<Brush>> Board2
        {
            get { return board2; }
            set
            {
                if (board2 != value)
                {
                    board2 = value;
                    OnPropertyChanged("Board2");
                }
            }
        }
        public int Score2
        {
            get { return score2; }
            set
            {
                if (score2 != value)
                {
                    score2 = value;
                    OnPropertyChanged("Score2");
                }
            }
        }
        public int Rekord2
        {
            get { return rekord2; }
            set
            {
                if (rekord2 != value)
                {
                    rekord2 = value;
                    OnPropertyChanged("Rekord2");
                }
            }
        }
        public ICommand MoveLeftCommand2
        {
            get;
            private set;
        }
        public ICommand MoveRightCommand2
        {
            get;
            private set;
        }
        public ICommand RotateCommand2
        {
            get;
            private set;
        }
        public ICommand MoveFasterCommand2
        {
            get;
            private set;
        }
        public int getScore2()
        {
            return score2;
        }
        private void currBlockDraw2()
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            Brush color = currBlock2.getColor();
            bool canBeDrawn = true;

            //check if can be drawn
            for (int i = 0; i < currBlock2.getHeight(); i++)
            {
                for (int j = 0; j < currBlock2.getWidth(); j++)
                {
                    if (shape[j, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if ((int)(position.Y) + i * fluentMoveRate + a >= rows || (int)(position.X) + j >= cols) break;
                            if (board2[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                            {
                                canBeDrawn = false;
                            }
                        }
                    }
                }
            }

            if (canBeDrawn)
            {
                for (int i = 0; i < currBlock2.getWidth(); i++)
                {
                    for (int j = 0; j < currBlock2.getHeight(); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            for (int k = 0; k < fluentMoveRate; k++)
                            {
                                board2[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = color;
                            }
                        }
                    }
                }
            }
            else
            {
                currBlock2.moveUp();
                currBlockDraw2();
            }
        }
        private void currBlockErase2()
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        for (int k = 0; k < fluentMoveRate; k++)
                        {

                            board2[(int)(j * fluentMoveRate + position.Y + k)][(int)(i + position.X)] = noBrush;
                        }
                    }
                }
            }
        }
        private void checkRows2()
        {
            bool full;
            for (int i = rows - 1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < cols; j++)
                {
                    if (board2[i][j] == noBrush)
                    {
                        full = false;
                    }
                }
                if (full)
                {
                    for (int a = 0; a < fluentMoveRate; a++)
                    {
                        removeRows2(i);
                    }
                    i++;
                    Score2 += 100;
                }
            }
        }
        private void removeRows2(int row)
        {
            for (int i = row; i > 0; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    board2[i][j] = board2[i - 1][j];
                }
            }
        }
        #region Block moves
        public void moveLeft2(object param)
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            bool move = true;
            currBlockErase2();
            if (position.X <= 0)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock2.getHeight(); i++)
                {

                    if (shape[0, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board2[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) - 1] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }

                }
            }

            if (move)
            {
                currBlock2.moveLeft();
            }
            currBlockDraw2();
        }
        public void moveRight2(object param)
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            currBlockErase2();
            bool move = true;
            if (position.X + currBlock2.getWidth() >= cols)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock2.getHeight(); i++)
                {

                    if (shape[currBlock2.getHeight() - 1, i] == 1)
                    {
                        for (int a = 0; a < fluentMoveRate; a++)
                        {
                            if (board2[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + currBlock2.getWidth()] != noBrush)
                            {
                                move = false;
                            }
                        }
                    }
                }
            }
            if (move)
            {
                currBlock2.moveRight();
            }
            currBlockDraw2();
        }
        public void moveFaster2(object param)
        {
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / superSpeed / difficultyLevel));
        }
        public void moveSlower2()
        {
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, (int)(timeSpan / fluentMoveRate / difficultyLevel));
        }
        public void moveDown2()
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            bool move = true;
            if (position.Y + currBlock2.getHeight() * fluentMoveRate >= rows)
            {
                move = false;
            }
            else
            {
                for (int i = 0; i < currBlock2.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock2.getWidth(); j++)
                    {
                        if (shape[j, i] == 1 && (i == currBlock2.getHeight() - 1 || shape[j, i + 1] == 0))
                        {
                            if (board2[(int)(position.Y) + i * fluentMoveRate + fluentMoveRate][(int)(position.X) + j] != noBrush)
                            {
                                move = false;
                            }

                        }
                    }
                }
            }
            if (move)
            {
                currBlockErase2();
                currBlock2.moveDown();
                currBlockDraw2();
            }
            else
            {
                if (position.Y == 0)
                {
                    gameover2 = true;
                    return;
                }
                checkRows2();
                currBlock2 = new Block();
            }
        }
        public void rotate2(object param)
        {
            Point position = currBlock2.getPosition();
            int[,] shape = currBlock2.getMatrix();
            currBlockErase2();
            currBlock2.turnRight();
            bool success = true;
            if (position.X + currBlock2.getWidth() > cols) success = false;
            else
                for (int i = 0; i < currBlock2.getHeight(); i++)
                {
                    for (int j = 0; j < currBlock2.getWidth(); j++)
                    {
                        if (shape[j, i] == 1)
                        {
                            for (int a = 0; a < fluentMoveRate; a++)
                            {
                                if (board2[(int)(position.Y) + i * fluentMoveRate + a][(int)(position.X) + j] != noBrush)
                                {
                                    success = false;
                                }
                            }
                        }
                    }
                }
            if (!success)
            {
                currBlock2.turnLeft();
            }
            currBlockDraw2();
        }
        #endregion
        private void DispatcherTimer2Setup()
        {
            dispatcherTimer2 = new DispatcherTimer();
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 0, timeSpan / fluentMoveRate);
            dispatcherTimer2.Tick += new EventHandler(TimeTick2);
            dispatcherTimer2.Start();
        }


        private void TimeTick2(object sender, EventArgs e)
        {
            if (gameover1 || gameover2)
            {
                ((DispatcherTimer)sender).Stop();
                dispatcherTimer1.Tick -= TimeTick1;
                dispatcherTimer2.Stop();
                dispatcherTimer2.Tick -= TimeTick2;
                if (gameover1)
                {
                MessageBox.Show($"Koniec gry. Wygrał {userName2}");
                }
                else
                {
                    MessageBox.Show($"Koniec gry. Wygrał {userName1}");
                }
                gameover1 = gameover2 = false;
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
            Board2 = board2;
            moveDown2();
            moveSlower2();
            updateRekord2();
        }


        private void updateRekord2()
        {
            if (score2 > rekord2)
                Rekord2 = score2;

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
                cmd.Parameters.AddWithValue("@Login", User.Username2);


                query = "UPDATE user_tbl SET TetrisScore=@TetrisScore WHERE Login=@Login";

                cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TetrisScore", Rekord2);
                cmd.Parameters.AddWithValue("@Login", User.Username2);
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

#endregion


        public MultiplayerGameViewModel(NavigationViewModel nvm)
        {
            this.nvm = nvm;
            UserName1 = User.Username;
            UserName2 = User.Username2;
            prepareButtonsAndCommands();
            board1 = new ObservableCollection<ObservableCollection<Brush>>();
            board2 = new ObservableCollection<ObservableCollection<Brush>>();
            timer = "0";
            score1 = 0;
            score2 = 0;
            var block = new Block();
            noBrush = block.getNoBrush();
            this.rekord1 = 0;
            this.rekord2 = 0;
            for (int i = 0; i < rows; i++)
            {
                board1.Add(new ObservableCollection<Brush>());
                board2.Add(new ObservableCollection<Brush>());
                for (int j = 0; j < cols; j++)
                {
                    board1[i].Add(noBrush);
                    board2[i].Add(noBrush);
                }
            }
        }

        private void prepareButtonsAndCommands()
        {
            startGameButtonCommand = new RelayCommand(o => startGameClick("startGameButton"));
            EndMultiplayerButtonCommand = new RelayCommand(goToSingleplayer);
            MoveLeftCommand1 = new RelayCommand(moveLeft1);
            MoveRightCommand1 = new RelayCommand(moveRight1);
            RotateCommand1 = new RelayCommand(rotate1);
            MoveFasterCommand1 = new RelayCommand(moveFaster1);
            MoveLeftCommand2 = new RelayCommand(moveLeft2);
            MoveRightCommand2 = new RelayCommand(moveRight2);
            RotateCommand2 = new RelayCommand(rotate2);
            MoveFasterCommand2 = new RelayCommand(moveFaster2);
        }
        private void goToSingleplayer(object param)
        {
            this.nvm.SelectedViewModel = new GameViewModel(this.nvm);
        }
        private void startGameClick(object sender)
        {
            prepareNewGame();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board1[i][j] = noBrush;
                    board2[i][j] = noBrush;
                }
            }
            currBlock1 = new Block();
            currBlockDraw1();
            currBlock2 = new Block();
            currBlockDraw2();
        }
        private void prepareNewGame()
        {
            DispatcherTimer1Setup();
            DispatcherTimer2Setup();
            this.Level = 1;
            this.score1 = 0;
            this.score2 = 0;
            gameover1 = false;
            gameover2 = false;
            difficultyLevel = 1;
            difficultyCounter = 1;

            NormalUser user = new NormalUser();
            Rekord1 = user.getTetrisScore(userName1);
            Rekord2 = user.getTetrisScore(userName2);
        }
    }
}
