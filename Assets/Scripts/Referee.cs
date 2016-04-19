using System;

namespace Assets.Scripts
{
    public class Referee
    {
        public static int Score(Board board)
        {
            var s = 0;
            s += HorizontalScore(board);
            s += VerticalScore(board);
            s += AllDiagonalUpScore(board);
            s += AllDiagonalDownScore(board);
            return s;
        }

        private static int HorizontalScore(Board board)
        {
            var s = 0;
            for (int y = 0; y < board.Height; y++)
            {
                s += RowScore(y, board);
            }
            return s;
        }

        private static int VerticalScore(Board board)
        {
            int s = 0;
            for (int x = 0; x < board.Width; x++)
            {
                s += ColumnScore(x, board);
            }
            return s;
        }

        private static int RowScore(int y, Board board)
        {
            int s = 0;
            for (int x = 0; x < board.Width - 3; x++)
            {
                s += Heuristic(board[x, y], board[x + 1, y], board[x + 2, y], board[x + 3, y]);
            }
            return s;
        }

        private static int ColumnScore(int x, Board board)
        {
            int s = 0;
            for (int y = 0; y < board.Height - 3; y++)
            {
                s += Heuristic(board[x, y], board[x, y + 1], board[x, y + 2], board[x, y + 3]);
            }
            return s;
        }

        private static int AllDiagonalDownScore(Board board)
        {
            int s = 0;
            for (int x = 0; x < board.Width - 3; x++)
            {
                for (int y = 0; y < board.Height - 3; y++)
                {
                    s += DiagonalDownScore(x, y, board);
                }
            }
            return s;
        }

        private static int AllDiagonalUpScore(Board board)
        {
            int s = 0;
            for (int x = 0; x < board.Width - 3; x++)
            {
                for (int y = 0; y < board.Height - 3; y++)
                {
                    s += DiagonalUpScore(x, y + 3, board);
                }
            }
            return s;
        }

        private static int DiagonalDownScore(int x, int y, Board board)
        {
            return Heuristic(board[x, y], board[x + 1, y + 1], board[x + 2, y + 2], board[x + 3, y + 3]);
        }

        private static int DiagonalUpScore(int x, int y, Board board)
        {
            return Heuristic(board[x, y], board[x + 1, y - 1], board[x + 2, y - 2], board[x + 3, y - 3]);
        }

        private static int Heuristic(int a, int b, int c, int d)
        {
            var x = 0;
            var o = 0;
            switch (a)
            {
                case 1:
                    x++;
                    break;
                case 2:
                    o++;
                    break;
            }
            switch (b)
            {
                case 1:
                    x++;
                    break;
                case 2:
                    o++;
                    break;
            }
            switch (c)
            {
                case 1:
                    x++;
                    break;
                case 2:
                    o++;
                    break;
            }
            switch (d)
            {
                case 1:
                    x++;
                    break;
                case 2:
                    o++;
                    break;
            }

            if (x > 0 && o > 0)
                return 0;
            if (x > 0) return (int) (Math.Pow(10, x - 1));
            if (o > 0) return -(int) (Math.Pow(10, o - 1));
            return 0;
        }
    }
}