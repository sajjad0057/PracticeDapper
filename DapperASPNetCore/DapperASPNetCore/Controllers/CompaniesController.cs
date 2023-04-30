using DapperASPNetCore.Contracts;
using DapperASPNetCore.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DapperASPNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRespository;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyRepository companyRespository, ILogger<CompaniesController> logger)
        {
            _companyRespository = companyRespository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _companyRespository.GetCompanies();

                return Ok(companies);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);

                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyRespository.GetCompany(id); 

                return Ok(company);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message,ex.StackTrace);

                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyCreationDto company)
        {
            try
            {
                var createdCompany = await _companyRespository.CreateCompany(company);

                return CreatedAtRoute("CompanyById", new {id = createdCompany.Id }, createdCompany);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message,ex.StackTrace);

                return Problem(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody]CompanyUpdateDto company)
        {
            try
            {
                var dbCompany = await _companyRespository.GetCompany(id);

                if (dbCompany is null)
                    return NotFound();

                await _companyRespository.UpdateCompany(id, company);

                return Ok("Company Updated !");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var dbCompany = await _companyRespository.GetCompany(id);
                if(dbCompany is null)
                    return NotFound();

                await _companyRespository.DeleteCompany(id);

                return Ok("Company deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                return Problem(ex.Message);
            }
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> getCompanyForEmployee(int id)
        {
            try
            {
                var company = await _companyRespository.GetCompanyByEmployeeId(id);

                if (company is null)
                    return NotFound();

                return Ok(company);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            try
            {
                var company = await _companyRespository.GetMultipleResults(id);

                if(company is null)
                    return NotFound();

                return Ok(company);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                return Problem(ex.Message);
            }
        }
    }
}
