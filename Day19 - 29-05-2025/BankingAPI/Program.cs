using Microsoft.EntityFrameworkCore;
using BankingAPI.Contexts;
using BankingAPI.Models;
using BankingAPI.Interfaces;
using BankingAPI.Models.DTOs.Banking;
using BankingAPI.Repositories;
using BankingAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });


builder.Services.AddDbContext<BankingContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<IRepository<int, CustomerModel>, CustomerRepository>();
builder.Services.AddTransient<IRepository<int, AccountModel>, AccountRepository>();
builder.Services.AddTransient<ICustomerServices, CustomerService>();
builder.Services.AddTransient<IAccountServices, AccountService>();
builder.Services.AddTransient<IAccountServices, AccountDetailsServiceTransaction>(); // it will run the transaction service which works with stored procedures
builder.Services.AddTransient<IRepository<int, TransactionModel>, TransactionRepository>();
builder.Services.AddTransient<ITransactionServices, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
