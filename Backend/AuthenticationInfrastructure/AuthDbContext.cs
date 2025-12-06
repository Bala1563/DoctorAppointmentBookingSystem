using AuthenticationDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuthenticationInfrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============================================
            // User Configuration
            // =============================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                entity.HasIndex(e => e.UserType)
                    .HasDatabaseName("IX_Users_UserType");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.UserType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Users_UserType",
                        "[UserType] IN ('Patient', 'Doctor', 'Admin')");
                });
            });

            // =============================================
            // Patient Configuration
            // =============================================
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId);

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_Patients_UserId");

                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasDatabaseName("IX_Patients_Name");

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Patient)
                    .HasForeignKey<Patient>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Patients_Gender",
                        "[Gender] IN ('Male', 'Female', 'Other')");
                });
            });

            // =============================================
            // Specialization Configuration
            // =============================================
            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.HasKey(e => e.SpecializationId);

                entity.HasIndex(e => e.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Specializations_Name");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // Doctor Configuration
            // =============================================
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.DoctorId);

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctors_UserId");

                entity.HasIndex(e => e.LicenseNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctors_LicenseNumber");

                entity.HasIndex(e => e.SpecializationId)
                    .HasDatabaseName("IX_Doctors_SpecializationId");

                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasDatabaseName("IX_Doctors_Name");

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Doctor)
                    .HasForeignKey<Doctor>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Specialization)
                    .WithMany(s => s.Doctors)
                    .HasForeignKey(e => e.SpecializationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.ConsultationFee)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Rating)
                    .HasColumnType("decimal(3,2)")
                    .HasDefaultValue(0.00m);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // RefreshToken Configuration
            // =============================================
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_RefreshTokens_UserId");

                entity.HasIndex(e => e.Token)
                    .HasDatabaseName("IX_RefreshTokens_Token");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // EmailVerificationToken Configuration
            // =============================================
            modelBuilder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_EmailVerificationTokens_UserId");

                entity.HasIndex(e => e.Token)
                    .HasDatabaseName("IX_EmailVerificationTokens_Token");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // PasswordResetToken Configuration
            // =============================================
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_PasswordResetTokens_UserId");

                entity.HasIndex(e => e.Token)
                    .HasDatabaseName("IX_PasswordResetTokens_Token");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // AuditLog Configuration
            // =============================================
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_AuditLogs_UserId");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_AuditLogs_CreatedAt");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =============================================
            // Seed Data
            // =============================================
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Specializations
            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { SpecializationId = 1, Name = "Cardiology", Description = "Heart and cardiovascular system", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 2, Name = "Dermatology", Description = "Skin, hair, and nails", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 3, Name = "Neurology", Description = "Brain and nervous system", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 4, Name = "Orthopedics", Description = "Bones, joints, and muscles", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 5, Name = "Pediatrics", Description = "Children's health", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 6, Name = "Psychiatry", Description = "Mental health", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 7, Name = "General Medicine", Description = "General health and common illnesses", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 8, Name = "Ophthalmology", Description = "Eye care and vision", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 9, Name = "ENT", Description = "Ear, Nose, and Throat", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { SpecializationId = 10, Name = "Dentistry", Description = "Oral and dental health", IsActive = true, CreatedAt = DateTime.UtcNow }
            );
        }

        // Override SaveChanges to update UpdatedAt timestamp
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                    user.UpdatedAt = DateTime.UtcNow;
                else if (entry.Entity is Patient patient)
                    patient.UpdatedAt = DateTime.UtcNow;
                else if (entry.Entity is Doctor doctor)
                    doctor.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}