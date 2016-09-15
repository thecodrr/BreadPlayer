using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.UI.Core;

namespace Macalifa.Common
{
    class DirectoryWalker
    {   
        public async static Task<IEnumerable<string>> GetFiles(string dirPath)
        {
            string[] files = { };
            
                files = Directory.GetFiles(dirPath, "*.mp3", SearchOption.AllDirectories);
            GC.Collect();

            return files;
        }
    }
}
