using Microsoft.EntityFrameworkCore;
using Craftmatrix.org.Model;
using SIGLAT.API.Model;

namespace Craftmatrix.org.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        // schemas
        public DbSet<IdentityDto> Identity { get; set; }
        public DbSet<CoordinatesDto> Coordinates { get; set; }
        // public DbSet<RoleDto> Roles { get; set; }
        public DbSet<VerificationDto> Verifications { get; set; }
        public DbSet<AlertDto> Alerts { get; set; }
        public DbSet<UserXYZDto> UserXYZ { get; set; }
        public DbSet<ContactDto> Contact { get; set; }
        public DbSet<ChatDto> Chat { get; set; }
        public DbSet<LoginLogsDto> LoginLogs { get; set; }
        public DbSet<ReportDto> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<RoleDto>()
            //                 .HasKey(r => r.Name);

            modelBuilder.Entity<CoordinatesDto>()
                            .HasOne<IdentityDto>()
                            .WithMany()
                            .HasForeignKey(c => c.DriverId)
                            .HasPrincipalKey(i => i.Id)
                            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VerificationDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(v => v.Id)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<AlertDto>()
            //     .HasOne<IdentityDto>()
            //     .WithMany()
            //     .HasForeignKey(a => a.Uid)
            //     .HasPrincipalKey(i => i.Id)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.Entity<AlertDto>()
            //     .HasOne<IdentityDto>()
            //     .WithMany()
            //     .HasForeignKey(a => a.Responder)
            //     .HasPrincipalKey(i => i.Id)
            //     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportDto>()
                .HasOne<IdentityDto>()
                .WithMany()
                .HasForeignKey(r => r.WhoReportedId)
                .HasPrincipalKey(i => i.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
