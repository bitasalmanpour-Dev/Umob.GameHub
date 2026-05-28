# Umob.GameHub

Umob.GameHub is an application built for the umob Backend Software Engineer assignment. The application uses GBFS data from shared mobility providers to generate multiple-choice quiz questions. Users can register, log in, start a one-minute game attempt, answer as many questions as possible, and view their previous attempts and scores.

## Assignment Summary

The goal of the assignment is to build a functional application that:

- Integrates at least three GBFS providers from the MobilityData `systems.csv` list.
- Allows users to create an account and authenticate.
- Generates multiple-choice questions based on GBFS feed data.
- Gives the user one minute to answer as many questions as possible.
- Awards `+50` points for each correct answer.
- Deducts `-20` points for each wrong answer.
- Considers the user a winner if their score remains positive at the end of the game.
- Allows the user to view previous game attempts and restart the game.

## Tech Stack

- **.NET 8 / ASP.NET Core Web API**
- **SQL Server 2022**
- **Entity Framework Core**
- **MediatR**
- **FluentValidation**
- **JWT Bearer Authentication**
- **NUnit and Moq for unit tests**
- **Microsoft PasswordHasher**
- **HttpClientFactory**
- **Swagger / OpenAPI**

## Architecture Overview

The project is organized into four main projects:

### 1. Umob.GameHub.Domain

Contains the core domain entities and business concepts.

Example responsibilities:

- User entity
- Game attempt entity
- Game attempt question entity
- Game attempt question option entity
- GBFS provider entity
- Provider feed entity
- Question template entity
- Question template GBFS field entity

This layer does not depend on external libraries such as Entity Framework Core or ASP.NET Core.

### 2. Umob.GameHub.Application

Contains the application use cases, commands, queries, handlers, validators, DTOs, and abstractions.

Example responsibilities:

- Register user
- Login user
- Start game attempt
- Generate questions
- Generate dynamic multiple choice options
- Submit answer
- Calculate score base on strategy type
- Get user game history
- Define interfaces such as repositories, JWT token service, password hashing service, and GBFS services

This layer depends on the Domain layer but not on Infrastructure or API.

### 3. Umob.GameHub.Infrastructure

Contains technical implementations for external concerns.

Example responsibilities:

- Entity Framework Core `ApplicationDbContext`
- Repository implementations
- SQL Server configuration
- JWT token generation
- Password hashing implementation
- GBFS HTTP client implementation
- Database entity configurations

This layer implements the abstractions defined in the Application layer.

### 4. Umob.GameHub API / Presentation

Contains the ASP.NET Core Web API project.

Example responsibilities:

- Controllers
- Swagger configuration
- Web page
- Dependency injection setup
- API routing
- HTTP request/response handling

Controllers are intentionally thin and delegate business logic to MediatR handlers.

## Database Overview

The MVP database design includes the following main tables:

### Users

Stores registered users.

Important fields:

- `Id`
- `Email`
- `Username`
- `PasswordHash`
- `CreatedOn`
- `ModifiedOn`

### GbfsProviders

Stores selected GBFS providers.

Important fields:

- `Id`
- `Name`
- `GbfsAutoDiscoveryUrl`
- `CreatedOn`

The `GbfsAutoDiscoveryUrl` is important because GBFS providers expose a discovery document first. The application reads this document to find feed URLs such as `station_information`, `station_status`, and `vehicle_status`.

### QuestionTemplates

Stores static question templates.

Example:

```text
Which provider has the highest number of available bikes?
```

Question templates do not store generated runtime answers directly.

### QuestionTemplateGbfsFields

Stores the GBFS feed and field configuration needed to calculate answers for each question template.

Important fields:

- `QuestionTemplateId`
- `ProviderId`
- `FeedName`
- `CollectionPath`
- `FieldName`
- `FieldRole`
- `CreatedOn`

This table makes it possible to know which provider, feed, collection, and JSON field should be used to calculate the answer for a specific question.

### GameAttempts

Stores each one-minute game round for a user.

Important fields:

- `Id`
- `UserId`
- `StartedOn`
- `EndedOn`
- `Score`
- `IsWon`
- `CreatedOn`

A new row is created when the user starts a new game attempt.

### GameAttemptQuestions

Stores generated runtime questions for a specific game attempt.

Important fields:

- `Id`
- `GameAttemptId`
- `QuestionTemplateId`
- `ProviderId`
- `QuestionText`
- `OrderNo`
- `IsAnswered`
- `AnsweredOn`
- `CreatedOn`

### GameAttemptQuestionOptions

Stores generated multiple-choice options for each runtime question.

Important fields:

- `Id`
- `GameAttemptQuestionId`
- `OptionKey`
- `OptionText`
- `IsCorrect`
- `CreatedOn`

The frontend submits the selected option ID. The backend checks the `IsCorrect` field and updates the score.

## Authentication Flow

The application uses simple JWT authentication.

### Register

```http
POST /api/v1/auth/register
```

Creates a new user, hashes the password, saves the user, and returns a JWT access token.

### Login

```http
POST /api/v1/auth/login
```

Validates the user credentials and returns a JWT access token.


