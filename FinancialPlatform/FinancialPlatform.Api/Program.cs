using FinancialPlatform.Application.Behaviors;
using FinancialPlatform.Application.Commands;
using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Application.Queries;
using FinancialPlatform.Application.Validators;
using FinancialPlatform.Infrastructure.BackgroundServices;
using FinancialPlatform.Infrastructure.Clients;
using FinancialPlatform.Infrastructure.Data;
using FinancialPlatform.Infrastructure.Settings;

using FluentValidation;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using MongoDB.Driver;

using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

//PostgreSQL Database Context Configuration
builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

string mongoDbConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Value;

builder.Services.AddSingleton<IMongoClient> (new MongoClient(mongoDbConnectionString));

builder.Services.AddSingleton<IMarketDataRepository, MongoMarketDataRepository>();

// External API Settings Configuration
builder.Services.Configure<ExternalApiSettings>(
    builder.Configuration.GetSection("ExternalApiSettings"));

// Register the FinnhubClient as a service
builder.Services.AddHttpClient<FinnhubClient>().
    AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
        3, configurePolicy => TimeSpan.FromSeconds(Math.Pow(2, configurePolicy))));

builder.Services.AddHttpClient<AlphaVantageClient>().
    AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
        3, configurePolicy => TimeSpan.FromSeconds(Math.Pow(2, configurePolicy))));

builder.Services.AddTransient<IMarketDataProvider>(sp =>
{
    var finnhubClient = sp.GetRequiredService<FinnhubClient>();
    var alphaVantageClient = sp.GetRequiredService<AlphaVantageClient>();
    var logger = sp.GetRequiredService<ILogger<FallbackDataProvider>>();

    var fallbackProvider = new FallbackDataProvider(finnhubClient, alphaVantageClient, logger);

    var distributedCache = sp.GetRequiredService<IDistributedCache>();

    return new CachedMarketDataProvider(fallbackProvider, distributedCache);
});

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => 
        {
            host.Username(builder.Configuration["RabbitMq:login"]);
            host.Password(builder.Configuration["RabbitMq:password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(SaveMarketDataCommand).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(typeof(SaveMarketDataCommandValidator).Assembly);

// Add background service for market data collection
builder.Services.AddHostedService<MarketDataCollectorService>();

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
