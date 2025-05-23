using System;
using System.IO;
using ProxyPattern.Interfaces;
using ProxyPattern.Models;
using ProxyPattern.Policies;
using ProxyPattern.Services;

namespace ProxyPattern.Services
{
    public class RealFile : IFile
    {
        private readonly string _filePath;

        public RealFile(string filePath)
        {
            _filePath = filePath;
        }

        public void Read(User user)
        {
            var content = File.ReadAllText(_filePath);
            Console.WriteLine(content);
        }

        public FileMetadata GetMetadata()
        {
            var info = new FileInfo(_filePath);
            return new FileMetadata { FileName = info.Name, SizeInBytes = info.Length };
        }
    }
}
