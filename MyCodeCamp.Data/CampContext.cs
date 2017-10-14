using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyCodeCamp.Data.Entities;

namespace MyCodeCamp.Data
{
  public class AppContextFactory : IDesignTimeDbContextFactory<CampContext>
  {
    public CampContext CreateDbContext(string[] args)
    {
      IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
      var builder = new DbContextOptionsBuilder<CampContext>();
      builder.UseNpgsql("Host=localhost;Database=MyCodeCampDb");
      return new CampContext(builder.Options, configuration);
    }
  }

  public class CampContext : IdentityDbContext
  {
    private IConfiguration _config;

    public CampContext(DbContextOptions options, IConfiguration config)
      : base(options)
    {
      _config = config;
    }

    public DbSet<Camp> Camps { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    public DbSet<Talk> Talks { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<Camp>()
        .Property(c => c.Moniker)
        .IsRequired();
      builder.Entity<Camp>()
        .Property(c => c.RowVersion)
        .ValueGeneratedOnAddOrUpdate()
        .IsConcurrencyToken();
      builder.Entity<Speaker>()
        .Property(c => c.RowVersion)
        .ValueGeneratedOnAddOrUpdate()
        .IsConcurrencyToken();
      builder.Entity<Talk>()
        .Property(c => c.RowVersion)
        .ValueGeneratedOnAddOrUpdate()
        .IsConcurrencyToken();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      base.OnConfiguring(optionsBuilder);

      optionsBuilder.UseNpgsql(_config["Data:ConnectionString"]);
    }
  }
}
