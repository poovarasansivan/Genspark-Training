namespace FitnessTracking.Models
{
    public class AppointmentModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Default status is Pending
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}