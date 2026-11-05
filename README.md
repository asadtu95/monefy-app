# Monefy

A personal money-tracking app.

- **backend/** – ASP.NET Core 8 Web API (Entity Framework Core + PostgreSQL)
- **frontend/** – Angular 21 app
- **Database & auth** – [Supabase](https://supabase.com) (PostgreSQL + Supabase Auth)

---

## Prerequisites

Install these once on the machine:

| Tool | Version | Download |
|------|---------|----------|
| Git | any recent | https://git-scm.com/downloads |
| .NET SDK | 8.x | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Node.js | 20.19+ or 22 LTS (includes npm) | https://nodejs.org |

Check they are installed:

```bash
git --version
dotnet --version
node --version
npm --version
```

You also need the **Supabase database connection string**. Get it from
Supabase dashboard → your project → **Connect** → *Connection pooler* string.
It is a secret and is **not** stored in this repo.

---

## 1. Clone the repo

```bash
git clone https://github.com/asadtu95/monefy-app.git
cd monefy-app
```

---

## 2. Run the backend

```bash
cd backend
```

**Save the database connection string** (only needed once per machine – it is stored
outside the project folder, so it never gets committed):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your Supabase connection string>"
```

**Start the API:**

```bash
dotnet run --launch-profile http
```

- API runs at: http://localhost:5045
- Swagger (API docs): http://localhost:5045/swagger
- Database migrations and seed data are applied automatically on startup.

Leave this terminal open.

---

## 3. Run the frontend

Open a **second** terminal in the repo folder:

```bash
cd frontend
npm install
npm start
```

- App runs at: http://localhost:4200
- `npm install` is only needed the first time and whenever `package.json` changes.

The frontend's development settings (API URL, Supabase URL and public anon key) are in
`frontend/src/environments/environment.development.ts`.

---

## Daily workflow

```bash
git pull                 # get latest changes before you start
# ... make changes ...
git add .
git commit -m "Describe what you changed"
git push
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `ConnectionStrings:DefaultConnection is not configured` | Run the `dotnet user-secrets set ...` command from step 2 inside `backend/`. |
| Backend can't connect to the database | Check the connection string/password, and that the Supabase project isn't paused. |
| Frontend loads but API calls fail (CORS / network error) | Make sure the backend is running on http://localhost:5045 and the frontend on http://localhost:4200. |
| `ng: command not found` | Use `npm start` instead of `ng serve` (it uses the project's local Angular CLI). |
| Port already in use | Close the other app using port 5045 / 4200, or stop old `dotnet` / `node` processes. |
