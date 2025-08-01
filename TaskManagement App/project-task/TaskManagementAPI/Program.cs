using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using Serilog;
using TaskManagementAPI.CustomExceptions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using TaskManagementAPI.Interfaces.Auth;
using TaskManagementAPI.Services;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Mapper;
using TaskManagementAPI.Repositories.Auth;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TaskManagementAPI.Hubs;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;



var builder = WebApplication.CreateBuilder(args);

// var vaultUri = builder.Configuration["AzureKeyVault:VaultUri"];
// builder.Configuration.AddAzureKeyVault(
//     new Uri(vaultUri),
//     new DefaultAzureCredential()
// );

var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManagementAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.WriteIndented = true;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true; 
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// builder.Services.AddSingleton(x =>
// {
//     var config = builder.Configuration.GetSection("AzureBlob");
//     return new BlobServiceClient(config["ConnectionString"]);
// });



#region serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

builder.Services.AddSignalR();
#region  dbConnection
builder.Services.AddDbContext<TaskManagementDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// builder.Services.AddDbContext<TaskManagementDbContext>(opts =>
// {
//     opts.UseNpgsql(builder.Configuration["DbConnectionString"]);
// });
#endregion

#region jwttoken
var jwtSettings = configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(keyString))
    throw new InvalidOperationException("JWT Key is missing in configuration.");
var key = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

     options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/taskHub"))
            {
                Console.WriteLine("[JWT] Access token extracted from SignalR query string.");
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
#endregion

#region Repositories
builder.Services.AddScoped<IRepository<Guid, User>, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskFileRepository, TaskFileRepository>(); 
builder.Services.AddScoped<IRepository<Guid, TaskItem>, TaskItemRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ITaskStatusLogRepository, TaskStatusLogRepository>();

#endregion

#region services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskFileService, TaskFileService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskStatusLogService, TaskStatusLogService>();
//builder.Services.AddScoped<IFileBlobService, FileBlobService>();
builder.Services.AddHostedService<DraftExpiryBackgroundService>();

#endregion

#region mapper
builder.Services.AddSingleton<AuthMapper>();
builder.Services.AddSingleton<UserMapper>();
builder.Services.AddScoped<TaskItemMapper>();
builder.Services.AddScoped<TaskItemDraftMapper>();
builder.Services.AddScoped<TaskItemUpdateMapper>();

#endregion

#region cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins(
                  "http://127.0.0.1:8080",
                  "http://localhost:8080",
                  "http://localhost:4200"
              );
    });
});
#endregion

var app = builder.Build();

app.UseRouting();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.MapHub<TaskHub>("/taskHub");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
