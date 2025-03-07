using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project_APIs.Models;

public partial class LearningManagementSystemContext : DbContext
{
    public LearningManagementSystemContext()
    {
    }

    public LearningManagementSystemContext(DbContextOptions<LearningManagementSystemContext> options)
        : base(options)
    {
    }

<<<<<<< HEAD
  

    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<AddStudent> AddStudents { get; set; }
    public virtual DbSet<DisplayStudent> DisplayStudents { get; set; }


=======
    public virtual DbSet<AddStudent> AddStudent { get; set; }
    public virtual DbSet<DisplayStudent> DisplayStudent { get; set; }

>>>>>>> 7d9cb62866f0ffd6130bd54396e8938dfeb5c62e

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=192.168.150.242;Initial Catalog=Learning_Management_System;User ID=admin;Password=admin;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
<<<<<<< HEAD
        modelBuilder.Entity<Role>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<AddStudent>().HasNoKey().ToView(null);
=======
        modelBuilder.Entity<AddStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayStudent>().HasNoKey().ToView(null);

>>>>>>> 7d9cb62866f0ffd6130bd54396e8938dfeb5c62e

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
