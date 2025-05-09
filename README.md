# Running and configuration
### If using docker
Copy in root directory appsettings.json and modify "DefaultConnection" connection string with your hosting db settings.
Create Image...
Host it...

# ERD SCHEMA
```
will be here
```
# INSTALL OR UPDATE DOTNET TOOL
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```


## EF Core commands during development
```
dotnet ef migrations add Initial --project App.DAL.EF --startup-project WebApp --context ApplicationDbContext 

dotnet ef migrations remove --project App.DAL.EF --startup-project WebApp --context ApplicationDbContext 
 
dotnet ef database update --project App.DAL.EF --startup-project WebApp --context ApplicationDbContext

dotnet ef database drop --project App.DAL.EF --startup-project WebApp
```

# Web Controllers scaffolding

Mandatory packages in WebApp for scaffolding

Microsoft.VisualStudio.Web.CodeGeneration.Design
Microsoft.EntityFrameworkCore.SqlServer


# MVC

cd WebApp
```
dotnet aspnet-codegenerator controller -name AllergensController       -actions -m  Allergen    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name CategoriesController       -actions -m  Category    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name FoodsController       -actions -m  Food    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name FoodAllergensController       -actions -m  FoodAllergen    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name FoodIngredientsController       -actions -m  FoodIngredient    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name FoodNutrientsController       -actions -m  FoodNutrient   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name IngredientsController       -actions -m  Ingredient   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name NutrientsController       -actions -m  Nutrient   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name IngredientNutrientsController       -actions -m  App.Domain.IngredientNutrient    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name OpenHoursController       -actions -m  OpenHours   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name RestaurantsController       -actions -m  Restaurant   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name SubAdminsController       -actions -m  SubAdmin   -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name UnitsController       -actions -m  Unit    -dc ApplicationDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
```


# REST API CONTROLLERS

cd WebApp
```
dotnet aspnet-codegenerator controller -name RestaurantsController -actions -m App.Domain.Restaurant -dc ApplicationDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name UnitsController -actions -m App.Domain.Unit -dc ApplicationDbContext -outDir ApiControllers -api --useAsyncActions -f
```


Generate Identity UI
~~~bash
- cd WebApp
dotnet aspnet-codegenerator identity -dc App.DAL.EF.ApplicationDbContext --userClass AppUser -f
~~~

# DOCKER
1. Create a file called ***docker-compose.yml***
2. Add necessary stuff there, services, volumes, environment, ports
3. In VS Code, choose compose restart
4. In Dbeaver connect to a database. Using correct port. In PostgreSql choose **show all databases**
   


# REPOSITORIES
1. Create in App.Contracts.DAL Repository interfaces for every Domain object:
   **interface is derived from IBaseRepository**
2. In App.DAL.EF directory called Repositories create actual Repositories that implements previously created interfaces
3. Use Repositories in Controllers.


# Restaurant Opening Hours - Representation of Days of the Week
This part outlines the agreed-upon format and values for representing days of the week in the Restaurant Opening Hours application. Consistency between the frontend and backend ensures smooth communication and data handling.

### Frontend Representation
In the frontend (Vue), days of the week will be represented as strings in the following format:

- 'Sunday'
- 'Monday'
- 'Tuesday'
- 'Wednesday'
- 'Thursday'
- 'Friday'
- 'Saturday'
### Backend Expectation 
In the backend (C#), the Day property will expect and handle days of the week using the same string format as specified above.


### Upload CSV
- To successfully map ingredients and nutrients CSV file should have this headers: Name (ENG), Energy, Fat, Saturated fatty acids, Carbohydrates, Sugar, Fiber, Protein, Salt
- CsvRecordDto keeps all headers that should be mapped to objects in code.
- In database we keep 7 basic nutrients in lowercase, so on search make it sure that search string is lowercased
