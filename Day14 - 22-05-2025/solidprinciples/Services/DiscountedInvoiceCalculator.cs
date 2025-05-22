using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    // It follows the Open/Closed Principle (OCP) by allowing new types of invoice calculators 
    // to be added without modifying existing code.

    public class DiscountedInvoiceCalculator : IInvoiceCalculator
    {
        private readonly double _discountPercentage;

        public DiscountedInvoiceCalculator(double discountPercentage)
        {
            _discountPercentage = discountPercentage;
        }

        public void CalculateTotal(Invoice invoice)
        {
            double total = invoice.Quantity * invoice.PricePerItem;
            invoice.Total = total - (total * _discountPercentage / 100);
        }
    }
}
