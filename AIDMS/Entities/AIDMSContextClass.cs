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
        //public DbSet<Supervisor> Supervisors { get; set; }


        // Handle "Arabic Language" && "DateOnly prop" && "decimal props" && Nulls(for deleted props) In Db  
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

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(a => a.Students)
                .HasForeignKey(f => f.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Student>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.Student)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Notifications)
                .WithOne(a => a.Student)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Documents)
                .WithOne(a => a.Student)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee
            modelBuilder.Entity<Employee>()
                .Property(n => n.firstName)
                .IsUnicode(true);

            modelBuilder.Entity<Employee>()
                .Property(n => n.lastName)
                .IsUnicode(true);

            modelBuilder.Entity<Employee>()
            .HasOne(a => a.Role)
            .WithMany(s => s.Employees)
            .HasForeignKey(a => a.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
            .HasMany(a => a.Applications)
            .WithOne(s => s.Employee)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
            .HasMany(a => a.Notifications)
            .WithOne(s => s.Employee)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
            .HasOne(a => a.Role)
            .WithMany(s => s.Employees)
            .HasForeignKey(a => a.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

            // Notification
            modelBuilder.Entity<Notification>()
                .Property(n => n.Message)
                .IsUnicode(true);

            modelBuilder.Entity<Notification>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Notifications)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
            .HasOne(a => a.Employee)
            .WithMany(s => s.Notifications)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

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

            modelBuilder.Entity<AIDocument>()
              .HasOne(a => a.Student)
              .WithMany(s => s.Documents)
              .HasForeignKey(a => a.StudentId)
              .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AIDocument>()
              .HasOne(a => a.Application)
              .WithMany(s => s.Documents)
              .HasForeignKey(a => a.ApplicationId)
              .OnDelete(DeleteBehavior.SetNull);


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

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Applications)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Employee)
                .WithMany(s => s.Applications)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Application>()
               .HasMany(a => a.Documents)
               .WithOne(s => s.Application)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Application>()
            .HasOne(p => p.Payment)
            .WithOne(a => a.Application) 
            .HasForeignKey<Application>(a => a.PaymentId); 
        }

    }
}
