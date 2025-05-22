using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    // Contains all the concrete service implementations.

    public class InvoiceCalculator
    {
        public virtual void CalculateTotal(Invoice invoice)
        {
            invoice.Total = invoice.Quantity * invoice.PricePerItem;
        }
    }
}