using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ngo_Project3_Api.Model;
using System.Data;

namespace Ngo_Project3_Api.Controllers
{
    public class ProgramsController : Controller
    {
        private readonly IConfiguration _configuration;
        private string _connectionString = "";

        public ProgramsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
    
        //private readonly string _connectionString = "Server=localhost;Port=3306;Database=sys;User=root;Password=ngo_project3;";


        [HttpGet("GetProgram")]
        public async Task<IActionResult> GetPrograms()
        {
            var program = new List<Programs>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM programs";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            program.Add(new Programs
                            {
                                Id = reader.GetInt32("ID"),
                                ProgramName = reader.GetString("PROGRAM_NAME"),
                                Depcription = reader.GetString("DESCRIPTION"),
                                NgoId = reader.GetInt32("NGO_ID"),
                                CreateTime = reader.GetDateTime("CREATE_TIME"),
                                UpdateTime = reader.GetDateTime("UPDATE_TIME")
                            });
                        }
                    }
                }

            }

            return Ok(program);
        }


        [HttpPost("CreatePrograms")]
        public async Task<IActionResult> InsertProgram([FromBody] Programs program)
        {
            Response res = null;
            try
            {

                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query2 = "INSERT INTO programs (PROGRAM_NAME, DESCRIPTION,NGO_ID, CREATE_TIME, UPDATE_TIME) VALUES (@ProgramName, @Descreption,@NgoId, @CreateTime, @UpdateTime)";
                    using (var command = new MySqlCommand(query2, connection))
                    {
                        command.Parameters.AddWithValue("@ProgramName", program.ProgramName);
                        command.Parameters.AddWithValue("@Descreption", program.Depcription);
                        command.Parameters.AddWithValue("@NgoId", program.NgoId);
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


        [HttpPost("UpdateProgram")]
        public async Task<IActionResult> UpdateProgram([FromBody] Programs program)
        {

            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE programs SET PROGRAM_NAME = @ProgramName, DESCRIPTION = @Descreption,NGO_ID = @NgoId, UPDATE_TIME = @UpdateTime WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProgramName", program.ProgramName);
                    command.Parameters.AddWithValue("@Descreption", program.Depcription);
                    command.Parameters.AddWithValue("@NgoId", program.NgoId);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.UtcNow);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Program not found."
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


        [HttpDelete("progarm/{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            Response res = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM programs WHERE ID = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        res = new Response
                        {
                            code = "99",
                            error = "Program not found."
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
