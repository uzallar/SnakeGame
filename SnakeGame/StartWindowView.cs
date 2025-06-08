using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SnakeGame
{
    public class StartWindowView: INotifyPropertyChanged
    {
        public ICommand RegisterAndLoginCommand { get; }
        public Action OpenLoginWindowAction { get; set; }
        public Action CloseStartWindowAction { get; set; }


        public StartWindowView() {
            RegisterAndLoginCommand = new RelayCommand(_ =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Application.Current.Windows.OfType<StartWindow>().FirstOrDefault()?.Close(); ;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
