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
    - JWT Json Web Token (creation of middleware)
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

-(manage inheritance with EF) http://www.learnentityframeworkcore.com/inheritance/table-per-hierarchy
      
-(ssl handshake) https://support.microsoft.com/en-us/help/257591/description-of-the-secure-sockets-layer-ssl-handshake
      
-(ssl handshake) http://www.slashroot.in/understanding-ssl-handshake-protocol

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

**implementation of PUT**

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

**implementing POST of an INNER ASSOCIATION**

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

### Functional API

the meaning is that we can't do all by REST(resource based approach), sometime we need to reset a server, or clear cache... so we have tofind a way to organize this specific part of API.

he create "OperationsController.cs" where we are simulating the reloading of the configuration bundle.

      //LD STEP11

we are using a specific verb available for "Options"

      [HttpOptions("reloadConfig")]
      
### Securing APIs

**SSL - secure socket layer**
the comunication between client and server has to be secure. To face up to that we use SSL, is based on the concept of TRUST+ENCRIPTION between the two sides.

slide useful for SSL -->
[7-aspdotnetcore-implementing-securing-api-m7-slides.pdf](https://trello-attachments.s3.amazonaws.com/579776d92c5f0fd947aff4a8/58a2f698a47cf8c6e3a9969a/c8c0f5cb9b43e0a7bcf23fcccbfbd418/7-aspdotnetcore-implementing-securing-api-m7-slides.pdf) 

- Secure in transit - **SYMMETRIC ENCRYPTION**, 
  - here we encript, decript by same algorithm(AES)+key. the KEY is SHARED. the KEY is PRIVATE

- Secure in transit - **ASYMMETRIC ENCRYPTION**
  - here we keep secret the private key! If somebody still the public key can only encript data.
  - entity "A" encript the data using a PUBLIC KEY supplied by entity "B". this KEY CAN ONLY ENCRIPT
  - entity "B" use a never given PRIVATE KEY paired with the public, able to decript.

- Secure in transit - **SSL HANDSHAKE**(use a combination of symmetric and asymmetric technique)
  - 1 - entity "A" send a signal of "begin request"
  - 2 - entity "B" reply with a "certificate"(to show that the part can be trusted). the "certificate" cointain a PUBLIC KEY.
  - 3 - entity "A" use the public key to encript a NEW symmetric key, then return it. 
  - 4 - since this moment the communication can be encripted. 

DEMO - ** Supporting SSL **

the below MIDDLEWARE is a FILTER, it manages secure SSL request coming in, if the request is not secure it is automatically managed as http PROTOCOL

we have to set "Startup.cs"

      //LD STEP12

we have to enable SSL in the property of the prooject, and just use it as the first part of our http request.

      manu project -> property -> debug -> enable SSL

we can add the following code in "Startup.cs" to enable SSL JUST IN PRODUCTION

                if (!_env.IsProduction())
                {
                    opt.SslPort = 44342; //LD same port we see in configuration
                }

STEP 2 - enable SSL request in POSTMAN and enable SSL in project by the properties settings.


** Supporting CORS **

CORS -> Cross Origin Resource Sharing
- It is a mechanism that allows restricted resources (e.g. fonts) on a web page to be requested from another domain outside the domain from which the first resource was served. A web page may freely embed cross-origin images, stylesheets, scripts, iframes, and videos.

we can enable CORS in our website in order to allow calls to API from specific path , or by specific header etc...

let's see how to implement it! In "Startup.cs"

            //LD STEP13
            services.AddCors();

and 

            //LD STEP14
             app.UseCors(cfg =>
            {
                cfg.AllowAnyHeader()
                   .AllowAnyMethod()
                   .WithOrigins("http://wildermuth.com");
            });

so any call coming from "http://wildermuth.com" is allowed to the API. But this is GLOBAL, if we want more granularity, we have to set some POLICY in "Startup.cs" -> "ConfigureServices"method.
We have to comment //LD STEP13, and then add POLICY

            //LD STEP15
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("Wildermuth", bldr =>
                {
                    bldr.AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithOrigins("http://wildermuth.com");
                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                      .WithMethods("GET")
                      .AllowAnyOrigin();
                });
            });

