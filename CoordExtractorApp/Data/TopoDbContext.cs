using Microsoft.EntityFrameworkCore;

namespace CoordExtractorApp.Data
{
    public class TopoDbContext : DbContext
    {
        public TopoDbContext()
        {
        }
        public TopoDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Prompt> Prompts { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ConversionJob> ConversionJobs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);   // Optional if 'Id' is the convention
                entity.Property(e => e.Username).HasMaxLength(50);  // define max length is MAX
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Password).HasMaxLength(60);
                entity.Property(e => e.Lastname).HasMaxLength(50);
                entity.Property(e => e.Firstname).HasMaxLength(50);
                entity.Property(e => e.UserRole).HasMaxLength(20).HasConversion<string>();
                entity.Property(e => e.InsertedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            });

            modelBuilder.Entity<Prompt>(entity =>
            {
                entity.ToTable("Prompts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PromptName).HasMaxLength(100);
                entity.Property(e => e.PromptText);
                entity.Property(e => e.InsertedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.PromptName, "IX_Prompts_PromptName").IsUnique();
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProjectName).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.InsertedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.ProjectName, "IX_Projects_ProjectName").IsUnique();

                entity.HasMany(e => e.Users).WithMany(e => e.Projects)
               .UsingEntity<Dictionary<string, object>>("ProjectsUsers");
                           

            });

            modelBuilder.Entity<ConversionJob>(entity =>
            {
                entity.ToTable("ConversionJobs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalFileName).HasMaxLength(50);
                entity.Property(e => e.CroppedFileName).HasMaxLength(50);
                entity.Property(e => e.ModelUsed).HasMaxLength(50);
                entity.Property(e => e.WktOutput);
                entity.Property(e => e.ImageFileId).HasMaxLength(50);

                entity.Property(e => e.InsertedAt)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("GETUTCDATE()");


                entity.HasIndex(e => e.UserId, "IX_ConversionJobs_UserId");
                entity.HasIndex(e => e.PromptId, "IX_ConversionJobs_PromptId");
                entity.HasIndex(e => e.ProjectId, "IX_ConversionJobs_ProjectId");


                entity.HasOne(e => e.User)
                .WithMany( e=> e.ConversionJobs) //για να έχει πολλά Jobs
                //.HasForeignKey(e => e.UserId) είχαμε πει υπονοείται απο το entity που εχει foreign key
                .OnDelete(DeleteBehavior.Restrict); //αν διαγραφεί ο user να μην κανει cascade delete

                entity.HasOne(e => e.Prompt)
                .WithMany() //για να έχει πολλά Jobs              
                .OnDelete(DeleteBehavior.Restrict); //αν διαγραφεί το prompt να μην κανει cascade delete

                entity.HasOne (e => e.Project)
                .WithMany( e=> e.ConversionJobs)
                .OnDelete(DeleteBehavior.Restrict);
            });


        }

    }

    }
