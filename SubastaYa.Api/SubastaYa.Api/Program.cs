using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Api.Hubs;
using SubastaYa.Api.Workers;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "FrontendPolicy";
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<AuctionClosingWorker>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAuctionNotifier, SignalRAuctionNotifier>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",   // Vite (React/Vue)
                "http://localhost:3000",   // Create React App / Next
                "http://127.0.0.1:5500",   // Live Server (VS Code)
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auctions").RequireCors(CorsPolicy);

app.Run();
