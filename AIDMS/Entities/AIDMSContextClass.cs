using Microsoft.EntityFrameworkCore;

namespace AIDMS.Entities
{
    public class AIDMSContextClass:DbContext
    {
        public AIDMSContextClass()
        { }

        public AIDMSContextClass(DbContextOptions<AIDMSContextClass> options) : base(options)
        { }

        // DB Tables 
        public DbSet<Student> Students { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AIDocument> Documents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Handle "Arabic Language" && "DateOnly prop" && "decimal prop" In Db  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Details Including Student & Employee
            modelBuilder.Entity<UserDetails>()
                .Property(u => u.FirstName)
                .IsUnicode(true);

            modelBuilder.Entity<UserDetails>()
                .Property(u => u.LastName)
                .IsUnicode(true);

            modelBuilder.Entity<UserDetails>()
                .Property(e => e.dateOfBirth)
                .HasColumnType("date");

            // Role
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsUnicode(true);

            modelBuilder.Entity<Role>()
                .Property(r => r.Description)
                .IsUnicode(true);

            // Notification
            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .IsUnicode(true);

            // Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // Document 
            modelBuilder.Entity<AIDocument>()
               .Property(d => d.FileName)
               .IsUnicode(true);

            modelBuilder.Entity<AIDocument>()
               .Property(d => d.FileType)
               .IsUnicode(true);

            // Application
            modelBuilder.Entity<Application>()
               .Property(a => a.Title)
               .IsUnicode(true);

            modelBuilder.Entity<Application>()
               .Property(a => a.Status)
               .IsUnicode(true);

            modelBuilder.Entity<Application>()
               .Property(a => a.Description)
               .IsUnicode(true);

        }
    }
}
