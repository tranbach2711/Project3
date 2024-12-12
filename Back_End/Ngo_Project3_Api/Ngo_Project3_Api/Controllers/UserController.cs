using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System;
using System.Data;

namespace Ngo_Project3_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";

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
            }

            return Ok(users);
        }

        // GET: api/User/email
        [HttpGet("{email}")]
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
        }

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
                    command.Parameters.AddWithValue("@Password", request.Password);
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
            }

            return Ok(user);
        }

        
        [HttpPost("CreateUser")]
        public async Task<IActionResult> InsertUser([FromBody] Users user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO users (FULL_NAME, EMAIL,USER_NAME, PASSWORD, ROLE, STATUS, CREATE_TIME, UPDATE_TIME) VALUES (@FullName, @Email, @Username, @Password, @Role, @Status, @CreateTime, @UpdateTime)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", "00");
                    command.Parameters.AddWithValue("@Status", "00");
                    command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPost("CreateUserorAdmin")]
        public async Task<IActionResult> InsertUserAdmin([FromBody] Users user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO users (FULL_NAME, EMAIL,USER_NAME, PASSWORD, ROLE, STATUS, CREATE_TIME, UPDATE_TIME) VALUES (@FullName, @Email, @Username, @Password, @Role, @Status, @CreateTime, @UpdateTime)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Status", "00");
                    command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users user)
        {

            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE users SET FULL_NAME = @FullName, EMAIL = @Email, USER_NAME = @UserName, PASSWORD = @Password, ROLE = @Role, STATUS = @Status, UPDATE_TIME = @UpdateTime WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
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
            }

            res = new Response
            {
                code = "00",
                error = "Success."
            };

            return Ok(res);
        }

        // DELETE: api/User/{id}
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
