using DapperASPNetCore.Contracts;
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

                return BadRequest(ex.Message);
            }
        }
    }
}
