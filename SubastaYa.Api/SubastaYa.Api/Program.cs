using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Api.Hubs;
using SubastaYa.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<AuctionClosingWorker>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAuctionNotifier, SignalRAuctionNotifier>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SubastaYa.Api.Swagger.UserIdHeaderFilter>();
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      
    app.UseSwaggerUI();    
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auctions");

app.Run();
