using System;
using System.Drawing;
using System.Text;

namespace ImgPixelHide
{
    internal class ReadData
    {
        string Path;
        Bitmap Img;

        public ReadData(string path)
        {
            Path = path;
            Img = LoadImage(path);
        }

        Bitmap LoadImage(string imgPath)
        {
            return new Bitmap(imgPath);
        }

        public string ReadDataFromPixels()
        {
            Bitmap image = Img;

            // Read the length of the message (16 bits)
            string lengthBits = "";
            for (int y = 0; y < 2; y++) // Read from the first two pixels
            {
                for (int x = 0; x < 1; x++) // Read one pixel from each row
                {
                    Color pixel = image.GetPixel(x, y);
                    lengthBits += Convert.ToString(pixel.R, 2).PadLeft(8, '0')[7]; // Get LSB of R
                    lengthBits += Convert.ToString(pixel.G, 2).PadLeft(8, '0')[7]; // Get LSB of G
                    lengthBits += Convert.ToString(pixel.B, 2).PadLeft(8, '0')[7]; // Get LSB of B
                }
            }

            // Convert lengthBits from binary to an integer
            int messageLength = Convert.ToInt32(lengthBits, 2);
            Console.WriteLine($"Length of the message: {messageLength} characters.");

            // Read the actual message based on the determined length
            string messageBin = "";
            int bitsRead = 0;

            for (int y = 0; y < image.Height && bitsRead < messageLength * 8; y++)
            {
                for (int x = 0; x < image.Width && bitsRead < messageLength * 8; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    // Read LSBs from RGB components
                    messageBin += Convert.ToString(pixel.R, 2).PadLeft(8, '0')[7]; // Get LSB of R
                    messageBin += Convert.ToString(pixel.G, 2).PadLeft(8, '0')[7]; // Get LSB of G
                    messageBin += Convert.ToString(pixel.B, 2).PadLeft(8, '0')[7]; // Get LSB of B
                }
            }

            // Check if we read enough bits
            if (messageBin.Length < messageLength * 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning: Not enough bits read for the message.");
                Console.ResetColor();
                return string.Empty; // Return empty if not enough bits
            }

            // Convert the binary message to bytes and then to a string
            byte[] messageBytes = new byte[messageLength];

            for (int i = 0; i < messageLength; i++)
            {
                // Get 8 bits for each byte
                string byteBin = messageBin.Substring(i * 8, 8);
                messageBytes[i] = Convert.ToByte(byteBin, 2);
            }

            return Encoding.UTF8.GetString(messageBytes); // Convert bytes back to string
        }
    }
}