then I have to specify the property in the controller

      [EnableCors("AnyGET")] //LD STEP16

we can act at ACTION LEVEL, and enable for that specific action

      [EnableCors("Wildermuth")] //LD STEP17

in this case, just people from "Wildermuth.com" can update 

### API Authentication and Authorization

There are different types of Authentication we will be focused on "**Asp.Net Identity**", a simple way to store user **roles** and **claims**. It can be used for both **cookie** and **token** authentication

**Asp.Net Identity**

to use it:

- install the NUGET "microsoft.aspnetcore.identity"

note that we have "public class CampUser : IdentityUser"

      //LD STEP18

so we have to add the identity that inherit from "IdentityUser" in Startup.cs".

      //LD STEP19 (used for methods "ConfigureServices" and "Configure")

so this type is going to represent the type and the user in our system. Note that we specify the context that cointain the informations for our user.

      //LD STEP19
      services.AddIdentity<CampUser, IdentityRole>().AddEntityFrameworkStores<CampContext>();

then in the controller we add "[Authorize]"

          [Authorize] //LD STEP20

with this in place, if I execute 
  
      "http://localhost:8088/api/camps/getspecificmoniker/ATL2016" 

in a browser I can see that the framework try to redirect me to the "login" page, instead in postman I get error "404 not found"

this is the url returned by the browser(I get https because I have SSL enabled)

      https://localhost:44342/Account/Login?ReturnUrl=%2Fapi%2Fcamps%2Fgetspecificmoniker%2FATL2016

in order to avoid a "404 Not Found" error when we are not Authorized to do something, we have to configurate "IdentityOptions"

            //LD STEP21
            services.Configure<IdentityOptions>(config =>
            {
                config.Cookies.ApplicationCookie.Events =
                  new CookieAuthenticationEvents()
                  {
                      OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                      OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    }
                  };
            });

we set the "Events" when we use the cookie authentication, in order to change the common workflow of the framework

      //LD STEP22

we are overriding the action "OnRedirectToLogin"


**Cookie Authentication** with Asp.Net Identity

we start by creating an "AuthController", a controller dedicated to manage all the authentication requestes.

      //LD STEP23
      public class AuthController : Controller

note that the controller get as parameter a " SignInManager<CampUser> signInMgr". 

what we want do is let the user post "username" and "password" in order to login

      //LD STEP24
      [HttpPost("api/auth/login")] 

once we are passing over the network sensitive data, we have to use **SSL**

with the "false" in this row we explicitly set that we don't want persist the cookie in the browswer after it is closed

      //LD STEP25
      var result = await _signInMgr.PasswordSignInAsync(model.UserName, model.Password, false, false);

when we do SIGNIN, THE FRAMEWORK AUTOMATICALLY DROPS A COOKIE IN THE CLIENT RESPONCE

then we need to initialize the users in the database, using "CampIdentityInitializer"

      //LD STEP26

then set this context in "Startup.cs"

      //LD STEP27 - context

      //LD STEP28 - seed

      //LD STEP29 - execute seed

then we create an SSL POST call in postman

      https://localhost:44342/api/auth/login

with this data

      {
             "userName": "shawnwildermuth",
             "password": "P@ssw0rd!"
      }

NOW A COOKIE WILL BE RETURNED, is enought have a look on the COOKIE TAB, I will find an object od type ".AspNetCore.Identity.Application"

**Using Identity Information**

we are adding the [Authorize] attribute to the actions "Post" and "Delete" of "SpeakerController.cs"

      //LD STEP30

now we add the "User" to the controller, by using the **UserManager**. We can't assign streight 

      speaker.user = this.user

because in this case we are assign a type user.principal to a speaker user. We need to grab the "IdentityUser" in this way

      var campUser = await _serMgr.FindByNameAsync(this.User.Identity.Name);

I used this label for the operation

      //LD STEP31 (used 4 times)
      
Postman POST call example

      https://localhost:44342/api/camps/ATL2016/speakers

