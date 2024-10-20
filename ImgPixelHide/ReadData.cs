using System;
using System.Collections.Generic;
using System.Drawing;

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
            Console.ForegroundColor = ConsoleColor.Green; // Change console color
            Console.WriteLine("Image loaded successfully");
            Console.ResetColor(); // Reset console color
            return new Bitmap(imgPath);
        }

        public void ReadDataFromPixels()
        {
            Bitmap image = Img;
            int bitCounter = 0;
            string bitLengthRecovered = "";
            string msgRecoveredBits = "";

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    string R = Convert.ToString(pixel.R, 2).PadLeft(8, '0');
                    string G = Convert.ToString(pixel.G, 2).PadLeft(8, '0');
                    string B = Convert.ToString(pixel.B, 2).PadLeft(8, '0');

                    List<string> rgb = new List<string> { R, G, B };

                    for (int i = 0; i < rgb.Count; i++)
                    {
                        char lsbBit = rgb[i][rgb[i].Length - 1]; 

                        if (bitCounter < 16)
                        {
                            bitLengthRecovered += lsbBit.ToString();
                        }
                        else if (bitCounter >= 16 && bitCounter < Convert.ToInt32(bitLengthRecovered, 2))
                        {
                            msgRecoveredBits += lsbBit.ToString();
                        }
                        else
                        {
                            break; 
                        }
                        bitCounter++;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Size of the recovered message: {bitLengthRecovered.Length} bits");
            Console.WriteLine($"Recovered bits: {msgRecoveredBits}");
            Console.ResetColor();

            string decodedMsg = "";

            for (int i = 0; i <= msgRecoveredBits.Length - 8; i += 8)
            {
                string bin = msgRecoveredBits.Substring(i, 8);
                int binToInt = Convert.ToInt32(bin, 2);
                decodedMsg += (char)binToInt;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Decoded message: ");
            Console.WriteLine(decodedMsg);
            Console.ResetColor();
        }
    }
}
