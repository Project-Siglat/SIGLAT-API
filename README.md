# SIGLAT-API

Emergency Response System - Backend API

> A robust ASP.NET Core Web API for emergency response management with PostgreSQL database integration.

## 🚀 Features

- **RESTful API**: Complete REST API for emergency management
- **Authentication & Authorization**: JWT-based auth with role management
- **Database Integration**: PostgreSQL with Entity Framework Core
- **Real-time Capabilities**: Chat system and emergency alerts
- **Admin Management**: User verification and system administration
- **Report Analytics**: Emergency report generation and analytics
- **Scalable Architecture**: Clean architecture with service layers

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core 8.0.11
- **Authentication**: JWT Bearer tokens with BCrypt password hashing
- **ORM**: Entity Framework Core + Dapper (dual strategy)
- **Documentation**: Swagger/OpenAPI
- **Containerization**: Docker with multi-stage builds

## 📋 Prerequisites

- .NET 8.0 SDK
- PostgreSQL 13+
- Docker (optional, for containerized deployment)

## 🚀 Quick Start

### Development Setup

```bash
# Clone the repository
git clone git@github.com:Craftmatrix24/SIGLAT-API.git
cd SIGLAT-API

# Restore dependencies
dotnet restore

# Update database connection string in appsettings.json
# Run database migrations
dotnet ef database update

# Start development server
dotnet run

# API will be available at https://localhost:7045
```

### Environment Configuration

Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=siglat_db;Username=your_user;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-at-least-32-characters",
    "Issuer": "SIGLAT-API",
    "Audience": "SIGLAT-Client",
    "ExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📁 Project Structure

```
├── Controllers/              # API Controllers
│   ├── AdminController.cs    # Admin management endpoints
│   ├── AuthController.cs     # Authentication endpoints
│   ├── ChatController.cs     # Chat/messaging system
│   ├── ReportController.cs   # Emergency reports
│   ├── UserController.cs     # User management
│   └── [Empty Controllers]   # TODO: Implement missing endpoints
│       ├── BFPController.cs      # Bureau of Fire Protection
│       ├── PNPController.cs      # Philippine National Police
│       ├── SiglatController.cs   # Main emergency alerts
│       ├── TyphoonController.cs  # Typhoon management
│       └── FloodController.cs    # Flood management
├── Data/
│   └── AppDBContext.cs       # Entity Framework context
├── Models/                   # Data Transfer Objects (DTOs)
│   ├── AdminModels.cs
│   ├── AuthModels.cs
│   ├── ChatModels.cs
│   ├── ReportModels.cs
│   └── UserModels.cs
├── Services/                 # Business logic services
│   ├── DatabaseService.cs   # Database operations
│   └── UtilityService.cs     # Utility functions
├── Middlewares/
│   └── ErrorHandlingMiddleware.cs  # Global error handling
├── Migrations/               # 51 EF Core migrations
├── Program.cs               # Application entry point
└── appsettings.json        # Configuration
```

## 🗄️ Database Schema

### Core Tables

- **Identity**: User management with roles (Admin, User, Ambulance)
- **Verifications**: User verification system
- **Reports**: Emergency reports and incidents
- **Chat**: Messaging system for coordination
- **LoginLogs**: Authentication audit trail
- **Coordinates**: User location tracking
- **Contact**: Emergency contact information
- **Alerts**: Emergency alert system

### Entity Relationships

```
Identity (Users) ←→ Reports (One-to-Many)
Identity (Users) ←→ Chat (One-to-Many)
Identity (Users) ←→ Coordinates (One-to-Many)
Identity (Users) ←→ Verifications (One-to-One)
```

## 🛡️ API Endpoints

### Authentication (`/api/v1/auth`)
```
POST   /register          # User registration
POST   /login             # User authentication
GET    /profile           # Get current user profile
PUT    /profile           # Update user profile
```

### Admin Management (`/api/v1/admin`)
```
GET    /users             # Get all users
GET    /users/{id}        # Get user by ID
PUT    /users/{id}/verify # Verify user account
DELETE /users/{id}        # Delete user
GET    /verifications     # Get pending verifications
GET    /contacts          # Get emergency contacts
POST   /contacts          # Add emergency contact
```

### Reports (`/api/v1/report`)
```
GET    /                  # Get all reports
GET    /{id}              # Get report by ID
POST   /                  # Create new report
PUT    /{id}              # Update report
DELETE /{id}              # Delete report
GET    /analytics         # Get report analytics
```

