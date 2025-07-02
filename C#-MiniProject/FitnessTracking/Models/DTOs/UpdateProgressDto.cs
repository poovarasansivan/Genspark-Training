namespace FitnessTracking.Models.DTOs
{
    public class UpdateProgressDto
    {
        public Guid WorkOutLogId { get; set; }
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public double BodyFatPercentage { get; set; }
        public double MuscleMass { get; set; }
        public double WaterPercentage { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}