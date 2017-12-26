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
      
### Reading Data

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

### DB Seeding, Controller Setup, CRUD operations

**Reading Data**

http postman call example:
    ```
    http://localhost:8088/api/camps/ATL2016/speakers
    ```

- implementation of the first controller 
     ``` 
     //LD STEP1
     ```
     
- GET SPECIFIC INSTANCE and use QUERY STRING PARAMETER

     ```
     //LD STEP55 - the query string parameter is "includeSpeakers"

     POSTMAN CALL
     http://localhost:8088/api/camps/getspecific/5?includeSpeakers=true
     ```
     
- avoid CYCLIC REFERENCES

      //LD STEP88

**Routing**

- OLD WAY TO ROUTING, in a static way.
      
      ```
      app.UseMvc(config =>
      {
        //config.MapRoute("MainAPIRoute", "api/{controller}/{action}");
      });
      ```
      
- NEW WAY, DEFINE ATTRIBUTES IN THE CONTROLLER
    
    ```
    //LD STEP2
    [HttpGet("api/cazzo")]
    ```
    
**Database Seed**

- update "Startup.cs" in order to add the reference to the class that has to SEED the database 

      ```
      //LD STEP55
      ```
      
- then inside "Startup.cs" we have to update the "Configure" method, by updating the parameters:
      
      ```
      //LD STEP66
      ```
      
and call the method that receive the context and if the database is empty, then seed it

      ```
      //LD STEP77
      ```
      
**Post and Model Binding**

      ```
      //LD STEP99
      ```
      
in POSTMAN -> "BODY" I have to tick "raw" and then select "JSON", and in visual studio do this action

      ```
      public IActionResult Post([FromBody]Camp model)
      ```
      
we specify [FromBody], the meaning is that we want that the parser will try to PARSE from the BODY OF THE REQUEST

+ now I want store a new record and then return back a LINK that point to that specific record. So I have to send a LINK TO THE GET ACTION.

before I name the ROUTING to the GET ACTION

      ```
      //LD STEP100
      ```
      
then I implement the code to store and return the link

      ```
      //LD STEP101

      [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]Camp model)
        //public async Task<IActionResult> Post([FromBody]CampModel model)
        {
            try
            {
                _logger.LogInformation("Creating a new Code Camp");

                //    var camp = _mapper.Map<Camp>(model);
                //LD STEP101
                _repo.Add(model);

                if (await _repo.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { id = model.Id });
                    return Created(newUri, model);
                }
                else
                {
                    _logger.LogWarning("Could not save Camp to the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while saving Camp: {ex}");
            }
        ```

- in the HEADER tab  that I have in POSTMAN I will receive the new URL that point to my new object

**Put**

      ``` 
      //LD STEP104
      ```
      
I can call it by a PUT request like this 

      ```
      http://localhost:8088/api/camps/5
      ```
      
**Delete**

easly implemented. See code example in project.

### Logging

+ the LOGGING SYSTEM is INTEGRATED WITH DEPENDENCY INJECTION, in order to use it we have just t say that we waant log for a specific class, in our case we want log in a controller

        ``` 
        //LD STEP102
        public CampsController(ICampRepository repo, ILogger<CampsController> logger) //LD STEP102
        {
            _repo = repo;
            _logger = logger;
        }
        ```
        
then use it in the ACTION

      ``` 
      //LD STEP103
      ```
      
is possible see the logging in the OUTPUT WINDOW, by selecting DEBUG in the "Show Output Option" combo

### Entities and Models

**Adding a View Model**

- he just create all the DTO, so a flat model to call, in order to don't send to the user informations tha we don't want

**Using MODEL MAPPING AUTOMAPPER**

- install AUTOMAPPER  -> "automapper.extensions.microsoft.dependencyinjection"
  - close and reopen visual studio, if i'm not able to import libreries
      
      ```
      //LD STEP3 (I used this label in more points of configuration, search more times)      
      ```
      
then add automapper to the controller 

      ```
      //LD STEP3
      ```
      
then create a new "CampModel" based on the "camps" data. NOTE THAT is not an "IEnumerable"

      ```
      return Ok(_mapper.Map<CampModel>(camps));
      ```
      
then add a class than inherit from "profile" and do the actual mapping 

      ```
      CampMappingProfile.cs
      ```
      
