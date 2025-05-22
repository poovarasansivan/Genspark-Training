using SolidPrinciples.Interfaces;
using SolidPrinciples.Models;
using System;

namespace SolidPrinciples.Services
{
    // for saving invoices
    // This class is responsible for saving invoices to the database.
    public class InvoiceRepository : IInvoiceRepository
    {
        public void Save(Invoice invoice)
        {
            Console.WriteLine($"Invoice saved with total: {invoice.Total}");
        }
    }
}
