using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Data;

namespace Ngo_Project3_Api.Controllers
{
    public class NgoController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _connectionString = "";

        public NgoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        //private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";

        // GET: api/Ngo
        [HttpGet("GetNgo")]
        public async Task<IActionResult> GetNgo()
        {
            var ngo = new List<Ngo>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM ngo";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ngo.Add(new Ngo
                            {
                                Id = reader.GetInt32("ID"),
                                Name = reader.GetString("NAME"),
                                Address = reader.GetString("ADDRESS"),
                                Email = reader.GetString("EMAIL"),
                                Phone = reader.GetString("PHONE"),
                                Website = reader.GetString("WEBSITE"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return Ok(ngo);
        }


        [HttpPost("CreateNgo")]
        public async Task<IActionResult> InsertNgo([FromBody] Ngo ngo)
        {
            Response res = null;
            try
            {

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query2 = "INSERT INTO ngo (NAME, ADDRESS,EMAIL,PHONE,WEBSITE, CREATE_TIME, UPDATE_TIME) VALUES (@Name, @Address,@Email,@Phone,@Website, @CreateTime, @UpdateTime)";
                    using (var command = new MySqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@Name", ngo.Name);
                        command.Parameters.AddWithValue("@Address", ngo.Address);
                        command.Parameters.AddWithValue("@Email", ngo.Email);
                        command.Parameters.AddWithValue("@Phone", ngo.Phone);
                        command.Parameters.AddWithValue("@Website", ngo.Website);
                        command.Parameters.AddWithValue("@CreateTime", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                        await command.ExecuteNonQueryAsync();
                    }
                    await connection.CloseAsync();
                }


                res = new Response
                {
                    code = "00",
                    error = "Success."
                };


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


        [HttpPost("UpdateNgo")]
        public async Task<IActionResult> UpdateNgo([FromBody] Ngo ngo)
        {

            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE causes SET NAME = @Name, ADDRESS = @Address,EMAIL = @Email,PHONE = @Phone, WEBSITE = @Website, UPDATE_TIME = @UpdateTime WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ngo.Id);
                    command.Parameters.AddWithValue("@Name", ngo.Name);
                    command.Parameters.AddWithValue("@Address", ngo.Address);
                    command.Parameters.AddWithValue("@Email", ngo.Email);
                    command.Parameters.AddWithValue("@Phone", ngo.Phone);
                    command.Parameters.AddWithValue("@Website", ngo.Website);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Ngo not found."
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


        [HttpDelete("ngo/{id}")]
        public async Task<IActionResult> DeleteNgo(int id)
        {
            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM ngo WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Ngo not found."
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
}
