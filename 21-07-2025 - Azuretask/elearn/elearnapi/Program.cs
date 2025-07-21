using ElearnApi.Models;
using ElearnApi.Context;
using ElearnApi.Repositories;
using ElearnApi.Interface;
using ElearnApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Database Configuration
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});
#endregion

builder.Services.AddScoped<ManageVideoRepository>();
builder.Services.AddScoped<IVideoService, VideoService>();

builder.Services.AddHttpContextAccessor();

#region CORS Configuration
// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhostFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

