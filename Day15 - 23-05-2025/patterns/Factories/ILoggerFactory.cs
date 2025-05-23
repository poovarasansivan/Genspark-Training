using Patterns.Interfaces;

// Abstract Factory Interface
// IloggerFactory is an interface acts as an abstract factory to create families of similar products 
// (TextLogger, JsonLogger) by encapsulating the Object creation logic.
// Client can switch between different logger Factory types without changing the code.


namespace Patterns.Factories
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(string filePath);
        // Any class that implements this interface must provide a method 
        // called CreateLogger with same parameters and return type should be ILogger.
    }
}
