var builder = WebApplication.CreateBuilder(args);
// Add services (e.g., controllers, CORS)
builder.Services.AddControllers();

// allow your frontend to call APIs (adjust origins for production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://localhost:5000") // dev ports + your frontend
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowLocalDev");

// Serve static files from wwwroot
app.UseDefaultFiles();    // serves index.html by default
app.UseStaticFiles();

app.MapControllers(); // API controllers

// If you have a SPA with client routing, fallback to index.html
app.MapFallbackToFile("index.html");

app.Run();