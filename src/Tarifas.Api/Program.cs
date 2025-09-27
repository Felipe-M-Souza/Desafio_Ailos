using Tarifas.Api.Controllers;
using Tarifas.Application.Handlers;
using Tarifas.Application.Services;
using Tarifas.Infrastructure.Data;
using Tarifas.Infrastructure.Messaging;
using Tarifas.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ITarifaRepository, TarifaRepository>();
builder.Services.AddScoped<ITarifaCobradaRepository, TarifaCobradaRepository>();

// Application Services
builder.Services.AddScoped<ITarifaService, TarifaService>();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ProcessarTarifaHandler).Assembly);
});

// Kafka
builder.Services.AddScoped<IKafkaMessageProducer, KafkaMessageProducer>();
builder.Services.AddHostedService<TransferenciaConsumerService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = new DbInitializer(app.Configuration, scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>());
    await dbInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
