using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Meltdown
{
    /// <summary>
    /// This is a partial implementation of Meltdown: The Send Email functionality
    /// </summary>
    public static partial class Meltdown
    {
        /// <summary>
        /// Send a HTML formatted email using SmtpClient.
        /// Simpler parameter list than SendEmail()
        /// </summary>
        /// <param name="Name">Sender's name</param>
        /// <param name="FromEmail">Sender's email address</param>
        /// <param name="FromPassword">Sender's Password</param>
        /// <param name="ToEmail">(Single) target email</param>
        /// <param name="Subject">Message Subject</param>
        /// <param name="htmlText">HTML formatted HTML message</param>
        /// <param name="Url">Optional The target Mail Service. Default smtp.office365.com. Use smtp.gmail.com for Gmail</param>
        /// <param name="Port">Optional The Mail Service port. Default 587</param>
        /// <returns></returns>
        public static string SendMailMinimal(
            string Name, string FromEmail, string FromPassword,
            string ToEmail,
            string Subject, string htmlText,
            string Url = "smtp.office365.com", int Port = 587)
        {
            if (string.IsNullOrEmpty(Name))
                return "Sender name required";
            else if (string.IsNullOrEmpty(FromEmail))
                return "Sender email address required";
            else if (string.IsNullOrEmpty(ToEmail))
                return "To email address required";
            return SendMail(
                new MailAddress(FromEmail, Name), FromPassword,
                new MailAddress(ToEmail, ToEmail),
                Subject, htmlText,
                Url, Port);
        }

        /// <summary>
        /// Send a HTML formatted email using SmtpClient
        /// </summary>
        /// <param name="From">The sender MailAddress</param>
        /// <param name="FromPassword">The sender Password</param>
        /// <param name="To">The receiver MailAdress</param>
        /// <param name="Subject">The message Subject</param>
        /// <param name="htmlText">The HTML formatted message</param>
        /// <param name="Url">Optional The target Mail Service. Default smtp.office365.com. Use smtp.gmail.com for Gmail</param>
        /// <param name="Port">Optional The Mail Service port. Default 587</param>
        /// <param name="Tos">Optional csv list of target emails. To is ignored if used.</param>
        /// <param name="Ccs">Optional csv list of CC targets emails</param>
        /// <param name="Bccs">Optional csv list of BCC target emails</param>
        /// <returns></returns>
        public static string SendMail(
            MailAddress From, string FromPassword,
            MailAddress To,    
            string Subject, string htmlText,
            string Url= "smtp.office365.com", int Port=587,
            string Tos= "",     //Csv list of emails
            string Ccs="",      //Csv list of emails
            string Bccs= "")    //Csv list of emails
        {
            if (From == null)
                return "From MailAddress required.";
            else if (string.IsNullOrEmpty(From.DisplayName))
                return "Sender name required";
            else if (string.IsNullOrEmpty(From.Address))
                return "Sender email address required";
            else if (string.IsNullOrEmpty(Subject))
                return "Subject required";
            else if (string.IsNullOrEmpty(htmlText))
                return "Message required";
            else if (string.IsNullOrEmpty(Url))
                return "Mail service Url required";
            else if ((To == null) && (Tos == null))
                return "Target required.";
            else if (To != null)
            {
                if (string.IsNullOrEmpty(To.Address))
                    return "Target email address required";
            }
            string res = "Sent OK";
            MailMessage msg = new MailMessage();
            msg.From = From;
            if (To != null)
                msg.To.Add(To);
            else if (!string.IsNullOrEmpty(Tos))
                msg.To.Add(Tos);
            else
            {
                res = "No target for message";
                return res;
            }
            if (!string.IsNullOrEmpty(Ccs))
            {
                msg.CC.Add(Ccs);
            }
            if (!string.IsNullOrEmpty(Bccs))
            {
                msg.Bcc.Add(Bccs);
            }

            msg.Subject = Subject;
            msg.IsBodyHtml = true;
            msg.Body = $@"<html><body><h1><font color=""red"">{Subject}</font></h1>{htmlText}</body></html>";
            msg.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(From.Address, FromPassword);
            client.Port = Port;
            client.Host = Url;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;
        }
    }
}
