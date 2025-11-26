using Loan.Domain.Entities;
using Loan.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Loan.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = 1,
                RoleName = RoleType.Admin.ToString(),
            });
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = 2,
                RoleName = RoleType.Accountant.ToString(),
            });
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = 3,
                RoleName = RoleType.User.ToString(),
            });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@loan.com",
                Age = 30,
                Salary = 0,
                Username = "admin",
                Password = PasswordHasher.Hash("Admin123!"),
                IsBlocked = false
            }); 
            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                UserId = 1,
                RoleId = 1
            });

            // დუპლიკატების თავიდან ასაცილებლად კომპოზიტური key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // დეფოლტად IsBlocked იქნება false
            modelBuilder.Entity<User>()
                .Property(u => u.IsBlocked)
                .HasDefaultValue(false);

            // კონფიგურაცია Many-to-Many ურთიერთობისთვის User და Role შორის
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // კონფიგურაცია Many-to-Many ურთიერთობისთვის Role და User შორის
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}
