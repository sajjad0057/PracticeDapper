using Dapper;
using DapperASPNetCore.Context;
using DapperASPNetCore.Contracts;
using DapperASPNetCore.Entities;

namespace DapperASPNetCore.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DapperContext _context;

	public CompanyRepository(DapperContext context) => _context = context;

    public async Task<IList<Company>> GetCompanies()
    {
        var query = "SELECT * FROM Companies";

        using (var connection = _context.CreateConnection())
        {
            var companies = await connection.QueryAsync<Company>(query);

            return companies.ToList();
        }
    }
}
