using Microsoft.AspNetCore.Mvc;

using CompanySystemAPI.Data;
using CompanySystemAPI.Models;

namespace CompanySystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(
            AppDbContext context
        )
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Attendance>>
            GetAttendance()
        {
            return _context.Attendance.ToList();
        }

        [HttpPost]
        public ActionResult<Attendance>
            AddAttendance(
                Attendance attendance
            )
        {
            _context.Attendance.Add(attendance);

            _context.SaveChanges();

            return Ok(attendance);
        }

     [HttpPut("{id}")]
public IActionResult UpdateAttendance(
    int id,
    Attendance updated
)
{
    var attendance =
        _context.Attendance.Find(id);

    if (attendance == null)
    {
        return NotFound();
    }

    attendance.Name = updated.Name;

    attendance.Status = updated.Status;

    _context.SaveChanges();

    return Ok(attendance);
}
    }
}