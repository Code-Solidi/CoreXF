# Readme

This is the Presentation layer of Clean Architecture. Controllers is inherited from ASP.NET WebAPI.

### CRUD to HTTP Verbs mappings
1. **C** - Create, **POST**
2. **R** - Read, **GET**
3. **U** - Update, **PUT**
4. **D** - Delete, **DELETE**


This will require a refactoring of Hiro.WebApi.Client and Hiro.Admin.Client. The corresponding return values from WebAPI controller actions should be as described here: https://www.restapitutorial.com/lessons/httpmethods.html.

### REST URLS Best Practices
A good source is https://nordicapis.com/10-best-practices-for-naming-api-endpoints/. This will have an impact on the URL paths defined in 
HiroApiResources.cs, HiroAdminApiResources.cs, and HiroAdminRoutes.cs.

### Hiro - Not Quite Clear and Predictable
Some important questions regrding controllers here:
1. Why all data ops are **POSTs**?! Why not **GET, PUT, POST, DELETE**?
2. Blog ops are quite limited:
    - public const string AdminPostsEdit = "Admin/Posts/EditJson";
    - public const string AdminPostsIndex = "Admin/Posts/IndexJson";
    - public const string AdminPostsDelete = "Admin/Posts/Delete";
    - public const string AdminPostsAddOrUpdate = "Admin/Posts/AddOrUpdate";

so, I see no way to create/manage categories.

3. How does one add comments to blog posts? 
4. How does one add blog categories? 

