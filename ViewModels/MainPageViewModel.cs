using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sandwich.Manager;
using Sandwich.Models;
using Sandwich.Views;

namespace Sandwich.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Pack> Packs { get; private set; }
        private static int PackIndex = 0;
        private MainPage Parent;
        public MainPageViewModel(MainPage parent, IEnumerable<Pack> packs)
        {
            Packs = new ObservableCollection<Pack>(packs);
            this.Parent = parent;
            OnPropertyChanged(nameof(Packs));
        }

        public ICommand SidebarCommand => new Command(ToggleSidebar);
        public ICommand SettingsCommand => new Command<Pack>(OpenSettingsPage);
        public ICommand LaunchCommand => new Command<Pack>(LaunchPack);
        public ICommand NewPackCommand => new Command(AddPack);

        private bool SidebarActive = false;
        async void ToggleSidebar()
        {
            SidebarActive = !SidebarActive;
            Parent.SetSidebarButtonText(SidebarActive ? "<" : ">");
            await Parent.MoveSidebar(SidebarActive ? 0 : -224);
        }

        async void OpenSettingsPage(Pack pack)
        {
            PackIndex = Packs.IndexOf(pack);
            await Parent.NavigateTo(new SettingsPage(pack, this));
        }

        public void UpdateCurrentPack(Pack pack)
        {
            Packs[PackIndex] = pack;
        }

        public void RemoveCurrentPack()
        {
            Packs.RemoveAt(PackIndex);
        }

        void LaunchPack(Pack pack)
        {
            FileManager.InjectProfile(pack);
            Process.Start($"{DataStore.GameDirectory}\\minecraft.exe");
        }

        void AddPack()
        {
            Pack p = new Pack(new Manifest(), "");
            Packs.Add(p);
            OnPropertyChanged(nameof(Packs));
            OpenSettingsPage(p);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