Postman POST body call example

      {
    "url": "https://localhost:44342/api/camps/ATL2016/speakers/4",
    "name": "aliviSpeaker",
    "companyName": "alivi corporation",
    "phoneNumber": "555-1212",
    "websiteUrl": "http://wildermuth.com",
    "twitterName": "shawnwildermuth",
    "gitHubName": "shawnwildermuth",
    "bio": "luca is a speaker for ATL2016",
    "headShotUrl": "http://wilderminds.com/images/minds/shawnwildermuth.jpg",
    "talks": []
   }
      

### Token Authentication

**JWT Json Web Token** - theory

are self contained: credentials+claims+other informations

STRUCTURE of a JWT:
HEADER (encription algotithm + type es "JWT") +
PAYLOAD (json that contain info the server may want: some numbers+some claims + some names) +

the JWT is then ENCODED
they encrypt each piece HEADER+PAYLOAD in "base64encode", separating it by "." and at the end they concatenate an "ENCRYPTED SIGNATURE". 

the "ENCRIPTED SIGNATURE" is an HASH of the HEADER+PAYLOAD+ a SECRET signed by the server, that just the server know, and that the clien doesn't need to decript. The client if needed can access to the HEADER+PAYLOAD.


**Generating JWT** - Demo 

now we update "AuthController.cs" in order to jenerate a token when login

      //LD STEP32

to use JWT I need to install the nuget "System.IdentityModel.Tokens.Jwt"

then we will CREATE CLAIM in order to insert them in the TOKEN

      //LD STEP33 (there are a lot of comments in this method)

to remember thet we SEED the CLAIM "superuser"

      //LD STEP33BIS

then just try to authenticate by POSTMAN

      https://localhost:44342/api/auth/token

and I will RECEIVE the TOKEN

      {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzaGF3bndpbGRlcm11dGgiLCJqdGkiOiJjNTU2MDI1ZC00ODYxLTQ2ZmItYjAzOC0wZmJjMzE0NWZmNDMiLCJnaXZlbl9uYW1lIjoiU2hhd24iLCJmYW1pbHlfbmFtZSI6IldpbGRlcm11dGgiLCJlbWFpbCI6InNoYXduQHdpbGRlcm11dGguY29tIiwiU3VwZXJVc2VyIjoiVHJ1ZSIsImV4cCI6MTUwMjU0MTA5NywiaXNzIjoid3d3Lmx1Y2FkYW5nZWxvLml0IiwiYXVkIjoid3d3Lmx1Y2FkYW5nZWxvLml0In0.I1CfYmIiWoOhWxI0q5zQZqQgOMRPrzDLPasHmGD5upU",
    "expiration": "2017-08-12T12:31:37Z"}

there is an interesting website to VERIFY TOKENS: https://jwt.io/

now we set "AppSettings.json" to avoid to "hard code"

     //LD STEP34
 
**Create Middleware for JWT** - validate JWT to use it as security tokens 

we need to set the "Configure" method of "startup.cs". (include as well: Microsoft.AspNetCore.Authentication.JwtBearer) we ask to the framework that if find the token then authenticate.

      //LD STEP35

FOR EXAMPLE we can VERIFY AUTHORIZATION. If we do a simple request like

      https://localhost:44342/api/camps/getall

we will get "401 Unhautorized", but if in the HEADER we specify the KEY-VALUE

      Authorization - bearer+"active token string previously generated"

the get request will be executed.


**Authorizing with Claims**

we have to add a specific POLICY to authorize specific users with specific claims to do specific actions.

      //LD STEP36 (code to add in "startup.cs")

      //LD STEP37 (code to add in an action controller)
      [Authorize(Policy = "Superusers")] 
      

### Versioning API

the pdf
   
      9-aspdotnetcore-implementing-securing-api-m9-slides.pdf

that describe the way to let the customer specify the version of the api, by query index, of by header etc...


**Adding Versioning** (setting)

we have to add the package  "microsoft.aspnetcore.mvc.versioning"

now in "startup.cs" we have to add the SERVICE "service.AddApiVersioning"

      //LD STEP38

this is useful to specify how VERSIONING IS HANDLED in our application.

