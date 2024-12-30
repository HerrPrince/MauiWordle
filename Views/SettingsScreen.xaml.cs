using Microsoft.Maui.Controls;

namespace WordleApp.Views;

public partial class SettingsScreen : ContentPage
{
    public SettingsScreen()
    {
        InitializeComponent();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}