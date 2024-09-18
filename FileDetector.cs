namespace FileRecovery
{
    public class FileDetector
    {
        public bool IsJpegHeader(byte[] buffer)
        {
            // Check for JPEG header (FF D8 FF)
            return buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
        }

        public bool IsPngHeader(byte[] buffer)
        {
            // Check for PNG header (89 50 4E 47 0D 0A 1A 0A)
            return buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                   buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
        }

        public bool IsJpegFooter(byte[] buffer, uint bytesRead)
        {
            // Check for JPEG footer (FF D9)
            for (int i = 0; i < bytesRead - 1; i++)
            {
                if (buffer[i] == 0xFF && buffer[i + 1] == 0xD9)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPngFooter(byte[] buffer, uint bytesRead)
        {
            // Check for PNG footer (IEND chunk: 49 45 4E 44 AE 42 60 82)
            for (int i = 0; i < bytesRead - 7; i++)
            {
                if (buffer[i] == 0x49 && buffer[i + 1] == 0x45 && buffer[i + 2] == 0x4E && buffer[i + 3] == 0x44 &&
                    buffer[i + 4] == 0xAE && buffer[i + 5] == 0x42 && buffer[i + 6] == 0x60 && buffer[i + 7] == 0x82)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
