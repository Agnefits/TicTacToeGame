using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe_Game.Models
{
    public enum GameStatus
    {
        Running,
        Player1Won,
        Player2Won,
        Draw
    }

    public class Game
    {
        public int Id { get; set; }
        public List<Tut> gameTuts { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer { get; set; }
        public GameStatus Status { get; set; }


        public Game(int id, Player player1, Player player2, Player currentPlayer, GameStatus status)
        {
            Id = id;
            Player1 = player1;
            Player2 = player2;
            Status = status;

            CurrentPlayer = currentPlayer; 
        }
 /*
        private void InitializeGameTuts()
        {
            gameTuts = dbHelper.GetGameTuts(Id);
            if (gameTuts.Count == 0)
            {
                gameTuts = new List<Tut>();
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        gameTuts.Add(new Tut(row, col, ' '));
                        dbHelper.UpdateTut(Id, row, col, ' ');
                    }
                }
            }
        }
       
        public void SwitchPlayer()
        {
            CheckWinner();
            if (Status == GameStatus.Running)
            {
                CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
                dbHelper.UpdateGameStatus(Id, Status.ToString());
                if (CurrentPlayer is Computer)
                {
                    MakeMove(((Computer)CurrentPlayer).ApplyLogic(this));
                }
            }
        }

        public void MakeMove(int row, int col)
        {
            Tut move = gameTuts.FirstOrDefault(t => t.Row == row && t.Column == col);
            if (move != null && move.Symbol == ' ')
            {
                move.Symbol = CurrentPlayer.Mark;
                dbHelper.UpdateTut(Id, row, col, CurrentPlayer.Mark);
                SwitchPlayer();
            }
        }

        public void MakeMove(Tut move)
        {
            MakeMove(move.Row, move.Column);
        }

        public void CheckWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (IsWinningLine(gameTuts.Where(t => t.Row == i).ToList()))
                {
                    Status = CurrentPlayer == Player1 ? GameStatus.Player1Won : GameStatus.Player2Won;
                    dbHelper.UpdateGameStatus(Id, Status.ToString());

                    if (Status == GameStatus.Player1Won) Player1.Score++;
                    else Player2.Score++;

                    return;
                }

                if (IsWinningLine(gameTuts.Where(t => t.Column == i).ToList()))
                {
                    Status = CurrentPlayer == Player1 ? GameStatus.Player1Won : GameStatus.Player2Won;
                    dbHelper.UpdateGameStatus(Id, Status.ToString());

                    if (Status == GameStatus.Player1Won) Player1.Score++;
                    else Player2.Score++;

                    return;
                }
            }

            if (IsWinningLine(gameTuts.Where(t => t.Row == t.Column).ToList()))
            {
                Status = CurrentPlayer == Player1 ? GameStatus.Player1Won : GameStatus.Player2Won;
                dbHelper.UpdateGameStatus(Id, Status.ToString());

                if (Status == GameStatus.Player1Won) Player1.Score++;
                else Player2.Score++;

                return;
            }

            if (IsWinningLine(gameTuts.Where(t => t.Row + t.Column == 2).ToList()))
            {
                Status = CurrentPlayer == Player1 ? GameStatus.Player1Won : GameStatus.Player2Won;
                dbHelper.UpdateGameStatus(Id, Status.ToString());

                if (Status == GameStatus.Player1Won) Player1.Score++;
                else Player2.Score++;

                return;
            }

            if (gameTuts.All(t => t.Symbol != ' ')) 
            {
                Status = GameStatus.Draw;
                dbHelper.UpdateGameStatus(Id, "Draw");

                Player1.Draw++;
                Player2.Draw++;
            }
        }

        private bool IsWinningLine(List<Tut> line)
        {
            return line.Count == 3 && line.All(t => t.Symbol == CurrentPlayer.Mark);
        }*/
    }
}
