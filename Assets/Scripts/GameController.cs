using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        private enum GameState
        {
            InGame,
            PostScreen
        }

        private GameState _state = GameState.InGame;
        private Board _b;
        private Connect4Ai _ai;

        public GameObject[,] BoardObjets = new GameObject[6, 7];
        public Text InfoText;

        private bool _playerTurn = true;

        public void Start()
        {
            for (var y = 0; y < 6; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    BoardObjets[y, x] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    BoardObjets[y, x].transform.SetParent(transform);
                    BoardObjets[y, x].transform.position = new Vector3(x * 2 - 6, 14 - y * 2 - 7, 0);
                }
            }

            _b = new Board(this);
            _ai = new Connect4Ai(_b);
        
            _b.DisplayBoard();
        }

        private IEnumerator ProcessMove(int move)
        {
            _playerTurn = false;
            _ai.LetOpponentMove(move);
            _b.DisplayBoard();

            var gameResult = _ai.GameResult(_b);

            switch (gameResult)
            {
                case 1:
                    AiWins();
                    break;
                case 2:
                    PlayerWin();
                    break;
                case 0:
                    Draw();
                    break;
            }
            
            yield return new WaitForEndOfFrame();
            var aiMove = _ai.GetAiMove();
            _b.PlaceMove(aiMove, 1);
            yield return new WaitForEndOfFrame();

            _b.DisplayBoard();
            gameResult = _ai.GameResult(_b);

            switch (gameResult)
            {
                case 1:
                    AiWins();
                    break;
                case 2:
                    PlayerWin();
                    break;
                case 0:
                    Draw();
                    break;
            }
            _playerTurn = true;
            
        }

        private void Draw()
        {
            _state = GameState.PostScreen;
            InfoText.text = "Draw!";
        }

        private void PlayerWin()
        {
            _state = GameState.PostScreen;
            InfoText.text = "Player Win!";
        }

        private void AiWins()
        {
            _state = GameState.PostScreen;
            InfoText.text = "AI Win!";
        }

        public void Update()
        {
            switch (_state)
            {
                case GameState.InGame:
                    if (_playerTurn)
                    {
                        if (Input.GetKeyUp(KeyCode.Alpha1))
                        {
                            StartCoroutine(ProcessMove(1));
                        }
                        else if (Input.GetKeyUp(KeyCode.Alpha2))
                        {
                            StartCoroutine(ProcessMove(2));
                        }
                        else if (Input.GetKeyUp(KeyCode.Alpha3))
                        {
                            StartCoroutine(ProcessMove(3));
                        }

                        else if (Input.GetKeyUp(KeyCode.Alpha4))
                        {
                            StartCoroutine(ProcessMove(4));
                        }
                        else if (Input.GetKeyUp(KeyCode.Alpha5))
                        {
                            StartCoroutine(ProcessMove(5));
                        }

                        else if (Input.GetKeyUp(KeyCode.Alpha6))
                        {
                            StartCoroutine(ProcessMove(6));
                        }

                        else if (Input.GetKeyUp(KeyCode.Alpha7))
                        {
                            StartCoroutine(ProcessMove(7));
                        }
                    }
                    break;
                case GameState.PostScreen:
                    if (Input.anyKeyDown)
                    {
                        _state = GameState.InGame;
                        _b = new Board(this);
                        _ai = new Connect4Ai(_b);

                        InfoText.text = "";

                        _b.DisplayBoard();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


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
            return _board[0, column] == 0;
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


    public class Connect4Ai
    {
        private readonly Board _b;
        private int _nextMoveLocation = -1;
        private const int MaxDepth = 7;

        public Connect4Ai(Board b)
        {
            _b = b;
        }


        public void LetOpponentMove(int move)
        {
            _b.PlaceMove(move - 1, 2);
        }

        public int GameResult(Board b)
        {
            int aiScore = 0, humanScore = 0;
            for (int i = 5; i >= 0; --i)
            {
                for (int j = 0; j <= 6; ++j)
                {
                    if (b[i, j] == 0) continue;

                    if (j <= 3)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (b[i, j + k] == 1) aiScore++;
                            else if (b[i, j + k] == 2) humanScore++;
                            else break;
                        }
                        if (aiScore == 4) return 1;
                        else if (humanScore == 4) return 2;
                        aiScore = 0;
                        humanScore = 0;
                    }

                    //Checking cells up
                    if (i >= 3)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (b[i - k, j] == 1) aiScore++;
                            else if (b[i - k, j] == 2) humanScore++;
                            else break;
                        }
                        if (aiScore == 4) return 1;
                        else if (humanScore == 4) return 2;
                        aiScore = 0;
                        humanScore = 0;
                    }

                    //Checking diagonal up-right
                    if (j <= 3 && i >= 3)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (b[i - k, j + k] == 1) aiScore++;
                            else if (b[i - k, j + k] == 2) humanScore++;
                            else break;
                        }
                        if (aiScore == 4) return 1;
                        else if (humanScore == 4) return 2;
                        aiScore = 0;
                        humanScore = 0;
                    }

                    //Checking diagonal up-left
                    if (j >= 3 && i >= 3)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (b[i - k, j - k] == 1) aiScore++;
                            else if (b[i - k, j - k] == 2) humanScore++;
                            else break;
                        }
                        if (aiScore == 4) return 1;
                        else if (humanScore == 4) return 2;
                        aiScore = 0;
                        humanScore = 0;
                    }
                }
            }

            for (int j = 0; j < 7; ++j)
            {
                //Game has not ended yet
                if (b[0, j] == 0) return -1;
            }
            //Game draw!
            return 0;
        }

        //Evaluate board favorableness for AI
        public int EvaluateBoard(Board board)
        {
            return Referee.Score(board);
        }

        public int Minimax(int depth, int turn, int alpha, int beta)
        {
            if (beta <= alpha)
            {
                if (turn == 1) return int.MaxValue;
                else return int.MinValue;
            }
            int gameResult = GameResult(_b);

            if (gameResult == 1) return int.MaxValue/2;
            else if (gameResult == 2) return int.MinValue/2;
            else if (gameResult == 0) return 0;

            if (depth == MaxDepth) return EvaluateBoard(_b);

            int maxScore = int.MinValue, minScore = int.MaxValue;

            for (int j = 0; j <= 6; ++j)
            {
                int currentScore = 0;

                if (!_b.IsLegalMove(j)) continue;

                if (turn == 1)
                {
                    _b.PlaceMove(j, 1);
                    currentScore = Minimax(depth + 1, 2, alpha, beta);

                    if (depth == 0)
                    {
                        if (currentScore > maxScore) _nextMoveLocation = j;
                        if (currentScore == int.MaxValue/2)
                        {
                            _b.UndoMove(j);
                            break;
                        }
                    }

                    maxScore = Math.Max(currentScore, maxScore);

                    alpha = Math.Max(currentScore, alpha);
                }
                else if (turn == 2)
                {
                    _b.PlaceMove(j, 2);
                    currentScore = Minimax(depth + 1, 1, alpha, beta);
                    minScore = Math.Min(currentScore, minScore);

                    beta = Math.Min(currentScore, beta);
                }
                _b.UndoMove(j);
                if (currentScore == int.MaxValue || currentScore == int.MinValue) break;
            }
            return turn == 1 ? maxScore : minScore;
        }

        public int GetAiMove()
        {
            _nextMoveLocation = -1;
            
            Minimax(0, 1, int.MinValue, int.MaxValue);
            return _nextMoveLocation;
        }
    }


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