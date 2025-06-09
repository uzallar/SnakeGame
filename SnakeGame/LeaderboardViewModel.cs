
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SnakeGame;
using static SnakeGame.ViewModels.LeaderboardViewModel;


namespace SnakeGame.ViewModels
{
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
    }

    public interface IDBHelper
    {
        IEnumerable<User> GetTopPlayers(int count);
    }

    public class DBHelperWrapper : IDBHelper
    {
        public IEnumerable<User> GetTopPlayers(int count)
        {
            return DBHelper.GetTopPlayers(count);
        }
    }

    public class FakeDBHelper : IDBHelper
    {
        public List<User> TestUsers { get; } = new List<User>();

        public IEnumerable<User> GetTopPlayers(int count)
        {
            return TestUsers
                .OrderByDescending(u => u.MaxScore)
                .Take(count);
        }
    }

    public class LeaderboardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LeaderboardEntry> TopPlayers { get; private set; }

        private readonly IDBHelper _dbHelper;

        public LeaderboardViewModel(IDBHelper dbHelper = null)
        {
            _dbHelper = dbHelper ?? new DBHelperWrapper();
            LoadLeaderboard();
        }

        public void LoadLeaderboard()
        {
            TopPlayers = new ObservableCollection<LeaderboardEntry>();
            var topUsers = _dbHelper.GetTopPlayers(10); 
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
