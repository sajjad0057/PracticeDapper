using Dapper;
using DapperASPNetCore.Context;
using DapperASPNetCore.Contracts;
using DapperASPNetCore.DTOs;
using DapperASPNetCore.Entities;
using System.Data;

namespace DapperASPNetCore.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DapperContext _context;

	public CompanyRepository(DapperContext context) => _context = context;

    public async Task<Company> CreateCompany(CompanyCreationDto company)
    {
        var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country);" + 
            "SELECT CAST(SCOPE_IDENTITY() AS int);";

        var parameters = new DynamicParameters();
        parameters.Add("Name", company.Name, DbType.String);
        parameters.Add("Address", company.Address, DbType.String);
        parameters.Add("Country", company.Country, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QuerySingleAsync<int>(query, parameters);

            var createdCompany = new Company
            {
                Id = id,
                Name = company.Name,
                Address = company.Address,
                Country = company.Country,
            };

            return createdCompany;
        }
    }

    public async Task<IList<Company>> GetCompanies()
    {
        var query = "SELECT * FROM Companies";

        using (var connection = _context.CreateConnection())
        {
            var companies = await connection.QueryAsync<Company>(query);

            return companies.ToList();
        }
    }

    public async Task<Company> GetCompany(int id)
    {
        var query = "SELECT * FROM Companies WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("Id", id);

            var company = await connection.QuerySingleOrDefaultAsync<Company>(query, dynamicParams);

            return company;
        }
    }

    public async Task UpdateCompany(int id, CompanyUpdateDto company)
    {
        var query = "UPDATE Companies SET Name = @Name, Address = @Address, Country = @Country WHERE Id = @Id";

        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int64);
        parameters.Add("Name", company.Name, DbType.String);
        parameters.Add("Address", company.Address, DbType.String);
        parameters.Add("Country", company.Country, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }

    }

    public async Task DeleteCompany(int id)
    {
        var query = "DELETE FROM Companies WHERE Id = @Id";

        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int64);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<Company> GetCompanyByEmployeeId(int id)
    {
        var procedureName = "ShowCompanyByEmployeeId";
        var parameters = new DynamicParameters();
        parameters.Add("Id",id, DbType.Int64, ParameterDirection.Input);

        using(var connection = _context.CreateConnection())
        {
            var company = await connection.QueryFirstOrDefaultAsync<Company>
                (procedureName, parameters, commandType: CommandType.StoredProcedure);

            return company;
        }
    }

    public async Task<Company> GetMultipleResults(int id)
    {
        var query = "SELECT * FROM Companies WHERE Id = @Id;" +
            "SELECT * FROM Employees WHERE CompanyId = @Id;";

        using (var connection = _context.CreateConnection())
        using (var multi = await connection.QueryMultipleAsync(query, new { id }))
        {
            var company = await multi.ReadSingleOrDefaultAsync<Company>();

            if(company is not null)
            {
                company.Employees = (await multi.ReadAsync<Employee>()).ToList();
            }

            return company?? new Company();
        }
    }

    public async Task<IList<Company>> MultipleMapping()
    {
        var query = "SELECT * FROM Companies c join Employees e ON c.Id = e.CompanyId;";

        using (var connection = _context.CreateConnection())
        {
            var companyDict = new Dictionary<int, Company>();

            var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if(!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(company.Id, currentCompany);
                        }

                        currentCompany.Employees.Add(employee);

                        return currentCompany;
                    }
                );

            return companyDict.Values.ToList();
        }
    }
}
