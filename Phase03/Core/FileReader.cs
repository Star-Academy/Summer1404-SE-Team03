using System.IO;

namespace Phase02.Core
{
    public static class FileReader
    {
        public static string[] ReadAllFileNames(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return new string[0];
            }

            return Directory.GetFiles(folderPath);
        }
    }
}
