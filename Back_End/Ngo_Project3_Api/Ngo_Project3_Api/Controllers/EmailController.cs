using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ngo_Project3_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";

        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] GetEmail getEmail)
        {

            Response res = null;
            res = new Response
            {
                code = "00",
                error = "Success."
            };

            try
            {
                SendOtp(getEmail.email);
            }
            catch (Exception ex) {
                res = new Response
                {
                    code = "99",
                    error = ex.Message
                };
            }



            return Ok(res);
        }

        public static void SendOtp(string toEmail)
        {
            const string fromEmail = "ttsp@tax24.com.vn"; //requires valid email id
            const string password = "z080G&jK"; // correct password for email id
            Console.WriteLine("TLSEmail Start");

            var smtpClient = new SmtpClient("192.168.1.225")
            {
                Port = 25,
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
                Timeout = 20000
            };

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);

            string SUB = "Invitation to join NGO fundraising";
            string body = "Please go to this link to register an account and experience it : ";

            SendEmail(smtpClient, toEmail, SUB, body);
        }



        public static void SendEmail(SmtpClient smtpClient, string toEmail, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("ttsp@tax24.com.vn");
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

       
                message.To.Add(toEmail);

                smtpClient.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }
        public class GetEmail
        {
            public string email { get; set; }
        }

    }
}
