using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TicTacToe_Game.Models
{
    public class Player
    {
        protected int id;
        protected string type;
        protected string alias;

        public int Id { get { return id; } }
        public string Type { get { return type; } }
        public string Alias { get { return alias; } }
        public char Mark { set; get; }
        public int Score { set; get; }
        public int Draw { set; get; }

        public Player(int id, string type, string alias, char mark)
        {
            this.id = id;
            this.type = type;
            this.alias = alias;
            Mark = mark;
            Score = 0;
            Draw = 0;
        }
        public Player(int id, string type, string alias, char mark, int score, int draw)
        {
            this.id = id;
            this.type = type;
            this.alias = alias;
            Mark = mark;
            Score = score; 
            Draw = draw;
        }
    }

}