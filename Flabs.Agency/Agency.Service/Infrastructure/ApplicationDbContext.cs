using Agency.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agency.Service.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Appointment>  Appointments { get; set; }
    public DbSet<Holiday>  Holidays { get; set; }
}