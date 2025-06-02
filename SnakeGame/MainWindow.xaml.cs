using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SnakeGameViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new SnakeGameViewModel();
            DataContext = viewModel;

            CompositionTarget.Rendering += RenderGame;
        }

        private void RenderGame(object sender, System.EventArgs e)
        {
            GameCanvas.Children.Clear();

            foreach (var part in viewModel.SnakeParts)
            {
                var rect = new Rectangle
                {
                    Width = SnakeGameModel.SnakeSize,
                    Height = SnakeGameModel.SnakeSize,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(rect, part.X);
                Canvas.SetTop(rect, part.Y);
                GameCanvas.Children.Add(rect);
            }

            var foodRect = new Rectangle
            {
                Width = SnakeGameModel.SnakeSize,
                Height = SnakeGameModel.SnakeSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(foodRect, viewModel.FoodPosition.X);
            Canvas.SetTop(foodRect, viewModel.FoodPosition.Y);
            GameCanvas.Children.Add(foodRect);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this); // нужно, чтобы InputBindings работали
        }
    }
}