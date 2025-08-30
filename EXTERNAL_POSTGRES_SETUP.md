# External PostgreSQL Setup

This API is configured to connect to an **existing PostgreSQL database** rather than creating its own database container.

## Prerequisites

1. **PostgreSQL Server**: Ensure you have a PostgreSQL instance running and accessible
2. **Database Access**: The API needs permissions to create databases and tables
3. **Network Access**: Ensure the API can reach your PostgreSQL server

## Configuration

### 1. Environment Variables

Copy `.env.example` to `.env` and configure your PostgreSQL connection:

```bash
cp .env.example .env
```

Update the database connection settings in `.env`:

```env
DB_HOST=your-postgres-host           # PostgreSQL server hostname/IP
DB_PORT=5432                         # PostgreSQL port
DB_USER=your-postgres-user           # PostgreSQL username  
DB_PASS=your-postgres-password       # PostgreSQL password
DB_DB=siglat_db                      # Database name (created automatically)
```

### 2. PostgreSQL Setup Examples

#### Local PostgreSQL (Development)
```env
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASS=your-password
DB_DB=siglat_db
```

#### Docker Host PostgreSQL
```env
DB_HOST=host.docker.internal
DB_PORT=5432
DB_USER=postgres
DB_PASS=your-password
DB_DB=siglat_db
```

#### External PostgreSQL Server
```env
DB_HOST=postgres.yourcompany.com
DB_PORT=5432
DB_USER=siglat_user
DB_PASS=secure-password
DB_DB=siglat_production
```

### 3. PostgreSQL User Permissions

The API user needs the following permissions:
```sql
-- Create a dedicated user for SIGLAT API
CREATE USER siglat_api WITH PASSWORD 'secure-password';

-- Grant database creation privileges
ALTER USER siglat_api CREATEDB;

-- Grant connect privileges
GRANT CONNECT ON DATABASE postgres TO siglat_api;

-- After database creation, grant schema privileges
GRANT ALL PRIVILEGES ON DATABASE siglat_db TO siglat_api;
GRANT ALL PRIVILEGES ON SCHEMA public TO siglat_api;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO siglat_api;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO siglat_api;
```

## Deployment

### Using Docker Compose

The `docker-compose.yml` is configured for external PostgreSQL:

```bash
# Start the API (connects to external PostgreSQL)
docker-compose up -d

# View logs
docker-compose logs -f siglat-api

# Stop the API
docker-compose down
```

### Using Docker Run

```bash
# Build the image
docker build -t siglat-api .

# Run with external PostgreSQL
docker run -d \
  --name siglat-api \
  -p 5000:80 \
  --env-file .env \
  siglat-api
```

## Database Migration

The API automatically:
1. **Creates the database** if it doesn't exist
2. **Runs migrations** on startup using Entity Framework
3. **Seeds initial data** (roles, etc.)

No manual database setup is required - just ensure PostgreSQL is accessible with the configured credentials.

## Troubleshooting

### Connection Issues
- Verify PostgreSQL is running and accessible
- Check firewall rules allow connections on the configured port
- Ensure PostgreSQL `postgresql.conf` allows external connections
- Verify `pg_hba.conf` allows authentication from API host

### Permission Issues
- Ensure the database user has `CREATEDB` privileges
- Verify the user can connect to PostgreSQL
- Check that the user has appropriate schema permissions

### Common PostgreSQL Configuration
```bash
# Allow external connections (postgresql.conf)
listen_addresses = '*'

# Allow API authentication (pg_hba.conf)
host    all             siglat_api      0.0.0.0/0               md5
```

## Monitoring

The API includes health checks that verify PostgreSQL connectivity:
- Health endpoint: `http://localhost:5000/health`
- Docker health checks run automatically
- Monitor logs for database connection status