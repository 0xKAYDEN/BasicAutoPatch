using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicAutoPatch
{
    class Configrations
    {
        public static string
        PatchListFile = "/Patch.txt",
        HashListFile = "/Hash.txt",
        PatchPath = "Patch",
        ServerAddress = "http://127.0.0.1",
        GameExecutable = "",//add you exe path in here
        PatchHash = "";//leave it like this
        public static uint
            PatchNumber = 0,
            ServerVersion = 0,
            ClientVersion = 0;
    }
}
