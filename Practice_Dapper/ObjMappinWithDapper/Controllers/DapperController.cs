using Dapper;
using Microsoft.AspNetCore.Mvc;
using ObjMappinWithDapper.Models;
using System.Data.SqlClient;

namespace ObjMappinWithDapper.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DapperController : ControllerBase
{
    private readonly string? _CONNECTION_STRING;
    public DapperController(IConfiguration configuration) => 
        _CONNECTION_STRING = configuration?.GetConnectionString("DefaultConnection");

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var sql = @"SELECT 
	           [Title]
              ,[FirstName]
              ,[LastName]
              ,[Gender]
              FROM [Practice_Dapper1].[dbo].[Person]";

        using (var connection = new SqlConnection(_CONNECTION_STRING))
        {
            var persons = await connection.QueryAsync<Person>(sql);

            return Ok(persons);
        }
    }
}