when we do like above, for any http request we have to specify the version, the default way is by query string

       https://localhost:44342/api/auth/token?api-version=1.0

so by just specifying 

       //LD STEP38
       services.AddApiVersioning

in "startup.cs" is mandatory specify versioning in any http request. THE VERSION "1.0" is the default one, so this is valid in the situation we don't specify any version in "startup.cs"

we can OVERRIDE the DEFAULT API VERSION

      cfg.DefaultApiVersion = new ApiVersion(1, 1);

by adding the below code we say that if in the request there is not specified any version, the framework will consider the default one

      cfg.AssumeDefaultVersionWhenUnspecified = true;

we can send back in the header the supported api versions

      cfg.ReportApiVersions = true;

for instance:
  
      request: "https://localhost:44342/api/auth/token"
      part of the information in the returned header:  "api-supported-versions →1.1"


**Using Versioning Attributes**

we add the attribute in "SpeakerController.cs"

      //LD STEP39
      [ApiVersion("1.0")]
      [ApiVersion("1.1")]

so in this case all the requests for api 1.0 and 1.1 will be allowed

- IF I TRY TO DO A REQUEST where is enabled "[Authorize]", REMEMBER TO INCLUDE A VALID TOKEN IN THE HEADER, is possible create a token with "https://localhost:44342/api/auth/token"
in this case for this example we don'r use [Authorize] to go faster.

I can specify version for a specific action by using

      //LD STEP40
      [MapToApiVersion("1.1")]

until now we did a versioning of small changes by just versioning the actions with MapToApiVersion, but if we have bigger changes we have to use VERSIONED CONTROLLERS

**Using Versioned Controllers**

for this controller we INHERIT from the "SpeakersController". He created "Speakers2Controller"

      //LD STEP41

we use the same routing but different API version

      [Route("api/camps/{moniker}/speakers")]
      [ApiVersion("2.0")]

then we OVERRIDE the method in //LD STEP40
so I can version at controller level or at action level.
 
**Versioning Payload**

now I create the new model "Speaker2Model.cs", we inherit from "SpeakerModel" and just add a property.

      //LD STEP42

then we update the "CampMappingProfile.cs" with the mapping for the new model "Speaker2Model.cs"

              CreateMap<Speaker, Speaker2Model>()
              .IncludeBase<Speaker, SpeakerModel>()
              .ForMember(s => s.BadgeName, opt => opt.ResolveUsing(s => $"{s.Name} (@{s.TwitterName})"));

we are going to use it in the new controller "Speaker2Controller.cs" 

      //LD STEP43

WHAT I DID: we did inherit both controller and model, we did an api versioning of the controller and we update the "mapper". now I have a clean API VERSION 2.0

**Customizing Versioning Methods** (Query Index or Header)

what we do here is specify in our rest call the version of the API we want call 
we are going to customize startup.cs in order to use the "**Header**" of the request to version the api we are going to call. The **query string** will not work anymore, will be ignored.

this is the update to do in "Startup.cs"

      //LD STEP44
      var rdr = new HeaderApiVersionReader("ver"); 

now the system will read the "ver" header valie that we will specify in the header. For example in the header we have to specify

      ver - 2.0

THIS "HeaderApiVersionReader" IS NOT WORKING FOR A QUESTION OF VERSIONING OF THE PACKAGE I THINK. 
Anyway, by "Query Index Versioning" or by "Header Versioning" we are covering all the needs we have.


**Use versioning Conventions**

is possible specify in "Startup.cs" a configuration useful for avoid attributes for versioning in controllers. So we can specify in "Startup.cs" all the vesions per controller and actionis accepted for specific controller.

      //LD STEP45
      

### CACHE 

**InMemoryCache** settings: 

we have to add the MEMORY CACHE setting in "Startup.cs"

       //LD STEP51
       services.AddMemoryCache();

then add the cache dependency injection to the controller

      //LD STEP52

we have to know the "MemoryCacheEntryOptions", where I have to specify when cache has to expire:

  1 - Absolute expiration (the cache expire after a certain amount of time)
  
  2 - Sliding Expiration (the cache expire timeout is resetted any time the cache is used)
  
  3 - Cache Priority (we can define how important is the cache, so aspnet is able to manage the eviction of it) 
  
  4 - Post Eviction Delegate (called when the cache will be evicted from the memory)

