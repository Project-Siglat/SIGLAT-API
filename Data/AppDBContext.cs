using Microsoft.EntityFrameworkCore;
using Craftmatrix.org.API.Models;

namespace Craftmatrix.org.API.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        // schemas
        public DbSet<IdentityDto> Identity { get; set; }
        public DbSet<RoleDto> Roles { get; set; }
        public DbSet<CoordinatesDto> Coordinates { get; set; }
        public DbSet<VerificationDto> Verifications { get; set; }
        public DbSet<VerificationLogsDto> VerificationLogs { get; set; }
        public DbSet<ContactVerificationTokenDto> ContactVerificationTokens { get; set; }
        public DbSet<AlertDto> Alerts { get; set; }
        public DbSet<ContactDto> Contact { get; set; }
        public DbSet<ChatDto> Chat { get; set; }
        public DbSet<ReportDto> Reports { get; set; }
        public DbSet<UserLoginTrackingDto> UserLoginTracking { get; set; }
        public DbSet<RefreshTokenDto> RefreshTokens { get; set; }
        public DbSet<VerificationTypeDto> VerificationTypes { get; set; }
        public DbSet<AccountVerificationDto> AccountVerifications { get; set; }
        public DbSet<TypeOfIncidentDto> TypeOfIncidents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity -> Role relationship (using RoleId foreign key only)
            modelBuilder.Entity<IdentityDto>()
                .HasIndex(i => i.RoleId);

            // Coordinates -> Identity relationship
            modelBuilder.Entity<CoordinatesDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Verification -> Identity relationship
            modelBuilder.Entity<VerificationDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // VerificationLogs -> Verification relationship
            modelBuilder.Entity<VerificationLogsDto>()
                .HasOne<VerificationDto>()
                .WithMany()
                .HasForeignKey(vl => vl.VerificationId)
                .OnDelete(DeleteBehavior.Cascade);

            // VerificationLogs -> Identity (Admin) relationship
            modelBuilder.Entity<VerificationLogsDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(vl => vl.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ContactVerificationTokens -> Identity relationship
            modelBuilder.Entity<ContactVerificationTokenDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(cvt => cvt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Report -> Identity relationship
            modelBuilder.Entity<ReportDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(r => r.WhoReportedId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserLoginTracking configuration
            modelBuilder.Entity<UserLoginTrackingDto>()
                .HasKey(ult => ult.Id);

            modelBuilder.Entity<UserLoginTrackingDto>()
                .ToTable("UserLoginTracking");

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.UserId)
                .IsRequired();

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.IpAddress)
                .HasMaxLength(45)
                .IsRequired();

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.UserAgent)
                .HasMaxLength(500)
                .IsRequired(false);

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.LoginTimestamp)
                .IsRequired();

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.LogoutTimestamp)
                .IsRequired(false);

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.LoginStatus)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.FailureReason)
                .HasMaxLength(200)
                .IsRequired(false);

            modelBuilder.Entity<UserLoginTrackingDto>()
                .Property(ult => ult.AttemptedEmail)
                .HasMaxLength(255)
                .IsRequired(false);

            // RefreshToken configuration
            modelBuilder.Entity<RefreshTokenDto>()
                .HasKey(rt => rt.Id);

            modelBuilder.Entity<RefreshTokenDto>()
                .ToTable("RefreshTokens");

            modelBuilder.Entity<RefreshTokenDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshTokenDto>()
                .Property(rt => rt.Token)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<RefreshTokenDto>()
                .Property(rt => rt.IpAddress)
                .HasMaxLength(45);

            modelBuilder.Entity<RefreshTokenDto>()
                .Property(rt => rt.UserAgent)
                .HasMaxLength(500);

            // Alert -> Identity relationships (User and Responder)
            modelBuilder.Entity<AlertDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlertDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(a => a.ResponderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Chat -> Identity relationships (Sender and Receiver)
            modelBuilder.Entity<ChatDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contact -> Identity relationship (optional)
            modelBuilder.Entity<ContactDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // VerificationTypes configuration
            modelBuilder.Entity<VerificationTypeDto>()
                .HasKey(vt => vt.Id);

            modelBuilder.Entity<VerificationTypeDto>()
                .ToTable("VerificationTypes");

            modelBuilder.Entity<VerificationTypeDto>()
                .Property(vt => vt.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<VerificationTypeDto>()
                .Property(vt => vt.Description)
                .HasMaxLength(200);

            // AccountVerifications configuration
            modelBuilder.Entity<AccountVerificationDto>()
                .HasKey(av => av.Id);

            modelBuilder.Entity<AccountVerificationDto>()
                .ToTable("AccountVerifications");

            // AccountVerifications -> Identity (User) relationship
            modelBuilder.Entity<AccountVerificationDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(av => av.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AccountVerifications -> VerificationTypes relationship
            modelBuilder.Entity<AccountVerificationDto>()
                .HasOne<VerificationTypeDto>()
                .WithMany()
                .HasForeignKey(av => av.VerificationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // AccountVerifications -> Identity (Reviewer) relationship
            modelBuilder.Entity<AccountVerificationDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(av => av.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AccountVerificationDto>()
                .Property(av => av.DocumentNumber)
                .HasMaxLength(200);

            modelBuilder.Entity<AccountVerificationDto>()
                .Property(av => av.DocumentName)
                .HasMaxLength(100);

            modelBuilder.Entity<AccountVerificationDto>()
                .Property(av => av.ImageMimeType)
                .HasMaxLength(50);

            modelBuilder.Entity<AccountVerificationDto>()
                .Property(av => av.AdminNotes)
                .HasMaxLength(500);

            modelBuilder.Entity<AccountVerificationDto>()
                .Property(av => av.Status)
                .HasMaxLength(20)
                .IsRequired();

            // TypeOfIncident configuration
            modelBuilder.Entity<TypeOfIncidentDto>()
                .HasKey(toi => toi.Id);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .ToTable("TypeOfIncidents");

            // TypeOfIncident -> Identity (WhoAddedIt) relationship
            modelBuilder.Entity<TypeOfIncidentDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(toi => toi.WhoAddedItID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.NameOfIncident)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.AddedDateTime)
                .IsRequired();

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.isBFPTrue)
                .HasDefaultValue(false);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.isPNPTrue)
                .HasDefaultValue(false);

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<TypeOfIncidentDto>()
                .Property(toi => toi.UpdatedAt)
                .IsRequired();

            // Seed default roles
            modelBuilder.Entity<RoleDto>().HasData(
                new { Id = 1, Name = "Admin", Description = "System Administrator", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 2, Name = "User", Description = "Regular User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 3, Name = "Ambulance", Description = "Ambulance Personnel", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 4, Name = "PNP", Description = "Philippine National Police", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 5, Name = "BFP", Description = "Bureau of Fire Protection", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );

            // Seed default verification types
            modelBuilder.Entity<VerificationTypeDto>().HasData(
                new { Id = 1, Name = "Passport", Description = "Philippine Passport", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 2, Name = "National ID", Description = "Philippine National ID (PhilID)", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 3, Name = "Student ID", Description = "Valid Student ID from Educational Institution", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
