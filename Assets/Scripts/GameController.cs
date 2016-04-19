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
            PostScreen,
            ConfigPlayer1,
            ConfigPlayer2
        }

        public enum PlayerType
        {
            Human,
            AiEasy,
            AiMedium,
            AiHard,
            AiUber
        }

        private GameState _state = GameState.ConfigPlayer1;
        private Board _b;
        private Connect4Ai _ai;

        public GameObject[,] BoardObjets = new GameObject[6, 7];
        public Text InfoText;

        public PlayerType Player1 = PlayerType.AiUber; // 1 on board
        public PlayerType Player2 = PlayerType.AiUber; // 2 on board

        private bool _firstPlayerTurn = true;
        

        public void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            for (var y = 0; y < 6; y++)
            {
                for (var x = 0; x < 7; x++)
                {
                    BoardObjets[y, x] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    BoardObjets[y, x].transform.SetParent(transform);
                    BoardObjets[y, x].transform.position = new Vector3(x*2 - 6, 14 - y*2 - 7, 0);
                }
            }

            _b = new Board(this);
            _ai = new Connect4Ai(_b);

            _b.DisplayBoard();
        }

        private void AiMove(int player, int depth)
        {
            if (Input.anyKeyDown)
            {

                var aiMove = _ai.GetAiMove(depth, player);
                _b.PlaceMove(aiMove, player);

                _b.DisplayBoard();
                var gameResult = _ai.GameResult(_b);

                switch (gameResult)
                {
                    case 1:
                        Player1Win();
                        break;
                    case 2:
                        Player2Win();
                        break;
                    case 0:
                        Draw();
                        break;
                }

                _firstPlayerTurn = !_firstPlayerTurn;
            }
        }
    

        private void PlayerMove(int move, int player)
        {
            _firstPlayerTurn = !_firstPlayerTurn;
            _ai.HumanMove(move, player); 
            _b.DisplayBoard();
             
            var gameResult = _ai.GameResult(_b);

            switch (gameResult)
            {
                case 1:
                    Player1Win();
                    break;
                case 2:
                    Player2Win();
                    break;
                case 0:
                    Draw();
                    break;
            }
        }

        private void Draw()
        {
            _state = GameState.PostScreen;
            InfoText.text = "Draw!";
        }

        private void Player2Win()
        {
            _state = GameState.PostScreen;
            InfoText.text = "Player2 Win! : " + Player2;
        }

        private void Player1Win()
        {
            _state = GameState.PostScreen;
            InfoText.text = "Player1 Win! " + Player1;
        }

        public void Update()
        {
            switch (_state)
            {
                case GameState.InGame:
                    DoInGame();
                    break;
                case GameState.PostScreen:
                    DoPostScreen();
                    break;
                case GameState.ConfigPlayer1:
                    DoConfigPlayer1();
                    break;
                case GameState.ConfigPlayer2:
                    DoConfigPlayer2();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoConfigPlayer2()
        {
            InfoText.text = "Player 2 (green): ";
            for (var kc = KeyCode.Alpha1; kc <= KeyCode.Alpha4; kc++)
            {
                if (!Input.GetKeyUp(kc)) continue;
                switch ((int) kc - 49)
                {
                    case 0:
                        Player2 = PlayerType.Human;
                        break;
                    case 1:
                        Player2 = PlayerType.AiEasy;
                        break;
                    case 2:
                        Player2 = PlayerType.AiMedium;
                        break;
                    case 3:
                        Player2 = PlayerType.AiHard;
                        break;
                }
                _state = GameState.InGame;
            }
        }

        private void DoConfigPlayer1()
        {
            InfoText.text = "Player 1 (red): ";
            for (var kc = KeyCode.Alpha1; kc <= KeyCode.Alpha4; kc++)
            {
                if (!Input.GetKeyUp(kc)) continue;
                switch ((int) kc - 49)
                {
                    case 0:
                        Player1 = PlayerType.Human;
                        break;
                    case 1:
                        Player1 = PlayerType.AiEasy;
                        break;
                    case 2:
                        Player1 = PlayerType.AiMedium;
                        break;
                    case 3:
                        Player1 = PlayerType.AiHard;
                        break;
                }
                _state = GameState.ConfigPlayer2;
            }
        }

        private void DoPostScreen()
        {
            if (Input.anyKeyDown)
            {
                _state = GameState.ConfigPlayer1;
                _b = new Board(this);
                _ai = new Connect4Ai(_b);

                InfoText.text = "";

                _b.DisplayBoard();
            }
        }

        private void DoInGame()
        {
            InfoText.text = "";
            ProcessMove(_firstPlayerTurn ? Player1 : Player2, _firstPlayerTurn ? 1 : 2);
        }

        private void ProcessMove(PlayerType type, int player)
        {
            switch (type)
            {
                case PlayerType.Human:
                    for (var kc = KeyCode.Alpha1; kc <= KeyCode.Alpha7; kc++)
                    {
                        if (Input.GetKeyUp(kc))
                        {
                            PlayerMove((int) kc - 48, player);
                        }
                    }
                    break;
                case PlayerType.AiEasy:
                    AiMove(player, 3);
                    break;
                case PlayerType.AiMedium:
                    AiMove(player, 5);
                    break;
                case PlayerType.AiHard:
                    AiMove(player, 7);
                    break;
                case PlayerType.AiUber:
                    AiMove(player, 9);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}