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

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]bool getSajjad)
    {
        var sql = new StringBuilder(@"SELECT
               [Id]
	          ,[Title]
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
            var persons = await connection.QueryAsync<Person>(sql.ToString(), dynamicParameters);

            return Ok(persons);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Person person)
    {
        var sql = new StringBuilder(@"INSERT INTO [dbo].[Person] (
                                             [Title], [FirstName], [LastName], [Gender])  VALUES (
                                              @Title, @FirstName, @LastName, @Gender)");

        if (person is not null)
        {
            var dynamicParameters = new DynamicParameters();

            foreach (var item in person.GetType().GetProperties())
            {
                //Console.WriteLine($"{item.Name} , {item.GetValue(person)}");
                dynamicParameters.Add($"{item.Name}", $"{item.GetValue(person)}");
            }

            using (var connection = new SqlConnection(_CONNECTION_STRING))
            {
                var result = await connection.ExecuteAsync(sql.ToString(), dynamicParameters);

                if (result > 0)
                    return Ok($"{result} row is affected");
                else
                    return BadRequest("There have a problem occured in inserting new row");
            }
        }
        else
        {
            return BadRequest("Person can't be null!");
        }
    }
}
