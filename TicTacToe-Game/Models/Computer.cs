using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TicTacToe_Game.Models
{
    public class Computer : Player
    {
        public Computer(int id, char mark) : base(id, "computer", "CPU", mark) { }
        public Computer(int id, char mark, int score, int draw) : base(id, "computer", "CPU", mark, score, draw) { }

        public static int GenerateRandIndex(List<Tut> gameTuts)
        {
            Random random = new Random();
            return (int)Math.Floor(random.NextDouble() * gameTuts.Count);
        }

        public Tut InitializeMove(List<Tut> gameTuts)
        {
            Tut move = gameTuts[GenerateRandIndex(gameTuts)];
            while (move.Symbol != ' ')
            {
                move = gameTuts[GenerateRandIndex(gameTuts)];
            }
            return move;
        }

        public Tut ApplyLogic(Game game)
        {
            if (game.CurrentPlayer != this) return null;

            // Find winning
            var winningMove = FindBestMove(game, this.Mark);
            if (winningMove != null) return winningMove;

            // Defense
            var defensiveMove = FindBestMove(game, game.Player1.Mark);
            if (defensiveMove != null) return defensiveMove;

            // Find best native move
            var strategicMove = FindStrategicMove(game);
            if (strategicMove != null) return strategicMove;

            // Select randomly
            var availableMoves = game.gameTuts.Where(t => t.Symbol == ' ').ToList();
            if (availableMoves.Any())
            {
                Random random = new Random();
                return availableMoves[random.Next(availableMoves.Count)];
            }
            return null;

        }

        private Tut FindBestMove(Game game, char symbol)
        {
            foreach (var tut in game.gameTuts)
            {
                if (game.gameTuts.Count(t => t.Row == tut.Row && t.Symbol == symbol) == 2 &&
                    game.gameTuts.Any(t => t.Row == tut.Row && t.Symbol == ' '))
                {
                    return game.gameTuts.FirstOrDefault(t => t.Row == tut.Row && t.Symbol == ' ');
                }

                if (game.gameTuts.Count(t => t.Column == tut.Column && t.Symbol == symbol) == 2 &&
                    game.gameTuts.Any(t => t.Column == tut.Column && t.Symbol == ' '))
                {
                    return game.gameTuts.FirstOrDefault(t => t.Column == tut.Column && t.Symbol == ' ');
                }
            }
            return null;
        }

        private Tut FindStrategicMove(Game game)
        {
            foreach (var tut in game.gameTuts)
            {
                if (game.gameTuts.Any(t => t.Row == tut.Row && t.Symbol == this.Mark) &&
                    !game.gameTuts.Any(t => t.Row == tut.Row && t.Symbol == game.Player1.Mark))
                {
                    var possibleMove = game.gameTuts.FirstOrDefault(t => t.Row == tut.Row && t.Symbol == ' ');
                    if (possibleMove != null) return possibleMove;
                }

                if (game.gameTuts.Any(t => t.Column == tut.Column && t.Symbol == this.Mark) &&
                    !game.gameTuts.Any(t => t.Column == tut.Column && t.Symbol == game.Player1.Mark))
                {
                    var possibleMove = game.gameTuts.FirstOrDefault(t => t.Column == tut.Column && t.Symbol == ' ');
                    if (possibleMove != null) return possibleMove;
                }
            }
            return null;
        }

    }
}