using Craftmatrix.org.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Craftmatrix.org.API.Middleware;
using Craftmatrix.org.API.Services;

DotEnv.Load();
var Origin = "_Origin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add model validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddSingleton<IPostgreService, PostgreService>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();

builder.Services.AddDbContext<AppDBContext>((serviceProvider, options) =>
{
    var postgreService = serviceProvider.GetRequiredService<IPostgreService>();
    options.UseNpgsql(postgreService.GetConnectionString());
});

builder.Services.AddHostedService<DatabaseInitializer>();
builder.Services.AddCors(options =>
{
    var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS");
    var allowedOrigins = !string.IsNullOrEmpty(corsOrigins) 
        ? corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
        : new[] { "http://localhost:2424", "http://localhost:2425", "http://localhost:2426" };

    options.AddPolicy(name: Origin,
                      policy =>
                      {
                           policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                      });
});

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIGLAT API - Emergency Response System",
        Version = "v1.0",
        Description = "Comprehensive API for emergency response management including user authentication, incident reporting, real-time communication, and multi-agency coordination.",
        Contact = new OpenApiContact
        {
            Name = "Craftmatrix24",
            Email = "support@craftmatrix.org",
            Url = new Uri("https://github.com/Craftmatrix24")
        },
        License = new OpenApiLicense
        {
            Name = "ISC License",
            Url = new Uri("https://opensource.org/licenses/ISC")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "Bearer",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        new string[]{}
    }});

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Enable annotations for better documentation
    // c.EnableAnnotations();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure DateTime serialization to use ISO 8601 format
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Use default DateTime format which is ISO 8601
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep property names as-is
    });
builder.Services.AddHttpClient();

var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Add error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors(Origin);
// Configure the HTTP request pipeline.
app.UseSwagger();
// app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
