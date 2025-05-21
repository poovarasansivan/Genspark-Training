using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using ManageAppointments.Interfaces;
using ManageAppointments.Models;

namespace ManageAppointments
{
    public class ManageAppointments
    {
        private readonly IAppointmentServices _appointmentServices;

        public ManageAppointments(IAppointmentServices appointmentServices)
        {
            _appointmentServices = appointmentServices;
        }

        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                int opt = 0;
                while (int.TryParse(Console.ReadLine(), out opt) == false || (opt < 1 && opt > 2))
                {
                    Console.WriteLine("Invalid input, please enter a number.");
                }

                switch (opt)
                {
                    case 1:
                        AddAppointment();
                        break;
                    case 2:
                        SearchAppointment();
                        break;
                    case 3:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        public void PrintMenu()
        {
            Console.WriteLine("1. Add Appointment");
            Console.WriteLine("2. Search Appointment");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
        }

        public void AddAppointment()
        {
            Appointment appointment = new Appointment();
            appointment.TakeAppointmentFromPatient();
            int id = _appointmentServices.AddAppointment(appointment);
            Console.WriteLine($"Appointment added with ID: {id}");
        }

        public void SearchAppointment()
        {
            var searchModel = PrintSearchMenu();
            var appointments = _appointmentServices.SearchAppointment(searchModel);
            if (appointments == null)
            {
                Console.WriteLine("No Employees for the search");
            }
            PrintAppointments(appointments);
        }

        public void PrintAppointments(List<Appointment>? appointments)
        {
            if (appointments == null || appointments.Count == 0)
            {
                Console.WriteLine("No Appointments found.");
                return;
            }

            foreach (var appointment in appointments)
            {
                Console.WriteLine(appointment);
            }
        }

        private AppointmentSearchModel PrintSearchMenu()
        {
            Console.WriteLine("Please select the search option");

            AppointmentSearchModel searchModel = new AppointmentSearchModel();
            Console.WriteLine("Search by Patient Name? 1 - Yes , 2 - No: ");
            int idOption = 0;
            while (int.TryParse(Console.ReadLine(), out idOption) == false)
            {
                Console.WriteLine("Invalid input, please enter a number.");
            }
            if (idOption == 1)
            {
                Console.WriteLine("Enter Patient Name: ");
                string? name = Console.ReadLine();
                searchModel.PatientName = name;
                idOption = 0;
            }
            Console.WriteLine("Search by Patient Age? 1 - Yes , 2 - No: ");
            while (int.TryParse(Console.ReadLine(), out idOption) == false)
            {
                Console.WriteLine("Invalid input, please enter a number.");
            }
            if (idOption == 1)
            {
                searchModel.Age = new RangeInt<int>();
                Console.WriteLine("Enter Patient Minimum Age: ");
                int age;
                while (int.TryParse(Console.ReadLine(), out age) == false)
                {
                    Console.WriteLine("Invalid input, please enter a number.");
                }
                searchModel.Age.minVal = age;
                Console.WriteLine("Enter Patient Maximum Age: ");
                while (int.TryParse(Console.ReadLine(), out age) == false)
                {
                    Console.WriteLine("Invalid input, please enter a number.");
                }
                searchModel.Age.maxVal = age;
                idOption = 0;
            }
            Console.WriteLine("Search by Appointment Date? 1 - Yes , 2 - No: ");
            while (int.TryParse(Console.ReadLine(), out idOption) == false)
            {
                Console.WriteLine("Invalid input, please enter a number.");
            }
            if (idOption == 1)
            {
                Console.WriteLine("Enter Appointment Date (yyyy-mm-dd): ");
                DateTime appointmentDate;
                while (DateTime.TryParse(Console.ReadLine(), out appointmentDate) == false)
                {
                    Console.WriteLine("Invalid input, please enter a valid date.");
                }
                searchModel.AppointmentDate = appointmentDate;
            }
            return searchModel;
        }
    }

}