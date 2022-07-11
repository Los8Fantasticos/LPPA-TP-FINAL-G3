using System;
using System.Collections.Generic;
using System.Text;
using Transversal.EmailService.SendGrid;

namespace Transversal.EmailService
{
    public class EmailConfiguration
    {
        public string Type { get; set; }
        public EmailSendGridConfiguration EmailSendGridConfiguration { get; set; }
        public EmailSMTPConfiguration EmailSMTPConfiguration { get; set; }
        public bool TestEnabled { get; set; }
    }
}
