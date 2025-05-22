using System;
using System.Collections;
using System.Collections.Generic;
using SolidPrinciples.Models;
using SolidPrinciples.Interfaces;

namespace SolidPrinciples.Interfaces
{
    public interface IInvoiceEmailSender
    {
        void SendEmail(string email, string subject, string body);
    }
}