using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using TicTacToe_Game.Models;

namespace TicTacToe_Game.Controllers
{
    public class GameController : Controller
    {
        private DatabaseHelper db = new DatabaseHelper();
        public Game CurrentGame { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(string player1Mark, string opponent)
        {
            char mark = player1Mark == "x" ? 'X' : 'O';
            char opponentMark = mark == 'X' ? 'O' : 'X';

            int player1Id = db.AddPlayer(opponent == "cpu" ? "You" : "Player 1", "human", mark);
            int player2Id = db.AddPlayer(opponent == "cpu" ? "CPU" : "Player 2", opponent, opponentMark);

            int gameId = db.CreateGame(player1Id, player2Id, mark == 'X'? player1Id : player2Id);

            CurrentGame = db.GetGame(gameId);

            return Json(new { success = true, gameId });
        }

        [HttpPost]
        public JsonResult PlayAgain(int player1Id, int player2Id, int xPlayerId)
        {

            int gameId = db.CreateGame(player1Id, player2Id, xPlayerId);

            CurrentGame = db.GetGame(gameId);

            return Json(new { success = true, gameId });
        }


        public ActionResult Play(int gameId = -1)
        {
            if (CurrentGame == null)
                CurrentGame = db.GetGame(gameId);
            if (CurrentGame == null)
                return RedirectToAction("Index", "Game");

            if (CurrentGame.CurrentPlayer is Computer)
            {
                Tut move = ((Computer)CurrentGame.CurrentPlayer).ApplyLogic(CurrentGame);
                MakeMove(CurrentGame.Id, move.Row, move.Column, CurrentGame.CurrentPlayer.Mark);
            }

            return View(CurrentGame);
        }

        [HttpPost]
        public JsonResult MakeMove(int gameId, int row, int col, char symbol)
        {
            if (CurrentGame == null)
                CurrentGame = db.GetGame(gameId);
            /*  if (CurrentGame == null || CurrentGame.Status != GameStatus.Running)
                  return Json(new { success = false, message = "Invalid game." });
            */
            // Prevent overwriting an occupied cell
            Tut existingMove = CurrentGame.gameTuts.FirstOrDefault(t => t.Row == row && t.Column == col);
            if (existingMove != null && (existingMove.Symbol == 'X' || existingMove.Symbol == 'O'))
                return Json(new { success = false, message = "Cell already occupied." });

            existingMove.Symbol = CurrentGame.CurrentPlayer.Mark;

            // Update the move
            db.UpdateTut(gameId, row, col, symbol);

            // Check if this move leads to a win or a draw
            CurrentGame.Status = CheckGameStatus(CurrentGame);

            // Update game status in DB
            db.UpdateGameStatus(gameId, CurrentGame.Status.ToString());

            // Switch Player if Game is Still Running
            if (CurrentGame.Status == GameStatus.Running)
            {
                int nextPlayerId = (CurrentGame.CurrentPlayer.Id == CurrentGame.Player1.Id) ? CurrentGame.Player2.Id : CurrentGame.Player1.Id;
                db.UpdateCurrentPlayer(gameId, nextPlayerId);

                CurrentGame.CurrentPlayer = CurrentGame.Player1.Id == nextPlayerId ? CurrentGame.Player1 : CurrentGame.Player2;
            }

            return Json(new { success = true });
        }


        private GameStatus CheckGameStatus(Game game)
        {
            char[,] board = new char[3, 3];

            foreach (var tut in game.gameTuts)
                board[tut.Row, tut.Column] = tut.Symbol;

            // Check rows, columns, and diagonals
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != ' ' && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return game.Player1.Mark == board[i, 0] ? GameStatus.Player1Won : GameStatus.Player2Won;

                if (board[0, i] != ' ' && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return game.Player1.Mark == board[0, i] ? GameStatus.Player1Won : GameStatus.Player2Won;
            }

            if (board[0, 0] != ' ' && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return game.Player1.Mark == board[0, 0] ? GameStatus.Player1Won : GameStatus.Player2Won;

            if (board[0, 2] != ' ' && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return game.Player1.Mark == board[0, 2] ? GameStatus.Player1Won : GameStatus.Player2Won;

            // Check for a draw (all cells occupied)
            if (game.gameTuts.All(t => t.Symbol == 'X' || t.Symbol == 'O'))
                return GameStatus.Draw;

            return GameStatus.Running;
        }
    }
}
