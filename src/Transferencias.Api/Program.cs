using Transferencias.Api.Controllers;
using Transferencias.Application.Handlers;
using Transferencias.Application.Services;
using Transferencias.Infrastructure.Data;
using Transferencias.Infrastructure.Messaging;
using Transferencias.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();

// Application Services
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();
builder.Services.AddScoped<IContaCorrenteService, ContaCorrenteService>();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(TransferirEntreContasHandler).Assembly);
});

// HTTP Client
builder.Services.AddHttpClient<IContaCorrenteService, ContaCorrenteService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ContaCorrenteApi:BaseUrl"] ?? "http://localhost:5009");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Kafka
builder.Services.AddScoped<IKafkaMessageProducer, KafkaMessageProducer>();

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
