using SolidPrinciples.Models;
using SolidPrinciples.Interfaces;

namespace SolidPrinciples.Processors
{
    // Dependency Inversion Principle (DIP)
    // High-level modules (like InvoiceProcessor) should depend on abstractions (interfaces) not 
    // on low-level modules (concrete classes).,
    // The InvoiceProcessor class depends on abstractions (IInvoiceCalculator and IInvoiceRepository)

    public class InvoiceProcessor
    {
        private readonly IInvoiceCalculator _calculator;
        private readonly IInvoiceRepository _repository;

        public InvoiceProcessor(IInvoiceCalculator calculator, IInvoiceRepository repository)
        {
            _calculator = calculator;
            _repository = repository;
        }

        public void Process(Invoice invoice)
        {
            _calculator.CalculateTotal(invoice);
            _repository.Save(invoice);
        }
    }
}
