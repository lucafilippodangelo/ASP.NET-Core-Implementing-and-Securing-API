
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCodeCamp2.Entities;

namespace MyCodeCamp2.Data
{
  public class CampContext : IdentityDbContext
  {
    

    //public CampContext() : base(){ } //LD we need of a parameterless constructor
        
    public CampContext(DbContextOptions<CampContext> options) : base(options)
    {    }

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
            //LD STEP48
            builder.Entity<Talk>() 
              .Property(c => c.RowVersion)
              .ValueGeneratedOnAddOrUpdate()
              .IsRequired() 
              .IsConcurrencyToken();
        }

        ////LD IMPORTANT -> this version of entity framework CORE, REQUIRE as setting, the 
        //// use of the "OnConfiguring" method. DIFFERENTLY THAN THE PROJECT "Identity Test"
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    //LD QUELLO SOTTO FUNONZIA
        //    //optionsBuilder.UseSqlServer("Data Source=LUCA;Database='DbMyCodeCamp2';Integrated Security=False;User ID=sa;Password=Luca111q;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        //    optionsBuilder.UseSqlServer(_config["ConnectionStrings:LdConnectionStringMyCodeCamp2"]);
        //}

    }
}


