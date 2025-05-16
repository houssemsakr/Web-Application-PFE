using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_Application_PFE.Models;
using Web_Application_PFE.ViewModels;

namespace Web_Application_PFE.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VersionEntity> VersionEntities { get; set; }
        public DbSet<AddRFQ> AddRFQs { get; set; }
        public DbSet<EntityClient> EntityClients { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Brouillon> Brouillons { get; set; }
        public DbSet<Epingle> Epingles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
               .Property(u => u.Nom)
               .HasMaxLength(256);

            builder.Entity<User>()
                .Property(u => u.Prenom)
                .HasMaxLength(256);


            // Configuration de la relation entre RFQ et Client
            builder.Entity<AddRFQ>()
                .HasOne(r => r.Client) // Une RFQ appartient à un Client
                .WithMany(c => c.AddRFQs) // Un Client peut avoir plusieurs RFQs
                .HasForeignKey(r => r.ClientId);
           builder.Entity<AddRFQ>()
               .HasOne(r => r.Creator)
               .WithMany(u => u.CreatedRFQs)
               .HasForeignKey(r => r.CreatorId);

            builder.Entity<Brouillon>()
               .HasOne(r => r.Client) // Une RFQ appartient à un Client
               .WithMany(c => c.Brouillons) // Un Client peut avoir plusieurs RFQs
               .HasForeignKey(r => r.ClientId);

            builder.Entity<Epingle>()
              .HasOne(r => r.Client) // Une RFQ appartient à un Client
              .WithMany(c => c.Epingles) // Un Client peut avoir plusieurs RFQs
              .HasForeignKey(r => r.ClientId);
            builder.Entity<VersionEntity>()
               .HasOne(v => v.Client) // Une VersionEntity appartient à un Client
               .WithMany(c => c.VersionEntities)// Un Client peut avoir plusieurs VersionEntity
               .HasForeignKey(v => v.ClientId); // Clé étrangère dans RFQ de la table Client

            builder.Entity<VersionEntity>()
               .HasOne(v => v.AddRFQ)
               .WithMany(a => a.VersionEntities)
               .HasForeignKey(v => v.AddRFQId)
               .OnDelete(DeleteBehavior.Cascade);
            // Configuration des autres tables
            builder.Entity<AddRFQ>()
                .Property(r => r.QuoteName)
                .HasMaxLength(200);

            builder.Entity<EntityClient>()
            .Property(c => c.Sales)
                .HasMaxLength(100);

            builder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuration des rôles avec IDs fixes
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = "2",
                    Name = "Lecteur",
                    NormalizedName = "LECTEUR",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole()
                {
                    Id = "3",
                    Name = "Validateur",
                    NormalizedName = "VALIDATEUR",
                    ConcurrencyStamp = "3"
                },
                  new IdentityRole()
                {
                    Id = "4",
                    Name = "Ingénieur RFQ",
                    NormalizedName = " RFQ",
                    ConcurrencyStamp = "4"
                });

        }
        public DbSet<Web_Application_PFE.ViewModels.VersionViewModel> VersionViewModel { get; set; } = default!;




    }
}
