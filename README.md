# Event Registration

## Prerequisites
- .NET 9 SDK (download: https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- You can use Visual Studio 2022+ or JetBrains Rider as an alternative IDE.

## Install and run
1. Clone repository and open in VS Code
```bash
git clone https://github.com/Danwerk/event-registration.git
cd event-registration
dotnet restore
code .
```
2. Run F5


# Architecture Overview

## Project Structure
| Project | Description                                                                                                                                                                      |
|:--------|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Base.Contracts.DAL** | Defines base interfaces for generic Unit of Work and Repository patterns.                                                                                                        |
| **Base.DAL.EF** | Provides EF Core implementations for the base repository and unit of work patterns.                                                                                              |
| **Base.Contracts.Domain** | Common domain contracts, like `IDomainEntityId` (base ID support for entities).                                                                                                  |
| **Base.Domain** | Basic abstract domain classes, e.g., `DomainEntityId`, which provides ID handling.                                                                                               |
| **App.Domain** | Contains domain-specific entities like `Event`, `Participant`, `PrivatePerson`, and `LegalPerson`.                                                                               |
| **App.Contracts.DAL** | Application-specific repository interfaces extending from base repository contracts.                                                                                             |
| **App.DAL.EF** | Concrete repository implementations using EF Core for App domain models, Migrations and data seeding. AppDbContext is located here and is responsible for database interaction. |
| **WebApp** | ASP.NET Core MVC project handling HTTP requests, controllers, views, and frontend logic.                                                                                         |
| **Tests.WebApp** | Contains unit and integration tests for API endpoints and application logic.                                                                                                     |

## Layers and Responsibilities

| Layer | Responsibility |
|:------|:----------------|
| Domain | Defines **entities** (data structures) without persistence or business logic. |
| Contracts.DAL | Defines **interfaces** for data access (repositories). |
| DAL.EF | Provides **actual database access** using EF Core, implements repositories. |
| WebApp | Handles **user interaction**, **HTTP requests**, and renders **views**. |
| Tests.WebApp | Ensures **application correctness** with automated tests. |


## Main Architectural Patterns

### 1. Repository Pattern
- Generic Repository interface:  
  `IBaseRepository<TEntity, TKey>`
- Provides CRUD operations (`Add`, `Update`, `Remove`, `FindAsync`, etc.) for entities.
- EF Core-based implementation:  
  `EFBaseRepository<TEntity, TKey, TDbContext>`

### 2. Unit of Work (UOW)
- Interface: `IBaseUOW`
- Concrete class: `EFBaseUOW<TDbContext>`
- Manages saving changes (`SaveChangesAsync()`) in one atomic transaction.

### 3. Domain Layer
- Each entity implements `IDomainEntityId<TKey>` (by default `Guid`) for a unique identifier.
- Shared domain logic (e.g., `DomainEntityId`) provided via `Base.Domain`.

### 4. Layered Clean Architecture
Separation of concerns between:
- **Presentation Layer** (WebApp — MVC Controllers + Razor Views)
- **Application Layer** (Repositories, UOW coordination)
- **Persistence Layer** (EF Core)
- **Domain Layer** (Models/entities)

## Entity Relationships
- **Event** has many **Participants** (via `EventParticipant`).
- **Participant** can be either:
  - `PrivatePerson` (individual) or
  - `LegalPerson` (company).

# DEVELOPMENT
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
dotnet aspnet-codegenerator controller -name PaymentMethodsController       -actions -m  PaymentMethod    -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
```