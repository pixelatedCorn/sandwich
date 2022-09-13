using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sandwich.Models;
using Newtonsoft.Json;
using Sandwich.Views;
using System.Diagnostics;

namespace Sandwich.Manager
{
    internal static class FileManager
    {
        public static List<Pack> GetPacks()
        {
            if (!Directory.Exists("./data"))
            {
                Directory.CreateDirectory("./data");
                return new List<Pack>();
            }

            List<Pack> packs = new List<Pack>();
            string[] dirs = Directory.GetDirectories($"{Environment.CurrentDirectory}\\data");
            foreach(string dir in dirs)
            {
                try
                {
                    string data;
                    using (TextReader tr = new StreamReader($"{dir}/manifest.json"))
                    {
                        data = tr.ReadToEnd();
                    }
                    Manifest manifest = JsonConvert.DeserializeObject<Manifest>(data);
                    packs.Add(new Pack(manifest, dir));
                }
                catch { }
            }

            return packs;
        }

        public static List<string> GetPackNames()
        {
            if (!Directory.Exists("./data"))
            {
                Directory.CreateDirectory("./data");
                return new List<string>();
            }

            List<string> names = new List<string>();
            string[] dirs = Directory.GetDirectories($"{Environment.CurrentDirectory}\\data");
            foreach (string dir in dirs)
            {
                try
                {
                    string data;
                    using (TextReader tr = new StreamReader($"{dir}/manifest.json"))
                    {
                        data = tr.ReadToEnd();
                    }
                    Manifest manifest = JsonConvert.DeserializeObject<Manifest>(data);
                    string[] split = dir.Split('\\');
                    names.Add(split[split.Length-1].ToLower());
                    names.Add(manifest.Title.ToLower());
                }
                catch { }
            }

            return names;
        }

        public static void InitializeDataStore()
        {
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(docs + "\\sandwich")) Directory.CreateDirectory(docs + "\\sandwich");
            Environment.CurrentDirectory = docs + "\\sandwich";
            if (!File.Exists("./data.json")) FirstTimeSetup();
            else
            {
                string data;
                using (TextReader tr = new StreamReader("./data.json"))
                {
                    data = tr.ReadToEnd();
                }
                DataStore.Initialize(JsonConvert.DeserializeObject<DataStore>(data));
            }
        }

        public static void FirstTimeSetup()
        {
            DataStore.Initialize();
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists($"{roaming}/.minecraft"))
            {
                _ = MainPage.instance.ShowAlert("Error", "Could not auto detect game directory. Please manually set it in the options menu.", "OK");
                DataStore.GameDirectory = "";
            }
            else
            {
                DataStore.GameDirectory = $"{roaming}/.minecraft";
            }
            
            SaveDataStore();
        }

        public static void SaveDataStore()
        {
            string data = JsonConvert.SerializeObject(DataStore.store, Formatting.Indented);
            using (TextWriter tw = new StreamWriter("./data.json"))
            {
                tw.Write(data);
            }
        }

        public static List<string> GetVersions()
        {
            List<string> versions = new List<string>();
            if (!string.IsNullOrEmpty(DataStore.GameDirectory))
            {
                versions = Directory.GetDirectories($"{DataStore.GameDirectory}/versions").ToList();
            }
            for (int i = 0; i < versions.Count; i++)
            {
                string[] split = versions[i].Split('\\');
                versions[i] = split[split.Length - 1];
            }

            return versions;
        }

        public static void SaveManifest(Pack p)
        {
            if (p.Path == "")
            {
                p.Path = $"{Environment.CurrentDirectory}\\data\\{p.Title}";
                InitializePackFolder(p);
            }
            string data = JsonConvert.SerializeObject(p.Manifest, Formatting.Indented);
            using (TextWriter tw = new StreamWriter(p.Path + "\\manifest.json"))
            {
                tw.Write(data);
            }
        }

        public static void InitializePackFolder(Pack p)
        {
            Directory.CreateDirectory(p.Path);
            Directory.CreateDirectory(p.Path + "\\mods");
        }

        public static void RemovePack(Pack p)
        {
            Directory.Delete(p.Path, true);
        }

        public static void InjectProfile(Pack pack)
        {
            string data;
            using (TextReader tr = new StreamReader($"{DataStore.GameDirectory}\\launcher_profiles.json"))
            {
                data = tr.ReadToEnd();
            }
            LauncherProfiles profiles = JsonConvert.DeserializeObject<LauncherProfiles>(data);
            if (profiles.profiles.ContainsKey("sandwich"))
            {
                Profile profile = profiles.profiles["sandwich"];
                profile.lastUsed = DateTime.Now;
                profile.gameDir = pack.Path;
                profile.javaArgs = $"-Xmx{pack.Manifest.Memory}G {pack.Manifest.JVMArgs}";
                profile.name = pack.Title;
                profile.lastVersionId = pack.Manifest.GameVersion;
                profiles.profiles["sandwich"] = profile;
            }
            else
            {
                Profile profile = new Profile();
                profile.type = "custom";
                profile.name = pack.Title;
                profile.icon = "Furnace";
                profile.created = DateTime.Now;
                profile.lastUsed = DateTime.Now;
                profile.gameDir = pack.Path;
                profile.javaArgs = $"-Xmx{pack.Manifest.Memory}G {pack.Manifest.JVMArgs}";
                profile.lastVersionId = pack.Manifest.GameVersion;
                profiles.profiles.Add("sandwich", profile);
            }
            data = JsonConvert.SerializeObject(profiles, Formatting.Indented);
            using (TextWriter tw = new StreamWriter($"{DataStore.GameDirectory}\\launcher_profiles.json"))
            {
                tw.Write(data);
            }
        }
    }
}
