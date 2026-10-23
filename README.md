# Employee Management System

Full-stack Employee Management System built for the BDO IT Consulting Internship 2026 (.NET Full-Stack Learning Work Plan).

Users can create, view, update and delete employee records through a React interface backed by an ASP.NET Core Web API and a SQL Server database, all running in Docker Compose.

## Tech Stack

| Layer      | Technology                          |
|------------|-------------------------------------|
| Frontend   | React (Vite)                        |
| Backend    | C#, ASP.NET Core Web API            |
| Data access| Entity Framework Core               |
| Database   | SQL Server                          |
| Testing    | xUnit                               |
| Containers | Docker, Docker Compose              |

## Project Structure

```
employee-management-system/
├── backend/
│   ├── EmployeeManagement.Api/      # ASP.NET Core Web API
│   └── EmployeeManagement.Tests/    # xUnit test project
├── frontend/                        # React app
├── docker-compose.yml
├── .env.example                     # Template for environment variables
└── README.md
```

## API Endpoints

| Method | Endpoint               | Description             |
|--------|------------------------|-------------------------|
| GET    | /api/employees         | Retrieve all employees  |
| GET    | /api/employees/{id}    | Retrieve employee by ID |
| POST   | /api/employees         | Create a new employee   |
| PUT    | /api/employees/{id}    | Update employee details |
| DELETE | /api/employees/{id}    | Delete an employee      |

## Setup

_To be completed on Day 3 (Docker Compose instructions)._

## Running Tests

_To be completed on Day 3._

## Git Workflow

- `main` is protected; all work happens on feature branches.
- Branch names: `feature/...`, `bugfix/...`, `hotfix/...`, `chore/...` (lowercase, hyphenated).
- Commit messages: `type: short description` (`feat`, `fix`, `refactor`, `docs`, `test`, `chore`).
- Push before 4:00 PM daily; unfinished work goes on a `-wip` branch.
- Merge to `main` only through a reviewed Pull Request.