# Observations
* Async was used the wrong way in some places:
  * Calling .Result will force the async operation to be done synchronously, which results in loss of throughput and can cause deadlocks. Fixed by awaiting.
  * Async Void Methods are not awaited which can cause implicit parallelism. Fixed by Async Task
* No tests. I think tests are very important, I've added some in the .Test assembly.
* Data can become inconsistent:
  * Adding a user with an address was done as two separate transactions. Fixed by doing one transaction.
  * Multiple users could be added with the same login. Fixed by adding unique constraint.
* General logic errors, such as null pointer exception due to calling .First on a empty list. Fixed by considering each individually.
* Possible performance problems:
  * Some queries would shovel the whole table into memory and filter the data there. Fixed by filtering in the database.
  * Some columns that were used in a where clause did not have indexes. Fixed by adding indexes.
* Security:
  * Password was stored in plain text. **Normally I would use a library such as Microsoft's Identity for managing users, because there are a lot of things that can go wrong when you try to do security by yourself, however this seems against the spirit of the test.** I've Implemented password hashing using PBKDF2 with HMAC-SHA256, 128 bit salt, 10000 iterations. Its more secure than plaintext, but I don't think I've done a perfect job.
  * Any user could see any other user's password just by querying it. Fixed by writing a method that redacts it, in a bigger project a more complex approach is needed for hiding sensible data.
* Public api was unfriendly for consumers. Made better by:
  * Following Rest conventions like returning the correct status code, body for post data, etc.
  * Added more metadata to swagger, such as ProducesResponseType.
  * Added api versioning using the route.
* Since UserEntity is a aggregate root, and AddressEntity is not:
  * I've hidden access to the address by not exposing the address without the user.
  * Change the relationship to be one-on-one

# New Architecture
For larger projects I think it makes sense to use clean architecture to structure the solution.
That architecture features a center that is the business Domain. Surrounding the Domain are Assemblies that use/implement things defined in the domain. Separation is done by only letting the surrounding assemblies communicate by using the abstractions provided by the Domain.  

I've added a Domain Assembly that is contains the Entities, Models, Managers(Services) Interfaces.
Referencing just the domain there are:
* Identity.Persistence: which implements database persistance using entity framework. I've removed the Repositories from the initial project, because I think in this case they cause more harm then good. Instead of the repositories I've used DbSet<T> directly, which is conceptually a repository.
* Identity.Security: Does the password hashing logic. Again I wish I didn't have to write it since its really hard to do your own crypto.
* Identity.Security.Test: Added to show where would the test code go. Testing coverage is not very good since I had limited time.
* Identity.Api: Controllers live there. This project has no reference to the dbContext or Entity Framework, It is better isolated as a leaf around the Domain.

Identity.Startup is the startup project, I's purpose is the start the web server and to configure dependency injection, this is why it has a reference to everything.

# Future Improvements
* The app needs Authorization. Depending on what the clients are a possible approach is using JWT's, OIDC.
* Logging is important, the actual deployment should read the built in logging data and store it remotely.
* Secrets such as the connection string should not be versioned over git. Azure Key Vault or a similar tool should work.
* Address Entity has two audit columns: CreatedAt, UpdatedAt. If this is the kind of audit we want, we should apply it over all Entities, and be done automatically, using the Entity Framework change tracker.
* Generic error handling and resiliency. Retry policies, replicas, request throttling could be good ideas.
* AddressEntity would benefit from validation. Validating Countries, Cities, State, etc.
* I don't love long Id columns. Guids help if you want to use multiple databases and leak less information than longs. Currently an attacker can randomly query Users by Ids since the Ids are sequential and start at 1.
