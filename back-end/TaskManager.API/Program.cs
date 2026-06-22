using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.API.Authentication;
using TaskManager.API.Data;
using TaskManager.API.Handlers;
using TaskManager.API.Middleware;
using TaskManager.API.Repositories;
using TaskManager.API.Repositories.Contracts;
using TaskManager.API.Services;
using TaskManager.API.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        BasicAuthenticationHandler.SchemeName, null);
builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:4200"])
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Return structured validation errors instead of default ProblemDetails
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                success = false,
                message = "Validation failed.",
                errors
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1",
        Description = "RESTful API for managing tasks with Basic Authentication.",
    });

    // Add Basic Auth input to Swagger UI
    options.AddSecurityDefinition("BasicAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Enter username and password (default: admin / Admin@123)",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "BasicAuth" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<RequestCancellationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger at root "/"
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
