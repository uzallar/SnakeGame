// LeaderboardViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SnakeGame;

namespace SnakeGame.ViewModels
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
    }

    public class LeaderboardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LeaderboardEntry> TopPlayers { get; set; }

        public LeaderboardViewModel()
        {
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            TopPlayers = new ObservableCollection<LeaderboardEntry>();
            var topUsers = DBHelper.GetTopPlayers(10);
            int rank = 1;

            foreach (var user in topUsers)
            {
                TopPlayers.Add(new LeaderboardEntry
                {
                    Rank = rank++,
                    Username = user.Username,
                    Score = user.MaxScore
                });
            }
            OnPropertyChanged(nameof(TopPlayers));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
