# Shortly — URL Shortener

A modern URL shortener built with **ASP.NET Core** and **Entity Framework Core**. Create short, memorable links from long URLs and track how many times they've been clicked.

## Features

- **URL Shortening** — Paste a long URL, get a short one instantly.
- **Click Tracking** — Monitor how many times each short link has been accessed.
- **User Authentication** — Register, log in, and manage your own links securely with cookie-based authentication.
- **Auto Redirection** — Short URLs automatically redirect visitors to the original destination.
- **Dashboard** — View all your shortened links, click counts, and copy them with one click.

## Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core 10 (Razor Pages) |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| Auth | Cookie Authentication |
| Hashing | BCrypt.Net-Next |
| Logging | Serilog |
| Frontend | Vanilla CSS (Windows 7 Aero theme) |

## Project Structure

```
Shortly/
├── Application/
│   ├── Interfaces/          # Service contracts (IUserService, ILinkService)
│   └── Services/            # Business logic (UserService, LinkService)
├── Domain/
│   └── Entities/            # User, Link
├── Infrastructure/
│   ├── Persistence/         # AppDbContext (EF Core)
│   ├── Migrations/          # EF Core migrations
│   └── Seed/                # DbInitializer (auto seed)
├── Pages/                   # Razor Pages (Index, Login, Register, Logout)
├── docs/                    # UML diagrams (PlantUML)
│   ├── CLASS_DIAGRAM.puml
│   └── DOMAIN_MODEL.puml
├── wwwroot/css/             # Stylesheets
└── Program.cs               # Entry point and configuration
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run Locally

```bash
git clone https://github.com/VicenteeFigueroa/Shortly.git
cd Shortly/Shortly
dotnet run
```

The app will start at `http://localhost:5237`. The database is automatically created and seeded on first run.

### Default Seed User

| Email | Password |
|---|---|
| `admin@shortly.com` | `admin123` |

## Usage

1. **Register** a new account or log in with the seed user.
2. **Paste** a long URL into the dashboard form and click **Shorten**.
3. **Copy** the generated short URL and share it.
4. **Track** clicks in real time from your dashboard.

## Design Patterns & Principles

- **Dependency Injection** — Services registered via `AddScoped<>` in `Program.cs`.
- **Service Layer** — Business logic encapsulated in `UserService` and `LinkService`.
- **Interface Segregation** — `IUserService` and `ILinkService` define separate contracts.
- **Encapsulation** — Entities use `private set`, `private` constructors, and `sealed` classes.
- **PRG Pattern** — Post-Redirect-Get prevents duplicate form submissions on refresh.

## Author

**Vicente Figueroa**

## License

This project is for academic purposes.
