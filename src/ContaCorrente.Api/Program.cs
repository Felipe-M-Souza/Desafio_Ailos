using ContaCorrente.Application.Behaviors;
using ContaCorrente.Application.Handlers;
using ContaCorrente.Application.Services;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Infrastructure.Data;
using ContaCorrente.Infrastructure.Messaging;
using ContaCorrente.Infrastructure.Repositories;
using ContaCorrente.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conta Corrente API",
        Version = "v1",
        Description = "API para gerenciamento de conta corrente com operações de movimentação financeira",
        Contact = new OpenApiContact
        {
            Name = "Desenvolvedor",
            Email = "dev@example.com"
        }
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Habilitar annotations do Swagger
    c.EnableAnnotations();
});

// Configurar autenticação JWT
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT12345678901234567890";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BankMore";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BankMoreUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Configurar MediatR
builder.Services.AddMediatR(typeof(CriarContaHandler).Assembly);

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

// Repositórios
builder.Services.AddScoped<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
builder.Services.AddScoped<ITarifaRepository, TarifaRepository>();
builder.Services.AddScoped<ITarifaCobradaRepository, TarifaCobradaRepository>();

// Serviços
builder.Services.AddScoped<ContaCorrente.Domain.Interfaces.ITarifaService, TarifaService>();

// Kafka
builder.Services.AddSingleton<IMessageProducer, KafkaMessageProducer>();
builder.Services.AddScoped<IEventPublisher, KafkaEventPublisher>();

// Cache Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "BankMore";
});

// Memory Cache
builder.Services.AddMemoryCache();

// Serviços de Cache
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlite(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379")
    .AddKafka(options =>
    {
        options.BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
    });

// Log para debug
Console.WriteLine("Kafka services registrados!");
Console.WriteLine("Redis cache configurado!");
Console.WriteLine("Health checks configurados!");


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conta Corrente API v1");
    c.RoutePrefix = "swagger"; // Swagger UI em /swagger
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
    c.EnableValidator();
});
}

// Servir arquivos estáticos da interface web
app.UseStaticFiles();

// Configurar rota padrão para servir index.html na raiz
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                exception = entry.Value.Exception?.Message,
                duration = entry.Value.Duration.ToString()
            }),
            duration = report.TotalDuration.ToString()
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Configurar Prometheus
app.UseHttpMetrics();
app.UseRouting();
app.UseAuthorization();
app.MapMetrics();

app.MapControllers();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = new DbInitializer(
        scope.ServiceProvider.GetRequiredService<IConfiguration>(),
        scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>());
    
    await dbInitializer.InitializeAsync();
}


app.Run();

// Tornar Program acessível para testes
public partial class Program { }