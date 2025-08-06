namespace SearchEngine
{
    public static class AppConfig
    {
        public static string DataDirectory { get; }

        static AppConfig()
        {
            var baseDir = AppContext.BaseDirectory;
            // baseDir = ...\bin\Debug\net8.0\
            var projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            DataDirectory = Path.Combine(projectRoot, "EnglishData");
        }
    }
}