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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity'sinin Role özelliğini string'e çevirmek için
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

       // Assistant-User ilişkilendirmesi
    modelBuilder.Entity<Assistant>()
        .HasOne(a => a.User)
        .WithMany()
        .HasForeignKey(a => a.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Instructor-User ilişkilendirmesi
    modelBuilder.Entity<Instructor>()
        .HasOne(i => i.User)
        .WithMany()
        .HasForeignKey(i => i.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Assistant> Assistants { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Emergency> Emergencies { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<MailEmergency> MailEmergencies { get; set; }
}

