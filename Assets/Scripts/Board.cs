using UnityEngine;

namespace Assets.Scripts
{
    public class Board
    {
        private readonly int[,] _board;
        private readonly GameController _controller;
        internal readonly int Width = 6;
        internal readonly int Height = 7;

        public int this[int x, int y]
        {
            get { return _board[x, y]; }
        }


        public Board(GameController controller)
        {
            _board = new[,]
            {
                {0, 0, 0, 0, 0, 0, 0,}, {0, 0, 0, 0, 0, 0, 0,}, {0, 0, 0, 0, 0, 0, 0,}, {0, 0, 0, 0, 0, 0, 0,}, {0, 0, 0, 0, 0, 0, 0,}, {0, 0, 0, 0, 0, 0, 0,},
            };

            _controller = controller;
        }

        public bool IsLegalMove(int column)
        {
            return column < 7 && -1 < column && _board[0, column] == 0;
        }

        //Placing a Move on the board
        public bool PlaceMove(int column, int player)
        {
            if (!IsLegalMove(column))
            { 
                return false;
            }

            for (var i = 5; i >= 0; --i)
            {
                if (_board[i, column] != 0) continue;
                _board[i, column] = player;
                return true;
            }
            return false;
        }

        public void UndoMove(int column)
        {
            for (var i = 0; i <= 5; ++i)
            {
                if (_board[i, column] == 0) continue;
                _board[i, column] = 0;
                break;
            }
        }

        public void DisplayBoard()
        {
            for (var i = 0; i <= 5; ++i)
            {
                for (var j = 0; j <= 6; ++j)
                {
                    _controller.BoardObjets[i, j].GetComponent<Renderer>().material.color = _board[i, j] == 1 ? Color.red : _board[i, j] == 2 ? Color.green : Color.white;
                }
            }
        }
    }
}