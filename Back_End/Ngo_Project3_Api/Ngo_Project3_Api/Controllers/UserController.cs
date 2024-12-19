using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Middleware;
using Ngo_Project3_Api.Model;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Ngo_Project3_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _connectionString = "";

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        //private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = new List<Users>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM users";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new Users
                            {
                                Id = reader.GetInt32("ID"),
                                FullName = reader.GetString("FULL_NAME"),
                                UserName = reader.GetString("USER_NAME"),
                                Email = reader.GetString("EMAIL"),
                                Role = reader.GetString("ROLE"),
                                Status = reader.GetString("STATUS"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return Ok(users);
        }

        // GET: api/User/email
/*        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            Users user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM users WHERE EMAIL = @email";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new Users
                            {
                                Id = reader.GetInt32("ID"),
                                FullName = reader.GetString("FULL_NAME"),
                                UserName = reader.GetString("USER_NAME"),
                                Email = reader.GetString("EMAIL"),
                                Role = reader.GetString("ROLE"),
                                Status = reader.GetString("STATUS"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            };
                        }
                    }
                }
            }

            return Ok(user);
        }*/

        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Users user = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM users WHERE USER_NAME = @user AND PASSWORD = @Password AND STATUS = '00'";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user", request.User);
                    command.Parameters.AddWithValue("@Password", MD5Encryption.EncryptMD5(request.Password));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new Users
                            {
                                Id = reader.GetInt32("ID"),
                                FullName = reader.GetString("FULL_NAME"),
                                UserName = reader.GetString("USER_NAME"),
                                Email = reader.GetString("EMAIL"),
                                Role = reader.GetString("ROLE")
                            };
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return Ok(user);
        }

        
        [HttpPost("CreateUser")]
        public async Task<IActionResult> InsertUser([FromBody] Users user)
        {
            Response res = null;
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT count(*) FROM users WHERE USER_NAME = @user ";
                    using (var command1 = new MySqlCommand(query, connection))
                    {
                        command1.Parameters.AddWithValue("@user", user.UserName);
                        var count = Convert.ToInt32(await command1.ExecuteScalarAsync());
                        if (count > 0)
                        {
                            // User đã tồn tại
                            res = new Response
                            {
                                code = "99",
                                error = "User already exists."
                            };
                            Console.WriteLine("User already exists.");
                        }
                        else
                        {
                            using (connection)
                            {
                                await connection.OpenAsync();
                                string query2 = "INSERT INTO users (FULL_NAME, EMAIL,USER_NAME, PASSWORD, ROLE, STATUS, CREATE_TIME, UPDATE_TIME) VALUES (@FullName, @Email, @Username, @Password, @Role, @Status, @CreateTime, @UpdateTime)";
                                using (var command = new MySqlCommand(query2, connection))
                                {
                                    command.Parameters.AddWithValue("@FullName", user.FullName);
                                    command.Parameters.AddWithValue("@Email", user.Email);
                                    command.Parameters.AddWithValue("@Username", user.UserName);
                                    command.Parameters.AddWithValue("@Password", MD5Encryption.EncryptMD5(user.Password));
                                    command.Parameters.AddWithValue("@Role", "00");
                                    command.Parameters.AddWithValue("@Status", "00");
                                    command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            res = new Response
                            {
                                code = "00",
                                error = "Success."
                            };
                        }
                    }
                    await connection.CloseAsync();
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

        [HttpPost("CreateUserorAdmin")]
        public async Task<IActionResult> InsertUserAdmin([FromBody] Users user)
        {
            Response res = null;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT count(*) FROM users WHERE USER_NAME = @user ";
                    using (var command1 = new MySqlCommand(query, connection))
                    {
                        command1.Parameters.AddWithValue("@user", user.UserName);
                        var count = Convert.ToInt32(await command1.ExecuteScalarAsync());
                        if (count > 0)
                        {
                            // User đã tồn tại
                            res = new Response
                            {
                                code = "99",
                                error = "User already exists."
                            };
                            Console.WriteLine("User already exists.");
                        }
                        else
                        {
                            using (connection)
                            {
                                await connection.OpenAsync();
                                string query2 = "INSERT INTO users (FULL_NAME, EMAIL,USER_NAME, PASSWORD, ROLE, STATUS, CREATE_TIME, UPDATE_TIME) VALUES (@FullName, @Email, @Username, @Password, @Role, @Status, @CreateTime, @UpdateTime)";
                                using (var command = new MySqlCommand(query2, connection))
                                {
                                    command.Parameters.AddWithValue("@FullName", user.FullName);
                                    command.Parameters.AddWithValue("@Email", user.Email);
                                    command.Parameters.AddWithValue("@Username", user.UserName);
                                    command.Parameters.AddWithValue("@Password", MD5Encryption.EncryptMD5(user.Password));
                                    command.Parameters.AddWithValue("@Role", user.Role);
                                    command.Parameters.AddWithValue("@Status", "00");
                                    command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            res = new Response
                            {
                                code = "00",
                                error = "Success."
                            };
                        }
                    }
                    await connection.CloseAsync();
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

        // PUT: api/User/{id}
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] Users user)
        {

            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE users SET FULL_NAME = @FullName, EMAIL = @Email, USER_NAME = @UserName, ROLE = @Role, STATUS = @Status, UPDATE_TIME = @UpdateTime WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Email", user.Email);
//                    command.Parameters.AddWithValue("@Password", EncryptMD5(user.Password));
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Status", user.Status);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "User not found."
                        };

                        return Ok(res);
                    }
                }
                await connection.CloseAsync();
            }

            res = new Response
            {
                code = "00",
                error = "Success."
            };

            return Ok(res);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM users WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "User not found."
                        };

                        return Ok(res);
                    }
                }
                await connection.CloseAsync();
            }

            res = new Response
            {
                code = "00",
                error = "Success."
            };

            return Ok(res);
        }

    }

    
    public class LoginRequest
    {
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class Response
    {
        public string code { get; set; }
        public string error { get; set; }
    }

}
