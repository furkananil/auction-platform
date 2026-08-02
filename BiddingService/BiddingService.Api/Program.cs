using Microsoft.EntityFrameworkCore;
using BiddingService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using BiddingService.Infrastructure.Persistence.Repositories;
using BiddingService.Domain.Repositories;
using BiddingService.Application.Common.Interfaces;
using BiddingService.Infrastructure.BackgroundJobs;
using BiddingService.Application.Features.Bids.Commands.PlaceBid;
using CatalogService.Api.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BiddingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHostedService<OutboxPublisherBackgroundService>();
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(PlaceBidCommand).Assembly));

builder.Services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>(o =>
    o.Address = new Uri(builder.Configuration["GrpcSettings:CatalogUrl"]!));

builder.Services.AddScoped<ICatalogGrpcClient, BiddingService.Infrastructure.Clients.CatalogGrpcClient>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BiddingDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();