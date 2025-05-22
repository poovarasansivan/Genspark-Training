// Contains interface definitions for following ISP & DIP.
// ISP: Interface Segregation Principle
// The IInvoiceCalculator interface is designed to be implemented by classes that calculate invoice amounts.
// Interfaces follows abstraction. It defines a what should be done, not how it should be done.
// Don’t force classes to depend on methods they don’t use.

using SolidPrinciples.Models;
using SolidPrinciples.Services;

namespace SolidPrinciples.Interfaces
{
    public interface IInvoiceCalculator
    {
        void CalculateTotal(Invoice invoice);
    }
}