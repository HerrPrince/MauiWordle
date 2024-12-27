namespace WordleApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new Views.StartScreen());
            MainPage = navigationPage;
        }
    }
}