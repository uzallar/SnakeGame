using System;
using System.Collections.Generic;
using System.Windows;

namespace SnakeGame
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow()
        {
            InitializeComponent();

            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            try
            {
                var topUsers = DBHelper.GetTopPlayers(10);

                var rankedUsers = new List<dynamic>();
                int rank = 1;
                foreach (var user in topUsers)
                {
                    rankedUsers.Add(new
                    {
                        Rank = rank++,
                        Username = user.Username,
                        Score = user.MaxScore
                    });
                }

                LeaderboardListView.ItemsSource = rankedUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки таблицы лидеров: " + ex.Message);
            }
        }
    }
}