with AUTOMAPPER is possible map a related information of the main entity(with cardinality 1to1). In this case to get the "Location" of a "Camp" we are adding the prefix "Location"

      //LD STEP5

- in AUTO MAPPER PROFILE, I can chose by FLUENT API to map a specific field name to another one, so it's not mandatory that the FIELD NAMES HAD TO MATCH 100%

      //LD STEP6
      CreateMap<Camp, CampModel>()
      .ForMember(c => c.StartDate,
      opt => opt.MapFrom(camp => camp.EventDate)) 

**mapping URLs**
fon an entity, instead of in ID, we want return an unique URL that represent that specific entity.

      //LD STEP777  (used many times) 

+ we could create an URL in the controller action like in //LDSTEP101, but we want do that by MAPPER, the solutionwith this mapper or with other is use an URLRESOLVER class.

      //LD STEP888 (used many times) 

set a BASE CONTROLLER in order to get "OnActionExecuting" the URL

- the BASE CONTROLLER is useful to centralize any logic we reuse.

add dependency injection 

      //LD STEP888

so now the URL is RETURNED as a field of the view model, to test it call by POSTMAN:

      by ID -> http://localhost:8088/api/camps/getspecific/5
      by a FIELD -> http://localhost:8088/api/camps/getspecificmoniker/CAZZO moniker

**using MAPPING IN POST**

then in sequence

- map from "CampModel" to "Camp"

- then persist the "camp"

- then return to the user a mapping from "Cam"

command: 

      http://localhost:8088/api/camps/postmapper

** using MODEL STATE and VALIDATION **

+ we are adding a check, see if the "ViewModel" passed in in the command POST is VALID

      //LD STEP9991
      if (!ModelState.IsValid) return BadRequest(ModelState); 

** implementation of PUT **

      //LD STEP9992

### Associations in API 

Data is related, sometiome the user like dig on data relations.
We are creating an ASSOCIATION CONTROLLER for "Camps" called "Speakers".
In this "Speakers" controller, starting from a specific "Camp" we will query for the list of speahers associated or for a specific speaker.

      api/camps/campName/speakers
      api/camps/campName/speakers/speakerName


- here we define the ROUTING for all the actions of the controller 

        //LD STEP7
        [Route("api/camps/{moniker}/speakers")]

- I do those calls 

      http://localhost:8088/api/camps/ATL2016/speakers
      http://localhost:8088/api/camps/ATL2016/speakers/1

- then I have to update the "CampMappingProfile.cs" in order to use the MAPPER

      //LD STEP72

- then I have to add a "SpeakerUrlResolver" in order to get the proper URL

      //LD STEP73

- **implementing POST of an INNER ASSOCIATION**

here we pass the "SpeakerModel" information and the specific "moniker" for who we want save.

then in sequence

      - we get the specific camp record by searching by moniker
      - after we map the "SpeakerModel" to "Speaker" entity
      - then we assign the "Camp" entity to the "Speaker" entity

      //LD STEP74

this is the postman call

      http://localhost:8088/api/camps/ATL2016/speakers/

this is the content of the call

    "name": "Luca D'Angelo",
    "companyName": "alivi corporation",
    "phoneNumber": "555-1212",
    "websiteUrl": "http://wildermuth.com",
    "twitterName": "shawnwildermuth",
    "gitHubName": "shawnwildermuth",
    "bio": "luca is a speaker for ATL2016",
    "headShotUrl": 
     "http://wilderminds.com/images/minds/shawnwildermuth.jpg"


**FILTER** filters are ways to interrupt direct call to actions. Then appy some code, check, conditions

**adding VALIDATION, ActionFilterAttribute**

we are adding validation to "SpeakerModel.cs", the action to verify if the MODEL IS VALID is repeated in any controller, so we are going to use a common

      //LD STEP8

so by this code, if the model is not valid the action is not executed at all

to USE THE FILTER we just need to add it in the controller or in the specific action if we don't want use it for all the actions.

      //LD STEP9

he shows as well how to use QUERY STRINGS, but is the same approach of the previous training.

- adding the ** Talk Association **
below an example of request

      http://localhost:8088/api/camps/ATL2016/speakers/1/talks

- after he show how to use QUERY STRING in the speaker controller, we are able to specify in url when or not load "talks"

      //LD STEP10
      public IActionResult Get(string moniker, bool includeTalks = false)
