using Patterns.Factories;
using Patterns.Interfaces;
using System;

namespace Patterns
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Csharp\patterns\Data\logs.txt";

            ILoggerFactory factory = new TextLoggerFactory();
            ILogger logger = factory.CreateLogger(filePath);

            logger.Log("Application started.");
            logger.Log("Performing some operations.");
            logger.Log("Application finished.");

            Console.WriteLine("Logs read from file:");
            foreach (var line in logger.ReadAll())
            {
                Console.WriteLine(line);
            }

            logger.Close();

            Console.WriteLine("File closed. Application ended.");
        }
    }
}
