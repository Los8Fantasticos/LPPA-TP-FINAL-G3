using Core.Contracts.Services;
using Core.Domain.ApplicationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Transversal.EmailService;
using Transversal.EmailService.SendGrid;

namespace Core.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly EmailSendGridConfiguration _emailSendGridConfiguration;

        public EmailService(ILogger<EmailService> logger, UserManager<Users> userManager, EmailSendGridConfiguration emailSendGridConfiguration)
        {
            _logger = logger;
            _userManager = userManager;
            _emailSendGridConfiguration = emailSendGridConfiguration;
        }

        public async Task RegistrationEmailAsync(Users user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $"ConfirmAccountPage?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
            await SendEmailRegisterUser(user, url);
        }
        private async Task SendEmailRegisterUser(Users user, string url)
        {
            var message = new Message
                (
                    to: new string[] { user.Email },
                    subject: "Registro de usuario",
                    content: "Hola. Acepte registrarte",
                    emailFrom: _emailSendGridConfiguration.From,
                    attachments: null
                );
            await SendEmailAsync(user.Email, message);
        }
        private async Task<string> SendEmailAsync(string toEmail, Message message, IFormFileCollection files = null)
        {
            EmailSendgridSender emailSendgridSender = new EmailSendgridSender(_emailSendGridConfiguration);
            return await emailSendgridSender.SendEmailAsync(message);
        }
    }
}
