using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToe_Game.Models
{
    public class Human : Player
    {
        public Human(int id, char mark, string name) : base(id, "human", name, mark) { }
        public Human(int id, char mark, string name, int score, int draw) : base(id, "human", name, mark, score, draw) { }
    }
}