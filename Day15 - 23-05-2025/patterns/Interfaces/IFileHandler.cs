using System;
using System.Collections.Generic;

// Interface for File Handler - manages open/close once

namespace Patterns.Interfaces
{

    public interface IFileHandler
    {
        void Write(string content);
        string[] ReadAll();
        void Close();
    }

}