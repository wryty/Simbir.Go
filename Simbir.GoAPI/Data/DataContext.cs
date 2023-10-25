using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Models;
using System.Net;

namespace Simbir.GoAPI.Data;
public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.Migrate();
    }
    public DbSet<Transport> Transports { get; set; }
    public DbSet<Rent> Rents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /*modelBuilder.Entity<Transport>()
                .HasOne(t => t.TransportType)
                .WithMany(tt => tt.Transports)
                .HasForeignKey(t => t.Id) 
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleType>().HasData(
            new VehicleType { Id = 1, Type = "Car" },
            new VehicleType { Id = 2, Type = "Bike" },
            new VehicleType { Id = 3, Type = "Scooter" }
        );*/
        modelBuilder.Entity<IdentityRole<long>>().HasData(
            new IdentityRole<long> { Id = 1, Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
            new IdentityRole<long> { Id = 2, Name = "Member", NormalizedName = "MEMBER" }
        );

        /*modelBuilder.Entity<Rent>()
        .HasOne(r => r.Transport)
        .WithMany(t => t.Rents)
        .HasForeignKey(r => r.TransportId)
        .IsRequired();

        // Определение внешнего ключа для поля UserId в таблице Rents
        modelBuilder.Entity<Rent>()
            .HasOne(r => r.User)
            .WithMany(u => u.Rents)
            .HasForeignKey(r => r.UserId)
            .IsRequired();*/

    }
}