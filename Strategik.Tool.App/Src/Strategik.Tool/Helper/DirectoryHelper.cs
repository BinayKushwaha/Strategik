using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategik.Tool.Helper
{
    public static class DirectoryHelper
    {
        public static void CreateDirectories(List<string> paths)
        {
            paths.ForEach(path =>
            {
                Directory.CreateDirectory(path);

            });
        }
    }
}
