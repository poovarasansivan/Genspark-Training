namespace SolidPrinciples.Models
{
    // Holds the data model.
    // This class is responsible for holding the data of the invoice.
    public class Invoice
    {
        public int Quantity { get; set; }
        public double PricePerItem { get; set; }
        public double Total { get; set; }
    }

}