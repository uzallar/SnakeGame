using System.Windows;
using System.Windows.Input;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        public MainWindow(User user = null, SnakeSpeed? selectedSpeed = null)
        {
            InitializeComponent();
            DataContext = new SnakeGameViewModel(user, selectedSpeed);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
        }
    }
}