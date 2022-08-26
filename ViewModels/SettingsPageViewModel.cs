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
        SettingsPage parent;
        Pack pack { get; set; }
        Manifest manifest { get => pack.Manifest; set => pack.Manifest = value; }
        public string Title { get => manifest.Title; set => manifest.Title = value; }
        private string memory;
        public string Memory { get => memory; set => memory = value; }
        public string JVMArgs { get => manifest.JVMArgs; set => manifest.JVMArgs = value; }
        private int pickerIndex = -1;
        public int PickerIndex { get => pickerIndex; set => pickerIndex = value; }
        public ObservableCollection<string> Versions { get; private set; }

        public SettingsPageViewModel(SettingsPage page, Pack p)
        {
            parent = page;
            pack = p;
            memory = manifest.Memory.ToString();
            Versions = new ObservableCollection<string>(FileManager.GetVersions());
            OnPropertyChanged(nameof(pack));
            OnPropertyChanged(nameof(Versions));
            if (Versions.Contains(manifest.GameVersion)) PickerIndex = Versions.IndexOf(manifest.GameVersion);
        }

        public ICommand BackCommand => new Command(SaveAndReturn);
        public ICommand OpenFolderCommand => new Command(OpenPackFolder);
        public ICommand DeletePackCommand => new Command(DeletePack);

        private void SaveAndReturn()
        {
            int mem = 2;
            if (!Int32.TryParse(memory, out mem))
            {
                parent.ShowAlert("Error", "Invalid memory size...", "OK");
                return;
            }
            if (PickerIndex < 0)
            {
                parent.ShowAlert("Error", "Please select a game version...", "OK");
                return;
            }
            manifest.GameVersion = Versions[PickerIndex];
            manifest.Memory = mem;
            MainPageViewModel.UpdateCurrentPack(pack);
            FileManager.SaveManifest(pack);
            MainPage.instance.NavigateBack();
        }

        private void OpenPackFolder()
        {
            if (pack.Title == "")
            {
                parent.ShowAlert("Error", "Please set a pack title...", "OK");
                return;
            }
            if (pack.Path == "")
            {
                pack.Path = $"{Environment.CurrentDirectory}\\data\\{pack.Title}";
                FileManager.InitializePackFolder(pack);
            }
            Process.Start("explorer.exe", pack.Path);
        }

        private void DeletePack()
        {
            parent.ConfirmDelete();
        }

        public void ActuallyDeletePack()
        {
            FileManager.RemovePack(pack);
            MainPageViewModel.RemoveCurrentPack();
            MainPage.instance.NavigateBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
