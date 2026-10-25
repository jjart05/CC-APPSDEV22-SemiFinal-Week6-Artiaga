# CC-APPSDEV22 Midterm Week 6 — Student Roster DB API

ASP.NET Core Web API upgraded from Week 5 with Entity Framework Core relationships,
DTOs, AutoMapper, a service layer with transactions, and JWT authentication.

## Setup

```bash
dotnet restore
dotnet ef database update
```

## Run

```bash
dotnet watch run
```

Open Swagger UI at `/swagger`.

## Test accounts (mock)

- Admin: `{"username": "admin", "password": "password123"}`
- User: `{"username": "user", "password": "password123"}` (Attempt 2 — can view students, cannot delete)

## Attempt 2 additions

- `DeleteStudentAsync` logs an audit row inside a transaction
- Global exception handler returns a clean JSON 500
- `POST /api/courses` plus `courseIds` on student create for many-to-many linking
