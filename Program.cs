using Microsoft.EntityFrameworkCore;
using Serilog;
using Shortly.Infrastructure.Persistence;
using Shortly.Infrastructure.Seed;

// The main entry point of the application, responsible for configuring and running the web application.

// 1. Create a web application builder with the provided command-line arguments
var builder = WebApplication.CreateBuilder(args);

// Add web services to the builder.
builder.Services.AddRazorPages();

// Add database context with SQLite provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext") ??
        throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

// Add Serilog
builder.Host.UseSerilog((hostingContext, services, configuration) =>
{
    configuration.ReadFrom.Configuration(hostingContext.Configuration);
});

// 2. Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

// 3. Create a scope to access the services for initialization tasks.
using (var scope = app.Services.CreateScope())
{
    // Get the required services for database initialization
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Get the logger factory to create loggers for the seeding process
    var logFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

    // Create a logger for the Program class to log the seeding process
    var log = logFactory.CreateLogger<Program>();

    log.LogDebug("Initializing ..");

    // Initialize the system by applying migrations and seeding the database with default data.
    await DbInitializer.InitializeAsync(dbContext, log);

    log.LogDebug("Initializing ok.");
}

// Run the web application
app.Run();