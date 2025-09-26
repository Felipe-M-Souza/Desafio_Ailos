using ContaCorrente.Application.Behaviors;
using ContaCorrente.Application.Handlers;
using ContaCorrente.Infrastructure.Data;
using ContaCorrente.Infrastructure.Repositories;
using ContaCorrente.Domain.Interfaces;
using ContaCorrente.Tarifas.Api.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddMediatR(typeof(CriarTarifaHandler).Assembly);

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

// Repositórios
builder.Services.AddScoped<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddScoped<ITarifaRepository, TarifaRepository>();
builder.Services.AddScoped<ITarifaCobradaRepository, TarifaCobradaRepository>();
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// Serviços
builder.Services.AddScoped<ContaCorrente.Domain.Interfaces.ITarifaService, TarifaService>();

// Cache
builder.Services.AddMemoryCache();

// KafkaFlow
builder.Services.AddKafkaFlow(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = new DbInitializer(scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>());
    dbInitializer.Initialize();
}

app.Run();