example;

      //LD STEP X001

**ROWVERSION** setting

we used FLUENT API to store the rowversion and concurrency settings to the specific entity "TALKS"

              //LD STEP48
              builder.Entity<Talk>() 
              .Property(c => c.RowVersion)
              .ValueGeneratedOnAddOrUpdate()
              .IsRequired() 
              .IsConcurrencyToken();

es. https://www.tektutorialshub.com/property-mappings-using-fluent-api/

**DEFINITION**
When a clientmake a request, the server attach in the responce an **ETag** describing the version of the data returned.
Then if the client do the same request again the server can check the ETag, if this doesn't change, then the server return an empty body, because the client already have the latest data available in cache.

**IMPLEMENTATION - ETag - GET**
the **ETag** is an id number, we are going to use the **RowVersion**

      //LD STEP46

we attach this "ETag" the responce to the header and SET IN CACHE the new(just saved) ROWVERSION FOR THE SPECIFIC TALK ID

      //LD STEP47

this is the call, now we will be able to see the ETag in our responce HEADER 

      https://localhost:44342/api/camps/ATL2016/speakers/1/talks/1

now in POSTMAN I have to add "If-None-Match" with the ETag received in the responce header after the first get, in this case:

      If-None-Match - AAAAAAAAB9Q=

then we have to add the check in the controller

            //LD STEP50
            if (Request.Headers.ContainsKey("If-None-Match"))
            {
                var oldETag = Request.Headers["If-None-Match"].First();
                if (_cache.Get($"Talk-{id}-{oldETag}") != null)
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
            }



**IMPLEMENTATION - ETag - UPDATE/DELETE**

now we have to update the PUT method of the "Talks.cs" controller, by doing this we are able to handle CONCURRENCY

                //LD STEP53
                if (Request.Headers.ContainsKey("If-Match"))
                {
                    var etag = Request.Headers["If-Match"].First();
                    if (etag != Convert.ToBase64String(talk.RowVersion))
                    {
                        return StatusCode((int)HttpStatusCode.PreconditionFailed);
                    }
                }

I UPDATE THE ROWVERSION IN CACHE

      //LD STEP54

now if I do a PUT REQUEST and specify

      If-Match - "ETag string"

In case somebody else did update the record after I got the last RowVersion(and I will transmit this info in the header), I will get a "PreconditionFailed" status.

same code for DELETE action

      //LD STEP54

**DELETE CACHE when delete action**, I added it

      //LD STEP55

**TO RESUME GET** 

if in cache == rownumber received 
	do nothing (mean that nobody updated the specific id)
  else
	execute query db + return instance + update cache + return rowversion to client in header
  
**TO RESUME PUT** 
  if ETag received by server != actual in db(query executed)
	PreconditionFailed (concurrency detected)
  else
	update instance + update cache with new rowversion + return New rowversion

Postman GET and PUT calls
https://localhost:44342/api/camps/ATL2016/speakers/1/talks/1

need to specify in  the header "If-Match", "If-None-Match"


### CACHE (definitions)

**Cache definition**: 

the cache is a separate component that 

- accepts requests from the consumer to the API
- receives responces from the API and stores them if cacheable

To resume is a kind of "middle-man" of request/responce communication between the consuming appication and the API

**Cache types definition**:

