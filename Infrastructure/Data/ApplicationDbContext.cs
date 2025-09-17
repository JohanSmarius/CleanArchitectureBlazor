using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBlazor.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Staff> Staff { get; set; } = null!;
        public DbSet<StaffAssignment> StaffAssignments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Event entity
            builder.Entity<Event>()
                .HasMany(e => e.Shifts)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Shift entity
            builder.Entity<Shift>()
                .HasMany(s => s.StaffAssignments)
                .WithOne(sa => sa.Shift)
                .HasForeignKey(sa => sa.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Staff entity
            builder.Entity<Staff>()
                .HasMany(s => s.StaffAssignments)
                .WithOne(sa => sa.Staff)
                .HasForeignKey(sa => sa.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            builder.Entity<Event>()
                .HasIndex(e => e.StartDate);

            builder.Entity<Shift>()
                .HasIndex(s => s.StartTime);

            builder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}
