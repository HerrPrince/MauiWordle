﻿using System.Windows.Input;
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
                ((Command)ViewHistoryCommand).ChangeCanExecute();
            }
        }

        // Commands
        public ICommand StartGameCommand { get; }
        public ICommand ViewHistoryCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        // Command Methods
        private async void StartGame()
        {
            try
            {
                // Pass the PlayerName to the GameScreen
                var navigationParams = new Dictionary<string, object>
        {
            { "playerName", PlayerName }
        };

                await Shell.Current.GoToAsync("GameScreen", navigationParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        // Command logic for navigation
        private async void ViewHistory()
        {
            try
            {
                // Pass PlayerName as a navigation parameter
                var navigationParams = new Dictionary<string, object>
        {
            { "playerName", PlayerName }
        };

                await Shell.Current.GoToAsync("HistoryScreen", navigationParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation to HistoryScreen failed: {ex.Message}");
            }
        }

        private void OpenSettings()
        {
            // Navigate to the settings page
            Application.Current.MainPage.DisplayAlert("Navigation", "Settings clicked!", "OK");
        }

        private bool CanStartGame()
        {
            return !string.IsNullOrWhiteSpace(PlayerName);
        }
    }
}