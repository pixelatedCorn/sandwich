using Sandwich.Models;
using Sandwich.ViewModels;
using Sandwich.Manager;
namespace Sandwich.Views
{
    public partial class MainPage : ContentPage
    {
        public static MainPage instance;
        public MainPage()
        {
            Singleton();
            InitializeComponent();
            FileManager.InitializeDataStore();
            BindingContext = new MainPageViewModel(FileManager.GetPacks());
        }
        
        private void Singleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("MainPage Singleton failed...");
            }
        }

        public async Task NavigateTo(Page page)
        {
            await Navigation.PushAsync(page);
        }

        public async Task NavigateBack()
        {
            await Navigation.PopAsync();
        }

        public async Task MoveSidebar(double x)
        {
            await Sidebar.TranslateTo(x, 0, 500, Easing.BounceOut);
        }

        public async Task ShowAlert(string title, string msg, string option)
        {
            await DisplayAlert(title, msg, option);
        }

        public void SetSidebarButtonText(string text)
        {
            SidebarButton.Text = text;
        }
    }
}