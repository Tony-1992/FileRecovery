using FileRecovery;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Enter the drive letter to scan (e.g., C, D, E): ");
            string driveLetter = Console.ReadLine().ToUpperInvariant();

            if (string.IsNullOrEmpty(driveLetter) || driveLetter.Length != 1 || !char.IsLetter(driveLetter[0]))
            {
                Console.WriteLine("Invalid drive letter. Please enter a single letter (e.g., C).");
                return;
            }

            var driveReader = new DriveReader();
            var fileWriter = new FileWriter();
            var fileDetector = new FileDetector();

            var fileRecoveryService = new FileRecoveryService(driveReader, fileWriter, fileDetector);

            Console.Clear();
            await fileRecoveryService.RecoverFilesAsync(driveLetter);
            Console.WriteLine("File recovery process completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
