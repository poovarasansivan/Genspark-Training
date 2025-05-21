using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ManageAppointments.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        private string _message = "Duplicate entity found";

        public DuplicateEntityException(string message)
        {
            _message = message;

        }
        
        public override string Message => _message;

    }
}