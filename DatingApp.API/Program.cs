using DatingApp.API.Data;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAplicationService(builder.Configuration);
builder.Services.AddDependencyInjections();
builder.Services.AddAuthenticationService(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch(Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();

