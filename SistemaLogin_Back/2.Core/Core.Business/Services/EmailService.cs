﻿using Core.Contracts.Services;
using Core.Domain.ApplicationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Transversal.EmailService;
using Transversal.EmailService.Configurations;
using Transversal.EmailService.Factory;
using Transversal.Helpers;

namespace Core.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly UserManager<Users> _userManager;
        private readonly EmailSendGridConfiguration _emailSendGridConfiguration;
        private readonly IGenericEmailFactory _genericEmailFactory;

        public EmailService(
            ILogger<EmailService> logger,
            UserManager<Users> userManager,
            EmailSendGridConfiguration emailSendGridConfiguration,
            IGenericEmailFactory GenericEmailFactory
            )
        {
            _logger = logger;
            _userManager = userManager;
            _emailSendGridConfiguration = emailSendGridConfiguration;
            _genericEmailFactory = GenericEmailFactory;
        }

        public async Task RegistrationEmailAsync(Users user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $"https://localhost:44308/Auth/Confirm?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";
            await SendEmailRegisterUser(user, url);
        }



        #region Helpers
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

            string body = HtmlDocumentHelper.GetHtmlDocument("Register.html", null, new List<string> { url });
            message.Content = body;
            await SendEmailAsync(user.Email, message);
        }
        
        private async Task<string> SendEmailAsync(string toEmail, Message message, IFormFileCollection files = null)
        {
            var EmailService = _genericEmailFactory.GetDefault();
            var result = await EmailService.SendEmailAsync(message);
            return result.Data;
        }
        #endregion
    }
}
