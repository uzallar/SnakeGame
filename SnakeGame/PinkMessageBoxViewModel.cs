using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SnakeGame
{
    public class PinkMessageBoxViewModel : INotifyPropertyChanged
    {
        private string _message;
        private bool _isConfirmation;
        private MessageBoxResult _result = MessageBoxResult.None;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsConfirmation
        {
            get => _isConfirmation;
            set
            {
                _isConfirmation = value;
                OnPropertyChanged(nameof(IsConfirmation));
                OnPropertyChanged(nameof(ShowYesNoButtons));
                OnPropertyChanged(nameof(ShowOkButton));
            }
        }

        public bool ShowYesNoButtons => IsConfirmation;
        public bool ShowOkButton => !IsConfirmation;

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand OkCommand { get; }

        public Action CloseAction { get; set; }
        public MessageBoxResult Result => _result;

        public PinkMessageBoxViewModel(string message, bool isConfirmation)
        {
            Message = message;
            IsConfirmation = isConfirmation;

            YesCommand = new RelayCommand(_ => SetResult(MessageBoxResult.Yes));
            NoCommand = new RelayCommand(_ => SetResult(MessageBoxResult.No));
            OkCommand = new RelayCommand(_ => SetResult(MessageBoxResult.OK));
        }

        private void SetResult(MessageBoxResult result)
        {
            _result = result;
            CloseAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}