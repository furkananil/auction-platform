using Microsoft.EntityFrameworkCore;
using OrderingService.Infrastructure.Persistence;
using OrderingService.Infrastructure.Persistence.Repositories;
using OrderingService.Infrastructure.Clients;
using OrderingService.Application.Common.Interfaces;
using OrderingService.Infrastructure.BackgroundJobs;
using PaymentService.Api.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentGrpcClient, PaymentGrpcClient>();

builder.Services.AddGrpcClient<PaymentGrpc.PaymentGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcSettings:PaymentUrl"]!);
});

builder.Services.AddHostedService<AuctionClosedConsumerBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
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