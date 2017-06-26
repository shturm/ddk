using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;

namespace Ddk.Web.Services
{
    public class EmailSender
    {
        private readonly IConfigurationRoot configuration;

        private const string ConfigurationFileName = "appsettings.json";

        private const string AdminEmail = "office@daidakaram.com";

        private const string HostName = "alfa.superhosting.bg";

        private const int SmtpPort = 465;

        public EmailSender()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigurationFileName);
            this.configuration = builder.Build();
        }

        public void SendEmail(string subject, string message)
        {
            var username = this.configuration["EmailSender:username"];
            var password = this.configuration["EmailSender:password"];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            for (int index = 0; index < int.MaxValue; index++)
            {
                var email = this.configuration[$"EmailSender:sentTo:{index}"];
                if (string.IsNullOrEmpty(email))
                {
                    break;
                }

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(AdminEmail));
                mimeMessage.To.Add(new MailboxAddress(email));
                mimeMessage.Subject = subject;

                mimeMessage.Body = new TextPart("plain")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(HostName, SmtpPort, useSsl: true);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(username, password);

                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
            }
        }
    }
}
