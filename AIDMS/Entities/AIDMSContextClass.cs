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
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AIDocument> Documents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Supervisor> Supervisors { get; set; }


        // Handle "Arabic Language" && "DateOnly prop" && "decimal prop" In Db  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Student
            modelBuilder.Entity<Student>()
                .Property(s => s.GPA)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Student>()
                .Property(s => s.TotalPassedHours)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Student>()
                .Property(n => n.firstName)
                .IsUnicode(true);
            
            modelBuilder.Entity<Student>()
                .Property(n => n.lastName)
                .IsUnicode(true);

            // Supervisor
            modelBuilder.Entity<Supervisor>()
                .Property(n => n.firstName)
                .IsUnicode(true);

            modelBuilder.Entity<Supervisor>()
                .Property(n => n.lastName)
                .IsUnicode(true);

            // Employee
            modelBuilder.Entity<Employee>()
                .Property(n => n.firstName)
                .IsUnicode(true);

            modelBuilder.Entity<Employee>()
                .Property(n => n.lastName)
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
