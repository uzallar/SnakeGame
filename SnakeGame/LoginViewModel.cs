using System.ComponentModel;
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
                MessageBox.Show("Введите имя пользователя и пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DBHelper.ValidateUser(Username, Password))
            {
                var user = DBHelper.GetUserByUsername(Username);
                MessageBox.Show($"Добро пожаловать, {user.Username}!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                var difficultyWindow = new DifficultySelectionWindow();
                if (difficultyWindow.ShowDialog() == true)
                {
                    var selectedSpeed = (SnakeSpeed)Application.Current.Properties["SelectedSpeed"];
                    var mainWindow = new MainWindow(user, selectedSpeed);
                    mainWindow.Show();
                    Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                }
            }
            else
            {
                MessageBox.Show("Неверные имя пользователя или пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Введите имя пользователя и пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingUser = DBHelper.GetUserByUsername(Username);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = DBHelper.CreateUser(Username, Password);
            if (newUser != null)
            {
                MessageBox.Show($"Пользователь {newUser.Username} успешно зарегистрирован",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}