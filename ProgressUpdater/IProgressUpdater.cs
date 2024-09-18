namespace FileRecovery.ProgressUpdater
{
    public interface IProgressUpdater
    {
        void UpdateProgress(long totalBytes, long totalBytesRead, int fileCounter);
    }
}
