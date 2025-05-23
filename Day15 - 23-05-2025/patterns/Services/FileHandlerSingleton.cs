using System;
using System.IO;
using Patterns.Interfaces;

// Singleton pattern for file handling
// It ensures that only one instance of the file handler is created
// and provides a global point of access to it. Open/close is managed
// to ensure that the file is not opened multiple times.
// Design Pattern Role: Singleton
// Prevents File being locked by multiple writers

namespace Patterns.Services
{
    public class FileHandlerSingleton : IFileHandler
    {
        private static FileHandlerSingleton? _instance;
        private static readonly object _lock = new object();
        //_lock: used to safely create the instance 
        // if multiple threads are trying to use it at once (thread-safety).
        
        private StreamWriter _writer;
        private readonly string _filePath;
        private bool _isClosed = false;

        private FileHandlerSingleton(string filePath)
        {
            _filePath = filePath;

            // Allow reading from file while writing
            _writer = new StreamWriter(new FileStream(
                _filePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read
            ));
        }

        public static FileHandlerSingleton GetInstance(string filePath)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new FileHandlerSingleton(filePath);
                    }
                }
            }
            return _instance!;
        }

        public void Write(string content)
        {
            _writer.WriteLine(content);
        }

        public string[] ReadAll()
        {
            if (!_isClosed)
            {
                _writer.Flush();
            }

            using var stream = new FileStream(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite
            );

            using var reader = new StreamReader(stream);
            var lines = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines.ToArray();
        }


        public void Close()
        {
            if (!_isClosed)
            {
                _writer.Flush();
                _writer.Close();
                _isClosed = true;
            }
        }
    }
}
