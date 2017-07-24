using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Library

{
    public class Block
    {
        private int[,] matrix;
        private static int size = 4;
        private int type;
        private int width { set; get; }
        private int height { set; get; }
        private Brush color { get; }
        public bool canMove;
        private Point currPosition;
        private Brush noBrush = Brushes.LightGray;
        public Block(Brush color)
        {
            this.color = color;
        }


        public Block()
        {
            Random rand = new Random();
            this.type = rand.Next() % 7;
            var clr = PickBrush();
            while (clr == noBrush) clr = PickBrush();
            this.color = clr;
            this.canMove = true;
            currPosition = new Point(0, 0);
            matrix = new int[size, size];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++) { matrix[i, j] = 0; }
            }
            switch (type)
            {
                case 1:
                    matrix[1, 0] = 1;
                    matrix[2, 0] = 1;
                    matrix[3, 0] = 1;
                    matrix[0, 0] = 1;
                    width = 4;
                    height = 1;
                    break;
                case 2:
                    matrix[1, 0] = 1;
                    matrix[1, 1] = 1;
                    matrix[0, 1] = 1;
                    matrix[0, 2] = 1;
                    width = 2;
                    height = 3;
                    break;
                case 3:
                    matrix[0, 0] = 1;
                    matrix[0, 1] = 1;
                    matrix[1, 1] = 1;
                    matrix[1, 2] = 1;
                    width = 2;
                    height = 3;
                    break;
                case 4:
                    matrix[0, 0] = 1;
                    matrix[0, 1] = 1;
                    matrix[0, 2] = 1;
                    matrix[1, 2] = 1;
                    width = 2;
                    height = 3;
                    break;
                case 5:
                    matrix[1, 0] = 1;
                    matrix[1, 1] = 1;
                    matrix[1, 2] = 1;
                    matrix[0, 2] = 1;
                    width = 2;
                    height = 3;
                    break;
                case 6:
                    matrix[1, 0] = 1;
                    matrix[0, 1] = 1;
                    matrix[1, 1] = 1;
                    matrix[2, 1] = 1;
                    width = 3;
                    height = 2;
                    break;
                default:
                    matrix[0, 0] = 1;
                    matrix[0, 1] = 1;
                    matrix[1, 0] = 1;
                    matrix[1, 1] = 1;
                    width = 2;
                    height = 2;
                    break;
            }
        }
        public Point getPosition()
        {
            return currPosition;
        }
        public int getHeight()
        {
            return height;
        }
        public int getWidth()
        {
            return width;
        }
        public Brush getColor()
        {
            return color;
        }
        public int[,] getMatrix()
        {
            return matrix;
        }
        public void turnRight()
        {
            int[,] newMatrix = new int[size, size];
            int correction = 4 - this.height;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    newMatrix[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (size - 1 - j - correction < 0) break;
                    newMatrix[size - 1 - j - correction, i] = this.matrix[i, j];
                }
            }
            this.matrix = newMatrix;
            int hlp;
            hlp = this.width;
            this.width = this.height;
            this.height = hlp;
        }

        public void turnLeft()
        {
            int[,] newMatrix = new int[size, size];
            int correction = 4 - this.width;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    newMatrix[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (size - 1 - i - correction < 0) break;
                    newMatrix[j, size - 1 - i - correction] = this.matrix[i, j];
                }
            }
            this.matrix = newMatrix;
            int hlp;
            hlp = this.width;
            this.width = this.height;
            this.height = hlp;
        }
        public void moveLeft()
        {
            currPosition.X -= 1;
        }
        public void moveRight()
        {
            currPosition.X += 1;
        }
        public void moveDown()
        {
            currPosition.Y += 1;
        }
        public void moveUp()
        {
            currPosition.Y -= 1;
        }
        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();
            int color = rnd.Next()%11;

            switch (color)
            {
                case 0:
                    result = Brushes.Chocolate;
                    break;
                case 1:
                    result = Brushes.Blue;
                    break;
                case 2:
                    result = Brushes.Red;
                    break;
                case 3:
                    result = Brushes.Purple;
                    break;
                case 4:
                    result = Brushes.Green;
                    break;
                case 5:
                    result = Brushes.Maroon;
                    break;
                case 6:
                    result = Brushes.Orchid;
                    break;
                case 7:
                    result = Brushes.SeaGreen;
                    break;
                case 8:
                    result = Brushes.Turquoise;
                    break;
                case 9:
                    result = Brushes.RosyBrown;
                    break;
                default:
                    result = Brushes.PeachPuff;
                    break;
            }
            return result;
        }
        public Brush getNoBrush()
        {
            return noBrush;
        }
    }
}
