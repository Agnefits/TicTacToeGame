using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToe_Game.Models
{
    public class Tut
    {
        private int row;
        private int column;

        public int Row { get { return row; } }
        public int Column { get { return column; } }
        public char Symbol { get; set; }

        public Tut(int row, int column, char symbol)
        {
            this.row = row;
            this.column = column;
            Symbol = symbol;
        }
    }
}