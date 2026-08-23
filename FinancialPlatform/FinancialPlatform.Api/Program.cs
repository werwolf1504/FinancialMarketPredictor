using FinancialPlatform.Infrastructure.Data;
using FinancialPlatform.Infrastructure.Settings;
using FinancialPlatform.Infrastructure.Clients;

using Microsoft.EntityFrameworkCore;

using MongoDB.Driver;
using FinancialPlatform.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//PostgreSQL Database Context Configuration
builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

string mongoDbConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
builder.Services.AddSingleton<IMongoClient> (new MongoClient(mongoDbConnectionString));

// External API Settings Configuration
builder.Services.Configure<ExternalApiSettings>(
    builder.Configuration.GetSection("ExternalApiSettings"));

// Register the FinnhubClient as a service
builder.Services.AddHttpClient<IMarketDataProvider,FinnhubClient>();
builder.Services.AddHttpClient<IMarketDataProvider, AlphaVantageClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
