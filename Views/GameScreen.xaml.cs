using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace WordleApp.Views
{
    public partial class GameScreen : ContentPage
    {
        private const string WordsUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private const string WordsFileName = "words.txt";
        private string _targetWord;
        private string[] _wordList;

        public GameScreen()
        {
            InitializeComponent();

            LoadWordListAsync();
        }

        private async void LoadWordListAsync()
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, WordsFileName);

            if (!File.Exists(filePath))
            {
                try
                {
                    using var httpClient = new HttpClient();
                    string wordData = await httpClient.GetStringAsync(WordsUrl);
                    await File.WriteAllTextAsync(filePath, wordData);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to download word list: {ex.Message}", "OK");
                    return;
                }
            }

            _wordList = await File.ReadAllLinesAsync(filePath);

            Random random = new Random();
            _targetWord = _wordList[random.Next(_wordList.Length)].Trim().ToUpper();

            DebugWordLabel.Text = $"Debug: Target Word = {_targetWord}";
        }

        private void OnLetterChanged(object sender, TextChangedEventArgs e)
        {
            var currentBox = sender as Entry;

            // Handle backspace
            if (string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(e.OldTextValue))
            {
                if (currentBox == Letter5) Letter4.Focus();
                else if (currentBox == Letter4) Letter3.Focus();
                else if (currentBox == Letter3) Letter2.Focus();
                else if (currentBox == Letter2) Letter1.Focus();
                return;
            }

            // Automatically move to the next box after valid input
            if (e.NewTextValue?.Length == 1)
            {
                if (currentBox == Letter1) Letter2.Focus();
                else if (currentBox == Letter2) Letter3.Focus();
                else if (currentBox == Letter3) Letter4.Focus();
                else if (currentBox == Letter4) Letter5.Focus();
                else if (currentBox == Letter5) Letter5.Unfocus(); // Last letter
            }
        }

        private void OnSubmitGuess(object sender, EventArgs e)
        {
            string guess = $"{Letter1.Text}{Letter2.Text}{Letter3.Text}{Letter4.Text}{Letter5.Text}".ToUpper();

            if (guess.Length != 5)
            {
                DisplayAlert("Invalid Guess", "Please enter all 5 letters.", "OK");
                return;
            }

            // Example feedback logic
            FeedbackArea.Children.Add(new Label
            {
                Text = $"Your guess: {guess}",
                FontSize = 18
            });

            // Clear the boxes for the next guess
            Letter1.Text = Letter2.Text = Letter3.Text = Letter4.Text = Letter5.Text = string.Empty;
            Letter1.Focus();
        }



        private Entry GetBoxByIndex(int index)
        {
            return index switch
            {
                0 => Letter1,
                1 => Letter2,
                2 => Letter3,
                3 => Letter4,
                4 => Letter5,
                _ => null
            };
        }
    }
}