using Microsoft.EntityFrameworkCore;
using AuctionService.Infrastructure.Persistence;
using AuctionService.Infrastructure.Persistence.Repositories;
using AuctionService.Infrastructure.Caching;
using AuctionService.Domain.Repositories;
using AuctionService.Application.Common.Interfaces;
using AuctionService.Application.Features.Auctions.Commands.CreateAuction;
using AuctionService.Infrastructure.BackgroundJobs;
using AuctionService.Infrastructure.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
});

builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuctionCacheService, AuctionCacheService>();
builder.Services.AddHostedService<AuctionOutboxPublisherBackgroundService>();
builder.Services.AddHostedService<AuctionBidConsumerBackgroundService>();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateAuctionCommand).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();