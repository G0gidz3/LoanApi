using FluentValidation;
using FluentValidation.AspNetCore;
using Loan.Api.Middleware;
using Loan.Application.Models;
using Loan.Application.Repositories.Abstraction;
using Loan.Application.Services.Abstraction;
using Loan.Application.Services.Implementation;
using Loan.Persistence;
using Loan.Persistence.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog ის კონფიგურაცია
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt",
        rollingInterval: RollingInterval.Day,     // Creates new log file for every day
        retainedFileCountLimit: 30,               // Optional: keep last 30 days of logs
        shared: true)                             // Allow multi-thread writing
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ბაზასთან კავშირის რეგისტრაცია
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
       .EnableSensitiveDataLogging()
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// ავტორიზაციის ტოკენის გენერაციისთვის სეკრეტის რეგისტრაცია
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// ზემოთ რეგისტრირებული სეკრეტის მიხედვით ტოკენის აუთენთიკაციის კონფიგურაცია
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
       .AddJwtBearer(x =>
       {
           x.RequireHttpsMetadata = false;
           x.SaveToken = true;
           x.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(key),
               ValidateIssuer = false,
               ValidateAudience = false
           };
       });

// სერვისების და რეპოზიტორების რეგისტრაცია
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// FluentValidator -ის კონფიგურაცია
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
// სვაგერის კონფიგურაცია
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
        options.RoutePrefix = "swagger";
    });
}

// თითოეული რექუესტის ლოგირებისთვის
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// ექსფენშენის ლოგირების მიდლვეარი
app.UseMiddleware<ExceptionLoggingMiddleware>();

// ავტორიზაცია / აუთენთიკაციისთვის
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
