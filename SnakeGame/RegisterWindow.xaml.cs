using System.Windows;

namespace SnakeGame
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            var existingUser = DBHelper.GetUserByUsername(username);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким именем уже существует");
                return;
            }

            var user = DBHelper.CreateUser(username, password);
            if (user != null)
            {
                MessageBox.Show($"Пользователь {user.Username} успешно зарегистрирован");
                new LoginWindow().Show();
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка регистрации пользователя");
            }
        }

        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
