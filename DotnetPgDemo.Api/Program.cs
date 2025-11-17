using DotnetPgDemo.Api.Models;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//registering out AppDbcontext in the DI container
//Is it registered as a scoped lifetime
string connectionsString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbcontext>(options => options.UseNpgsql(connectionsString));

var app = builder.Build();

app.MapControllers();

app.Run();