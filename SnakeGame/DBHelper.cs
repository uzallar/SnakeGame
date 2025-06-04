using System;
using System.Collections.Generic;
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
            var mainDbName = "Gamers";
            var adminConnStr = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=postgres";

            try
            {
                // Шаг 1: Подключаемся к postgres и создаем БД, если её нет
                using var adminConn = new NpgsqlConnection(adminConnStr);
                adminConn.Open();

                using (var checkCmd = adminConn.CreateCommand())
                {
                    checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @dbname";
                    checkCmd.Parameters.AddWithValue("dbname", mainDbName);
                    var exists = checkCmd.ExecuteScalar();

                    if (exists == null)
                    {
                        using var createCmd = adminConn.CreateCommand();
                        createCmd.CommandText = $"CREATE DATABASE \"{mainDbName}\"";
                        createCmd.ExecuteNonQuery();
                    }
                }

                // Шаг 2: Подключаемся к созданной БД
                var connStr = $"Host=localhost;Port=5432;Username=postgres;Password=1234;Database={mainDbName}";
                _conn = new NpgsqlConnection(connStr);
                _conn.Open();

                // Шаг 3: Создаём таблицу users, если её нет
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                _conn = null;
                MessageBox.Show($"Ошибка подключения или инициализации БД: {ex.Message}");
            }
        }

        private static void InitializeDatabase()
        {
            if (_conn == null) return;

            try
            {
                using var cmd = _conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id SERIAL PRIMARY KEY,
                        username TEXT NOT NULL UNIQUE,
                        password_hash TEXT NOT NULL,
                        max_score INTEGER
                    );
                ";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания таблицы: {ex.Message}");
            }
        }

        public static User? CreateUser(string username, string password)
        {
            if (_conn == null) return null;

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
                cmd.CommandText = "SELECT id, username, password_hash, max_score FROM users WHERE username = @username";
                cmd.Parameters.AddWithValue("username", username);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = reader.GetString(2),
                        MaxScore = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
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

        public static List<User> GetTopPlayers(int count)
        {
            var users = new List<User>();

            if (_conn == null) return users;

            try
            {
                using var cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT id, username, max_score FROM users WHERE max_score IS NOT NULL ORDER BY max_score DESC LIMIT @count";
                cmd.Parameters.AddWithValue("count", count);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        MaxScore = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы лидеров: {ex.Message}");
            }

            return users;
        }
    }
}
