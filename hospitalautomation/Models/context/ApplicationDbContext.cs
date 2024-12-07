using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace hospitalautomation.Models.Context;

public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        const string connectionString = "server=localhost;port=3306;database=HospitalAutomation;user=root;password=melisa123!*;Charset=utf8;";

        optionsBuilder
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .EnableSensitiveDataLogging()
            .ConfigureWarnings(warnings =>
            {
                warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning);
            });
    }

    public DbSet<Assistant> assistants { get; set; }
    public DbSet<Instructor> ınstructors { get; set; }
    public DbSet<Department> departments { get; set; }
    public DbSet<Shift> shifts { get; set; }
    public DbSet<Emergency> emergencies { get; set; }

    public DbSet<Interview> ınterviews { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<MailEmergency> mailEmergencies { get; set; }
}

