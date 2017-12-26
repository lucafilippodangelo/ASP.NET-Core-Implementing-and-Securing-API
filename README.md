# ASP.NET CORE Implementing and Securing API Exercise

The exercise is based on an ASP.NET CORE MVC project pluralsight course. Main Topics:

- HTTP protocol
- REST, resource Based Architecture
  - REST constraints

- routing

- Setting of data model and database
  - constructor injection
  - database seeding, use of fluent api
  
- API, reading data(postman)
  - query string parameters
- API, post data
- API, put data
- API, delete data

- avoid CYCLIC REFERENCE once using json

- Logging

- Model mapping AUTOMAPPER
  - mapping url
  - MODEL STATE and VALIDATION

- Associations in API
  - POST of an INNER ASSOCIATION

- Filters
  - ActionFilterAttribute

- Functional API

- Securing API
  - SSL ("secure socket layer" to ensure comunication between client and server)
  - SYMMETRIC ENCRYPTION
  - ASYMMETRIC ENCRYPTION

- CORS (CROSS-ORIGIN RESOURCE SHARING)

- API Authentication and Authorization
  - Identity
    - IdentityOptions
  - Cookie Authentication
    - UserManager
  - Token Authentication
    - JWT Json Web Token
    - CLAIM 
    - POLICY 

- API VERSIONING
  - Versioning Attributes
  - Versioned Controllers
  - Versioning Payload
  
- CACHE (definitions)
- CACHE (implementation InMemoryCache)
  - MemoryCacheEntryOptions
  - ETags
  - ROWVERSION
  - FLUENT API 
  - CONCURRENCY SETTINGS 

**Useful Links**

-(in memory caching) http://www.dotnetcurry.com/aspnet-mvc/1246/inmemory-caching-aspnet-mvc-6-core

-(response caching) http://www.c-sharpcorner.com/article/response-caching-in-asp-net-core/

-(caching in asp.net core) https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core

-(cache performance) http://jakeydocs.readthedocs.io/en/latest/performance/caching/index.html

-(caching) https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core

-(distributed cache) https://www.codeproject.com/Articles/1161890/Distributed-cache-using-Redis-and-ASP-NET-Core

-(response caching) http://www.c-sharpcorner.com/article/response-caching-in-asp-net-core/

## Personal notes and reflections

Is possible match the code I wrote and the notes by searching for instance "//LD STEP0,//LD STEP1, etc.." 

### HTTP protocol

HTTP work in this way, is a comunication where we have a REQUEST and a responce.

a REQUEST is composed by:
- VERB --> es. "Post" (is the action) [GET,POST,PUT,PATCH,DELETE]
- HEADER --> (information about the request)
- CONTENT

a RESPONCE is composed by:
- STATUS CODE
- HEADER
- CONTENT

all that is STATELESS, the server doesn't keep track of all the requestes.

### REST, Resource Based Architecture

DEFINITION --> "Representational state patttern"
- separation of client and server
- server requests are stateless
- cacheable requests

- is the CONCEPT for which resources are rapresentative of real world entityes, relations are tipically nested

- URI is the path to the resource
- QUERY STRINGS used in the request to transfer info not related with data elements but with preferences(es. sorting, paging)

### setting of "Program.cs"

it is the starting point for dot.net core, as dot.net core is a console application, here we configure the server that will start listen our requestes and set as well a link to initialize the "Startup.cs" class.

      startup.cs --> "ConfigureServices"

the purpose of the above code is setup all the dependency injection

      startup.cs --> "Configure"

the purpose of the above method is to handle the web requestes

+ "Program.cs" file where we set the web server that will listen for the requestes

      //LD STEP0
      
### READING DATA - API basics

Rules:
- URI should point to NOUNS
- don't expose primary key

CONSTRUCTOR INJECTION of "ICampRepository repo"
 
      //LD STEP22

add the SERVICE for dependency injection in "Startup.cs"

      //LD STEP33

then we INJECT the DBCONTEXT as well
      
     //LD STEP44

now we try to gett a list of records

        //LD STEP55
        [HttpGet ("GetAllCamps")]
        public IActionResult GetAllCamps()
        {
            var camps = _repo.GetAllCamps();
            return Ok(camps);
        }

**list of the nuget packages needed**
   
    <PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.1.1" />
    <PackageReference Include="Microsoft.AspNetCore" Version="1.1.2" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="1.1.2" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="1.1.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="1.1.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="1.1.2" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="1.1.2" />

**Database setting useful classes**

- DbContext

```
  public class CampContext : IdentityDbContext
  {
    
    private IConfigurationRoot _config;
    public CampContext() : base(){ } //LD we need of a parameterless constructor
        
    public CampContext(DbContextOptions options, IConfigurationRoot config) : base(options)
    {
        _config = config;
    }

    public DbSet<Camp> Camps { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    public DbSet<Talk> Talks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Camp>()
            //  .Property(c => c.Moniker)
            //  .IsRequired();
            //builder.Entity<Camp>()
            //  .Property(c => c.RowVersion)
            //  .ValueGeneratedOnAddOrUpdate()
            //  .IsConcurrencyToken();
            //builder.Entity<Speaker>()
            //  .Property(c => c.RowVersion)
            //  .ValueGeneratedOnAddOrUpdate()
            //  .IsConcurrencyToken();
            //builder.Entity<Talk>()
            //  .Property(c => c.RowVersion)
            //  .ValueGeneratedOnAddOrUpdate()
            //  .IsConcurrencyToken();
        }

        //LD IMPORTANT -> this version of entity framework CORE, REQUIRE as setting, the 
        // use of the "OnConfiguring" method. DIFFERENTLY THAN THE PROJECT "Identity Test"
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=LUCA;Database='DbMyCodeCamp2';Integrated Security=False;User ID=sa;Password=Luca111q;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

    }
```

- Startup

```
public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            Configuration = builder.Build();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddDbContext<CampContext>(ServiceLifetime.Scoped);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) ///,CampIdentityInitializer identitySeeder,CampDbInitializer seeder,
        {

            app.UseIdentity();

            //app.UseMvc();

        }
    }
```
