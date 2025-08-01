namespace SearchEngine.Core
{
    public static class FileReader
    {
        public static string[] ReadAllFileNames(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(folderPath);
        }
    }
}