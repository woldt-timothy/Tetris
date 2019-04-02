using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFTetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        Board myBoard;

        public MainWindow()
        {

            InitializeComponent();
        }
        void MainWindow_Initilized(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(GameTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            GameStart();
        }

        //Button Click Starts
        private void Button_Start(object sender, RoutedEventArgs e)
        {
            GameStart();
        }

        private void Button_Pause(object sender, RoutedEventArgs e)
        {
            GamePause();
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            GameEnd();
        }
        // Button Click Ends

        private void GameStart()
        {
            MainGrid.Children.Clear();
            myBoard = new Board(MainGrid);
            Timer.Start();
        }

        private void GamePause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }

        private void GameEnd()
        {
            Close();
        }

        void GameTick(object sender, EventArgs e)
        {

            Score.Text = myBoard.getScore().ToString("000000");
            Line.Text = myBoard.getLines().ToString("000000");
            myBoard.CurrTetraminoMoveDown();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Timer.IsEnabled) myBoard.CurrTetraminoMoveLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) myBoard.CurrTetraminoMoveRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) myBoard.CurrTetraminoMoveDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) myBoard.CurrTetraminoMoveRotate();
                    break;
                default:
                    break;
            }
        }
    }

    public class Tetramino
    {
        private Point currPosition;
        private Point[] currShape;
        private Brush currColor;
        private bool rotate;
        public Tetramino()
        {
            currPosition = new Point(0, -1);
            currColor = Brushes.Transparent;
            currShape = setRandomShape();
        }
        public Brush getCurrColor() { return currColor; }
        public Point getCurrPosition() { return currPosition; }
        public Point[] getCurrShape() { return currShape; }
        public void moveLeft() { currPosition.X -= 1; }
        public void moveRight() { currPosition.X += 1; }
        public void moveDown() { currPosition.Y += 1; }
        public void moveRotate()
        {
            if (rotate)
            {
                for (int i = 0; i < currShape.Length; i++)
                {
                    double x = currShape[i].X;
                    currShape[i].X = currShape[i].Y * -1;
                    currShape[i].Y = x;
                }
            }
        }

        private Point[] setRandomShape()
        {
            Random rand = new Random();
            switch (rand.Next() % 7)
            {
                case 0: 
                    rotate = true;
                    currColor = Brushes.GreenYellow;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)
                    };
                case 1: 
                    rotate = true;
                    currColor = Brushes.Blue;
                    return new Point[] {
                        new Point(1,-1),
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0)
                    };
                case 2: 
                    rotate = true;
                    currColor = Brushes.Orange;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(1,-1)
                    };
                case 3: 
                    rotate = false;
                    currColor = Brushes.Lime;
                    return new Point[] {
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)
                    };
                case 4: 
                    rotate = true;
                    currColor = Brushes.Green;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0)
                    };
                case 5: 
                    rotate = true;
                    currColor = Brushes.Purple;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(0,1)
                    };
                case 6: 
                    rotate = true;
                    currColor = Brushes.Red;
                    return new Point[] {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,1),
                        new Point(1,1)
                    };
                default:
                    return null;
            }
        }

    }



    public class Board
    {
        private int Rows;
        private int Cols;
        private int Score;
        private int LinesFilled;
        private Tetramino currTetris;
        private Label[,] BlockControls;

        static private Brush NoBrush = Brushes.Transparent;
        static private Brush PurpleBrush = Brushes.Black;
        public Board(Grid TetrisGrid)
        {
            Rows = TetrisGrid.RowDefinitions.Count;
            Cols = TetrisGrid.ColumnDefinitions.Count;

            Score = 0;
            LinesFilled = 0;

            BlockControls = new Label[Cols, Rows];
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    BlockControls[i, j] = new Label();
                    BlockControls[i, j].Background = NoBrush;
                    BlockControls[i, j].BorderBrush = PurpleBrush;
                    BlockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(BlockControls[i, j], j);
                    Grid.SetColumn(BlockControls[i, j], i);
                    TetrisGrid.Children.Add(BlockControls[i, j]);
                }
            }
            currTetris = new Tetramino();
            currTetrisDraw();
        }
        public int getScore() { return Score; }
        public int getLines() { return LinesFilled; }
        private void currTetrisDraw()
        {
            
            Point Position = currTetris.getCurrPosition();
            
            Point[] Shape = currTetris.getCurrShape();
            
            Brush Color = currTetris.getCurrColor();
            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1),
                              (int)(S.Y + Position.Y) + 2].Background = Color;
            }
        }
        private void currTetraminoErase()
        {
            
            Point Position = currTetris.getCurrPosition();
            
            Point[] Shape = currTetris.getCurrShape();
            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + Position.X) + ((Cols / 2) - 1),
                              (int)(S.Y + Position.Y) + 2].Background = NoBrush;
            }
        }
        private void CheckRows()
        {
            bool Full;
            for (int i = Rows - 1; i > 0; i--)
            {
                Full = true;
                for (int j = 0; j < Cols; j++)
                {
                    if (BlockControls[j, i].Background == NoBrush)
                    {
                        Full = false;
                    }
                }
                if (Full)
                {
                    RemoveRow(i);
                    CheckRows();
                    Score += 10;
                    LinesFilled += 1;
                }
            }
        }
        private void RemoveRow(int row)
        {
            for (int i = row; i > 1; i--)
            {
                for (int j = 0; j < Cols; j++)
                {
                    BlockControls[j, i].Background = BlockControls[j, i - 1].Background;
                }
            }
        }
        public void CurrTetraminoMoveLeft()
        {
            Point Position = currTetris.getCurrPosition();
            Point[] Shape = currTetris.getCurrShape();
            bool move = true;
            currTetraminoErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1) < 0)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1),
                                        (int)(S.Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetris.moveLeft();
                currTetrisDraw();
            }
            else
            {
                currTetrisDraw();
            }
        }
        public void CurrTetraminoMoveRight()
        {
            Point Position = currTetris.getCurrPosition();
            Point[] Shape = currTetris.getCurrShape();
            bool move = true;
            currTetraminoErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1) >= Cols)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1),
                                     (int)(S.Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetris.moveRight();
                currTetrisDraw();
            }
            else
            {
                currTetrisDraw();
            }
        }
        public void CurrTetraminoMoveDown()
        {
            Point Position = currTetris.getCurrPosition();
            Point[] Shape = currTetris.getCurrShape();
            bool move = true;
            currTetraminoErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.Y + Position.Y) + 2 + 1) >= Rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1)),
                                     (int)(S.Y + Position.Y) + 2 + 1].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetris.moveDown();
                currTetrisDraw();
            }
            else
            {
                currTetrisDraw();
                CheckRows();
                currTetris = new Tetramino();
            }
        }
        public void CurrTetraminoMoveRotate()
        {
            Point Position = currTetris.getCurrPosition();
            Point[] S = new Point[4];
            Point[] Shape = currTetris.getCurrShape();
            bool move = true;
            Shape.CopyTo(S, 0);
            currTetraminoErase();
            for (int i = 0; i < S.Length; i++)
            {
                double x = S[i].X;
                S[i].X = S[i].Y * -1;
                S[i].Y = x;
                if (((int)((S[i].Y + Position.Y) + 2)) > Rows)
                {
                    move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) < 0)
                {
                    move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) >= Rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S[i].X + Position.X) + ((Cols / 2) - 1)),
                                        (int)(S[i].Y + Position.Y + 2)].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetris.moveRotate();
                currTetrisDraw();
            }
            else
            {
                currTetrisDraw();
            }
        }
    }
}
