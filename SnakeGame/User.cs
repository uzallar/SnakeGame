namespace SnakeGame
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Хэш пароля

        public int MaxScore { get; set; } // Максимальный рекорд игрока
    }
}
