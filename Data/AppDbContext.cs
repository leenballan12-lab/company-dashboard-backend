using Microsoft.EntityFrameworkCore;
using CompanySystemAPI.Models;

namespace CompanySystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<Project> Projects { get; set; }

    }
}