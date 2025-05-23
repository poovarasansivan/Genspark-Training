using Patterns.Interfaces;
using Patterns.Services;

// Concrete Factory - TextLoggerFactory
// LoggerFactoryBase is a superclass and this file textloggerfactory (subclass) create instances of
// different logger types without the client needing to know the details (eg. TextLoggerFactory, JsonLogger).
// This class implements the factory method to create a TextLogger instance.
// It inherits from LoggerFactoryBase and provides the implementation for the CreateLogger method.
// Design Pattern Role: Concrete Factory

namespace Patterns.Factories
{
    public class TextLoggerFactory : LoggerFactoryBase
    {
        public override ILogger CreateLogger(string filePath)
        {
            var fileHandler = FileHandlerSingleton.GetInstance(filePath);
            return new TextLogger(fileHandler);
        }
    }
}
