using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Configuration;

namespace ImgPixelHide
{
    internal class Program
    {
        #region Main Class
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("[1] Hide data in image");
                Console.WriteLine("[2] Get data from image");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Enter image path: ");
                    string path =  Console.ReadLine().Trim('"');
                    path = $@"{path}";

                    Console.Write("\nMessage to hide: ");
                    string msg = Console.ReadLine().Trim();

                    HideData hideData = new HideData(msg, path);

                    if (hideData.VerifyImageCapacity())
                    {
                        hideData.HideDataInPixels();
                    }
                }
                else if (choice == "2")
                {
                    Console.Write("Enter image path: ");
                    string path = Console.ReadLine().Trim('"');
                    path = $@"{path}";

                    ReadData reader = new ReadData(path);

                    string hiddenMessage = reader.ReadDataFromPixels();
                    Console.WriteLine("Hidden Message: " + hiddenMessage);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Answer: ONE(1) OR TWO(2)");
                    Console.ResetColor();
                }

                Console.WriteLine("\nReturn? (yes/no)");
                if (Console.ReadLine().ToLower() != "yes") break;
            }
        }
        #endregion

        #region Hide Data Class
        internal class HideData
        {
            string Msg;
            string Path;
            Bitmap Img;

            public HideData(string msg, string path)
            {
                Msg = msg;
                Path = path;
                Img = LoadImage(path);
            }

            Bitmap LoadImage(string imgPath)
            {
                Console.WriteLine("Image Loaded");
                return new Bitmap(imgPath);
            }

            void SaveImage(string originalPath, Bitmap image)
            {
                string newPath = originalPath.Insert(originalPath.LastIndexOf('.'), "_hidden");
                image.Save(newPath);
                Console.WriteLine($"Imagen saved in: {newPath}");
            }

            byte[] GetMessageBytes(string msg)
            {
                return Encoding.UTF8.GetBytes(msg);
            }

            public string GetLengthBits()
            {
                return Convert.ToString(CalculateRequiredBits(GetMessageBytes(Msg)), 2).PadLeft(16, '0');
            }

            int CalculateRequiredBits(byte[] Data)
            {
                int messageBits = Data.Length * 8;

                int lengthBits = 16;

                if (lengthBits % 8 != 0)
                {
                    lengthBits = (int)(Math.Ceiling(lengthBits / 8.0) * 8);
                }

                return messageBits + lengthBits;
            }

            public bool VerifyImageCapacity()
            {
                Bitmap image = Img;
                byte[] Data = GetMessageBytes(Msg);

                int capacityInBits = image.Width * image.Height * 3;
                int requiredBits = CalculateRequiredBits(Data);

                if (requiredBits <= capacityInBits)
                {
                    Console.WriteLine($"The image hace the capacity to save {capacityInBits} bits. You need {requiredBits} bits of space");
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The image dont have the needed pixels to hide your data. You need min {requiredBits} bits");
                    Console.ResetColor();
                    return false;
                }
            }

            public void HideDataInPixels()
            {
                Bitmap image = Img;

                string lengthBin = GetLengthBits();
                byte[] Data = GetMessageBytes(Msg);

                int totalBits = CalculateRequiredBits(Data);
                int bitCounter = 0;
                string msgBin = "";

                foreach (byte b in Msg)
                {
                    msgBin += Convert.ToString(b, 2).PadLeft(8, '0');
                }

                string completeData = lengthBin + msgBin;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Binary Length of Message: " + lengthBin);
                Console.WriteLine("Binary Message: " + msgBin);
                Console.ResetColor();

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Complete Data to Hide: " + completeData);
                Console.ResetColor();

                for (int y = 0; y < image.Height && bitCounter < totalBits; y++)
                {
                    for (int x = 0; x < image.Width && bitCounter < totalBits; x++)
                    {
                        Color pixel = image.GetPixel(x, y);

                        string R = Convert.ToString(pixel.R, 2).PadLeft(8, '0');
                        string G = Convert.ToString(pixel.G, 2).PadLeft(8, '0');
                        string B = Convert.ToString(pixel.B, 2).PadLeft(8, '0');

                        List<string> rgb = new List<string> { R, G, B };
                        List<int> rgbInts = new List<int>(3);

                        for (int i = 0; i < rgb.Count && bitCounter < completeData.Length; i++)
                        {
                            string combinedBinary = rgb[i].Substring(0, rgb[i].Length - 1) + completeData[bitCounter];
                            int newRgbValue = Convert.ToInt32(combinedBinary, 2);

                            newRgbValue = Math.Max(0, Math.Min(255, newRgbValue));
                            rgbInts.Add(newRgbValue);

                            bitCounter++;
                        }

                        while (rgbInts.Count < 3)
                        {
                            rgbInts.Add(pixel.R);
                            rgbInts.Add(pixel.G);
                            rgbInts.Add(pixel.B);
                        }

                        image.SetPixel(x, y, Color.FromArgb(rgbInts[0], rgbInts[1], rgbInts[2]));

                        Color newPixel = image.GetPixel(x, y);

                        string newR = Convert.ToString(newPixel.R, 2).PadLeft(8, '0');
                        string newG = Convert.ToString(newPixel.G, 2).PadLeft(8, '0');
                        string newB = Convert.ToString(newPixel.B, 2).PadLeft(8, '0');

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Old Pixel at (x: {x}, y: {y}) - R: {R}, G: {G}, B: {B}");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"New Pixel at (x: {x}, y: {y}) - R: {newR}, G: {newG}, B: {newB}");
                        Console.ResetColor();

                        Console.WriteLine();
                    }
                }
                SaveImage(Path, image);
            }
        }
        #endregion
    }
}
