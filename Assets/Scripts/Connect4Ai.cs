using System;

namespace Assets.Scripts
{
    public class Connect4Ai
    {
        private readonly Board _b;
        private int _nextMoveLocation = -1;
        private int _maxDepth = 7;

        public Connect4Ai(Board b)
        {
            _b = b;
        }


        public void HumanMove(int move, int player)
        {
            _b.PlaceMove(move - 1, player);
        }

        public int GameResult(Board b)
        {
            int aiScore = 0, humanScore = 0;
            for (var i = 5; i >= 0; --i)
            {
                for (var j = 0; j <= 6; ++j)
                {
                    if (b[i, j] == 0) continue;

                    if (j <= 3)
                    {
                        for (var k = 0; k < 4; ++k)
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
                        for (var k = 0; k < 4; ++k)
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
                    if (j < 3 || i < 3) continue;
                    {
                        for (var k = 0; k < 4; ++k)
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

            for (var j = 0; j < 7; ++j)
            {
                //Game has not ended yet
                if (b[0, j] == 0) return -1;
            }
            //Game draw!
            return 0;
        }

        //Evaluate board favorableness for AI
        public int EvaluateBoard(Board board, int player)
        {
            return -1 * Referee.Score(board);
        }

        public int Minimax(int depth, int turn, int alpha, int beta, int player) // turn - 1 for max, 2 for min
        {
            if (beta <= alpha)
            {
                return (turn == 1 ? int.MaxValue : int.MinValue);
            }
            var gameResult = GameResult(_b);
            
            switch (gameResult)
            {
                case 1:
                    return int.MaxValue/2;
                case 2:
                    return int.MinValue/2;
                case 0:
                    return 0;
            }
            
            if (depth == _maxDepth) return EvaluateBoard(_b, player);

            int maxScore = int.MinValue, minScore = int.MaxValue;

            for (var j = 0; j <= 6; ++j)
            {
                var currentScore = 0;

                if (!_b.IsLegalMove(j)) continue;

                if (turn == 1)
                {
                    _b.PlaceMove(j, 1);
                    currentScore = Minimax(depth + 1, 2, alpha, beta, player);

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
                    currentScore = Minimax(depth + 1, 1, alpha, beta, player);
                    minScore = Math.Min(currentScore, minScore);

                    beta = Math.Min(currentScore, beta);
                }
                _b.UndoMove(j);
                if (currentScore == int.MaxValue || currentScore == int.MinValue) break;
            }
            return turn == 1 ? maxScore : minScore;
        }

        public int GetAiMove(int depth, int player)
        {
            _maxDepth = depth;
            _nextMoveLocation = 0;
            
            Minimax(0, 1, int.MinValue, int.MaxValue, player);
            return _nextMoveLocation;
        }
    }
}