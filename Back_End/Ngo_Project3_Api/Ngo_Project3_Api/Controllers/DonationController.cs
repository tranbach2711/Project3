using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Data;
using System.Net.Mail;
using System.Net.Security;
using System.Net;

namespace Ngo_Project3_Api.Controllers
{
    public class DonationController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _connectionString = "";

        public DonationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        //private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";

        [HttpGet("GetAllDonation")]
        public async Task<IActionResult> GetDonation()
        {
            var donation = new List<Donations>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT d.*,  p.PROGRAM_NAME AS PROGRAM_NAME,  c.CAUSE_NAME AS CAUSE_NAME,  u.FULL_NAME AS USER_NAME " +
                    "FROM sys.donations d INNER JOIN  sys.programs p ON d.PROGRAM_ID = p.ID " +
                    "INNER JOIN sys.causes c ON d.CAUSE_ID = c.ID " +
                    "INNER JOIN sys.users u ON d.USER_ID = u.ID";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            donation.Add(new Donations
                            {
                                Id = reader.GetInt32("ID"),
                                ProgramName = reader.GetString("PROGRAM_NAME"),
                                CauseName = reader.GetString("CAUSE_NAME"),
                                UserName = reader.GetString("USER_NAME"),
                                ProgramId = reader.GetInt32("PROGRAM_ID"),
                                CauseId = reader.GetInt32("CAUSE_ID"),
                                UserId = reader.GetInt32("USER_ID"),
                                DonationAmount = reader.GetDecimal("DONATION_AMOUNT"),
                                PaymentMethod = reader.GetString("PAYMENT_METHOD"),
                                AccNumber = reader.GetString("ACC_NUMBER"),
                                PaymentStatus = reader.GetString("PAYMENT_STATUS"),
                                PaymentDate = reader.GetDateTime("PAYMENT_DATE"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }
             
            }

            return Ok(donation);
        }

        [HttpGet("GetDonationUser")]
        public async Task<IActionResult> GetDonationUser(int Id)
        {
            var donation = new List<Donations>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT d.*,  p.PROGRAM_NAME AS PROGRAM_NAME,  c.CAUSE_NAME AS CAUSE_NAME,  u.FULL_NAME AS USER_NAME " +
                    "FROM donations d INNER JOIN  programs p ON d.PROGRAM_ID = p.ID " +
                    "INNER JOIN causes c ON d.CAUSE_ID = c.ID " +
                    "INNER JOIN users u ON d.USER_ID = u.ID where d.USER_ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            donation.Add(new Donations
                            {
                                Id = reader.GetInt32("ID"),
                                ProgramName = reader.GetString("PROGRAM_NAME"),
                                CauseName = reader.GetString("CAUSE_NAME"),
                                UserName = reader.GetString("USER_NAME"),
                                ProgramId = reader.GetInt32("PROGRAM_ID"),
                                CauseId = reader.GetInt32("CAUSE_ID"),
                                UserId = reader.GetInt32("USER_ID"),
                                DonationAmount = reader.GetDecimal("DONATION_AMOUNT"),
                                PaymentMethod = reader.GetString("PAYMENT_METHOD"),
                                AccNumber = reader.GetString("ACC_NUMBER"),
                                PaymentStatus = reader.GetString("PAYMENT_STATUS"),
                                PaymentDate = reader.GetDateTime("PAYMENT_DATE"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }

            }

            return Ok(donation);
        }

        [HttpPost("CreateDonation")]
        public async Task<IActionResult> InsertDonation([FromBody] Donations donation)
        {
            Response res = null;
            try
            {

                bool pay = Payment(donation.DonationAmount, donation.AccNumber);


                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query2 = "INSERT INTO donations (PROGRAM_ID, CAUSE_ID, USER_ID, DONATION_AMOUNT, PAYMENT_METHOD, ACC_NUMBER, PAYMENT_STATUS, PAYMENT_DATE,CREATE_TIME,UPDATE_TIME) " +
                        "VALUES (@ProgramId, @CauseId, @UserId, @DonationAmount, @PaymentMethod, @AccNumber, @PaymentStatus, @PaymentDate,  @CreateTime, @UpdateTime)";
                    using (var command = new MySqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@ProgramId", donation.ProgramId);
                        command.Parameters.AddWithValue("@CauseId", donation.CauseId);
                        command.Parameters.AddWithValue("@UserId", donation.UserId);
                        command.Parameters.AddWithValue("@DonationAmount", donation.DonationAmount);
                        command.Parameters.AddWithValue("@PaymentMethod", donation.PaymentMethod);
                        command.Parameters.AddWithValue("@AccNumber", donation.AccNumber);
                        if (pay)
                        {
                            command.Parameters.AddWithValue("@PaymentStatus", "Success");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@PaymentStatus", "Fail");
                        }
                        
                        
                        command.Parameters.AddWithValue("@PaymentDate", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                

                if (pay)
                {

                    string email = null;
                    string name = null;

                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        string query = "SELECT * FROM users WHERE ID = @id";
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", donation.UserId);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {

                                    email = reader.GetString("EMAIL");
                                    name = reader.GetString("FULL_NAME");
                                }
                            }
                        }
                    }

                    if(email != null && !"".Equals(email) && name != null && !"".Equals(name))
                    {
                        SendtoEmail(email, name, donation.DonationAmount.ToString(), donation.PaymentMethod);
                    }

                    res = new Response
                    {
                        code = "00",
                        error = "Success."
                    };
                }
                else
                {
                    res = new Response
                    {
                        code = "99",
                        error = "Payment fail."
                    };
                }

            }
            catch (Exception ex)
            {
                res = new Response
                {
                    code = "99",
                    error = ex.Message
                };
            }



            return Ok(res);
        }


        public bool Payment(decimal amount, string Acc)
        {
            bool pay = true;

            try
            {
                if(amount > 0 && "0123456789".Equals(Acc))
                {

                }
                else
                {
                    pay = false;
                }


            }catch (Exception ex)
            {
                pay = false;
            }

            return pay;
        }

        public static void SendtoEmail(string toEmail,string name,string donationAmount, string paymentMethod)
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

            string SUB = "Thank You for Your Generous Donation!";

            string body = $@"
        <p>Dear <strong>{name}</strong>,</p>

        <p>We hope this message finds you well and in good health.</p>

        <p>On behalf of <strong>Give-AID Organization</strong>, we would like to express our heartfelt gratitude for your generous donation. Your contribution has been successfully received.</p>

        <h3>Donation Details:</h3>
        <ul>

            <li><strong>Amount Donated:</strong> ${donationAmount}</li>
            <li><strong>Payment Method:</strong> {paymentMethod}</li>
            <li><strong>Date:</strong> {DateTime.UtcNow.ToString("yyyy-MM-dd")}</li>
        </ul>

        <p>Your support will help us to continue our mission of:</p>
        <ul>
            <li>Supporting education for underprivileged children.</li>
            <li>Providing healthcare services to remote communities.</li>
            <li>Protecting the environment through various initiatives.</li>
        </ul>

        <p>Once again, thank you for spreading love and hope. If you would like to learn more about our upcoming programs or participate in our activities, feel free to visit our website or reach out to us.</p>

        <p><strong>Contact Us:</strong><br>
        Website: <a href='http://localhost:5169/swagger/index.html'>Give-AID Website</a><br>
        Email: NgoProject3@gmail.com<br>
        Hotline: 0123456789</p>

        <p>Together, we are making the world a better place.</p>

        <p>Sincerely,<br>
        <strong>Give-AID Organization</strong></p>";

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
    }
}
