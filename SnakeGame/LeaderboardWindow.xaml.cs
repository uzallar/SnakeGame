// LeaderboardWindow.xaml.cs
using SnakeGame.ViewModels;
using System.Windows;

namespace SnakeGame
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();
            DataContext = new LeaderboardViewModel();
        }
    }
}
