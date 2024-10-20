using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

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

                    reader.ReadDataFromPixels();
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
    }
}
