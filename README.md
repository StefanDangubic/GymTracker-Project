# GymTracker

GymTracker is a full-stack web application for logging and reviewing gym workouts. Users can register, track their training sessions, and monitor their progress over time through a dashboard and weekly/monthly statistics.

## Features

- User registration and login
- Workout CRUD (create, read, update, delete)
- Workout types: Cardio, Strength, Flexibility
- Duration, calories, intensity, fatigue, notes, and date/time per workout
- Dashboard with current-month statistics and recent workouts
- Monthly progress view with weekly statistics
- Frontend and backend validation
- User data ownership (users can only access their own workouts)
- Responsive UI

## Technologies

### Backend

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT authentication
- `PasswordHasher`
- Swagger / OpenAPI

The backend follows Clean Architecture, separating the application into distinct layers.

### Frontend

- Angular 21
- Standalone components
- Reactive Forms
- TypeScript

## Architecture

| Layer          | Responsibility                                        |
| -------------- | ------------------------------------------------------ |
| Domain         | Entities and enums                                    |
| Application    | Business logic, DTOs and repository interfaces        |
| Infrastructure | EF Core, database and repository implementations      |
| API            | Controllers, middleware and application configuration |

The frontend is organized into `core` (auth, guards, interceptors), `shared` (reusable components, models, utils), and `features` (auth, dashboard, workouts, progress).

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 20+
- npm
- Angular CLI 21.x (`npm install -g @angular/cli`, or use `npx ng`)
- SQL Server LocalDB or another SQL Server instance

### Clone the repository

```bash
git clone https://github.com/StefanDangubic/GymTracker-Project.git
cd GymTracker-Project
```

### Backend setup

```bash
cd backend/src/GymTracker.Api
dotnet restore
dotnet build
```

In Development, the application automatically applies pending EF Core migrations and creates the database on first run. By default it connects to a local LocalDB instance (`(localdb)\MSSQLLocalDB`); update the connection string in `appsettings.Development.json` if you want to use a different SQL Server instance.

### JWT configuration

The JWT signing key is not committed to the repository and must be configured locally using .NET user secrets:

```bash
dotnet user-secrets set "Jwt:Key" "<your-secret-key>"
```

### Run backend

```bash
dotnet run
```

The API runs at `http://localhost:5186`, with Swagger UI available at `http://localhost:5186/swagger`.

### Frontend setup

```bash
cd frontend
npm install
npm start
```

The frontend runs at `http://localhost:4200`. The Angular development proxy forwards `/api` requests to the backend at `http://localhost:5186`.

## Notes

 Run the backend and frontend applications. The backend must be running for API requests from the frontend to succeed. The JWT signing key must be set via user secrets, or the API will not be able to issue or validate tokens.
