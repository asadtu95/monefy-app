using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Monefy.Api.Data;
using Monefy.Api.Data.Seed;
using Monefy.Api.Extensions;
using Monefy.Api.Middleware;
using Monefy.Api.Services;
using Monefy.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Lets you paste a Supabase access token into Swagger's Authorize box.
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the Supabase access token (without the 'Bearer ' prefix)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Enums travel as readable strings so the Angular string enums line up.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddPersistence(builder.Configuration, builder.Environment);
builder.Services.AddFrontendCors(builder.Configuration);
builder.Services.AddSupabaseAuth(builder.Configuration);

// Base address is the auth root; the client appends "admin/users".
builder.Services.AddHttpClient<ISupabaseAdminClient, SupabaseAdminClient>(client =>
{
    var issuer = builder.Configuration["Supabase:Issuer"]
        ?? throw new InvalidOperationException("Supabase:Issuer is not configured.");

    client.BaseAddress = new Uri(issuer.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors(CorsExtensions.FrontendPolicy);

app.UseAuthentication();
// Runs after authentication so the caller's profile row exists before any handler.
app.UseMiddleware<UserProvisioningMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Connectivity probe — confirms the API can actually reach Supabase.
app.MapGet("/health/db", async (AppDbContext db, IWebHostEnvironment env) =>
{
    try
    {
        await db.Database.OpenConnectionAsync();
        var connection = db.Database.GetDbConnection();
        var serverVersion = connection.ServerVersion;
        await db.Database.CloseConnectionAsync();

        return Results.Ok(new
        {
            status = "healthy",
            database = connection.Database,
            server = serverVersion
        });
    }
    catch (Exception ex)
    {
        // Only leak the driver message in development — it can echo the host and user.
        var detail = env.IsDevelopment()
            ? $"{ex.GetType().Name}: {ex.Message} | inner: {ex.InnerException?.Message}"
            : "Cannot connect to the database.";

        return Results.Problem(detail, statusCode: 503);
    }
}).AllowAnonymous();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedAsync(db);
}

app.Run();
