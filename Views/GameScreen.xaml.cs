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
        private readonly int Rows = 6; // Number of attempts
        private readonly int Columns = 5; // Number of letters in the word
        private string TargetWord = "APPLE"; // Example word to guess
        private int CurrentRow = 0;
        private int CurrentColumn = 0;

        private Dictionary<string, Button> KeyboardButtons = new Dictionary<string, Button>();

        public GameScreen()
        {
            InitializeComponent();
            BuildLetterGrid();
            BuildKeyboard();
        }

        // Build the letter grid dynamically
        private void BuildLetterGrid()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    // Create the Label with dynamic font size adjustment
                    var label = new Label
                    {
                        Text = "",
                        FontSize = 18, // Initial font size
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.White,
                        LineBreakMode = LineBreakMode.NoWrap
                    };

                    // Create the Frame that holds the Label
                    var box = new Frame
                    {
                        BackgroundColor = Colors.Black,
                        BorderColor = Colors.Gray,
                        CornerRadius = 0, // To make it look square
                        WidthRequest = 50,
                        HeightRequest = 50,
                        Padding = 0, // Remove extra padding
                        Content = label
                    };

                    // Set the row and column in the grid
                    Grid.SetRow(box, row);
                    Grid.SetColumn(box, col);
                    LetterGrid.Children.Add(box);
                }
            }
        }
        private void BuildKeyboard()
        {
            // Rows of letters and the special buttons row
            string[] rows = { "QWERTYUIOP", "ASDFGHJKL" };

            // Create a stack layout for the keyboard
            var keyboardLayout = new VerticalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End
            };

            // Build the first two rows of letters
            for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
            {
                string row = rows[rowIndex];

                var rowLayout = new HorizontalStackLayout
                {
                    Spacing = 5,
                    HorizontalOptions = LayoutOptions.Center
                };

                foreach (char letter in row)
                {
                    string letterString = letter.ToString();

                    var keyButton = new Button
                    {
                        Text = letterString,
                        FontSize = 18,
                        BackgroundColor = Colors.Gray,
                        TextColor = Colors.White,
                        CornerRadius = 5,
                        WidthRequest = 40,
                        HeightRequest = 50,
                        Margin = new Thickness(2),
                        Command = new Command(() => OnKeyPressed(letterString))
                    };

                    KeyboardButtons[letterString] = keyButton;
                    rowLayout.Children.Add(keyButton);
                }

                keyboardLayout.Children.Add(rowLayout);
            }

            // Add the last row with "Enter", letters, and "Delete"
            var lastRowLayout = new HorizontalStackLayout
            {
                Spacing = 5,
                HorizontalOptions = LayoutOptions.Center
            };

            // Add the "Enter" button
            var enterButton = new Button
            {
                Text = "Enter",
                FontSize = 16,
                BackgroundColor = Colors.Gray,
                TextColor = Colors.White,
                CornerRadius = 5,
                WidthRequest = 80, // Adjusted width for better fit
                HeightRequest = 50,
                Margin = new Thickness(2),
                IsEnabled = false, // Initially disabled
                Command = new Command(OnEnterPressed)
            };
            KeyboardButtons["Enter"] = enterButton;
            lastRowLayout.Children.Add(enterButton);

            // Add the letter keys for the last row
            string lastRowLetters = "ZXCVBNM";
            foreach (char letter in lastRowLetters)
            {
                string letterString = letter.ToString();

                var keyButton = new Button
                {
                    Text = letterString,
                    FontSize = 18,
                    BackgroundColor = Colors.Gray,
                    TextColor = Colors.White,
                    CornerRadius = 5,
                    WidthRequest = 40,
                    HeightRequest = 50,
                    Margin = new Thickness(2),
                    Command = new Command(() => OnKeyPressed(letterString))
                };

                KeyboardButtons[letterString] = keyButton;
                lastRowLayout.Children.Add(keyButton);
            }

            // Add the "Delete" button
            var deleteButton = new Button
            {
                Text = "Delete",
                FontSize = 16,
                BackgroundColor = Colors.Gray,
                TextColor = Colors.White,
                CornerRadius = 5,
                WidthRequest = 80, // Adjusted width for better fit
                HeightRequest = 50,
                Margin = new Thickness(2),
                IsEnabled = false, // Initially disabled
                Command = new Command(OnDeletePressed)
            };
            KeyboardButtons["Delete"] = deleteButton;
            lastRowLayout.Children.Add(deleteButton);

            keyboardLayout.Children.Add(lastRowLayout);

            // Add the keyboard layout to the main content
            KeyboardGrid.Children.Clear(); // Clear any existing grid children
            KeyboardGrid.Add(keyboardLayout);
        }
        private void OnEnterPressed()
        {
            // Process the current word
            string currentWord = string.Concat(
                LetterGrid.Children
                    .OfType<Label>()
                    .Where(lbl => Grid.GetRow(lbl) == CurrentRow)
                    .Select(lbl => lbl.Text)
            );

            if (currentWord.Length != Columns)
            {
                DisplayAlert("Error", "Complete the word before submitting.", "OK");
                return;
            }

            // Compare the word or perform your game logic
            CheckWord();
            CurrentRow++;
            CurrentColumn = 0;

            // Disable the "Enter" button after submission
            KeyboardButtons["Enter"].IsEnabled = false;
        }

        private void OnDeletePressed()
        {
            if (CurrentColumn > 0)
            {
                // Move to the previous box
                CurrentColumn--;

                // Get the target frame and its content (Label)
                var targetFrame = LetterGrid.Children
                    .OfType<Frame>()
                    .FirstOrDefault(frame => Grid.GetRow(frame) == CurrentRow && Grid.GetColumn(frame) == CurrentColumn);

                if (targetFrame != null && targetFrame.Content is Label targetLabel)
                {
                    // Clear the text in the label
                    targetLabel.Text = string.Empty;

                    // If no more input to delete, reset the "Delete" button color
                    if (CurrentColumn == 0)
                    {
                        KeyboardButtons["Delete"].IsEnabled = false;
                    }

                }
            }

            // Always reset the "Enter" button's color and disable it if the word is incomplete
            if (CurrentColumn < Columns)
            {
                KeyboardButtons["Enter"].IsEnabled = false;
            }
        }

        // Handle keyboard button presses
        private void OnKeyPressed(string letter)
        {
            if (CurrentRow >= Rows || CurrentColumn >= Columns) return;

            // Get the target frame for the current cell
            var targetFrame = LetterGrid.Children
                .OfType<Frame>()
                .FirstOrDefault(frame => Grid.GetRow(frame) == CurrentRow && Grid.GetColumn(frame) == CurrentColumn);

            if (targetFrame != null && targetFrame.Content is Label targetLabel)
            {
                // Update the text in the label
                targetLabel.Text = letter.ToUpper();
                CurrentColumn++;

                // Enable "Enter" if the word is complete
                if (CurrentColumn == Columns)
                {
                    KeyboardButtons["Enter"].IsEnabled = true;
                    KeyboardButtons["Enter"].BackgroundColor = Colors.Green;
                }

                // Enable and change the "Delete" button color to red if there's input to delete
                if (CurrentColumn > 0)
                {
                    KeyboardButtons["Delete"].BackgroundColor = Colors.Red;
                    KeyboardButtons["Delete"].IsEnabled = true;
                }
            }
        }

        private void CheckWord()
        {
            // Get the guessed word from the current row
            string guessedWord = string.Concat(
                LetterGrid.Children
                    .OfType<Label>()
                    .Where(lbl => Grid.GetRow(lbl) == CurrentRow)
                    .Select(lbl => lbl.Text)
            );

            // Compare to target word and disable keys as necessary
            for (int i = 0; i < guessedWord.Length; i++)
            {
                string guessedLetter = guessedWord[i].ToString();
                if (!TargetWord.Contains(guessedLetter))
                {
                    if (KeyboardButtons.TryGetValue(guessedLetter, out Button button))
                    {
                        button.IsEnabled = false;
                    }
                }
            }
        }
    }
}