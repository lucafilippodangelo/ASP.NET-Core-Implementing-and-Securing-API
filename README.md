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
