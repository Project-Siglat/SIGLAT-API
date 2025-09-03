# SIGLAT-API

Emergency Response System Backend API built with ASP.NET Core 8.0

## System Architecture

```mermaid
graph TB
    Client[Mobile/Web Client] --> API[SIGLAT API]
    API --> Auth[JWT Authentication]
    API --> DB[(PostgreSQL Database)]
    API --> Swagger[API Documentation]
    
    subgraph "API Controllers"
        API --> AlertC[Alert Controllers]
        API --> ReportC[Report Controller]
        API --> ChatC[Chat Controller]
        API --> AdminC[Admin Controller]
        API --> UserC[User Controller]
    end
    
    subgraph "Emergency Services"
        AlertC --> BFP[Bureau of Fire Protection]
        AlertC --> PNP[Philippine National Police]
        AlertC --> SIGLAT[SIGLAT Emergency]
    end
    
    subgraph "Disaster Management"
        API --> Flood[Flood Management]
        API --> Typhoon[Typhoon Management]
    end
```

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 12+
- Resend.com account (for email services)

### Environment Setup

1. **Clone the repository**
```bash
git clone git@github.com:Siglat/SIGLAT-API.git
cd SIGLAT-API
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure environment variables**
```bash
cp .env.example .env
```

Edit the `.env` file with your configuration:

```env
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASS=your-database-password
DB_DB=siglat-localdb

# JWT Configuration (IMPORTANT: Change in production!)
JWT_SECRET=your-super-secret-jwt-key-minimum-32-characters
JWT_ISSUER=siglat.app
JWT_AUDIENCE=siglat-api

# Email Service (Get API key from https://resend.com/api-keys)
RESEND_API_KEY=re_your_resend_api_key_here

# API Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000

# CORS Origins
CORS_ORIGINS=http://localhost:2424,http://localhost:2425,http://localhost:2426
```

4. **Setup PostgreSQL Database**
```bash
# Create database (if not exists)
createdb siglat-localdb

# The application will automatically create tables on first run
```

5. **Run the application**
```bash
dotnet run
```

6. **Verify installation**
- API: http://localhost:5000
- Swagger Documentation: http://localhost:5000/swagger

### Environment Variables Reference

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `DB_HOST` | ✅ | PostgreSQL host | `localhost` |
| `DB_PORT` | ✅ | PostgreSQL port | `5432` |
| `DB_USER` | ✅ | Database username | `postgres` |
| `DB_PASS` | ✅ | Database password | `your-password` |
| `DB_DB` | ✅ | Database name | `siglat-localdb` |
| `JWT_SECRET` | ✅ | JWT signing key (32+ chars) | `your-secret-key` |
| `JWT_ISSUER` | ✅ | JWT issuer | `siglat.app` |
| `JWT_AUDIENCE` | ✅ | JWT audience | `siglat-api` |
| `RESEND_API_KEY` | ✅ | Resend.com API key | `re_xxxx` |
| `ASPNETCORE_ENVIRONMENT` | ❌ | Environment | `Development` |
| `ASPNETCORE_URLS` | ❌ | Server URLs | `http://localhost:5000` |
| `CORS_ORIGINS` | ❌ | Allowed origins | `http://localhost:3000` |

## Tech Stack

- **ASP.NET Core 8.0** - Web API framework
- **PostgreSQL** - Primary database
- **Entity Framework Core** - ORM
- **JWT Authentication** - Security
- **Docker** - Containerization

## Project Structure

```
├── Controllers/          # API endpoints
│   ├── Alert/           # Emergency alerts (BFP, PNP, SIGLAT)
│   ├── Calamity/        # Disaster management
│   ├── Feature/         # Chat system
│   ├── Interactor/      # User management
│   └── Reports/         # Incident reporting
├── Models/              # Data models
├── Services/            # Business logic
├── Data/                # Database context
└── Migrations/          # Database migrations
```

## API Endpoints

### API Flow Chart

```mermaid
graph LR
    User[User] --> Auth{Authentication}
    Auth -->|Login/Register| JWT[JWT Token]
    JWT --> API[Protected Endpoints]
    
    API --> Reports[📊 Reports]
    API --> Chat[💬 Chat]
    API --> Admin[👨‍💼 Admin]
    API --> Alerts[🚨 Alerts]
    
    Reports --> CreateR[Create Report]
    Reports --> ListR[List Reports]
    Reports --> GetR[Get Report]
    
    Chat --> SendM[Send Message]
    Chat --> GetM[Get Messages]
    
    Admin --> ManageU[Manage Users]
    Admin --> VerifyU[Verify Users]
    
    Alerts --> BFP[🔥 Fire Protection]
    Alerts --> PNP[👮 Police]
    Alerts --> Emergency[🆘 Emergency]
```

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `GET /api/v1/auth/profile` - Get profile

