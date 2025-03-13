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
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<TblStudent> TblStudents { get; set; }
    public virtual DbSet<AddStudent> AddStudents { get; set; }
    public virtual DbSet<DisplayStudent> DisplayStudents { get; set; }
    public virtual DbSet<AssignTrainings> AssignTrainings { get; set; }
    public virtual DbSet<EditStudentProfile> EditStudentProfiles { get; set; }
    public virtual DbSet<LoginUser> LoginUsers { get; set; }
    public virtual DbSet<ResetPassword> ResetPasswords { get; set; }
    public virtual DbSet<ForgetPassword> ForgetPasswords { get; set; }
    public virtual DbSet<Logo> Logo { get; set; }

    public virtual DbSet<CourseCatalog> CourseCatalogs { get; set; }
    public virtual DbSet<DisplayEnrollment> DisplayEnrollments { get; set; }
    public virtual DbSet<DisplayIDP> DisplayIDPs { get; set; }
    public virtual DbSet<IDPSearching> IDPSearchings { get; set; }
    public virtual DbSet<TrainingTrascriptData> TrainingTrascriptDatas { get; set; }
    public virtual DbSet<TranscriptSearching> TranscriptSearchings { get; set; } 
    public virtual DbSet<TrainingStartModel> TrainingStartModels { get; set; } 
    public virtual DbSet<tbl_Training> Tbl_Training { get; set; } 
    public virtual DbSet<PendingApproval> PendingApprovals { get; set; } 



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=192.168.150.242;Initial Catalog=Learning_Management_System;User ID=admin;Password=admin;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasNoKey().ToView(null);
        modelBuilder.Entity<TblStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayStudent>().HasNoKey().ToView(null);

        modelBuilder.Entity<AddStudent>().HasNoKey().ToView(null);
        modelBuilder.Entity<AssignTrainings>().HasNoKey().ToView(null);
        modelBuilder.Entity<EditStudentProfile>().HasNoKey().ToView(null);
        modelBuilder.Entity<LoginUser>().HasNoKey().ToView(null);
        modelBuilder.Entity<ResetPassword>().HasNoKey().ToView(null);
        modelBuilder.Entity<ForgetPassword>().HasNoKey().ToView(null);
        modelBuilder.Entity<Logo>().HasNoKey().ToView(null);

        modelBuilder.Entity<CourseCatalog>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayEnrollment>().HasNoKey().ToView(null);
        modelBuilder.Entity<DisplayIDP>().HasNoKey().ToView(null);
        modelBuilder.Entity<IDPSearching>().HasNoKey().ToView(null);
        modelBuilder.Entity<TrainingTrascriptData>().HasNoKey().ToView(null);
        modelBuilder.Entity<TranscriptSearching>().HasNoKey().ToView(null); 
        modelBuilder.Entity<TrainingStartModel>().HasNoKey().ToView(null);
        modelBuilder.Entity<PendingApproval>().HasNoKey().ToView(null);

        modelBuilder.Entity<tbl_Training>()
       .HasKey(t => t.training_id);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
