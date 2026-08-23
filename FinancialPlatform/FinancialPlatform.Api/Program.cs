using FinancialPlatform.Infrastructure.Data;
using FinancialPlatform.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;

using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

string mongoDbConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
builder.Services.AddSingleton<IMongoClient> (new MongoClient(mongoDbConnectionString));

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