- "Client cache"(browser), called private because the resources are not shared with anyone else.
- "Gateway cache"(server), this is shared
- "Proxy cache"(network, doesn't live in consumer neither server side), this is shared.

**Server "Expiration Model" definition**:

allows the server to state how long a responce is considered fresh
-cahce control header, where is possible decide between private and public server memory and expiration time in seconds

**Cache and .net core**

ASP.NET Core supports several different caches. The simplest cache is based on the IMemoryCache, which represents a cache stored in the memory of the web server. 

Apps which run on a server farm of multiple servers should ensure that sessions are sticky when using the in-memory cache. Sticky sessions ensure that subsequent requests from a client all go to the same server. For example, Azure Web apps use Application Request Routing (ARR) to route all subsequent requests to the same server.

Non-sticky sessions in a web farm require a distributed cache to avoid cache consistency problems. For some apps, a distributed cache can support higher scale out than an in-memory cache. Using a distributed cache offloads the cache memory to an external process.

- Strategy to evict items under memory pressure "CacheItemPriority":
The IMemoryCache cache will evict cache entries under memory pressure unless the cache priority is set to CacheItemPriority.NeverRemove. You can set the CacheItemPriority to adjust the priority the cache evicts items under memory pressure.

- Type of objects for "InMemoryCache":
The in-memory cache can store any object; the distributed cache interface is limited to byte[].

Example "Delegates" with "MemoryCacheEntryOptions"

      //LD STEP999

Postman call: "https://localhost:44342/api/camps/ATL2016/speakers/1/talks/EvictCache"

**InMemory Cache**

The information is stored in the memory of the server.

  - So we deal with a specific server, then works for "Sticky Sessions", subsequest requests need to be done to the same server.
  - It will work with any type of objects.
  - We use the "IMemoryCache" Interface.

- **Cache Tag Helper**

use the same "InMemoryCache" approach but for razor code

      <cacheexpires-after="@TimeSpan.FromSeconds(30)">
            @await Component.InvokeAsync("CategoryMenu")
      </cache>

**Distribuited Cache**

DEFINITION:the cache will not be stored in the specific server memory, but in a centralized place. So the cache will be:

- identical and available for all the servers.
- no sticky sessions required
- not impacted with server reboots

https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed

https://dotnetcoretutorials.com/2017/03/05/using-inmemory-cache-net-core/

**Response Caching**
https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response

https://www.codeproject.com/Articles/1204097/ASP-NET-Core-Response-Caching

DEFINITION: the responce cache is based on Headers that are included on responce. Based on that the client can cache the entire responce and reuse that. This limit the load of the server. We are able to setup the behaviour of the client by doing setting on server side.

to work with responce cache we have to use the "ResponceCache" attribute.

for instance:

      [ResponseCache(Duration = 30, VaryByHeader = "User-Agent")]

we have available options to specify on headers:

- Location (allows to specify where the cache has to happen: any/client/non)
- Duration
- NoStore (cache has to not happen at all)
- VaryByHeader (allows to specify what header we want to check on)

es:

                  services.AddMvc
                (
                    config =>
                    {
                        config.Filters.AddService(typeof(TimerAction));
                        config.CacheProfiles.Add("Default",
                            new CacheProfile()
                            {
                                Duration = 30,
                                Location = ResponseCacheLocation.Any
                            });
                        config.CacheProfiles.Add("None",
                            new CacheProfile()
                            {
                                Location = ResponseCacheLocation.None,
                                NoStore = true
                            });
                    }
                )      


### FILTER test

This topic is not included in the course

I added an "**ActionFilterAttribute**" just for exercise, is possible test it by calling the "TalksController.cs" by postman

      https://localhost:44342/api/camps/ATL2016/speakers/1/talks/1

      //LD STEP004 (multiple times)
      
### REST Constraints

List of REST Constraints:

- Client -Server
- Stateless Server (no state preserved, es session state)
- Cache
- Uniform Interface
- Layered System
- Code -On-Demand

### CACHE (implementation DistribuitedCache)

1 - install nuget packages: "redis-64"

2 - run "redis-server.exe" in the path "C:\Users\Luca\.nuget\packages\redis-64\3.0.503\tools". So now the distribuited cache is running in local

3 - install nuget "Microsoft.Extensions.Caching.Redis" ver.1.1.0

4 - set startup.cs

      //LD STEPdist1

5 - use distributed cache in the controller

      //LD STEPdist2
      //LD STEPdist3

6 - I didn't implement the example, it can be followed from here:

      https://app.pluralsight.com/player?course=aspdotnet-core-mvc-enterprise-application&author=gill-cleeren&name=81741cc0-66db-4b7a-a4fe-673f61981301&clip=7&mode=live
