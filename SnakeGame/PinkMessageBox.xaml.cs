using System.Windows;

namespace SnakeGame
{
    public partial class PinkMessageBox : Window
    {
        private MessageBoxResult _result = MessageBoxResult.None;

        public PinkMessageBox(string message, bool isConfirmation)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;

            if (isConfirmation)
            {
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
                OkButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                OkButton.Visibility = Visibility.Visible;
                YesButton.Visibility = Visibility.Collapsed;
                NoButton.Visibility = Visibility.Collapsed;
            }
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
                return box._result;
            }

            Show(message);
            return MessageBoxResult.OK;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            Close();
        }
    }
}