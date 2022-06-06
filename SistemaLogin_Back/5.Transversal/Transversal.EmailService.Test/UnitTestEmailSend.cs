using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transversal.EmailService.Test
{
    [TestClass]
    public class UnitTestEmailSend
    {

        [TestMethod]
        public async Task SendGridEmailAsync()
        {
            try
            {
                var apiKey = Environment.GetEnvironmentVariable("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");
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("test@example.com", "Example User");
                var subject = "Sending with SendGrid is Fun";
                var to = new EmailAddress("test@example.com", "Example User");
                var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                int asd = 1;
                int asd2 = 0;
                var r = asd / asd2;
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
