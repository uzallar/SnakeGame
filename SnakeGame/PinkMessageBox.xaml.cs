using System.Windows;

namespace SnakeGame
{
    public partial class PinkMessageBox : Window
    {
        private readonly PinkMessageBoxViewModel _viewModel;

        public PinkMessageBox(string message, bool isConfirmation)
        {
            InitializeComponent();
            _viewModel = new PinkMessageBoxViewModel(message, isConfirmation)
            {
                CloseAction = () => Close()
            };
            DataContext = _viewModel;
        }

        public static void Show(string message)
        {
            var box = new PinkMessageBox(message, false);
            box.ShowDialog();
        }

        public static MessageBoxResult Show(string message, MessageBoxButton buttons)
        {
            if (buttons == MessageBoxButton.YesNo)
            {
                var box = new PinkMessageBox(message, true);
                box.ShowDialog();
                return box._viewModel.Result;
            }

            Show(message);
            return MessageBoxResult.OK;
        }
    }
}