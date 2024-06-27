using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//using System.Net;
//using System.Net.Mail;
//using System.Net.Mail;
//using System.Net;
using System.Text;
using System.Threading.Tasks;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;


namespace Common
{
    public class Email
    {
      
        public void SendEmail(string recipient, string subject, string body)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("tanuja.gadhe@coditas.com"));
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject =subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using (var smtp = new SmtpClient())
            {
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                smtp.Connect("smtp.gmail.com", 587);
                smtp.Authenticate("tanuja.gadhe@coditas.com", "Tanu@0218");
                smtp.Send(email);
                smtp.Disconnect(true);
            }

        }
    }
}
