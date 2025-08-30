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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity -> Role relationship (using RoleId foreign key only)
            modelBuilder.Entity<IdentityDto>()
                .HasIndex(i => i.RoleId);

            // Coordinates -> Identity relationship
            modelBuilder.Entity<CoordinatesDto>()
                .HasOne(c => c.Identity)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Verification -> Identity relationship
            modelBuilder.Entity<VerificationDto>()
                .HasOne(v => v.Identity)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // VerificationLogs -> Verification relationship
            modelBuilder.Entity<VerificationLogsDto>()
                .HasOne(vl => vl.Verification)
                .WithMany()
                .HasForeignKey(vl => vl.VerificationId)
                .OnDelete(DeleteBehavior.Cascade);

            // VerificationLogs -> Identity (Admin) relationship
            modelBuilder.Entity<VerificationLogsDto>()
                .HasOne(vl => vl.AdminIdentity)
                .WithMany()
                .HasForeignKey(vl => vl.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ContactVerificationTokens -> Identity relationship
            modelBuilder.Entity<ContactVerificationTokenDto>()
                .HasOne(cvt => cvt.Identity)
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

            // Alert -> Identity relationships (User and Responder)
            modelBuilder.Entity<AlertDto>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlertDto>()
                .HasOne(a => a.Responder)
                .WithMany()
                .HasForeignKey(a => a.ResponderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Chat -> Identity relationships (Sender and Receiver)
            modelBuilder.Entity<ChatDto>()
                .HasOne(c => c.SenderUser)
                .WithMany()
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatDto>()
                .HasOne(c => c.ReceiverUser)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contact -> Identity relationship (optional)
            modelBuilder.Entity<ContactDto>()
                .HasOne(c => c.Identity)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed default roles
            modelBuilder.Entity<RoleDto>().HasData(
                new { Id = 1, Name = "Admin", Description = "System Administrator", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 2, Name = "User", Description = "Regular User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 3, Name = "Ambulance", Description = "Ambulance Personnel", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 4, Name = "PNP", Description = "Philippine National Police", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new { Id = 5, Name = "BFP", Description = "Bureau of Fire Protection", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
