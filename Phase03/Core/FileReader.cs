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

        public static bool TryReadFile(string documentPath, out string content)
        {
            try
            {
                content = File.ReadAllText(documentPath);
                return true;
            }
            catch (IOException)
            {
                content = null;
                return false;
            }
        }
    }
}