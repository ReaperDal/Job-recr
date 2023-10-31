using Job_Recruitment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Job_Recruitment.Contexts;

public class JobDbContext : IdentityDbContext<User>
{
    public JobDbContext(DbContextOptions contextOptions) : base(contextOptions)
    {

    }

    public DbSet<Application> Applications { get; set; }
    public DbSet<Interview> Interview { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Image> Images { get; set; }
}
