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
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
});

// Configurar autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_here_change";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ContaCorrente";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ContaCorrente";

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

// Log para debug
Console.WriteLine("Kafka services registrados!");

// Cache
builder.Services.AddMemoryCache();


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