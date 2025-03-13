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

    public virtual DbSet<AddStudent> AddStudents { get; set; }
    public virtual DbSet<DisplayStudent> DisplayStudent { get; set; }

    public virtual DbSet<TblTraining> TblTraining { get; set; }

    public virtual DbSet<TblTrainingType> TblTrainingTypes { get; set; }

    public virtual DbSet<SearchTraining> SearchTrainings { get; set; }

    public virtual DbSet<SearchTrainingType> SearchTrainingTypes { get; set; }

    public virtual DbSet<TblAssignStudents> TblAssignStudentss { get; set; }

    public virtual DbSet<TblCountAdminDashboard> TblCountAdminDashboards { get; set; }

    public virtual DbSet<TblCountStudentDashboard> TblCountStudentDashboards { get; set; }

    public virtual DbSet<TblUpdateTraining> TblUpdateTraining { get; set; }

    public virtual DbSet<TblConfiguration>  TblConfigurations { get; set; }

    public virtual DbSet<DisplayConfiguration> DisplayConfigurations { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=192.168.150.242;Initial Catalog=Learning_Management_System;User ID=admin;Password=admin;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<SearchTraining>().HasNoKey().ToView(null);
        modelBuilder.Entity<SearchTrainingType>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblTrainingType>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblTraining>().HasKey(t => t.TrainingId);
        modelBuilder.Entity<TblAssignStudents>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblCountAdminDashboard>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblCountStudentDashboard>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblUpdateTraining>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblConfiguration>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayConfiguration>().HasNoKey().ToView(null);

        modelBuilder.Entity<TblTraining>().ToTable("TblTraining"); // Ensure correct table mapping

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
