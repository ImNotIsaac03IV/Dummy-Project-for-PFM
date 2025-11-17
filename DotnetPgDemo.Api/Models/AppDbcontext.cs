using System;
using Microsoft.EntityFrameworkCore;

namespace DotnetPgDemo.Api.Models;

public class AppDbcontext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options)
    {
        
    }
    public DbSet<Person> People { get; set; }

}
