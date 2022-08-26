using Sandwich.Views;

namespace Sandwich
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            if (window != null)
            {
                window.Title = "Sandwich";
            }

            return window;
        }
    }
}