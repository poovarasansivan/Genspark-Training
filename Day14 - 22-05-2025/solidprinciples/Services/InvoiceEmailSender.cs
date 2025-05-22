using SolidPrinciples.Models;

namespace SolidPrinciples.Services
{
    // for sending invoice emails
    // This class is responsible for sending emails related to invoices.
    public class InvoiceEmailSender
    {
        public void SendEmail(Invoice invoice)
        {
            Console.WriteLine($"Email sent for invoice with total: {invoice.Total}");
        }
    }
}
