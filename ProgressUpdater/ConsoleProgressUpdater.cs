namespace FileRecovery.ProgressUpdater
{
    public class ConsoleProgressUpdater : IProgressUpdater
    {
        public void UpdateProgress(long totalBytes, long totalBytesRead, int fileCounter)
        {
            Console.SetCursorPosition(0, 1);
            Console.Write($"Total: {FormatBytes(totalBytes),10} | Progress: {FormatBytes(totalBytesRead),10} | Items Recovered: {fileCounter}");
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "MB", "GB", "TB" };
            double len = bytes / 1024.0 / 1024.0; // Start with MB
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{Math.Round(len, 2):0.00} {sizes[order]}";
        }
    }
}
