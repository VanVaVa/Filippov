using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Models;

namespace HRPlatform.Data.Context;

public class HRDbContext : DbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasColumnName("id");
            entity.Property(d => d.Name).HasColumnName("name");
            entity.Property(d => d.Code).HasColumnName("code");
            entity.Property(d => d.ManagerId).HasColumnName("manager_id");
            entity.HasIndex(d => d.Code).IsUnique();
            entity.HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("positions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Title).HasColumnName("title");
            entity.Property(p => p.Level).HasColumnName("level").HasConversion<string>();
            entity.Property(p => p.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.ToTable("vacancies");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).HasColumnName("id");
            entity.Property(v => v.Title).HasColumnName("title");
            entity.Property(v => v.Description).HasColumnName("description");
            entity.Property(v => v.DepartmentId).HasColumnName("department_id");
            entity.Property(v => v.PositionId).HasColumnName("position_id");
            entity.Property(v => v.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(v => v.SalaryRange).HasColumnName("salary_range");
            entity.HasOne(v => v.Department)
                .WithMany(d => d.Vacancies)
                .HasForeignKey(v => v.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(v => v.Position)
                .WithMany(p => p.Vacancies)
                .HasForeignKey(v => v.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("leave_requests");
            entity.HasKey(lr => lr.Id);
            entity.Property(lr => lr.Id).HasColumnName("id");
            entity.Property(lr => lr.EmployeeId).HasColumnName("employee_id");
            entity.Property(lr => lr.StartDate).HasColumnName("start_date");
            entity.Property(lr => lr.EndDate).HasColumnName("end_date");
            entity.Property(lr => lr.Type).HasColumnName("type").HasConversion<string>();
            entity.Property(lr => lr.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(lr => lr.Comment).HasColumnName("comment");
            entity.HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("skills");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id");
            entity.Property(s => s.Name).HasColumnName("name");
            entity.Property(s => s.Category).HasColumnName("category").HasConversion<string>();
        });

        modelBuilder.Entity<EmployeeSkill>(entity =>
        {
            entity.ToTable("employee_skills");
            entity.HasKey(es => new { es.EmployeeId, es.SkillId });
            entity.Property(es => es.EmployeeId).HasColumnName("employee_id");
            entity.Property(es => es.SkillId).HasColumnName("skill_id");
            entity.Property(es => es.ProficiencyLevel).HasColumnName("proficiency_level").HasConversion<string>();
            entity.HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(es => es.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(es => es.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("user_accounts");
            entity.HasKey(ua => ua.Id);
            entity.Property(ua => ua.Id).HasColumnName("id");
            entity.Property(ua => ua.Username).HasColumnName("username");
            entity.Property(ua => ua.HashedPassword).HasColumnName("hashed_password");
            entity.Property(ua => ua.EmployeeId).HasColumnName("employee_id");
            entity.Property(ua => ua.Role).HasColumnName("role").HasConversion<string>();
            entity.HasIndex(ua => ua.Username).IsUnique();
            entity.HasOne(ua => ua.Employee)
                .WithOne(e => e.UserAccount)
                .HasForeignKey<UserAccount>(ua => ua.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("api_keys");
            entity.HasKey(ak => ak.Id);
            entity.Property(ak => ak.Id).HasColumnName("id");
            entity.Property(ak => ak.Key).HasColumnName("key");
            entity.Property(ak => ak.Name).HasColumnName("name");
            entity.Property(ak => ak.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(ak => ak.IsActive).HasColumnName("is_active");
            entity.HasIndex(ak => ak.Key).IsUnique();
        });
    }
}

