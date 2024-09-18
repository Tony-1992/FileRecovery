using System.Collections.Concurrent;

public class FileWriter
{
    private readonly BlockingCollection<(string path, byte[] data, int count, bool isLastChunk)> _writeQueue = new BlockingCollection<(string, byte[], int, bool)>();

    public FileWriter()
    {
        Task.Run(() => ProcessWriteQueue());
    }

    public void EnqueueWrite(string outputPath, byte[] buffer, int bytesToWrite, bool isLastChunk)
    {
        var data = new byte[bytesToWrite];
        Array.Copy(buffer, data, bytesToWrite);
        _writeQueue.Add((outputPath, data, bytesToWrite, isLastChunk));
    }

    private async Task ProcessWriteQueue()
    {
        var openFiles = new Dictionary<string, FileStream>();

        foreach (var (path, data, count, isLastChunk) in _writeQueue.GetConsumingEnumerable())
        {
            if (!openFiles.TryGetValue(path, out FileStream fs))
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                openFiles[path] = fs;
            }

            await fs.WriteAsync(data, 0, count);

            if (isLastChunk)
            {
                fs.Close();
                openFiles.Remove(path);
            }
        }

        foreach (var fs in openFiles.Values)
        {
            fs.Close();
        }
    }

    public void Complete() => _writeQueue.CompleteAdding();
}
