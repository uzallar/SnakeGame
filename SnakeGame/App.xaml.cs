using System;
using System.Windows;
using System.Windows.Media;

namespace SnakeGame
{
    public partial class App : Application
    {
        private MediaPlayer backgroundMusicPlayer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            PlayBackgroundMusic();
        }

        private void PlayBackgroundMusic()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Backgroundmusic.mp3");

                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show("Музыкальный файл не найден: " + path);
                    return;
                }

                backgroundMusicPlayer = new MediaPlayer();
                backgroundMusicPlayer.Open(new Uri(path));
                backgroundMusicPlayer.Volume = 0.5;
                backgroundMusicPlayer.MediaEnded += (s, e) =>
                {
                    backgroundMusicPlayer.Position = TimeSpan.Zero;
                    backgroundMusicPlayer.Play();
                };
                backgroundMusicPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка воспроизведения музыки: " + ex.Message);
            }
        }
    }
}