### Reports
- `GET /api/v1/report` - List reports
- `POST /api/v1/report` - Create report
- `GET /api/v1/report/{id}` - Get report

### Admin
- `GET /api/v1/admin/users` - Manage users
- `PUT /api/v1/admin/users/{id}/verify` - Verify user

### Chat
- `GET /api/v1/chat` - Get messages
- `POST /api/v1/chat` - Send message

## Database Schema

```mermaid
erDiagram
    Identity {
        int id PK
        string email
        string password_hash
        string first_name
        string last_name
        string role
        datetime created_at
        double latitude
        double longitude
    }
    
    Reports {
        int id PK
        int user_id FK
        string title
        string description
        string type
        string status
        datetime created_at
        double latitude
        double longitude
    }
    
    Chat {
        int id PK
        int user_id FK
        string message
        datetime sent_at
        string chat_type
    }
    
    Verifications {
        int id PK
        int user_id FK
        string status
        string remarks
        datetime verified_at
    }
    
    LoginLogs {
        int id PK
        int user_id FK
        datetime login_time
        string ip_address
        string user_agent
    }
    
    Contact {
        int id PK
        string name
        string phone
        string email
        string relationship
    }
    
    Alert {
        int id PK
        string title
        string description
        string alert_type
        string status
        datetime created_at
    }
    
    Identity ||--o{ Reports : creates
    Identity ||--o{ Chat : sends
    Identity ||--|| Verifications : has
    Identity ||--o{ LoginLogs : logs
    Reports }o--|| Alert : triggers
```

Create `.env` file with the following variables:

```env
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASS=your-database-password
DB_DB=siglat-localdb

# JWT Configuration (IMPORTANT: Change in production!)
JWT_SECRET=your-super-secret-jwt-key-minimum-32-characters
JWT_ISSUER=siglat.app
JWT_AUDIENCE=siglat-api

# Email Service Configuration
RESEND_API_KEY=re_your_resend_api_key_here

# API Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000

# CORS Origins
CORS_ORIGINS=http://localhost:2424,http://localhost:2425,http://localhost:2426
```

## Development

```bash
# Run development server
dotnet watch run

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# View API docs
# Navigate to https://localhost:7045/swagger
```

## Docker

```bash
# Build and run
docker build -t siglat-api .
docker run -p 5069:80 siglat-api
```

## Current Status

### Implementation Progress

```mermaid
pie title API Implementation Status
    "Completed" : 60
    "In Progress" : 25
    "Planned" : 15
```

### Feature Status Chart

```mermaid
gantt
    title SIGLAT API Development Timeline
    dateFormat  X
    axisFormat %s
    
    section Core Features
    Authentication & JWT     :done, auth, 0, 3
    Database & Migrations    :done, db, 0, 4
    User Management         :done, users, 0, 3
    
    section Current Features
    Report System           :done, reports, 3, 2
    Chat Messaging          :done, chat, 4, 2
    Admin Panel            :done, admin, 5, 2
    
    section In Development
    Emergency Alerts        :active, alerts, 6, 3
    BFP Integration        :active, bfp, 7, 2
    PNP Integration        :active, pnp, 7, 2
    
    section Planned
    Real-time Notifications :planned, realtime, 9, 2
    Disaster Management     :planned, disaster, 10, 3
    Analytics Dashboard     :planned, analytics, 11, 2
```

✅ **Implemented**
- User authentication & authorization
- Report management system
- Chat messaging
- Admin user management
- Database migrations

⚠️ **In Progress**
- Emergency alert controllers (BFP, PNP, SIGLAT)
- Disaster management endpoints (Flood, Typhoon)
- Real-time notifications

🔄 **Planned**
- Advanced analytics
- Push notifications
- Mobile app integration

## Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/name`)
3. Commit changes (`git commit -m 'Add feature'`)
4. Push to branch (`git push origin feature/name`)
5. Open Pull Request

## License

ISC License

---

**SIGLAT** - Sistema Integrated Geographic Location Alert and Tracking