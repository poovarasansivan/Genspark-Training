using Patterns.Interfaces;

// Concrete Product  - TextLogger
// Implements the ILogger interface
// Uses the IFileHandler interface to manage file operations
// Design Pattern Role: Concrete Product
// This class is responsible for logging messages to a text file

namespace Patterns.Services
{
    public class TextLogger : ILogger
    {
        private readonly IFileHandler _fileHandler;

        public TextLogger(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public void Log(string message)
        {
            _fileHandler.Write($"[TEXT] {DateTime.Now}: {message}");
        }

        public string[] ReadAll()
        {
            return _fileHandler.ReadAll();
        }

        public void Close()
        {
            _fileHandler.Close();
        }
    }
}
