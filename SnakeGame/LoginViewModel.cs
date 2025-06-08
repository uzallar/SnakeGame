using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SnakeGame
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
            RegisterCommand = new RelayCommand(_ => Register());
        }

        private void Login()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                PinkMessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            if (DBHelper.ValidateUser(Username, Password))
            {
                var user = DBHelper.GetUserByUsername(Username);
                PinkMessageBox.Show($"Добро пожаловать, {user.Username}!");

                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
            }
            else
            {
                PinkMessageBox.Show("Неверные имя пользователя или пароль");
            }
        }

        private void Register()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                PinkMessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            var existingUser = DBHelper.GetUserByUsername(Username);
            if (existingUser != null)
            {
                PinkMessageBox.Show("Пользователь с таким именем уже существует");
                return;
            }

            var newUser = DBHelper.CreateUser(Username, Password);
            if (newUser != null)
            {
                PinkMessageBox.Show($"Пользователь {newUser.Username} успешно зарегистрирован");
            }
            else
            {
                PinkMessageBox.Show("Ошибка при регистрации пользователя");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
