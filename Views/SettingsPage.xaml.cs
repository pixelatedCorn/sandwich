using Sandwich.Models;
using Sandwich.ViewModels;

namespace Sandwich.Views
{
    public partial class SettingsPage : ContentPage
    {
        internal SettingsPage(Pack pack, MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = new SettingsPageViewModel(this, pack, vm);
        }

        public async Task ShowAlert(string title, string msg, string option)
        {
            await DisplayAlert(title, msg, option);
        }

        public async Task ConfirmDelete()
        {
            if (await DisplayAlert("Confirmation", $"Are you sure you want to delete the pack?", "Yes", "No"))
            {
                ((SettingsPageViewModel)BindingContext).ActuallyDeletePack();
            }
            
        }
    }
}