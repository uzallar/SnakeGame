using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SnakeGame
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            if (DBHelper.ValidateUser(username, password))
            {
                var user = DBHelper.GetUserByUsername(username);
                MessageBox.Show($"Добро пожаловать, {user.Username}!");
                var mainWin = new MainWindow(user);
                mainWin.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверные имя пользователя или пароль");
            }
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

            var newUser = DBHelper.CreateUser(username, password);
            if (newUser != null)
            {
                MessageBox.Show($"Пользователь {newUser.Username} успешно зарегистрирован");
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации пользователя");
            }
        }
    }
}

