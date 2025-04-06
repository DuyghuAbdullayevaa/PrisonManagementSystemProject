using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Data
{
    public class PrisonDbContext : IdentityDbContext<User, Role, string>
    {
        // Constructor to initialize DbContext with options
        public PrisonDbContext(DbContextOptions<PrisonDbContext> options)
            : base(options)
        {
        }

        // Define DbSet properties for your entities
        public DbSet<Prisoner> Prisoners { get; set; }
        public DbSet<PrisonerCrime> PrisonerCrimes { get; set; }
        public DbSet<Prison> Prisons { get; set; }
        public DbSet<PrisonerIncident> PrisonerIncidents { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<PrisonStaff> PrisonStaffs { get; set; }
        public DbSet<IncidentPunishment> IncidentPunishments { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Punishment> Punishments { get; set; }
        public DbSet<Crime> Crimes { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PrisonerPunishment> PrisonerPunishments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<IncidentCell> IncidentCells { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Override OnModelCreating method to apply configurations and seed data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Generate a constant ID for seeding 
            var adminId = "b1a7c630-88e0-4fbd-a8a1-0123456789ab";

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    UserName = "admin@prison.com",
                    NormalizedUserName = "ADMIN@PRISON.COM",
                    Email = "admin@prison.com",
                    NormalizedEmail = "ADMIN@PRISON.COM",
                    FirstName = "Admin",
                    LastName = "User",
                    BirthDate = new DateTime(1990, 1, 1),
                    PhoneNumber = "+994501234567", 
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "12345677Aa!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType.IsEnum)
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Name)
                            .HasConversion<string>();
                    }
                }
            }
        }

    
}
}
