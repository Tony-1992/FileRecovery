using FileRecovery;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

public class FileRecoveryService
{
    private readonly DriveReader _driveReader;
    private readonly FileWriter _fileWriter;
    private readonly FileDetector _fileDetector;
    private readonly int _bufferSize;

    public FileRecoveryService(DriveReader driveReader, FileWriter fileWriter, FileDetector fileDetector, int bufferSize = 4096)
    {
        _driveReader = driveReader;
        _fileWriter = fileWriter;
        _fileDetector = fileDetector;
        _bufferSize = bufferSize;
    }

    public async Task RecoverFilesAsync(string driveLetter)
    {
        string drivePath = $"\\\\.\\{driveLetter}:";
        SafeFileHandle handle = _driveReader.OpenDrive(drivePath);
        if (handle.IsInvalid)
        {
            Console.WriteLine("Failed to open drive.");
            return;
        }

        byte[] buffer = new byte[_bufferSize];
        long totalBytesRead = 0;
        long totalBytes = new DriveInfo($"{driveLetter}:").TotalSize;
        int fileCounter = 0;

        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string outputDirectory = Path.Combine(downloadsPath, "RecoveryFileTests");

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
            Console.WriteLine($"Output directory created at {outputDirectory}");
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        while (await _driveReader.ReadFileAsync(handle, buffer) is uint bytesRead && bytesRead > 0)
        {
            totalBytesRead += bytesRead;

            if (_fileDetector.IsJpegHeader(buffer) || _fileDetector.IsPngHeader(buffer))
            {
                string fileType = _fileDetector.IsJpegHeader(buffer) ? "jpeg" : "png";
                string outputPath = Path.Combine(outputDirectory, $"RecoveredImage_{fileCounter++}.{fileType}");

                _fileWriter.EnqueueWrite(outputPath, buffer, (int)bytesRead, false);

                byte[] additionalBuffer = new byte[_bufferSize];
                while (await _driveReader.ReadFileAsync(handle, additionalBuffer) is uint additionalBytesRead && additionalBytesRead > 0)
                {
                    totalBytesRead += additionalBytesRead;
                    if (_fileDetector.IsJpegFooter(additionalBuffer, additionalBytesRead) || _fileDetector.IsPngFooter(additionalBuffer, additionalBytesRead))
                    {
                        _fileWriter.EnqueueWrite(outputPath, additionalBuffer, (int)additionalBytesRead, true);
                        break;
                    }

                    _fileWriter.EnqueueWrite(outputPath, additionalBuffer, (int)additionalBytesRead, false);
                }
            }

            Console.SetCursorPosition(0, 1);
            Console.Write($"Total: {FormatBytes(totalBytes),10} | Progress: {FormatBytes(totalBytesRead),10} | Items Recovered: {fileCounter}");
        }

        handle.Close();
        _fileWriter.Complete();
        stopwatch.Stop();
        Console.SetCursorPosition(0, Console.CursorTop + 1);
        Console.WriteLine("Scan complete.");
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
