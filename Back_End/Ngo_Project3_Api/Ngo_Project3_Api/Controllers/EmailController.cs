using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Tls;

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
                SendtoEmail(getEmail.email);
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

        public static void SendtoEmail(string toEmail)
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

            string SUB = "Let's help the community together - Join us!";

            string body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
            <p>Dear Mr/Mrs,</p>
            <p>We hope this email finds you well and in good health.</p>
            <p>I would like to introduce myself, I am <strong>Admin</strong>, a representative from the NGO <strong>Give-AID</strong>. We are on a mission to bring hope and support to those who need it most.</p>
            <p>With the support of the community, we have implemented many meaningful programs, including:</p>
            <ul>
                <li><strong>Supporting education:</strong> Bringing knowledge to children in difficult circumstances.</li>
                <li><strong>Health care:</strong> Organizing free medical examinations for remote areas.</li>
                <li><strong>Protecting the environment:</strong> Implementing tree planting and environmental cleaning campaigns.</li>
            </ul>
            <p>However, to continue and expand these efforts, we need the help of people like you. <strong>Every contribution, no matter how small, makes a big difference.</strong></p>
            <p><strong>Please join us in donating today to spread love.</strong></p>
            <p>You can choose:</p>
            <ul>
                <li><strong>Program:</strong> support, donate</li>
                <li><strong>Reason:</strong> Education, Health, Children, Environmental Protection, etc.</li>
            </ul>
            <p><a href='http://localhost:5169/swagger/index.html' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Donate Now</a></p>
            <p>If you have any questions or would like to participate in the organization's activities, please do not hesitate to contact us via this email or call the hotline: <strong>0123456789</strong>.</p>
            <p>Thank you for taking the time to read this letter and considering supporting us. Hopefully, together we will do better things for the community.</p>
            <p>Sincerely,</p>
            <p><strong>Admin</strong><br>
            Give-AID Organization<br>
            Email: <a href='mailto:NgoProject3@gmail.com'>NgoProject3@gmail.com</a><br>
            Website: <a href='http://localhost:5169/swagger/index.html'>http://localhost:5169/swagger/index.html</a></p>
        </body>
        </html>";

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
