using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace WordleApp.ViewModels
{
    public class StartScreenViewModel : BindableObject
    {
        private string _playerName;
        private bool _isNameEntered;

        public StartScreenViewModel()
        {
            StartGameCommand = new Command(StartGame, CanStartGame);
            ViewHistoryCommand = new Command(ViewHistory);
            OpenSettingsCommand = new Command(OpenSettings);
        }

        // Player Name Property
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
                IsNameEntered = !string.IsNullOrWhiteSpace(_playerName);
            }
        }

        // Boolean to Enable/Disable Start Button
        public bool IsNameEntered
        {
            get => _isNameEntered;
            set
            {
                _isNameEntered = value;
                OnPropertyChanged();
                ((Command)StartGameCommand).ChangeCanExecute();
            }
        }

        // Commands
        public ICommand StartGameCommand { get; }
        public ICommand ViewHistoryCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        // Command Methods
        private void StartGame()
        {
            // Navigate to the game page
            Application.Current.MainPage.DisplayAlert("Navigation", "Start Game clicked!", "OK");
        }

        private void ViewHistory()
        {
            // Navigate to the history page
            Application.Current.MainPage.DisplayAlert("Navigation", "View History clicked!", "OK");
        }

        private void OpenSettings()
        {
            // Navigate to the settings page
            Application.Current.MainPage.DisplayAlert("Navigation", "Settings clicked!", "OK");
        }

        private bool CanStartGame()
        {
            return IsNameEntered;
        }
    }
}