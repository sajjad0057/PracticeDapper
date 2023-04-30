using DapperASPNetCore.DTOs;
using DapperASPNetCore.Entities;

namespace DapperASPNetCore.Contracts;

public interface ICompanyRepository
{
    public Task<IList<Company>> GetCompanies();
    public Task<Company> GetCompany(int id);
    public Task<Company> CreateCompany(CompanyCreationDto company);
    public Task UpdateCompany (int id, CompanyUpdateDto company);
    public Task DeleteCompany (int id);
    public Task<Company> GetCompanyByEmployeeId(int id);
    public Task<Company> GetMultipleResults(int id);
    public Task<IList<Company>> MultipleMapping();
    public Task CreateMultipleCompanies(List<CompanyCreationDto> companies);
}
