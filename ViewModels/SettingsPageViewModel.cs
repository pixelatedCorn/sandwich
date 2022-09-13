using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sandwich.Manager;
using Sandwich.Models;
using Sandwich.Views;

namespace Sandwich.ViewModels
{
    internal class SettingsPageViewModel : INotifyPropertyChanged
    {
        SettingsPage Parent;
        MainPageViewModel MainPageVM;
        Pack Pack { get; set; }
        Manifest Manifest { get => Pack.Manifest; set => Pack.Manifest = value; }
        public string Title { get => Manifest.Title; set => Manifest.Title = value; }
        private string memory = "2";
        public string Memory { get => memory; set => memory = value; }
        public string JVMArgs { get => Manifest.JVMArgs; set => Manifest.JVMArgs = value; }
        private int pickerIndex = -1;
        public int PickerIndex { get => pickerIndex; set => pickerIndex = value; }
        public ObservableCollection<string> Versions { get; private set; }

        public SettingsPageViewModel(SettingsPage page, Pack p, MainPageViewModel mainPageVM)
        {
            Parent = page;
            Pack = p;
            Memory = Manifest.Memory.ToString();
            Versions = new ObservableCollection<string>(FileManager.GetVersions());
            OnPropertyChanged(nameof(Pack));
            OnPropertyChanged(nameof(Versions));
            if (Versions.Contains(Manifest.GameVersion)) PickerIndex = Versions.IndexOf(Manifest.GameVersion);
            MainPageVM = mainPageVM;
        }

        public ICommand BackCommand => new Command(SaveAndReturn);
        public ICommand OpenFolderCommand => new Command(OpenPackFolder);
        public ICommand DeletePackCommand => new Command(DeletePack);

        private void SaveAndReturn()
        {
            Save();
            _ = MainPage.instance.NavigateBack();
        }

        private void Save()
        {
            int mem = 2;
            if (!Int32.TryParse(Memory, out mem) || mem < 2)
            {

                _ = Parent.ShowAlert("Error", "Invalid memory size...", "OK");
                return;
            }
            if (PickerIndex < 0)
            {
                _ = Parent.ShowAlert("Error", "Please select a game version...", "OK");
                return;
            }
            if (Title == "")
            {
                _ = Parent.ShowAlert("Error", "Please enter a pack title...", "OK");
                return;
            }
            if (FileManager.GetPackNames().Contains(Title.ToLower()))
            {
                _ = Parent.ShowAlert("Error", "Pack name unavailable...", "OK");
                return;
            }
            Manifest.GameVersion = Versions[PickerIndex];
            Manifest.Memory = mem;
            MainPageVM.UpdateCurrentPack(Pack);
            FileManager.SaveManifest(Pack);
        }

        private void OpenPackFolder()
        {
            if (Pack.Title == "")
            {
                _ = Parent.ShowAlert("Error", "Please set a pack title...", "OK");
                return;
            }
            if (Pack.Path == "")
            {
                Pack.Path = $"{Environment.CurrentDirectory}\\data\\{Pack.Title}";
                FileManager.InitializePackFolder(Pack);
            }
            Save();
            Process.Start("explorer.exe", Pack.Path);
        }

        private void DeletePack()
        {
            _ = Parent.ConfirmDelete();
        }

        public void ActuallyDeletePack()
        {
            FileManager.RemovePack(Pack);
            MainPageVM.RemoveCurrentPack();
            _ = MainPage.instance.NavigateBack();
        }
#pragma warning restore CS4014

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
