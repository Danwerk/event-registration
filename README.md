TODOs
- Andmebaasi diagramm
- Testimine xUnit (Unit Test EventParticipantRepository või ParticipantRepository peale. Integration Test API või Controllerite vastu (nt xUnit + TestServer).)
- dokumentatsioon, 
- ilmselt hea oleks dockeri konteiner teha
- 

# Running and configuration
### If using docker
Copy in root directory appsettings.json and modify "DefaultConnection" connection string with your hosting db settings.
Create Image...
Host it...

# INSTALL OR UPDATE DOTNET TOOL
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator

```


## EF Core commands during development
```
dotnet ef migrations add Initial --project App.DAL.EF --startup-project WebApp --context AppDbContext 

dotnet ef migrations remove --project App.DAL.EF --startup-project WebApp --context AppDbContext 
 
dotnet ef database update --project App.DAL.EF --startup-project WebApp --context AppDbContext

dotnet ef database drop --project App.DAL.EF --startup-project WebApp
```

# Web Controllers scaffolding

Mandatory packages in WebApp for scaffolding

Microsoft.VisualStudio.Web.CodeGeneration.Design
Microsoft.EntityFrameworkCore.SqlServer


# MVC

cd WebApp
```
dotnet aspnet-codegenerator controller -name EventsController       -actions -m  Event    -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name ParticipantsController       -actions -m  Participant    -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
```


# REST API CONTROLLERS

cd WebApp
```
dotnet aspnet-codegenerator controller -name UnitsController -actions -m App.Domain.Unit -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
```


Generate Identity UI
~~~bash
- cd WebApp
dotnet aspnet-codegenerator identity -dc App.DAL.EF.AppDbContext --userClass AppUser -f
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