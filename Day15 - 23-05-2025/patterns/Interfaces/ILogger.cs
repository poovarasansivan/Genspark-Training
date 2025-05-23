using System;
// Interface for Logger
namespace Patterns.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        string[] ReadAll();
        void Close();
    }
}