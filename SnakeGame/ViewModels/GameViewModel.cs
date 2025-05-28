using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using SnakeGame.Models;

namespace SnakeGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Point> SnakeBody { get; set; } = new();
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand RestartCommand { get; }

        public GameViewModel()
        {
            // TODO: Команды и логика привязки
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
