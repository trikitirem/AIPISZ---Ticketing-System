using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TicketingSystem.Application.Mappers;
using TicketingSystem.Application.Services;
using TicketingSystem.Application.Validators;
using TicketingSystem.Domain.Policies;
using TicketingSystem.Infrastructure.Middleware;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure routes to be lowercase
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ticketing System API",
        Version = "v1",
        Description = "API do zarządzania zgłoszeniami w systemie ticketingowym"
    });
    
    // Handle file uploads in Swagger - must be configured before other filters
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
    
    c.SchemaFilter<FileUploadSchemaFilter>();
    c.ParameterFilter<FileUploadParameterFilter>();
    c.OperationFilter<FileUploadOperationFilter>();
    
    // Handle enum values in Swagger
    c.SchemaFilter<EnumSchemaFilter>();
    c.DocumentFilter<EnumDocumentFilter>();
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Repositories
builder.Services.AddSingleton<ILogger<TicketRepository>>(sp =>
    sp.GetRequiredService<ILoggerFactory>().CreateLogger<TicketRepository>());
builder.Services.AddSingleton<TicketRepository>();

builder.Services.AddSingleton<ILogger<UserRepository>>(sp =>
    sp.GetRequiredService<ILoggerFactory>().CreateLogger<UserRepository>());
builder.Services.AddSingleton<UserRepository>();

builder.Services.AddSingleton<ILogger<TeamRepository>>(sp =>
    sp.GetRequiredService<ILoggerFactory>().CreateLogger<TeamRepository>());
builder.Services.AddSingleton<TeamRepository>();

builder.Services.AddSingleton<ILogger<AttachmentRepository>>(sp =>
    sp.GetRequiredService<ILoggerFactory>().CreateLogger<AttachmentRepository>());
builder.Services.AddSingleton<AttachmentRepository>();

// Policies
builder.Services.AddSingleton<ResolutionPolicy>();
builder.Services.AddSingleton<EscalationPolicy>();
builder.Services.AddSingleton<WorkerEscalationPolicy>();
builder.Services.AddSingleton<SpecialistResolutionPolicy>();
builder.Services.AddSingleton<TicketStatusPolicy>();

// Mappers
builder.Services.AddSingleton<TicketMapper>();
builder.Services.AddSingleton<UserMapper>();
builder.Services.AddSingleton<TeamMapper>();
builder.Services.AddSingleton<CommentMapper>();

// Services
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TeamService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTicketRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
