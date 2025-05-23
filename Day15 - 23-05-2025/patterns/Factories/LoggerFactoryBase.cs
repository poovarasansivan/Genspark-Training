using Patterns.Interfaces;

// Abstract Factory Base Class
// This class defines the method for creating loggers.
// it is an abstract class that act as a base factory for shared logic.

namespace Patterns.Factories
{
    public abstract class LoggerFactoryBase : ILoggerFactory
    {
        public abstract ILogger CreateLogger(string filePath);
    }
}
