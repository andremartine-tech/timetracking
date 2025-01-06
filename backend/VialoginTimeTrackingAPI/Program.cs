using System.Text;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Filters;
using Application.Interfaces;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Mappings;
using RabbitMQ.Client;
using Application.UseCases;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging;
using VialoginTestTimeTrackingAPI.Application.Interfaces;
using VialoginTestTimeTrackingAPI.Infrastructure.BackgroundJobs;
using System.Threading.RateLimiting;
using VialoginTimeTrackingAPI.WebAPI.Middlewares;
using VialoginTimeTrackingAPI.Core.Interfaces;
using VialoginTimeTrackingAPI.Infrastructure.Repositories.Infrastructure.Repositories;
using VialoginTimeTrackingAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using VialoginTimeTrackingAPI.Infrastructure.Persistence.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Memory Cache
builder.Services.AddMemoryCache();

// Configuração do Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // Porta HTTP
    //options.ListenAnyIP(5001, listenOptions =>
    //{
    //    listenOptions.UseHttps(); // Porta HTTPS
    //});
});

// Limitação de login em 5 por minuto para evitar força bruta
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginLimiter", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Configuração do DbContext para PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// Configuração da autenticação JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vialogin Test TimeLog API",
        Version = "v1",
        Description = "API para gerenciamento de usuários e registros de ponto.",
        Contact = new OpenApiContact
        {
            Name = "Suporte da TimeLog",
            Email = "suporte@timelog.com",
            Url = new Uri("https://timelog.com")
        }
    });
    
    // Configurações para Autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato 'Bearer {seu token}'"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSingleton<IHostedService>(provider =>
{
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMqService = new RabbitMqService(connectionFactory);
    return new AddTimeLogWorker(serviceProvider, rabbitMqService, queueName: "add_timelog_queue", consumerCount: 5);
});

builder.Services.AddSingleton<IHostedService>(provider =>
{
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMqService = new RabbitMqService(connectionFactory);
    return new AddUserWorker(serviceProvider, rabbitMqService, queueName: "add_user_queue", consumerCount: 5);
});

builder.Services.AddSingleton<IHostedService>(provider =>
{
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMqService = new RabbitMqService(connectionFactory);
    return new UpdateTimeLogWorker(serviceProvider, rabbitMqService, queueName: "update_timelog_queue", consumerCount: 5);
});

builder.Services.AddSingleton<IHostedService>(provider =>
{
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMqService = new RabbitMqService(connectionFactory);
    return new DeleteTimeLogWorker(serviceProvider, rabbitMqService, queueName: "delete_timelog_queue", consumerCount: 5);
});

// Adiciona ConnectionFactory (para RabbitMQ ao container de serviços)
builder.Services.AddSingleton<IConnectionFactory>(_ =>
    new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMq:Hostname"],
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        RequestedConnectionTimeout = TimeSpan.FromSeconds(30), // Aumenta o timeout
        SocketReadTimeout = TimeSpan.FromSeconds(30),          // Timeout de leitura
        SocketWriteTimeout = TimeSpan.FromSeconds(30)          // Timeout de escrita
    }); 

builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<RabbitMqInitializer>();

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<BackgroundTaskWorker>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<IProcessMessageAddTimeLogUseCase, ProcessMessageAddTimeLogUseCase>();
builder.Services.AddScoped<IProcessMessageAddUserUseCase, ProcessMessageAddUserUseCase>();
builder.Services.AddScoped<IProcessMessageUpdateTimeLogUseCase, ProcessMessageUpdateTimeLogUseCase>();
builder.Services.AddScoped<IProcessMessageDeleteTimeLogUseCase, ProcessMessageDeleteTimeLogUseCase>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRevokedTokensRepository, RevokedTokensRepository>();
builder.Services.AddScoped<IDisposable, RabbitMqService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITimeLogRepository, TimeLogRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
builder.Services.AddScoped<ITimeLogService, TimeLogService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly));
}

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ExceptionFilter>();
});

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permite qualquer origem
              .AllowAnyMethod() // Permite qualquer método HTTP
              .AllowAnyHeader(); // Permite qualquer cabeçalho
    });

    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Inicializa o RabbitMQ
var rabbitMqInitializer = app.Services.GetRequiredService<RabbitMqInitializer>();
await rabbitMqInitializer.InitializeAsync("add_timelog_queue");
await rabbitMqInitializer.InitializeAsync("add_user_queue");
await rabbitMqInitializer.InitializeAsync("update_timelog_queue");
await rabbitMqInitializer.InitializeAsync("delete_timelog_queue");

// Middleware de CORS
app.UseCors("AllowAll");

app.Services.GetService<IServiceProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeLog API v1");
        options.RoutePrefix = string.Empty; // Abre diretamente em http://localhost:5000/
    });
}

// Seeders
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }