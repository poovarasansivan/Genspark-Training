using System;

namespace ManageAppointments.Models
{
    public class AppointmentSearchModel
    {
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public RangeInt<int>? Age { get; set; }
    }

    public class RangeInt<T>
    {
        public T? minVal { get; set; }
        public T? maxVal { get; set; }
    }

}