## Game Flow

### 1. User registers or logs in

The user receives a JWT token.

### 2. User starts a game attempt

The backend creates a new `GameAttempts` record.

### 3. Backend generates questions

The backend uses question templates and GBFS field mappings to generate multiple-choice questions from GBFS data.

For the current MVP, GBFS data is fetched from external provider feeds at runtime. The application uses the configured provider, feed name, collection path, and field name stored in database to read the required values and generate answers dynamically.

### 4. User submits an answer

The frontend submits the selected option ID once user choose an option.

The backend:

- Finds the selected option.
- Checks whether it is correct.
- Adds `+50` points for a correct answer.
- Deducts `-20` points for a wrong answer.
- Marks the question as answered.

### 5. Game ends after one minute

When the game ends, the backend stores the final score and calculates whether the user won.

The user wins if the final score is positive. Frontend is responsible for validity time of the game.

### 6. User can view previous attempts

The user can see previous game attempts and scores.

## Instructions to Build and Run

### Prerequisites

Make sure the following tools are installed:

- .NET 8 SDK
- SQL Server
- SQL Server Management Studio or another SQL client
- Visual Studio 2022, Rider, or VS Code
- Git

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Umob.GameHub
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure the Database Connection

Update the connection string in `appsettings.json` or `appsettings.Development.json` inside the API project.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UmobGameHub;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

If SQL Server uses username and password authentication, use a connection string similar to:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UmobGameHub;User Id=<username>;Password=<password>;TrustServerCertificate=True"
  }
}
```

### 4. Create and Prepare the Database

To create and prepare the database, run the `script.sql` file.

### 5. Configure JWT Settings

Add JWT settings to the API configuration file.

Example:

```json
{
  "Jwt": {
    "Issuer": "Umob.GameHub",
    "Audience": "Umob.GameHub",
    "SecretKey": "replace-this-with-a-secure-secret-key-for-development",
    "ExpirationInMinutes": 60
  }
}
```

For production, secrets should not be stored directly in `appsettings.json`. They should be stored in environment variables, user secrets, or a secure secret manager.


### 6. Run the API

```bash
dotnet run --project src/Presentation/Umob.GameHub
```

The API should start locally. Swagger is usually available at:

```text
https://localhost:<port>/swagger
```

or:

```text
http://localhost:<port>/swagger
```

The exact port depends on the local launch settings.


## GBFS Integration Approach

GBFS stands for General Bikeshare Feed Specification. It is a standardized JSON format used by shared mobility providers to publish data about systems, stations, vehicles, pricing, alerts, and availability.

The application does not use GBFS as a booking, payment, or unlocking service. It only consumes published GBFS feed data.

The integration flow is:

1. Store selected providers in `GbfsProviders`.
2. Read each provider's `GbfsAutoDiscoveryUrl`.
3. Parse the discovery document.
4. Find required feed URLs such as:
   - `system_information`
   - `station_information`
   - `station_status`
   - `vehicle_status` or `free_bike_status`
5. Fetch the required feed data.
6. Store or process the feed data.
7. Generate quiz questions based on configured GBFS fields manually.

## Error Handling and Validation

The application uses FluentValidation for request validation. Invalid requests are rejected before they reach business handlers.

Examples of validation rules:

- Email is required.
- Email must have a valid format.
- Password is required.
- Username is required.
- Selected option ID is required when submitting an answer.

Business errors are handled inside the application layer. For example:

- Registering with an already existing email should return an appropriate error.
- Submitting an answer for an expired game attempt should not be allowed.
- Submitting an answer for an already answered question should not be allowed.


## Things I Would Improve With More Time

### 1. Improve GBFS Data Synchronization

I would add a background job to periodically fetch and refresh GBFS data from providers instead of fetching it manually or during request handling.

Possible improvements:

- Retry policy
- Timeout handling
- Provider health status

### 2. Add Caching

GBFS data can change frequently, but it should not be fetched from external providers for every gameplay request.

With more time, I would add caching using Redis or in-memory caching to improve performance and reduce external API calls.

### 3. Add More Question Types

The MVP uses static question templates and configured GBFS fields. With more time, I would add more advanced question generation logic.

Examples:

- Compare availability between providers.
- Ask about stations with the highest or lowest number of bikes.
- Ask about vehicle types.
- Ask about system information.
- Ask about pricing plans.
- Ask about alerts or service status.

### 4. Add Integration Tests

I would add integration tests for the full API flow:

- Register
- Login
- Start game
- Answer question
- Finish game
- Retrieve history

These tests would use a test database or containerized SQL Server.

### 5. Improve Observability

For production readiness, I would add better logging and monitoring.

Possible improvements:

- Structured logging
- Correlation IDs
- Request/response logging
- Application metrics
- Health checks
- External provider monitoring


## Final Notes

This project focuses on delivering a clean, maintainable backend MVP for the assignment. The main priorities are:

- Clear architecture
- Simple authentication
- GBFS data integration
- Dynamic question generation and multiple choice options
- Game attempt and score tracking
- Easy local setup

The architecture keeps the solution flexible enough to add caching, background jobs, deployment, and more advanced analytics later.

