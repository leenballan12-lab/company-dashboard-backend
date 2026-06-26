using Microsoft.AspNetCore.Mvc;
using CompanySystemAPI.Data;
using CompanySystemAPI.Models;

namespace CompanySystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Department>> GetDepartments()
        {
            return _context.Departments.ToList();
        }

        [HttpPost]
        public ActionResult<Department> AddDepartment(
            Department department)
        {
            _context.Departments.Add(department);

            _context.SaveChanges();

            return Ok(department);
        }
    }
}