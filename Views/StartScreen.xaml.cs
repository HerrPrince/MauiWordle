namespace WordleApp.Views
{
    public partial class StartScreen : ContentPage
    {
        public StartScreen()
        {
            InitializeComponent();
            BindingContext = new ViewModels.StartScreenViewModel();
        }
    }
}