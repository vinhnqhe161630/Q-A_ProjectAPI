﻿using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using PRN231_ProjectQA_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Service.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public void SendEmail(EmailDto request)
        {
            var email = new MimeKit.MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Email"]));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Body
            };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_config["EmailSettings:Host"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["EmailSettings:Email"], _config["EmailSettings:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
