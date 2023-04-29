using Dapper;
using Microsoft.AspNetCore.Mvc;
using ObjMappinWithDapper.Models;
using System.Data.SqlClient;
using System.Text;

namespace ObjMappinWithDapper.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DapperController : ControllerBase
{
    private readonly string? _CONNECTION_STRING;
    public DapperController(IConfiguration configuration) => 
        _CONNECTION_STRING = configuration?.GetConnectionString("DefaultConnection");

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery]bool getSajjad)
    {
        var sql = new StringBuilder(@"SELECT 
	           [Title]
              ,[FirstName]
              ,[LastName]
              ,[Gender]
              FROM [Practice_Dapper1].[dbo].[Person]");

        var dynamicParameters = new DynamicParameters();

        if (getSajjad)
        {
            sql.Append("WHERE FirstName = @firstName");
            dynamicParameters.Add("firstName", "Sajjad");
        }

        using (var connection = new SqlConnection(_CONNECTION_STRING))
        {
            var persons = await connection.QueryAsync<Person>(sql.ToString(), new { firstName = "Sajjad"});

            return Ok(persons);
        }
    }
}
