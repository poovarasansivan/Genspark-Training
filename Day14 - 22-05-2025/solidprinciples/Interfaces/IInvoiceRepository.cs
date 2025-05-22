
using SolidPrinciples.Models;

namespace SolidPrinciples.Interfaces
{
    // This interface defines the contract for saving invoices.
    public interface IInvoiceRepository
    {
        void Save(Invoice invoice);
    }
}

