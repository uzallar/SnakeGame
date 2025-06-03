using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Npgsql;

namespace SnakeGame
{
    internal static class DBHelper
    {
        private static NpgsqlConnection? _conn;

        static DBHelper()
        {
            var connStr = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=Gamers";
            try
            {
                _conn = new NpgsqlConnection(connStr);
                _conn.Open();
            }
            catch (Exception ex)
            {
                _conn = null;
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}");
            }
        }

        public static User? CreateUser(string username, string password)
        {
            if (_conn == null) return null;

            // Проверка на существование пользователя
            if (GetUserByUsername(username) != null)
                return null;

            try
            {
                string hash = HashPassword(password);

                using var cmd = _conn.CreateCommand();
                cmd.CommandText = "INSERT INTO users (username, password_hash) VALUES (@username, @hash) RETURNING id";
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("hash", hash);

                var id = (int)cmd.ExecuteScalar();
                return new User { Id = id, Username = username, PasswordHash = hash };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания пользователя: {ex.Message}");
                return null;
            }
        }

        public static User? GetUserByUsername(string username)
        {
            if (_conn == null) return null;

            try
            {
                using var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT id, username, password_hash FROM users WHERE username = @username";
                cmd.Parameters.AddWithValue("username", username);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = reader.GetString(2)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения пользователя: {ex.Message}");
                return null;
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null) return false;

            string inputHash = HashPassword(password);
            return user.PasswordHash == inputHash;
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static void UpdateMaxScore(int userId, int newScore)
        {
            if (_conn == null) return;

            try
            {
                using var checkCmd = _conn.CreateCommand();
                checkCmd.CommandText = "SELECT max_score FROM users WHERE id = @id";
                checkCmd.Parameters.AddWithValue("@id", userId);

                var currentMaxScoreObj = checkCmd.ExecuteScalar();
                int currentMaxScore = currentMaxScoreObj != DBNull.Value ? Convert.ToInt32(currentMaxScoreObj) : 0;

                if (newScore > currentMaxScore)
                {
                    using var updateCmd = _conn.CreateCommand();
                    updateCmd.CommandText = "UPDATE users SET max_score = @score WHERE id = @id";
                    updateCmd.Parameters.AddWithValue("@score", newScore);
                    updateCmd.Parameters.AddWithValue("@id", userId);
                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления счёта: {ex.Message}");
            }
        }
    }
}
