using SolidPrinciples.Models;
using SolidPrinciples.Services;
using SolidPrinciples.Interfaces;
using SolidPrinciples.Processors;
using System;

namespace SolidPrinciples
{
    class Program
    {
        static void Main(string[] args)
        {
            var invoice = new Invoice { Quantity = 10, PricePerItem = 20 };

            // It follows LSP principle
            //  Allows to substitute a subclass or implementation without breaking the app.
            // Superclass objects can be replaceable with subclass objects without 
            // without breaking the program.
            
            IInvoiceCalculator calculator = new DiscountedInvoiceCalculator(10);

            IInvoiceRepository repository = new InvoiceRepository();

            var processor = new InvoiceProcessor(calculator, repository);
            processor.Process(invoice);

            Console.WriteLine($"Calculated Total after discount: {invoice.Total}");

            var emailSender = new InvoiceEmailSender();
            emailSender.SendEmail(invoice);
        }
    }
}
