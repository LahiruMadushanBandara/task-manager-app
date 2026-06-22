## Running the App

1. Run `TaskManagerDB.sql` against SQL Server.
2. Start the API (`dotnet run` in `back-end/TaskManager.API`).
3. Confirm `environment.ts` `apiUrl` matches the API port.
4. Start the frontend (`ng serve` in `front-end/task-manager-ui`).
5. Open http://localhost:4200 and log in with `admin` / `Admin@123`.


## API Endpoints

All `/api/tasks` endpoints require a Basic auth header and operate only on the authenticated user's tasks.

| Method | Route                         | Description                       |
| ------ | ----------------------------- | --------------------------------- |
| POST   | `/api/auth/verify`            | Validates credentials (login)     |
| GET    | `/api/tasks`                  | List tasks (filter + sort params) |
| GET    | `/api/tasks/{id}`             | Get a single task                 |
| POST   | `/api/tasks`                  | Create a task                     |
| PUT    | `/api/tasks/{id}`             | Update a task                     |
| PATCH  | `/api/tasks/{id}/complete`    | Toggle completion status          |
| DELETE | `/api/tasks/{id}`             | Delete a task                     |

**`GET /api/tasks` query parameters:** `search`, `isCompleted`, `priority`, `sortBy` (`title` \| `priority` \| `duedate` \| `createdat`), `sortDir` (`asc` \| `desc`). Filtering and sorting are performed server-side.


## Features

- Username/password authentication (no JWT), credentials sent as HTTP Basic on every request
- Full CRUD on tasks, plus a one-click complete toggle
- Side-by-side task list and add/edit form
- Server-side search, status/priority filtering, and sorting
- Per-user data isolation — users only ever see their own tasks
- Validation, consistent error responses, and user-facing success/error messages


## Notes

- HTTP Basic auth sends base64-encoded credentials on every request, so it should only be used over **HTTPS** in any real deployment.
- The seeded password is hashed with SHA256 + salt for demo purposes; production systems should use a slow password hash such as BCrypt, Argon2, or PBKDF2.
