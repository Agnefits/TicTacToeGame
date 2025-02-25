using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace TicTacToe_Game.Models
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["TicTacToeDB"].ConnectionString;
        }

        public int AddPlayer(string alias, string type, char mark)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Players (Alias, Type, Mark) 
            OUTPUT INSERTED.Id 
            VALUES (@Alias, @Type, @Mark)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Alias", alias);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@Mark", mark);

                    conn.Open();
                    int playerId = (int)cmd.ExecuteScalar();
                    return playerId;
                }
            }
        }


        public Player GetPlayer(int playerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT p.Id, P.Alias, P.Type, P.Mark,
                   COALESCE(SUM(CASE WHEN (g.Player1Id = p.Id AND g.Status = 'Player1Won') 
                                     OR (g.Player2Id = p.Id AND g.Status = 'Player2Won') 
                                     THEN 1 ELSE 0 END), 0) AS Score,
                   COALESCE(SUM(CASE WHEN (g.Player1Id = p.Id OR g.Player2Id = p.Id) 
                                     AND g.Status = 'Draw' 
                                     THEN 1 ELSE 0 END), 0) AS Draws
            FROM Players p
            LEFT JOIN Games g ON g.Player1Id = p.Id OR g.Player2Id = p.Id
            WHERE p.Id = @Id
            GROUP BY p.Id, p.Mark, p.Alias, p.Type";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", playerId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        char mark = Convert.ToChar(reader["Mark"]);
                        string alias = reader["Alias"].ToString();
                        string type = reader["Type"].ToString();
                        int score = (int)reader["Score"];
                        int draws = (int)reader["Draws"];

                        if (type == "human")
                            return new Human(id, mark, alias, score, draws);
                        else
                            return new Computer(id, mark, score, draws);
                    }
                }
            }
            return null;
        }

        public int CreateGame(int player1Id, int player2Id, int currentPlayerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Games (Player1Id, Player2Id, CurrentPlayerId, Status) OUTPUT INSERTED.Id VALUES (@P1, @P2, @CP, 'Running')";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@P1", player1Id);
                cmd.Parameters.AddWithValue("@P2", player2Id);
                cmd.Parameters.AddWithValue("@CP", currentPlayerId);

                conn.Open();
                int gameId = (int)cmd.ExecuteScalar();

                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        InsertGameTut(gameId, row, col, ' ');
                    }
                }

                return gameId;
            }
        }

        public Game GetGame(int gameId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Games WHERE Id = @GameId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GameId", gameId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int player1Id = Convert.ToInt32(reader["Player1Id"]);
                    int player2Id = Convert.ToInt32(reader["Player2Id"]);
                    int currentPlayerId = Convert.ToInt32(reader["CurrentPlayerId"]);
                    GameStatus status = (GameStatus)Enum.Parse(typeof(GameStatus), reader["Status"].ToString());

                    Player player1 = GetPlayer(player1Id);
                    Player player2 = GetPlayer(player2Id);
                    Player currentPlayer = currentPlayerId == player1Id ? player1 : player2;

                    Game game = new Game(gameId, player1, player2, currentPlayer, status);

                    reader.Close(); // Close first reader before executing a new command

                    // Fetch game moves (GameTuts)
                    string tutQuery = "SELECT * FROM GameTuts WHERE GameId = @GameId";
                    SqlCommand tutCmd = new SqlCommand(tutQuery, conn);
                    tutCmd.Parameters.AddWithValue("@GameId", gameId);

                    SqlDataReader tutReader = tutCmd.ExecuteReader();
                    List<Tut> gameTuts = new List<Tut>();

                    while (tutReader.Read())
                    {
                        gameTuts.Add(new Tut
                        (
                            Convert.ToInt32(tutReader["RowIndex"]),
                            Convert.ToInt32(tutReader["ColumnIndex"]),
                            Convert.ToChar(tutReader["Symbol"])
                        ));
                    }

                    game.gameTuts = gameTuts;

                    return game;
                }
            }
            return null;
        }


        private void InsertGameTut(int gameId, int row, int col, char symbol)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO GameTuts (GameId, RowIndex, ColumnIndex, Symbol) VALUES (@GameId, @Row, @Col, @Symbol)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GameId", gameId);
                cmd.Parameters.AddWithValue("@Row", row);
                cmd.Parameters.AddWithValue("@Col", col);
                cmd.Parameters.AddWithValue("@Symbol", symbol);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateGameStatus(int gameId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Games SET Status = @Status WHERE Id = @GameId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@GameId", gameId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Tut> GetGameTuts(int gameId)
        {
            List<Tut> tuts = new List<Tut>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM GameTuts WHERE GameId = @GameId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GameId", gameId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tuts.Add(new Tut(
                            (int)reader["RowIndex"],
                            (int)reader["ColumnIndex"],
                            Convert.ToChar(reader["Symbol"])
                        ));
                    }
                }
            }
            return tuts;
        }

        public void UpdateTut(int gameId, int row, int col, char symbol)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE GameTuts SET Symbol = @Symbol WHERE GameId = @GameId AND RowIndex = @Row AND ColumnIndex = @Col";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Symbol", symbol);
                cmd.Parameters.AddWithValue("@GameId", gameId);
                cmd.Parameters.AddWithValue("@Row", row);
                cmd.Parameters.AddWithValue("@Col", col);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateCurrentPlayer(int gameId, int nextPlayerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Games SET CurrentPlayerId = @NextPlayer WHERE Id = @GameId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NextPlayer", nextPlayerId);
                cmd.Parameters.AddWithValue("@GameId", gameId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
