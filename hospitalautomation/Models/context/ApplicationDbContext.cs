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
            const string connectionString = "server=localhost;port=3306;database=HospitalAutomation;user=mainroot;password=616161;Charset=utf8;";

            optionsBuilder
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning);
                });
        }
       

    }

