using BTPayPro; // ton projet principal (qui contient DbContext & services)
using BTPayPro.Api.Models;
using BTPayPro.Autmpay.Parsers;
using BTPayPro.Autmpay.Reports;
using BTPayPro.Autmpay.Services;
using BTPayPro.CPMPay.Models;
using BTPayPro.CPMPay.Reports;
using BTPayPro.CPMPay.Services;
using BTPayPro.data;
using BTPayPro.Domaine;
using BTPayPro.Interfaces;
using BTPayPro.Security;
using BTPayPro.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// 🔹 Connexion EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<AutmpayFileParser>(
    builder.Configuration.GetSection("FileParserSettings"));
builder.Services.Configure<CPMPayParser>(
    builder.Configuration.GetSection("FileParserSettings"));

//builder.Services.AddDbContext<AppDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("btpayprodb")));
Console.WriteLine("JWT Key from config: " + builder.Configuration["Jwt:Key"]);

// 🔹 Ajouter les services applicatifs (par ex. JWT, PasswordHasher)
builder.Services.AddScoped<BTPayPro.Services.ITokenService, BTPayPro.Services.TokenService>();       // JWT service
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<AutmpayFileParser>();
builder.Services.AddScoped<HeaderRecordParser>();
builder.Services.AddScoped<DetailRecordParser>();
builder.Services.AddScoped<TrailerRecordParser>();
builder.Services.AddScoped<AccountingReportGenerator>();
builder.Services.AddScoped<CPMPayParser>();
builder.Services.AddScoped<FileHeaderRecord>();
builder.Services.AddScoped<FileDetailRecord>();
builder.Services.AddScoped<FileTrailerRecord>();
builder.Services.AddScoped<CPMPayReportGenerator>();
// 1. Configure Clictopay Settings
builder.Services.Configure<ClictopaySettings>(builder.Configuration.GetSection(ClictopaySettings.SectionName));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 2. Register Simulated Services (Replace with real DB/EF Core services in production)
builder.Services.AddScoped(typeof(IRepositories<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletServices, WalletService>();
builder.Services.AddScoped<ITransactionCrudService, TransactionCrudService>();
builder.Services.AddScoped<IUserCrudService, UserCrudService>();
builder.Services.AddScoped<IWalletCrudService, WalletCrudService>();



// 3. Register Clictopay HTTP Client and Client Implementation
builder.Services.AddHttpClient<IPaymentGatewayClient, ClictopayClient>();

// 4. Register Payment Service
builder.Services.AddScoped<IPaymentService, PaymentService>();


// 🔹 Authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5029", "https://localhost:7086")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    // Disable HTTPS redirect in development to avoid CORS preflight issues
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

