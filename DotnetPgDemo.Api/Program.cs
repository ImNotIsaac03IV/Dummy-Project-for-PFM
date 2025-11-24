using System.Threading;
using DotnetPgDemo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//registering out AppDbcontext in the DI container
//Is it registered as a scoped lifetime
string connectionsString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbcontext>(options => options.UseNpgsql(connectionsString));

var app = builder.Build();

// apply pending migrations on startup with retry
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<DotnetPgDemo.Api.Models.AppDbcontext>();

    var attempts = 0;
    var maxAttempts = 10;
    while (true)
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied.");
            break;
        }
        catch (Exception ex)
        {
            attempts++;
            logger.LogWarning(ex, "Failed to migrate DB (attempt {Attempt}/{Max}). Retrying in 3s...", attempts, maxAttempts);
            if (attempts >= maxAttempts) throw;
            Thread.Sleep(3000);
        }
    }
}

app.MapControllers();
app.Run();