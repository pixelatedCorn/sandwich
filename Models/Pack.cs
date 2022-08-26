using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwich.Models
{
    internal class Pack
    {
        public Manifest Manifest;
        public string Title { get => Manifest.Title; }
        public string Path;
        public string PackImage { get => Path + "\\thumbnail.png";}

        public Pack(Manifest manifest, string path)
        {
            Manifest = manifest;
            Path = path;
        }
    }

    internal class Manifest
    {
        public int Version = 1;
        public string Title = "";
        public string GameVersion = "";
        public int Memory = 2;
        public string JVMArgs = "";
    }
}