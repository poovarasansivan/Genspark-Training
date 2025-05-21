using System;

namespace ManageAppointments.Models
{
    public class Appointment : IComparable<Appointment>, IEquatable<Appointment>
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;

        public Appointment()
        {

        }
        public Appointment(int id, string? patientName, int patientAge, DateTime appointmentDate, string reason)
        {
            Id = id;
            PatientName = patientName;
            PatientAge = patientAge;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }

        public void TakeAppointmentFromPatient()
        {
            // Console.WriteLine("Please enter the Patient Id ");
            // int id;
            // while (!int.TryParse(Console.ReadLine(), out id))
            // {
            //     Console.WriteLine("Invalid input. Please enter a valid integer for Patient Id.");
            // }
            // Id = id;
            Console.WriteLine("Please enter the Patient Name ");
            PatientName = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the Patient Age ");
            int age;
            while (!int.TryParse(Console.ReadLine(), out age))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer for Patient Age.");
            }
            PatientAge = age;
            Console.WriteLine("Please enter the Appointment Date (yyyy-mm-dd hh:mm:ss):");
            DateTime appointmentDate;
            while (!DateTime.TryParse(Console.ReadLine(), out appointmentDate))
            {
                Console.WriteLine("Invalid input. Please enter a valid date for Appointment Date.");
            }
            AppointmentDate = appointmentDate;
            Console.WriteLine("Please enter the Reason for Appointment ");
            Reason = Console.ReadLine() ?? "";
            Console.WriteLine("Appointment taken successfully.");
        }
        public override string ToString()
        {
            return $"Id: {Id}, Patient Name: {PatientName}, Patient Age: {PatientAge}, Appointment Date: {AppointmentDate.ToShortDateString()}, Reason: {Reason}";
        }

        public void DisplayAppointment()
        {
            Console.WriteLine($"Id: {Id}, Patient Name: {PatientName}, Patient Age: {PatientAge}, Appointment Date: {AppointmentDate.ToShortDateString()}, Reason: {Reason}");
        }

        public int CompareTo(Appointment? other)
        {
            if (other == null) return 1;
            return this.Id.CompareTo(other.Id);
        }
        public bool Equals(Appointment? other)
        {
            return this.Id == other?.Id;
        }
    }
}