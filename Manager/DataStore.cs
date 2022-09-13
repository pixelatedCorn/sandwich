using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwich.Manager
{
    internal class DataStore
    {
        public static DataStore store { get; set; }
        private string gameDirectory = "";
        public static string GameDirectory { get { return store.gameDirectory; } set { store.gameDirectory = value; } }

        public DataStore()
        {
        }

        public static void Initialize()
        {
            if (store == null)
            {
                store = new DataStore();
            }
            else
            {
                throw new Exception("DataStore Singleton failed...");
            }
        }

        public static void Initialize(DataStore s)
        {
            if (store == null)
            {
                store = s;
            }
            else
            {
                throw new Exception("DataStore Singleton failed...");
            }
        }
    }
}