### Chat (`/api/v1/chat`)
```
GET    /                  # Get chat messages
POST   /                  # Send message
GET    /{id}              # Get specific message
```

### User Operations (`/api/v1/user`)
```
POST   /coordinates       # Update user location
GET    /coordinates       # Get user location
```

## 🔐 Security Features

### Authentication & Authorization
- **JWT Bearer Tokens**: Secure stateless authentication
- **BCrypt Password Hashing**: Industry-standard password security
- **Role-based Authorization**: Admin, User, Ambulance roles
- **Token Expiration**: Configurable token lifetime

### Security Middleware
- **CORS Configuration**: Cross-origin request handling
- **Error Handling**: Secure error responses
- **Input Validation**: Data annotation validation
- **SQL Injection Protection**: Parameterized queries

## 🐳 Docker Deployment

### Build and Run

```bash
# Build Docker image
docker build -t siglat-api .

# Run with environment variables
docker run -d \
  --name siglat-api \
  -p 5069:80 \
  -e ConnectionStrings__DefaultConnection="Host=db;Database=siglat;Username=postgres;Password=password" \
  -e Jwt__Key="your-secret-key" \
  siglat-api
```

### Docker Compose (Full Stack)

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5069:80"
    depends_on:
      - db
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=siglat;Username=postgres;Password=password
      
  db:
    image: postgres:13
    environment:
      - POSTGRES_DB=siglat
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=password
    volumes:
      - postgres_data:/var/lib/postgresql/data
      
volumes:
  postgres_data:
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# TODO: Add comprehensive unit tests
# TODO: Add integration tests
```

## 📊 Database Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Generate SQL script
dotnet ef migrations script
```

## 🔧 Configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=siglat_db;Username=postgres;Password=password;Port=5432"
  }
}
```

### JWT Configuration
```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key-here",
    "Issuer": "SIGLAT-API",
    "Audience": "SIGLAT-Client",
    "ExpiryDays": 7
  }
}
```

### CORS Settings
```json
{
  "AllowedOrigins": [
    "http://localhost:5173",
    "https://siglat-ui.com"
  ]
}
```

## 📈 Performance & Monitoring

### Database Performance
- **Connection Pooling**: EF Core connection pooling
- **Async Operations**: All database operations are async
- **Indexed Queries**: Proper database indexing
- **Query Optimization**: Efficient LINQ queries

### Monitoring
```bash
# Health check endpoint
GET /health

# Application metrics
GET /metrics

# TODO: Add structured logging with Serilog
# TODO: Add application monitoring
```

## 🚧 Known Issues & TODOs

### Critical
- [ ] **Empty Controllers**: Implement BFP, PNP, SIGLAT, Typhoon, Flood endpoints
- [ ] **Dual Database Pattern**: Standardize on EF Core OR Dapper, not both
- [ ] **Missing Tests**: Add comprehensive unit and integration tests

### High Priority
- [ ] **Real-time Features**: Implement SignalR for live updates
- [ ] **Logging**: Add structured logging with Serilog
- [ ] **Caching**: Implement Redis caching for performance
- [ ] **Rate Limiting**: Add API rate limiting

### Medium Priority
- [ ] **API Versioning**: Implement proper API versioning
- [ ] **Pagination**: Add pagination for large datasets
- [ ] **Background Jobs**: Add Hangfire for background processing
- [ ] **Health Checks**: Comprehensive health check endpoints

## 🛣️ Roadmap

### Phase 1: Core Completion
- Complete missing controller implementations
- Add comprehensive testing suite
- Standardize database access patterns

### Phase 2: Performance & Scalability
- Implement caching strategy
- Add real-time capabilities with SignalR
- Performance optimization and monitoring

### Phase 3: Advanced Features
- Microservices architecture consideration
- Event-driven architecture for alerts
- Advanced analytics and reporting

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Use async/await for all I/O operations
- Add XML documentation for public APIs
- Include unit tests for new features
- Update API documentation

## 🔍 API Documentation

- **Swagger UI**: Available at `/swagger` when running in Development
- **OpenAPI Spec**: Available at `/swagger/v1/swagger.json`
- **Postman Collection**: Import the OpenAPI spec into Postman

## 📄 License

This project is licensed under the ISC License.

## 📞 Support

For support and questions:
- Create an issue in this repository
- Contact: Craftmatrix24

---

**SIGLAT-API** - Sistema Integrated Geographic Location Alert and Tracking Backend