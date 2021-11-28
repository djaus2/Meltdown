using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;
using BlazMail.Shared;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace BlazMail.Data
{


	public class IgnoreEmail
    {
		[Key]
		[Column("Id")]
		[JsonPropertyName("Id")]
		public int Id { get; set; }

		[Column("Email")]
		[JsonPropertyName("Email")]
		public string Email { get; set; }
    }
	public class EmailMsg
	{
		private readonly AppSettings Settings;
		public EmailMsg(AppSettings _settings)
		{
			Settings = _settings;
			clientHost = Settings.ClientHost;
			fromName = Settings.fromName;
			fromaEmail = Settings.fromEmail;
			fromPassword = Settings.fromPassword;
			string clientPortStr = Settings.ClientPortStr;
			if (!int.TryParse(clientPortStr, out clientPort))
			{
				clientPort = 587;
			}
		}

		private string clientHost = "smtp.office365.com";
		private int clientPort = 587;
		//private string clientHost = "smtp.gmail.com";
		//private int clientPort = 587;
		private string fromName = "<From Name>";
		private string fromaEmail = "<From Email>";
		private string fromPassword = "<From Password>";


		public async Task<string> SendEmail(EmailUser from, string emailSubject, string emailMessage, List<EmailUser> users)
		{
			string response = "OK";
			Task t;
			t = Task.Run(() =>
			{
				MailMessage msg = new MailMessage();

				msg.From = new MailAddress(from.Email, from.Name); 
				msg.Subject = emailSubject; 
				msg.IsBodyHtml = true;
				msg.Body = $@"<html><body><h1><font color=""red"">{emailSubject}</font></h1>" + emailMessage +"<br/><br/><b>From: <i>Secretary, Athletics Essendon</i></b><br/><i>Nb:Replies go to the Secretary.</i></body></html>";
				msg.BodyEncoding = System.Text.Encoding.UTF8;

				msg.ReplyToList.Add(from.Email);
				SmtpClient client = new SmtpClient();
				client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential(from.Email, from.Password);
				client.Port = clientPort; 
				client.Host = clientHost; 
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.EnableSsl = true;
				if (users != null)
				{
					foreach (var usr in users)
					{
						msg.Bcc.Add(new MailAddress(usr.Email, usr.Name));
					}
				}
				int count = msg.Bcc.Count;
				try
				{
					client.Send(msg);
					Console.WriteLine("Email/s Successfully Sent");
					System.Diagnostics.Debug.WriteLine($"{count} Emails Successfully Sent by: {fromName}");
					response = $"{count} Emails Successfully Sent From: {fromName}";
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					System.Diagnostics.Debug.WriteLine(ex.Message);
					response = $"Error: {ex.Message}";
				}
			}
			);
			await t;
			return response;
		}
	}
}
