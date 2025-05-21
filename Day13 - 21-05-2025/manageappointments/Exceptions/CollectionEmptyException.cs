using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ManageAppointments.Exceptions
{
    public class CollectionEmptyException : Exception
    {
        private string _message = "Collection is empty";

        public CollectionEmptyException(string message)
        {
            _message = message;

        }
        
        public override string Message => _message;

    }
}