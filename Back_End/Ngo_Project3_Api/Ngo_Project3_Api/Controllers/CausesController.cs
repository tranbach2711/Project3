using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Data;

namespace Ngo_Project3_Api.Controllers
{
    public class CausesController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _connectionString = "";

        public CausesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        //private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";


        [HttpGet("GetCause")]
        public async Task<IActionResult> GetCauses()
        {
            var causes = new List<Causes>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM causes";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            causes.Add(new Causes
                            {
                                Id = reader.GetInt32("ID"),
                                CauseName = reader.GetString("CAUSE_NAME"),
                                Depcription = reader.GetString("DESCRIPTION"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }
             
            }

            return Ok(causes);
        }


        [HttpPost("CreateCause")]
        public async Task<IActionResult> InsertCause([FromBody] Causes cause)
        {
            Response res = null;
            try
            {

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query2 = "INSERT INTO causes (CAUSE_NAME, DESCRIPTION, CREATE_TIME, UPDATE_TIME) VALUES (@CauseName, @Descreption, @CreateTime, @UpdateTime)";
                    using (var command = new MySqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@CauseName", cause.CauseName);
                        command.Parameters.AddWithValue("@Descreption", cause.Depcription);
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


        [HttpPost("UpdateCause")]
        public async Task<IActionResult> UpdateCause([FromBody] Causes cause)
        {

            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE causes SET CAUSE_NAME = @CauseName, DESCRIPTION = @Descreption, UPDATE_TIME = @UpdateTime WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", cause.Id);
                    command.Parameters.AddWithValue("@CauseName", cause.CauseName);
                    command.Parameters.AddWithValue("@Descreption", cause.Depcription);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Cause not found."
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


        [HttpDelete("causes/{id}")]
        public async Task<IActionResult> DeleteCause(int id)
        {
            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM cause WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Cause not found."
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
}
